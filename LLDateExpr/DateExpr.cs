using LLParserLexerLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ET_DW_Builder
{
	public partial class DateExprParser
	{
		public DateExprParser() : base(0) { }

		public DateExpr Start(LexReader rd)
		{
			this.init(rd);
			var v = this.date_expr(null);
			return v;
		}

		public static string CheckSyntax(string fieldName, string fieldValue)
		{
			try
			{
				var p = new DateExprParser();
				var r = p.Start(new LexReader(new StringReader(fieldValue), fieldName));
				return "";
			}
			catch (SyntaxError ex)
			{
				return ex.Message;
			}
		}
		public static DateTime Eval(DateTime dt, string fieldValue)
		{
			var p = new DateExprParser();
			var r = p.Start(new LexReader(new StringReader(fieldValue), ""));
			return r.Eval(dt);
		}
	}

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

	public class DateValueExpr : DateExpr
	{
		DateTime dt;
		public DateValueExpr(TokenAST YYYY, TokenAST MM, TokenAST DD)
		{
			try
			{
				int yyyy = int.Parse(YYYY.strRead, CultureInfo.InvariantCulture);
				int mm = int.Parse(MM.strRead, CultureInfo.InvariantCulture);
				int dd = int.Parse(DD.strRead, CultureInfo.InvariantCulture);
				dt = new DateTime(yyyy, mm, dd);
			}
			catch 
			{
				throw new SyntaxError((ISourceTrackable)YYYY, "wrong date format");
			}
		}
		public override DateTime Eval(DateTime dt)
		{
			return this.dt;
		}
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
		public WeekExpr(TokenAST a, TokenAST b, TokenAST c)
		{
			this.num = a;
			this.first_last = b;
			this.offset = c;
		}

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
		// sMONTH  '(' NUM ',' first_last ',' offset ')'
		TokenAST num;
		TokenAST first_last;
		TokenAST offset;
		public MonthExpr(TokenAST a, TokenAST b, TokenAST c) 
		{
			this.num = a;
			this.first_last = b;
			this.offset = c;
		}

	public override DateTime Eval(DateTime dt)
		{
			var num = int.Parse(this.num.strRead, CultureInfo.InvariantCulture);
			bool last = false;
			if (first_last != null)
				last = this.first_last.strRead == "last";
			var offset = 0;
			if (this.offset != null)
				offset = int.Parse(this.offset.strRead, CultureInfo.InvariantCulture);

			var d = new DateTime(dt.Year, dt.Month, 1).AddMonths(num);  // l'inizio del mese attuale, aggiungo i mesi in ingresso
			if (last)
				d = d.AddMonths(+1).AddDays(-1); // se vuole la fine prendo il mese successivo - 1 giorno
			return d.AddDays(offset);
		}
	}

}
