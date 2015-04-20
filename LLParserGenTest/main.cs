using System;
using System.Diagnostics;
using LLParserLexerLib;
using System.Collections.Generic;

namespace LLParserGenTest
{
	class _ {
		public static void Main(string[] args) {
			using (var rd = (args.Length == 1 ? new LexReader(args[0]) : new LexReader(Console.In, "stdin"))) {

				try {
					var p = new MParser();
					var s = p.Start(rd);

					using (Context ctx = new Context()) {
						ctx.GenerateCode(s);
					}
				}
				catch (SyntaxError ex) {
					Console.WriteLine(ex.Message);
				}
			}
		}
	}

	public abstract class Type : IAST {
		public virtual bool IsBool   { get { return false; } }
		public virtual bool IsInt    { get { return false; } }
		public virtual bool IsVoid   { get { return false; } }
		public virtual bool IsArray  { get { return false; } }
		public virtual bool IsObject { get { return false; } }
	}
	public class SimpleType : Type {
		public SimpleType(TokenAST id) { this.id = id.v; }
		public SimpleType(string id)   { this.id = id;   }

		public override bool IsBool { get { return this.id == "bool"; } }
		public override bool IsInt  { get { return this.id == "int";  } }
		public override bool IsVoid { get { return this.id == "void"; } }
		public override bool IsObject { get { return !(IsBool || IsInt || IsVoid); } }
		readonly string id;
	}
	public class ArrayType : Type {
		public ArrayType(Type t) { this.t = t; }
		public readonly Type t;
		public override bool IsArray { get { return true; } }
	}

	public class ExprValue {
		public ExprValue(string s) { this._reg = s; }
		public ExprValue(int c) { this._c = c; this.type = new SimpleType("int"); }
		public ExprValue(bool c) { this._c = c ? 1 : 0; this.type = new SimpleType("bool"); }

		string _reg;
		int _c;
		public Type type;

		public int c { get { return _c; } }
		public string reg { get { return _reg; } }

		public void SetReg(string r) { _reg = r; }

		public bool IsReg { get { return _reg != null; } }
		public bool IsConst { get { return _reg == null; } }

		public override string ToString() {
			if (IsReg) return this._reg;
			return this._c.ToString();
		}
	}

	public class FunList : IAST, IEnumerable<Fun> {

		public IEnumerator<Fun> GetEnumerator() { return _lst.GetEnumerator(); }

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return _lst.GetEnumerator(); }

		public FunList() { _lst = new List<Fun>(); }
		public FunList Add(Fun a) { _lst.Add(a); return this; }
		readonly List<Fun> _lst;
	}

	public class Fun : IAST
	{
		public readonly string name;
		public readonly FunArgList args;
		public readonly Type ret;
		public readonly StmtRoot body;
		public Fun(TokenAST name, FunArgList args, Type ret, StmtRoot body) {
			this.name = name.v;
			this.args = args;
			this.ret = ret;
			this.body = body;
		}
	}

	public class FunArgList : IAST {
		public readonly List<string> args = new List<string>();
		public readonly List<Type> type = new List<Type>();

		public FunArgList() {}
		public FunArgList(TokenAST a, Type ty) { this.Add(a, ty); }
		public FunArgList Add(TokenAST arg, Type ty) { args.Add(arg.v); type.Add(ty); return this; }
	}

	public abstract class StmtRoot : IAST {
		// ritorna true se next unreachable
		public abstract bool GenCode(FunctionContex ctx);
		public readonly TokenAST tk;

		protected StmtRoot(TokenAST tk) { this.tk = tk; }
	
		public void Warning(string msg, params object [] args) {
			Console.WriteLine("{0} Warning: {1}", this.tk.TrackMsg, U.F(msg, args)); 
		}
		public void Error(string msg, params object [] args) {
			throw new SyntaxError(this.tk, msg, args);
		}
	}

	public class StmtList : StmtRoot {
		readonly List<StmtRoot> a;

		public StmtList() : base(null) { this.a = new List<StmtRoot>(); }
		public StmtList(StmtRoot s) : base(s.tk) { this.a = new List<StmtRoot>(); this.Add(s); }

		public StmtList Add(StmtRoot s) { this.a.Add(s); return this; }

		public override bool GenCode(FunctionContex ctx) {
			bool nu = false;
			foreach (var s in a) {
				if (nu == true) {
					s.Warning("Unreachable code");
					break;
				}
				nu = s.GenCode(ctx);
			}
			return nu;
		}
	}

	public class StmtVar : StmtRoot {
		public readonly TokenAST a;
		public readonly Type type;
		public StmtVar(TokenAST tk, TokenAST a, Type ty) : base(tk) { this.a = a; this.type = ty; }

		public override bool GenCode(FunctionContex ctx) {
			ctx.AddDefVar(this.a.v);
			ctx.Push(FunctionContex.StmtTk.Var, null, null, this.a.v);
			return false;
		}
	}
	public class StmtBlock : StmtRoot {
		readonly StmtRoot sa;

		public StmtBlock(TokenAST tk, StmtRoot a) : base(tk) { this.sa = a; }
		public StmtBlock(StmtRoot a) : base(a.tk) { this.sa = a; }

		public override bool GenCode(FunctionContex ctx) {
			ctx.Push(FunctionContex.StmtTk.Block, null, null, null);
			bool nu = sa.GenCode(ctx);
			ctx.Pop(FunctionContex.StmtTk.Block);
			return nu;
		}
	}

	public class StmtIf : StmtRoot {
		readonly ExprRoot e;
		readonly StmtRoot sa;
		readonly StmtRoot sb;

		public StmtIf(TokenAST tk, ExprRoot e, StmtRoot sa, StmtRoot sb = null) : base(tk) {
			this.e = e;
			this.sa = sa;
			this.sb = sb;
		}

		public override bool GenCode(FunctionContex ctx) {
			bool nua = false;
			bool nub = false;

			if (e.IsConstExpr()) {
				this.Warning("Const expression");

				if (sb == null) {
					if (e.EvalRight(ctx, null).c != 0) {
						nua = this.sa.GenCode(ctx);
						nub = true;
					}
				} else {
					if (e.EvalRight(ctx, null).c != 0) {
						nua = this.sa.GenCode(ctx);
						nub = true;
					} else {
						nua = true;
						nub = this.sb.GenCode(ctx);
					}
				}
			} else {
				if (this.sb == null) {
					var lbl_false = ctx.NewLbl();
					e.EvalBool(ctx, null, lbl_false);
					this.sa.GenCode(ctx);
					ctx.emit(lbl_false);
					nua = false;
					nub = false;

				} else {
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

	public class StmtWhile : StmtRoot {
		readonly ExprRoot e;
		readonly StmtRoot s;
		public StmtWhile(TokenAST tk, ExprRoot e, StmtRoot s) : base(tk) { this.e = e; this.s = s; }

		public override bool GenCode(FunctionContex ctx) {
			var lbl_break = ctx.NewLbl();
			var lbl_continue = ctx.NewLbl();
			ctx.Push(FunctionContex.StmtTk.While, lbl_break, lbl_continue, null);

			bool nu = false;
			if (this.e.IsConstExpr()) {
				this.Warning("Const expression");

				if (this.e.EvalRight(ctx, null).c != 0) {
					var lbl_true = ctx.Context.NewLbl();
					ctx.emit(lbl_continue);
					ctx.emit(lbl_true);
					nu = s.GenCode(ctx);
					if (nu == false) ctx.Context.jmp(lbl_true);
					ctx.emit(lbl_break);
					nu = false;
				}
			} else {
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
	public class StmtBreak : StmtRoot {
		public StmtBreak(TokenAST tk) : base(tk) {}

		public override bool GenCode(FunctionContex ctx) {
			if (ctx.Break() == false)
				Error("Illegal break");
			return true;
		}
	}
	public class StmtContinue : StmtRoot {
		public StmtContinue(TokenAST tk) : base(tk) {}

		public override bool GenCode(FunctionContex ctx) {
			if (ctx.Continue() == false)
				Error("Illegal continue");
			return true;
		}
	}


	public class StmtReturn : StmtRoot {
		readonly ExprRoot e;
		public StmtReturn(TokenAST tk) : base(tk) { e = null; }
		public StmtReturn(TokenAST tk, ExprRoot e) : base(tk) { this.e = e; }

		public override bool GenCode(FunctionContex ctx) {
			if (e != null) {
				var r = e.EvalRight(ctx, null);
				ctx.Return(r);
				ctx.Context.ret(r);
			} else {
				ctx.Return(null);
				ctx.Context.ret(new ExprValue(0));
			}
			return true;
		}
	}

	public class StmtExpr : StmtRoot {
		readonly ExprRoot e;
		public StmtExpr(TokenAST tk, ExprRoot e) : base(tk) { this.e = e; }
		public override bool GenCode(FunctionContex ctx) { e.EvalRight(ctx, null); return false; }
	}

	///////////////////////////////////////////////////////////

	public abstract class ExprRoot : IAST {
		public abstract ExprValue EvalRight(FunctionContex ctx, string rdest);
		public abstract bool IsConstExpr();

		public virtual void EvalBool(FunctionContex ctx, string lbl_true, string lbl_false) {
			if (this.IsConstExpr()) {
				var s = EvalRight(ctx, null);
				if (lbl_true != null) {
					if (s.c != 0) ctx.Context.jmp(lbl_true);
				}
				else {
					if (s.c == 0) ctx.Context.jmp(lbl_false);
				}
			} else {
				var s = EvalRight(ctx, null);
				if (lbl_true != null)
					ctx.Context.bne(s, new ExprValue(0), lbl_true);
				else
					ctx.Context.beq(s, new ExprValue(0), lbl_false);
			}
		}

		public virtual string EvalLeft(FunctionContex ctx) {
			throw new Exception("errore - no left side");
		}
	}

	public class ExprAss : ExprRoot {
		protected ExprRoot a;
		protected ExprRoot b;

		public ExprAss(ExprRoot a, ExprRoot b) { this.a = a; this.b = b; }

		public override string EvalLeft(FunctionContex ctx) { return a.EvalLeft(ctx); }
		public override bool IsConstExpr() { return false; } // comunque NON è una espressione constante

		public override ExprValue EvalRight(FunctionContex ctx, string rdest) {
			// ignoro volutamente il const.
			var ra = a.EvalLeft(ctx);
			var rb = b.EvalRight(ctx, ra); // rdest DEVE essere risolto.
			return rb;  // qui posso ritornare la costante (se è const oppure la var...)
		}
	}

	public abstract class ExprBin : ExprRoot {
		protected ExprBin(ExprRoot a, ExprRoot b) { this.a = a; this.b = b; }
		protected ExprRoot a;
		protected ExprRoot b;
		public override bool IsConstExpr() { return a.IsConstExpr() && b.IsConstExpr(); }
	}

	public abstract class ExprUni : ExprRoot {
		protected ExprUni(ExprRoot a) { this.a = a; }
		protected ExprRoot a;
		public override bool IsConstExpr() { return a.IsConstExpr(); }
	}

	public class ExprBinLogical : ExprBin {
		readonly string op;

		public ExprBinLogical(string op, ExprRoot a, ExprRoot b) : base(a, b) { 
			this.op = op;
			switch (this.op) {
			case "&&": break;
			case "||": break;
			default: Debug.Assert(false); break;
			}
		}

		public override ExprValue EvalRight(FunctionContex ctx, string rdest) {

			if (a.IsConstExpr() && b.IsConstExpr()) {
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);
				ExprValue rr;
				switch (this.op) {
				case "&&": rr = new ExprValue(aa.c != 0 && bb.c != 0 ? 1 : 0); break;
				case "||": rr = new ExprValue(aa.c != 0 || bb.c != 0 ? 1 : 0); break;
				default: Debug.Assert(false); return null;
				}
				if (rdest != null) ctx.Context.ld(rdest, rr.c);
				return rr;
			} else {
				if (op == "||") {
					if (rdest == null) rdest = ctx.Context.NewTmp();
					string lbl_false = ctx.NewLbl();
					string lbl_true = ctx.NewLbl();
					ctx.Context.ld(rdest, 0);
					a.EvalBool(ctx, lbl_true, null);
					b.EvalBool(ctx, null, lbl_false);
					ctx.emit(lbl_true);
					ctx.Context.ld(rdest, 1);
					ctx.emit(lbl_false);
					return new ExprValue(rdest); 
				} else if (op == "&&") {
					if (rdest == null) rdest = ctx.Context.NewTmp();
					string lbl_false = ctx.NewLbl();
					string lbl_true = ctx.NewLbl();
					ctx.Context.ld(rdest, 1);
					a.EvalBool(ctx, null, lbl_false);
					b.EvalBool(ctx, lbl_true, null);
					ctx.emit(lbl_false);
					ctx.Context.ld(rdest, 0);
					ctx.emit(lbl_true);
					return new ExprValue(rdest); 
				} else {
					Debug.Assert(false);
					return null;
				}
			}
		}

		public override void EvalBool(FunctionContex ctx, string lbl_true, string lbl_false) {
			if (a.IsConstExpr() && b.IsConstExpr()) {

				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);

				if (lbl_true != null) {
					switch (this.op) {
					case "&&": if (aa.c != 0 && bb.c != 0) ctx.Context.jmp(lbl_true); break;
					case "||": if (aa.c != 0 || bb.c != 0) ctx.Context.jmp(lbl_true); break;
					default: Debug.Assert(false); break;
					}
				} else {
					switch (this.op) {
					case "&&": if (!(aa.c != 0 && bb.c != 0)) ctx.Context.jmp(lbl_false); break;
					case "||": if (!(aa.c != 0 || bb.c != 0)) ctx.Context.jmp(lbl_false); break;
					default: Debug.Assert(false); break;
					}
				}
			} else {

				if (op == "&&") {
					if (lbl_true != null) {
						lbl_false = ctx.NewLbl();
						a.EvalBool(ctx, null, lbl_false);
						b.EvalBool(ctx, lbl_true, null);
						ctx.emit(lbl_false);
					} else {
						a.EvalBool(ctx, null, lbl_false);
						b.EvalBool(ctx, null, lbl_false);
					}

				} else if (op == "||") {
					if (lbl_true != null) {
						a.EvalBool(ctx, lbl_true, null);
						b.EvalBool(ctx, lbl_true, null);
					} else {
						lbl_true = ctx.NewLbl();
						a.EvalBool(ctx, lbl_true, null);
						b.EvalBool(ctx, null, lbl_false);
						ctx.emit(lbl_true);
					}
				} else {
					Debug.Assert(false);
				}
			}
		}
	}
	public class ExprBinCompare : ExprBin {
		string op;

		public ExprBinCompare(string op, ExprRoot a, ExprRoot b) : base(a, b) {
			this.op = op; 
			switch (this.op) {
			case "==": break;
			case "!=": break;
			case ">":  break;
			case ">=": break;
			case "<":  break;
			case "<=": break;
			default: Debug.Assert(false); break;
			}
		}

		public override ExprValue EvalRight(FunctionContex ctx, string rdest) {

			if (a.IsConstExpr() && b.IsConstExpr()) {
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);
				ExprValue rr;
				switch (this.op) {
				case "==": rr = new ExprValue(aa.c == bb.c ? 1 : 0); break;
				case "!=": rr = new ExprValue(aa.c != bb.c ? 1 : 0); break;
				case ">":  rr = new ExprValue(aa.c > bb.c ? 1 : 0); break;
				case ">=": rr = new ExprValue(aa.c >= bb.c ? 1 : 0); break;
				case "<":  rr = new ExprValue(aa.c < bb.c ? 1 : 0); break;
				case "<=": rr = new ExprValue(aa.c <= bb.c ? 1 : 0); break;
				default: Debug.Assert(false); return null;
				}
				if (rdest != null) ctx.Context.ld(rdest, rr.c);
				return rr;
			} else {
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);
				if (rdest == null) rdest = ctx.Context.NewTmp();
				string lbl_false = ctx.NewLbl();
				ctx.Context.ld(rdest, 0);
				switch (this.op) {
				case "==": ctx.Context.bne(aa, bb, lbl_false); break;
				case "!=": ctx.Context.beq(aa, bb, lbl_false); break;
				case ">":  ctx.Context.ble(aa, bb, lbl_false); break;
				case ">=": ctx.Context.blt(aa, bb, lbl_false); break;
				case "<":  ctx.Context.bge(aa, bb, lbl_false); break;
				case "<=": ctx.Context.blt(aa, bb, lbl_false); break;
				default: Debug.Assert(false); return null;
				}
				ctx.Context.ld(rdest, 1);
				ctx.emit(lbl_false);
				return new ExprValue(rdest); 
			}
		}

		public override void EvalBool(FunctionContex ctx, string lbl_true, string lbl_false) {
			if (a.IsConstExpr() && b.IsConstExpr()) {
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);

				if (lbl_true != null) {
					switch (this.op) {
					case "==": if (aa.c == bb.c) ctx.Context.jmp(lbl_true); break;
					case "!=": if (aa.c != bb.c) ctx.Context.jmp(lbl_true); break;
					case ">":  if (aa.c > bb.c)  ctx.Context.jmp(lbl_true); break;
					case ">=": if (aa.c >= bb.c) ctx.Context.jmp(lbl_true); break;
					case "<":  if (aa.c < bb.c)  ctx.Context.jmp(lbl_true); break;
					case "<=": if (aa.c <= bb.c) ctx.Context.jmp(lbl_true); break;
					}
				} else {
					switch (this.op) {
					case "==": if (aa.c != bb.c) ctx.Context.jmp(lbl_false); break;
					case "!=": if (aa.c == bb.c) ctx.Context.jmp(lbl_false); break;
					case ">":  if (aa.c <= bb.c) ctx.Context.jmp(lbl_false); break;
					case ">=": if (aa.c < bb.c)  ctx.Context.jmp(lbl_false); break;
					case "<":  if (aa.c >= bb.c) ctx.Context.jmp(lbl_false); break;
					case "<=": if (aa.c > bb.c)  ctx.Context.jmp(lbl_false); break;
					}
				}
			} else {
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);

				if (lbl_true != null) {
					switch (this.op) {
					case "==": ctx.Context.beq(aa, bb, lbl_true); break;
					case "!=": ctx.Context.bne(aa, bb, lbl_true); break;
					case ">":  ctx.Context.bgt(aa, bb, lbl_true); break;
					case ">=": ctx.Context.bge(aa, bb, lbl_true); break;
					case "<":  ctx.Context.blt(aa, bb, lbl_true); break;
					case "<=": ctx.Context.ble(aa, bb, lbl_true); break;
					}
				} else {
					switch (this.op) {
					case "==": ctx.Context.bne(aa, bb, lbl_false); break;
					case "!=": ctx.Context.beq(aa, bb, lbl_false); break;
					case ">":  ctx.Context.ble(aa, bb, lbl_false); break;
					case ">=": ctx.Context.blt(aa, bb, lbl_false); break;
					case "<":  ctx.Context.bge(aa, bb, lbl_false); break;
					case "<=": ctx.Context.blt(aa, bb, lbl_false); break;
					}
				}
			}
		}
	}


	public interface IBinOp {
		int c_op(int a, int b);
		void a_op(Context ctx, string dd, ExprValue aa, ExprValue bb);
	}

	public class BinAdd : IBinOp {
		public int  c_op(int a, int b) { return a + b; }
		public void a_op(Context ctx, string dd, ExprValue aa, ExprValue bb) { ctx.add(dd, aa, bb); }
	}
	public class BinSub : IBinOp {
		public int  c_op(int a, int b) { return a - b; }
		public void a_op(Context ctx, string dd, ExprValue aa, ExprValue bb) { ctx.sub(dd, aa, bb); }
	}
	public class BinMul : IBinOp {
		public int  c_op(int a, int b) { return a * b; }
		public void a_op(Context ctx, string dd, ExprValue aa, ExprValue bb) { ctx.mul(dd, aa, bb); }
	}
	public class BinDiv : IBinOp {
		public int  c_op(int a, int b) { return a / b; }
		public void a_op(Context ctx, string dd, ExprValue aa, ExprValue bb) { ctx.div(dd, aa, bb); }
	}
	public class BinRem : IBinOp {
		public int  c_op(int a, int b) { return a % b; }
		public void a_op(Context ctx, string dd, ExprValue aa, ExprValue bb) { ctx.rem(dd, aa, bb); }
	}
	public class BinOr_ : IBinOp {
		public int  c_op(int a, int b) { return a | b; }
		public void a_op(Context ctx, string dd, ExprValue aa, ExprValue bb) { ctx.or_(dd, aa, bb); }
	}
	public class BinXor : IBinOp {
		public int  c_op(int a, int b) { return a ^ b; }
		public void a_op(Context ctx, string dd, ExprValue aa, ExprValue bb) { ctx.xor(dd, aa, bb); }
	}
	public class BinAnd : IBinOp {
		public int  c_op(int a, int b) { return a & b; }
		public void a_op(Context ctx, string dd, ExprValue aa, ExprValue bb) { ctx.and(dd, aa, bb); }
	}
	public class BinShL : IBinOp {
		public int  c_op(int a, int b) { return a << b; }
		public void a_op(Context ctx, string dd, ExprValue aa, ExprValue bb) { ctx.shl(dd, aa, bb); }
	}
	public class BinShR : IBinOp {
		public int  c_op(int a, int b) { return a >> b; }
		public void a_op(Context ctx, string dd, ExprValue aa, ExprValue bb) { ctx.shr(dd, aa, bb); }
	}

	public class ExprBinGen : ExprBin {
		readonly IBinOp sop;

		public ExprBinGen(string op, ExprRoot a, ExprRoot b) : base(a, b) { 
			switch (op) {
			case "+":  sop = new BinAdd(); break;
			case "-":  sop = new BinSub(); break;
			case "*":  sop = new BinMul(); break;
			case "/":  sop = new BinDiv(); break;
			case "%":  sop = new BinRem(); break;
			case "|":  sop = new BinOr_(); break;
			case "&":  sop = new BinAnd(); break;
			case "^":  sop = new BinXor(); break;
			case "<<": sop = new BinShL(); break;
			case ">>": sop = new BinShR(); break;
			default: Debug.Assert(false); break;
			}
		}

		public override ExprValue EvalRight(FunctionContex ctx, string rdest) {
			if (a.IsConstExpr() && b.IsConstExpr()) {
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);
				var rr = sop.c_op(aa.c, bb.c);
				if (rdest != null) ctx.Context.ld(rdest, rr);
				return new ExprValue(rr);
			} else {
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);
				if (rdest == null) rdest = ctx.Context.NewTmp();
				sop.a_op(ctx.Context, rdest, aa, bb);
				return new ExprValue(rdest); 
			}
		}
	}


	public class ExprPlus : ExprUni {
		public ExprPlus(ExprRoot a) : base(a) { }
		public override ExprValue EvalRight(FunctionContex ctx, string rdest) { return a.EvalRight(ctx, rdest); }
	}

	public class ExprNeg : ExprUni {
		public ExprNeg(ExprRoot a) : base(a) { }

		public override ExprValue EvalRight(FunctionContex ctx, string rdest) {
			if (a.IsConstExpr()) {
				var n = a.EvalRight(ctx, null);
				if (rdest != null) ctx.Context.ld(rdest, -n.c);
				return n;
			}
			else {
				var ra = a.EvalRight(ctx, null);
				if (rdest == null) rdest = ctx.Context.NewTmp();
				ctx.Context.sub(rdest, new ExprValue(0), ra);
				return new ExprValue(rdest);
			}
		}
	}

	public class ExprNum : ExprRoot {
		public ExprNum(TokenAST a) { this.a = a; }
		readonly TokenAST a;

		public override ExprValue EvalRight(FunctionContex ctx, string rdest) {
			var n = new ExprValue(int.Parse(a.v));
			if (rdest != null) ctx.Context.ld(rdest, n.c);
			return n;
		}
		public override bool IsConstExpr() { return true; }
	}
	public class ExprBool : ExprRoot {
		public ExprBool(bool v) { this.v = v; }
		readonly bool v;

		public override ExprValue EvalRight(FunctionContex ctx, string rdest) {
			var n = new ExprValue(v);
			if (rdest != null) ctx.Context.ld(rdest, n.c);
			return n;
		}
		public override bool IsConstExpr() { return true; }
	}

	public class ExprId : ExprRoot {
		public ExprId(TokenAST a) { this.a = a; }
		readonly TokenAST a;

		public override string EvalLeft(FunctionContex ctx) { return ctx.GerVar(a.v); }

		public override ExprValue EvalRight(FunctionContex ctx, string rdest) {
			string rvar = ctx.GerVar(a.v);
			if (rdest == null) rdest = rvar;
			if (rdest != rvar) ctx.Context.add(rdest, new ExprValue(rvar), new ExprValue(0));
			return new ExprValue(rvar);
		}
		public override bool IsConstExpr() { return false; }
	}

	public class ExprFun : ExprRoot {
		public ExprFun(TokenAST f, ExprList a) { this.f = f; this.a = a; }
		readonly TokenAST f;
		readonly ExprList a;

		public override ExprValue EvalRight(FunctionContex ctx, string rdest) {
			foreach (var ea in this.a) {
				ea.EvalRight(ctx, "rp");
			}
			if (rdest == null) rdest = ctx.Context.NewTmp();
			ctx.Context.js(rdest, this.f.v);
			return new ExprValue(rdest);
		}
		public override bool IsConstExpr() { return false; }
	}

	public class ExprList : IAST, IEnumerable<ExprRoot> {
		readonly List<ExprRoot> a;

		public ExprList() { a = new List<ExprRoot>(); }
		public ExprList(ExprRoot e) { a = new List<ExprRoot>(); Add(e); }
		public ExprList Add(ExprRoot e) { a.Add(e); return this; }

		public IEnumerator<ExprRoot> GetEnumerator() { return a.GetEnumerator(); }
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return a.GetEnumerator(); }
	}
	////////////////////////

	public partial class MParser {
		public MParser() : base(0) { }

		public FunList Start(LexReader rd) {
			this.init(rd);
			return this.start(null);
		}
	}
}
