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

	public abstract class StmtRoot : IAST {
		public abstract void GenCode(Context ctx);
	}

	public class StmtList : StmtRoot {
		readonly List<StmtRoot> a;

		public StmtList() {
			this.a = new List<StmtRoot>();
		}

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
		readonly StmtRoot s;

		public StmtIf(ExprRoot e, StmtRoot s) {
			this.e = e;
			this.s = s;
		}

		public override void GenCode(Context ctx) {
			var lbl_false = ctx.NewLbl();
			e.EvalBool(ctx, null, lbl_false);
			s.GenCode(ctx);
			ctx.emit(lbl_false);
		}
	}

	public class StmtExpr : StmtRoot {
		readonly ExprRoot e;

		public StmtExpr(ExprRoot e) {
			this.e = e;
		}

		public override void GenCode(Context ctx) {
			e.EvalRight(ctx, null);
		}
	}


	public abstract class ExprRoot : IAST {
		public abstract string EvalRight(Context ctx, string rdest);

		public virtual void EvalBool(Context ctx, string lbl_true, string lbl_false) {
			string s = EvalRight(ctx, null);
			if (lbl_true != null)
				ctx.bne(s, "r0", lbl_true);
			else
				ctx.beq(s, "r0", lbl_false);
		}

		public virtual string EvalLeft(Context ctx) {
			throw new Exception("errore - no right ride");
		}
	}

	public class ExprAss : ExprRoot {
		protected ExprRoot a;
		protected ExprRoot b;

		public ExprAss(ExprRoot a, ExprRoot b) {
			this.a = a;
			this.b = b;
		}

		public override string EvalLeft(Context ctx) {
			return a.EvalLeft(ctx);
		}

		public override string EvalRight(Context ctx, string rdest) {
			string ra = a.EvalLeft(ctx);
			string rb = b.EvalRight(ctx, ra);
			if (rdest == null) rdest = rb;
			if (rdest != rb) ctx.add(rdest, "r0", rb);
			return rdest;
		}
	}

	public abstract class ExprBin : ExprRoot {
		protected ExprBin(ExprRoot a, ExprRoot b) {
			this.a = a;
			this.b = b;
		}

		protected ExprRoot a;
		protected ExprRoot b;
	}

	public abstract class ExprUni : ExprRoot {
		protected ExprUni(ExprRoot a) {
			this.a = a;
		}

		protected ExprRoot a;
	}

	public class ExprEq : ExprBin {
		string op;

		public ExprEq(string op, ExprRoot a, ExprRoot b)
			: base(a, b) {
			this.op = op;
		}

		public override string EvalRight(Context ctx, string rdest) {
			string aa = a.EvalRight(ctx, null);
			string bb = b.EvalRight(ctx, null);
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
			}
			ctx.ld(rdest, 1);
			ctx.emit(lbl_false);
			return rdest; 
		}

		public override void EvalBool(Context ctx, string lbl_true, string lbl_false) {
			string aa = a.EvalRight(ctx, null);
			string bb = b.EvalRight(ctx, null);

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

	public class ExprAdd : ExprBin {
		public ExprAdd(ExprRoot a, ExprRoot b)
			: base(a, b) {
		}

		public override string EvalRight(Context ctx, string rdest) {
			string aa = a.EvalRight(ctx, null);
			string bb = b.EvalRight(ctx, null);
			if (rdest == null) rdest = ctx.NewTmp();
			ctx.add(rdest, aa, bb);
			return rdest; 
		}
	}

	public class ExprSub : ExprBin {
		public ExprSub(ExprRoot a, ExprRoot b)
			: base(a, b) {
		}

		public override string EvalRight(Context ctx, string rdest) {
			string aa = a.EvalRight(ctx, null);
			string bb = b.EvalRight(ctx, null);
			if (rdest == null) rdest = ctx.NewTmp();
			ctx.sub(rdest, aa, bb);
			return rdest;
		}
	}

	public class ExprMul : ExprBin {
		public ExprMul(ExprRoot a, ExprRoot b)
			: base(a, b) {
		}

		public override string EvalRight(Context ctx, string rdest) {
			string aa = a.EvalRight(ctx, null);
			string bb = b.EvalRight(ctx, null);
			if (rdest == null) rdest = ctx.NewTmp();
			ctx.mul(rdest, aa, bb);
			return rdest;
		}
	}

	public class ExprDiv : ExprBin {
		public ExprDiv(ExprRoot a, ExprRoot b)
			: base(a, b) {
		}

		public override string EvalRight(Context ctx, string rdest) {
			string aa = a.EvalRight(ctx, null);
			string bb = b.EvalRight(ctx, null);
			if (rdest == null) rdest = ctx.NewTmp();
			ctx.div(rdest, aa, bb);
			return rdest;
		}
	}

	public class ExprPlus : ExprUni {
		public ExprPlus(ExprRoot a)
			: base(a) {
		}

		public override string EvalRight(Context ctx, string rdest) {
			return a.EvalRight(ctx, rdest);
		}
	}

	public class ExprNeg : ExprUni {
		public ExprNeg(ExprRoot a)
			: base(a) {
		}

		public override string EvalRight(Context ctx, string rdest) {
			string ra = a.EvalRight(ctx, rdest);
			if (rdest == null) rdest = ctx.NewTmp();
			ctx.sub(rdest, "r0", ra);
			return rdest;
		}
	}

	public class ExprNum : ExprRoot {
		public ExprNum(TokenAST a) {
			this.a = a;
		}

		readonly TokenAST a;

		public override string EvalRight(Context ctx, string rdest) {
			if (rdest == null) rdest = ctx.NewTmp();
			ctx.ld(rdest, int.Parse(a.v));
			return rdest;
		}
	}

	public class ExprId : ExprRoot {
		public ExprId(TokenAST a) {
			this.a = a;
		}

		readonly TokenAST a;

		public override string EvalLeft(Context ctx) {
			return ctx.GerVar(a.v);
		}

		public override string EvalRight(Context ctx, string rdest) {
			string rvar = ctx.GerVar(a.v);
			if (rdest == null) rdest = rvar;
			if (rdest != rvar) ctx.add(rdest, "r0", rvar);
			return rdest;
		}
	}

	////////////////////////

	public partial class MParser {
		public MParser()
			: base(0) {
		}

		public StmtRoot Start(LexReader rd) {
			this.init(rd);
			return this.start(null);
		}
	}
}
