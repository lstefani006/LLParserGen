using ET_DW_Builder;
using LLParserLexerLib;
using System;
using System.IO;

namespace ET_DW_Builder
{

}


namespace LLDateExpr
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				string s = "day(,-1)";
				//s = "month(-1, last)";
				//s = "month(-1, first, +1)";
				//s = "week(-1, first)";

				DateTime dt = CalcDate(DateTime.Now, s);

				Console.WriteLine(dt);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

		}







		private static DateTime CalcDate(DateTime dt, string s)
		{
			var p = new DateExprParser();
			var sr = new StringReader(s);
			var expr = p.Start(new LexReader(sr, "Data da"));
			return expr.Eval(dt);
		}
	}
}
