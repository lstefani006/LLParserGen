using System;
using System.Diagnostics;
using LLParserLexerLib;

namespace LLParserGenTest
{
	class _
	{
		public static void Main(string[] args)
		{
			using (var rd = (args.Length == 1 ? new LexReader(args[0]) : new LexReader(Console.In, "stdin")))
			{

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

	public abstract class StmtRoot : IAST
	{
		public abstract void GenCode(Context ctx);
	}

	public class StmtIf : StmtRoot
	{
		ExprRoot e;
		StmtRoot s;
		public StmtIf(ExprRoot e, StmtRoot s) { this.e = e; this.s = s; }
		public override void GenCode(Context ctx)
		{
			var lbl_false = ctx.NewLbl();
			e.EvalBool(ctx, null, lbl_false);
			s.GenCode(ctx);
			ctx.emit(lbl_false);
		}
	}
	public class StmtExpr : StmtRoot
	{
		ExprRoot e;
		public StmtExpr(ExprRoot e) { this.e = e; }
		public override void GenCode(Context ctx)
		{
			e.EvalRight(ctx, null);
		}
	}


	public abstract class ExprRoot : IAST
	{
		public abstract string EvalRight(Context ctx, string rdest);
		public virtual void EvalBool(Context ctx, string lbl_true, string lbl_false)
		{
			string s = EvalRight(ctx, null);
			if (lbl_true != null) 
				ctx.bne(s, "r0", lbl_true); 
			else 
				ctx.beq(s, "r0", lbl_false);
		}
		public virtual string EvalLeft(Context ctx)
		{
			throw new Exception("errore - no right ride");
		}
	}

	public class ExprAss : ExprRoot
	{
		protected ExprRoot a;
		protected ExprRoot b;

		public ExprAss(ExprRoot a, ExprRoot b) { this.a = a; this.b = b; }
	
		public override string EvalLeft(Context ctx)
		{
			return EvalRight(ctx, null);
		}
		public override string EvalRight(Context ctx, string rdest) 
		{
			string ra = a.EvalLeft(ctx);
			string rb = b.EvalRight(ctx, ra);
			if (ra != rb)
				ctx.add(ra, "r0", rb);
			if (rdest != null && rdest != ra)
				ctx.add(rdest, "r0", ra);
			return ra;
		}
	}

	public abstract class ExprBin : ExprRoot
	{
		protected ExprBin(ExprRoot a, ExprRoot b) { this.a = a; this.b = b; }
		protected ExprRoot a;
		protected ExprRoot b;
	}
	public abstract class ExprUni : ExprRoot
	{
		protected ExprUni(ExprRoot a) { this.a = a; }
		protected ExprRoot a;
	}
	public class ExprAdd : ExprBin
	{
		public ExprAdd(ExprRoot a, ExprRoot b) : base(a, b) { }
		public override string EvalRight(Context ctx, string rdest) 
		{
			string aa = a.EvalRight(ctx, null);
			string bb = b.EvalRight(ctx, null);
			if (rdest == null) rdest = ctx.NewTmp();
			ctx.add(rdest, aa, bb);
			return rdest; 
		}
	}
	public class ExprSub : ExprBin
	{
		public ExprSub(ExprRoot a, ExprRoot b) : base(a, b) { }
		public override string EvalRight(Context ctx, string rdest)
		{
			string aa = a.EvalRight(ctx, null);
			string bb = b.EvalRight(ctx, null);
			if (rdest == null) rdest = ctx.NewTmp();
			ctx.sub(rdest, aa, bb);
			return rdest;
		}
	}
	public class ExprMul : ExprBin
	{
		public ExprMul(ExprRoot a, ExprRoot b) : base(a, b) { }
		public override string EvalRight(Context ctx, string rdest)
		{
			string aa = a.EvalRight(ctx, null);
			string bb = b.EvalRight(ctx, null);
			if (rdest == null) rdest = ctx.NewTmp();
			ctx.mul(rdest, aa, bb);
			return rdest;
		}
	}
	public class ExprDiv : ExprBin
	{
		public ExprDiv(ExprRoot a, ExprRoot b) : base(a, b) { }
		public override string EvalRight(Context ctx, string rdest)
		{
			string aa = a.EvalRight(ctx, null);
			string bb = b.EvalRight(ctx, null);
			if (rdest == null) rdest = ctx.NewTmp();
			ctx.div(rdest, aa, bb);
			return rdest;
		}
	}
	public class ExprPlus : ExprUni
	{
		public ExprPlus(ExprRoot a) : base(a) { }
		public override string EvalRight(Context ctx, string rdest) { return a.EvalRight(ctx, rdest); }
	}
	public class ExprNeg : ExprUni
	{
		public ExprNeg(ExprRoot a) : base(a) { }
		public override string EvalRight(Context ctx, string rdest) {
			string ra = a.EvalRight(ctx, rdest);
			ctx.sub(ra, "r0", ra);
			return ra;
		}
	}
	public class ExprNum : ExprRoot
	{
		public ExprNum(LLParserLexerLib.TokenAST a) { this.a = a; }
		LLParserLexerLib.TokenAST a;
		public override string EvalRight(Context ctx, string rdest) {
			if (rdest != null) rdest = ctx.NewTmp();
			ctx.ld(rdest, int.Parse(a.Value));
			return rdest; 
		}
	}
	public class ExprId : ExprRoot
	{
		public ExprId(LLParserLexerLib.TokenAST a) { this.a = a; }
		LLParserLexerLib.TokenAST a;
		public override string EvalRight(Context ctx, string rdest)
		{
			string rvar = "r4";
			if (rdest != rvar)
				ctx.add(rdest, "r0", rvar);
			return rvar;
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
