using System;
using System.Diagnostics;
using LLParserLexerLib;
using System.Collections.Generic;
using System.Text;

namespace LLParserGenTest
{
	class _
	{
		public static void Main(string[] args)
		{
			using (var rd = (args.Length == 1 ? new LexReader(args[0]) : new LexReader(Console.In, "stdin")))
			{
				var b = new int[3][,][,,];
				b[2] = new int[2,2][,,];
				try
				{
					var p = new MParser();
					var dg = p.Start(rd);

					dg.SetFather(null);

					dg.CheckSyntax(dg);

					using (Context ctx = new Context(dg))
					{
						dg.GenCode(ctx);

						Console.WriteLine();
						Console.WriteLine("########################");
						Console.WriteLine(ctx.ToString());
					}
				}
				catch (SyntaxError ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}
	}

	public enum TypeBase
	{
		None,

		Bool,
		Char,
		Int,
		Handle,
		Double,
		Void,

		Obj,
	}

	public abstract class RefTypeRoot : IAST
	{
		protected DeclRoot _dr;

		public abstract TypeBase TypeBase { get; }

		public override bool Equals(object obj) { if (obj is RefTypeRoot) return this == (obj as RefTypeRoot); else return false; }
		public override int GetHashCode() { return base.GetHashCode(); }

		public abstract bool ResolveType(DeclRoot dr);
		public abstract string AssName { get; }
		public DeclRoot DeclRoot { get { return _dr; } }

		public static RefTypeSimple Void;
		public static RefTypeSimple Bool;
		public static RefTypeSimple Char;
		public static RefTypeSimple Int;
		public static RefTypeSimple Double;
		public static RefTypeSimple Handle;

		public static bool operator ==(RefTypeRoot a, RefTypeRoot b)
		{
			if (((object)a) == null && (object)b == null) return true;
			if (((object)a) == null && ((object)b) != null) return false;
			if (((object)a) != null && ((object)b) == null) return false;
			return a.AssName == b.AssName;
		}
		public static bool operator !=(RefTypeRoot a, RefTypeRoot b) { return !(a == b); }
	}

	public class RefTypeSimple : RefTypeRoot
	{
		List<string> id = new List<string>();

		public RefTypeSimple(TokenAST id) { Debug.Assert(this.id.Count == 0);  this.id.Add(id.strRead); }
		public RefTypeSimple(string id) { Debug.Assert(this.id.Count == 0); this.id.Add(id); }

		public RefTypeSimple Add(string id) { this.id.Add(id); return this; }
		public RefTypeSimple Add(TokenAST id) { return Add(id.strRead); }

		public override TypeBase TypeBase
		{
			get
			{
				string v = _dr.AssName;
				if (v == "$v") return LLParserGenTest.TypeBase.Void;
				if (v == "$b") return LLParserGenTest.TypeBase.Bool;
				if (v == "$c") return LLParserGenTest.TypeBase.Char;
				if (v == "$i") return LLParserGenTest.TypeBase.Int;
				if (v == "$f") return LLParserGenTest.TypeBase.Double;
				if (v == "$h") return LLParserGenTest.TypeBase.Handle;
				return TypeBase.Obj;
			}
		}

		public override string ToString() { 
			var s = new StringBuilder();
			foreach (var r in this.id)
			{
				if (s.Length > 0) s.Append(".");
				s.Append(r);
			}
			return s.ToString();
		}

		public override bool ResolveType(DeclRoot dr)
		{
			if (_dr != null) return true;
			_dr = dr.GetDecl(this.id, 0);
			return _dr != null;
		}

		public override string AssName { get { return _dr.AssName; } }
	}

	public class RefTypeArray : RefTypeRoot
	{
		public RefTypeArray(RefTypeRoot t, int rank) { this.t = t; this.rank = rank;  }
		public readonly RefTypeRoot t;
		public readonly int rank;
		public override TypeBase TypeBase { get { return TypeBase.Obj; } }

		public override bool ResolveType(DeclRoot dr) { return t.ResolveType(dr); }

		public override string ToString() { return t.ToString() + "[]"; }
		public override string AssName { get { return "$a" + t.AssName; } }	
	}


	//public class TypeFun : TypeRoot
	//{
	//	public TypeFun(DeclRoot t, string member) { this.t = t; this.member = member; }
	//	public readonly DeclRoot t;
	//	public readonly string member;
	//	public override bool IsFun { get { return true; } }

	//	protected override bool TypeEqual(TypeRoot t)
	//	{
	//		var at = t as TypeFun;
	//		if ((object)at != null)
	//			return this.t == at.t && this.member != at.member;
	//		return false;
	//	}

	//	public override string ToString() { return t.ToString() + "." + member; }
	//	public override string AssName { get { return "$f"; } }
	//}

	public class RefTypeRootList : List<RefTypeRoot>, IAST
	{
		public new RefTypeRootList Add(RefTypeRoot a) { base.Add(a); return this; }
	}

	public class RefTypeRoot_or_Base : IAST
	{
		public readonly RefTypeRoot t;
		public readonly BaseInit b;
		public RefTypeRoot_or_Base(RefTypeRoot t) { this.t = t; }
		public RefTypeRoot_or_Base(BaseInit b) { this.b = b; }
	}
	public class BaseInit {
		public readonly TokenAST t;
		public readonly ExprList e;
		public BaseInit(TokenAST t, ExprList e) { this.t = t; this.e = e; }
	}


	/// <summary>
	/// Descrive un tipo e il valore costante eventualmente associato.
	/// </summary>
	public class ExprType
	{
		public ExprType(RefTypeRoot t) { this._type = t; }
		public ExprType(bool c) { this._const = c; this._type = RefTypeSimple.Bool; }
		public ExprType(int c) { this._const = c; this._type = RefTypeSimple.Int; }
		public ExprType(char c) { this._const = c; this._type = RefTypeSimple.Char; }
		public ExprType(double d) { this._const = d; this._type = RefTypeSimple.Double; }
		public ExprType(Null obj, RefTypeRoot r) { this._const = obj; this._type = r; }

		readonly object _const;
		readonly RefTypeRoot _type;

		public RefTypeRoot Type { get { return _type; } }

		public bool IsNumeric { get { return _type.TypeBase == TypeBase.Int || _type.TypeBase == TypeBase.Double; } }
		public bool IsBool { get { return _type.TypeBase == TypeBase.Bool; } }

		public TypeBase TypeBase { get { return _type.TypeBase; } }

		// costanti
		public bool IsConst { get { return _const != null; } }
		public int Int { get { Debug.Assert(TypeBase == TypeBase.Int); return (int)_const; } }
		public double Dbl { get { Debug.Assert(TypeBase == TypeBase.Double); return (double)_const; } }
		public bool Bool { get { Debug.Assert(TypeBase == TypeBase.Bool); return (bool)_const; } }
		public char Char { get { Debug.Assert(TypeBase == TypeBase.Char); return (char)_const; } }

		public override string ToString()
		{
			string s = _type.ToString();
			if (this._const != null) s += " (" + this._const.ToString() + ")";
			return s;
		}

		private static int cmp(ExprType a, ExprType b)
		{
			if ((object)a == (object)b) return 0;
			if ((object)a != null && (object)b == null) return +1;
			if ((object)a == null && (object)b != null) return -1;

			Debug.Assert(a.IsConst && b.IsConst);

			switch (a.TypeBase)
			{
			case TypeBase.Bool: return a.Bool.CompareTo(b.Bool);
			case TypeBase.Int: return a.Int.CompareTo(b.Int);
			case TypeBase.Double: return a.Dbl.CompareTo(b.Dbl);
			case TypeBase.Obj:
			case TypeBase.Void:
				Debug.Assert(false);
				break;
			}
			return 0;
		}

		public override bool Equals(object obj)
		{
			var b = obj as ExprType;
			if (b == null) return false;
			return cmp(this, b) == 0;
		}
		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public static bool operator !=(ExprType a, ExprType b) { return cmp(a, b) != 0; }
		public static bool operator ==(ExprType a, ExprType b) { return cmp(a, b) == 0; }
		public static bool operator <(ExprType a, ExprType b) { return cmp(a, b) < 0; }
		public static bool operator >(ExprType a, ExprType b) { return cmp(a, b) > 0; }
		public static bool operator <=(ExprType a, ExprType b) { return cmp(a, b) <= 0; }
		public static bool operator >=(ExprType a, ExprType b) { return cmp(a, b) >= 0; }

		public static ExprType operator +(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int) return new ExprType(a.Int + b.Int);
			if (a._type.TypeBase == TypeBase.Double) return new ExprType(a.Dbl + b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator -(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int) return new ExprType(a.Int - b.Int);
			if (a._type.TypeBase == TypeBase.Double) return new ExprType(a.Dbl - b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator *(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int) return new ExprType(a.Int * b.Int);
			if (a._type.TypeBase == TypeBase.Double) return new ExprType(a.Dbl * b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator /(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int) return new ExprType(a.Int / b.Int);
			if (a._type.TypeBase == TypeBase.Double) return new ExprType(a.Dbl / b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator %(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int) return new ExprType(a.Int % b.Int);
			if (a._type.TypeBase == TypeBase.Double) return new ExprType(a.Dbl % b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator |(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int) return new ExprType(a.Int | b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprType operator &(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int) return new ExprType(a.Int & b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprType operator ^(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int) return new ExprType(a.Int ^ b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprType lsh(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int) return new ExprType(a.Int << b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprType rsh(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int) return new ExprType(a.Int >> b.Int);
			throw new InvalidOperatorException();
		}
	}

	/// <summary>
	/// Descrive il tipo e il valore constante o in un registro associato
	/// </summary>
	public class ExprValue
	{
		public ExprValue(string reg, RefTypeRoot t) { this._reg = reg; this._type = t; }

		public ExprValue(int c) { this._const = c; this._type = RefTypeSimple.Int; }
		public ExprValue(bool c) { this._const = c; this._type = RefTypeSimple.Bool; }
		public ExprValue(double d) { this._const = d; this._type = RefTypeSimple.Double; }
		public ExprValue(char d) { this._const = d; this._type = RefTypeSimple.Char; }

		public ExprValue(Label lbl) { this._const = lbl; this._type = RefTypeSimple.Int; }
		public ExprValue(ExprType t)
		{
			_reg = null;
			_type = t.Type;
			if (t.IsConst)
			{
				switch (_type.TypeBase)
				{
				case TypeBase.Bool: _const = t.Bool; break;
				case TypeBase.Char: _const = t.Char; break;
				case TypeBase.Int: _const = t.Int; break;
				case TypeBase.Double: _const = t.Dbl; break;
				case TypeBase.Obj: _const = new Null(); break;
				default: Debug.Assert(false); break;
				}
			}
		}

		string _reg;
		readonly object _const;
		readonly RefTypeRoot _type;

		public override string ToString()
		{
			if (_reg != null) return _reg;
			if (TypeBase == LLParserGenTest.TypeBase.Char)
				return U.F("'{0}'", _const);
			return _const.ToString();
		}


		public bool IsConst { get { return _const != null; } }
		public bool IsReg { get { return _reg != null; } }



		public RefTypeRoot Type { get { return _type; } }
		public TypeBase TypeBase { get { return _type.TypeBase; } }

		public int Int { get { Debug.Assert(_type.TypeBase == TypeBase.Int); return (int)_const; } }
		public double Dbl { get { Debug.Assert(_type.TypeBase == TypeBase.Double); return (double)_const; } }
		public bool Bool { get { Debug.Assert(_type.TypeBase == TypeBase.Bool); return (bool)_const; } }
		public string Reg { get { Debug.Assert(string.IsNullOrEmpty(_reg) == false); return _reg; } }

		public void SetReg(string r) { Debug.Assert(_reg != null); _reg = r; }
	}

	public class InvalidOperatorException : Exception
	{
	}

	public class NodeRoot : IAST
	{
		public readonly TokenAST tk;

		protected NodeRoot(TokenAST tk) { this.tk = tk; }
		public void Warning(string msg, params object[] args)
		{
			Console.WriteLine("{0} Warning: {1}", this.tk.TrackMsg, U.F(msg, args));
		}
		public void Error(string msg, params object[] args)
		{
			var s = U.F("{0} Error: {1}", this.tk.TrackMsg, U.F(msg, args));
			throw new SyntaxError(s);
		}
	}

	////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////

	public abstract class DeclRoot : NodeRoot
	{
		public readonly TokenAST name;
		public DeclRoot(TokenAST name) : base(name) { this.name = name; }

		private DeclRoot _father;
		public DeclRoot Father { get { return _father; } }
		public virtual void SetFather(DeclRoot p)
		{
			Debug.Assert(_father == null);
			_father = p;
		}

		public abstract string AssName { get; }
		public abstract void CheckSyntax(DeclGlobal dg);
		public abstract void GenCode(Context ctx);

		public abstract DeclRoot GetDecl(List<string> id, int i);
		public abstract DeclFun GetFun(string name, List<RefTypeRoot> args);
	}

	public class DeclList : List<DeclRoot>, IAST
	{
		public new DeclList Add(DeclRoot a) { base.Add(a); return this; }
		public void SetFather(DeclRoot a)
		{
			foreach (var e in this)
				e.SetFather(a);
		}
		public DeclRoot GetDecl(List<string> id, int i)
		{
			foreach (var d in this)
			{
				if (d is DeclFun) continue;
				if (d.name.strRead == id[i])
				{
					if (i + 1 == id.Count)
						return d;
					return d.GetDecl(id, i + 1);
				}
			}
			return null;
		}
	}

	public class DeclGlobal : DeclRoot
	{
		public readonly DeclList members;
		public DeclGlobal(DeclList members) : base(null) { 
			this.members = members;
 
			var tk = new TokenAST("", 1, MParser.ID, "ID", "int");
			this.members.Add(new DeclBase(new TokenAST(tk, MParser.ID, "ID", "void")));
			this.members.Add(new DeclBase(new TokenAST(tk, MParser.ID, "ID", "bool")));
			this.members.Add(new DeclBase(new TokenAST(tk, MParser.ID, "ID", "char")));
			this.members.Add(new DeclBase(new TokenAST(tk, MParser.ID, "ID", "int")));
			this.members.Add(new DeclBase(new TokenAST(tk, MParser.ID, "ID", "double")));
			this.members.Add(new DeclBase(new TokenAST(tk, MParser.ID, "ID", "handle")));

	
			RefTypeSimple.Void = new RefTypeSimple("void");
			RefTypeSimple.Bool = new RefTypeSimple("bool");
			RefTypeSimple.Char = new RefTypeSimple("char");
			RefTypeSimple.Int = new RefTypeSimple("int");
			RefTypeSimple.Double = new RefTypeSimple("double");
			RefTypeSimple.Handle = new RefTypeSimple("handle");
			RefTypeSimple.Void = new RefTypeSimple("void");

			RefTypeSimple.Void.ResolveType(this);
			RefTypeSimple.Bool.ResolveType(this);
			RefTypeSimple.Char.ResolveType(this);
			RefTypeSimple.Int.ResolveType(this);
			RefTypeSimple.Double.ResolveType(this);
			RefTypeSimple.Handle.ResolveType(this);
			RefTypeSimple.Void.ResolveType(this);
		
		}

		public override void SetFather(DeclRoot p)
		{
			base.SetFather(p);
			members.SetFather(this);
		}

		public override string AssName { get { return ""; } }

		public override void CheckSyntax(DeclGlobal dg)
		{
			foreach (var d in this.members)
				d.CheckSyntax(dg);
		}

		public override void GenCode(Context ctx)
		{
			foreach (var d in this.members)
				d.GenCode(ctx);
		}
		public override DeclRoot GetDecl(List<string> id, int i)
		{
			return members.GetDecl(id, i);
		}

		public override DeclFun GetFun(string name, List<RefTypeRoot> args)
		{
			foreach (var m in members)
			{
				var f = m as DeclFun;
				if (f == null) break;
				var r = f.MatchFun(name, args);
				if (r == DeclFun.MatchFunRes.Yes)
					return f;
			}
			return null;
		}

		public override string ToString()
		{
			return "global";
		}
	}

	public class DeclBase : DeclRoot
	{
		public DeclBase(TokenAST id) : base(id) {}

		public override void SetFather(DeclRoot p)
		{
			base.SetFather(p);
		}

		public override string AssName { 
			get {
				if (name.strRead == "void") return "$v";
				if (name.strRead == "bool") return "$b";
				if (name.strRead == "char") return "$c";
				if (name.strRead == "int") return "$i";
				if (name.strRead == "double") return "$f";
				if (name.strRead == "handle") return "$h";
				Debug.Assert(false);
				return "";
			} 
		}

		public override void CheckSyntax(DeclGlobal dg)
		{
		}

		public override void GenCode(Context ctx)
		{
		}
		public override DeclRoot GetDecl(List<string> id, int i)
		{
			if (id.Count == 1 && id[0] == this.name.strRead)
				return this;
			return null;
		}

		public override DeclFun GetFun(string name, List<RefTypeRoot> args)
		{
			return null;
		}

		public override string ToString()
		{
			return "base " + name.strRead;
		}
	}

	public class DeclNamespace : DeclRoot
	{
		public readonly DeclList members;
		public DeclNamespace(TokenAST name, DeclList members) : base(name) { this.members = members; }

		public override string ToString()
		{
			return "namespace " + name.strRead;
		}
		public override void SetFather(DeclRoot p)
		{
			base.SetFather(p);
			members.SetFather(this);
		}
		public override void CheckSyntax(DeclGlobal dg)
		{
			foreach (var d in this.members)
				d.CheckSyntax(dg);
		}

		public override void GenCode(Context ctx)
		{
			foreach (var d in this.members)
				d.GenCode(ctx);
		}

		string _assName;
		public override string AssName { get { return _assName ?? (_assName = Father.AssName + "$n" + name.strRead); } }

		public override DeclRoot GetDecl(List<string> id, int i)
		{
			var w = members.GetDecl(id, i);
			if (w != null) return w;
			return Father.GetDecl(id, i);
		}

		public override DeclFun GetFun(string name, List<RefTypeRoot> args)
		{
			bool b = true;
			foreach (var m in members)
			{
				var f = m as DeclFun;
				if (f == null) break;
				var r = f.MatchFun(name, args);
				if (r == DeclFun.MatchFunRes.Yes)
					return f;
				if (r == DeclFun.MatchFunRes.OnlyName)
					b = false;
			}
			if (b) return Father.GetFun(name, args);
			return null;
		}

	}

	public class DeclClass : DeclRoot
	{
		public readonly DeclList members;
		public readonly RefTypeRootList baseList;
		public Label vt
		{
			get
			{
				return new Label(this.AssName + "$vt");
			}
		}
		public DeclClass(TokenAST name, RefTypeRootList baseList, DeclList members) : base(name) { this.members = members; this.baseList = baseList; }

		public override string ToString()
		{
			return "class " + name.strRead;
		}
		public override void SetFather(DeclRoot p)
		{
			base.SetFather(p);
			members.SetFather(this);
		}

		string _assName;
		public override string AssName { get { return _assName ?? (_assName = Father.AssName + "$c" + this.name.strRead); } }

		public override void CheckSyntax(DeclGlobal dg)
		{
			if (this.baseList.Count == 0)
			{
				if (this.name.strRead != "Object")
					this.baseList.Add(new RefTypeSimple("Object"));
			}

			foreach (var d in this.members)
				d.CheckSyntax(dg);
		}
		public override void GenCode(Context ctx)
		{
			ctx.dataLbl(vt);
			foreach (var f in members)
				if (f is DeclFun)
					ctx.putInt(new Label(f.AssName));

			foreach (var d in members)
				d.GenCode(ctx);
		}

		public int MemberSize(DeclRoot dr)
		{
				int sz = 0;
				foreach (var m in members)
					if (m is DeclVar)
						sz += 1;
				if (baseList.Count > 0)
				{
					baseList[0].ResolveType(dr);
					var dc = baseList[0].DeclRoot as DeclClass;
					sz += dc.MemberSize(dr);
				}
				return sz;
		}
		public override DeclRoot GetDecl(List<string> id, int i)
		{
			var w = members.GetDecl(id, i);
			if (w != null) return w;
			return Father.GetDecl(id, i);
		}


		public override DeclFun GetFun(string name, List<RefTypeRoot> args)
		{
			bool b = true;
			foreach (var m in members)
			{
				var f = m as DeclFun;
				if (f == null) break;
				var r = f.MatchFun(name, args);
				if (r == DeclFun.MatchFunRes.Yes)
					return f;
				if (r == DeclFun.MatchFunRes.OnlyName)
					b = false;
			}
			if (b) return Father.GetFun(name, args);
			return null;
		}

	}
	public class DeclInterface : DeclRoot
	{
		public readonly DeclList members;
		public readonly RefTypeRootList baseList;
		public Label vt
		{
			get
			{
				return new Label(this.AssName + "$vt");
			}
		}
		public DeclInterface(TokenAST name, RefTypeRootList baseList, DeclList members) : base(name) { this.members = members; this.baseList = baseList; }

		public override string ToString()
		{
			return "interface " + name.strRead;
		}
		public override void SetFather(DeclRoot p)
		{
			base.SetFather(p);
			members.SetFather(this);
		}

		string _assName;
		public override string AssName { get { return _assName ?? (_assName = Father.AssName + "$c" + this.name.strRead); } }

		public override void CheckSyntax(DeclGlobal dg)
		{
			if (this.baseList.Count == 0)
			{
				if (this.name.strRead != "Object")
					this.baseList.Add(new RefTypeSimple("Object"));
			}

			foreach (var d in this.members)
				d.CheckSyntax(dg);
		}
		public override void GenCode(Context ctx)
		{
			ctx.dataLbl(vt);
			foreach (var f in members)
				if (f is DeclFun)
					ctx.putInt(new Label(f.AssName));

			foreach (var d in members)
				d.GenCode(ctx);
		}

		public int MemberSize(DeclRoot dr)
		{
			int sz = 0;
			foreach (var m in members)
				if (m is DeclVar)
					sz += 1;
			if (baseList.Count > 0)
			{
				baseList[0].ResolveType(dr);
				var dc = baseList[0].DeclRoot as DeclClass;
				sz += dc.MemberSize(dr);
			}
			return sz;
		}
		public override DeclRoot GetDecl(List<string> id, int i)
		{
			var w = members.GetDecl(id, i);
			if (w != null) return w;
			return Father.GetDecl(id, i);
		}


		public override DeclFun GetFun(string name, List<RefTypeRoot> args)
		{
			bool b = true;
			foreach (var m in members)
			{
				var f = m as DeclFun;
				if (f == null) break;
				var r = f.MatchFun(name, args);
				if (r == DeclFun.MatchFunRes.Yes)
					return f;
				if (r == DeclFun.MatchFunRes.OnlyName)
					b = false;
			}
			if (b) return Father.GetFun(name, args);
			return null;
		}

	}
	public class DeclPropGetSet : IAST
	{
		public readonly bool isSet; 
		public readonly StmtRoot s;
		public DeclPropGetSet(bool isSet, StmtRoot s)
		{
			this.isSet = isSet; this.s = s;
		}
	}
	public class DeclProp : DeclRoot
	{
		public readonly RefTypeRoot ty;
		public readonly DeclPropGetSet a;
		public readonly DeclPropGetSet b;

		public DeclFun fset;
		public DeclFun fget;

		public DeclProp(TokenAST name, DeclPropGetSet a, DeclPropGetSet b)
			: base(name)
		{
			this.a = a; 
			this.b = b;
		}
		public DeclProp(TokenAST name, RefTypeRoot ty, DeclProp pp)
			: base(name) 
		{
			this.ty = ty;
			this.a = pp.a;
			this.b = pp.b;
		}

		public override string ToString()
		{
			return "prop " + name.strRead;
		}
		public override void SetFather(DeclRoot p)
		{
			base.SetFather(p);
		}

		string _assName;
		public override string AssName { get { return _assName ?? (_assName = Father.AssName + "$p" + this.name.strRead); } }

		public override void CheckSyntax(DeclGlobal dg)
		{
			if (a != null && b != null)
			{
				if (a.isSet && b.isSet) Error("cannot declare two set properties");
				if (!a.isSet && !b.isSet) Error("cannot declare two get properties");
			}

			var setArgs = new FunArgList(); setArgs.Add(new FunArg(new TokenAST(this.name, MParser.ID, "ID", "value"), this.ty));
			var getArgs = new FunArgList();
			if (a != null)
			{
				if (a.isSet)
					this.fset = new DeclFun(null, this.name, setArgs, new RefTypeRoot_or_Base(RefTypeRoot.Void), a.s, this.name);
				else
					this.fget = new DeclFun(null, this.name, getArgs, new RefTypeRoot_or_Base(this.ty), a.s, this.name);
			}
			if (b != null)
			{
				if (b.isSet)
					this.fset = new DeclFun(null, this.name, setArgs, new RefTypeRoot_or_Base(RefTypeRoot.Void), b.s, this.name);
				else
					this.fget = new DeclFun(null, this.name, setArgs, new RefTypeRoot_or_Base(this.ty), b.s, this.name);
			}

			if (fget != null) { fget.SetFather(this.Father); fget.CheckSyntax(dg); }
			if (fset != null) { fset.SetFather(this.Father); fset.CheckSyntax(dg); }
		}
		public override void GenCode(Context ctx)
		{
			if (this.fget != null) this.fget.GenCode(ctx);
			if (this.fset != null) this.fset.GenCode(ctx);
		}

		public override DeclRoot GetDecl(List<string> id, int i) { return Father.GetDecl(id, i); }


		public override DeclFun GetFun(string name, List<RefTypeRoot> args) { return null; }
	}

	public class DeclVar : DeclRoot
	{
		public readonly RefTypeRoot Type;
		public DeclVar(TokenAST name, RefTypeRoot type) : base(name) { this.Type = type; }
		public override string AssName { get { return this.name.strRead; } }
		public override void CheckSyntax(DeclGlobal dg)
		{
			if (Type.ResolveType(this) == false)
				Error("cannot resolve type '{0}'", Type.ToString());
		}
		public override string ToString() { return "var " + name.strRead; }

		public override void GenCode(Context ctx) {}

		public override DeclRoot GetDecl(List<string> id, int i) { return Father.GetDecl(id, i); }
		public override DeclFun GetFun(string name, List<RefTypeRoot> args) { return null; }
	}

	public class DeclFun : DeclRoot
	{
		public readonly FunArgList args;
		public  BaseInit baseInit;
		public  RefTypeRoot ret;
		public readonly StmtRoot body;
		public readonly TokenAST lastCurly;
		public readonly TokenAST funMod;
		public bool constructor;

		public DeclFun(TokenAST funMod, TokenAST name, FunArgList args, RefTypeRoot_or_Base tb, StmtRoot body, TokenAST lastCurly)
			: base(name)
		{
			this.args = args;
			this.funMod = funMod;

			if (tb != null)
			{
				if ((object)tb.t != null)
					this.ret = tb.t;
				else
					this.baseInit = tb.b;
			}

			this.body = body;
			this.lastCurly = lastCurly;
		}

		public override string ToString() { return "fun " + name.strRead; }

		public override string AssName { get {  return Father.AssName + "$f" + this.name.strRead + this.args.AssName; } }
		public override void CheckSyntax(DeclGlobal dg)
		{
			if (this.Father is DeclClass)
			{
				var father = this.Father as DeclClass;
				if (father.name.strRead == this.name.strRead)
				{
					// costruttore
					this.constructor = true;

					if (this.ret != null)
						Error("constructors cannot have return type");
					this.ret = new RefTypeSimple(this.name);   // il costruttore ritorna sempre this.
					this.ret.ResolveType(this);

					// se non chiama esplicitamente base o this vuole dire che chiama implicitamente base()
					if (this.baseInit == null && this.name.strRead != "Object")
						this.baseInit = new BaseInit(new TokenAST(this.name, MParser.BASE, "BASE", "base"), new ExprList());

					father.baseList.ForEach(p=>p.ResolveType(this));
				}
				else
				{
					// funzione membro
				}
			}
			else
			{
				// funzione globale.
			}

			if (this.ret.ResolveType(this) == false)
				Error("cannot resolve type '{0}'", this.ret.ToString());

			foreach (var a in args)
				if (a.ArgType.ResolveType(this) == false)
					Error("cannot resolve type '{0}'", a.ArgType.ToString());

			//this.body.CheckType(ctx);

			if (this.baseInit != null)
			{
			}
		}

		public bool IsStatic
		{
			get {
				if (this.Father is DeclClass) return false;
				if (this.Father is DeclNamespace) return true;
				if (this.Father is DeclGlobal) return true;
				Debug.Assert(false);
				return true;
			}
		}

		public override void GenCode(Context ctx)
		{
			bool debug = false;
			bool optimize = true;

			using (var fctx = new FunctionContex(ctx, this))
			{
				int codeStart = ctx._code.Count;
				int dataStart = ctx._data.Count;

				if (true)
				{
					if (this.Father is DeclClass)
					{
						var tt = new RefTypeSimple(this.Father.name.strRead);
						tt.ResolveType(this);
						fctx.AddArgVar(new TokenAST(this.name, MParser.THIS, "THIS", "this"), tt);
					}
					foreach (var a in this.args)
						fctx.AddArgVar(a.ArgName, a.ArgType);
				}

				ctx.codeLbl(new Label(this.AssName));
				if (this.Father is DeclClass)
				{
					// la cosa è complicata... devo cercare il : base(....) se c'è altrimenti si suppone che ci sia il costruttore base()
					if (this.baseInit != null)
					{
						DeclClass cls = this.Father as DeclClass;
						if (this.baseInit.t.strRead == "base")
						{
							cls.baseList[0].ResolveType(this);
							cls = cls.baseList[0].DeclRoot as DeclClass;				
						}
						else if (this.baseInit.t.strRead == "this")
						{
							cls = this.Father as DeclClass;
						}
						else
							Debug.Assert(false);

						List<RefTypeRoot> ta = new List<RefTypeRoot>();
						foreach (var tb in this.baseInit.e)
							ta.Add(tb.CheckType(fctx).Type);
						var bf = cls.GetFun(cls.name.strRead, ta);
						if (bf == null)
							Error("cannot find function '{0}' with args", cls.name.strRead);

						var rt = fctx.GetVar(new TokenAST(this.name, MParser.THIS, "THIS", "this"));
						ctx.ld("rp", rt);
						foreach (var tb in this.baseInit.e)
							tb.GenRight(fctx, "rp");
						ctx.js(rt.Reg, new Label(bf.AssName), new ExprType(this.ret));
					}
				}
				bool unreachable = this.body.GenCode(fctx);
				if (unreachable == false)
				{
					if (this.ret.TypeBase != TypeBase.Void)
					{
						if (this.ret.TypeBase != TypeBase.Obj && this.constructor == false)
							Error("missing return stmt");
					}	
					if (this.constructor)
					{
						var re = new ExprThis(new TokenAST(this.lastCurly, MParser.THIS, "THIS", "this"));
						var r = re.GenRight(fctx, null);
						fctx.Context.ret(r);
					}
					else
						fctx.Context.ret(null);
				}

				int codeEnd = ctx._code.Count;
				int dataEnd = ctx._data.Count;


				// le var in ingresso anche se non servono più non vengono sovrascritte
				// da altre variabili (se invece si vuole ottimizzare al max si può omettere il Foreach).
				var live = new List<string>();
				if (optimize == false) this.args.ForEach(v => live.Add(fctx.GetVar(v.ArgName).Reg));
				ctx.ComputeLive(codeStart, codeEnd, live);
				if (debug)
				{
					Console.WriteLine("Codice generato per la funzione {0} {1}/{2}", this.name, codeStart, codeEnd);
					Console.WriteLine("Live variables");
					Console.WriteLine(ctx.CodeToString(codeStart, codeEnd));
				}

				var gr = ctx.CreateGraph(codeStart, codeEnd);
				if (debug)
				{
					Console.WriteLine("Grafo");
					Console.WriteLine(gr);
				}

				bool ok = false;
				var regAllocati = this.args.Count;
				for (int k = regAllocati; k < 32; ++k)
				{
					var col = gr.Color(k);
					if (col != null)
					{
						if (debug)
							Console.WriteLine("Ci vogliono k={0} da r0 a r{1}, prossimo per push/js r{0}", k, k - 1);
						var regs = col.GetRegs();
						ctx.SetTemps(codeStart, codeEnd, regs);
						ok = true;
						break;
					}
				}

				if (ok == false)
					Error("Cannot allocate registers");

				if (debug)
				{
					Console.WriteLine(ctx.CodeToString(codeStart, codeEnd));
					Console.WriteLine(ctx.DataToString(dataStart, dataEnd));
					Console.WriteLine("#####################");
				}
			}
		}

		public override DeclRoot GetDecl(List<string> id, int i)
		{
			return Father.GetDecl(id, i);
		}

		public enum MatchFunRes { No, OnlyName, Yes }

		public MatchFunRes MatchFun(string name, List<RefTypeRoot> args)
		{
			if (name != this.name.strRead) return MatchFunRes.No;
			if (args.Count != this.args.Count) return MatchFunRes.OnlyName;

			for (int i = 0; i < args.Count; ++i)
				if (this.args[i].ArgType != args[i]) 
					return  MatchFunRes.OnlyName;
			return MatchFunRes.Yes;
		}

		public override DeclFun GetFun(string name, List<RefTypeRoot> args)
		{
			var r = MatchFun(name, args);
			if (r == MatchFunRes.Yes)
				return this;
			return null;
		}

	}

	public class FunArg
	{
		public FunArg(TokenAST argName, RefTypeRoot argType)
		{
			this.ArgName = argName;
			this.ArgType = argType;
		}
		public readonly TokenAST ArgName;
		public readonly RefTypeRoot ArgType;
	}


	public class FunArgList : List<FunArg>, IAST 
	{
		public string AssName
		{
			get
			{
				var r = "";
				if (this.Count == 0)
					r += "$v";
				else
					foreach (var v in this)
						r += v.ArgType.AssName;

				return r;
			}
		}
		public FunArgList Add(TokenAST arg, RefTypeRoot ty) { base.Add(new FunArg(arg, ty)); return this; }
	}

	/////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////


	public abstract class StmtRoot : NodeRoot
	{
		// ritorna true se next unreachable
		public abstract bool GenCode(FunctionContex ctx);
		public abstract void CheckType(FunctionContex ctx);
		
		protected StmtRoot(TokenAST tk) : base(tk) { }
	}

	public class StmtList : StmtRoot
	{
		readonly List<StmtRoot> a;

		public StmtList() : base(null) { this.a = new List<StmtRoot>(); }
		public StmtList(StmtRoot s) : base(s.tk) { this.a = new List<StmtRoot>(); this.Add(s); }

		public StmtList Add(StmtRoot s) { this.a.Add(s); return this; }

		public override void CheckType(FunctionContex ctx)
		{
			foreach (var s in a) s.CheckType(ctx);
		}

		public override bool GenCode(FunctionContex ctx)
		{
			bool nu = false;
			foreach (var s in a)
			{
				if (nu == true)
				{
					s.Warning("Unreachable code");
					break;
				}
				nu = s.GenCode(ctx);
			}
			return nu;
		}
	}

	public class StmtVar : StmtRoot
	{
		public readonly TokenAST a;
		public readonly RefTypeRoot type;
		public StmtVar(TokenAST tk, TokenAST a, RefTypeRoot ty) : base(tk) { this.a = a; this.type = ty; }

		public override void CheckType(FunctionContex ctx)
		{
			if (this.type.ResolveType(ctx.fun) == false)
				Error("cannot resolve tye '{0}'", this.type.ToString());
		}

		public override bool GenCode(FunctionContex ctx)
		{
			CheckType(ctx);
			ctx.AddDefVar(this.a, this.type);
			ctx.Push(FunctionContex.StmtTk.Var, null, null, this.a);
			return false;
		}
	}
	public class StmtBlock : StmtRoot
	{
		readonly StmtRoot sa;

		public StmtBlock(TokenAST tk, StmtRoot a) : base(tk) { this.sa = a; }
		public StmtBlock(StmtRoot a) : base(a.tk) { this.sa = a; }

		public override void CheckType(FunctionContex ctx)
		{
			sa.CheckType(ctx);
		}

		public override bool GenCode(FunctionContex ctx)
		{
			CheckType(ctx);
			ctx.Push(FunctionContex.StmtTk.Block, null, null, null);
			bool nu = sa.GenCode(ctx);
			ctx.Pop(FunctionContex.StmtTk.Block);
			return nu;
		}
	}

	public class StmtIf : StmtRoot
	{
		readonly ExprRoot e;
		readonly StmtRoot sa;
		readonly StmtRoot sb;

		public StmtIf(TokenAST tk, ExprRoot e, StmtRoot sa, StmtRoot sb = null)
			: base(tk)
		{
			this.e = e;
			this.sa = sa;
			this.sb = sb;
		}

		public override void CheckType(FunctionContex ctx)
		{
			e.CheckType(ctx);
			sa.CheckType(ctx);
			if (sb != null) sb.CheckType(ctx);
		}


		public override bool GenCode(FunctionContex ctx)
		{
			CheckType(ctx);

			bool nua = false;
			bool nub = false;

			var te = e.CheckType(ctx);
			if (te.TypeBase != TypeBase.Bool)
				this.Error("while requires boolean expression.");

			if (te.IsConst)
			{
				this.Warning("const expression");

				if (sb == null)
				{
					if (te.Bool)
					{
						nua = this.sa.GenCode(ctx);
						nub = true;
					}
				}
				else
				{
					if (te.Bool)
					{
						nua = this.sa.GenCode(ctx);
						nub = true;
					}
					else
					{
						nua = true;
						nub = this.sb.GenCode(ctx);
					}
				}
			}
			else
			{
				if (this.sb == null)
				{
					var lbl_false = ctx.NewLbl();
					e.GenBool(ctx, null, lbl_false);
					this.sa.GenCode(ctx);
					ctx.emit(lbl_false);
					nua = false;
					nub = false;

				}
				else
				{
					var lbl_out = ctx.NewLbl();
					var lbl_false = ctx.NewLbl();
					e.GenBool(ctx, null, lbl_false);
					nua = this.sa.GenCode(ctx);
					if (nua == false) ctx.Context.jmp(lbl_out);
					ctx.emit(lbl_false);
					nub = this.sb.GenCode(ctx);
					if (nub == false) ctx.emit(lbl_out);
				}
			}
			return nua && nub;
		}
	}

	public class StmtWhile : StmtRoot
	{
		readonly ExprRoot e;
		readonly StmtRoot s;
		public StmtWhile(TokenAST tk, ExprRoot e, StmtRoot s) : base(tk) { this.e = e; this.s = s; }


		public override void CheckType(FunctionContex ctx)
		{
			e.CheckType(ctx);
			s.CheckType(ctx);
		}

		public override bool GenCode(FunctionContex ctx)
		{
			CheckType(ctx);

			var lbl_break = ctx.NewLbl();
			var lbl_continue = ctx.NewLbl();
			ctx.Push(FunctionContex.StmtTk.While, lbl_break, lbl_continue, null);

			var te = e.CheckType(ctx);

			if (te.TypeBase != TypeBase.Bool)
				this.Error("while statments require bool expression");

			bool nu = false;
			if (te.IsConst)
			{
				this.Warning("Const expression");

				if (te.Bool)
				{
					var lbl_true = ctx.Context.NewLbl();
					ctx.emit(lbl_continue);
					ctx.emit(lbl_true);
					nu = s.GenCode(ctx);
					if (nu == false) ctx.Context.jmp(lbl_true);
					ctx.emit(lbl_break);
					nu = false;
				}
			}
			else
			{
				var lbl_true = ctx.NewLbl();
				ctx.Context.jmp(lbl_continue);
				ctx.emit(lbl_true);
				this.s.GenCode(ctx);
				ctx.emit(lbl_continue);
				this.e.GenBool(ctx, lbl_true, null);
				ctx.emit(lbl_break);
				nu = false;
			}

			ctx.Pop(FunctionContex.StmtTk.While);
			return nu;
		}
	}
	public class StmtBreak : StmtRoot
	{
		public StmtBreak(TokenAST tk) : base(tk) { }

		public override void CheckType(FunctionContex ctx)
		{
		}
		public override bool GenCode(FunctionContex ctx)
		{
			if (ctx.Break() == false)
				Error("illegal break");
			return true;
		}
	}
	public class StmtContinue : StmtRoot
	{
		public StmtContinue(TokenAST tk) : base(tk) { }

		public override void CheckType(FunctionContex ctx)
		{
		}
		public override bool GenCode(FunctionContex ctx)
		{
			if (ctx.Continue() == false)
				Error("Illegal continue");
			return true;
		}
	}

	public class StmtReturn : StmtRoot
	{
		readonly ExprRoot e;
		public StmtReturn(TokenAST tk) : base(tk) { e = null; }
		public StmtReturn(TokenAST tk, ExprRoot e) : base(tk) { this.e = e; }

		public override void CheckType(FunctionContex ctx)
		{
			if (this.e != null)
				e.CheckType(ctx);
		}
		public override bool GenCode(FunctionContex ctx)
		{
			CheckType(ctx);

			if (ctx.fun.constructor)
			{
				if (this.e != null) Error("constructs cannot return value");
				var re = new ExprId(new TokenAST(this.tk, MParser.THIS, "THIS", "this"));
				var r = re.GenRight(ctx, null);
				ctx.Return(r);
				ctx.Context.ret(r);
				return true;
			}

			if (ctx.fun.ret.TypeBase == TypeBase.Void && this.e != null) Error("return expression with void function");
			if (ctx.fun.ret.TypeBase != TypeBase.Void && this.e == null) Error("return without value");

			if (ctx.fun.ret.TypeBase != TypeBase.Void)
			{
				var te = e.CheckType(ctx);
				RefTypeRoot aaa = te.Type;
				RefTypeRoot bbb = ctx.fun.ret;

				if (aaa.AssName != bbb.AssName)
					Error("wrong return type");
			}

			if (e != null)
			{
				var r = e.GenRight(ctx, null);
				ctx.Return(r);
				ctx.Context.ret(r);
			}
			else
			{
				ctx.Return(null);
				ctx.Context.ret(new ExprValue(0));
			}
			return true;
		}
	}

	public class StmtExpr : StmtRoot
	{
		readonly ExprRoot e;
		public StmtExpr(TokenAST tk, ExprRoot e) : base(tk) { this.e = e; }
		public override bool GenCode(FunctionContex ctx) { e.GenRight(ctx, null); return false; }
		public override void CheckType(FunctionContex ctx)
		{
			e.CheckType(ctx);
		}
	}

	///////////////////////////////////////////////////////////

	public abstract class ExprRoot : NodeRoot
	{
		protected ExprRoot(TokenAST tk) : base(tk) { }

		public abstract ExprValue GenRight(FunctionContex ctx, string rdest);

		public abstract ExprType CheckType(FunctionContex ctx);

		public virtual void GenBool(FunctionContex ctx, Label lbl_true, Label lbl_false)
		{
			var s = GenRight(ctx, null);
			if (s.TypeBase != TypeBase.Bool)
				Error("boolean expression required");

			if (s.IsConst)
			{
				if (lbl_true != null)
				{
					if (s.Bool) ctx.Context.jmp(lbl_true);
				}
				else
				{
					if (s.Bool == false) ctx.Context.jmp(lbl_false);
				}
			}
			else
			{
				if (lbl_true != null)
					ctx.Context.bne(s, new ExprValue(0), lbl_true);
				else
					ctx.Context.beq(s, new ExprValue(0), lbl_false);
			}
		}

		/// <summary>
		/// Ritorna il registro da assegnare come risultato dell'espressione.
		/// Null se invece l'expressione non è riconducibile solo ad una var locale.
		/// </summary>
		/// <returns>The left pre.</returns>
		/// <param name="ctx">Context.</param>
		public virtual string GenLeftPre(FunctionContex ctx)
		{
			Error("cannot evaluate expression to assign");
			return null;
		}

		/// <summary>
		/// Data una espressione già valutata in v la assegna alla parte sinistra dell'assegnamento
		/// </summary>
		/// <returns>The left post.</returns>
		/// <param name="ctx">Context.</param>
		/// <param name="v">V.</param>
		public virtual ExprValue GenLeftPost(FunctionContex ctx, ExprValue v)
		{
			Error("cannot evaluate expression to assign");
			return null;
		}
	}

	public class ExprAss : ExprRoot
	{
		protected ExprRoot dst;
		protected ExprRoot src;

		public ExprAss(ExprRoot dst, TokenAST tk, ExprRoot src) : base(tk) { this.dst = dst; this.src = src; }

//		public override string GenLeftPre(FunctionContex ctx) { return dst.GenLeftPre(ctx); }

		public override ExprType CheckType(FunctionContex ctx)
		{
			var tdst = dst.CheckType(ctx); 
			var tsrc = src.CheckType(ctx); 

			if (tdst.Type != tsrc.Type)
				Error("cannot assign expression of type '{0}' to '{1}'", tdst.Type, tsrc.Type);
			return tsrc;
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			CheckType(ctx);
			var ra = dst.GenLeftPre(ctx);
			if (ra != null)
			{
				var rb = src.GenRight(ctx, ra);
				if (rb.IsReg == false || (rb.IsReg == true && rb.Reg != ra)) ctx.Context.ld(ra, rb);
				if (rdest != null) ctx.Context.ld(rdest, rb);
				return rb;  // qui posso ritornare la costante (se è const oppure la var...)
			}
			else
			{
				var rb = src.GenRight(ctx, null);
				return dst.GenLeftPost(ctx, rb);
			}
		}
	}

	public abstract class ExprBin : ExprRoot
	{
		protected ExprBin(ExprRoot a, TokenAST tk, ExprRoot b) : base(tk) { this.a = a; this.b = b; }
		protected ExprRoot a;
		protected ExprRoot b;

		public string op { get { return tk.strRead; } }
	}

	public abstract class ExprUni : ExprRoot
	{
		protected ExprUni(TokenAST tk, ExprRoot a) : base(tk) { this.a = a; }
		protected ExprRoot a;

		public string op { get { return tk.strRead; } }
	}

	public class ExprBinLogical : ExprBin
	{
		public ExprBinLogical(ExprRoot a, TokenAST tk, ExprRoot b)
			: base(a, tk, b)
		{
			switch (this.op)
			{
			case "&&": break;
			case "||": break;
			default: Debug.Assert(false); break;
			}
		}

		public override ExprType CheckType(FunctionContex ctx)
		{
			var ta = a.CheckType(ctx);
			var tb = b.CheckType(ctx);

			if (ta.IsBool == false || tb.IsBool == false) Error("'{0}' operator requires boolean expressions", this.op);
			if (ta.IsConst && tb.IsConst)
			{
				ExprType rr;
				switch (this.op)
				{
				case "&&": rr = new ExprType(ta.Bool && tb.Bool); break;
				case "||": rr = new ExprType(ta.Bool || tb.Bool); break;
				default: Debug.Assert(false); return null;
				}
				return rr;
			}
			return new ExprType(RefTypeSimple.Bool);
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var t = CheckType(ctx);

			if (t.IsConst)
			{
				return new ExprValue(t);
			}
			else
			{
				if (rdest == null) rdest = ctx.NewTmp();
				if (op == "||")
				{
					var lbl_false = ctx.NewLbl();
					var lbl_true = ctx.NewLbl();
					ctx.Context.ld(rdest, 0);
					a.GenBool(ctx, lbl_true, null);
					b.GenBool(ctx, null, lbl_false);
					ctx.emit(lbl_true);
					ctx.Context.ld(rdest, 1);
					ctx.emit(lbl_false);
					return new ExprValue(rdest, RefTypeSimple.Bool);
				}
				else if (op == "&&")
				{
					var lbl_false = ctx.NewLbl();
					var lbl_true = ctx.NewLbl();
					ctx.Context.ld(rdest, 1);
					a.GenBool(ctx, null, lbl_false);
					b.GenBool(ctx, lbl_true, null);
					ctx.emit(lbl_false);
					ctx.Context.ld(rdest, 0);
					ctx.emit(lbl_true);
					return new ExprValue(rdest, RefTypeSimple.Bool);
				}
				else
				{
					Debug.Assert(false);
					return null;
				}
			}
		}

		public override void GenBool(FunctionContex ctx, Label lbl_true, Label lbl_false)
		{
			var t = CheckType(ctx);

			if (t.IsConst)
			{
				if (lbl_true != null)
				{
					switch (this.op)
					{
					case "&&": if (t.Bool == true) ctx.Context.jmp(lbl_true); break;
					case "||": if (t.Bool == true) ctx.Context.jmp(lbl_true); break;
					default: Debug.Assert(false); break;
					}
				}
				else
				{
					switch (this.op)
					{
					case "&&": if (t.Bool == false) ctx.Context.jmp(lbl_false); break;
					case "||": if (t.Bool == false) ctx.Context.jmp(lbl_false); break;
					default: Debug.Assert(false); break;
					}
				}
			}
			else
			{
				if (op == "&&")
				{
					if (lbl_true != null)
					{
						lbl_false = ctx.NewLbl();
						a.GenBool(ctx, null, lbl_false);
						b.GenBool(ctx, lbl_true, null);
						ctx.emit(lbl_false);
					}
					else
					{
						a.GenBool(ctx, null, lbl_false);
						b.GenBool(ctx, null, lbl_false);
					}
				}
				else if (op == "||")
				{
					if (lbl_true != null)
					{
						a.GenBool(ctx, lbl_true, null);
						b.GenBool(ctx, lbl_true, null);
					}
					else
					{
						lbl_true = ctx.NewLbl();
						a.GenBool(ctx, lbl_true, null);
						b.GenBool(ctx, null, lbl_false);
						ctx.emit(lbl_true);
					}
				}
				else
				{
					Debug.Assert(false);
				}
			}
		}
	}
	public class ExprBinCompare : ExprBin
	{
		public ExprBinCompare(ExprRoot a, TokenAST tk, ExprRoot b)
			: base(a, tk, b)
		{
			switch (this.op)
			{
			case "==": break;
			case "!=": break;
			case ">": break;
			case ">=": break;
			case "<": break;
			case "<=": break;
			default: Debug.Assert(false); break;
			}
		}

		public override ExprType CheckType(FunctionContex ctx)
		{
			var ta = a.CheckType(ctx);
			var tb = b.CheckType(ctx);

			bool error = false;
			switch (this.op)
			{
			case "==":
			case "!=":
				if (ta.TypeBase == TypeBase.Void || tb.TypeBase == TypeBase.Void) error = true;
				if (ta.Type != tb.Type) error = true;
				break;
			case ">":
			case ">=":
			case "<":
			case "<=":
				if (ta.Type != tb.Type) error = true;
				if (!(ta.TypeBase == TypeBase.Int || ta.TypeBase == TypeBase.Double)) error = true;
				break;
			default:
				Debug.Assert(false);
				break;
			}
			if (error)
				Error("'{0}' operator cannot be used with '{1}' and '{2}' types", this.op, ta.Type, tb.Type);

			if (ta.IsConst && tb.IsConst)
			{
				ExprType tr = null;
				switch (this.op)
				{
				case "==": tr = new ExprType(ta == tb ? true : false); break;
				case "!=": tr = new ExprType(ta != tb ? true : false); break;
				case ">": tr = new ExprType(ta > tb ? true : false); break;
				case ">=": tr = new ExprType(ta >= tb ? true : false); break;
				case "<": tr = new ExprType(ta < tb ? true : false); break;
				case "<=": tr = new ExprType(ta <= tb ? true : false); break;
				default: Debug.Assert(false); break;
				}
				return tr;
			}

			return new ExprType(RefTypeSimple.Bool);
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var t = CheckType(ctx);

			if (t.IsConst)
			{
				return new ExprValue(t);
			}
			else
			{
				var aa = a.GenRight(ctx, null);
				var bb = b.GenRight(ctx, null);
				var lbl_false = ctx.NewLbl();
				if (rdest == null) rdest = ctx.NewTmp();
				ctx.Context.ld(rdest, 0);
				switch (this.op)
				{
				case "==": ctx.Context.bne(aa, bb, lbl_false); break;
				case "!=": ctx.Context.beq(aa, bb, lbl_false); break;
				case ">": ctx.Context.ble(aa, bb, lbl_false); break;
				case ">=": ctx.Context.blt(aa, bb, lbl_false); break;
				case "<": ctx.Context.bge(aa, bb, lbl_false); break;
				case "<=": ctx.Context.blt(aa, bb, lbl_false); break;
				default: Debug.Assert(false); return null;
				}
				ctx.Context.ld(rdest, 1);
				ctx.emit(lbl_false);
				return new ExprValue(rdest, RefTypeSimple.Bool);
			}
		}

		public override void GenBool(FunctionContex ctx, Label lbl_true, Label lbl_false)
		{
			var t = CheckType(ctx);

			if (t.IsConst)
			{
				var aa = a.CheckType(ctx);
				var bb = b.CheckType(ctx);

				if (lbl_true != null)
				{
					switch (this.op)
					{
					case "==": if (aa == bb) ctx.Context.jmp(lbl_true); break;
					case "!=": if (aa != bb) ctx.Context.jmp(lbl_true); break;
					case ">": if (aa > bb) ctx.Context.jmp(lbl_true); break;
					case ">=": if (aa >= bb) ctx.Context.jmp(lbl_true); break;
					case "<": if (aa < bb) ctx.Context.jmp(lbl_true); break;
					case "<=": if (aa <= bb) ctx.Context.jmp(lbl_true); break;
					}
				}
				else
				{
					switch (this.op)
					{
					case "==": if (aa != bb) ctx.Context.jmp(lbl_false); break;
					case "!=": if (aa == bb) ctx.Context.jmp(lbl_false); break;
					case ">": if (aa <= bb) ctx.Context.jmp(lbl_false); break;
					case ">=": if (aa < bb) ctx.Context.jmp(lbl_false); break;
					case "<": if (aa >= bb) ctx.Context.jmp(lbl_false); break;
					case "<=": if (aa > bb) ctx.Context.jmp(lbl_false); break;
					}
				}
			}
			else
			{
				var aa = a.GenRight(ctx, null);
				var bb = b.GenRight(ctx, null);

				if (lbl_true != null)
				{
					switch (this.op)
					{
					case "==": ctx.Context.beq(aa, bb, lbl_true); break;
					case "!=": ctx.Context.bne(aa, bb, lbl_true); break;
					case ">": ctx.Context.bgt(aa, bb, lbl_true); break;
					case ">=": ctx.Context.bge(aa, bb, lbl_true); break;
					case "<": ctx.Context.blt(aa, bb, lbl_true); break;
					case "<=": ctx.Context.ble(aa, bb, lbl_true); break;
					}
				}
				else
				{
					switch (this.op)
					{
					case "==": ctx.Context.bne(aa, bb, lbl_false); break;
					case "!=": ctx.Context.beq(aa, bb, lbl_false); break;
					case ">": ctx.Context.ble(aa, bb, lbl_false); break;
					case ">=": ctx.Context.blt(aa, bb, lbl_false); break;
					case "<": ctx.Context.bge(aa, bb, lbl_false); break;
					case "<=": ctx.Context.blt(aa, bb, lbl_false); break;
					}
				}
			}
		}
	}

	public class ExprBinGen : ExprBin
	{
		public ExprBinGen(ExprRoot a, TokenAST tk, ExprRoot b)
			: base(a, tk, b)
		{
		}

		public override ExprType CheckType(FunctionContex ctx)
		{
			var ta = a.CheckType(ctx);
			var tb = b.CheckType(ctx);

			if (ta.Type != tb.Type) Error("operator '{0}' requires same types", tk.strRead);
			if (!(ta.TypeBase == TypeBase.Int || tb.TypeBase == TypeBase.Double)) Error("'{0}' requires int or double type", tk.strRead);
			if (!(tb.TypeBase == TypeBase.Int || ta.TypeBase == TypeBase.Double)) Error("'{0}' requires int or double type", tk.strRead);

			if (ta.IsConst && tb.IsConst)
			{
				ExprType rr;
				switch (tk.strRead)
				{
				case "+": rr = ta + tb; break;
				case "-": rr = ta - tb; break;
				case "*": rr = ta * tb; break;
				case "/": rr = ta / tb; break;
				case "%": rr = ta % tb; break;
				case "|": rr = ta | tb; break;
				case "&": rr = ta & tb; break;
				case "^": rr = ta ^ tb; break;
				case "<<": rr = ExprType.lsh(ta, tb); break;
				case ">>": rr = ExprType.rsh(ta, tb); break;
				default: Debug.Assert(false); rr = null; break;
				}
				return rr;
			}
			return new ExprType(ta.Type);
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var t = this.CheckType(ctx);

			if (t.IsConst)
			{
				return new ExprValue(t);
			}
			else
			{
				var aa = a.GenRight(ctx, null);
				var bb = b.GenRight(ctx, null);

				if (rdest == null) rdest = ctx.NewTmp();
				switch (tk.strRead)
				{
				case "+": ctx.Context.add(rdest, aa, bb); break;
				case "-": ctx.Context.sub(rdest, aa, bb); break;
				case "*": ctx.Context.mul(rdest, aa, bb); break;
				case "/": ctx.Context.div(rdest, aa, bb); break;
				case "%": ctx.Context.rem(rdest, aa, bb); break;
				case "|": ctx.Context.or_(rdest, aa, bb); break;
				case "&": ctx.Context.and(rdest, aa, bb); break;
				case "^": ctx.Context.xor(rdest, aa, bb); break;
				case "<<": ctx.Context.shl(rdest, aa, bb); break;
				case ">>": ctx.Context.shr(rdest, aa, bb); break;
				default: Debug.Assert(false); break;
				}

				return new ExprValue(rdest, t.Type);
			}
		}
	}


	public class ExprPlus : ExprUni
	{
		public ExprPlus(TokenAST tk, ExprRoot a) : base(tk, a) { }
		public override ExprType CheckType(FunctionContex ctx)
		{
			var ta = a.CheckType(ctx);
			if (!(ta.IsNumeric)) Error("'{0}' requires int or double type", tk.strRead);
			if (ta.IsConst)
			{
				return ta;
			}
			return ta;
		}
		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var t = CheckType(ctx);
			if (t.IsConst)
				return new ExprValue(t);

			return a.GenRight(ctx, rdest);
		}
	}

	public class ExprNeg : ExprUni
	{
		public ExprNeg(TokenAST tk, ExprRoot a) : base(tk, a) { }

		public override ExprType CheckType(FunctionContex ctx)
		{
			var ta = a.CheckType(ctx);
			if (!(ta.IsNumeric)) Error("'{0}' requires int or double type", tk.strRead);
			if (ta.IsConst)
			{
				/***/if (ta.TypeBase == TypeBase.Int) return new ExprType(0) - ta;
				else if (ta.TypeBase == TypeBase.Double) return new ExprType(0.0) - ta;
				else Debug.Assert(false);
			}
			return ta;
		}
		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var t = CheckType(ctx);
			if (t.IsConst)
			{
				return new ExprValue(t);
			}
			else
			{
				var ra = a.GenRight(ctx, null);
				if (rdest == null) rdest = ctx.NewTmp();
				/***/if (ra.TypeBase == TypeBase.Int) ctx.Context.sub(rdest, new ExprValue(0), ra);
				else if (ra.TypeBase == TypeBase.Double) ctx.Context.sub(rdest, new ExprValue(0.0), ra);
				else Debug.Assert(false);
				return new ExprValue(rdest, a.CheckType(ctx).Type);
			}
		}
	}

	public class ExprCast : ExprRoot
	{
		public ExprCast(TokenAST tk, RefTypeRoot rtype, ExprRoot e) : base(tk) { this.ctype = rtype; this.e = e; }
		readonly RefTypeRoot ctype;
		readonly ExprRoot e;

		public override ExprType CheckType(FunctionContex ctx)
		{
			var ta = e.CheckType(ctx);

			bool error = true;
			if (ctype.TypeBase == TypeBase.Int)
			{
				if (ta.IsNumeric) error = false;
			}
			else if (ctype.TypeBase == TypeBase.Double)
			{
				if (ta.IsNumeric) error = false;
			}

			if (error) Error("cannot cast from '{0}' to '{1}'", ta.Type, ctype);

			if (ta.IsConst)
			{
				if (ctype.TypeBase == TypeBase.Int)
				{
					/***/if (ta.TypeBase == TypeBase.Int) return new ExprType((double)ta.Int);
					else if (ta.TypeBase == TypeBase.Double) return new ExprType((int)ta.Dbl);
				}
				else if (ctype.TypeBase == TypeBase.Double)
				{
					/***/if (ta.TypeBase == TypeBase.Int) return new ExprType((double)ta.Int);
					else if (ta.TypeBase == TypeBase.Double) return new ExprType((double)ta.Dbl);
				}
			}
			return new ExprType(ctype);
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var t = CheckType(ctx);
			if (t.IsConst)
				return new ExprValue(t);

			var te = e.CheckType(ctx);
			if (te.Type == ctype)
			{
				// nessuna conversione
				return e.GenRight(ctx, rdest);
			}
			else if (ctype.TypeBase == TypeBase.Int)
			{
				var aa = e.GenRight(ctx, null);
				if (rdest == null) rdest = ctx.NewTmp();
				/***/if (te.TypeBase == TypeBase.Int) Debug.Assert(false);
				else if (te.TypeBase == TypeBase.Double) ctx.Context.d2i(rdest, aa.Reg);
				else Debug.Assert(false);
				return new ExprValue(rdest, ctype);
			}
			else if (ctype.TypeBase == TypeBase.Double)
			{
				var aa = e.GenRight(ctx, null);
				if (rdest == null) rdest = ctx.NewTmp();
				/***/if (te.TypeBase == TypeBase.Int) ctx.Context.i2d(rdest, aa.Reg);
				else if (te.TypeBase == TypeBase.Double) Debug.Assert(false);
				else Debug.Assert(false);
				return new ExprValue(rdest, ctype);
			}
			else
			{
				Debug.Assert(false);
				return null;
			}
		}
	}

	public class ExprNewObj : ExprRoot
	{
		public ExprNewObj(TokenAST tk, RefTypeSimple id, ExprList el) : base(tk) { this.type = id; this.el = el; }
		readonly RefTypeSimple type;
		readonly ExprList el;

		DeclClass cls;
		DeclFun fun;

		public override ExprType CheckType(FunctionContex ctx)
		{
			type.ResolveType(ctx.fun);
			cls = type.DeclRoot as DeclClass;
			if (cls == null)
				Error("cannot find class '{0}'", this.type.ToString());

			var et = new List<RefTypeRoot>();
			foreach (var e in el)
				et.Add(e.CheckType(ctx).Type);

			this.fun = cls.GetFun(cls.name.strRead, et);
			if (this.fun == null)
				Error("cannot find constructor with {0} arguments of class '{1}'", et.Count, this.type);

			return new ExprType(this.type);
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var t = CheckType(ctx);
			Debug.Assert(t.IsConst == false);

			int sz = cls.MemberSize(ctx.fun);

			ctx.Context.newobj("rp", sz, cls.vt);

			for (int i = 0; i < this.el.Count; ++i)
			{
				var ra = this.el[i].GenRight(ctx, "rp");
				if (ra.IsReg == false || ra.Reg != "rp") ctx.Context.ld("rp", ra);
			}
			if (rdest == null) rdest = ctx.NewTmp();
			ctx.Context.js(rdest, new Label(this.fun.AssName), t);
			return new ExprValue(rdest, t.Type);
		}
	}

	public class ExprNewArray : ExprRoot
	{
		public ExprNewArray(TokenAST tk, RefTypeSimple id, ExprRankList el) : base(tk) { this.type = id; this.el = el; }
		readonly RefTypeSimple type;
		readonly ExprRankList el;

		DeclRoot cls;

		public override ExprType CheckType(FunctionContex ctx)
		{
			type.ResolveType(ctx.fun);

			foreach (var e in this.el[0])
			{
				var te = e.CheckType(ctx);
				if (te.Type.TypeBase != TypeBase.Int)
					Error("int expression required in array index");
			}


			cls = type.DeclRoot;
			if (cls == null)
				Error("cannot find type '{0}'", this.type.ToString());

			if (this.el.Count > 1)
			{
				// ci sono altri jagged array.
				Debug.Assert(false);

			}
			return new ExprType(new RefTypeArray(type, this.el[0].Count));
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var t = CheckType(ctx);
			Debug.Assert(t.IsConst == false);

			int rank = this.el[0].Count;
			foreach (var e in this.el[0])
			{
				var te = e.CheckType(ctx);
				if (te.Type.TypeBase != TypeBase.Int)
					Error("int expression required in array index");
			}

			t.Type.ResolveType(ctx.fun);
			DeclClass dc = t.Type.DeclRoot as DeclClass;

			for (int i = 0; i < this.el[0].Count; ++i)
			{
				var ra = this.el[0][i].GenRight(ctx, "rp");
				if (ra.IsReg == false || ra.Reg != "rp") ctx.Context.ld("rp", ra);
			}
			if (rdest == null) rdest = ctx.NewTmp();
			ctx.Context.newarray(rdest, rank, dc.vt);
			return new ExprValue(rdest, t.Type);
		}
	}

	public class ExprRankList : List<ExprList>, IAST
	{
		public new ExprRankList Add(ExprList a) { base.Add(a); return this; }
	}

	public class ExprNum : ExprRoot
	{
		public ExprNum(TokenAST a) : base(a) { this.a = a; }
		readonly TokenAST a;

		public override ExprType CheckType(FunctionContex ctx)
		{
			return new ExprType(int.Parse(a.strRead));
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var t = CheckType(ctx);
			return new ExprValue(t);

		}
	}
	public class ExprChr : ExprRoot
	{
		public ExprChr(TokenAST a) : base(a) { this.a = a; }
		readonly TokenAST a;

		public override ExprType CheckType(FunctionContex ctx)
		{
			return new ExprType(char.Parse(a.strRead));
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var t = CheckType(ctx);
			return new ExprValue(t);

		}
	}

	public class ExprStr : ExprRoot
	{
		public ExprStr(TokenAST a) : base(a) { this.a = a; }
		readonly TokenAST a;

		public override ExprType CheckType(FunctionContex ctx)
		{
			var t = new RefTypeSimple("System").Add("String");
			t.ResolveType(ctx.fun);
			return new ExprType(t);
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var te = CheckType(ctx);

			var lbl = ctx.NewLbl();
			ctx.Context.dataLbl(lbl);
			ctx.Context.putString(a.strRead);

			var cls = te.Type.DeclRoot as DeclClass;
			int sz = cls.MemberSize(ctx.fun);

			var rd = rdest ?? ctx.NewTmp();
			ctx.Context.newobj(rd, sz, cls.vt);
			ctx.Context.stm(rd, 0, new ExprValue(lbl));
			return new ExprValue(rd, te.Type);
		}
	}


	public class ExprBool : ExprRoot
	{
		public ExprBool(TokenAST tk, bool v) : base(tk) { this.v = v; }
		readonly bool v;

		public override ExprType CheckType(FunctionContex ctx)
		{
			return new ExprType(v);
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var t = CheckType(ctx);
			return new ExprValue(t);
		}
	}

	public class Null {
		public override string ToString() { return "0"; }
	}

	public class ExprNull : ExprRoot
	{
		public ExprNull(TokenAST a) : base(a) { this.a = a; }
		public readonly TokenAST a;

		public override ExprType CheckType(FunctionContex ctx)
		{
			return new ExprType(new Null(), new RefTypeSimple("Object"));
		}
		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var t = CheckType(ctx);
			return new ExprValue(t);
		}
	}


	public class ExprId : ExprRoot
	{
		public ExprId(TokenAST a) : base(a) { this.a = a; }
		public readonly TokenAST a;

		public override ExprType CheckType(FunctionContex ctx)
		{
			var v = ctx.GetVar(tk);
			return new ExprType(v.Type);
		}

		public override string GenLeftPre(FunctionContex ctx)
		{
			return ctx.GetVar(a).Reg;
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var rvar = ctx.GetVar(a);
			if (rdest != null) { ctx.Context.ld(rdest, rvar); return new ExprValue(rdest, rvar.Type); }
			return rvar;
		}
	}
	public class ExprThis : ExprRoot
	{
		public ExprThis(TokenAST a) : base(a) { this.a = a; }
		public readonly TokenAST a;

		public override ExprType CheckType(FunctionContex ctx)
		{
			var v = ctx.GetVar(tk);
			return new ExprType(v.Type);
		}

		public override string GenLeftPre(FunctionContex ctx)
		{
			return ctx.GetVar(a).Reg;
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var rvar = ctx.GetVar(a);
			if (rdest != null) { ctx.Context.ld(rdest, rvar); return new ExprValue(rdest, rvar.Type); }
			return rvar;
		}
	}

	

	public class ExprDot : ExprRoot
	{
		public ExprDot(ExprRoot e, TokenAST a) : base(a) { this.e = e; this.a = a; }

		public readonly ExprRoot e;
		public readonly TokenAST a;

		int _off_var;
		DeclVar _var;
		DeclFun _fun;
		DeclProp _prop;

		public override ExprType CheckType(FunctionContex ctx)
		{
			var te = e.CheckType(ctx);
			te.Type.ResolveType(ctx.fun);

			if (te.TypeBase == TypeBase.Obj == false) 
				Error("'.' operators require an object on left side");

			var cls = te.Type.DeclRoot as DeclClass;

			int off_var = 0;
			foreach (var m in cls.members)
			{
				if (m is DeclFun)
				{
					if (m.name.strRead == a.strRead)
					{
						_fun = (DeclFun)m;
						Debug.Assert(false);
						return null;
						//return new ExprType(new TypeFun(cls, m.name.strRead));
					}
				}
				else if (m is DeclVar) 
				{
					if (m.name.strRead == a.strRead)
					{
						_var = (DeclVar)m;
						_off_var = off_var;
						return new ExprType(((DeclVar)m).Type);
					}
					off_var += 1;
				}
				else if (m is DeclProp)
				{
					if (m.name.strRead == a.strRead)
					{
						_prop = ((DeclProp)m);
						if (_prop.fget == null)
							Error("cannot call property get");

						return new ExprType(_prop.fget.ret);
						//return new ExprType(new TypeFun(cls, m.name.strRead));
					}
				}
				else Debug.Assert(false);
			}
			Error(U.F("'{0}' member not found", a.strRead));
			return null;
		}

		public override string GenLeftPre(FunctionContex ctx)
		{
			// una espressione a.b non può essere valuta con un solo registro
			// per cui si può assegnare solo con GenLeftPost
			CheckType(ctx);
			return null;
		}

		public override ExprValue GenLeftPost(FunctionContex ctx, ExprValue v)
		{
			var eaa = e.GenRight(ctx, null);
			if (_var != null)
				ctx.Context.stm(eaa.Reg, _off_var, v);
			else if (_prop != null)
			{
				var ea = e.GenRight(ctx, "rp");
				ctx.Context.ld("rp", v);
				ctx.Context.js(null, new Label(_prop.fset.AssName), new ExprType(_prop.fset.ret));
				return new ExprValue(new ExprType(RefTypeRoot.Void));
			}
			else
				Debug.Assert(false);
			return v;
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var te = CheckType(ctx);
			if (_fun != null)
			{
				var ev = e.GenRight(ctx, null);
				return new ExprValue(ev.Reg, te.Type);
			}
			else if (_var != null)
			{
				var ea = e.GenRight(ctx, null);
				if (rdest == null) rdest = ctx.NewTmp();
				ctx.Context.ldm(rdest, ea, _off_var, te);
				return new ExprValue(rdest, _var.Type);
			}
			else if (_prop != null)
			{
				var ea = e.GenRight(ctx, "rp");
				if (rdest == null) rdest = ctx.NewTmp();
				ctx.Context.js(rdest, new Label(_prop.fget.AssName), new ExprType(_prop.fget.ret));
				return new ExprValue(rdest, _prop.fget.ret);
			}
			else
			{
				Debug.Assert(false);
				return null;
			}
		}
	}

	public class ExprFun : ExprRoot
	{
		readonly ExprRoot e;
		readonly TokenAST fun;
		readonly ExprList a;

		DeclFun _df;

		public ExprFun(ExprRoot e, TokenAST tk, ExprList a) : base(tk) 
		{
			if (e is ExprDot)
			{
				// una cosa del tipo a.bb()
				this.e = ((ExprDot)e).e;
				this.fun = ((ExprDot)e).a;
			}
			else if (e is ExprId)
			{
				// una cosa del tipo bb()
				this.e = null;
				this.fun = ((ExprId)e).a;
			}
			else
			{
				// una cosa del tipo a[i]()... un delegato...
				this.e = e;
			}
			this.a = a;
		}

		public override ExprType CheckType(FunctionContex ctx)
		{
			ExprType te = null;
			if (this.e != null)
			{
				te = e.CheckType(ctx);
				te.Type.ResolveType(ctx.fun);

				if (te.TypeBase == TypeBase.Obj == false)
					Error("object required");

				// TODO ... prima era cosi
				// non è giusto, ma per ora puo' andare.....
				//if (te.Type.TypeName[te.Type.TypeName.Count - 1] == this.fun.strRead)
				//	Error("cannot call class construct - use new operator");
			}


			var targs = new List<RefTypeRoot>();
			foreach (var ai in this.a)
				targs.Add(ai.CheckType(ctx).Type);

			_df = ctx.fun.GetFun(fun.strRead, targs);
			if (_df == null)
				Error("function '{0}' not found", fun.strRead);

			if (te == null && _df.IsStatic == false)
					Error("cannot call member function", fun.strRead);
			if (te != null && _df.IsStatic == false)
					Error("cannot call static function", fun.strRead);

			return new ExprType(_df.ret);
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var te = this.CheckType(ctx);

			var targs = new List<ExprType>();
			foreach (var e in this.a)
				targs.Add(e.CheckType(ctx));

			if (this.e != null)
			{
				var ra = this.e.GenRight(ctx, "rp");
				if (ra.IsReg == false || ra.Reg != "rp") ctx.Context.ld("rp", ra);
			}
			for (int i = 0; i < this.a.Count; ++i)
			{
				var ra = this.a[i].GenRight(ctx, "rp");
				if (ra.IsReg == false || ra.Reg != "rp") ctx.Context.ld("rp", ra);
			}
			if (rdest == null) rdest = ctx.NewTmp();
			ctx.Context.js(rdest, new Label(this._df.AssName), te);
			return new ExprValue(rdest, te.Type);
		}
	}
	public class ExprArray : ExprRoot
	{
		readonly ExprRoot e;
		readonly ExprList a;

		public ExprArray(ExprRoot e, TokenAST tk, ExprList a)
			: base(tk)
		{
			this.e = e;
			this.a = a;
		}

		public override ExprType CheckType(FunctionContex ctx)
		{
			var te = e.CheckType(ctx);
			if (te.TypeBase != TypeBase.Obj)
				Error("array required");

			var ta = te.Type as RefTypeArray;
			if (ta == null)
				Error("array required");

			// TODO controllare il rank

			var targs = new List<RefTypeRoot>();
			foreach (var ai in this.a)
				targs.Add(ai.CheckType(ctx).Type);

			return new ExprType(ta.t);
		}
		public override string GenLeftPre(FunctionContex ctx)
		{
			// una espressione a[x,yz] non può essere valuta con un solo registro
			// per cui si può assegnare solo con GenLeftPost
			CheckType(ctx);
			return null;
		}

		public override ExprValue GenLeftPost(FunctionContex ctx, ExprValue v)
		{
			var ea = e.GenRight(ctx, null);
			//TODO ctx.Context.stm(ea.Reg, _off_var, v);
			return v;
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var te = this.CheckType(ctx);

			var targs = new List<ExprType>();
			foreach (var e in this.a)
				targs.Add(e.CheckType(ctx));

			var ra = this.e.GenRight(ctx, "rp");
			if (ra.IsReg == false || ra.Reg != "rp") ctx.Context.ld("rp", ra);

			ctx.Context.ld("rp", this.a.Count);

			for (int i = 0; i < this.a.Count; ++i)
			{
				ra = this.a[i].GenRight(ctx, "rp");
				if (ra.IsReg == false || ra.Reg != "rp") ctx.Context.ld("rp", ra);
			}
			if (rdest == null) rdest = ctx.NewTmp();
			// TODO
			//ctx.Context.js(rdest, this._df.AssName, te);
			return new ExprValue(rdest, te.Type);
		}
	}

	public class ExprList : IAST, IEnumerable<ExprRoot>
	{
		readonly List<ExprRoot> a;

		public ExprList() { a = new List<ExprRoot>(); }
		public ExprList(ExprRoot e) { a = new List<ExprRoot>(); Add(e); }
		public ExprList Add(ExprRoot e) { a.Add(e); return this; }

		public int Count { get { return a.Count; } }
		public IEnumerator<ExprRoot> GetEnumerator() { return a.GetEnumerator(); }
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return a.GetEnumerator(); }

		public ExprRoot this[int i]
		{
			get { return a[i]; }
		}
	}
	////////////////////////

	public partial class MParser
	{
		public MParser() : base(0) { }

		public DeclGlobal Start(LexReader rd)
		{
			this.init(rd);
			var v = this.start(null);
			return new DeclGlobal(v);
		}
	}
}
