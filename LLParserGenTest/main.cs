using System;
using System.Diagnostics;
using LLParserLexerLib;

namespace LLParserGenTest
{
	class _
	{
		public static void Main(string [] args)
		{
			using (var rd = (args.Length == 1 ? new LexReader(args[0]) : new LexReader(Console.In, "stdin"))) {

				var p = new MParser();
				var s = p.Start(rd);

				using (Context ctx = new Context())
				{
					s.GenCode(ctx);
					Console.WriteLine();
				}
			}
		}
	}

	public abstract class StmtRoot : IAST {
		public abstract void GenCode(Context ctx);
	}

	public class StmtIf : StmtRoot {
		ExprRoot e;
		StmtRoot s;
		public StmtIf(ExprRoot e, StmtRoot s) { this.e = e; this.s = s; }
		public override void GenCode(Context ctx) {
		}
	}
	public class StmtExpr : StmtRoot {
		ExprRoot e;
		public StmtExpr(ExprRoot e) { this.e = e; }
		public override void GenCode(Context ctx) {
		}
	}


	public abstract class ExprRoot : IAST {
		public abstract string EvalLeft(Context ctx);
		public abstract string EvalRight(Context ctx, string rdest);
		public abstract void EvalBool(Context ctx, string lbl_true, string lbl_false);
	}

	public class ExprAss : ExprRoot {
		public ExprAss(ExprRoot a, ExprRoot b) { this.a = a; this.b = b; }
		protected ExprRoot a;
		protected ExprRoot b;
		public override string EvalLeft(Context ctx)
		{
			return "";
		}
		public override string EvalRight(Context ctx, string rdest){ return ""; }
		public override void EvalBool(Context ctx, string lbl_true, string lbl_false) {
			string s = this.EvalRight(ctx, null);
			//if (lbl_true != null) ctx.bne(s, "#0");
			//else if (lbl_false != null) ctx.be(s, "#0");
			//else Debug.Assert(false);
		}

	}

	public abstract class ExprBin : ExprRoot
	{
		protected ExprBin(ExprRoot a, ExprRoot b) { this.a = a; this.b = b; }
		protected ExprRoot a;
		protected ExprRoot b;
		public override void EvalBool(Context ctx, string lbl_true, string lbl_false) {
			string s = this.EvalRight(ctx, null);
			//if (lbl_true != null) ctx.bne(s, "#0");
			//else if (lbl_false != null) ctx.be(s, "#0");
			//else Debug.Assert(false);
		}
	}
	public abstract class ExprUni : ExprRoot
	{
		protected ExprUni(ExprRoot a) { this.a = a; }
		protected ExprRoot a;
		public override void EvalBool(Context ctx, string lbl_true, string lbl_false) {
			string s = this.EvalRight(ctx, null);
			//if (lbl_true != null) ctx.bne(s, "#0");
			//else if (lbl_false != null) ctx.be(s, "#0");
			//else Debug.Assert(false);
		}
	}
	public class ExprAdd : ExprBin
	{
		public ExprAdd(ExprRoot a, ExprRoot b) : base(a, b) {}
		public override string EvalRight(Context ctx, string rdest) { return a.EvalRight(ctx, null) + b.EvalRight(ctx, null); }
		public override string EvalLeft(Context ctx)
		{
			return "";
		}
	}
	public class ExprSub : ExprBin
	{
		public ExprSub(ExprRoot a, ExprRoot b) : base(a, b) {}
		public override string EvalRight(Context ctx, string rdest) { 
			return a.EvalRight(ctx, null) + "-" + b.EvalRight(ctx, null); 
		}
		public override string EvalLeft(Context ctx)
		{
			return "";
		}
	}
	public class ExprMul : ExprBin
	{
		public ExprMul(ExprRoot a, ExprRoot b) : base(a, b) {}
		public override string EvalRight(Context ctx, string rdest) { return a.EvalRight(ctx,null) + "*" + b.EvalRight(ctx,null); }
		public override string EvalLeft(Context ctx)
		{
			return "";
		}
	}
	public class ExprDiv : ExprBin
	{
		public ExprDiv(ExprRoot a, ExprRoot b) : base(a, b) {}
		public override string EvalRight(Context ctx, string rdest) { return a.EvalRight(ctx,null) + "/" + b.EvalRight(ctx,null); }
		public override string EvalLeft(Context ctx)
		{
			return "";
		}
	}
	public class ExprPlus : ExprUni
	{
		public ExprPlus(ExprRoot a) : base(a) {}
		public override string EvalRight(Context ctx, string rdest) { return a.EvalRight(ctx,null); }
		public override string EvalLeft(Context ctx)
		{
			return "";
		}
	}
	public class ExprNeg : ExprUni
	{
		public ExprNeg(ExprRoot a) : base(a) {}
		public override string EvalRight(Context ctx, string rdest) { return a.EvalRight(ctx,null); }
		public override string EvalLeft(Context ctx)
		{
			return "";
		}
	}
	public class ExprNum : ExprRoot
	{
		public ExprNum(LLParserLexerLib.TokenAST a) { this.a = a; }
		LLParserLexerLib.TokenAST a;
		public override string EvalRight(Context ctx, string rdest) { return a.v; }
		public override string EvalLeft(Context ctx)
		{
			return "";
		}
		public override void EvalBool(Context ctx, string lbl_true, string lbl_false) {
			string s = this.EvalRight(ctx, null);
			//if (lbl_true != null) ctx.bne(s, "#0");
			//else if (lbl_false != null) ctx.be(s, "#0");
			//else Debug.Assert(false);
		}
	}

	////////////////////////

	public partial class MParser
	{
		public MParser()
			: base(0)
		{
		}
		public StmtRoot Start(LexReader rd)
		{
			this.init(rd);
			return this.start(null);
		}
	}
}
