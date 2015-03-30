#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.

using System;
using System.Collections.Generic;
using LLParserLexerLib;

namespace LLParserGenTest
{
	public partial class MParser : ParserBase
	{
		public const int FUN = -2;
		public const int ID = -3;
		public const int IF = -4;
		public const int VAR = -5;
		public const int WHILE = -6;
		public const int BREAK = -7;
		public const int CONTINUE = -8;
		public const int ELSE = -9;
		public const int OROR = -10;
		public const int ANDAND = -11;
		public const int EQ = -12;
		public const int NE = -13;
		public const int LT = -14;
		public const int LE = -15;
		public const int GT = -16;
		public const int GE = -17;
		public const int SHL = -18;
		public const int SHR = -19;
		public const int NUM = -20;
		
		Dictionary<int, string> _token;
		public override Dictionary<int, string> Token
		{
			get
			{
				if (_token == null)
				{
					_token = new Dictionary<int, string>();
					_token.Add(-1, "EOF");
					_token.Add(-2, "FUN");
					_token.Add(-3, "ID");
					_token.Add(-4, "IF");
					_token.Add(-5, "VAR");
					_token.Add(-6, "WHILE");
					_token.Add(-7, "BREAK");
					_token.Add(-8, "CONTINUE");
					_token.Add(-9, "ELSE");
					_token.Add(-10, "OROR");
					_token.Add(-11, "ANDAND");
					_token.Add(-12, "EQ");
					_token.Add(-13, "NE");
					_token.Add(-14, "LT");
					_token.Add(-15, "LE");
					_token.Add(-16, "GT");
					_token.Add(-17, "GE");
					_token.Add(-18, "SHL");
					_token.Add(-19, "SHR");
					_token.Add(-20, "NUM");
				}
				return _token;
			}
		}
		
		Fun start(IAST start_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case FUN:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			Fun start_s = default(Fun);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = fun(nt1_i);
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
		
		Fun fun(IAST fun_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case FUN:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			Fun fun_s = default(Fun);
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
					var nt7_i = default(IAST);
					var nt8_i = default(IAST);
					
					TokenAST nt1_s = Match(FUN, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match('(', nt3_i);
					var nt4_s = funargse(nt4_i);
					TokenAST nt5_s = Match(')', nt5_i);
					TokenAST nt6_s = Match('{', nt6_i);
					var nt7_s = stmtlist(nt7_i);
					TokenAST nt8_s = Match('}', nt8_i);
					fun_s = new Fun(nt2_s, nt4_s, nt7_s);
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
			return fun_s;
		}
		
		FunArgList funargse(IAST funargse_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case ')':
				alt = 0;
				break;
			case ID:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			FunArgList funargse_s = default(FunArgList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					funargse_s = new FunArgList();
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = funargs(nt1_i);
					funargse_s = nt1_s;
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ')':
				break;
			default:
				Error();
				break;
			}
			return funargse_s;
		}
		
		FunArgList funargs(IAST funargs_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case ID:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			FunArgList funargs_s = default(FunArgList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(ID, nt1_i);
					nt2_i = new FunArgList(nt1_s); 
					var nt2_s = tmp_1(nt2_i);
					funargs_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ')':
				break;
			default:
				Error();
				break;
			}
			return funargs_s;
		}
		
		FunArgList tmp_1(IAST tmp_1_i)
		{
			int alt = 0;
			switch (Next.ch)
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
			
			FunArgList tmp_1_s = default(FunArgList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_1_s = (FunArgList)tmp_1_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(',', nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					nt3_i = ((FunArgList)tmp_1_i).Add(nt2_s); 
					var nt3_s = tmp_1(nt3_i);
					tmp_1_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ')':
				break;
			default:
				Error();
				break;
			}
			return tmp_1_s;
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
			case VAR:
				alt = 2;
				break;
			case '{':
				alt = 3;
				break;
			case WHILE:
				alt = 4;
				break;
			case BREAK:
				alt = 5;
				break;
			case CONTINUE:
				alt = 6;
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
					var nt6_i = default(IAST);
					
					TokenAST nt1_s = Match(IF, nt1_i);
					TokenAST nt2_s = Match('(', nt2_i);
					var nt3_s = e_ass(nt3_i);
					TokenAST nt4_s = Match(')', nt4_i);
					var nt5_s = stmt(nt5_i);
					var nt6_s = tmp_13(nt1_s, nt2_s, nt3_s, nt4_s, nt5_s, nt6_i);
					stmt_s = nt6_s;
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
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(VAR, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match(';', nt3_i);
					stmt_s = new StmtDecl(nt2_s);
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('{', nt1_i);
					var nt2_s = stmtliste(nt2_i);
					TokenAST nt3_s = Match('}', nt3_i);
					stmt_s = nt2_s;
				}
				break;
			case 4:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(WHILE, nt1_i);
					TokenAST nt2_s = Match('(', nt2_i);
					var nt3_s = e_ass(nt3_i);
					TokenAST nt4_s = Match(')', nt4_i);
					var nt5_s = stmt(nt5_i);
					stmt_s = new StmtWhile(nt3_s, nt5_s);
				}
				break;
			case 5:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(BREAK, nt1_i);
					TokenAST nt2_s = Match(';', nt2_i);
					stmt_s = new StmtBreak();;
				}
				break;
			case 6:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(CONTINUE, nt1_i);
					TokenAST nt2_s = Match(';', nt2_i);
					stmt_s = new StmtContinue();;
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ELSE:
			case IF:
			case VAR:
			case '{':
			case WHILE:
			case BREAK:
			case CONTINUE:
			case '-':
			case '+':
			case NUM:
			case ID:
			case '(':
			case '}':
				break;
			default:
				Error();
				break;
			}
			return stmt_s;
		}
		
		StmtRoot tmp_13(IAST nt1_s, IAST nt2_s, IAST nt3_s, IAST nt4_s, IAST nt5_s, IAST tmp_13_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case IF:
			case VAR:
			case '{':
			case WHILE:
			case BREAK:
			case CONTINUE:
			case '-':
			case '+':
			case NUM:
			case ID:
			case '(':
			case '}':
				alt = 0;
				break;
			case ELSE:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			StmtRoot tmp_13_s = default(StmtRoot);
			switch (alt)
			{
			case 0:
				{
					var nt6_i = default(IAST);
					
					tmp_13_s = new StmtIf(((ExprRoot)nt3_s), ((StmtRoot)nt5_s));
				}
				break;
			case 1:
				{
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					
					TokenAST nt6_s = Match(ELSE, nt6_i);
					var nt7_s = stmt(nt7_i);
					tmp_13_s = new StmtIf(((ExprRoot)nt3_s), ((StmtRoot)nt5_s), nt7_s);
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ELSE:
			case IF:
			case VAR:
			case '{':
			case WHILE:
			case BREAK:
			case CONTINUE:
			case '-':
			case '+':
			case NUM:
			case ID:
			case '(':
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_13_s;
		}
		
		StmtList stmtliste(IAST stmtliste_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case IF:
			case VAR:
			case '{':
			case WHILE:
			case BREAK:
			case CONTINUE:
			case '-':
			case '+':
			case NUM:
			case ID:
			case '(':
				alt = 0;
				break;
			case '}':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			StmtList stmtliste_s = default(StmtList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = stmtlist(nt1_i);
					stmtliste_s = nt1_s;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					stmtliste_s = new StmtList();;
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '}':
				break;
			default:
				Error();
				break;
			}
			return stmtliste_s;
		}
		
		StmtList stmtlist(IAST stmtlist_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case IF:
			case VAR:
			case '{':
			case WHILE:
			case BREAK:
			case CONTINUE:
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
			
			StmtList stmtlist_s = default(StmtList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = stmt(nt1_i);
					nt2_i = new StmtList(nt1_s); 
					var nt2_s = tmp_2(nt2_i);
					stmtlist_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '}':
				break;
			default:
				Error();
				break;
			}
			return stmtlist_s;
		}
		
		StmtList tmp_2(IAST tmp_2_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '}':
				alt = 0;
				break;
			case IF:
			case VAR:
			case '{':
			case WHILE:
			case BREAK:
			case CONTINUE:
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
			
			StmtList tmp_2_s = default(StmtList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_2_s = (StmtList)tmp_2_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = stmt(nt1_i);
					nt2_i = ((StmtList)tmp_2_i).Add(nt1_s); 
					var nt2_s = tmp_2(nt2_i);
					tmp_2_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_2_s;
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
					
					var nt1_s = e_oror(nt1_i);
					var nt2_s = tmp_14(nt1_s, nt2_i);
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
		
		ExprRoot tmp_14(IAST nt1_s, IAST tmp_14_i)
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
			
			ExprRoot tmp_14_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt2_s = Match('=', nt2_i);
					var nt3_s = e_ass(nt3_i);
					tmp_14_s = new ExprAss(((ExprRoot)nt1_s), nt3_s);
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					
					tmp_14_s = ((ExprRoot)nt1_s);
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
			return tmp_14_s;
		}
		
		ExprRoot e_oror(IAST e_oror_i)
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
			
			ExprRoot e_oror_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_andand(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_3(nt2_i);
					e_oror_s = nt2_s; 
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
			return e_oror_s;
		}
		
		ExprRoot tmp_3(IAST tmp_3_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '=':
			case ')':
			case ';':
				alt = 0;
				break;
			case OROR:
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
					var nt1_i = default(IAST);
					
					tmp_3_s = (ExprRoot)tmp_3_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(OROR, nt1_i);
					var nt2_s = e_andand(nt2_i);
					nt3_i = new ExprBinLogical("||", ((ExprRoot)tmp_3_i), nt2_s); 
					var nt3_s = tmp_3(nt3_i);
					tmp_3_s = nt3_s; 
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
			return tmp_3_s;
		}
		
		ExprRoot e_andand(IAST e_andand_i)
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
			
			ExprRoot e_andand_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_or(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_4(nt2_i);
					e_andand_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case OROR:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_andand_s;
		}
		
		ExprRoot tmp_4(IAST tmp_4_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case OROR:
			case '=':
			case ')':
			case ';':
				alt = 0;
				break;
			case ANDAND:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_4_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_4_s = (ExprRoot)tmp_4_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(ANDAND, nt1_i);
					var nt2_s = e_or(nt2_i);
					nt3_i = new ExprBinLogical("&&", ((ExprRoot)tmp_4_i), nt2_s); 
					var nt3_s = tmp_4(nt3_i);
					tmp_4_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case OROR:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_4_s;
		}
		
		ExprRoot e_or(IAST e_or_i)
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
			
			ExprRoot e_or_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_xor(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_5(nt2_i);
					e_or_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_or_s;
		}
		
		ExprRoot tmp_5(IAST tmp_5_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				alt = 0;
				break;
			case '|':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_5_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_5_s = (ExprRoot)tmp_5_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('|', nt1_i);
					var nt2_s = e_xor(nt2_i);
					nt3_i = new ExprBinGen("|", ((ExprRoot)tmp_5_i), nt2_s); 
					var nt3_s = tmp_5(nt3_i);
					tmp_5_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_5_s;
		}
		
		ExprRoot e_xor(IAST e_xor_i)
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
			
			ExprRoot e_xor_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_and(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_6(nt2_i);
					e_xor_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_xor_s;
		}
		
		ExprRoot tmp_6(IAST tmp_6_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				alt = 0;
				break;
			case '^':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_6_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_6_s = (ExprRoot)tmp_6_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('^', nt1_i);
					var nt2_s = e_and(nt2_i);
					nt3_i = new ExprBinGen("^", ((ExprRoot)tmp_6_i), nt2_s); 
					var nt3_s = tmp_6(nt3_i);
					tmp_6_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_6_s;
		}
		
		ExprRoot e_and(IAST e_and_i)
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
			
			ExprRoot e_and_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_eq(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_7(nt2_i);
					e_and_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_and_s;
		}
		
		ExprRoot tmp_7(IAST tmp_7_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				alt = 0;
				break;
			case '&':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_7_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_7_s = (ExprRoot)tmp_7_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('&', nt1_i);
					var nt2_s = e_eq(nt2_i);
					nt3_i = new ExprBinGen("&", ((ExprRoot)tmp_7_i), nt2_s); 
					var nt3_s = tmp_7(nt3_i);
					tmp_7_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_7_s;
		}
		
		ExprRoot e_eq(IAST e_eq_i)
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
			
			ExprRoot e_eq_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_lt(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_8(nt2_i);
					e_eq_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_eq_s;
		}
		
		ExprRoot tmp_8(IAST tmp_8_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				alt = 0;
				break;
			case EQ:
				alt = 1;
				break;
			case NE:
				alt = 2;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_8_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_8_s = (ExprRoot)tmp_8_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(EQ, nt1_i);
					var nt2_s = e_lt(nt2_i);
					nt3_i = new ExprBinCompare("==", ((ExprRoot)tmp_8_i), nt2_s); 
					var nt3_s = tmp_8(nt3_i);
					tmp_8_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(NE, nt1_i);
					var nt2_s = e_lt(nt2_i);
					nt3_i = new ExprBinCompare("!=", ((ExprRoot)tmp_8_i), nt2_s); 
					var nt3_s = tmp_8(nt3_i);
					tmp_8_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_8_s;
		}
		
		ExprRoot e_lt(IAST e_lt_i)
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
			
			ExprRoot e_lt_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_sh(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_9(nt2_i);
					e_lt_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_lt_s;
		}
		
		ExprRoot tmp_9(IAST tmp_9_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				alt = 0;
				break;
			case LT:
				alt = 1;
				break;
			case LE:
				alt = 2;
				break;
			case GT:
				alt = 3;
				break;
			case GE:
				alt = 4;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_9_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_9_s = (ExprRoot)tmp_9_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(LT, nt1_i);
					var nt2_s = e_sh(nt2_i);
					nt3_i = new ExprBinCompare("<",  ((ExprRoot)tmp_9_i), nt2_s); 
					var nt3_s = tmp_9(nt3_i);
					tmp_9_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(LE, nt1_i);
					var nt2_s = e_sh(nt2_i);
					nt3_i = new ExprBinCompare("<=", ((ExprRoot)tmp_9_i), nt2_s); 
					var nt3_s = tmp_9(nt3_i);
					tmp_9_s = nt3_s; 
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(GT, nt1_i);
					var nt2_s = e_sh(nt2_i);
					nt3_i = new ExprBinCompare(">",  ((ExprRoot)tmp_9_i), nt2_s); 
					var nt3_s = tmp_9(nt3_i);
					tmp_9_s = nt3_s; 
				}
				break;
			case 4:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(GE, nt1_i);
					var nt2_s = e_sh(nt2_i);
					nt3_i = new ExprBinCompare(">=", ((ExprRoot)tmp_9_i), nt2_s); 
					var nt3_s = tmp_9(nt3_i);
					tmp_9_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_9_s;
		}
		
		ExprRoot e_sh(IAST e_sh_i)
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
			
			ExprRoot e_sh_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_add(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_10(nt2_i);
					e_sh_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case LT:
			case LE:
			case GT:
			case GE:
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_sh_s;
		}
		
		ExprRoot tmp_10(IAST tmp_10_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case LT:
			case LE:
			case GT:
			case GE:
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				alt = 0;
				break;
			case SHL:
				alt = 1;
				break;
			case SHR:
				alt = 2;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_10_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_10_s = (ExprRoot)tmp_10_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(SHL, nt1_i);
					var nt2_s = e_add(nt2_i);
					nt3_i = new ExprBinGen("<<", ((ExprRoot)tmp_10_i), nt2_s); 
					var nt3_s = tmp_10(nt3_i);
					tmp_10_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(SHR, nt1_i);
					var nt2_s = e_add(nt2_i);
					nt3_i = new ExprBinGen(">>", ((ExprRoot)tmp_10_i), nt2_s); 
					var nt3_s = tmp_10(nt3_i);
					tmp_10_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case LT:
			case LE:
			case GT:
			case GE:
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_10_s;
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
					var nt2_s = tmp_11(nt2_i);
					e_add_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case SHL:
			case SHR:
			case ')':
			case LT:
			case LE:
			case GT:
			case GE:
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_add_s;
		}
		
		ExprRoot tmp_11(IAST tmp_11_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case SHL:
			case SHR:
			case ')':
			case LT:
			case LE:
			case GT:
			case GE:
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
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
			
			ExprRoot tmp_11_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_11_s = (ExprRoot)tmp_11_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('+', nt1_i);
					var nt2_s = e_mul(nt2_i);
					nt3_i = new ExprBinGen("+", ((ExprRoot)tmp_11_i), nt2_s); 
					var nt3_s = tmp_11(nt3_i);
					tmp_11_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('-', nt1_i);
					var nt2_s = e_mul(nt2_i);
					nt3_i = new ExprBinGen("-", ((ExprRoot)tmp_11_i), nt2_s); 
					var nt3_s = tmp_11(nt3_i);
					tmp_11_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case SHL:
			case SHR:
			case ')':
			case LT:
			case LE:
			case GT:
			case GE:
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_11_s;
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
					var nt2_s = tmp_12(nt2_i);
					e_mul_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '+':
			case '-':
			case SHL:
			case SHR:
			case ')':
			case LT:
			case LE:
			case GT:
			case GE:
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_mul_s;
		}
		
		ExprRoot tmp_12(IAST tmp_12_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '+':
			case '-':
			case SHL:
			case SHR:
			case ')':
			case LT:
			case LE:
			case GT:
			case GE:
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
				alt = 0;
				break;
			case '*':
				alt = 1;
				break;
			case '/':
				alt = 2;
				break;
			case '%':
				alt = 3;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_12_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_12_s = (ExprRoot)tmp_12_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('*', nt1_i);
					var nt2_s = e_una(nt2_i);
					nt3_i = new ExprBinGen("*", ((ExprRoot)tmp_12_i), nt2_s); 
					var nt3_s = tmp_12(nt3_i);
					tmp_12_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('/', nt1_i);
					var nt2_s = e_una(nt2_i);
					nt3_i = new ExprBinGen("/", ((ExprRoot)tmp_12_i), nt2_s); 
					var nt3_s = tmp_12(nt3_i);
					tmp_12_s = nt3_s; 
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('%', nt1_i);
					var nt2_s = e_una(nt2_i);
					nt3_i = new ExprBinGen("%", ((ExprRoot)tmp_12_i), nt2_s); 
					var nt3_s = tmp_12(nt3_i);
					tmp_12_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '+':
			case '-':
			case SHL:
			case SHR:
			case ')':
			case LT:
			case LE:
			case GT:
			case GE:
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_12_s;
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
			case '%':
			case '+':
			case '-':
			case SHL:
			case SHR:
			case ')':
			case LT:
			case LE:
			case GT:
			case GE:
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
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
					e_prim_s = new ExprId(nt1_s);
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
			case '%':
			case '+':
			case '-':
			case SHL:
			case SHR:
			case ')':
			case LT:
			case LE:
			case GT:
			case GE:
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
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
			acts.Add(new RegAnd(new RegAnd(new RegToken('f'), new RegToken('u')), new RegToken('n')), FUN);
			acts.Add(new RegAnd(new RegAnd(new RegToken('v'), new RegToken('a')), new RegToken('r')), VAR);
			acts.Add(new RegAnd(new RegToken('i'), new RegToken('f')), IF);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('e'), new RegToken('l')), new RegToken('s')), new RegToken('e')), ELSE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('w'), new RegToken('h')), new RegToken('i')), new RegToken('l')), new RegToken('e')), WHILE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('b'), new RegToken('r')), new RegToken('e')), new RegToken('a')), new RegToken('k')), BREAK);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('c'), new RegToken('o')), new RegToken('n')), new RegToken('t')), new RegToken('i')), new RegToken('n')), new RegToken('u')), new RegToken('e')), CONTINUE);
			acts.Add(new RegToken(','), ',');
			acts.Add(new RegToken(';'), ';');
			acts.Add(new RegToken('+'), '+');
			acts.Add(new RegToken('-'), '-');
			acts.Add(new RegToken('*'), '*');
			acts.Add(new RegToken('%'), '%');
			acts.Add(new RegToken('|'), '|');
			acts.Add(new RegToken('!'), '!');
			acts.Add(new RegToken('^'), '^');
			acts.Add(new RegToken('&'), '&');
			acts.Add(new RegToken('/'), '/');
			acts.Add(new RegToken('('), '(');
			acts.Add(new RegToken(')'), ')');
			acts.Add(new RegToken('{'), '{');
			acts.Add(new RegToken('}'), '}');
			acts.Add(new RegToken('='), '=');
			acts.Add(new RegToken('>'), GT);
			acts.Add(new RegAnd(new RegToken('>'), new RegToken('=')), GE);
			acts.Add(new RegToken('<'), LT);
			acts.Add(new RegAnd(new RegToken('<'), new RegToken('=')), LE);
			acts.Add(new RegAnd(new RegToken('='), new RegToken('=')), EQ);
			acts.Add(new RegAnd(new RegToken('!'), new RegToken('=')), NE);
			acts.Add(new RegAnd(new RegToken('>'), new RegToken('>')), SHR);
			acts.Add(new RegAnd(new RegToken('<'), new RegToken('<')), SHL);
			acts.Add(new RegAnd(new RegToken('&'), new RegToken('&')), ANDAND);
			acts.Add(new RegAnd(new RegToken('|'), new RegToken('|')), OROR);
			acts.Add(new RegAnd(new RegOr(new RegOr(new RegToken('_'), new RegTokenRange(97, 122)), new RegTokenRange(65, 90)), new RegZeroOrMore(new RegOr(new RegOr(new RegOr(new RegToken('_'), new RegTokenRange(97, 122)), new RegTokenRange(65, 90)), new RegTokenRange(48, 57)))), ID);
			acts.Add(new RegOneOrMore(new RegTokenRange(48, 57)), NUM);
			acts.Add(new RegOneOrMore(new RegOr(new RegOr(new RegOr(new RegToken(' '), new RegToken(10)), new RegToken(13)), new RegToken(9))));
			acts.Add(new RegAnd(new RegAnd(new RegToken('/'), new RegToken('/')), new RegZeroOrMore(new RegTokenOutsideRange(10, 10))));
			return acts;
		}
	}
}
