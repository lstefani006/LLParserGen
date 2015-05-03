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

	public abstract class ExprType : IAST
	{
		public virtual bool IsBool { get { return false; } }
		public virtual bool IsInt { get { return false; } }
		public virtual bool IsDbl { get { return false; } }
		public virtual bool IsVoid { get { return false; } }
		public virtual bool IsArray { get { return false; } }
		public virtual bool IsObject { get { return false; } }

		protected abstract bool TypeEqual(ExprType t);
		public static bool operator ==(ExprType a, ExprType b)
		{
			return a.TypeEqual(b);
		}
		public static bool operator !=(ExprType a, ExprType b)
		{
			return a.TypeEqual(b) == false;
		}

		public override bool Equals(object obj)
		{
			ExprType t = obj as ExprType;
			return this.TypeEqual(t);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public class SimpleExprType : ExprType
	{
		public SimpleExprType(TokenAST id) { this.id = id.v; }
		public SimpleExprType(string id) { this.id = id; }

		public override bool IsBool   { get { return this.id == "bool"; } }
		public override bool IsInt    { get { return this.id == "int"; } }
		public override bool IsDbl { get { return this.id == "double"; } }
		public override bool IsVoid { get { return this.id == "void"; } }
		public override bool IsObject { get { return !(IsBool || IsInt || IsVoid || IsDbl); } }
		readonly string id;

		protected override bool TypeEqual(ExprType t)
		{
			var st = t as SimpleExprType;
			if ((object)st != null)
				return this.id == st.id;
			return false;
		}

		public override string ToString()
		{
			return this.id;
		}
	}

	public class ArrayExprType : ExprType
	{
		public ArrayExprType(ExprType t) { this.t = t; }
		public readonly ExprType t;
		public override bool IsArray { get { return true; } }

		protected override bool TypeEqual(ExprType t)
		{
			var at = t as ArrayExprType;
			if ((object)at != null)
				return this.t == at.t;
			return false;
		}

		public override string ToString()
		{
			return t.ToString() + "[]";
		}
	}

	public class ExprValue
	{
		public ExprValue(string s, ExprType t) { this._reg = s; this.type = t; }
		public ExprValue(int c) { this._v = c; this.type = new SimpleExprType("int"); }
		public ExprValue(bool c) { this._v = c; this.type = new SimpleExprType("bool"); }
		public ExprValue(double d) { this._v = d; this.type = new SimpleExprType("double"); }

		string _reg;
		object _v;
		public readonly ExprType type;

		public int Int { get { return (int)_v; } }
		public double Dbl { get { return (double)_v; } }
		public bool Bool { get { return (bool)_v; } }
		public string reg { get { return _reg; } }

		public void SetReg(string r) { _reg = r; }

		public bool IsReg { get { return _reg != null; } }
		public bool IsConst { get { return _reg == null; } }

		public override string ToString()
		{
			if (IsReg) return this._reg;
			return this._v.ToString();
		}


		private static int cmp(ExprValue a, ExprValue b)
		{
			if ((object)a == (object)b) return 0;
			if ((object)a != null && (object)b == null) return +1;
			if ((object)a == null && (object)b != null) return -1;

			Debug.Assert(a.IsConst && b.IsConst);
			if (a.type.IsInt) return a.Int.CompareTo(b.Int);
			if (a.type.IsDbl) return a.Dbl.CompareTo(b.Dbl);
			if (a.type.IsBool) return a.Bool.CompareTo(b.Bool);
			Debug.Assert(false);
			return 0;
		}

		public static bool operator !=(ExprValue a, ExprValue b) { return cmp(a, b) != 0; }
		public static bool operator ==(ExprValue a, ExprValue b) { return cmp(a, b) == 0; }
		public static bool operator < (ExprValue a, ExprValue b) { return cmp(a, b) < 0; }
		public static bool operator > (ExprValue a, ExprValue b) { return cmp(a, b) > 0; }
		public static bool operator <=(ExprValue a, ExprValue b) { return cmp(a, b) <= 0; }
		public static bool operator >=(ExprValue a, ExprValue b) { return cmp(a, b) >= 0; }


		public static ExprValue operator + (ExprValue a, ExprValue b) 
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a.type.IsInt) return new ExprValue(a.Int + b.Int);
			if (a.type.IsDbl) return new ExprValue(a.Dbl + b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprValue operator -(ExprValue a, ExprValue b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a.type.IsInt) return new ExprValue(a.Int - b.Int);
			if (a.type.IsDbl) return new ExprValue(a.Dbl - b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprValue operator *(ExprValue a, ExprValue b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a.type.IsInt) return new ExprValue(a.Int * b.Int);
			if (a.type.IsDbl) return new ExprValue(a.Dbl * b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprValue operator /(ExprValue a, ExprValue b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a.type.IsInt) return new ExprValue(a.Int / b.Int);
			if (a.type.IsDbl) return new ExprValue(a.Dbl / b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprValue operator %(ExprValue a, ExprValue b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a.type.IsInt) return new ExprValue(a.Int % b.Int);
			if (a.type.IsDbl) return new ExprValue(a.Dbl % b.Dbl);
			throw new InvalidOperatorException();
		}
		public static ExprValue operator | (ExprValue a, ExprValue b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a.type.IsInt) return new ExprValue(a.Int | b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprValue operator &(ExprValue a, ExprValue b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a.type.IsInt) return new ExprValue(a.Int & b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprValue operator ^(ExprValue a, ExprValue b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a.type.IsInt) return new ExprValue(a.Int ^ b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprValue lsh(ExprValue a, ExprValue b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a.type.IsInt) return new ExprValue(a.Int << b.Int);
			throw new InvalidOperatorException();
		}
		public static ExprValue rsh(ExprValue a, ExprValue b)
		{
			Debug.Assert(a.IsConst && b.IsConst);
			if (a.type.IsInt) return new ExprValue(a.Int >> b.Int);
			throw new InvalidOperatorException();
		}
	}

	public class InvalidOperatorException : Exception
	{
	}

	public class FunList : IAST, IEnumerable<Fun>
	{
		public IEnumerator<Fun> GetEnumerator() { return _lst.GetEnumerator(); }
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return _lst.GetEnumerator(); }

		public FunList() { _lst = new List<Fun>(); }
		public FunList Add(Fun a) { _lst.Add(a); return this; }
		readonly List<Fun> _lst;
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
			throw new SyntaxError(this.tk, msg, args);
		}
	}

	public class Fun : NodeRoot
	{
		public readonly TokenAST name;
		public readonly FunArgList args;
		public readonly ExprType ret;
		public readonly StmtRoot body;
		public readonly TokenAST lastCurly;

		public Fun(TokenAST name, FunArgList args, ExprType ret, StmtRoot body, TokenAST lastCurly) : base(name)
		{
			this.name = name;
			this.args = args;
			this.ret = ret;
			this.body = body;
			this.lastCurly = lastCurly;
		}
	}
	public class FunArg
	{
		public FunArg(TokenAST argName, ExprType argType)
		{
			this.ArgName = argName;
			this.ArgType = argType;
		}
		public readonly TokenAST ArgName;
		public readonly ExprType ArgType;
	}

	public class FunArgList : IAST
	{
		public readonly List<FunArg> args = new List<FunArg>();

		public FunArgList() { }
		public FunArgList(TokenAST a, ExprType ty) { this.Add(a, ty); }
		public FunArgList Add(TokenAST arg, ExprType ty) { args.Add(new FunArg(arg, ty)); return this; }

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
		public readonly ExprType type;
		public StmtVar(TokenAST tk, TokenAST a, ExprType ty) : base(tk) { this.a = a; this.type = ty; }

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

			if (e.ExprType(ctx).IsBool == false)
				this.Error("while require bool expression");

			if (e.IsConstExpr())
			{
				this.Warning("const expression");

				if (sb == null)
				{
					if (e.EvalRight(ctx, null).Bool)
					{
						nua = this.sa.GenCode(ctx);
						nub = true;
					}
				}
				else
				{
					if (e.EvalRight(ctx, null).Bool)
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
					e.EvalBool(ctx, null, lbl_false);
					this.sa.GenCode(ctx);
					ctx.emit(lbl_false);
					nua = false;
					nub = false;

				}
				else
				{
					var lbl_out = ctx.NewLbl();
					var lbl_false = ctx.NewLbl();
					e.EvalBool(ctx, null, lbl_false);
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

			if (e.ExprType(ctx).IsBool == false)
				this.Error("while statments require bool expression");

			bool nu = false;
			if (this.e.IsConstExpr())
			{
				this.Warning("Const expression");

				if (this.e.EvalRight(ctx, null).Bool)
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
				this.e.EvalBool(ctx, lbl_true, null);
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
				if (e.ExprType(ctx) != ctx.fun.ret) Error("wrong return type");
			}

			if (e != null)
			{
				var r = e.EvalRight(ctx, null);
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
		public override bool GenCode(FunctionContex ctx) { e.EvalRight(ctx, null); return false; }
	}

	///////////////////////////////////////////////////////////

	public abstract class ExprRoot : NodeRoot
	{
		protected ExprRoot(TokenAST tk) : base(tk) { }

		public abstract ExprValue EvalRight(FunctionContex ctx, string rdest);
		public abstract bool IsConstExpr();
		public abstract ExprType ExprType(FunctionContex ctx);

		public virtual void EvalBool(FunctionContex ctx, string lbl_true, string lbl_false)
		{
			if (this.IsConstExpr())
			{
				var s = EvalRight(ctx, null);
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
				var s = EvalRight(ctx, null);
				if (lbl_true != null)
					ctx.Context.bne(s, new ExprValue(0), lbl_true);
				else
					ctx.Context.beq(s, new ExprValue(0), lbl_false);
			}
		}

		public virtual string EvalLeft(FunctionContex ctx)
		{
			Error("no left side");
			return null;
		}
	}

	public class ExprAss : ExprRoot
	{
		protected ExprRoot a;
		protected ExprRoot b;

		public ExprAss(ExprRoot a, TokenAST tk, ExprRoot b) : base(tk) { this.a = a; this.b = b; }

		public override string EvalLeft(FunctionContex ctx) { return a.EvalLeft(ctx); }
		public override bool IsConstExpr() { return false; } // comunque NON è una espressione constante


		public override ExprType ExprType(FunctionContex ctx)
		{
			if (a.ExprType(ctx) == b.ExprType(ctx))
				return a.ExprType(ctx);
			Error("cannot assign expression of type '{0}' to '{1}'", a, b);
			return null;
		}

		public override ExprValue EvalRight(FunctionContex ctx, string rdest)
		{
			// ignoro volutamente il const.
			var ra = a.EvalLeft(ctx);
			var rb = b.EvalRight(ctx, ra); // rdest DEVE essere risolto.
			return rb;  // qui posso ritornare la costante (se è const oppure la var...)
		}
	}

	public abstract class ExprBin : ExprRoot
	{
		protected ExprBin(ExprRoot a, TokenAST tk, ExprRoot b) : base(tk) { this.a = a; this.b = b; }
		protected ExprRoot a;
		protected ExprRoot b;
		public override bool IsConstExpr() { return a.IsConstExpr() && b.IsConstExpr(); }
	}

	public abstract class ExprUni : ExprRoot
	{
		protected ExprUni(TokenAST tk, ExprRoot a) : base(tk) { this.a = a; }
		protected ExprRoot a;
		public override bool IsConstExpr() { return a.IsConstExpr(); }
	}

	public class ExprBinLogical : ExprBin
	{
		readonly string op;

		public ExprBinLogical(ExprRoot a, TokenAST tk, ExprRoot b)
			: base(a, tk, b)
		{
			this.op = tk.v;
			switch (this.op)
			{
			case "&&": break;
			case "||": break;
			default: Debug.Assert(false); break;
			}
		}

		public override ExprType ExprType(FunctionContex ctx)
		{
			if (a.ExprType(ctx).IsBool == false || b.ExprType(ctx).IsBool == false) Error("wrong type");
			return a.ExprType(ctx);
		}

		public override ExprValue EvalRight(FunctionContex ctx, string rdest)
		{
			ExprType(ctx);

			if (a.IsConstExpr() && b.IsConstExpr())
			{
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);
				ExprValue rr;
				switch (this.op)
				{
				case "&&": rr = new ExprValue(aa.Bool && bb.Bool); break;
				case "||": rr = new ExprValue(aa.Bool || bb.Bool); break;
				default: Debug.Assert(false); return null;
				}
				if (rdest != null) ctx.Context.ld(rdest, rr);
				return rr;
			}
			else
			{
				if (op == "||")
				{
					if (rdest == null) rdest = ctx.Context.NewTmp();
					string lbl_false = ctx.NewLbl();
					string lbl_true = ctx.NewLbl();
					ctx.Context.ld(rdest, 0);
					a.EvalBool(ctx, lbl_true, null);
					b.EvalBool(ctx, null, lbl_false);
					ctx.emit(lbl_true);
					ctx.Context.ld(rdest, 1);
					ctx.emit(lbl_false);
					return new ExprValue(rdest, new SimpleExprType("bool"));
				}
				else if (op == "&&")
				{
					if (rdest == null) rdest = ctx.Context.NewTmp();
					string lbl_false = ctx.NewLbl();
					string lbl_true = ctx.NewLbl();
					ctx.Context.ld(rdest, 1);
					a.EvalBool(ctx, null, lbl_false);
					b.EvalBool(ctx, lbl_true, null);
					ctx.emit(lbl_false);
					ctx.Context.ld(rdest, 0);
					ctx.emit(lbl_true);
					return new ExprValue(rdest, new SimpleExprType("bool"));
				}
				else
				{
					Debug.Assert(false);
					return null;
				}
			}
		}

		public override void EvalBool(FunctionContex ctx, string lbl_true, string lbl_false)
		{
			ExprType(ctx);

			if (a.IsConstExpr() && b.IsConstExpr())
			{
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);

				if (lbl_true != null)
				{
					switch (this.op)
					{
					case "&&": if (aa.Bool && bb.Bool) ctx.Context.jmp(lbl_true); break;
					case "||": if (aa.Bool || bb.Bool) ctx.Context.jmp(lbl_true); break;
					default: Debug.Assert(false); break;
					}
				}
				else
				{
					switch (this.op)
					{
					case "&&": if (!(aa.Bool && bb.Bool)) ctx.Context.jmp(lbl_false); break;
					case "||": if (!(aa.Bool || bb.Bool)) ctx.Context.jmp(lbl_false); break;
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
						a.EvalBool(ctx, null, lbl_false);
						b.EvalBool(ctx, lbl_true, null);
						ctx.emit(lbl_false);
					}
					else
					{
						a.EvalBool(ctx, null, lbl_false);
						b.EvalBool(ctx, null, lbl_false);
					}
				}
				else if (op == "||")
				{
					if (lbl_true != null)
					{
						a.EvalBool(ctx, lbl_true, null);
						b.EvalBool(ctx, lbl_true, null);
					}
					else
					{
						lbl_true = ctx.NewLbl();
						a.EvalBool(ctx, lbl_true, null);
						b.EvalBool(ctx, null, lbl_false);
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
		string op { get { return tk.v; } }

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

		public override ExprType ExprType(FunctionContex ctx)
		{
			switch (this.op)
			{
			case "==":
			case "!=":
				if (a.ExprType(ctx).IsVoid || b.ExprType(ctx).IsVoid) Error("wrong type");
				if (a.ExprType(ctx) != b.ExprType(ctx)) Error("wrong type");
				break;
			case ">":
			case ">=":
			case "<":
			case "<=":
				if (a.ExprType(ctx) != b.ExprType(ctx)) Error("wrong type");
				if (!(a.ExprType(ctx).IsInt || a.ExprType(ctx).IsDbl)) Error("wrong type");
				break;
			default:
				Debug.Assert(false);
				break;
			}
			return new SimpleExprType("bool");
		}

		public override ExprValue EvalRight(FunctionContex ctx, string rdest)
		{
			ExprType(ctx);

			if (a.IsConstExpr() && b.IsConstExpr())
			{
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);
				ExprValue rr;
				switch (this.op)
				{
				case "==": rr = new ExprValue(aa == bb ? true : false); break;
				case "!=": rr = new ExprValue(aa != bb ? true : false); break;
				case ">": rr = new ExprValue(aa > bb ? true : false); break;
				case ">=": rr = new ExprValue(aa >= bb ? true : false); break;
				case "<": rr = new ExprValue(aa < bb ? true : false); break;
				case "<=": rr = new ExprValue(aa <= bb ? true : false); break;
				default: Debug.Assert(false); return null;
				}
				if (rdest != null) ctx.Context.ld(rdest, rr.Int);
				return rr;
			}
			else
			{
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);
				if (rdest == null) rdest = ctx.Context.NewTmp();
				string lbl_false = ctx.NewLbl();
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
				return new ExprValue(rdest, new SimpleExprType("bool"));
			}
		}

		public override void EvalBool(FunctionContex ctx, string lbl_true, string lbl_false)
		{
			ExprType(ctx);

			if (a.IsConstExpr() && b.IsConstExpr())
			{
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);

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
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);

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

		public override ExprType ExprType(FunctionContex ctx)
		{
			if (a.ExprType(ctx) != b.ExprType(ctx)) Error("operatore {0} requires same types", tk.v);
			if (!(a.ExprType(ctx).IsInt || a.ExprType(ctx).IsDbl)) Error("{0} requires int/double type", tk.v);
			if (!(b.ExprType(ctx).IsInt || b.ExprType(ctx).IsDbl)) Error("{0} requires int/double type", tk.v);
			return a.ExprType(ctx);
		}

		public override ExprValue EvalRight(FunctionContex ctx, string rdest)
		{
			this.ExprType(ctx);

			if (a.IsConstExpr() && b.IsConstExpr())
			{
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);
				ExprValue rr;
				switch (tk.v)
				{
				case "+": rr = aa + bb; break;
				case "-": rr = aa - bb; break;
				case "*": rr = aa * bb; break;
				case "/": rr = aa / bb; break;
				case "%": rr = aa % bb; break;
				case "|": rr = aa | bb; break;
				case "&": rr = aa & bb; break;
				case "^": rr = aa ^ bb; break;
				case "<<": rr = ExprValue.lsh(aa, bb); break;
				case ">>": rr = ExprValue.rsh(aa, bb); break;
				default: Debug.Assert(false); rr = null;  break;
				}
				if (rdest != null) ctx.Context.ld(rdest, rr);
				return rr;
			}
			else
			{
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);
				if (rdest == null) rdest = ctx.Context.NewTmp();

				switch (tk.v)
				{
				case "+": ctx.Context.add(rdest, aa, bb); break;
				case "-": ctx.Context.sub(rdest, aa, bb); break;
				case "*": ctx.Context.mul(rdest, aa, bb); break;
				case "/": ctx.Context.div(rdest, aa, bb); break;
				case "%": ctx.Context.rem(rdest, aa, bb); break;
				case "|": ctx.Context.or(rdest, aa, bb); break;
				case "&": ctx.Context.and(rdest, aa, bb); break;
				case "^": ctx.Context.xor(rdest, aa, bb); break;
				case "<<": ctx.Context.shl(rdest, aa, bb); break;
				case ">>": ctx.Context.shr(rdest, aa, bb); break;
				default: Debug.Assert(false); break;
				}

				return new ExprValue(rdest, aa.type);
			}
		}
	}


	public class ExprPlus : ExprUni
	{
		public ExprPlus(TokenAST tk, ExprRoot a) : base(tk, a) { }
		public override ExprValue EvalRight(FunctionContex ctx, string rdest) { return a.EvalRight(ctx, rdest); }
		public override ExprType ExprType(FunctionContex ctx)
		{
			if (!(a.ExprType(ctx).IsInt || a.ExprType(ctx).IsDbl)) Error("{0} requires int type", tk.v);
			return a.ExprType(ctx);
		}
	}

	public class ExprCast : ExprRoot
	{
		public ExprCast(TokenAST tk, ExprType rtype, ExprRoot e) : base(tk) { this.rtype = rtype; this.e = e;  }
		readonly ExprType rtype;
		readonly ExprRoot e;

		public override ExprType ExprType(FunctionContex ctx) 
		{
			return rtype;
		}

		public override ExprValue EvalRight(FunctionContex ctx, string rdest)
		{
			var etype = e.ExprType(ctx);
			if (etype.IsBool && rtype.IsInt)
			{
				// nessuna conversione
				var r = e.EvalRight(ctx, rdest);
				return r;
			}
			else if (etype.IsArray && rtype.IsObject)
			{

			}
			else
				Error("cannot convert from '{0}' to '{1}' type", etype, rtype);
			return null;
		}
		public override bool IsConstExpr() { return false; }
	}


	public class ExprNeg : ExprUni
	{
		public ExprNeg(TokenAST tk, ExprRoot a) : base(tk, a) { }

		public override ExprType ExprType(FunctionContex ctx)
		{
			if (!(a.ExprType(ctx).IsInt || a.ExprType(ctx).IsDbl)) Error("'{0}' requires numeric type", tk.v);
			return a.ExprType(ctx);
		}
		public override ExprValue EvalRight(FunctionContex ctx, string rdest)
		{
			if (a.IsConstExpr())
			{
				var n = a.EvalRight(ctx, null);

				if (rdest != null)
				{
					if (n.type.IsInt) ctx.Context.ld(rdest, new ExprValue(0) - n);
					else if (n.type.IsDbl) ctx.Context.ld(rdest, new ExprValue(0.0) - n);
					else Error("operator '-' called with non numeric data");
				}
				return n;
			}
			else
			{
				var ra = a.EvalRight(ctx, null);
				if (rdest == null) rdest = ctx.Context.NewTmp();
				if (ra.type.IsInt) ctx.Context.sub(rdest, new ExprValue(0), ra);
				else if (ra.type.IsDbl) ctx.Context.sub(rdest, new ExprValue(0.0), ra);
				else Error("operator '-' called with non numeric data");
				return new ExprValue(rdest, a.ExprType(ctx));
			}
		}
	}

	public class ExprNum : ExprRoot
	{
		public ExprNum(TokenAST a) : base(a) { this.a = a; }
		readonly TokenAST a;

		public override ExprType ExprType(FunctionContex ctx)
		{
			return new SimpleExprType("int");
		}

		public override ExprValue EvalRight(FunctionContex ctx, string rdest)
		{
			var n = new ExprValue(int.Parse(a.v));
			if (rdest != null) ctx.Context.ld(rdest, n.Int);
			return n;
		}
		public override bool IsConstExpr() { return true; }
	}
	public class ExprBool : ExprRoot
	{
		public ExprBool(TokenAST tk, bool v) : base(tk) { this.v = v; }
		readonly bool v;

		public override ExprType ExprType(FunctionContex ctx) { return new SimpleExprType("bool"); }

		public override ExprValue EvalRight(FunctionContex ctx, string rdest)
		{
			var n = new ExprValue(v);
			if (rdest != null) ctx.Context.ld(rdest, n.Bool ? 1 : 0);
			return n;
		}
		public override bool IsConstExpr() { return true; }
	}

	public class ExprId : ExprRoot
	{
		public ExprId(TokenAST a) : base(a) { this.a = a; }
		readonly TokenAST a;

		public override ExprType ExprType(FunctionContex ctx)
		{
			var v = ctx.GetVar(tk);
			return v.Type;
		}

		public override string EvalLeft(FunctionContex ctx)
		{
			return ctx.GetVar(a).Reg;
		}

		public override ExprValue EvalRight(FunctionContex ctx, string rdest)
		{
			var rvar = ctx.GetVar(a).Reg;
			if (rdest == null) rdest = rvar;
			if (rdest != rvar) ctx.Context.add(rdest, new ExprValue(rvar, this.ExprType(ctx)), new ExprValue(0));
			return new ExprValue(rvar, this.ExprType(ctx));
		}
		public override bool IsConstExpr() { return false; }
	}

	public class ExprFun : ExprRoot
	{
		public ExprFun(TokenAST f, ExprList a) : base(f) { this.f = f; this.a = a; }
		readonly TokenAST f;
		readonly ExprList a;

		public override ExprType ExprType(FunctionContex ctx)
		{
			var fun = ctx.GetFun(f);
			return fun.ret;
		}

		public override ExprValue EvalRight(FunctionContex ctx, string rdest)
		{
			var fun = ctx.GetFun(this.f);
			if (fun.args.Count != this.a.Count)
				Error("wrong number of args calling function '{0}'", fun.name);

			for (int i = 0; i < this.a.Count; ++i)
			{
				if (this.a[i].ExprType(ctx) != fun.args[i].ArgType)
					Error("type mismatch while calling function '{0}' param {1}", fun.name, i);
				this.a[i].EvalRight(ctx, "rp");
			}
			if (rdest == null) rdest = ctx.Context.NewTmp();
			ctx.Context.js(rdest, this.f.v);
			return new ExprValue(rdest, this.ExprType(ctx));
		}
		public override bool IsConstExpr() { return false; }
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

		public FunList Start(LexReader rd)
		{
			this.init(rd);
			return this.start(null);
		}
	}
}
