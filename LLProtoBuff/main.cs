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
using System.Collections.Generic;

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

		//public static int write(Stream s, int fieldNumber, PbObject ww)
		//{
		//	int ret = 0;
		//	if (ww == null) return ret;
		//	int sz = ww.PbWrite(null);
		//	var aa = (fieldNumber << 3) | (int)WireType.LengthDelimited;
		//	ret += write_varint(s, aa);
		//	ret += write_varint(s, sz);
		//	ret += ww.PbWrite(s);
		//	return ret;
		//}
		//public static int write(Stream s, int fieldNumber, PbObject[] ww)
		//{
		//	int ret = 0;
		//	if (ww == null) return ret;
		//	for (int i = 0; i < ww.Length; ++i)
		//		ret += write(s, fieldNumber, ww[i]);

		//	return ret;
		//}

		//static async Task<Tuple<bool, ulong>> readVarintAsync(Stream s, bool allowEof, CancellationToken ct)
		//{
		//	var b = new byte[1];
		//	ulong ret = 0;
		//	int r = await s.ReadAsync(b, 0, 1, ct);
		//	if (r <= 0)
		//		return Tuple.Create(true, 0ul);

		//	for (int i = 0; ; i += 7)
		//	{
		//		if (r != 1) throw new EndOfStreamException();
		//		ulong by = b[0];
		//		ret |= (by & 127) << i;
		//		if ((by & 128) == 0)
		//			return Tuple.Create(false, ret);
		//		r = await s.ReadAsync(b, 0, 1, ct);
		//	}
		//}
		//public struct TagData
		//{
		//	public int fieldNumber;
		//	public int sz;
		//	public WireType wireType;
		//	public MemoryStream ms;
		//}
		//public static async Task<TagData> readTagAsync(Stream s, CancellationToken ct)
		//{
		//	var ret = new TagData { fieldNumber = 0, sz = -1 };
		//	var r = await readVarintAsync(s, allowEof: true, ct: ct);
		//	if (r.Item1) return ret;

		//	ret.fieldNumber = (int)(r.Item2 >> 3);
		//	ret.wireType = (WireType)(r.Item2 & 7);
		//	switch (ret.wireType)
		//	{
		//	case WireType.Bit32: ret.sz = 4; break;
		//	case WireType.Bit64: ret.sz = 8; break;
		//	case WireType.Varint: ret.sz = -1; break;
		//	case WireType.LengthDelimited: ret.sz = (int)(await readVarintAsync(s, false, ct)).Item2; break;
		//	default: throw new InvalidDataException();
		//	}

		//	if (ret.sz > 0)
		//	{
		//		var buff = new byte[ret.sz];
		//		await s.ReadAsync(buff, 0, ret.sz, ct);
		//		ret.ms = new MemoryStream(buff);
		//	}
		//	else
		//	{
		//		ret.ms = new MemoryStream();
		//		for (;;)
		//		{
		//			int b = s.ReadByte();
		//			if (b < 0) throw new EndOfStreamException();
		//			ret.ms.WriteByte((byte)b);
		//			if ((b & 0x80) == 0) break;
		//		}
		//		ret.ms.Position = 0;
		//	}
		//	return ret;	
		//}

		//public static async Task<int> readAsyncInt(Stream s, PbType pbType, int sz, CancellationToken ct)
		//{
		//	if (sz == -1)
		//		return (int)((await readVarintAsync(s, false, ct)).Item2);
		//	Debug.Assert(false);
		//	return 0;
		//}

		//public static async Task<string> readAsyncString(Stream s, PbType pbType, int sz, CancellationToken ct)
		//{
		//	var b = new byte[sz];
		//	await s.ReadAsync(b, 0, sz, ct);
		//	return Encoding.UTF8.GetString(b);
		//}
		//public static async Task<T> readObjectAsync<T>(Stream s, PbType pbType, CancellationToken ct) where T : PbObject, new()
		//{
		//	var r = new T();
		//	await r.PbReadAsync(s, ct);
		//	return r;
		//}
	}

	//public interface PbObject
	//{
	//	int PbWrite(Stream s);
	//	Task PbReadAsync(Stream s, CancellationToken ct);
	//}
	//class Leo : PbObject
	//{
	//	public int a;
	//	public string g;
	//	public Leo f;

	//	public async Task PbReadAsync(Stream s, CancellationToken ct)
	//	{
	//		a = 0;
	//		g = null;
	//		f = null;
	//		for (; ; )
	//		{
	//			var tk = await EE.readTagAsync(s, ct);
	//			if (tk.fieldNumber == 0) break;
	//			switch (tk.fieldNumber)
	//			{
	//			case 1:
	//				a = await EE.readAsyncInt(tk.ms, EE.PbType.INT32, tk.sz, ct);
	//				break;
	//			case 2:
	//				g = await EE.readAsyncString(tk.ms, EE.PbType.STRING, tk.sz, ct);
	//				break;
	//			case 3:
	//				f = await EE.readObjectAsync<Leo>(tk.ms, EE.PbType.embeddedMessage, ct);
	//				break;
	//			}
	//		}
	//	}
	//	public int PbWrite(Stream s)
	//	{
	//		int ret = 0;
	//		ret += EE.write(s, 1, EE.PbType.INT32, a);
	//		ret += EE.write(s, 2, g);
	//		ret += EE.write(s, 3, f);
	//		return ret;
	//	}

	//}

}


namespace LLProtoBuff
{
	public static class ParserExt
	{
		public static string pbType(this Repeated r) { return r.TYPE.strRead; }
		public static string pbType(this Optional r) { return r.TYPE.strRead; }

		public static string csType(this Repeated r) { return $"List<{csRequired(r.TYPE)}>"; }
		public static string csType(this Optional r) { return csRequired(r.TYPE); }

		public static string cppType(this Repeated r, List<string> enumList) { return $"std::vector<{cppRequired(r.TYPE, enumList)}>"; }
		public static string cppType(this Optional r, List<string> enumList) { return cppRequired(r.TYPE, enumList); }


		public static int tag(this Repeated r) { return int.Parse(r.NUM.strRead); }
		public static int tag(this Optional r) { return int.Parse(r.NUM.strRead); }

		public static string varName(this Repeated r) { return r.ID.strRead; }
		public static string varName(this Optional r) { return r.ID.strRead; }
		public static string varName(this OneOf r) => r.ID.strRead;

		public static bool isObject(this Optional r, List<string> enumList) => r.TYPE.token == MParser.ID && !enumList.Contains(r.pbType());
		public static bool isObject(this Repeated r, List<string> enumList) => r.TYPE.token == MParser.ID && !enumList.Contains(r.pbType());
		public static bool isEnum(this Optional r, List<string> enumList) => r.TYPE.token == MParser.ID && enumList.Contains(r.pbType());
		public static bool isEnum(this Repeated r, List<string> enumList) => r.TYPE.token == MParser.ID && enumList.Contains(r.pbType());


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

		static string cppRequired(TokenAST a, List<string> enumList)
		{
			switch (a.token)
			{
			case MParser.ID: if (enumList.Contains(a.strRead) == false) return a.strRead + "*"; else return a.strRead;
			case MParser.DOUBLE: return "double";
			case MParser.FLOAT: return "float";
			case MParser.INT32: return "int32_t";
			case MParser.INT64: return "int64_t";
			case MParser.UINT32: return "uint32_t";
			case MParser.UINT64: return "uint64_t";
			case MParser.SINT32: return "int32_t";
			case MParser.SINT64: return "int64_t";
			case MParser.FIXED32: return "int32_t";
			case MParser.FIXED64: return "int64_t";
			case MParser.SFIXED32: return "int32_t";
			case MParser.SFIXED64: return "int64_t";
			case MParser.BOOL: return "bool";
			case MParser.STRING: return "std::string";
			case MParser.BYTES: return "std::vector<uint8_t>";
			default:
				Debug.Assert(false);
				return null;
			}
		}

		public static string cppInit(this Optional r, List<string> enumList)
		{
			if (r.TYPE.token == MParser.STRING) return $"{r.varName()}.clear();";
			if (r.TYPE.token == MParser.BYTES) return $"{r.varName()}.clear();";
			if (r.TYPE.token == MParser.ID && enumList.Contains(r.pbType())) return $"{r.varName()} = ({r.cppType(enumList)})0;";
			if (r.TYPE.token == MParser.BOOL) return $"{r.varName()} = false;";
			if (r.TYPE.token == MParser.ID) return $"{r.varName()} = nullptr;";
			return $"{r.varName()} = 0;";
		}
		public static string cppInit(this Repeated r)
		{
			// sono tutti std::vector
			return $"{r.varName()}.clear();";
		}


	}

	class Program
	{
		static int Main(string[] args)
		{
			try
			{
				bool cs = false;
				bool hpp = false;
				bool cpp = false;

				int i;
				for (i = 0; i < args.Length; ++i)
				{
					bool b = false;
					switch (args[i])
					{
					case "-cs": cs = true; break;
					case "-cpp": cpp = true; break;
					case "-hpp": hpp = true; break;
					default:
						b = true;
						break;
					}
					if (b) break;
				}

				string fileIn = null;
				if (i < args.Length)
				{
					fileIn = args[i];
					++i;
				}
				else
				{
					Console.WriteLine("missing file");
					return 1;
				}

				using (var rd = new LexReader(fileIn))
				{
					var p = new MParser();
					var dg = p.Start(rd);

					using (TextWriter xw = Console.Out)
					{
						var tw = new U.CsStreamWriter(xw);

						if (dg.Count(e => e.IsSyntax) == 0) throw new SyntaxError("\"syntax\" declaration missing");
						if (dg.Count(e => e.IsSyntax) > 1) throw new SyntaxError("duplicate \"syntax\" declaration");
						if ((dg.FirstOrDefault(e => e.IsSyntax) as SyntaxDecl).ID.strRead != "\"proto3\"") throw new SyntaxError("invalid syntax declaration");

						// package (uno solo ??)
						if (dg.Count(e => e.IsPackage) > 2) throw new SyntaxError("duplicate \"package\" declaration");

						/***/
						if (cs) GenCS(dg, tw);
						else if (hpp)
						{
							string fhpp = fileIn != null ? Path.GetFileNameWithoutExtension(fileIn) + ".hpp" : null;
							GenHPP(dg, tw, fhpp);
						}
						else if (cpp)
						{
							string fhpp = fileIn != null ? Path.GetFileNameWithoutExtension(fileIn) + ".hpp" : null;
							GenCPP(dg, tw, fhpp);
						}
						else Console.WriteLine("please specify -cs -cpp -hpp");
					}
				}
			}
			catch (SyntaxError ex)
			{
				Console.Error.WriteLine(ex.Message);
				return 1;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
				return 1;
			}
			return 0;
		}

		private static void GenCS(DeclList dg, U.CsStreamWriter tw)
		{
			tw.WriteLine("using System.Collections.Generic;");
			tw.WriteLine();


			var pkg = dg.FirstOrDefault(e => e.IsPackage);
			if (pkg != null)
			{
				tw.WriteLine("namespace {0}", ((PackageDecl)pkg).Str.strRead);
				tw.WriteLine("{");
			}


			var enumList = new List<string>();

			foreach (var en in from v in dg where v.IsEnum select (EnumDecl)v)
			{
				enumList.Add(en.ID.strRead);
				tw.WriteLine("public enum {0}", en.ID.strRead);
				tw.WriteLine("{");
				foreach (var em in en.List)
					tw.WriteLine("{0} = {1},", em.ID.strRead, em.NUM.strRead);
				tw.WriteLine("}");

			}
			foreach (var en in from v in dg where v.IsMessage select (MessageDecl)v)
			{
				if (true)
				{
					tw.WriteLine("public class {0} : U.PB.PbObject", en.ID.strRead);
					tw.WriteLine("{");
					foreach (var em in en.Fields)
					{
						if (em.IsOneOf)
						{
							var r = em as OneOf;
							foreach (var er in r.List)
							{
								tw.WriteLine($"public bool IsSet{er.varName()} => _tag_{r.varName()} == {er.tag()};");
								tw.WriteLine($"public {er.csType()} {er.varName()}");
								tw.WriteLine("{");
								tw.WriteLine($"get => IsSet{er.varName()} ? ({er.csType()})_{r.varName()} : default({er.csType()});");
								tw.WriteLine($"set {{ _{r.varName()} = value; _tag_{r.varName()} = _{r.varName()} != null ? {er.tag()} : 0; }}");
								tw.WriteLine("}");
							}
						}
						else if (em.IsOptional)
						{
							var r = em as Optional;
							tw.WriteLine($"public {r.csType()} {r.varName()} {{ get => _{r.varName()}; set => _{r.varName()} = value; }}");
						}
						else if (em.IsRepeated)
						{
							var r = em as Repeated;
							tw.WriteLine($"public {r.csType()} {r.varName()} {{ get => _{r.varName()}; set => _{r.varName()} = value; }}");
						}
					}
					tw.WriteLine();

					if (true)
					{
						tw.WriteLine("void Clear()");
						tw.WriteLine("{");
						foreach (var em in en.Fields)
						{
							if (em is OneOf r)
							{
								tw.WriteLine($"_{r.varName()} = null;");
								tw.WriteLine($"_tag_{r.varName()} = 0;");
							}
							else if (em is Optional er)
							{
								tw.WriteLine($"{er.varName()} = default({er.csType()});");
							}
							else if (em is Repeated rp)
							{
								tw.WriteLine($"{rp.varName()}.Clear();");
							}
						}
						tw.WriteLine("}");
					}

					if (true)
					{
						tw.WriteLine($"public void Write(U.PB.PbStreamOut w)");
						tw.WriteLine("{");
						foreach (var em in en.Fields)
						{
							if (em.IsOneOf)
							{
								var r = em as OneOf;
								foreach (var er in r.List)
								{
									if (!er.isEnum(enumList))
										tw.WriteLine($"if (IsSet{er.varName()}) w.Write({er.tag()}, {er.varName()});");
									else
										tw.WriteLine($"if (IsSet{er.varName()}) w.Write({er.tag()}, (int){er.varName()});");
								}
							}
							else if (em.IsOptional)
							{
								var r = em as Optional;
								if (!r.isEnum(enumList))
									tw.WriteLine($"w.Write({r.tag()}, {r.varName()});");
								else
									tw.WriteLine($"w.Write({r.tag()}, (int){r.varName()});");
							}
							else if (em.IsRepeated)
							{
								var r = em as Repeated;
								if (!r.isEnum(enumList))
									tw.WriteLine($"w.Write({r.tag()}, {r.varName()});");
								else
									tw.WriteLine($"w.Write({r.tag()}, (int){r.varName()});");
							}
						}
						tw.WriteLine("}");
					}
					tw.WriteLine();

					if (true)
					{
						tw.WriteLine($"public void Read(U.PB.PbStreamIn r)");
						tw.WriteLine("{");
						tw.WriteLine("Clear();");
						tw.WriteLine("for (;;)");
						tw.WriteLine("{");
						tw.WriteLine("U.PB.WireType wt;");
						tw.WriteLine("int tag = r.ReadTag(out wt);");
						tw.WriteLine("switch (tag)");
						tw.WriteLine("{");
						tw.WriteLine("case 0: return;");

						foreach (var em in en.Fields)
						{
							if (em.IsOneOf)
							{

								var r = em as OneOf;
								foreach (var er in r.List)
								{
									if (!er.isEnum(enumList))
										tw.WriteLine($"case {er.tag()}: {{ var t = {er.varName()}; r.Read(wt, ref t); {er.varName()} = t; }} break;");
									else
										tw.WriteLine($"case {er.tag()}: {{ var t = (int){er.varName()}; r.Read(wt, ref t); {er.varName()} = ({er.csType()}) t; }} break;");
								}
							}
							else if (em.IsOptional)
							{
								var r = em as Optional;
								if (!r.isEnum(enumList))
									tw.WriteLine($"case {r.tag()}: r.Read(wt, ref _{r.varName()}); break;");
								else
									tw.WriteLine($"case {r.tag()}: {{ var t = (int){r.varName()}; r.Read(wt, ref t); {r.varName()} = ({r.csType()}) t; }} break;");
							}
							else if (em.IsRepeated)
							{
								var r = em as Repeated;
								if (!r.isEnum(enumList))
									tw.WriteLine($"case {r.tag()}: r.Read(wt, ref _{r.varName()}); break;");
								else
									tw.WriteLine($"case {r.tag()}: {{ var t; (int){r.varName()}; r.Read(wt, ref t); {r.varName()} = ({r.csType()}) t; }} break;");

							}
						}
						tw.WriteLine("default: r.Skip(wt); break;");
						tw.WriteLine("}");
						tw.WriteLine("}");
						tw.WriteLine("}");
					}
					tw.WriteLine();

					foreach (var em in en.Fields)
					{
						if (em.IsOneOf)
						{
							var r = em as OneOf;
							tw.WriteLine($"private object _{r.varName()};");
							tw.WriteLine($"private int _tag_{r.varName()};");
						}
						else if (em.IsOptional)
						{
							var r = em as Optional;
							tw.WriteLine($"private {r.csType()} _{r.varName()};");
						}
						else if (em.IsRepeated)
						{
							var r = em as Repeated;
							tw.WriteLine($"private {r.csType()} _{r.varName()};");
						}
					}
					tw.WriteLine();

					tw.WriteLine("}");
				}
			}

			foreach (var s in from v in dg where v.IsService select (ServiceDecl)v)
			{
				tw.WriteLine($"public static class {s.Name.strRead}");
				tw.WriteLine("{");
				foreach (var f in s.Fun)
				{
					tw.WriteLine($"public static {f.Res.strRead} {f.Name.strRead}(BFC_Interface t, {f.Req.strRead} req) => Stub.PbCall<{f.Req.strRead}, {f.Res.strRead}>(t, \"{f.Name.strRead}\", req);");
				}
				tw.WriteLine("}");
			}

			if (pkg != null)
				tw.WriteLine("}");
		}

		private static void GenHPP(DeclList dg, U.CsStreamWriter tw, string fileHpp)
		{
			tw.WriteLine("#ifndef __{0}__", fileHpp.Replace('.', '_'));
			tw.WriteLine("#define __{0}__", fileHpp.Replace('.', '_'));
			tw.WriteLine();
			tw.WriteLine("#include \"Pb.h\"");
			tw.WriteLine();

			var pkg = dg.FirstOrDefault(e => e.IsPackage);
			if (pkg != null)
			{
				tw.WriteLine("namespace {0}", ((PackageDecl)pkg).Str.strRead);
				tw.WriteLine("{");
			}
			tw.WriteLine();

			foreach (var en in from v in dg where v.IsMessage select (MessageDecl)v)
			{
				var className = en.ID.strRead;
				tw.WriteLine("class {0};", className);
			}
			tw.WriteLine();

			var enumList = new List<string>();

			foreach (var en in from v in dg where v.IsEnum select (EnumDecl)v)
			{
				enumList.Add(en.ID.strRead);
				tw.WriteLine("enum {0}", en.ID.strRead);
				tw.WriteLine("{");
				foreach (var em in en.List)
					tw.WriteLine("{0} = {1},", em.ID.strRead, em.NUM.strRead);
				tw.WriteLine("};");
			}
			tw.WriteLine();

			foreach (var en in from v in dg where v.IsMessage select (MessageDecl)v)
			{
				var className = en.ID.strRead;

				tw.WriteLine("class {0} : public PbObject", className);
				tw.WriteLine("{");
				tw.WriteLine("public:");
				tw.WriteLine($"{className}();");
				tw.WriteLine($"~{className}();");
				tw.WriteLine($"void Clear();");
				tw.WriteLine($"void Write(PbStreamOut &w);");
				tw.WriteLine($"void Read(PbStreamIn &r);");

				tw.WriteLine();
				if (true)
				{
					foreach (var em in en.Fields)
					{
						if (em.IsOneOf)
						{
							var r = em as OneOf;
							tw.WriteLine($"int {r.varName()};");

							foreach (var er in r.List)
								tw.WriteLine($"{er.cppType(enumList)} {er.varName()};");
						}
						else if (em.IsOptional)
						{
							var r = em as Optional;
							tw.WriteLine($"{r.cppType(enumList)} {r.varName()};");
						}
						else if (em.IsRepeated)
						{
							var r = em as Repeated;
							tw.WriteLine($"{r.cppType(enumList)} {r.varName()};");
						}
					}
				}

				tw.WriteLine("};");
				tw.WriteLine();
			}

			foreach (var s in from v in dg where v.IsService select (ServiceDecl)v)
			{
				tw.WriteLine($"class {s.Name.strRead}");
				tw.WriteLine("{");
				tw.WriteLine("public:");
				tw.WriteLine($"bool Exec(const std::string &fName, PbStreamIn &sin, PbStreamOut &sout);");
				foreach (var f in s.Fun)
				{
					tw.WriteLine($"virtual void {f.Name.strRead}({f.Req.strRead} &req, {f.Res.strRead} &res) = 0;");
				}
				tw.WriteLine("};");
			}

			if (pkg != null)
				tw.WriteLine("}");
			tw.WriteLine("#endif");
		}

		private static void GenCPP(DeclList dg, U.CsStreamWriter tw, string fileHpp)
		{
			if (!string.IsNullOrEmpty(fileHpp))
			{
				tw.WriteLine($"#include \"{fileHpp}\"");
				tw.WriteLine();
			}
			var pkg = dg.FirstOrDefault(e => e.IsPackage);
			if (pkg != null)
			{
				tw.WriteLine("namespace {0}", ((PackageDecl)pkg).Str.strRead);
				tw.WriteLine("{");
			}

			var enumList = new List<string>();
			foreach (var en in from v in dg where v.IsEnum select (EnumDecl)v)
				enumList.Add(en.ID.strRead);

			foreach (var en in from v in dg where v.IsMessage select (MessageDecl)v)
			{
				string className = en.ID.strRead;

				if (true)
				{
					tw.WriteLine($"{className}::{className}()");
					tw.WriteLine("{");
					foreach (var em in en.Fields)
					{
						if (em.IsOneOf)
						{
							var r = em as OneOf;
							foreach (var er in r.List)
								tw.WriteLine(er.cppInit(enumList));
						}
						else if (em.IsOptional)
						{
							var r = em as Optional;
							tw.WriteLine(r.cppInit(enumList));
						}
						else if (em.IsRepeated)
						{
							// repeated optional non sono compatibili.
							var r = em as Repeated;
							tw.WriteLine(r.cppInit());
						}
					}
					tw.WriteLine("}");
				}

				if (true)
				{
					tw.WriteLine($"{className}::~{className}()");
					tw.WriteLine("{");
					tw.WriteLine("Clear();");
					tw.WriteLine("}");
				}
				if (true)
				{
					tw.WriteLine($"void {className}::Clear()");
					tw.WriteLine("{");
					foreach (var em in en.Fields)
					{
						if (em.IsOneOf)
						{
							var r = em as OneOf;
							foreach (var er in r.List)
							{
								// TODO OneOf può essere repeated
								if (er.isObject(enumList))
									tw.WriteLine($"delete {er.varName()};");
								tw.WriteLine(er.cppInit(enumList));
							}
						}
						else if (em.IsOptional)
						{
							var r = em as Optional;
							if (r.isObject(enumList))
								tw.WriteLine($"delete {r.varName()};");
							tw.WriteLine(r.cppInit(enumList));
						}
						else if (em.IsRepeated)
						{
							var r = em as Repeated;
							if (r.isObject(enumList))
								tw.WriteLine($"for (auto p : {r.varName()}) delete p;");
							tw.WriteLine(r.cppInit());
						}
					}
					tw.WriteLine("}");
				}

				if (true)
				{
					tw.WriteLine($"void {className}::Write(PbStreamOut &w)");
					tw.WriteLine("{");
					foreach (var em in en.Fields)
					{
						if (em.IsOneOf)
						{
							var r = em as OneOf;
							foreach (var er in r.List)
							{
								tw.WriteLine($"if ({r.varName()} == {er.tag()})");
								if (!er.isEnum(enumList))
									tw.WriteLine($"w.Write({er.tag()}, {er.varName()});");
								else
									tw.WriteLine($"w.Write({er.tag()}, (int){er.varName()});");
							}
						}
						else if (em.IsOptional)
						{
							var r = em as Optional;
							if (!r.isEnum(enumList))
								tw.WriteLine($"w.Write({r.tag()}, {r.varName()});");
							else
								tw.WriteLine($"w.Write({r.tag()}, (int){r.varName()});");
						}
						else if (em.IsRepeated)
						{
							var r = em as Repeated;
							if (!r.isEnum(enumList))
								tw.WriteLine($"w.Write({r.tag()}, {r.varName()});");
							else
								tw.WriteLine($"w.Write({r.tag()}, (int){r.varName()});");
						}
					}
					tw.WriteLine("}");
				}

				if (true)
				{
					tw.WriteLine($"void {className}::Read(PbStreamIn &r)");
					tw.WriteLine("{");
					tw.WriteLine("Clear();");
					if (enumList.Count > 0) tw.WriteLine("int en = 0;");
					tw.WriteLine("for (;;)");
					tw.WriteLine("{");
					tw.WriteLine("WireType wt;");
					tw.WriteLine("int tag = r.ReadTag(wt);");
					tw.WriteLine("switch (tag)");
					tw.WriteLine("{");
					tw.WriteLine("case 0: return;");

					foreach (var em in en.Fields)
					{
						if (em.IsOneOf)
						{
							var r = em as OneOf;
							foreach (var er in r.List)
							{
								if (!er.isEnum(enumList))
									tw.WriteLine($"case {er.tag()}: r.Read(wt, {er.varName()}); {r.varName()} = {er.tag()}; break;");
								else
									tw.WriteLine($"case {er.tag()}: r.Read(wt, en); {er.varName()} = ({er.TYPE.strRead}) en; {r.varName()} = {er.tag()}; break;");
							}
						}
						else if (em.IsOptional)
						{
							var r = em as Optional;
							if (!r.isEnum(enumList))
								tw.WriteLine($"case {r.tag()}: r.Read(wt, {r.varName()}); break;");
							else
								tw.WriteLine($"case {r.tag()}: r.Read(wt, en); {r.varName()} = ({r.cppType(enumList)}) en; break;");
						}
						else if (em.IsRepeated)
						{
							var r = em as Repeated;
							if (!r.isEnum(enumList))
								tw.WriteLine($"case {r.tag()}: r.Read(wt, {r.varName()}); break;");
							else
								tw.WriteLine($"case {r.tag()}: r.Read(wt, en); {r.varName()} = ({r.cppType(enumList)}) en; break;");

						}
					}
					tw.WriteLine("default: r.Skip(wt); break;");
					tw.WriteLine("}");
					tw.WriteLine("}");
					tw.WriteLine("}");
				}

				tw.WriteLine();

			}

			if (true)
			{
				foreach (var s in from v in dg where v.IsService select (ServiceDecl)v)
				{
					tw.WriteLine($"bool {s.Name.strRead}::Exec(const std::string &fName, PbStreamIn &sin, PbStreamOut &sout)");
					tw.WriteLine("{");
					foreach (var f in s.Fun)
					{
						tw.WriteLine($"if (fName == \"{f.Name.strRead}\")");
						tw.WriteLine("{");
						tw.WriteLine($"{f.Req.strRead} req;");
						tw.WriteLine($"req.Read(sin);");
						tw.WriteLine($"{f.Res.strRead} res;");

						tw.WriteLine($"{f.Name.strRead}(req, res);");

						tw.WriteLine($"res.Write(sout);");
						tw.WriteLine("return true;");
						tw.WriteLine("}");
					}
					tw.WriteLine("return false;");
					tw.WriteLine("}");
				}
			}
			if (pkg != null)
				tw.WriteLine("}");
		}
	}
}
