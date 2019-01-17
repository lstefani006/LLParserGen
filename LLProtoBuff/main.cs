using LLParserLexerLib;
using PB;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using LLProtoBuff;
using System.Threading.Tasks;
using PBUtils;

namespace PB
{

	public partial class MParser
	{
		public MParser() : base(0) { }

		public DeclList Start(LexReader rd)
		{
			this.init(rd);
			var v = this.start(null);
			return v;
		}
	}
}

namespace PBUtils
{
	public class EE
	{
		public enum WireType
		{
			Varint = 0,          /* int32, int64, uint32, uint64, sint32, sint64, bool, enum */
			Bit64 = 1,           /* fixed64, sfixed64, double */
			LengthDelimited = 2, /* string, bytes, embedded messages, packed repeated fields */
			Bit32 = 5            /* fixed32, sfixed32, float */
		}


		public enum PbType
		{
			INT32 = 1,
			INT64 = 2,
			UINT32 = 3,
			UINT64 = 4,
			SINT32 = 5,
			SINT64 = 6,
			BOOL = 7,
			ENUM = 8,
			FIXED64 = 9,
			SFIXED64 = 10,
			DOUBLE = 11,
			STRING = 12,
			BYTES = 13,
			FIXED32 = 14,
			SFIXED32 = 15,
			FLOAT = 16,
			embeddedMessage = 17, /* un oggetto */
			packedRepeated = 32   /* può essere uno dei tipi sopra eccetto string/bytes/embeddedMessage */
		}

		static int write_varint(Stream s, long value) => write_varint(s, (ulong)value);

		static int write_varint(Stream s, ulong value)
		{
			if (s == null)
			{
				int ret = 0;
				while (value > 127)
				{
					ret++;
					value >>= 7;
				}
				ret++;
				return ret;
			}
			else
			{
				int ret = 0;
				while (value > 127)
				{
					var by = (byte)((value & 127) | 128);
					s.WriteByte(by);
					ret++;
					value >>= 7;
				}
				s.WriteByte((byte)(value & 127));
				ret++;
				return ret;
			}
		}

		static ulong zz64(ulong n) { return (n << 1) ^ (n >> 63); }
		static ulong zz64(long n) => zz64((ulong)n);

		public static int write(Stream s, int fieldNumber, PbType type, long n) => write(s, fieldNumber, type, (ulong)n);
		public static int write(Stream s, int fieldNumber, PbType type, ulong n)
		{
			int ret = 0;
			if (n == 0) return ret;

			switch (type)
			{
			case PbType.INT32:
			case PbType.INT64:
			case PbType.UINT32:
			case PbType.UINT64:
			case PbType.BOOL:
			case PbType.ENUM:
				{
					var aa = (fieldNumber << 3) | (int)WireType.Varint;
					ret += write_varint(s, (ulong)aa);
					ret += write_varint(s, n);
				}
				break;

			case PbType.SINT32:
			case PbType.SINT64:
				{
					var aa = (fieldNumber << 3) | (int)WireType.Varint;
					ret += write_varint(s, (ulong)aa);
					ret += write_varint(s, zz64(n));
				}
				break;

			case PbType.FIXED32:
			case PbType.SFIXED32:
			/* TODO */
			case PbType.FIXED64:
			case PbType.SFIXED64:
			/* TODO */
			case PbType.FLOAT:
			case PbType.DOUBLE:
			/* TODO */

			case PbType.STRING:
			case PbType.BYTES:
			case PbType.packedRepeated:
			case PbType.embeddedMessage:
			default:
				throw new ArgumentException();
			}

			return ret;
		}

		public static int write(Stream s, int fieldNumber, byte[] b)
		{
			int ret = 0;
			if (b == null || b.Length == 0) return ret;
			var aa = (fieldNumber << 3) | (int)WireType.LengthDelimited;
			ret += write_varint(s, aa);
			ret += write_varint(s, b.Length);
			s.Write(b, 0, b.Length);
			ret += b.Length;
			return ret;
		}
		public static int write(Stream s, int fieldNumber, string v)
		{
			int ret = 0;
			if (string.IsNullOrEmpty(v)) return ret;
			var b = Encoding.UTF8.GetBytes(v);
			ret += write(s, fieldNumber, b);
			return ret;
		}

		public static int write(Stream s, int fieldNumber, bool sint, int[] b)
		{
			int ret = 0;
			if (b == null || b.Length == 0)
				return ret;

			int sz = 0;
			for (int i = 0; i < b.Length; i++)
				if (sint)
					sz += write_varint(null, zz64(b[i]));
				else
					sz += write_varint(null, b[i]);

			var aa = (fieldNumber << 3) | (int)WireType.LengthDelimited;
			ret += write_varint(s, aa);
			ret += write_varint(s, sz);

			for (int i = 0; i < b.Length; i++)
				if (sint)
					ret += write_varint(s, zz64(b[i]));
				else
					ret += write_varint(s, b[i]);

			return ret;
		}

		public static int write(Stream s, int fieldNumber, PbObject ww)
		{
			int ret = 0;
			if (ww == null) return ret;
			int sz = ww.PbWrite(null);
			var aa = (fieldNumber << 3) | (int)WireType.LengthDelimited;
			ret += write_varint(s, aa);
			ret += write_varint(s, sz);
			ret += ww.PbWrite(s);
			return ret;
		}
		public static int write(Stream s, int fieldNumber, PbObject[] ww)
		{
			int ret = 0;
			if (ww == null) return ret;
			for (int i = 0; i < ww.Length; ++i)
				ret += write(s, fieldNumber, ww[i]);

			return ret;
		}

		static async Task<Tuple<bool, ulong>> readVarintAsync(Stream s, bool allowEof, CancellationToken ct)
		{
			var b = new byte[1];
			ulong ret = 0;
			int r = await s.ReadAsync(b, 0, 1, ct);
			if (r <= 0)
				return Tuple.Create(true, 0ul);

			for (int i = 0; ; i += 7)
			{
				if (r != 1) throw new EndOfStreamException();
				ulong by = b[0];
				ret |= (by & 127) << i;
				if ((by & 128) == 0)
					return Tuple.Create(false, ret);
				r = await s.ReadAsync(b, 0, 1, ct);
			}
		}

		public struct TagData
		{
			public int fieldNumber;
			public int sz;
			public WireType wireType;
			public MemoryStream ms;
		}
		public static async Task<TagData> readTagAsync(Stream s, CancellationToken ct)
		{
			var ret = new TagData { fieldNumber = 0, sz = -1 };
			var r = await readVarintAsync(s, allowEof: true, ct: ct);
			if (r.Item1) return ret;

			ret.fieldNumber = (int)(r.Item2 >> 3);
			ret.wireType = (WireType)(r.Item2 & 7);
			switch (ret.wireType)
			{
			case WireType.Bit32: ret.sz = 4; break;
			case WireType.Bit64: ret.sz = 8; break;
			case WireType.Varint: ret.sz = -1; break;
			case WireType.LengthDelimited: ret.sz = (int)(await readVarintAsync(s, false, ct)).Item2; break;
			default: throw new InvalidDataException();
			}

			if (ret.sz > 0)
			{
				var buff = new byte[ret.sz];
				await s.ReadAsync(buff, 0, ret.sz, ct);
				ret.ms = new MemoryStream(buff);
			}
			else
			{
				ret.ms = new MemoryStream();
				for (;;)
				{
					int b = s.ReadByte();
					if (b < 0) throw new EndOfStreamException();
					ret.ms.WriteByte((byte)b);
					if ((b & 0x80) == 0) break;
				}
				ret.ms.Position = 0;
			}
			return ret;	
		}

		public static async Task<int> readAsyncInt(Stream s, PbType pbType, int sz, CancellationToken ct)
		{
			if (sz == -1)
				return (int)((await readVarintAsync(s, false, ct)).Item2);
			Debug.Assert(false);
			return 0;
		}

		public static async Task<string> readAsyncString(Stream s, PbType pbType, int sz, CancellationToken ct)
		{
			var b = new byte[sz];
			await s.ReadAsync(b, 0, sz, ct);
			return Encoding.UTF8.GetString(b);
		}
		public static async Task<T> readObjectAsync<T>(Stream s, PbType pbType, CancellationToken ct) where T : PbObject, new()
		{
			var r = new T();
			await r.PbReadAsync(s, ct);
			return r;
		}
	}

	public interface PbObject
	{
		int PbWrite(Stream s);
		Task PbReadAsync(Stream s, CancellationToken ct);
	}
	class Leo : PbObject
	{
		public int a;
		public string g;
		public Leo f;

		public async Task PbReadAsync(Stream s, CancellationToken ct)
		{
			a = 0;
			g = null;
			f = null;
			for (; ; )
			{
				var tk = await EE.readTagAsync(s, ct);
				if (tk.fieldNumber == 0) break;
				switch (tk.fieldNumber)
				{
				case 1:
					a = await EE.readAsyncInt(tk.ms, EE.PbType.INT32, tk.sz, ct);
					break;
				case 2:
					g = await EE.readAsyncString(tk.ms, EE.PbType.STRING, tk.sz, ct);
					break;
				case 3:
					f = await EE.readObjectAsync<Leo>(tk.ms, EE.PbType.embeddedMessage, ct);
					break;
				}
			}
		}
		public int PbWrite(Stream s)
		{
			int ret = 0;
			ret += EE.write(s, 1, EE.PbType.INT32, a);
			ret += EE.write(s, 2, g);
			ret += EE.write(s, 3, f);
			return ret;
		}

	}

}


namespace LLProtoBuff
{
	class Program
	{
		static void Main(string[] args)
		{
			Leo f = new Leo();
			f.a = 3;
			f.g = "leo";

			var ms = new MemoryStream();
			f.PbWrite(ms);
			ms.Position = 0;
			Task.Run(async () => await f.PbReadAsync(ms, CancellationToken.None)).Wait();


			
			try
			{
				using (var rd = args.Length == 1 ? new LexReader(args[0]) : new LexReader(Console.In, "stdin"))
				{
					var p = new MParser();
					var dg = p.Start(rd);

					var tw = new U.CsStreamWriter(Console.Out);

					// syntax
					if (dg.Count(e => e.IsSyntax) == 0) throw new SyntaxError("\"syntax\" declaration missing");
					if (dg.Count(e => e.IsSyntax) > 1) throw new SyntaxError("duplicate \"syntax\" declaration");
					if ((dg.FirstOrDefault(e => e.IsSyntax) as SyntaxDecl).ID.strRead != "\"proto3\"") throw new SyntaxError("invalid syntax declaration");

					// package (uno solo ??)
					if (dg.Count(e => e.IsPackage) > 2) throw new SyntaxError("duplicate \"package\" declaration");

					var pkg = dg.FirstOrDefault(e => e.IsPackage);
					if (pkg != null)
					{
						tw.WriteLine("public enum {0}", ((PackageDecl)pkg).Str.strRead);
						tw.WriteLine("{");
					}

					foreach (var en in from v in dg where v.IsEnum select (EnumDecl)v)
					{
						tw.WriteLine("public enum {0}", en.ID.strRead);
						tw.WriteLine("{");
						foreach (var em in en.List)
							tw.WriteLine("{0} = {1};", em.ID.strRead, em.NUM.strRead);
						tw.WriteLine("}");
					}
					foreach (var en in from v in dg where v.IsMessage select (MessageDecl)v)
					{
						tw.WriteLine("public class {0}", en.ID.strRead);
						tw.WriteLine("{");
						foreach (var em in en.Fields)
						{
							if (em.IsOneOf)
							{
								var r = em as OneOf;
								tw.WriteLine($"public int {r.ID.strRead} {{ get; set; }}");
								foreach (var er in r.List)
								{
									tw.WriteLine($"public {csOptional(er.TYPE)} {er.ID.strRead} {{ get => _{er.ID.strRead}; set {{ _{er.ID.strRead} = value; {r.ID.strRead} = {er.NUM.strRead}; }} }}");
									tw.WriteLine($"private {csOptional(er.TYPE)} _{er.ID.strRead};");
									tw.WriteLine($"private bool IsSet_{er.ID.strRead} => {r.ID.strRead} == {er.NUM.strRead};");
								}
							}
							else if (em.IsOptional)
							{
								var r = em as Optional;
								tw.WriteLine($"public {csOptional(r.TYPE)} {r.ID.strRead} {{ get; set; }} }}");
							}
							else if (em.IsRepeated)
							{
								var r = em as Repeated;
								tw.WriteLine($"public List<{csRequired(r.TYPE)}> {r.ID.strRead} {{ get; set; }}");
							}
						}
						tw.WriteLine("}");
					}
					if (pkg != null)
						tw.WriteLine("}");

				}
			}
			catch (SyntaxError ex)
			{
				Console.WriteLine(ex.Message);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		static string csOptional(TokenAST a)
		{
			switch (a.token)
			{
			case MParser.ID: return a.strRead;
			case MParser.DOUBLE: return "double?";
			case MParser.FLOAT: return "float?";
			case MParser.INT32: return "int?";
			case MParser.INT64: return "long?";
			case MParser.UINT32: return "uint?";
			case MParser.UINT64: return "ulong?";
			case MParser.SINT32: return "int?";
			case MParser.SINT64: return "long?";
			case MParser.FIXED32: return "int?";
			case MParser.FIXED64: return "long?";
			case MParser.SFIXED32: return "int?";
			case MParser.SFIXED64: return "long?";
			case MParser.BOOL: return "bool?";
			case MParser.STRING: return "string";
			case MParser.BYTES: return "List<byte>";
			default:
				Debug.Assert(false);
				return null;
			}
		}
		static string csRequired(TokenAST a)
		{
			switch (a.token)
			{
			case MParser.ID: return a.strRead;
			case MParser.DOUBLE: return "double";
			case MParser.FLOAT: return "float";
			case MParser.INT32: return "int";
			case MParser.INT64: return "long";
			case MParser.UINT32: return "uint";
			case MParser.UINT64: return "ulong";
			case MParser.SINT32: return "int";
			case MParser.SINT64: return "long";
			case MParser.FIXED32: return "int";
			case MParser.FIXED64: return "long";
			case MParser.SFIXED32: return "int";
			case MParser.SFIXED64: return "long";
			case MParser.BOOL: return "bool";
			case MParser.STRING: return "string";
			case MParser.BYTES: return "List<byte>";
			default:
				Debug.Assert(false);
				return null;
			}
		}
	}
}
