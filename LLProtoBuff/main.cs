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

	}
}


namespace LLProtoBuff
{
	public static class ParserExt
	{
		public static string pbType(this Repeated r) { return r.TYPE.strRead; }
		public static string pbType(this Optional r) { return r.TYPE.strRead; }


		public static string pbBaseType(this Repeated r) { if (r.TYPE.token == MParser.ID) return "none"; else return r.TYPE.strRead; }
		public static string pbBaseType(this Optional r) { if (r.TYPE.token == MParser.ID) return "none"; else return r.TYPE.strRead; }



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
			case MParser.ID: if (!enumList.Contains(a.strRead)) return a.strRead + " *"; else return a.strRead + "_t";
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
		static List<string> fn = new List<string>();

		static int Main(string[] args)
		{
			try
			{
				bool cs = false;
				bool hpp = false;
				bool cpp = false;

				CsFlags csFlags = 0;
				CppFlags cppFlags = 0;

				string fileOut = null;

				int i;
				for (i = 0; i < args.Length; ++i)
				{
					bool b = false;
					switch (args[i])
					{
					case "-cs": cs = true; break;
					case "-cpp": cpp = true; break;
					case "-hpp": hpp = true; break;

					case "-cs:Messages": csFlags = csFlags | CsFlags.Messages; break;
					case "-cs:ServiceInterface": csFlags = csFlags | CsFlags.ServiceInterface; break;
					case "-cs:ServiceInterfaceAsync": csFlags = csFlags | CsFlags.ServiceInterfaceAsync; break;
					case "-cs:ClientPbCall": csFlags = csFlags | CsFlags.ClientPbCall; break;
					case "-cs:ClientPbCallAsync": csFlags = csFlags | CsFlags.ClientPbCallAsync; break;
					case "-cs:DataContractWS": csFlags = csFlags | CsFlags.DataContractWS; break;
					case "-cs:PbCallStub": csFlags = csFlags | CsFlags.PbCallStub; break;


					case "-cpp:Messages": cppFlags = cppFlags | CppFlags.Messages; break;
					case "-cpp:PbCallStub": cppFlags = cppFlags | CppFlags.PbCallStub; break;
					case "-cpp:ClientPbCall": cppFlags = cppFlags | CppFlags.ClientPbCall; break;

					default:
						if (args[i].StartsWith("-o:"))
						{
							if (args[i].Length == 3)
							{
								Console.Error.WriteLine("Invalid option {0}", args[i]);
								return 1;
							}
							fileOut = args[i].Substring(3);
						}
						else if (args[i] == "-o")
						{
							if (i + 1 >= args.Length)
							{
								Console.Error.WriteLine("Invalid option {0}", args[i]);
								return 1;
							}
							fileOut = args[i + 1];
							i++;
						}
						else if (args[i].StartsWith("-"))
						{
							Console.Error.WriteLine("Invalid option {0}", args[i]);
							return 1;
						}
						b = true;
						break;
					}
					if (b) break;
				}

				if (csFlags == 0)
				{
					csFlags = CsFlags.Messages | CsFlags.PbCallStub;
				}
				if (cppFlags == 0)
				{
					cppFlags = CppFlags.Messages | CppFlags.PbCallStub;
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

				void Parse(string fileInput, DeclList r)
				{
					using (var rd = new LexReader(fileInput))
					{
						var p = new MParser();


						foreach (var dc in p.Start(rd))
						{
							if (dc.IsImport)
							{
								string fi = ((ImportDecl)dc).ID.strRead;
								fi = fi.Remove(0, 1);
								fi = fi.Remove(fi.Length - 1, 1);

								if (!fn.Contains(fi))
								{
									//						Console.Error.WriteLine("Parsing {0}", fi);
									fn.Add(fi);
									Parse(fi, r);
								}
							}
							else
								r.Add(dc);
						}
					}
				}

				if (true)
				{
					var p = new MParser();

					DeclList dg = new DeclList();
					Parse(fileIn, dg);

					string fileTmp = "__tmp__.tmp";
					TextWriter xw = (fileOut != null) ? File.CreateText(fileTmp) : Console.Out;
					using (xw)
					{
						var tw = new U.CsStreamWriter(xw);

						if (dg.Count(e => e.IsSyntax) == 0) throw new SyntaxError("\"syntax\" declaration missing");
						//if (dg.Count(e => e.IsSyntax) > 1) throw new SyntaxError("duplicate \"syntax\" declaration");
						if ((dg.FirstOrDefault(e => e.IsSyntax) as SyntaxDecl).ID.strRead != "\"proto3\"") throw new SyntaxError("invalid syntax declaration");

						// package (uno solo ??)
						if (dg.Count(e => e.IsPackage) >= 2) throw new SyntaxError("duplicate \"package\" declaration");

						if (cs) GenCS(dg, tw, csFlags);
						else if (hpp)
						{
							string fhpp = fileIn != null ? Path.GetFileNameWithoutExtension(fileIn) + ".hpp" : null;
							GenHPP(dg, tw, fhpp, cppFlags);
						}
						else if (cpp)
						{
							string fhpp = fileIn != null ? Path.GetFileNameWithoutExtension(fileIn) + ".hpp" : null;
							GenCPP(dg, tw, fhpp, cppFlags);
						}
						else Console.WriteLine("please specify -cs -cpp -hpp");
					}

					if (fileOut != null)
					{
						if (!File.Exists(fileOut) || File.ReadAllText(fileTmp) != File.ReadAllText(fileOut))
						{
							File.Delete(fileOut);
							File.Move(fileTmp, fileOut);
						}
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
				Console.Error.WriteLine($"{ex.Message}");
				Console.Error.WriteLine(ex.StackTrace);
				return 1;
			}
			return 0;
		}

		[Flags]
		public enum CsFlags
		{
			Messages = 1,
			ServiceInterface = 2,
			ServiceInterfaceAsync = 4,
			ClientPbCall = 8,
			ClientPbCallAsync = 16,
			PbCallStub = 32,

			DataContractWS = 64
		}

		private static void GenCS(DeclList dg, U.CsStreamWriter tw, CsFlags csFlags)
		{
			tw.WriteLine("// Generated by LLProtoBuff. DO NOT MODIFY");
			tw.WriteLine();

			tw.WriteLine("using System.Collections.Generic;");
			tw.WriteLine("using System.Threading.Tasks;");
			if ((csFlags & CsFlags.DataContractWS) == CsFlags.DataContractWS)
			{
				tw.WriteLine("using System.Runtime.Serialization;");
				tw.WriteLine("using System.ServiceModel;");
			}
			tw.WriteLine();

			var pkgName = "";

			if (pkgName == "")
			{
				var opt = dg.FirstOrDefault(e => e.IsOption);
				if (opt != null)
				{
					var w = (OptionDecl)opt;
					if (w.Id.strRead == "csharp_namespace")
						pkgName = w.Str.strRead.Substring(1, w.Str.strRead.Length - 2);
				}
			}

			if (pkgName == "")
			{
				var pkg = dg.FirstOrDefault(e => e.IsPackage);
				if (pkg != null)
					pkgName = ((PackageDecl)pkg).Str.strRead;
			}

			if (pkgName != "")
			{
				tw.WriteLine("namespace {0}", pkgName);
				tw.WriteLine("{");
			}

			if ((csFlags & CsFlags.DataContractWS) == CsFlags.DataContractWS)
			{
				tw.WriteLine("public static class Constants");
				tw.WriteLine("{");
				tw.WriteLine("public const string Namespace = \"http://schemas.datacontract.org/2004/07/ET-Mobile\";");
				tw.WriteLine("}");
			}

			if ((csFlags & CsFlags.Messages) == CsFlags.Messages)
			{
				var enumList = new List<string>();
				foreach (var en in from v in dg where v.IsEnum select (EnumDecl)v)
				{
					enumList.Add(en.ID.strRead);
					if ((csFlags & CsFlags.DataContractWS) == CsFlags.DataContractWS)
						tw.WriteLine("[DataContract(Namespace = Constants.Namespace)]");
					tw.WriteLine("public enum {0}", en.ID.strRead);
					tw.WriteLine("{");
					foreach (var em in en.List)
					{
						if ((csFlags & CsFlags.DataContractWS) == CsFlags.DataContractWS)
							tw.Write("[EnumMember] ");
						tw.WriteLine("{0} = {1},", em.ID.strRead, em.NUM.strRead);
					}
					tw.WriteLine("}");
					tw.WriteLine("////////////////////////");
				}
				foreach (var en in from v in dg where v.IsMessage select (MessageDecl)v)
				{
					if ((csFlags & CsFlags.DataContractWS) == CsFlags.DataContractWS)
						tw.WriteLine("[DataContract(Namespace = Constants.Namespace)]");
					tw.WriteLine("public partial class {0} : U.PB.PbObject", en.ID.strRead);
					tw.WriteLine("{");
					int order = 0;
					foreach (var em in en.Fields)
					{
						if (order % 100 > 0) order -= order % 100;
						if (em.IsOneOf)
						{
							var r = em as OneOf;

							foreach (var er in r.List)
							{
								order += 1;
								tw.WriteLine($"public bool IsSet_{er.varName()} => _tag_{r.varName()} == {er.tag()};");
								if ((csFlags & CsFlags.DataContractWS) == CsFlags.DataContractWS)
									tw.Write($"[DataMember(Order = {order}, IsRequired = false))] ");
								tw.WriteLine($"public {er.csType()} {er.varName()}");
								tw.WriteLine("{");
								tw.WriteLine($"get => IsSet_{er.varName()} ? ({er.csType()})_{r.varName()} : default({er.csType()});");
								tw.WriteLine($"set {{ _{r.varName()} = value; _tag_{r.varName()} = _{r.varName()} != null ? {er.tag()} : 0; }}");
								tw.WriteLine("}");
							}
						}
						else if (em.IsOptional)
						{
							order += 100;
							var r = em as Optional;
							if ((csFlags & CsFlags.DataContractWS) == CsFlags.DataContractWS)
								tw.Write($"[DataMember(Order = {order}, IsRequired = {(r.OPTIONAL ? "false" : "true")})] ");
							tw.WriteLine($"public {r.csType()} {r.varName()} {{ get => _{r.varName()}; set => _{r.varName()} = value; }}");
						}
						else if (em.IsRepeated)
						{
							order += 100;
							var r = em as Repeated;
							if ((csFlags & CsFlags.DataContractWS) == CsFlags.DataContractWS)
								tw.Write($"[DataMember(Order = {order}, IsRequired = {(r.OPTIONAL ? "false" : "true")})] ");
							tw.WriteLine($"public {r.csType()} {r.varName()} {{ get => _{r.varName()}; set => _{r.varName()} = value; }}");
						}
					}
					tw.WriteLine();

					if (true)
					{
						tw.WriteLine("public void Clear()");
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
								tw.WriteLine($"{rp.varName()}?.Clear();");
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
										tw.WriteLine($"if (IsSet_{er.varName()}) w.Write({er.tag()}, U.PB.PbType.pb_{er.pbBaseType()}, {er.varName()});");
									else
										tw.WriteLine($"if (IsSet_{er.varName()}) w.Write({er.tag()}, U.PB.PbType.pb_{er.pbBaseType()}, (int){er.varName()});");
								}
							}
							else if (em.IsOptional)
							{
								var r = em as Optional;
								if (!r.isEnum(enumList))
									tw.WriteLine($"w.Write({r.tag()}, U.PB.PbType.pb_{r.pbBaseType()}, {r.varName()});");
								else
									tw.WriteLine($"w.Write({r.tag()}, U.PB.PbType.pb_{r.pbBaseType()}, (int){r.varName()});");
							}
							else if (em.IsRepeated)
							{
								var r = em as Repeated;
								if (!r.isEnum(enumList))
									tw.WriteLine($"w.Write({r.tag()}, U.PB.PbType.pb_{r.pbBaseType()}, {r.varName()});");
								else
									tw.WriteLine($"w.Write({r.tag()}, U.PB.PbType.pb_{r.pbBaseType()}, (int){r.varName()});");
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
										tw.WriteLine($"case {er.tag()}: {{ var t = {er.varName()}; r.Read(wt, U.PB.PbType.pb_{er.pbBaseType()}, ref t); {er.varName()} = t; }} break;");
									else
										tw.WriteLine($"case {er.tag()}: {{ var t = (int){er.varName()}; r.Read(wt, U.PB.PbType.pb_{er.pbBaseType()}, ref t); {er.varName()} = ({er.csType()}) t; }} break;");
								}
							}
							else if (em.IsOptional)
							{
								var r = em as Optional;
								if (!r.isEnum(enumList))
									tw.WriteLine($"case {r.tag()}: r.Read(wt, U.PB.PbType.pb_{r.pbBaseType()}, ref _{r.varName()}); break;");
								else
									tw.WriteLine($"case {r.tag()}: {{ var t = (int){r.varName()}; r.Read(wt, U.PB.PbType.pb_{r.pbBaseType()}, ref t); {r.varName()} = ({r.csType()}) t; }} break;");
							}
							else if (em.IsRepeated)
							{
								var r = em as Repeated;
								if (!r.isEnum(enumList))
									tw.WriteLine($"case {r.tag()}: r.Read(wt, U.PB.PbType.pb_{r.pbBaseType()}, ref _{r.varName()}); break;");
								else
									tw.WriteLine($"case {r.tag()}: {{ var t; (int){r.varName()}; r.Read(wt, U.PB.PbType.pb_{r.pbBaseType()}, ref t); {r.varName()} = ({r.csType()}) t; }} break;");

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
					tw.WriteLine("////////////////////////");
					tw.WriteLine();
				}
			}

			if ((csFlags & CsFlags.ClientPbCall) == CsFlags.ClientPbCall)
			{
				tw.WriteLine();
				tw.WriteLine("////////////////////");
				foreach (var s in from v in dg where v.IsService select (ServiceDecl)v)
				{
					tw.WriteLine($"public partial class {s.Name.strRead}Client : PbCore.PbClientBase");
					tw.WriteLine("{");
					tw.WriteLine($"public {s.Name.strRead}Client(string addr) : base(addr) {{}}");
					foreach (var f in s.Fun)
						tw.WriteLine($"public {f.Res.strRead} {f.Name.strRead}({f.Req.strRead} req) => PbCall<{f.Req.strRead}, {f.Res.strRead}>(\"{pkgName}\", \"{s.Name.strRead}\", \"{f.Name.strRead}\", req);");
					tw.WriteLine("}");
				}
			}
			if ((csFlags & CsFlags.ClientPbCallAsync) == CsFlags.ClientPbCallAsync)
			{
				tw.WriteLine();
				tw.WriteLine("////////////////////");
				foreach (var s in from v in dg where v.IsService select (ServiceDecl)v)
				{
					tw.WriteLine($"public partial class {s.Name.strRead}ClientAsync : PbCore.PbClientBase");
					tw.WriteLine("{");
					tw.WriteLine($"public {s.Name.strRead}ClientAsync(string addr) : base(addr) {{}}");
					foreach (var f in s.Fun)
						tw.WriteLine($"public async Task<{f.Res.strRead}> {f.Name.strRead}Async({f.Req.strRead} req) => await PbCallAsync<{f.Req.strRead}, {f.Res.strRead}>(\"{pkgName}\", \"{s.Name.strRead}\", \"{f.Name.strRead}\", req);");
					tw.WriteLine("}");
				}
			}

			if ((csFlags & CsFlags.PbCallStub) == CsFlags.PbCallStub)
			{
				tw.WriteLine();
				tw.WriteLine("////////////////////");
				foreach (var s in from v in dg where v.IsService select (ServiceDecl)v)
				{
					tw.WriteLine($"public partial class {s.Name.strRead}");
					tw.WriteLine("{");
					foreach (var f in s.Fun)
						tw.WriteLine($"public {f.Res.strRead} {f.Name.strRead}({f.Req.strRead} req) => PbCall<{f.Req.strRead}, {f.Res.strRead}>(\"{pkgName}\", \"{s.Name.strRead}\", \"{f.Name.strRead}\", req);");
					tw.WriteLine("}");
				}
			}

			if ((csFlags & CsFlags.ServiceInterfaceAsync) == CsFlags.ServiceInterfaceAsync)
			{
				tw.WriteLine();
				tw.WriteLine("////////////////////");
				foreach (var s in from v in dg where v.IsService select (ServiceDecl)v)
				{
					tw.WriteLine($"[U.PB.PbClass(\"{pkgName}\", \"{s.Name.strRead}\")]");
					if ((csFlags & CsFlags.DataContractWS) == CsFlags.DataContractWS)
						tw.WriteLine("[ServiceContract(Namespace = Constants.Namespace)]");
					tw.WriteLine($"public interface I{s.Name.strRead}Async");
					tw.WriteLine("{");
					foreach (var f in s.Fun)
					{
						if ((csFlags & CsFlags.DataContractWS) == CsFlags.DataContractWS)
							tw.Write("[OperationContract] ");
						tw.Write($"[U.PB.PbMethod(\"{f.Name.strRead}\")] ");
						tw.WriteLine($"Task<{f.Res.strRead}> {f.Name.strRead}Async({f.Req.strRead} req);");
					}
					tw.WriteLine("}");
				}
			}

			if ((csFlags & CsFlags.ServiceInterface) == CsFlags.ServiceInterface)
			{
				tw.WriteLine();
				tw.WriteLine("////////////////////");
				foreach (var s in from v in dg where v.IsService select (ServiceDecl)v)
				{
					tw.WriteLine($"[U.PB.PbClass(\"{pkgName}\", \"{s.Name.strRead}\")]");
					tw.WriteLine($"public interface I{s.Name.strRead}");
					tw.WriteLine("{");
					foreach (var f in s.Fun)
					{
						tw.Write($"[U.PB.PbMethod(\"{f.Name.strRead}\")] ");
						tw.WriteLine($"{f.Res.strRead} {f.Name.strRead}({f.Req.strRead} req);");
					}
					tw.WriteLine("}");
				}
			}

			if (pkgName.Length > 0)
				tw.WriteLine("}");
		}


		[Flags]
		public enum CppFlags
		{
			Messages = 1,
			ClientPbCall = 2,
			PbCallStub = 4
		}

		private static void GenHPP(DeclList dg, U.CsStreamWriter tw, string fileHpp, CppFlags cppFlags)
		{
			tw.WriteLine("// Generated by LLProtoBuff. DO NOT MODIFY");
			tw.WriteLine();
			tw.WriteLine("#ifndef __{0}__", fileHpp.Replace('.', '_'));
			tw.WriteLine("#define __{0}__", fileHpp.Replace('.', '_'));
			tw.WriteLine();
			tw.WriteLine("#include \"Pb.h\"");
			if ((cppFlags & CppFlags.ClientPbCall) == CppFlags.ClientPbCall)
				tw.WriteLine("#include \"pbClient.h\"");
			tw.WriteLine();

			var pkg = dg.FirstOrDefault(e => e.IsPackage);
			if (pkg != null)
			{
				tw.WriteLine("namespace {0}", ((PackageDecl)pkg).Str.strRead);
				tw.WriteLine("{");
			}
			tw.WriteLine();

			if ((cppFlags & CppFlags.Messages) == CppFlags.Messages)
			{

				// predichiarazione delle classi
				foreach (var en in from v in dg where v.IsMessage select (MessageDecl)v)
				{
					var className = en.ID.strRead;
					tw.WriteLine("class {0};", className);
				}
				tw.WriteLine();

				// enum
				var enumList = new List<string>();
				foreach (var en in from v in dg where v.IsEnum select (EnumDecl)v)
				{
					enumList.Add(en.ID.strRead);
					tw.WriteLine("enum class {0}_t", en.ID.strRead);
					tw.WriteLine("{");
					foreach (var em in en.List)
						tw.WriteLine("{0} = {1},", em.ID.strRead, em.NUM.strRead);
					tw.WriteLine("};");
					tw.WriteLine();
					tw.WriteLine("////////////////////////");
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

					tw.WriteLine("private:");
					tw.WriteLine($"{className}(const {className} &) = delete;");
					tw.WriteLine($"void operator =(const {className} &) = delete;");

					tw.WriteLine("};");
					tw.WriteLine();
					tw.WriteLine("////////////////////////");
				}
			}

			if ((cppFlags & CppFlags.PbCallStub) == CppFlags.PbCallStub)
			{
				// servizi
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
			}

			if ((cppFlags & CppFlags.ClientPbCall) == CppFlags.ClientPbCall)
			{
				foreach (var s in from v in dg where v.IsService select (ServiceDecl)v)
				{
					tw.WriteLine($"class {s.Name.strRead}Client : public ProtoClient");
					tw.WriteLine("{");
					tw.WriteLine("public:");
					tw.WriteLine($"{s.Name.strRead}Client(const std::string &name) : ProtoClient(name) {{}}");
					foreach (var f in s.Fun)
					{
						tw.WriteLine($"void {f.Name.strRead}({f.Req.strRead} &req, {f.Res.strRead} &res) {{ Exec(\"{f.Name.strRead}\", req, res); }};");
					}
					tw.WriteLine("};");
				}
			}

			if (pkg != null)
				tw.WriteLine("}");
			tw.WriteLine("#endif");
		}

		private static void GenCPP(DeclList dg, U.CsStreamWriter tw, string fileHpp, CppFlags cppFlags)
		{
			tw.WriteLine("// Generated by LLProtoBuff. DO NOT MODIFY");
			tw.WriteLine();

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

			if ((cppFlags & CppFlags.Messages) == CppFlags.Messages)
			{
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
										tw.WriteLine($"w.Write({er.tag()}, PbType::pb_{er.pbBaseType()}, {er.varName()});");
									else
										tw.WriteLine($"w.Write({er.tag()}, PbType::pb_{er.pbBaseType()}, (int){er.varName()});");
								}
							}
							else if (em.IsOptional)
							{
								var r = em as Optional;
								if (!r.isEnum(enumList))
									tw.WriteLine($"w.Write({r.tag()}, PbType::pb_{r.pbBaseType()}, {r.varName()});");
								else
									tw.WriteLine($"w.Write({r.tag()}, PbType::pb_{r.pbBaseType()}, (int){r.varName()});");
							}
							else if (em.IsRepeated)
							{
								var r = em as Repeated;
								if (!r.isEnum(enumList))
									tw.WriteLine($"w.Write({r.tag()}, PbType::pb_{r.pbBaseType()}, {r.varName()});");
								else
									tw.WriteLine($"w.Write({r.tag()}, PbType::pb_{r.pbBaseType()}, (int){r.varName()});");
							}
						}
						tw.WriteLine("}");
					}

					if (true)
					{
						bool needEn = false;
						foreach (var em in en.Fields)
						{
							if (em.IsOneOf)
							{
								var r = em as OneOf;
								foreach (var er in r.List)
								{
									if (er.isEnum(enumList))
										needEn = true;
								}
							}
							else if (em.IsOptional)
							{
								var r = em as Optional;
								if (r.isEnum(enumList))
									needEn = true;
							}
							else if (em.IsRepeated)
							{
								var r = em as Repeated;
								if (r.isEnum(enumList))
									needEn = true;
							}
						}


						tw.WriteLine($"void {className}::Read(PbStreamIn &r)");
						tw.WriteLine("{");
						tw.WriteLine("Clear();");
						if (needEn) tw.WriteLine("int en = 0;");
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
										tw.WriteLine($"case {er.tag()}: r.Read(wt, PbType::pb_{er.pbBaseType()}, {er.varName()}); {r.varName()} = {er.tag()}; break;");
									else
										tw.WriteLine($"case {er.tag()}: r.Read(wt, PbType::pb_{er.pbBaseType()}, en); {er.varName()} = ({er.TYPE.strRead}) en; {r.varName()} = {er.tag()}; break;");
								}
							}
							else if (em.IsOptional)
							{
								var r = em as Optional;
								if (!r.isEnum(enumList))
									tw.WriteLine($"case {r.tag()}: r.Read(wt, PbType::pb_{r.pbBaseType()}, {r.varName()}); break;");
								else
									tw.WriteLine($"case {r.tag()}: r.Read(wt, PbType::pb_{r.pbBaseType()}, en); {r.varName()} = ({r.cppType(enumList)}) en; break;");
							}
							else if (em.IsRepeated)
							{
								var r = em as Repeated;
								if (!r.isEnum(enumList))
									tw.WriteLine($"case {r.tag()}: r.Read(wt, PbType::pb_{r.pbBaseType()}, {r.varName()}); break;");
								else
									tw.WriteLine($"case {r.tag()}: r.Read(wt, PbType::pb_{r.pbBaseType()}, en); {r.varName()} = ({r.cppType(enumList)}) en; break;");

							}
						}
						tw.WriteLine("default: r.Skip(wt); break;");
						tw.WriteLine("}");
						tw.WriteLine("}");
						tw.WriteLine("}");
					}

					tw.WriteLine();
					tw.WriteLine("////////////////////////");
				}
			}

			if ((cppFlags & CppFlags.PbCallStub) == CppFlags.PbCallStub)
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
