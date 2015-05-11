#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.

using System;
using System.Collections.Generic;
using LLParserLexerLib;

namespace LLParserGenTest
{
	public partial class MParser : ParserBase
	{
		public const int CLASS = -2;
		public const int ID = -3;
		public const int VAR = -4;
		public const int FUN = -5;
		public const int IF = -6;
		public const int WHILE = -7;
		public const int BREAK = -8;
		public const int CONTINUE = -9;
		public const int RETURN = -10;
		public const int ELSE = -11;
		public const int OROR = -12;
		public const int ANDAND = -13;
		public const int EQ = -14;
		public const int NE = -15;
		public const int LT = -16;
		public const int LE = -17;
		public const int GT = -18;
		public const int GE = -19;
		public const int SHL = -20;
		public const int SHR = -21;
		public const int NUM = -22;
		public const int FALSE = -23;
		public const int TRUE = -24;
		public const int CAST = -25;
		public const int NEW = -26;
		
		Dictionary<int, string> _token;
		public override Dictionary<int, string> Token
		{
			get
			{
				if (_token == null)
				{
					_token = new Dictionary<int, string>();
					_token.Add(-1, "EOF");
					_token.Add(-2, "CLASS");
					_token.Add(-3, "ID");
					_token.Add(-4, "VAR");
					_token.Add(-5, "FUN");
					_token.Add(-6, "IF");
					_token.Add(-7, "WHILE");
					_token.Add(-8, "BREAK");
					_token.Add(-9, "CONTINUE");
					_token.Add(-10, "RETURN");
					_token.Add(-11, "ELSE");
					_token.Add(-12, "OROR");
					_token.Add(-13, "ANDAND");
					_token.Add(-14, "EQ");
					_token.Add(-15, "NE");
					_token.Add(-16, "LT");
					_token.Add(-17, "LE");
					_token.Add(-18, "GT");
					_token.Add(-19, "GE");
					_token.Add(-20, "SHL");
					_token.Add(-21, "SHR");
					_token.Add(-22, "NUM");
					_token.Add(-23, "FALSE");
					_token.Add(-24, "TRUE");
					_token.Add(-25, "CAST");
					_token.Add(-26, "NEW");
				}
				return _token;
			}
		}
		
		DeclList start(IAST start_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case -1:
				alt = 0;
				break;
			case FUN:
			case CLASS:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclList start_s = default(DeclList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					start_s = new DeclList();
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = tmp_1(nt1_i);
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
		
		DeclList tmp_1(IAST tmp_1_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case FUN:
				alt = 0;
				break;
			case CLASS:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclList tmp_1_s = default(DeclList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = decl_fun(nt1_i);
					nt2_i = (new DeclList()).Add(nt1_s); 
					var nt2_s = tmp_2(nt2_i);
					tmp_1_s = nt2_s; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = decl_cls(nt1_i);
					nt2_i = (new DeclList()).Add(nt1_s); 
					var nt2_s = tmp_2(nt2_i);
					tmp_1_s = nt2_s; 
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
			return tmp_1_s;
		}
		
		DeclList tmp_2(IAST tmp_2_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case -1:
				alt = 0;
				break;
			case FUN:
				alt = 1;
				break;
			case CLASS:
				alt = 2;
				break;
			default:
				Error();
				break;
			}
			
			DeclList tmp_2_s = default(DeclList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_2_s = (DeclList)tmp_2_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = decl_fun(nt1_i);
					nt2_i = ((DeclList)tmp_2_i).Add(nt1_s); 
					var nt2_s = tmp_2(nt2_i);
					tmp_2_s = nt2_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = decl_cls(nt1_i);
					nt2_i = ((DeclList)tmp_2_i).Add(nt1_s); 
					var nt2_s = tmp_2(nt2_i);
					tmp_2_s = nt2_s; 
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
			return tmp_2_s;
		}
		
		DeclClass decl_cls(IAST decl_cls_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case CLASS:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			DeclClass decl_cls_s = default(DeclClass);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(CLASS, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match('{', nt3_i);
					var nt4_s = cls_members_e(nt4_i);
					TokenAST nt5_s = Match('}', nt5_i);
					decl_cls_s = new DeclClass(nt2_s, nt4_s);
				}
				break;
			}
			
			switch (Next.ch)
			{
			case FUN:
			case CLASS:
			case -1:
				break;
			default:
				Error();
				break;
			}
			return decl_cls_s;
		}
		
		DeclList cls_members_e(IAST cls_members_e_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '}':
				alt = 0;
				break;
			case FUN:
			case VAR:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclList cls_members_e_s = default(DeclList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					cls_members_e_s = new DeclList();;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = cls_members(nt1_i);
					cls_members_e_s = nt1_s;
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
			return cls_members_e_s;
		}
		
		DeclList cls_members(IAST cls_members_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case FUN:
			case VAR:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			DeclList cls_members_s = default(DeclList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = cls_member(nt1_i);
					nt2_i = new DeclList().Add(nt1_s); 
					var nt2_s = tmp_3(nt2_i);
					cls_members_s = nt2_s; 
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
			return cls_members_s;
		}
		
		DeclList tmp_3(IAST tmp_3_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '}':
				alt = 0;
				break;
			case FUN:
			case VAR:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclList tmp_3_s = default(DeclList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_3_s = (DeclList)tmp_3_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = cls_member(nt1_i);
					nt2_i = ((DeclList)tmp_3_i).Add(nt1_s); 
					var nt2_s = tmp_3(nt2_i);
					tmp_3_s = nt2_s; 
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
			return tmp_3_s;
		}
		
		DeclRoot cls_member(IAST cls_member_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case FUN:
				alt = 0;
				break;
			case VAR:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclRoot cls_member_s = default(DeclRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = decl_fun(nt1_i);
					cls_member_s = nt1_s;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = decl_var(nt1_i);
					cls_member_s = nt1_s;
				}
				break;
			}
			
			switch (Next.ch)
			{
			case FUN:
			case VAR:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return cls_member_s;
		}
		
		DeclVar decl_var(IAST decl_var_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case VAR:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			DeclVar decl_var_s = default(DeclVar);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(VAR, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match(':', nt3_i);
					var nt4_s = type(nt4_i);
					TokenAST nt5_s = Match(';', nt5_i);
					decl_var_s = new DeclVar(nt2_s, nt4_s);
				}
				break;
			}
			
			switch (Next.ch)
			{
			case FUN:
			case VAR:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return decl_var_s;
		}
		
		DeclFun decl_fun(IAST decl_fun_i)
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
			
			DeclFun decl_fun_s = default(DeclFun);
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
					var nt9_i = default(IAST);
					var nt10_i = default(IAST);
					
					TokenAST nt1_s = Match(FUN, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match('(', nt3_i);
					var nt4_s = funargse(nt4_i);
					TokenAST nt5_s = Match(')', nt5_i);
					TokenAST nt6_s = Match(':', nt6_i);
					var nt7_s = type(nt7_i);
					TokenAST nt8_s = Match('{', nt8_i);
					var nt9_s = stmtliste(nt9_i);
					TokenAST nt10_s = Match('}', nt10_i);
					decl_fun_s = new DeclFun(nt2_s, nt4_s, nt7_s, nt9_s, nt10_s);
				}
				break;
			}
			
			switch (Next.ch)
			{
			case FUN:
			case CLASS:
			case -1:
			case VAR:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return decl_fun_s;
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
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match(ID, nt1_i);
					TokenAST nt2_s = Match(':', nt2_i);
					var nt3_s = type(nt3_i);
					nt4_i = new FunArgList(nt1_s, nt3_s); 
					var nt4_s = tmp_4(nt4_i);
					funargs_s = nt4_s; 
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
		
		FunArgList tmp_4(IAST tmp_4_i)
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
			
			FunArgList tmp_4_s = default(FunArgList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_4_s = (FunArgList)tmp_4_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(',', nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match(':', nt3_i);
					var nt4_s = type(nt4_i);
					nt5_i = ((FunArgList)tmp_4_i).Add(nt2_s, nt4_s); 
					var nt5_s = tmp_4(nt5_i);
					tmp_4_s = nt5_s; 
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
			return tmp_4_s;
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
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
			case RETURN:
				alt = 7;
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
					var nt3_s = expr(nt3_i);
					TokenAST nt4_s = Match(')', nt4_i);
					var nt5_s = stmt(nt5_i);
					var nt6_s = tmp_18(nt1_s, nt2_s, nt3_s, nt4_s, nt5_s, nt6_i);
					stmt_s = nt6_s;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = expr(nt1_i);
					TokenAST nt2_s = Match(';', nt2_i);
					stmt_s = new StmtExpr(nt2_s, nt1_s);
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(VAR, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match(':', nt3_i);
					var nt4_s = type(nt4_i);
					TokenAST nt5_s = Match(';', nt5_i);
					stmt_s = new StmtVar(nt1_s, nt2_s, nt4_s);
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
					stmt_s = new StmtBlock(nt1_s, nt2_s);
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
					var nt3_s = expr(nt3_i);
					TokenAST nt4_s = Match(')', nt4_i);
					var nt5_s = stmt(nt5_i);
					stmt_s = new StmtBlock(new StmtWhile(nt1_s, nt3_s, new StmtBlock(nt5_s)));
				}
				break;
			case 5:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(BREAK, nt1_i);
					TokenAST nt2_s = Match(';', nt2_i);
					stmt_s = new StmtBreak(nt1_s);;
				}
				break;
			case 6:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(CONTINUE, nt1_i);
					TokenAST nt2_s = Match(';', nt2_i);
					stmt_s = new StmtContinue(nt1_s);;
				}
				break;
			case 7:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(RETURN, nt1_i);
					var nt2_s = tmp_19(nt1_s, nt2_i);
					stmt_s = nt2_s;
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
			case RETURN:
			case '-':
			case '+':
			case NUM:
			case ID:
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return stmt_s;
		}
		
		StmtRoot tmp_18(IAST nt1_s, IAST nt2_s, IAST nt3_s, IAST nt4_s, IAST nt5_s, IAST tmp_18_i)
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
			case RETURN:
			case '-':
			case '+':
			case NUM:
			case ID:
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
			
			StmtRoot tmp_18_s = default(StmtRoot);
			switch (alt)
			{
			case 0:
				{
					var nt6_i = default(IAST);
					
					tmp_18_s = new StmtBlock(new StmtIf(((TokenAST)nt1_s), ((ExprRoot)nt3_s), new StmtBlock(((StmtRoot)nt5_s))));
				}
				break;
			case 1:
				{
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					
					TokenAST nt6_s = Match(ELSE, nt6_i);
					var nt7_s = stmt(nt7_i);
					tmp_18_s = new StmtBlock(new StmtIf(((TokenAST)nt1_s), ((ExprRoot)nt3_s), new StmtBlock(((StmtRoot)nt5_s)), new StmtBlock(nt7_s)));
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
			case RETURN:
			case '-':
			case '+':
			case NUM:
			case ID:
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_18_s;
		}
		
		StmtRoot tmp_19(IAST nt1_s, IAST tmp_19_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case ';':
				alt = 0;
				break;
			case '-':
			case '+':
			case NUM:
			case ID:
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			StmtRoot tmp_19_s = default(StmtRoot);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					
					TokenAST nt2_s = Match(';', nt2_i);
					tmp_19_s = new StmtReturn(((TokenAST)nt1_s));;
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					var nt2_s = expr(nt2_i);
					TokenAST nt3_s = Match(';', nt3_i);
					tmp_19_s = new StmtReturn(((TokenAST)nt1_s), nt2_s);;
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
			case RETURN:
			case '-':
			case '+':
			case NUM:
			case ID:
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_19_s;
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
			case RETURN:
			case '-':
			case '+':
			case NUM:
			case ID:
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
			case RETURN:
			case '-':
			case '+':
			case NUM:
			case ID:
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
					var nt2_s = tmp_5(nt2_i);
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
		
		StmtList tmp_5(IAST tmp_5_i)
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
			case RETURN:
			case '-':
			case '+':
			case NUM:
			case ID:
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			StmtList tmp_5_s = default(StmtList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_5_s = (StmtList)tmp_5_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = stmt(nt1_i);
					nt2_i = ((StmtList)tmp_5_i).Add(nt1_s); 
					var nt2_s = tmp_5(nt2_i);
					tmp_5_s = nt2_s; 
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
			return tmp_5_s;
		}
		
		ExprRoot expr(IAST expr_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '-':
			case '+':
			case NUM:
			case ID:
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot expr_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = e_ass(nt1_i);
					expr_s = nt1_s;
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
			return expr_s;
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
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
					var nt2_s = tmp_20(nt1_s, nt2_i);
					e_ass_s = nt2_s;
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_ass_s;
		}
		
		ExprRoot tmp_20(IAST nt1_s, IAST tmp_20_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '=':
				alt = 0;
				break;
			case ',':
			case ')':
			case ';':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_20_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt2_s = Match('=', nt2_i);
					var nt3_s = e_ass(nt3_i);
					tmp_20_s = new ExprAss(((ExprRoot)nt1_s), nt2_s, nt3_s);
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					
					tmp_20_s = ((ExprRoot)nt1_s);
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_20_s;
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
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
					var nt2_s = tmp_6(nt2_i);
					e_oror_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '=':
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_oror_s;
		}
		
		ExprRoot tmp_6(IAST tmp_6_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '=':
			case ',':
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
					
					TokenAST nt1_s = Match(OROR, nt1_i);
					var nt2_s = e_andand(nt2_i);
					nt3_i = new ExprBinLogical(((ExprRoot)tmp_6_i), nt1_s, nt2_s); 
					var nt3_s = tmp_6(nt3_i);
					tmp_6_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '=':
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_6_s;
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
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
					var nt2_s = tmp_7(nt2_i);
					e_andand_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_andand_s;
		}
		
		ExprRoot tmp_7(IAST tmp_7_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case OROR:
			case '=':
			case ',':
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
					
					TokenAST nt1_s = Match(ANDAND, nt1_i);
					var nt2_s = e_or(nt2_i);
					nt3_i = new ExprBinLogical(((ExprRoot)tmp_7_i), nt1_s, nt2_s); 
					var nt3_s = tmp_7(nt3_i);
					tmp_7_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_7_s;
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
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
					var nt2_s = tmp_8(nt2_i);
					e_or_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ANDAND:
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_or_s;
		}
		
		ExprRoot tmp_8(IAST tmp_8_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case ANDAND:
			case OROR:
			case '=':
			case ',':
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
					
					TokenAST nt1_s = Match('|', nt1_i);
					var nt2_s = e_xor(nt2_i);
					nt3_i = new ExprBinGen(((ExprRoot)tmp_8_i), nt1_s, nt2_s); 
					var nt3_s = tmp_8(nt3_i);
					tmp_8_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ANDAND:
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_8_s;
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
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
					var nt2_s = tmp_9(nt2_i);
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
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_xor_s;
		}
		
		ExprRoot tmp_9(IAST tmp_9_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ',':
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
					
					TokenAST nt1_s = Match('^', nt1_i);
					var nt2_s = e_and(nt2_i);
					nt3_i = new ExprBinGen(((ExprRoot)tmp_9_i), nt1_s, nt2_s); 
					var nt3_s = tmp_9(nt3_i);
					tmp_9_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_9_s;
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
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
					var nt2_s = tmp_10(nt2_i);
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
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_and_s;
		}
		
		ExprRoot tmp_10(IAST tmp_10_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ',':
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
					
					TokenAST nt1_s = Match('&', nt1_i);
					var nt2_s = e_eq(nt2_i);
					nt3_i = new ExprBinGen(((ExprRoot)tmp_10_i), nt1_s, nt2_s); 
					var nt3_s = tmp_10(nt3_i);
					tmp_10_s = nt3_s; 
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
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_10_s;
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
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
					var nt2_s = tmp_11(nt2_i);
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
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_eq_s;
		}
		
		ExprRoot tmp_11(IAST tmp_11_i)
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
			case ',':
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
					
					TokenAST nt1_s = Match(EQ, nt1_i);
					var nt2_s = e_lt(nt2_i);
					nt3_i = new ExprBinCompare(((ExprRoot)tmp_11_i), nt1_s, nt2_s); 
					var nt3_s = tmp_11(nt3_i);
					tmp_11_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(NE, nt1_i);
					var nt2_s = e_lt(nt2_i);
					nt3_i = new ExprBinCompare(((ExprRoot)tmp_11_i), nt1_s, nt2_s); 
					var nt3_s = tmp_11(nt3_i);
					tmp_11_s = nt3_s; 
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
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_11_s;
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
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
					var nt2_s = tmp_12(nt2_i);
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
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_lt_s;
		}
		
		ExprRoot tmp_12(IAST tmp_12_i)
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
			case ',':
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
					
					TokenAST nt1_s = Match(LT, nt1_i);
					var nt2_s = e_sh(nt2_i);
					nt3_i = new ExprBinCompare(((ExprRoot)tmp_12_i), nt1_s, nt2_s); 
					var nt3_s = tmp_12(nt3_i);
					tmp_12_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(LE, nt1_i);
					var nt2_s = e_sh(nt2_i);
					nt3_i = new ExprBinCompare(((ExprRoot)tmp_12_i), nt1_s, nt2_s); 
					var nt3_s = tmp_12(nt3_i);
					tmp_12_s = nt3_s; 
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(GT, nt1_i);
					var nt2_s = e_sh(nt2_i);
					nt3_i = new ExprBinCompare(((ExprRoot)tmp_12_i), nt1_s, nt2_s); 
					var nt3_s = tmp_12(nt3_i);
					tmp_12_s = nt3_s; 
				}
				break;
			case 4:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(GE, nt1_i);
					var nt2_s = e_sh(nt2_i);
					nt3_i = new ExprBinCompare(((ExprRoot)tmp_12_i), nt1_s, nt2_s); 
					var nt3_s = tmp_12(nt3_i);
					tmp_12_s = nt3_s; 
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
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_12_s;
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
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
					var nt2_s = tmp_13(nt2_i);
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
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_sh_s;
		}
		
		ExprRoot tmp_13(IAST tmp_13_i)
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
			case ',':
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
			
			ExprRoot tmp_13_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_13_s = (ExprRoot)tmp_13_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(SHL, nt1_i);
					var nt2_s = e_add(nt2_i);
					nt3_i = new ExprBinGen(((ExprRoot)tmp_13_i), nt1_s, nt2_s); 
					var nt3_s = tmp_13(nt3_i);
					tmp_13_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(SHR, nt1_i);
					var nt2_s = e_add(nt2_i);
					nt3_i = new ExprBinGen(((ExprRoot)tmp_13_i), nt1_s, nt2_s); 
					var nt3_s = tmp_13(nt3_i);
					tmp_13_s = nt3_s; 
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
			case ',':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_13_s;
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
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
					var nt2_s = tmp_14(nt2_i);
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
			case ',':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_add_s;
		}
		
		ExprRoot tmp_14(IAST tmp_14_i)
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
			case ',':
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
			
			ExprRoot tmp_14_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_14_s = (ExprRoot)tmp_14_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('+', nt1_i);
					var nt2_s = e_mul(nt2_i);
					nt3_i = new ExprBinGen(((ExprRoot)tmp_14_i), nt1_s, nt2_s); 
					var nt3_s = tmp_14(nt3_i);
					tmp_14_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('-', nt1_i);
					var nt2_s = e_mul(nt2_i);
					nt3_i = new ExprBinGen(((ExprRoot)tmp_14_i), nt1_s, nt2_s); 
					var nt3_s = tmp_14(nt3_i);
					tmp_14_s = nt3_s; 
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
			case ',':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_14_s;
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
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
					var nt2_s = tmp_15(nt2_i);
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
			case ',':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_mul_s;
		}
		
		ExprRoot tmp_15(IAST tmp_15_i)
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
			case ',':
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
			
			ExprRoot tmp_15_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_15_s = (ExprRoot)tmp_15_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('*', nt1_i);
					var nt2_s = e_una(nt2_i);
					nt3_i = new ExprBinGen(((ExprRoot)tmp_15_i), nt1_s, nt2_s); 
					var nt3_s = tmp_15(nt3_i);
					tmp_15_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('/', nt1_i);
					var nt2_s = e_una(nt2_i);
					nt3_i = new ExprBinGen(((ExprRoot)tmp_15_i), nt1_s, nt2_s); 
					var nt3_s = tmp_15(nt3_i);
					tmp_15_s = nt3_s; 
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('%', nt1_i);
					var nt2_s = e_una(nt2_i);
					nt3_i = new ExprBinGen(((ExprRoot)tmp_15_i), nt1_s, nt2_s); 
					var nt3_s = tmp_15(nt3_i);
					tmp_15_s = nt3_s; 
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
			case ',':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_15_s;
		}
		
		ExprRoot e_una(IAST e_una_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case NUM:
			case ID:
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
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
					e_una_s = new ExprNeg(nt1_s, nt2_s);
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match('+', nt1_i);
					var nt2_s = e_una(nt2_i);
					e_una_s = new ExprPlus(nt1_s, nt2_s);
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
			case ',':
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
			case FALSE:
				alt = 2;
				break;
			case TRUE:
				alt = 3;
				break;
			case '(':
				alt = 4;
				break;
			case CAST:
				alt = 5;
				break;
			case NEW:
				alt = 6;
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
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(ID, nt1_i);
					var nt2_s = tmp_21(nt1_s, nt2_i);
					e_prim_s = nt2_s;
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(FALSE, nt1_i);
					e_prim_s = new ExprBool(nt1_s, false);
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(TRUE, nt1_i);
					e_prim_s = new ExprBool(nt1_s, true);
				}
				break;
			case 4:
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
			case 5:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					
					TokenAST nt1_s = Match(CAST, nt1_i);
					TokenAST nt2_s = Match('(', nt2_i);
					var nt3_s = type(nt3_i);
					TokenAST nt4_s = Match(',', nt4_i);
					var nt5_s = expr(nt5_i);
					TokenAST nt6_s = Match(')', nt6_i);
					e_prim_s = new ExprCast(nt1_s, nt3_s, nt5_s);
				}
				break;
			case 6:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(NEW, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match('(', nt3_i);
					var nt4_s = e_list_opz(nt4_i);
					TokenAST nt5_s = Match(')', nt5_i);
					e_prim_s = new ExprNewObj(nt1_s, nt2_s, nt4_s);
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
			case ',':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_prim_s;
		}
		
		ExprRoot tmp_21(IAST nt1_s, IAST tmp_21_i)
		{
			int alt = 0;
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
			case ',':
			case ';':
				alt = 0;
				break;
			case '(':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_21_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					
					tmp_21_s = new ExprId(((TokenAST)nt1_s));
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt2_s = Match('(', nt2_i);
					var nt3_s = e_list_opz(nt3_i);
					TokenAST nt4_s = Match(')', nt4_i);
					tmp_21_s = new ExprFun(((TokenAST)nt1_s), nt3_s);
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
			case ',':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_21_s;
		}
		
		ExprList e_list_opz(IAST e_list_opz_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case ')':
				alt = 0;
				break;
			case '-':
			case '+':
			case NUM:
			case ID:
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprList e_list_opz_s = default(ExprList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					e_list_opz_s = new ExprList();
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = e_list(nt1_i);
					e_list_opz_s = nt1_s;
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
			return e_list_opz_s;
		}
		
		ExprList e_list(IAST e_list_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case '-':
			case '+':
			case NUM:
			case ID:
			case FALSE:
			case TRUE:
			case '(':
			case CAST:
			case NEW:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			ExprList e_list_s = default(ExprList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_ass(nt1_i);
					nt2_i = new ExprList(nt1_s); 
					var nt2_s = tmp_16(nt2_i);
					e_list_s = nt2_s; 
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
			return e_list_s;
		}
		
		ExprList tmp_16(IAST tmp_16_i)
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
			
			ExprList tmp_16_s = default(ExprList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_16_s = (ExprList)tmp_16_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(',', nt1_i);
					var nt2_s = e_ass(nt2_i);
					nt3_i = ((ExprList)tmp_16_i).Add(nt2_s); 
					var nt3_s = tmp_16(nt3_i);
					tmp_16_s = nt3_s; 
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
			return tmp_16_s;
		}
		
		TypeRoot type(IAST type_i)
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
			
			TypeRoot type_s = default(TypeRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(ID, nt1_i);
					nt2_i = new TypeSimple(nt1_s); 
					var nt2_s = tmp_17(nt2_i);
					type_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ';':
			case '{':
			case ',':
			case ')':
				break;
			default:
				Error();
				break;
			}
			return type_s;
		}
		
		TypeRoot tmp_17(IAST tmp_17_i)
		{
			int alt = 0;
			switch (Next.ch)
			{
			case ';':
			case '{':
			case ',':
			case ')':
				alt = 0;
				break;
			case '[':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			TypeRoot tmp_17_s = default(TypeRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_17_s = (TypeRoot)tmp_17_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('[', nt1_i);
					TokenAST nt2_s = Match(']', nt2_i);
					nt3_i = new TypeArray(((TypeRoot)tmp_17_i)); 
					var nt3_s = tmp_17(nt3_i);
					tmp_17_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.ch)
			{
			case ';':
			case '{':
			case ',':
			case ')':
				break;
			default:
				Error();
				break;
			}
			return tmp_17_s;
		}
		
		protected override RegAcceptList CreateRegAcceptList()
		{
			var acts = new RegAcceptList();
			acts.Add(new RegAnd(new RegAnd(new RegToken('f'), new RegToken('u')), new RegToken('n')), FUN);
			acts.Add(new RegAnd(new RegAnd(new RegToken('v'), new RegToken('a')), new RegToken('r')), VAR);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('c'), new RegToken('l')), new RegToken('a')), new RegToken('s')), new RegToken('s')), CLASS);
			acts.Add(new RegAnd(new RegAnd(new RegToken('n'), new RegToken('e')), new RegToken('w')), NEW);
			acts.Add(new RegAnd(new RegToken('i'), new RegToken('f')), IF);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('e'), new RegToken('l')), new RegToken('s')), new RegToken('e')), ELSE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('w'), new RegToken('h')), new RegToken('i')), new RegToken('l')), new RegToken('e')), WHILE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('b'), new RegToken('r')), new RegToken('e')), new RegToken('a')), new RegToken('k')), BREAK);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('c'), new RegToken('o')), new RegToken('n')), new RegToken('t')), new RegToken('i')), new RegToken('n')), new RegToken('u')), new RegToken('e')), CONTINUE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('c'), new RegToken('a')), new RegToken('s')), new RegToken('t')), CAST);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('r'), new RegToken('e')), new RegToken('t')), new RegToken('u')), new RegToken('r')), new RegToken('n')), RETURN);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('t'), new RegToken('r')), new RegToken('u')), new RegToken('e')), TRUE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('f'), new RegToken('a')), new RegToken('l')), new RegToken('s')), new RegToken('e')), FALSE);
			acts.Add(new RegToken(','), ',');
			acts.Add(new RegToken(';'), ';');
			acts.Add(new RegToken(':'), ':');
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
			acts.Add(new RegToken('['), '[');
			acts.Add(new RegToken(']'), ']');
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
