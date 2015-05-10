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
					var s = p.Start(rd);

					using (Context ctx = new Context())
					{
						ctx.GenerateCode(s);
					}
				}
				catch (SyntaxError ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}
	}

	public abstract class TypeRoot : IAST
	{
		public virtual bool IsBool { get { return false; } }
		public virtual bool IsInt { get { return false; } }
		public virtual bool IsDbl { get { return false; } }
		public virtual bool IsVoid { get { return false; } }
		public virtual bool IsArray { get { return false; } }
		public virtual bool IsObject { get { return false; } }

		protected abstract bool TypeEqual(TypeRoot t);
		public static bool operator ==(TypeRoot a, TypeRoot b)
		{
			return a.TypeEqual(b);
		}
		public static bool operator !=(TypeRoot a, TypeRoot b)
		{
			return a.TypeEqual(b) == false;
		}

		public override bool Equals(object obj)
		{
			TypeRoot t = obj as TypeRoot;
			return this.TypeEqual(t);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static TypeSimple Bool = new TypeSimple("bool");
		public static TypeSimple Int = new TypeSimple("int");
		public static TypeSimple Dbl = new TypeSimple("double");
		public static TypeSimple Void = new TypeSimple("void");

	}

	public class TypeSimple : TypeRoot
	{
		public TypeSimple(TokenAST id) { this.id = id.v; }
		public TypeSimple(string id) { this.id = id; }

		public override bool IsBool { get { return this.id == "bool"; } }
		public override bool IsInt { get { return this.id == "int"; } }
		public override bool IsDbl { get { return this.id == "double"; } }
		public override bool IsVoid { get { return this.id == "void"; } }
		public override bool IsObject { get { return !(IsBool || IsInt || IsVoid || IsDbl); } }
		readonly string id;

		protected override bool TypeEqual(TypeRoot t)
		{
			var st = t as TypeSimple;
			if ((object)st != null)
				return this.id == st.id;
			return false;
		}

		public override string ToString()
		{
			return this.id;
		}


	}

	public class TypeArray : TypeRoot
	{
		public TypeArray(TypeRoot t) { this.t = t; }
		public readonly TypeRoot t;
		public override bool IsArray { get { return true; } }

		protected override bool TypeEqual(TypeRoot t)
		{
			var at = t as TypeArray;
			if ((object)at != null)
				return this.t == at.t;
			return false;
		}

		public override string ToString()
		{
			return t.ToString() + "[]";
		}
	}

	/// <summary>
	/// Descrive un tipo e il valore costante eventualmente associato.
	/// </summary>
	public class ExprType
	{
		public ExprType(TypeRoot t) { this._type = t; }
		public ExprType(int c) { this._const = c; this._type = TypeSimple.Int; }
		public ExprType(bool c) { this._const = c; this._type = TypeSimple.Bool; }
		public ExprType(double d) { this._const = d; this._type = TypeSimple.Dbl; }

		readonly object _const;
		readonly TypeRoot _type;

		public TypeRoot Type { get { return _type; } }

		public int Int { get { Debug.Assert(_type.IsInt); return (int)_const; } }
		public double Dbl { get { Debug.Assert(_type.IsDbl); return (double)_const; } }
		public bool Bool { get { Debug.Assert(_type.IsBool); return (bool)_const; } }

		public bool IsConst { get { return _const != null; } }

		public bool IsBool { get { return _type.IsBool; } }
		public bool IsInt { get { return _type.IsInt; } }
		public bool IsDbl { get { return _type.IsDbl; } }
		public bool IsVoid { get { return _type.IsVoid; } }
		public bool IsObject { get { return _type.IsObject; } }
		public bool IsArray { get { return _type.IsArray; } }


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
			if (a.IsInt) return a.Int.CompareTo(b.Int);
			if (a.IsDbl) return a.Dbl.CompareTo(b.Dbl);
			if (a.IsBool) return a.Bool.CompareTo(b.Bool);
			Debug.Assert(false);
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
			if (a._type.IsInt) return new ExprType(a.Int + b.Int);
			if (a._type.IsDbl) return new ExprType(a.Dbl + b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator -(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.IsInt) return new ExprType(a.Int - b.Int);
			if (a._type.IsDbl) return new ExprType(a.Dbl - b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator *(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.IsInt) return new ExprType(a.Int * b.Int);
			if (a._type.IsDbl) return new ExprType(a.Dbl * b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator /(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.IsInt) return new ExprType(a.Int / b.Int);
			if (a._type.IsDbl) return new ExprType(a.Dbl / b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator %(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.IsInt) return new ExprType(a.Int % b.Int);
			if (a._type.IsDbl) return new ExprType(a.Dbl % b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprType operator |(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.IsInt) return new ExprType(a.Int | b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprType operator &(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.IsInt) return new ExprType(a.Int & b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprType operator ^(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.IsInt) return new ExprType(a.Int ^ b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprType lsh(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.IsInt) return new ExprType(a.Int << b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprType rsh(ExprType a, ExprType b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a._type.IsInt) return new ExprType(a.Int >> b.Int);
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

		public ExprValue(ExprType t)
		{
			_reg = null;
			_type = t.Type;
			if (t.IsConst)
			{
				if (t.IsBool) _const = t.Bool;
				else if (t.IsDbl) _const = t.Dbl;
				else if (t.IsInt) _const = t.Int;
				else Debug.Assert(false);
			}
		}

		string _reg;
		readonly object _const;
		readonly TypeRoot _type;

		public override string ToString()
		{
			if (_reg != null) return _reg;
			return _const.ToString();
		}


		public bool IsConst { get { return _const != null; } }
		public bool IsReg { get { return _reg != null; } }

		public bool IsBool { get { return _type.IsBool; } }
		public bool IsInt { get { return _type.IsInt; } }
		public bool IsDbl { get { return _type.IsDbl; } }
		public bool IsVoid { get { return _type.IsVoid; } }
		public bool IsObject { get { return _type.IsObject; } }
		public bool IsArray { get { return _type.IsArray; } }

		public TypeRoot Type { get { return _type; } }


		public int Int { get { Debug.Assert(_type.IsInt); return (int)_const; } }
		public double Dbl { get { Debug.Assert(_type.IsDbl); return (double)_const; } }
		public bool Bool { get { Debug.Assert(_type.IsBool); return (bool)_const; } }
		public string Reg { get { Debug.Assert(string.IsNullOrEmpty(_reg) == false); return _reg; } }
		public void SetReg(string r) { Debug.Assert(_reg != null); _reg = r; }
	}


	public class InvalidOperatorException : Exception
	{
	}

	public class DeclList : IAST, IEnumerable<DeclRoot>
	{
		public IEnumerator<DeclRoot> GetEnumerator() { return _lst.GetEnumerator(); }
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return _lst.GetEnumerator(); }

		public DeclList() { _lst = new List<DeclRoot>(); }
		public DeclList Add(DeclRoot a) { _lst.Add(a); return this; }
		readonly List<DeclRoot> _lst;
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

	public class DeclRoot : NodeRoot
	{
		public readonly TokenAST name;
		public DeclRoot(TokenAST name) : base(name) { this.name = name; }
	}

	public class DeclClass : DeclRoot
	{
		public readonly DeclList members;
		public DeclClass(TokenAST name, DeclList members) : base(name) { this.members = members; }
	}

	public class DeclVar : DeclRoot
	{
		public readonly TypeRoot type;
		public DeclVar(TokenAST name, TypeRoot type) : base(name) { this.type = type; }
	}

	public class DeclFun : DeclRoot
	{
		public readonly FunArgList args;
		public readonly TypeRoot ret;
		public readonly StmtRoot body;
		public readonly TokenAST lastCurly;

		public DeclFun(TokenAST name, FunArgList args, TypeRoot ret, StmtRoot body, TokenAST lastCurly)
			: base(name)
		{
			this.args = args;
			this.ret = ret;
			this.body = body;
			this.lastCurly = lastCurly;
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

	public class FunArgList : IAST
	{
		public readonly List<FunArg> args = new List<FunArg>();

		public FunArgList() { }
		public FunArgList(TokenAST a, TypeRoot ty) { this.Add(a, ty); }
		public FunArgList Add(TokenAST arg, TypeRoot ty) { args.Add(new FunArg(arg, ty)); return this; }

		public int Count { get { return args.Count; } }
		public FunArg this[int i] { get { return args[i]; } }
	}

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
			if (te.IsBool == false)
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

			if (te.IsBool == false)
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
			if (ctx.fun.ret.IsVoid && e != null) Error("return expression with void function");
			if (ctx.fun.ret.IsVoid == false && e == null) Error("return without value");

			if (ctx.fun.ret.IsVoid == false)
			{
				var te = e.CheckType(ctx);
				if (te.Type != ctx.fun.ret) Error("wrong return type");
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

		public virtual void GenBool(FunctionContex ctx, string lbl_true, string lbl_false)
		{
			var s = GenRight(ctx, null);
			if (s.IsBool == false)
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

		public virtual string GenLeft(FunctionContex ctx)
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

		public override string GenLeft(FunctionContex ctx) { return dst.GenLeft(ctx); }

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
			var ra = dst.GenLeft(ctx);
			var rb = src.GenRight(ctx, ra);
			if (rb.IsReg == false || (rb.IsReg == true && rb.Reg != ra)) ctx.Context.ld(ra, rb);
			if (rdest != null) ctx.Context.ld(rdest, rb);
			return rb;  // qui posso ritornare la costante (se è const oppure la var...)
		}
	}

	public abstract class ExprBin : ExprRoot
	{
		protected ExprBin(ExprRoot a, TokenAST tk, ExprRoot b) : base(tk) { this.a = a; this.b = b; }
		protected ExprRoot a;
		protected ExprRoot b;

		public string op { get { return tk.v; } }
	}

	public abstract class ExprUni : ExprRoot
	{
		protected ExprUni(TokenAST tk, ExprRoot a) : base(tk) { this.a = a; }
		protected ExprRoot a;

		public string op { get { return tk.v; } }
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
					string lbl_false = ctx.NewLbl();
					string lbl_true = ctx.NewLbl();
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
					string lbl_false = ctx.NewLbl();
					string lbl_true = ctx.NewLbl();
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

		public override void GenBool(FunctionContex ctx, string lbl_true, string lbl_false)
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
				if (ta.IsVoid || tb.IsVoid) error = true;
				if (ta.Type != tb.Type) error = true;
				break;
			case ">":
			case ">=":
			case "<":
			case "<=":
				if (ta.Type != tb.Type) error = true;
				if (!(ta.IsInt || ta.IsDbl)) error = true;
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
				string lbl_false = ctx.NewLbl();
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

		public override void GenBool(FunctionContex ctx, string lbl_true, string lbl_false)
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

			if (ta.Type != tb.Type) Error("operator '{0}' requires same types", tk.v);
			if (!(ta.IsInt || ta.IsDbl)) Error("'{0}' requires int or double type", tk.v);
			if (!(tb.IsInt || tb.IsDbl)) Error("'{0}' requires int or double type", tk.v);

			if (ta.IsConst && tb.IsConst)
			{
				ExprType rr;
				switch (tk.v)
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
				switch (tk.v)
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
			if (!(ta.IsInt || ta.IsDbl)) Error("'{0}' requires int or double type", tk.v);
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
			if (!(ta.IsInt || ta.IsDbl)) Error("'{0}' requires int or double type", tk.v);
			if (ta.IsConst)
			{
				/***/
				if (ta.IsInt) return new ExprType(0) - ta;
				else if (ta.IsDbl) return new ExprType(0.0) - ta;
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
				if (ra.IsInt) ctx.Context.sub(rdest, new ExprValue(0), ra);
				else if (ra.IsDbl) ctx.Context.sub(rdest, new ExprValue(0.0), ra);
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
			if (ctype.IsInt)
			{
				if (ta.IsInt || ta.IsDbl) error = false;
			}
			else if (ctype.IsDbl)
			{
				if (ta.IsInt || ta.IsDbl) error = false;
			}

			if (error) Error("cannot cast from '{0}' to '{1}'", ta.Type, ctype);

			if (ta.IsConst)
			{
				if (ctype.IsInt)
				{
					/***/if (ta.IsInt) return new ExprType((double)ta.Int);
					else if (ta.IsDbl) return new ExprType((int)ta.Dbl);
				}
				else if (ctype.IsDbl)
				{
					/***/if (ta.IsInt) return new ExprType((double)ta.Int);
					else if (ta.IsDbl) return new ExprType((double)ta.Dbl);
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
			else if (ctype.IsInt)
			{
				var aa = e.GenRight(ctx, null);
				if (rdest == null) rdest = ctx.NewTmp();
				/***/if (te.IsInt) Debug.Assert(false);
				else if (te.IsDbl) ctx.Context.d2i(rdest, aa.Reg);
				else Debug.Assert(false);
				return new ExprValue(rdest, ctype);
			}
			else if (ctype.IsDbl)
			{
				var aa = e.GenRight(ctx, null);
				if (rdest == null) rdest = ctx.NewTmp();
				/***/if (te.IsInt) ctx.Context.i2d(rdest, aa.Reg);
				else if (te.IsDbl) Debug.Assert(false);
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


	public class ExprNum : ExprRoot
	{
		public ExprNum(TokenAST a) : base(a) { this.a = a; }
		readonly TokenAST a;

		public override ExprType CheckType(FunctionContex ctx)
		{
			return new ExprType(int.Parse(a.v));
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var t = CheckType(ctx);
			return new ExprValue(t);

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

	public class ExprId : ExprRoot
	{
		public ExprId(TokenAST a) : base(a) { this.a = a; }
		readonly TokenAST a;

		public override ExprType CheckType(FunctionContex ctx)
		{
			var v = ctx.GetVar(tk);
			return new ExprType(v.Type);
		}

		public override string GenLeft(FunctionContex ctx)
		{
			return ctx.GetVar(a).Reg;
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var rvar = ctx.GetVar(a);
			if (rdest != null) ctx.Context.ld(rdest, rvar);
			return rvar;
		}
	}

	public class ExprFun : ExprRoot
	{
		public ExprFun(TokenAST f, ExprList a) : base(f) { this.f = f; this.a = a; }
		readonly TokenAST f;
		readonly ExprList a;

		public override ExprType CheckType(FunctionContex ctx)
		{
			var fun = ctx.GetFun(f);
			return new ExprType(fun.ret);
		}

		public override ExprValue GenRight(FunctionContex ctx, string rdest)
		{
			var fun = ctx.GetFun(this.f);
			if (fun.args.Count != this.a.Count)
				Error("wrong number of args calling function '{0}'", fun.name);

			for (int i = 0; i < this.a.Count; ++i)
			{
				if (this.a[i].CheckType(ctx).Type != fun.args[i].ArgType)
					Error("type mismatch while calling function '{0}' param {1}", fun.name, i);
				var ra = this.a[i].GenRight(ctx, null);
				ctx.Context.ld("rp", ra);
			}
			if (rdest == null) rdest = ctx.NewTmp();
			ctx.Context.js(rdest, this.f.v);
			return new ExprValue(rdest, this.CheckType(ctx).Type);
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

		public DeclList Start(LexReader rd)
		{
			this.init(rd);
			return this.start(null);
		}
	}
}
