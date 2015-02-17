using System;
using LLParserLexerLib;

namespace LLTest
{
	class _
	{
		public static void Main(string [] args)
		{
			using (var rd = new LexReader(args[0])) 
			{ 
				var p = new MParser(); 
				var e = p.Start(rd); 

				Console.WriteLine(e.Exec());
			}
		}
	}


	public abstract class ExprRoot : IAST
	{
		public abstract int Exec();
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
		public ExprAdd(ExprRoot a, ExprRoot b) : base(a, b) {}
		public override int Exec() { return a.Exec() + b.Exec(); }
	}
	public class ExprSub : ExprBin
	{
		public ExprSub(ExprRoot a, ExprRoot b) : base(a, b) {}
		public override int Exec() { return a.Exec() - b.Exec(); }
	}
	public class ExprMul : ExprBin
	{
		public ExprMul(ExprRoot a, ExprRoot b) : base(a, b) {}
		public override int Exec() { return a.Exec() * b.Exec(); }
	}
	public class ExprDiv : ExprBin
	{
		public ExprDiv(ExprRoot a, ExprRoot b) : base(a, b) {}
		public override int Exec() { return a.Exec() / b.Exec(); }
	}
	public class ExprPlus : ExprUni
	{
		public ExprPlus(ExprRoot a) : base(a) {}
		public override int Exec() { return a.Exec(); }
	}
	public class ExprNeg : ExprUni
	{
		public ExprNeg(ExprRoot a) : base(a) {}
		public override int Exec() { return a.Exec(); }
	}
	public class ExprNum : ExprRoot
	{
		public ExprNum(LLParserLexerLib.TokenAST a) { this.a = a; }
		LLParserLexerLib.TokenAST a;
		public override int Exec() { return int.Parse(a.v); }
	}

	////////////////////////

	public partial class MParser
	{
		public MParser()
			: base(0)
		{
		}
		public ExprRoot Start(LexReader rd)
		{
			this.init(rd);
			return this.start(null);
		}
	}
}
