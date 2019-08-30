#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.

using System;
using System.Text;
using System.Collections.Generic;
using LLParserLexerLib;

namespace ET_DW_Builder
{
	public partial class DateExprParser : ParserBase
	{
		public const int WEEK = -2;
		public const int NUM = -3;
		public const int MONTH = -4;
		public const int DAY = -5;
		public const int DATE = -6;
		public const int FIRST = -7;
		public const int LAST = -8;
		
		Dictionary<int, string> _token;
		public override Dictionary<int, string> Token
		{
			get
			{
				if (_token == null)
				{
					_token = new Dictionary<int, string>();
					_token.Add(-1, "EOF");
					_token.Add(-2, "WEEK");
					_token.Add(-3, "NUM");
					_token.Add(-4, "MONTH");
					_token.Add(-5, "DAY");
					_token.Add(-6, "DATE");
					_token.Add(-7, "FIRST");
					_token.Add(-8, "LAST");
				}
				return _token;
			}
		}
		
		DateExpr date_expr(IAST date_expr_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case WEEK:
				alt = 0;
				break;
			case MONTH:
				alt = 1;
				break;
			case DAY:
				alt = 2;
				break;
			case DATE:
				alt = 3;
				break;
			case -1:
				alt = 4;
				break;
			default:
				Error();
				break;
			}
			
			DateExpr date_expr_s = default(DateExpr);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					
					TokenAST nt1_s = Match(WEEK, nt1_i);
					TokenAST nt2_s = Match('(', nt2_i);
					TokenAST nt3_s = Match(NUM, nt3_i);
					TokenAST nt4_s = Match(',', nt4_i);
					var nt5_s = first_last(nt5_i);
					var nt6_s = tmp_1(nt1_s, nt2_s, nt3_s, nt4_s, nt5_s, nt6_i);
					date_expr_s = nt6_s;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					
					TokenAST nt1_s = Match(MONTH, nt1_i);
					TokenAST nt2_s = Match('(', nt2_i);
					TokenAST nt3_s = Match(NUM, nt3_i);
					TokenAST nt4_s = Match(',', nt4_i);
					var nt5_s = first_last(nt5_i);
					var nt6_s = tmp_2(nt1_s, nt2_s, nt3_s, nt4_s, nt5_s, nt6_i);
					date_expr_s = nt6_s;
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match(DAY, nt1_i);
					TokenAST nt2_s = Match('(', nt2_i);
					TokenAST nt3_s = Match(NUM, nt3_i);
					TokenAST nt4_s = Match(')', nt4_i);
					date_expr_s = new DayExpr(nt3_s);
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					var nt8_i = default(IAST);
					
					TokenAST nt1_s = Match(DATE, nt1_i);
					TokenAST nt2_s = Match('(', nt2_i);
					TokenAST nt3_s = Match(NUM, nt3_i);
					TokenAST nt4_s = Match(',', nt4_i);
					TokenAST nt5_s = Match(NUM, nt5_i);
					TokenAST nt6_s = Match(',', nt6_i);
					TokenAST nt7_s = Match(NUM, nt7_i);
					TokenAST nt8_s = Match(')', nt8_i);
					date_expr_s = new DateValueExpr(nt3_s, nt5_s, nt7_s);
				}
				break;
			case 4:
				{
					var nt1_i = default(IAST);
					
				}
				break;
			}
			
			switch (Next.token)
			{
			case -1:
				break;
			default:
				Error();
				break;
			}
			return date_expr_s;
		}
		
		DateExpr tmp_1(IAST nt1_s, IAST nt2_s, IAST nt3_s, IAST nt4_s, IAST nt5_s, IAST tmp_1_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ')':
				alt = 0;
				break;
			case ',':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DateExpr tmp_1_s = default(DateExpr);
			switch (alt)
			{
			case 0:
				{
					var nt6_i = default(IAST);
					
					TokenAST nt6_s = Match(')', nt6_i);
					tmp_1_s = new WeekExpr(((TokenAST)nt3_s), ((TokenAST)nt5_s), null);
				}
				break;
			case 1:
				{
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					var nt8_i = default(IAST);
					
					TokenAST nt6_s = Match(',', nt6_i);
					var nt7_s = offset(nt7_i);
					TokenAST nt8_s = Match(')', nt8_i);
					tmp_1_s = new WeekExpr(((TokenAST)nt3_s), ((TokenAST)nt5_s), nt7_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case -1:
				break;
			default:
				Error();
				break;
			}
			return tmp_1_s;
		}
		
		DateExpr tmp_2(IAST nt1_s, IAST nt2_s, IAST nt3_s, IAST nt4_s, IAST nt5_s, IAST tmp_2_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ')':
				alt = 0;
				break;
			case ',':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DateExpr tmp_2_s = default(DateExpr);
			switch (alt)
			{
			case 0:
				{
					var nt6_i = default(IAST);
					
					TokenAST nt6_s = Match(')', nt6_i);
					tmp_2_s = new MonthExpr(((TokenAST)nt3_s), ((TokenAST)nt5_s), null);
				}
				break;
			case 1:
				{
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					var nt8_i = default(IAST);
					
					TokenAST nt6_s = Match(',', nt6_i);
					var nt7_s = offset(nt7_i);
					TokenAST nt8_s = Match(')', nt8_i);
					tmp_2_s = new MonthExpr(((TokenAST)nt3_s), ((TokenAST)nt5_s), nt7_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case -1:
				break;
			default:
				Error();
				break;
			}
			return tmp_2_s;
		}
		
		TokenAST first_last(IAST first_last_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case FIRST:
				alt = 0;
				break;
			case LAST:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			TokenAST first_last_s = default(TokenAST);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(FIRST, nt1_i);
					first_last_s = nt1_s;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(LAST, nt1_i);
					first_last_s = nt1_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case ')':
			case ',':
				break;
			default:
				Error();
				break;
			}
			return first_last_s;
		}
		
		TokenAST offset(IAST offset_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case NUM:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			TokenAST offset_s = default(TokenAST);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(NUM, nt1_i);
					offset_s = nt1_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case ')':
				break;
			default:
				Error();
				break;
			}
			return offset_s;
		}
		
		protected override RegAcceptList CreateRegAcceptList()
		{
			var acts = new RegAcceptList();
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('w'), new RegToken('e')), new RegToken('e')), new RegToken('k')), WEEK);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('m'), new RegToken('o')), new RegToken('n')), new RegToken('t')), new RegToken('h')), MONTH);
			acts.Add(new RegAnd(new RegAnd(new RegToken('d'), new RegToken('a')), new RegToken('y')), DAY);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('d'), new RegToken('a')), new RegToken('t')), new RegToken('e')), DATE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('f'), new RegToken('i')), new RegToken('r')), new RegToken('s')), new RegToken('t')), FIRST);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('l'), new RegToken('a')), new RegToken('s')), new RegToken('t')), LAST);
			acts.Add(new RegToken('('), '(');
			acts.Add(new RegToken(')'), ')');
			acts.Add(new RegToken(','), ',');
			acts.Add(new RegAnd(new RegToken('-'), new RegOneOrMore(new RegTokenRange(48, 57))), NUM);
			acts.Add(new RegAnd(new RegToken('+'), new RegOneOrMore(new RegTokenRange(48, 57))), NUM);
			acts.Add(new RegOneOrMore(new RegTokenRange(48, 57)), NUM);
			acts.Add(new RegOneOrMore(new RegOr(new RegOr(new RegOr(new RegToken(' '), new RegToken(10)), new RegToken(13)), new RegToken(9))));
			return acts;
		}
	}
}
