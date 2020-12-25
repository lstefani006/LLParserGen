using LLParserLexerLib
	;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LLCLeo
{
	public partial class MParser
	{
		public MParser() : base(0) { }

		public DeclRootList Parse(LexReader rd)
		{
			this.init(rd);
			var v = this.start(null);
			return v;
		}
	}
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var fileInput = args[0];

				using (var rd = new LexReader(fileInput))
				{
					var p = new MParser();
					var declList = p.Parse(rd);

					declList.Add(new DeclNative("int"));
					declList.Add(new DeclNative("void"));
					declList.Add(new DeclNative("bool"));

					declList.SetContainer(null);

					using (var fout = File.CreateText("LL.c"))
					{
						var wr = new U.CsStreamWriter(fout);

						foreach (var dc in declList)
							dc.Declare(wr, 0);
						wr.WriteLine();
						foreach (var dc in declList)
							dc.Declare(wr, 1);
						wr.WriteLine();

						foreach (var dc in declList)
							dc.Declare(wr);
						wr.WriteLine();

						foreach (var dc in declList)
							dc.Define(wr);
						wr.WriteLine();

					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

	}

	public static class DeclExtension
	{
		public static void SetContainer(this DeclRoot dc, DeclRoot cont)
		{
			dc.container = cont;
			switch (dc)
			{
			case DeclClass a:
				foreach (var f in a.s) f.SetContainer(a);
				break;

			case DeclNamespace a:
				foreach (var f in a.s) f.SetContainer(a);
				break;

			case DeclFunction f:
				f.ret.Container = cont;
				f.ag.ForEach(a => a.type.Container = cont);
				break;

			case DeclVar a:
				if (a.type != null)
					a.type.Container = cont;
				break;

			case DeclDelegate _:
				break;

			case DeclNative n:
				break;

			case DeclRootList r:
				foreach (var d in r) d.SetContainer(r);
				break;

			default:
				Debug.Assert(false);
				break;
			}
		}


		public static void FunDeclare(this DeclFunction f, U.CsStreamWriter wr)
		{
			var className = f.container.CName;
			var funName = f.CName;

			var ctype = f.Resolve(f.ret).CName;

			wr.Write($"{ctype} {funName}({className} *self");
			foreach (var a in f.ag)
				wr.Write($", {f.Resolve(a.type).CName} {a.var.strRead}");
			wr.WriteLine(");");
		}
		public static void Declare(this DeclRoot dc, U.CsStreamWriter wr, int level)
		{
			switch (dc)
			{
			case DeclClass c:
				if (level == 0)
				{
					wr.WriteLine($"struct {c.CName};");
					wr.WriteLine($"typedef struct {c.CName} {c.CName};");
					foreach (var dic in c.s)
						dic.Declare(wr, level);
				}
				else if (level == 1)
				{
					wr.WriteLine($"{c.CName} * {c.CName}__New({c.CName} *self);");
					wr.WriteLine($"void {c.CName}__Free({c.CName} *self);");
					foreach (var dic in c.s)
						switch (dic)
						{
						case DeclFunction f:
							f.FunDeclare(wr);
							break;
						}
				}
				break;

			case DeclNamespace ns:
				foreach (var dic in ns.s)
					dic.Declare(wr, level);
				break;

			case DeclDelegate del:
				{
					if (level == 0)
					{
						wr.WriteLine($"struct {del.CName};");
						wr.WriteLine($"typedef struct {del.CName} {del.CName};");
					}
				}
				break;

			case DeclVar _:
			case DeclFunction _:
			case DeclNative _:
				break;

			default:
				Debug.Assert(false);
				break;
			}
		}

		public static void Define(this DeclRoot dc, U.CsStreamWriter wr)
		{
			switch (dc)
			{
			case DeclClass c:
				foreach (var dic in c.s)
					dic.Define(wr);
				break;

			case DeclNamespace ns:
				foreach (var dic in ns.s)
					dic.Define(wr);
				break;

			case DeclVar v:
				{
					if (!(v.container is DeclClass))
					{
						var ctype = v.Resolve(v.type);
						wr.WriteLine($"{ctype.CName} {v.CName} = 0;");
					}
				}
				break;

			case DeclFunction f:
				{
					var ret = f.Resolve(f.ret).CName;

					if (!(f.container is DeclClass))
					{
						wr.Write($"{ret} {f.CName}(").WriteComma(f.ag, a => $"{f.Resolve(a.type).CName} {a.var.strRead}").WriteLine(")");
						wr.WriteLine("{");
						wr.WriteLine("}");
					}
					else
					{
						var c = f.container as DeclClass;
						wr.Write($"{ret} {c.CName}__{f.tk.strRead}(");
						f.ag.Comma(wr, $"{c.CName} *self", a => $"{f.Resolve(a.type).CName} {a.var.strRead}");
						wr.WriteLine(")");
						wr.WriteLine("{");
						wr.WriteLine("}");
					}
				}
				break;

			case DeclDelegate del:
				{
					//var ret = del.Resolve(del.ret).CName;


				}
				break;

			case DeclNative n:
				//wr.WriteLine($"{n.tk.strRead} {n.CName} = 0;");
				break;

			default:
				Debug.Assert(false);
				break;
			}

		}

		public static void Comma<T>(this List<T> self, U.CsStreamWriter wr, Func<T, string> a)
		{
			for (int i = 0; i < self.Count; ++i)
			{
				wr.Write(a(self[i]));
				if (i < self.Count - 1) wr.Write(", ");
			}
		}
		public static void Comma<T>(this List<T> self, U.CsStreamWriter wr, string pre, Func<T, string> a)
		{
			wr.Write(pre);
			for (int i = 0; i < self.Count; ++i)
			{
				wr.Write(", ");
				wr.Write(a(self[i]));
			}
		}


		public static DeclRoot Resolve(this DeclRoot decl, TypeRoot type)
		{
			var s = new List<(string CName, DeclRoot Decl)>();

			switch (decl)
			{
			case DeclNamespace n:
				foreach (var t in n.s) { t.CRelativeName("", s); }
				break;
			case DeclClass n:
				foreach (var t in n.s) { t.CRelativeName("", s); }
				break;
			case DeclRootList n:
				foreach (var t in n) { t.CRelativeName("", s); }
				break;
			}

			var idx = s.FindIndex(r => r.CName == type.CName);
			if (idx >= 0)
				return s[idx].Decl;

			if (decl.container == null)
				throw new Exception($"{type.tk} - type not found");
			return decl.container.Resolve(type);
		}

		public static void Declare(this DeclRoot dc, U.CsStreamWriter wr)
		{
			switch (dc)
			{
			case DeclClass c:
				wr.WriteLine($"struct {c.CName}");
				wr.WriteLine("{");
				foreach (var dic in c.s)
					switch (dic)
					{
					case DeclVar v:
						{
							var ct = v.Resolve(v.type);
							wr.WriteLine($"{ct.CName} {v.tk.strRead};");
						}
						break;
					}
				wr.WriteLine("};");

				// eventuali classi dichiarate internamente alla classe
				foreach (var dic in c.s)
					dic.Declare(wr);
				break;

			case DeclNamespace ns:
				foreach (var dic in ns.s)
					dic.Declare(wr);
				break;

			case DeclDelegate f:
				{
					wr.WriteLine($"struct {f.CName}");
					wr.WriteLine("{");
					wr.WriteLine("void *obj;");

					var cr = f.Resolve(f.ret).CName;
					wr.WriteLine("union");
					wr.WriteLine("{");
					wr.Write($"{cr}(*cbm)(").WriteComma("void *self", f.args, a => $"{f.Resolve(a.type).CName} {a.var.strRead}").WriteLine(");");
					wr.Write($"{cr}(*cbs)(").WriteComma(f.args, a => $"{f.Resolve(a.type).CName} {a.var.strRead}").WriteLine(");");
					wr.WriteLine("};");
					wr.WriteLine("};");
				}
				break;

			case DeclVar _:
			case DeclFunction _:
				break;

			case DeclNative n:
				// i tipi nativi non bisogna dichiararli
				break;

			default:
				Debug.Assert(false);
				break;
			}
		}
	}
}
