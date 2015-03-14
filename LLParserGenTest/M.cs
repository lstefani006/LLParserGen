#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.

using System;
using System.Collections.Generic;
using LLParserLexerLib;

namespace LLParserGenTest
{
	public partial class MParser : ParserBase
	{
		public const int IF = -2;
		public const int NUM = -3;
		public const int ID = -4;
		
		Dictionary<int, string> _token;
		public override Dictionary<int, string> Token
		{
			get
			{
				if (_token == null)
				{
					_token = new Dictionary<int, string>();
					_token.Add(-1, "EOF");
					_token.Add(-2, "IF");
					_token.Add(-3, "NUM");
					_token.Add(-4, "ID");
				}
				return _token;
			}
		}
		
		StmtRoot start(IAST start_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case IF:
			case '-':
			case '+':
			case NUM:
			case ID:
			case '(':
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			StmtRoot start_s = default(StmtRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = stmt(nt1_i);
					start_s = nt1_s;
				}
				break;
			}
			
			switch (Next.ch)
			{
			case -1:
				break;
			default:
				Error();
				break;
			}
			return start_s;
		}
		
		StmtRoot stmt(IAST stmt_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case IF:
				alt = 0;
				break;
			case '-':
			case '+':
			case NUM:
			case ID:
			case '(':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			StmtRoot stmt_s = default(StmtRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(IF, nt1_i);
					TokenAST nt2_s = Match('(', nt2_i);
					var nt3_s = e_ass(nt3_i);
					TokenAST nt4_s = Match(')', nt4_i);
					var nt5_s = stmt(nt5_i);
					stmt_s = new StmtIf(nt3_s, nt5_s);
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_ass(nt1_i);
					TokenAST nt2_s = Match(';', nt2_i);
					stmt_s = new StmtExpr(nt1_s);
				}
				break;
			}
			
			switch (Next.ch)
			{
			case -1:
				break;
			default:
				Error();
				break;
			}
			return stmt_s;
		}
		
		ExprRoot e_ass(IAST e_ass_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '-':
			case '+':
			case NUM:
			case ID:
			case '(':
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot e_ass_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_add(nt1_i);
					var nt2_s = tmp_3(nt1_s, nt2_i);
					e_ass_s = nt2_s;
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_ass_s;
		}
		
		ExprRoot tmp_3(IAST nt1_s, IAST tmp_3_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '=':
				alt = 0;
				break;
			case ')':
			case ';':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_3_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt2_s = Match('=', nt2_i);
					var nt3_s = e_ass(nt3_i);
					tmp_3_s = new ExprAss(((ExprRoot)nt1_s), nt3_s);
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					
					tmp_3_s = ((ExprRoot)nt1_s);
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_3_s;
		}
		
		ExprRoot e_add(IAST e_add_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '-':
			case '+':
			case NUM:
			case ID:
			case '(':
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot e_add_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_mul(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_1(nt2_i);
					e_add_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_add_s;
		}
		
		ExprRoot tmp_1(IAST tmp_1_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '=':
			case ')':
			case ';':
				alt = 0;
				break;
			case '+':
				alt = 1;
				break;
			case '-':
				alt = 2;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_1_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_1_s = (ExprRoot)tmp_1_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('+', nt1_i);
					var nt2_s = e_mul(nt2_i);
					nt3_i = new ExprAdd(((ExprRoot)tmp_1_i), nt2_s); 
					var nt3_s = tmp_1(nt3_i);
					tmp_1_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('-', nt1_i);
					var nt2_s = e_mul(nt2_i);
					nt3_i = new ExprSub(((ExprRoot)tmp_1_i), nt2_s); 
					var nt3_s = tmp_1(nt3_i);
					tmp_1_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_1_s;
		}
		
		ExprRoot e_mul(IAST e_mul_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '-':
			case '+':
			case NUM:
			case ID:
			case '(':
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot e_mul_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_una(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_2(nt2_i);
					e_mul_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '+':
			case '-':
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_mul_s;
		}
		
		ExprRoot tmp_2(IAST tmp_2_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '+':
			case '-':
			case '=':
			case ')':
			case ';':
				alt = 0;
				break;
			case '*':
				alt = 1;
				break;
			case '/':
				alt = 2;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_2_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_2_s = (ExprRoot)tmp_2_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('*', nt1_i);
					var nt2_s = e_una(nt2_i);
					nt3_i = new ExprMul(((ExprRoot)tmp_2_i), nt2_s); 
					var nt3_s = tmp_2(nt3_i);
					tmp_2_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('/', nt1_i);
					var nt2_s = e_una(nt2_i);
					nt3_i = new ExprDiv(((ExprRoot)tmp_2_i), nt2_s); 
					var nt3_s = tmp_2(nt3_i);
					tmp_2_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '+':
			case '-':
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_2_s;
		}
		
		ExprRoot e_una(IAST e_una_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case NUM:
			case ID:
			case '(':
				alt = 0;
				break;
			case '-':
				alt = 1;
				break;
			case '+':
				alt = 2;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot e_una_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = e_prim(nt1_i);
					e_una_s = nt1_s;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match('-', nt1_i);
					var nt2_s = e_una(nt2_i);
					e_una_s = new ExprNeg(nt2_s);
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match('+', nt1_i);
					var nt2_s = e_una(nt2_i);
					e_una_s = new ExprPlus(nt2_s);
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '*':
			case '/':
			case '+':
			case '-':
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_una_s;
		}
		
		ExprRoot e_prim(IAST e_prim_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case NUM:
				alt = 0;
				break;
			case ID:
				alt = 1;
				break;
			case '(':
				alt = 2;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot e_prim_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(NUM, nt1_i);
					e_prim_s = new ExprNum(nt1_s);
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(ID, nt1_i);
					e_prim_s = new ExprNum(nt1_s);
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('(', nt1_i);
					var nt2_s = e_add(nt2_i);
					TokenAST nt3_s = Match(')', nt3_i);
					e_prim_s = nt2_s;
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '*':
			case '/':
			case '+':
			case '-':
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_prim_s;
		}
		
		protected override RegAcceptList CreateRegAcceptList()
		{
			var acts = new RegAcceptList();
			acts.Add(new RegToken('+'), '+');
			acts.Add(new RegToken('-'), '-');
			acts.Add(new RegToken('*'), '*');
			acts.Add(new RegToken('/'), '/');
			acts.Add(new RegToken('('), '(');
			acts.Add(new RegToken(')'), ')');
			acts.Add(new RegAnd(new RegOr(new RegOr(new RegToken('_'), new RegTokenRange(97, 122)), new RegTokenRange(65, 90)), new RegZeroOrMore(new RegOr(new RegOr(new RegOr(new RegToken('_'), new RegTokenRange(97, 122)), new RegTokenRange(65, 90)), new RegTokenRange(48, 57)))), ID);
			acts.Add(new RegOneOrMore(new RegTokenRange(48, 57)), NUM);
			acts.Add(new RegOneOrMore(new RegOr(new RegOr(new RegOr(new RegToken(' '), new RegToken(10)), new RegToken(13)), new RegToken(9))));
			return acts;
		}
	}
}
