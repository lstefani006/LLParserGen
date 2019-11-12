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

		//public static string csType(this Repeated r) => $"{csRequired(r.TYPE)}[]";
		public static string csType(this Repeated r) => $"List<{csRequired(r.TYPE)}>";
		public static string csType(this Optional r) => csRequired(r.TYPE);

		public static string wsType(this Repeated r, List<string> enumList)
		{
			var v = csRequired(r.TYPE);
			if (v == "DateTime_t") v = "DateTime";
			if (r.isEnum(enumList)) v = "int";
			return $"{v}[]";
		}
		public static string wsType(this Optional r, List<string> enumList)
		{
			var v = csRequired(r.TYPE);
			if (v == "DateTime_t") v = "DateTime";
			if (r.isEnum(enumList)) v = "int";
			return v;
		}

		public static bool csIsEnum(this Optional r, List<string> enumList)
		{
			if (r.TYPE.token == MParser.ID)
				return enumList.Exists(p => p == r.TYPE.strRead);
			return false;
		}
		public static bool csIsEnum(this Repeated r, List<string> enumList)
		{
			if (r.TYPE.token == MParser.ID)
				return enumList.Exists(p => p == r.TYPE.strRead);
			return false;
		}

		public static string cppType(this Repeated r, List<string> enumList) { return $"std::vector<{cppRequired(r.TYPE, enumList)}>"; }
		public static string cppType(this Optional r, List<string> enumList) { return cppRequired(r.TYPE, enumList); }


		public static int tag(this Repeated r) => int.Parse(r.NUM.strRead);
		public static int tag(this Optional r) => int.Parse(r.NUM.strRead);

		public static string varName(this Repeated r) => r.ID.strRead;
		public static string varName(this Optional r) => r.ID.strRead;
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
			case MParser.BYTES: return "byte[]";// "List<byte>";
			default:
				Debug.Assert(false);
				return null;
			}
		}

		static string cppRequired(TokenAST a, List<string> enumList)
		{
			switch (a.token)
			{
			case MParser.ID: if (!enumList.Contains(a.strRead)) return a.strRead + " *"; else return a.strRead.EndsWith("_t") ? a.strRead : a.strRead + "_t";
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


		private static void Help()
		{
			Console.WriteLine("C#");
			Console.WriteLine("-cs                        genera il codice C# con le opzioni che seguono");
			Console.WriteLine("-cs:Messages               genera le classi dei messaggi");
			Console.WriteLine();
			Console.WriteLine("-cs:ServiceInterface       genera l'interfaccia del servizio");
			Console.WriteLine("-cs:ServiceInterfaceAsync  genera l'interfaccia del servizio con chiamate async");
			Console.WriteLine("-cs:DataContractWS         aggiunge gli attributi per il SOAP");
			Console.WriteLine("-cs:PbCallStub             genera la classe del servizio con chiamate PbCall");
			Console.WriteLine();
			Console.WriteLine("-cs:ClientPbCall           genera il codice per chiamare il servizio lato client");
			Console.WriteLine("-cs:ClientPbCallAsync      genera il codice per chiamare il servizio lato client con chiamete async");
			Console.WriteLine();

			Console.WriteLine("C++");
			Console.WriteLine("-cpp                        genera il codice C++ .cpp con le opzioni che seguono");
			Console.WriteLine("-hpp                        genera il codice C++ .hpp con le opzioni che seguono");
			Console.WriteLine("-cpp:Messages               genera le classi dei messaggi");
			Console.WriteLine("-cpp:PbCallStub             genera le classi per le chiamate locali");
			Console.WriteLine("-cpp:ClientPbCall           genera la classe del servizio per le chiamate in versione \"old\"");
			Console.WriteLine("-cpp:ClientPbCall2          genera la classe del servizio per le chiamate in versione \"new\"");
			Console.WriteLine("-dll:<nome>                 genera le classi dei messaggi con le macro per l'export <nome>_DLL, <nome>_BUILD");
			Console.WriteLine();
			Console.WriteLine("C# / C++");
			Console.WriteLine("-o:<file>                   genera il file <file> in uscita");
		}


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
				string dll = null;

				if (args.Length == 0)
				{
					Help();
					return 0;
				}

				int i;
				for (i = 0; i < args.Length; ++i)
				{
					bool b = false;
					switch (args[i])
					{
					case "-h": Help(); return 0;
					case "-cs": cs = true; break;
					case "-cpp": cpp = true; break;
					case "-hpp": hpp = true; break;

					case "-cs:Messages": csFlags |= CsFlags.Messages; break;
					case "-cs:ServiceInterface": csFlags |= CsFlags.ServiceInterface; break;
					case "-cs:ServiceInterfaceAsync": csFlags |= CsFlags.ServiceInterfaceAsync; break;

					case "-cs:ClientPbCall": csFlags |= CsFlags.ClientPbCall; break;
					case "-cs:ClientPbCallAsync": csFlags |= CsFlags.ClientPbCallAsync; break;
					case "-cs:DataContractWS": csFlags |= CsFlags.DataContractWS; break;
					case "-cs:PbCallStub": csFlags |= CsFlags.PbCallStub; break;

					case "-cpp:Messages": cppFlags |= CppFlags.Messages; break;
					case "-cpp:PbCallStub": cppFlags |= CppFlags.PbCallStub; break;
					case "-cpp:ClientPbCall": cppFlags |= CppFlags.ClientPbCall; break;
					case "-cpp:ClientPbCall2": cppFlags |= CppFlags.ClientPbCall2; break;
					case "-cpp:ClientRestCall": cppFlags |= CppFlags.ClientRestCall; break;

					default:
						if (args[i].StartsWith("-dll:"))
						{
							dll = args[i].Substring("-dll:".Length);
							continue;
						}
						else if (args[i].StartsWith("-o:"))
						{
							fileOut = args[i].Substring("-o:".Length);
							continue;
						}
						else if (args[i] == "-o")
						{
							fileOut = args[++i];
							continue;
						}
						else if (args[i].StartsWith("-"))
						{
							Console.Error.WriteLine("Invalid option \"{0}\"", args[i]);
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

						bool error = false;
						foreach (var ee in dg
							.FindAll(r => r.IsMessage)
							.GroupBy(g => ((MessageDecl)g).ID.strRead)
							.Select(g => new { Key = g.Key, Count = g.Count() })
							.Where(g => g.Count > 1))
						{
							Console.Error.WriteLine("duplicated {0} ", ee.Key);
							error = true;
						}
						if (error)
						{
							Console.Error.WriteLine();
							throw new SyntaxError("duplicate \"message\" declaration");
						}

						if (cs)
						{
							if ((csFlags & CsFlags.DataContractWS) > 0)
								GenCS_WS(dg, tw, csFlags);
							else
							{
								csFlags = (CsFlags)((int)csFlags & ((int)(CsFlags.DataContractWS) - 1));
								GenCS_PB(dg, tw, csFlags);
							}
						}

						else if (hpp)
						{
							string fhpp = fileIn != null ? Path.GetFileNameWithoutExtension(fileIn) + ".hpp" : null;
							GenHPP(dg, tw, fhpp, cppFlags, dll);
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
					if (File.Exists(fileTmp))
						File.Delete(fileTmp);
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
			PbCallStub = 64,

			ServiceInterfaceJson = 128,

			DataContractWS = 256
		}

		private static void GenCS_PB(DeclList dg, U.CsStreamWriter tw, CsFlags csFlags)
		{
			tw.WriteLine("// Generated by LLProtoBuff. DO NOT MODIFY");
			tw.WriteLine();

			tw.WriteLine("using System;");
			tw.WriteLine("using System.Collections.Generic;");
			tw.WriteLine("using System.Threading.Tasks;");
			tw.WriteLine("using System.Net.Http;");
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

			tw.WriteLine("// Rappresenta una data/ora in localtime");
			tw.WriteLine("public partial class DateTime_t");
			tw.WriteLine("{");
			tw.WriteLine("public DateTime_t() {}");
			tw.WriteLine("public DateTime_t(DateTime dt)");
			tw.WriteLine("{");
			tw.WriteLine("var lt = dt.ToLocalTime();");
			tw.WriteLine("Year = lt.Year;");
			tw.WriteLine("Month = lt.Month;");
			tw.WriteLine("Day = lt.Day;");
			tw.WriteLine("Hour = lt.Hour;");
			tw.WriteLine("Minutes = lt.Minute;");
			tw.WriteLine("Seconds = lt.Second;");
			tw.WriteLine("}");
			tw.WriteLine("public DateTime ToDateTime() => GetDateTime();");
			tw.WriteLine("public bool HasValue => !(Year == 0 && Month == 0 && Day == 0);");
			tw.WriteLine("public DateTime? ToNullableDateTime() => HasValue ? (DateTime?)GetDateTime() : null;");

			tw.WriteLine("private DateTime GetDateTime()");
			tw.WriteLine("{");
			tw.WriteLine("try");
			tw.WriteLine("{");
			tw.WriteLine("return new DateTime(Year, Month, Day, Hour, Minutes, Seconds, 0, System.DateTimeKind.Local);");
			tw.WriteLine("}");
			tw.WriteLine("catch (Exception ex)");
			tw.WriteLine("{");
			tw.WriteLine("U.Log().Err(ex, $\"{Year}/{Month}/{Day}-{Hour}:{Minutes}:{Seconds}\");");
			tw.WriteLine("throw;");
			tw.WriteLine("}");
			tw.WriteLine("}");

			tw.WriteLine("}");

			if ((csFlags & CsFlags.Messages) == CsFlags.Messages)
			{
				var enumList = new List<string>();
				foreach (var en in from v in dg where v.IsEnum select (EnumDecl)v)
				{
					enumList.Add(en.ID.strRead);
					tw.WriteLine("public enum {0}", en.ID.strRead);
					tw.WriteLine("{");
					foreach (var em in en.List)
						tw.WriteLine("{0} = {1},", em.ID.strRead, em.NUM.strRead);
					tw.WriteLine("}");
					tw.WriteLine("////////////////////////");
				}
				foreach (var en in from v in dg where v.IsMessage select (MessageDecl)v)
				{
					tw.WriteLine("public partial class {0} : U.PB.IPbObject", en.ID.strRead);
					tw.WriteLine("{");

					int ws_order = 0;

					foreach (var em in en.Fields)
					{
						if (em.IsOneOf)
						{
							var r = em as OneOf;

							/*
							 * oneof viene tradotta in soap/wfc creando una classe base per ogni oneof
							 * tante classi derivate quante sono le possibilità di oneof
							 * e sfruttando KnownType
							 */
							foreach (var er in r.List)
							{
								if (er.TYPE.strRead == "DateTime_t")
								{
									tw.WriteLine($"public bool IsSet_{er.varName()} => _tag_{r.varName()} == {er.tag()};");
									tw.WriteLine($"public {er.csType()} {er.varName()}");
									tw.WriteLine("{");
									tw.WriteLine($"get => IsSet_{er.varName()} ? ({er.csType()})_{r.varName()} : default({er.csType()});");
									tw.WriteLine($"set {{ _{r.varName()} = value; _tag_{r.varName()} = _{r.varName()} != null ? {er.tag()} : 0; }}");
									tw.WriteLine("}");
								}
								else if (!er.isEnum(enumList))
								{
									tw.WriteLine($"public bool IsSet_{er.varName()} => _tag_{r.varName()} == {er.tag()};");
									tw.WriteLine($"public {er.csType()} {er.varName()}");
									tw.WriteLine("{");
									tw.WriteLine($"get => IsSet_{er.varName()} ? ({er.csType()})_{r.varName()} : default({er.csType()});");
									tw.WriteLine($"set {{ _{r.varName()} = value; _tag_{r.varName()} = value != null ? {er.tag()} : 0; }}");
									tw.WriteLine("}");
								}
								else
								{
									tw.WriteLine($"public bool IsSet_{er.varName()} => _tag_{r.varName()} == {er.tag()};");
									tw.WriteLine($"public {er.csType()} {er.varName()}");
									tw.WriteLine("{");
									tw.WriteLine($"get => IsSet_{er.varName()} ? ({er.csType()})_{r.varName()} : default({er.csType()});");
									tw.WriteLine($"set {{ _{r.varName()} = value; _tag_{r.varName()} = value != null ? {er.tag()} : 0; }}");
									tw.WriteLine("}");
								}
							}
						}
						else if (em.IsOptional)
						{
							var r = em as Optional;

							if (r.TYPE.strRead == "DateTime_t")  // in PB la data è una classe ----> è opzionale SEMPRE
							{
								tw.WriteLine($"public {r.csType()} {r.varName()} {{ get => _{r.varName()}; set => _{r.varName()} = value; }}");
							}
							else if (!r.isEnum(enumList))
							{
								// DataMember(IsRequired=false, EmitDefaultValue=true) <== è il DEFAULT di .NET
								//
								// è un tipo qualunque MA non enum.
								// In PB sono tutti opzionali.
								// In WS .... IsRequired=false ==> se manca leggo uno 0 per i numerici
								//            IsRequired=true  ==> non potrà mai mancare. Per essere coerenti qui dovremmo avere SEMPRE IsRequired=false
								// WS in scrittura ==> se EmitDefaultValue=true (che è il default) lo scrive sempre al limite con nil
								//    anche qui per essere coerenti dovremmo EmitDefaultValue=false. Ma la cosa è sconsigliata su MSDN perchè dicono che non è interoperabile
								tw.WriteLine($"public {r.csType()} {r.varName()} {{ get => _{r.varName()}; set => _{r.varName()} = value; }}");
							}
							else
							{
								// è un ENUM. 
								// In PB avrà valore 0 se non letto. Non si scrive se è 0 (il tutto a prescindere dai valori ammessi dall'enum)
								// In WS se leggo un nil=true ==> devo avere uno 0 sulla variabile.
								// Se non 
								tw.WriteLine($"public {r.csType()} {r.varName()} {{ get => _{r.varName()}; set => _{r.varName()} = value; }}");
							}
						}
						else if (em.IsRepeated)
						{
							var r = em as Repeated;

							if (r.TYPE.strRead == "DateTime_t")
							{
								tw.WriteLine($"public {r.csType()} {r.varName()} {{ get => _{r.varName()}; set => _{r.varName()} = value; }}");
							}
							else if (!r.isEnum(enumList))
							{
								tw.WriteLine($"public {r.csType()} {r.varName()} {{ get => _{r.varName()}; set => _{r.varName()} = value; }}");
							}
							else
							{
								tw.WriteLine($"public {r.csType()} {r.varName()} {{ get => _{r.varName()}; set => _{r.varName()} = value; }}");
							}
						}
						ws_order += 10;
					}
					tw.WriteLine();

					tw.WriteLine("#region PB");
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
								tw.WriteLine($"{rp.varName()} = null;");
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

							tw.WriteLine();
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
					tw.WriteLine("#endregion //PB");


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
					tw.WriteLine($"public partial class {s.Name.strRead}Client : U.PB.PbClientBase");
					tw.WriteLine("{");
					tw.WriteLine($"public {s.Name.strRead}Client(string addr) : base(addr, null, \"V2\") {{}}");
					tw.WriteLine($"public {s.Name.strRead}Client(string addr, string version) : base(addr, null, version) {{}}");
					tw.WriteLine($"public {s.Name.strRead}Client(string addr, HttpMessageHandler handler, string version = \"V2\") : base(addr, handler, version) {{}}");
					foreach (var f in s.Fun)
						tw.WriteLine($"public {f.Res.strRead} {f.Name.strRead}({f.Req.strRead} req) => PbCall<{f.Req.strRead}, {f.Res.strRead}>(\"{pkgName}\", \"{s.Name.strRead}\", \"{f.Name.strRead}\", req);");
					tw.WriteLine("}");
				}
			}
			if ((csFlags & CsFlags.ClientPbCallAsync) == CsFlags.ClientPbCallAsync)
			{
				tw.WriteLine();
				tw.WriteLine("//////////////////////");
				foreach (var s in from v in dg where v.IsService select (ServiceDecl)v)
				{
					tw.WriteLine($"public partial class {s.Name.strRead}ClientAsync : U.PB.PbClientBase");
					tw.WriteLine("{");
					tw.WriteLine($"public {s.Name.strRead}ClientAsync(string addr) : base(addr, null, \"V2\") {{}}");
					tw.WriteLine($"public {s.Name.strRead}ClientAsync(string addr, string version) : base(addr, null, version) {{}}");
					tw.WriteLine($"public {s.Name.strRead}ClientAsync(string addr, HttpMessageHandler handler, string version = \"V2\") : base(addr, handler, version) {{}}");
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
					tw.WriteLine($"public interface I{s.Name.strRead}");
					tw.WriteLine("{");
					foreach (var f in s.Fun)
					{
						tw.Write($"[U.PB.PbMethod(\"{f.Name.strRead}\")] ");
						tw.WriteLine($"Task<{f.Res.strRead}> {f.Name.strRead}({f.Req.strRead} req);");
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

		private static void GenCS_WS(DeclList dg, U.CsStreamWriter tw, CsFlags csFlags)
		{
			tw.WriteLine("// Generated by LLProtoBuff. DO NOT MODIFY");
			tw.WriteLine();

			tw.WriteLine("using System;");
			tw.WriteLine("using System.Collections.Generic;");
			tw.WriteLine("using System.Threading.Tasks;");
			tw.WriteLine("using System.Runtime.Serialization;");
			tw.WriteLine("using System.ServiceModel;");
			tw.WriteLine();

			var pkgName = "";
			if (true)
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
				tw.WriteLine("namespace {0}WS", pkgName);
				tw.WriteLine("{");
			}

			if (true)
			{
				tw.WriteLine("public static class Constants");
				tw.WriteLine("{");
				tw.WriteLine("public const string Namespace = \"http://schemas.datacontract.org/2004/07/ET-Mobile\";");
				tw.WriteLine("}");
				tw.WriteLine();
			}

			if ((csFlags & CsFlags.Messages) == CsFlags.Messages)
			{
				var enumList = new List<string>();
				foreach (var en in from v in dg where v.IsEnum select (EnumDecl)v)
					enumList.Add(en.ID.strRead);

				foreach (var en in from v in dg where v.IsMessage select (MessageDecl)v)
				{
					if (true)
					{
						// per ogni campo OneOf si dichiarano tanti classi derivata da una classe comunue.
						foreach (var of in from v in en.Fields where v.IsOneOf select (OneOf)v)
						{
							// classi comune
							tw.WriteLine("[DataContract(Namespace = Constants.Namespace)]");
							tw.WriteLine($"public class {en.ID.strRead}_{of.varName()}_t");
							tw.WriteLine("{");
							tw.WriteLine("}");
							tw.WriteLine();

							// classe derivata.
							foreach (var m in of.List)
							{
								tw.WriteLine("[DataContract(Namespace = Constants.Namespace)]");
								tw.WriteLine($"public class {en.ID.strRead}_{of.varName()}_{m.varName()}_t : {en.ID.strRead}_{of.varName()}_t");
								tw.WriteLine("{");
								tw.Write($"[DataMember(IsRequired = true)] ");
								tw.WriteLine($"public {m.wsType(enumList)} {m.varName()} {{ get; set; }}");
								tw.WriteLine("}");
								tw.WriteLine();
							}
						}

						// nella classe che dichiara il campo OneOf, si dichiarano i KnownType
						foreach (var of in from v in en.Fields where v.IsOneOf select (OneOf)v)
						{
							tw.WriteLine($"[KnownType(typeof({en.ID.strRead}_{of.varName()}_t))]");
							foreach (var m in of.List)
								tw.WriteLine($"[KnownType(typeof({en.ID.strRead}_{of.varName()}_{m.varName()}_t))]");
						}
					}
					tw.WriteLine("[DataContract(Namespace = Constants.Namespace)]"); 
					tw.WriteLine("public partial class {0}", en.ID.strRead);
					tw.WriteLine("{");

					int ws_order = 0;
					foreach (var em in en.Fields)
					{
						if (em.IsOneOf)
						{
							var r = em as OneOf;
							// oneof viene tradotta in soap / wfc creando una classe base per ogni oneof
							// tante classi derivate quante sono le possibilità di oneof
							// e sfruttando KnownType
							tw.Write($"[DataMember(Order = {ws_order}, IsRequired = false)] ");
							var wsBaseClass = $"{en.ID.strRead }_{r.varName()}_t";
							tw.WriteLine($"public {wsBaseClass} {r.varName()} {{ get; set; }}");
						}
						else if (em.IsOptional)
						{
							// i campi opzionali prendono il valore di default se non vengono letti dello stream
							// per cui NON sono nullabili.
							// L'unico campo nullabile è il DateTime perchè in PB è una classe che può essere nullabile

							var r = em as Optional;

							if (r.TYPE.strRead == "DateTime_t")  // in PB la data è una classe ----> è opzionale SEMPRE
							{
								tw.Write($"[DataMember(Order = {ws_order}, IsRequired = false)] ");
								tw.WriteLine($"public DateTime ? {r.varName()} {{ get; set; }}");
							}
							else if (!r.isEnum(enumList))
							{
								tw.Write($"[DataMember(Order = {ws_order}, IsRequired = false)] ");
								tw.WriteLine($"public {r.wsType(enumList)} {r.varName()} {{ get; set; }}");
							}
							else
							{
								// è un ENUM. 
								// In PB avrà valore 0 se non letto. Non si scrive se è 0 (il tutto a prescindere dai valori ammessi dall'enum)
								// In WS se leggo un nil=true ==> devo avere uno 0 sulla variabile.
								// Se non 
								tw.Write($"[DataMember(Order = {ws_order}, IsRequired = false)] ");
								tw.WriteLine($"public int {r.varName()} {{ get; set; }}");
							}
						}
						else if (em.IsRepeated)
						{
							// un campo ripetuto se non è presente nello stream lascia l'array null
							// mentre se arriva qualcosa, quel qualcosa non è null
							var r = em as Repeated;

							if (r.TYPE.strRead == "DateTime_t")
							{
								tw.Write($"[DataMember(Order = {ws_order}, IsRequired = false)] ");
								tw.WriteLine($"public DateTime[] {r.varName()} {{ get; set; }}");
							}
							else if (!r.isEnum(enumList))
							{
								tw.Write($"[DataMember(Order = {ws_order}, IsRequired = false)] ");
								tw.WriteLine($"public {r.wsType(enumList)} {r.varName()} {{ get; set; }}");
							}
							else
							{
								tw.Write($"[DataMember(Order = {ws_order}, IsRequired = false)] ");
								tw.WriteLine($"public int[] {r.varName()} {{ get; set; }}");
							}
						}
						ws_order += 10;
					}
					tw.WriteLine("}");
					tw.WriteLine("////////////////////////");
					tw.WriteLine();
				}
			}

			if ((csFlags & CsFlags.ServiceInterfaceAsync) == CsFlags.ServiceInterfaceAsync)
			{
				tw.WriteLine();
				tw.WriteLine("////////////////////");
				foreach (var s in from v in dg where v.IsService select (ServiceDecl)v)
				{
					tw.WriteLine("[ServiceContract(Namespace = Constants.Namespace)]");
					tw.WriteLine($"public interface I{s.Name.strRead}");
					tw.WriteLine("{");
					foreach (var f in s.Fun)
					{
						tw.Write("[OperationContract] ");
						tw.WriteLine($"Task<{f.Res.strRead}> {f.Name.strRead}({f.Req.strRead} req);");
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
					tw.WriteLine("[ServiceContract(Namespace = Constants.Namespace)]");
					tw.WriteLine($"public interface I{s.Name.strRead}");
					tw.WriteLine("{");
					foreach (var f in s.Fun)
					{
						tw.Write("[OperationContract] ");
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
			ClientRestCall = 4,
			PbCallStub = 8,

			ClientPbCall2 = 16,
		}

		private static void GenHPP(DeclList dg, U.CsStreamWriter tw, string fileHpp, CppFlags cppFlags, string dll)
		{

			tw.WriteLine("// Generated by LLProtoBuff. DO NOT MODIFY");
			tw.WriteLine();
			tw.WriteLine("#ifndef __{0}__", fileHpp.ToUpper().Replace('.', '_'));
			tw.WriteLine("#define __{0}__", fileHpp.ToUpper().Replace('.', '_'));
			tw.WriteLine();

			string api = "";
			string protoClient;

			if ((cppFlags & CppFlags.ClientPbCall2) == CppFlags.ClientPbCall2)
			{
				protoClient = "U::ProtoClient";

				tw.WriteLine("#include <U/U_Pb.h>");
				tw.WriteLine("#include <U/U_pbClient.hpp>");
				tw.WriteLine();
				if (!string.IsNullOrEmpty(dll))
				{
					tw.WriteLine();
					tw.WriteLine($"#ifdef {dll}_DLL");
					tw.WriteLine($"#	if defined({dll}_BUILD)");
					tw.WriteLine($"#		define {dll}_API U_EXPORT");
					tw.WriteLine($"#	else");
					tw.WriteLine($"#		define {dll}_API U_IMPORT");
					tw.WriteLine($"#	endif");
					tw.WriteLine($"#else");
					tw.WriteLine($"#	define {dll}_API");
					tw.WriteLine($"#endif");
					tw.WriteLine();
					api = $"{dll}_API ";
				}
			}
			else
			{
				protoClient = "ProtoClient";

				tw.WriteLine("#include \"Pb.h\"");
				if ((cppFlags & CppFlags.ClientPbCall) == CppFlags.ClientPbCall)
					tw.WriteLine("#include \"pbClient.h\"");
				tw.WriteLine();
				api = $"PB_API ";
			}
			var pkg = dg.FirstOrDefault(e => e.IsPackage);
			if (pkg != null)
			{
				tw.WriteLine("namespace {0}", ((PackageDecl)pkg).Str.strRead);
				tw.WriteLine("{");

				tw.WriteLine("using namespace U;");
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
					tw.WriteLine("#if __cuplusplus > 201103");
					if (!en.ID.strRead.EndsWith("_t"))
						tw.WriteLine("enum class {0}_t", en.ID.strRead);
					else
						tw.WriteLine("enum class {0}", en.ID.strRead);
					tw.WriteLine("{");
					foreach (var em in en.List)
						tw.WriteLine("{0} = {1},", em.ID.strRead, em.NUM.strRead);
					tw.WriteLine("};");

					tw.WriteLine("#else");
					if (!en.ID.strRead.EndsWith("_t"))
						tw.WriteLine("enum {0}_t", en.ID.strRead);
					else
						tw.WriteLine("enum {0}", en.ID.strRead);
					tw.WriteLine("{");
					foreach (var em in en.List)
						tw.WriteLine("{0}_{1} = {2},", en.ID.strRead, em.ID.strRead, em.NUM.strRead);
					tw.WriteLine("};");
					tw.WriteLine("#endif");
					tw.WriteLine();
					tw.WriteLine("////////////////////////");
				}
				tw.WriteLine();

				foreach (var en in from v in dg where v.IsMessage select (MessageDecl)v)
				{
					var className = en.ID.strRead;

					if (className == "DateTime_t") tw.WriteLine("// rappresenta una data/ora in local time");
					tw.WriteLine($"class {api}{className} : public PbObject");
					tw.WriteLine("{");
					tw.WriteLine("public:");
					tw.WriteLine($"{className}();");
					tw.WriteLine($"~{className}();");
					tw.WriteLine($"{className}(const {className} &);");
					tw.WriteLine($"void operator =(const {className} &);");
					tw.WriteLine($"void Clear();");
					tw.WriteLine($"void Write(PbStreamOut &w);");
					tw.WriteLine($"void Read(PbStreamIn &r);");

					if ((cppFlags & CppFlags.ClientRestCall) > 0)
					{
						tw.WriteLine($"void R(JlsObject &r);");
						tw.WriteLine($"void W(JlsObject &r) const;");
					}


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

			if ((cppFlags & CppFlags.ClientPbCall) == CppFlags.ClientPbCall ||
				(cppFlags & CppFlags.ClientPbCall2) == CppFlags.ClientPbCall2)
			{
				foreach (var s in from v in dg where v.IsService select (ServiceDecl)v)
				{
					tw.WriteLine($"class {s.Name.strRead}Client : public virtual {protoClient}");
					tw.WriteLine("{");
					tw.WriteLine("public:");
					tw.WriteLine($"{s.Name.strRead}Client(const std::string &url) : {protoClient}(url) {{}}");
					tw.WriteLine($"{s.Name.strRead}Client(const std::map<std::string, std::string> &url) : {protoClient}(url) {{}}");
					foreach (var f in s.Fun)
					{
						tw.WriteLine($"void {f.Name.strRead}({f.Req.strRead} &req, {f.Res.strRead} &res, bool rest = false) {{ Exec(\"{s.Name.strRead}\", \"{f.Name.strRead}\", req, res, rest); }};");
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

					// costruttore
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

					// distruttore
					if (true)
					{
						tw.WriteLine($"{className}::~{className}()");
						tw.WriteLine("{");
						tw.WriteLine("Clear();");
						tw.WriteLine("}");
					}

					// Clear
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
									tw.WriteLine($"for (size_t i = 0; i < {r.varName()}.size(); ++i) delete {r.varName()}[i];");
								tw.WriteLine(r.cppInit());
							}
						}
						tw.WriteLine("}");
					}

					// Write(PbStreamOut &w)
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
										tw.WriteLine($"w.Write({er.tag()}, pb_{er.pbBaseType()}, {er.varName()});");
									else
										tw.WriteLine($"w.Write({er.tag()}, pb_{er.pbBaseType()}, (int){er.varName()});");
								}
							}
							else if (em.IsOptional)
							{
								var r = em as Optional;
								if (!r.isEnum(enumList))
									tw.WriteLine($"w.Write({r.tag()}, pb_{r.pbBaseType()}, {r.varName()});");
								else
									tw.WriteLine($"w.Write({r.tag()}, pb_{r.pbBaseType()}, (int){r.varName()});");
							}
							else if (em.IsRepeated)
							{
								var r = em as Repeated;
								if (!r.isEnum(enumList))
									tw.WriteLine($"w.Write({r.tag()}, pb_{r.pbBaseType()}, {r.varName()});");
								else
									tw.WriteLine($"w.Write({r.tag()}, pb_{r.pbBaseType()}, (int){r.varName()});");
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

						tw.WriteLine($"{className}::{className}(const {className} &r)");
						tw.WriteLine("{");
						tw.WriteLine("PbStreamOut so;");
						tw.WriteLine($"const_cast<{className} &>(r).Write(so);");
						tw.WriteLine("PbStreamIn si(so.ToArray());");
						tw.WriteLine("this->Read(si);");
						tw.WriteLine("}");
						tw.WriteLine($"void {className}::operator =(const {className} &r)");
						tw.WriteLine("{");
						tw.WriteLine("PbStreamOut so;");
						tw.WriteLine($"const_cast<{className} &>(r).Write(so);");
						tw.WriteLine("PbStreamIn si(so.ToArray());");
						tw.WriteLine("this->Read(si);");
						tw.WriteLine("}");

						if ((cppFlags & CppFlags.ClientRestCall) > 0)
						{
							tw.WriteLine($"void {className}::R(JlsObject &r)");
							tw.WriteLine("{");
							foreach (var em in en.Fields)
							{
								if (em.IsOneOf)
								{
									var r = em as OneOf;
									foreach (var er in r.List)
										tw.WriteLine($"r.Read(\"{er.varName()}\", {er.varName()});");
								}
								else if (em.IsOptional)
								{
									var r = em as Optional;
									tw.WriteLine($"r.Read(\"{r.varName()}\", {r.varName()});");
								}
								else if (em.IsRepeated)
								{
									var r = em as Repeated;
									tw.WriteLine($"r.Read(\"{r.varName()}\", {r.varName()});");
								}
							}

							tw.WriteLine("}");
							tw.WriteLine($"void {className}::W(JlsObject &r) const");
							tw.WriteLine("{");
							foreach (var em in en.Fields)
							{
								if (em.IsOneOf)
								{
									var r = em as OneOf;
									foreach (var er in r.List)
										tw.WriteLine($"r.Write(\"{er.varName()}\", {er.varName()});");
								}
								else if (em.IsOptional)
								{
									var r = em as Optional;
									tw.WriteLine($"r.Write(\"{r.varName()}\", {r.varName()});");
								}
								else if (em.IsRepeated)
								{
									var r = em as Repeated;
									tw.WriteLine($"r.Write(\"{r.varName()}\", {r.varName()});");
								}
							}
							tw.WriteLine("}");
						}

						if (true)
						{
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
											tw.WriteLine($"case {er.tag()}: r.Read(wt, pb_{er.pbBaseType()}, {er.varName()}); {r.varName()} = {er.tag()}; break;");
										else
											tw.WriteLine($"case {er.tag()}: r.Read(wt, pb_{er.pbBaseType()}, en); {er.varName()} = ({er.TYPE.strRead}) en; {r.varName()} = {er.tag()}; break;");
									}
								}
								else if (em.IsOptional)
								{
									var r = em as Optional;
									if (!r.isEnum(enumList))
										tw.WriteLine($"case {r.tag()}: r.Read(wt, pb_{r.pbBaseType()}, {r.varName()}); break;");
									else
										tw.WriteLine($"case {r.tag()}: r.Read(wt, pb_{r.pbBaseType()}, en); {r.varName()} = ({r.cppType(enumList)}) en; break;");
								}
								else if (em.IsRepeated)
								{
									var r = em as Repeated;
									if (!r.isEnum(enumList))
										tw.WriteLine($"case {r.tag()}: r.Read(wt, pb_{r.pbBaseType()}, {r.varName()}); break;");
									else
										tw.WriteLine($"case {r.tag()}: r.Read(wt, pb_{r.pbBaseType()}, en); {r.varName()} = ({r.cppType(enumList)}) en; break;");

								}
							}
							tw.WriteLine("default: r.Skip(wt); break;");
							tw.WriteLine("}");
							tw.WriteLine("}");
							tw.WriteLine("}");
						}
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
