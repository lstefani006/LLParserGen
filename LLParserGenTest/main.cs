using System;
using System.Diagnostics;
using LLParserLexerLib;
using System.Collections.Generic;

namespace LLParserGenTest
{
	class _ {
		public static void Main(string[] args) {
			using (var rd = (args.Length == 1 ? new LexReader(args[0]) : new LexReader(Console.In, "stdin"))) {
				var p = new MParser();
				var s = p.Start(rd);

				using (Context ctx = new Context()) {
					ctx.GenerateCode(s);
				}
			}
		}
	}

	public class ExprValue {
		public ExprValue(string s) { this._reg = s; }
		public ExprValue(int c) { this._c = c; }

		string _reg;
		int _c;

		public int c { get { return _c; } }
		public string reg { get { return _reg; } }

		public void SetReg(string r) {
			_reg = r;
		}

		public bool IsReg { get { return _reg != null; } }
		public bool IsConst { get { return _reg == null; } }


		public override string ToString() {
			if (IsReg) return this._reg;
			return this._c.ToString();
		}
	}

	public class Fun : IAST
	{
		public readonly string name;
		public readonly FunArgList args;
		public readonly StmtRoot body;
		public Fun(TokenAST name, FunArgList args, StmtRoot body) {
			this.name = name.v;
			this.args = args;
			this.body = body;
		}
	}
	public class FunArgList : IAST {
		public readonly List<string> args = new List<string>();

		public FunArgList() {
		}
		public FunArgList(TokenAST a) {
			this.Add(a);
		}

		public FunArgList Add(TokenAST arg) {
			args.Add(arg.v);
			return this;
		}
	}

	public abstract class StmtRoot : IAST {
		public abstract void GenCode(Context ctx);
	}

	public class StmtList : StmtRoot {
		readonly List<StmtRoot> a;

		public StmtList() { this.a = new List<StmtRoot>(); }
		public StmtList(StmtRoot s) { this.a = new List<StmtRoot>(); this.Add(s); }

		public StmtList Add(StmtRoot s) {
			this.a.Add(s); 
			return this;
		}

		public override void GenCode(Context ctx) {
			foreach (var s in a)
				s.GenCode(ctx);
		}
	}

	public class StmtDecl : StmtRoot {
		TokenAST a;

		public StmtDecl(LLParserLexerLib.TokenAST a) {
			this.a = a;
		}

		public override void GenCode(Context ctx) {
			ctx.AddDefVar(this.a.v);
		}
	}

	public class StmtIf : StmtRoot {
		readonly ExprRoot e;
		readonly StmtRoot sa;
		readonly StmtRoot sb;

		public StmtIf(ExprRoot e, StmtRoot sa, StmtRoot sb = null) {
			this.e = e;
			this.sa = sa;
			this.sb = sb;
		}

		public override void GenCode(Context ctx) {
			if (e.IsConstExpr()) {
				if (sb == null) {
					if (e.EvalRight(ctx, null).c != 0)
						this.sa.GenCode(ctx);
				} else {
					if (e.EvalRight(ctx, null).c != 0)
						this.sa.GenCode(ctx);
					else
						this.sb.GenCode(ctx);
				}
				if (e.EvalRight(ctx, null).c != 0)
					this.sa.GenCode(ctx);
				else if (sb != null)
					this.sb.GenCode(ctx);
			} else {
				if (this.sb == null) {
					var lbl_false = ctx.NewLbl();
					e.EvalBool(ctx, null, lbl_false);
					this.sa.GenCode(ctx);
					ctx.emit(lbl_false);

				} else {
					var lbl_out = ctx.NewLbl();
					var lbl_false = ctx.NewLbl();
					e.EvalBool(ctx, null, lbl_false);
					this.sa.GenCode(ctx);
					ctx.jmp(lbl_out);
					ctx.emit(lbl_false);
					this.sb.GenCode(ctx);
					ctx.emit(lbl_out);
				}
			}
		}
	}

	public class StmtWhile : StmtRoot {
		readonly ExprRoot e;
		readonly StmtRoot s;
		public StmtWhile(ExprRoot e, StmtRoot s) { this.e = e; this.s = s; }

		public override void GenCode(Context ctx) {
			if (this.e.IsConstExpr()) {
				if (this.e.EvalRight(ctx, null).c != 0) {
					var lbl_true = ctx.NewLbl();
					ctx.emit(lbl_true);
					s.GenCode(ctx);
					ctx.jmp(lbl_true);
				}
			} else {
				var lbl_true = ctx.NewLbl();
				var lbl_loop = ctx.NewLbl();
				ctx.jmp(lbl_loop);
				ctx.emit(lbl_true);
				this.s.GenCode(ctx);
				ctx.emit(lbl_loop);
				this.e.EvalBool(ctx, lbl_true, null);
			}
		}
	}
		
	public class StmtExpr : StmtRoot {
		readonly ExprRoot e;
		public StmtExpr(ExprRoot e) { this.e = e; }
		public override void GenCode(Context ctx) { e.EvalRight(ctx, null); }
	}

	public abstract class ExprRoot : IAST {
		public abstract ExprValue EvalRight(Context ctx, string rdest);
		public abstract bool IsConstExpr();

		public virtual void EvalBool(Context ctx, string lbl_true, string lbl_false) {
			var s = EvalRight(ctx, null);
			if (s.IsConst) {
				if (s.c != 0)
					ctx.jmp(lbl_true);
				else
					ctx.jmp(lbl_false);
			} else {
				if (lbl_true != null)
					ctx.bne(s, new ExprValue(0), lbl_true);
				else
					ctx.beq(s, new ExprValue(0), lbl_false);
			}
		}

		public virtual string EvalLeft(Context ctx) {
			throw new Exception("errore - no left side");
		}
	}


	public class ExprAss : ExprRoot {
		protected ExprRoot a;
		protected ExprRoot b;

		public ExprAss(ExprRoot a, ExprRoot b) { this.a = a; this.b = b; }

		public override string EvalLeft(Context ctx) { return a.EvalLeft(ctx); }
		public override bool IsConstExpr() { return a.IsConstExpr(); }

		public override ExprValue EvalRight(Context ctx, string rdest) {
			string ra = a.EvalLeft(ctx);
			var rb = b.EvalRight(ctx, ra);
			if (rb.IsReg) {
				if (rdest == null) rdest = rb.reg;
				if (rdest != rb.reg) ctx.add(rdest, new ExprValue(0), rb);
				return new ExprValue(rdest);
			} else {
				if (rdest != null) {
					ctx.ld(rdest, rb.c);
					ctx.ld(ra, rb.c);
					return new ExprValue(rdest);
				} else {
					ctx.ld(ra, rb.c);
					return new ExprValue(ra);
				}
			}
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

	public class ExprEq : ExprBin {
		string op;

		public ExprEq(string op, ExprRoot a, ExprRoot b) : base(a, b) { this.op = op; }

		public override ExprValue EvalRight(Context ctx, string rdest) {

			if (a.IsConstExpr() && b.IsConstExpr()) {
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);
				switch (this.op) {
				case "==": return new ExprValue(aa.c == bb.c ? 1 : 0);
				case "!=": return new ExprValue(aa.c != bb.c ? 1 : 0);
				case ">":  return new ExprValue(aa.c > bb.c ? 1 : 0);
				case ">=": return new ExprValue(aa.c >= bb.c ? 1 : 0);
				case "<":  return new ExprValue(aa.c < bb.c ? 1 : 0);
				case "<=": return new ExprValue(aa.c <= bb.c ? 1 : 0);
				case "&&": return new ExprValue(aa.c != 0 && bb.c != 0 ? 1 : 0);
				case "||": return new ExprValue(aa.c != 0 || bb.c != 0 ? 1 : 0);
				default: Debug.Assert(false); return null;
				}
			} else {
				if (op != "||" && op != "&&") {
					var aa = a.EvalRight(ctx, null);
					var bb = b.EvalRight(ctx, null);
					if (rdest == null) rdest = ctx.NewTmp();
					string lbl_false = ctx.NewLbl();
					ctx.ld(rdest, 0);
					switch (this.op) {
					case "==": 
						ctx.bne(aa, bb, lbl_false);
						break;
					case "!=": 
						ctx.beq(aa, bb, lbl_false);
						break;
					case ">": 
						ctx.ble(aa, bb, lbl_false);
						break;
					case ">=": 
						ctx.blt(aa, bb, lbl_false);
						break;
					case "<": 
						ctx.bge(aa, bb, lbl_false);
						break;
					case "<=": 
						ctx.blt(aa, bb, lbl_false);
						break;
					default:
						Debug.Assert(false);
						return null;
					}
					ctx.ld(rdest, 1);
					ctx.emit(lbl_false);
					return new ExprValue(rdest); 
				} else if (op == "||") {
					if (rdest == null) rdest = ctx.NewTmp();
					string lbl_false = ctx.NewLbl();
					string lbl_true = ctx.NewLbl();
					ctx.ld(rdest, 0);
					a.EvalBool(ctx, lbl_true, null);
					b.EvalBool(ctx, null, lbl_false);
					ctx.emit(lbl_true);
					ctx.ld(rdest, 1);
					ctx.emit(lbl_false);
					return new ExprValue(rdest); 
				} else if (op == "&&") {
					if (rdest == null) rdest = ctx.NewTmp();
					string lbl_false = ctx.NewLbl();
					string lbl_true = ctx.NewLbl();
					ctx.ld(rdest, 1);
					a.EvalBool(ctx, null, lbl_false);
					b.EvalBool(ctx, lbl_true, null);
					ctx.emit(lbl_false);
					ctx.ld(rdest, 0);
					ctx.emit(lbl_true);
					return new ExprValue(rdest); 
				} else {
					Debug.Assert(false);
					return null;
				}
			}
		}

		public override void EvalBool(Context ctx, string lbl_true, string lbl_false) {
			if (a.IsConstExpr() && b.IsConstExpr()) {
				var aa = a.EvalRight(ctx, null);
				var bb = b.EvalRight(ctx, null);
			
				if (lbl_true != null) {
					switch (this.op) {
					case "==": if (aa.c == bb.c) ctx.jmp(lbl_true); break;
					case "!=": if (aa.c != bb.c) ctx.jmp(lbl_true); break;
					case ">": if (aa.c > bb.c) ctx.jmp(lbl_true); break;
					case ">=": if (aa.c >= bb.c) ctx.jmp(lbl_true); break;
					case "<": if (aa.c < bb.c) ctx.jmp(lbl_true); break;
					case "<=": if (aa.c <= bb.c) ctx.jmp(lbl_true); break;
					case "&&": if (aa.c != 0 && bb.c != 0) ctx.jmp(lbl_true); break;
					case "||": if (aa.c != 0 || bb.c != 0) ctx.jmp(lbl_true); break;
					}
				} else {
					switch (this.op) {
					case "==": if (aa.c != bb.c) ctx.jmp(lbl_false); break;
					case "!=": if (aa.c == bb.c) ctx.jmp(lbl_false); break;
					case ">": if (aa.c <= bb.c) ctx.jmp(lbl_false); break;
					case ">=": if (aa.c < bb.c) ctx.jmp(lbl_false); break;
					case "<": if (aa.c >= bb.c) ctx.jmp(lbl_false); break;
					case "<=": if (aa.c > bb.c) ctx.jmp(lbl_false); break;
					case "&&": if (!(aa.c != 0 && bb.c != 0)) ctx.jmp(lbl_false); break;
					case "||": if (!(aa.c != 0 || bb.c != 0)) ctx.jmp(lbl_false); break;
					}
				}
			} else {

				if (op == "&&") {
					if (lbl_true != null) {
						lbl_false = ctx.NewLbl();
						a.EvalBool(ctx, null, lbl_false);
						b.EvalBool(ctx, null, lbl_false);
						ctx.jmp(lbl_true);
						ctx.emit(lbl_false);
					} else {
						lbl_true = ctx.NewLbl();
						a.EvalBool(ctx, null, lbl_false);
						b.EvalBool(ctx, null, lbl_false);
						ctx.emit(lbl_true);
					}

				} else if (op == "||") {
					if (lbl_true != null) {
						lbl_false = ctx.NewLbl();
						a.EvalBool(ctx, lbl_true, null);
						b.EvalBool(ctx, lbl_true, null);
						ctx.emit(lbl_false);
					} else {
						lbl_true = ctx.NewLbl();
						a.EvalBool(ctx, lbl_true, null);
						b.EvalBool(ctx, lbl_true, null);
						ctx.jmp(lbl_false);
						ctx.emit(lbl_true);
					}
				} else {
					var aa = a.EvalRight(ctx, null);
					var bb = b.EvalRight(ctx, null);

					if (lbl_true != null) {
						switch (this.op) {
						case "==":
							ctx.beq(aa, bb, lbl_true);
							break;
						case "!=":
							ctx.bne(aa, bb, lbl_true);
							break;
						case ">":
							ctx.bgt(aa, bb, lbl_true);
							break;
						case ">=":
							ctx.bge(aa, bb, lbl_true);
							break;
						case "<":
							ctx.blt(aa, bb, lbl_true);
							break;
						case "<=":
							ctx.ble(aa, bb, lbl_true);
							break;
						}
					} else {
						switch (this.op) {
						case "==":
							ctx.bne(aa, bb, lbl_false);
							break;
						case "!=":
							ctx.beq(aa, bb, lbl_false);
							break;
						case ">":
							ctx.ble(aa, bb, lbl_false);
							break;
						case ">=":
							ctx.blt(aa, bb, lbl_false);
							break;
						case "<":
							ctx.bge(aa, bb, lbl_false);
							break;
						case "<=":
							ctx.blt(aa, bb, lbl_false);
							break;
						}
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
			case "+": sop = new BinAdd(); break;
			case "-": sop = new BinSub(); break;
			case "*": sop = new BinMul(); break;
			case "/": sop = new BinDiv(); break;
			case "%": sop = new BinRem(); break;
			case "|": sop = new BinOr_(); break;
			case "&": sop = new BinAnd(); break;
			case "^": sop = new BinXor(); break;
			case "<<": sop = new BinShL(); break;
			case ">>": sop = new BinShR(); break;
			default: Debug.Assert(false); break;
			}
		}

		public override ExprValue EvalRight(Context ctx, string rdest) {
			var aa = a.EvalRight(ctx, null);
			var bb = b.EvalRight(ctx, null);

			if (aa.IsConst && bb.IsConst) {
				if (rdest != null) ctx.ld(rdest, sop.c_op(aa.c, bb.c));
				return new ExprValue(sop.c_op(aa.c, bb.c));
			} else {
					if (rdest == null) rdest = ctx.NewTmp();
				sop.a_op(ctx, rdest, aa, bb);
				return new ExprValue(rdest); 
			}
		}
	}


	public class ExprPlus : ExprUni {
		public ExprPlus(ExprRoot a) : base(a) { }

		public override ExprValue EvalRight(Context ctx, string rdest) {
			return a.EvalRight(ctx, rdest);
		}
	}

	public class ExprNeg : ExprUni {
		public ExprNeg(ExprRoot a) : base(a) { }

		public override ExprValue EvalRight(Context ctx, string rdest) {
			var ra = a.EvalRight(ctx, rdest);
			if (ra.IsReg) {
				if (rdest == null) rdest = ctx.NewTmp();
				ctx.sub(rdest, new ExprValue(0), ra);
				return new ExprValue(rdest);
			} else {
				if (rdest != null) {
					ctx.ld(rdest, -ra.c);
					return new ExprValue(rdest);
				} else 
					return new ExprValue(-ra.c);
			}
		}
	}

	public class ExprNum : ExprRoot {
		public ExprNum(TokenAST a) { this.a = a; }
		readonly TokenAST a;

		public override ExprValue EvalRight(Context ctx, string rdest) {
			return new ExprValue(int.Parse(a.v));
		}
		public override bool IsConstExpr() { return true; }
	}

	public class ExprId : ExprRoot {
		public ExprId(TokenAST a) { this.a = a; }
		readonly TokenAST a;

		public override string EvalLeft(Context ctx) {
			return ctx.GerVar(a.v);
		}

		public override ExprValue EvalRight(Context ctx, string rdest) {
			string rvar = ctx.GerVar(a.v);
			if (rdest == null) rdest = rvar;
			if (rdest != rvar) ctx.add(rdest, new ExprValue(rvar), new ExprValue(0));
			return new ExprValue(rdest);
		}
		public override bool IsConstExpr() { return false; }
	}

	////////////////////////

	public partial class MParser {
		public MParser()
			: base(0) {
		}

		public Fun Start(LexReader rd) {
			this.init(rd);
			return this.start(null);
		}
	}
}
