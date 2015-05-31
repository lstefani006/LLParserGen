using System;
using System.Diagnostics;
using LLParserLexerLib;
using System.Collections.Generic;

namespace LLParserGenTest
{
	class _
	{
		public static void Main(string[] args)
		{
			using (var rd = (args.Length == 1 ? new LexReader(args[0]) : new LexReader(Console.In, "stdin")))
			{
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
		Int_,
		Hndl,
		Dbl_,
		Void,
		//Array,
		Obj_,
		//Fun,
	}

	public abstract class TypeRoot : IAST
	{
		public abstract TypeBase TypeBase { get; }

		public virtual string TypeName { get { Debug.Assert(false); return null; } }

		protected abstract bool TypeEqual(TypeRoot t);

		public override bool Equals(object obj) { return this.TypeEqual(obj as TypeRoot); }
		public override int GetHashCode() { return base.GetHashCode(); }

		public abstract string AssName { get; }

		public static TypeSimple Bool = new TypeSimple("bool");
		public static TypeSimple Int = new TypeSimple("int");
		public static TypeSimple Dbl = new TypeSimple("double");
		public static TypeSimple Char = new TypeSimple("char");
		public static TypeSimple Hndl = new TypeSimple("handle");
		public static TypeSimple Void = new TypeSimple("void");


		public static bool operator == (TypeRoot a, TypeRoot b)
		{
			if (((object)a) == null && (object)b == null) return true;
			if (((object)a) == null && ((object)b) != null) return false;
			if (((object)a) != null && ((object)b) == null) return false;
			return a.TypeEqual(b) == true;
		}
		public static bool operator !=(TypeRoot a, TypeRoot b) { return !(a == b); }
	}



	public class TypeSimple : TypeRoot
	{
		string id;

		public TypeSimple(TokenAST id) { this.id = id.strRead; }
		public TypeSimple(string id) { this.id = id; }

		public override TypeBase TypeBase
		{
			get
			{
				if (this.id == "bool") return TypeBase.Bool;
				if (this.id == "char") return TypeBase.Char;
				if (this.id == "handle") return TypeBase.Hndl;
				if (this.id == "int") return TypeBase.Int_;
				if (this.id == "void") return TypeBase.Void;
				if (this.id == "double") return TypeBase.Dbl_;
				return TypeBase.Obj_;
			}
		}
		public override string TypeName { get { Debug.Assert(TypeBase == TypeBase.Obj_); return id; } }

		public TypeSimple Add(TokenAST id)
		{
			this.id += "." + id.strRead;
			return this;
		}

		protected override bool TypeEqual(TypeRoot t)
		{
			var st = t as TypeSimple;
			if ((object)st != null)
				return this.id == st.id;
			return false;
		}

		public override string ToString() { return this.id; }
		public override string AssName { 
			get 
			{
				switch (TypeBase)
				{
				case TypeBase.Int_: return "$i";
				case TypeBase.Bool: return "$b";
				case TypeBase.Char: return "$c";
				case TypeBase.Void: return "$v";
				case TypeBase.Obj_: return "$o" + this.id;
				default: Debug.Assert(false); break;
				}
				return null;
			} 
		}
	}

	public class TypeArray : TypeRoot
	{
		public TypeArray(TypeRoot t) { this.t = t; }
		public readonly TypeRoot t;
		public override TypeBase TypeBase { get { return TypeBase.Obj_; } }

		protected override bool TypeEqual(TypeRoot t)
		{
			var at = t as TypeArray;
			if ((object)at != null)
				return this.t == at.t;
			return false;
		}

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

	public class TypeRootList : List<TypeRoot>, IAST
	{
		public new TypeRootList Add(TypeRoot a) { base.Add(a); return this; }
	}

	public class TypeRoot_or_Base : IAST
	{
		public readonly TypeRoot t;
		public readonly BaseInit b;
		public TypeRoot_or_Base(TypeRoot t) { this.t = t; }
		public TypeRoot_or_Base(BaseInit b) { this.b = b; }
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
		public ExprType(TypeRoot t) { this._type = t; }
		public ExprType(bool c) { this._const = c; this._type = TypeSimple.Bool; }
		public ExprType(int c) { this._const = c; this._type = TypeSimple.Int; }
		public ExprType(char c) { this._const = c; this._type = TypeSimple.Char; }
		public ExprType(double d) { this._const = d; this._type = TypeSimple.Dbl; }
		public ExprType(Null obj, TypeRoot r) { this._const = obj; this._type = r; }

		readonly object _const;
		readonly TypeRoot _type;

		public TypeRoot Type { get { return _type; } }

		public bool IsNumeric { get { return _type.TypeBase == TypeBase.Int_ || _type.TypeBase == TypeBase.Dbl_; } }
		public bool IsBool { get { return _type.TypeBase == TypeBase.Bool; } }

		public TypeBase TypeBase { get { return _type.TypeBase; } }

		// costanti
		public bool IsConst { get { return _const != null; } }
		public int Int { get { Debug.Assert(TypeBase == TypeBase.Int_); return (int)_const; } }
		public double Dbl { get { Debug.Assert(TypeBase == TypeBase.Dbl_); return (double)_const; } }
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
			case TypeBase.Int_: return a.Int.CompareTo(b.Int);
			case TypeBase.Dbl_: return a.Dbl.CompareTo(b.Dbl);
			case TypeBase.Obj_:
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
			if (a._type.TypeBase == TypeBase.Int_) return new ExprType(a.Int + b.Int);
			if (a._type.TypeBase == TypeBase.Dbl_) return new ExprType(a.Dbl + b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator -(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int_) return new ExprType(a.Int - b.Int);
			if (a._type.TypeBase == TypeBase.Dbl_) return new ExprType(a.Dbl - b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator *(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int_) return new ExprType(a.Int * b.Int);
			if (a._type.TypeBase == TypeBase.Dbl_) return new ExprType(a.Dbl * b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator /(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int_) return new ExprType(a.Int / b.Int);
			if (a._type.TypeBase == TypeBase.Dbl_) return new ExprType(a.Dbl / b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator %(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int_) return new ExprType(a.Int % b.Int);
			if (a._type.TypeBase == TypeBase.Dbl_) return new ExprType(a.Dbl % b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator |(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int_) return new ExprType(a.Int | b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprType operator &(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int_) return new ExprType(a.Int & b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprType operator ^(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int_) return new ExprType(a.Int ^ b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprType lsh(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int_) return new ExprType(a.Int << b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprType rsh(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.TypeBase == TypeBase.Int_) return new ExprType(a.Int >> b.Int);
			throw new InvalidOperatorException();
		}
	}

	/// <summary>
	/// Descrive il tipo e il valore constante o in un registro associato
	/// </summary>
	public class ExprValue
	{
		public ExprValue(string reg, TypeRoot t) { this._reg = reg; this._type = t; }

		public ExprValue(int c) { this._const = c; this._type = TypeSimple.Int; }
		public ExprValue(bool c) { this._const = c; this._type = TypeSimple.Bool; }
		public ExprValue(double d) { this._const = d; this._type = TypeSimple.Dbl; }
		public ExprValue(char d) { this._const = d; this._type = TypeSimple.Char; }

		public ExprValue(Label lbl) { this._const = lbl; this._type = TypeSimple.Int; }
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
				case TypeBase.Int_: _const = t.Int; break;
				case TypeBase.Dbl_: _const = t.Dbl; break;
				case TypeBase.Obj_: _const = new Null(); break;
				default: Debug.Assert(false); break;
				}
			}
		}

		string _reg;
		readonly object _const;
		readonly TypeRoot _type;

		public override string ToString()
		{
			if (_reg != null) return _reg;
			if (TypeBase == LLParserGenTest.TypeBase.Char)
				return U.F("'{0}'", _const);
			return _const.ToString();
		}


		public bool IsConst { get { return _const != null; } }
		public bool IsReg { get { return _reg != null; } }



		public TypeRoot Type { get { return _type; } }
		public TypeBase TypeBase { get { return _type.TypeBase; } }

		public int Int { get { Debug.Assert(_type.TypeBase == TypeBase.Int_); return (int)_const; } }
		public double Dbl { get { Debug.Assert(_type.TypeBase == TypeBase.Dbl_); return (double)_const; } }
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
	}

	public class DeclList : List<DeclRoot>, IAST
	{
		public new DeclList Add(DeclRoot a) { base.Add(a); return this; }
		public void SetFather(DeclRoot a)
		{
			foreach (var e in this)
				e.SetFather(a);
		}
	}

	public class DeclGlobal : DeclRoot
	{
		public readonly DeclList members;
		public DeclGlobal(DeclList members) : base(null) { this.members = members; }

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
	}
	public class DeclNamespace : DeclRoot
	{
		public readonly DeclList members;
		public readonly TokenAST id;
		public DeclNamespace(TokenAST id, DeclList members) : base(id) { this.id = id;  this.members = members; }

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

		public override string AssName { get { return "$n" + id.strRead; } }
	}

	public class DeclClass : DeclRoot
	{
		public readonly DeclList members;
		public readonly TypeRootList baseList;
		public Label vt;

		public DeclClass(TokenAST name, TypeRootList baseList, DeclList members) : base(name) { this.members = members; this.baseList = baseList; }

		public override void SetFather(DeclRoot p)
		{
			base.SetFather(p);
			members.SetFather(this);
		}
		public override string AssName { get { return this.Father.AssName + "$c" + this.name.strRead; } }

		public override void CheckSyntax(DeclGlobal dg)
		{
			if (this.baseList.Count == 0)
			{
				if (this.name.strRead != "object")
					this.baseList.Add(new TypeSimple("object"));
			}

			foreach (var d in this.members)
				d.CheckSyntax(dg);
		}
		public override void GenCode(Context ctx)
		{
			this.vt = new Label(AssName + "$vt");
			ctx.dataLbl(vt);
			foreach (var f in members)
				if (f is DeclFun)
					ctx.putInt(new Label(f.AssName));

			foreach (var d in members)
				d.GenCode(ctx);
		}
	}

	public class DeclVar : DeclRoot
	{
		public readonly TypeRoot Type;
		public DeclVar(TokenAST name, TypeRoot type) : base(name) { this.Type = type; }
		public override string AssName { get { return this.name.strRead; } }
		public override void CheckSyntax(DeclGlobal dg)
		{
		}
		public override void GenCode(Context ctx)
		{
		}

	}

	public class DeclFun : DeclRoot
	{
		public readonly FunArgList args;
		public  BaseInit baseInit;
		public  TypeRoot ret;
		public readonly StmtRoot body;
		public readonly TokenAST lastCurly;
		public readonly TokenAST funMod;
		public bool constructor;

		public DeclFun(TokenAST funMod, TokenAST name, FunArgList args, TypeRoot_or_Base tb, StmtRoot body, TokenAST lastCurly)
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

		public override string AssName { get { return this.Father.AssName + "$f" + this.name.strRead + this.args.AssName; } }
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
					this.ret = new TypeSimple(this.name);   // il costruttore ritorna sempre this.

					// se non chiama esplicitamente base o this vuole dire che chiama implicitamente base()
					if (this.baseInit == null && this.name.strRead != "object")
						this.baseInit = new BaseInit(new TokenAST(this.name, MParser.BASE, "BASE", "base"), new ExprList());
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
						fctx.AddArgVar(new TokenAST(this.name, MParser.THIS, "THIS", "this"), new TypeSimple(this.Father.name.strRead));

					foreach (var a in this.args)
						fctx.AddArgVar(a.ArgName, a.ArgType);
				}

				ctx.codeLbl(new Label(this.AssName));
				if (this.Father is DeclClass)
				{
					// la cosa è complicata... devo cercare il : base(....) se c'è altrimenti si suppone che ci sia il costruttore base()
					var c = this.Father as DeclClass;
					if (c.baseList.Count > 0)
					{
					}
				}
				bool unreachable = this.body.GenCode(fctx);
				if (unreachable == false)
				{
					if (this.ret.TypeBase != TypeBase.Obj_ && this.constructor == false)
						Error("missing return stmt");

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
	}

	public class FunArg
	{
		public FunArg(TokenAST argName, TypeRoot argType)
		{
			this.ArgName = argName;
			this.ArgType = argType;
		}
		public readonly TokenAST ArgName;
		public readonly TypeRoot ArgType;
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
		public FunArgList Add(TokenAST arg, TypeRoot ty) { base.Add(new FunArg(arg, ty)); return this; }
	}

	/////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////


	public abstract class StmtRoot : NodeRoot
	{
		// ritorna true se next unreachable
		public abstract bool GenCode(FunctionContex ctx);

		protected StmtRoot(TokenAST tk) : base(tk) { }
	}

	public class StmtList : StmtRoot
	{
		readonly List<StmtRoot> a;

		public StmtList() : base(null) { this.a = new List<StmtRoot>(); }
		public StmtList(StmtRoot s) : base(s.tk) { this.a = new List<StmtRoot>(); this.Add(s); }

		public StmtList Add(StmtRoot s) { this.a.Add(s); return this; }

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
		public readonly TypeRoot type;
		public StmtVar(TokenAST tk, TokenAST a, TypeRoot ty) : base(tk) { this.a = a; this.type = ty; }

		public override bool GenCode(FunctionContex ctx)
		{
			ctx.AddDefVar(this.a, type);
			ctx.Push(FunctionContex.StmtTk.Var, null, null, this.a);
			return false;
		}
	}
	public class StmtBlock : StmtRoot
	{
		readonly StmtRoot sa;

		public StmtBlock(TokenAST tk, StmtRoot a) : base(tk) { this.sa = a; }
		public StmtBlock(StmtRoot a) : base(a.tk) { this.sa = a; }

		public override bool GenCode(FunctionContex ctx)
		{
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

		public override bool GenCode(FunctionContex ctx)
		{
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

		public override bool GenCode(FunctionContex ctx)
		{
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

		public override bool GenCode(FunctionContex ctx)
		{
			if (ctx.fun.constructor)
			{
				if (this.e != null) Error("constructs cannot return value");
				var re = new ExprId(new TokenAST(this.tk, MParser.THIS, "THIS", "this"));
				var r = re.GenRight(ctx, null);
				ctx.Return(r);
				ctx.Context.ret(r);
				return true;
			}

			if (ctx.fun.ret.TypeBase == TypeBase.Void && e != null) Error("return expression with void function");
			if (ctx.fun.ret.TypeBase != TypeBase.Void && e == null) Error("return without value");

			if (ctx.fun.ret.TypeBase != TypeBase.Void)
			{
				var te = e.CheckType(ctx);
				TypeRoot aaa = te.Type;
				TypeRoot bbb = ctx.fun.ret;

				if (!(aaa == bbb)) 
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
			return new ExprType(TypeSimple.Bool);
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
					return new ExprValue(rdest, TypeSimple.Bool);
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
					return new ExprValue(rdest, TypeSimple.Bool);
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
				if (!(ta.TypeBase == TypeBase.Int_ || ta.TypeBase == TypeBase.Dbl_)) error = true;
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

			return new ExprType(TypeSimple.Bool);
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
				return new ExprValue(rdest, TypeSimple.Bool);
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
			if (!(ta.TypeBase == TypeBase.Int_ || tb.TypeBase == TypeBase.Dbl_)) Error("'{0}' requires int or double type", tk.strRead);
			if (!(tb.TypeBase == TypeBase.Int_ || ta.TypeBase == TypeBase.Dbl_)) Error("'{0}' requires int or double type", tk.strRead);

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
				/***/
				if (ta.TypeBase == TypeBase.Int_) return new ExprType(0) - ta;
				else if (ta.TypeBase == TypeBase.Dbl_) return new ExprType(0.0) - ta;
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
				/***/
				if (ra.TypeBase == TypeBase.Int_) ctx.Context.sub(rdest, new ExprValue(0), ra);
				else if (ra.TypeBase == TypeBase.Dbl_) ctx.Context.sub(rdest, new ExprValue(0.0), ra);
				else Debug.Assert(false);
				return new ExprValue(rdest, a.CheckType(ctx).Type);
			}
		}
	}

	public class ExprCast : ExprRoot
	{
		public ExprCast(TokenAST tk, TypeRoot rtype, ExprRoot e) : base(tk) { this.ctype = rtype; this.e = e; }
		readonly TypeRoot ctype;
		readonly ExprRoot e;

		public override ExprType CheckType(FunctionContex ctx)
		{
			var ta = e.CheckType(ctx);

			bool error = true;
			if (ctype.TypeBase == TypeBase.Int_)
			{
				if (ta.IsNumeric) error = false;
			}
			else if (ctype.TypeBase == TypeBase.Dbl_)
			{
				if (ta.IsNumeric) error = false;
			}

			if (error) Error("cannot cast from '{0}' to '{1}'", ta.Type, ctype);

			if (ta.IsConst)
			{
				if (ctype.TypeBase == TypeBase.Int_)
				{
					/***/
					if (ta.TypeBase == TypeBase.Int_) return new ExprType((double)ta.Int);
					else if (ta.TypeBase == TypeBase.Dbl_) return new ExprType((int)ta.Dbl);
				}
				else if (ctype.TypeBase == TypeBase.Dbl_)
				{
					/***/
					if (ta.TypeBase == TypeBase.Int_) return new ExprType((double)ta.Int);
					else if (ta.TypeBase == TypeBase.Dbl_) return new ExprType((double)ta.Dbl);
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
			else if (ctype.TypeBase == TypeBase.Int_)
			{
				var aa = e.GenRight(ctx, null);
				if (rdest == null) rdest = ctx.NewTmp();
				/***/
				if (te.TypeBase == TypeBase.Int_) Debug.Assert(false);
				else if (te.TypeBase == TypeBase.Dbl_) ctx.Context.d2i(rdest, aa.Reg);
				else Debug.Assert(false);
				return new ExprValue(rdest, ctype);
			}
			else if (ctype.TypeBase == TypeBase.Dbl_)
			{
				var aa = e.GenRight(ctx, null);
				if (rdest == null) rdest = ctx.NewTmp();
				/***/
				if (te.TypeBase == TypeBase.Int_) ctx.Context.i2d(rdest, aa.Reg);
				else if (te.TypeBase == TypeBase.Dbl_) Debug.Assert(false);
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
		public ExprNewObj(TokenAST tk, TokenAST id, ExprList el) : base(tk) { this.id = id; this.el = el; }
		readonly TokenAST id;
		readonly ExprList el;

		DeclClass cls;
		DeclFun fun;

		public override ExprType CheckType(FunctionContex ctx)
		{
			cls = ctx.Context.GetClass(this.id.strRead);
			if (cls == null)
				Error("cannot find class '{0}'", this.id.strRead);

			var et = new List<TypeRoot>();
			foreach (var e in el)
				et.Add(e.CheckType(ctx).Type);

			this.fun = ctx.Context.GetFun(this.id.strRead, this.id.strRead, et);
			if (this.fun == null)
				Error("cannot find constructor with {0} arguments of class '{1}'", et.Count, this.id);

			return new ExprType(new TypeSimple(this.id));
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var t = CheckType(ctx);
			Debug.Assert(t.IsConst == false);

			int size = 0;

			var c = cls;
			while (c != null)
			{
				foreach (var m in c.members) if (m is DeclVar) size += 1;
				if (c.baseList.Count == 0) break;
				var tb = c.baseList[0].TypeName;
				c = ctx.Context.GetClass(tb);
				if (c == null)
					Error("cannot find class '{0}'", tb);
			}

			ctx.Context.newobj("rp", size, cls.vt);

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
			return new ExprType(new TypeSimple("string"));
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var lbl = ctx.NewLbl();
			ctx.Context.dataLbl(lbl);
			ctx.Context.putString(a.strRead);

			var cls = ctx.Context.GetClass("string");

			var t = CheckType(ctx);

			var rd = rdest ?? ctx.NewTmp();
			ctx.Context.newobj(rd, 1, cls.vt);
			ctx.Context.stm(rd, 0, new ExprValue(lbl));
			return new ExprValue(rd, new TypeSimple("string"));
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
			return new ExprType(new Null(), new TypeSimple("object"));
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

		public override ExprType CheckType(FunctionContex ctx)
		{
			var te = e.CheckType(ctx);
			if (te.TypeBase == TypeBase.Obj_ == false) 
				Error("'.' operators require an object on left side");

			var cls = ctx.Context.GetClass(te.Type.TypeName);
			if (cls == null)
				Error("class '{0}' not found", te.Type.TypeName);

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
				else 
					Debug.Assert(false);
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
			var ea = e.GenRight(ctx, null);
			ctx.Context.stm(ea.Reg, _off_var, v);
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
				// una cosa del tipo a[i]()... in delegato...
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
				if (te.TypeBase == TypeBase.Obj_ == false)
					Error("object required");

				if (te.Type.TypeName == this.fun.strRead)
					Error("cannot call class construct - use new operator");
			}


			var targs = new List<TypeRoot>();
			foreach (var ai in this.a)
				targs.Add(ai.CheckType(ctx).Type);

			if (te == null)
				_df = ctx.Context.GetFun(fun.strRead, targs);
			else
				_df = ctx.Context.GetFun(te.Type.TypeName, fun.strRead, targs);
			if (_df == null)
				Error("function '{0}' not found", this.fun.strRead);
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
			if (te.TypeBase != TypeBase.Obj_)
				Error("array required");

			var ta = te.Type as TypeArray;
			if (ta == null)
				Error("array required");

			// TODO controllare il rank

			var targs = new List<TypeRoot>();
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
