using LLParserLexerLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ET_DW_Builder
{
	public static class DateTimeExtensions
	{
		public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
		{
			int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
			return dt.AddDays(-1 * diff).Date;
		}
	}
	public abstract class DateExpr
	{
		public abstract DateTime Eval(DateTime dt);

	}

	public class DayExpr : DateExpr
	{
		TokenAST a;
		public DayExpr(TokenAST a) => this.a = a;

		public override DateTime Eval(DateTime dt)
		{
			var r = int.Parse(this.a.strRead, CultureInfo.InvariantCulture);
			return dt.AddDays(r);
		}
	}
	public class WeekExpr : DateExpr
	{
		// | WEEK  '(' NUM ',' first_last ',' offset ')'
		TokenAST num;
		TokenAST first_last;
		TokenAST offset;
		public WeekExpr(TokenAST a, TokenAST b, TokenAST c) => (this.num, this.first_last, this.offset) = (a, b, c);

		public override DateTime Eval(DateTime dt)
		{
			var num = int.Parse(this.num.strRead, CultureInfo.InvariantCulture);
			bool last = false;
			if (first_last != null)
				last = this.first_last.strRead == "last";
			var offset = 0;
			if (this.offset != null)
				offset = int.Parse(this.offset.strRead, CultureInfo.InvariantCulture);

			dt = dt.StartOfWeek(DayOfWeek.Monday);  // l'inizio di questa settimana

			dt = dt.AddDays(-7);  // l'inizio nella settiman precendente
			if (last)
				dt = dt.AddDays(+7 - 1);  // la fine della settimand precedente
			return dt.AddDays(offset);

		}
	}
	public class MonthExpr : DateExpr
	{
		// | MONTH  '(' NUM ',' first_last ',' offset ')'
		TokenAST num;
		TokenAST first_last;
		TokenAST offset;
		public MonthExpr(TokenAST a, TokenAST b, TokenAST c) => (this.num, this.first_last, this.offset) = (a, b, c);

		public override DateTime Eval(DateTime dt)
		{
			var num = int.Parse(this.num.strRead, CultureInfo.InvariantCulture);
			bool last = false;
			if (first_last != null)
				last = this.first_last.strRead == "last";
			var offset = 0;
			if (this.offset != null)
				offset = int.Parse(this.offset.strRead, CultureInfo.InvariantCulture);

			var d = new DateTime(dt.Year, dt.Month, 1).AddMonths(num);
			if (last)
				d = d.AddMonths(+1).AddDays(-1);
			return d.AddDays(offset);
		}
	}

}
