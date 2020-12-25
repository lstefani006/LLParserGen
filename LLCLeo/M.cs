#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.

using System;
using System.Collections.Generic;
using LLParserLexerLib;

namespace LLCLeo
{
	public partial class MParser : ParserBase
	{
		public const int NAMESPACE = -2;
		public const int ID = -3;
		public const int FUN = -4;
		public const int DEL = -5;
		public const int VAR = -6;
		public const int CLASS = -7;
		public const int PROTECTED = -8;
		public const int PUBLIC = -9;
		public const int PRIVATE = -10;
		public const int EXTERN = -11;
		public const int OROR = -12;
		public const int ANDAND = -13;
		public const int EQ = -14;
		public const int NE = -15;
		public const int LT = -16;
		public const int LE = -17;
		public const int GT = -18;
		public const int GE = -19;
		public const int LSH = -20;
		public const int RSH = -21;
		public const int INC = -22;
		public const int DEC = -23;
		public const int NUM = -24;
		public const int TRUE = -25;
		public const int FALSE = -26;
		public const int NEW = -27;
		public const int CAST = -28;
		public const int IF = -29;
		public const int WHILE = -30;
		public const int FOR = -31;
		public const int RETURN = -32;
		public const int ELSE = -33;
		public const int INT = -34;
		public const int VOID = -35;
		public const int BOOL = -36;
		
		Dictionary<int, string> _token;
		public override Dictionary<int, string> Token
		{
			get
			{
				if (_token == null)
				{
					_token = new Dictionary<int, string>();
					_token.Add(-1, "EOF");
					_token.Add(-2, "NAMESPACE");
					_token.Add(-3, "ID");
					_token.Add(-4, "FUN");
					_token.Add(-5, "DEL");
					_token.Add(-6, "VAR");
					_token.Add(-7, "CLASS");
					_token.Add(-8, "PROTECTED");
					_token.Add(-9, "PUBLIC");
					_token.Add(-10, "PRIVATE");
					_token.Add(-11, "EXTERN");
					_token.Add(-12, "OROR");
					_token.Add(-13, "ANDAND");
					_token.Add(-14, "EQ");
					_token.Add(-15, "NE");
					_token.Add(-16, "LT");
					_token.Add(-17, "LE");
					_token.Add(-18, "GT");
					_token.Add(-19, "GE");
					_token.Add(-20, "LSH");
					_token.Add(-21, "RSH");
					_token.Add(-22, "INC");
					_token.Add(-23, "DEC");
					_token.Add(-24, "NUM");
					_token.Add(-25, "TRUE");
					_token.Add(-26, "FALSE");
					_token.Add(-27, "NEW");
					_token.Add(-28, "CAST");
					_token.Add(-29, "IF");
					_token.Add(-30, "WHILE");
					_token.Add(-31, "FOR");
					_token.Add(-32, "RETURN");
					_token.Add(-33, "ELSE");
					_token.Add(-34, "INT");
					_token.Add(-35, "VOID");
					_token.Add(-36, "BOOL");
				}
				return _token;
			}
		}
		
		DeclRootList start(IAST start_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case NAMESPACE:
			case PROTECTED:
			case PUBLIC:
			case PRIVATE:
			case EXTERN:
			case FUN:
			case DEL:
			case VAR:
			case CLASS:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			DeclRootList start_s = default(DeclRootList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = decl(nt1_i);
					nt2_i = new DeclRootList(nt1_s); 
					var nt2_s = tmp_2(nt2_i);
					start_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case -1:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return start_s;
		}
		
		DeclRootList tmp_2(IAST tmp_2_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case -1:
			case '}':
				alt = 0;
				break;
			case NAMESPACE:
			case PROTECTED:
			case PUBLIC:
			case PRIVATE:
			case EXTERN:
			case FUN:
			case DEL:
			case VAR:
			case CLASS:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclRootList tmp_2_s = default(DeclRootList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_2_s = (DeclRootList)tmp_2_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = decl(nt1_i);
					nt2_i = ((DeclRootList)tmp_2_i).Add(nt1_s); 
					var nt2_s = tmp_2(nt2_i);
					tmp_2_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case -1:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_2_s;
		}
		
		DeclRoot decl(IAST decl_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case PROTECTED:
			case PUBLIC:
			case PRIVATE:
			case EXTERN:
			case FUN:
			case DEL:
			case VAR:
			case CLASS:
				alt = 0;
				break;
			case NAMESPACE:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclRoot decl_s = default(DeclRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = decl_visibility(nt1_i);
					var nt2_s = tmp_22(nt1_s, nt2_i);
					decl_s = nt2_s;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(NAMESPACE, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match('{', nt3_i);
					var nt4_s = start(nt4_i);
					TokenAST nt5_s = Match('}', nt5_i);
					decl_s = new DeclNamespace(nt2_s, nt4_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case NAMESPACE:
			case PROTECTED:
			case PUBLIC:
			case PRIVATE:
			case EXTERN:
			case FUN:
			case DEL:
			case VAR:
			case CLASS:
			case -1:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return decl_s;
		}
		
		DeclRoot tmp_22(IAST nt1_s, IAST tmp_22_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case FUN:
				alt = 0;
				break;
			case DEL:
				alt = 1;
				break;
			case VAR:
				alt = 2;
				break;
			case CLASS:
				alt = 3;
				break;
			default:
				Error();
				break;
			}
			
			DeclRoot tmp_22_s = default(DeclRoot);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					var nt8_i = default(IAST);
					var nt9_i = default(IAST);
					
					TokenAST nt2_s = Match(FUN, nt2_i);
					TokenAST nt3_s = Match(ID, nt3_i);
					TokenAST nt4_s = Match('(', nt4_i);
					var nt5_s = argse(nt5_i);
					TokenAST nt6_s = Match(')', nt6_i);
					TokenAST nt7_s = Match(':', nt7_i);
					var nt8_s = type(nt8_i);
					var nt9_s = decl_body(nt9_i);
					tmp_22_s = new DeclFunction (((DeclVisibility)nt1_s), nt3_s, nt5_s, nt8_s, nt9_s);
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					var nt8_i = default(IAST);
					var nt9_i = default(IAST);
					
					TokenAST nt2_s = Match(DEL, nt2_i);
					TokenAST nt3_s = Match(ID, nt3_i);
					TokenAST nt4_s = Match('(', nt4_i);
					var nt5_s = argse(nt5_i);
					TokenAST nt6_s = Match(')', nt6_i);
					TokenAST nt7_s = Match(':', nt7_i);
					var nt8_s = type(nt8_i);
					TokenAST nt9_s = Match(';', nt9_i);
					tmp_22_s = new DeclDelegate (((DeclVisibility)nt1_s), nt3_s, nt5_s, nt8_s);
				}
				break;
			case 2:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt2_s = Match(VAR, nt2_i);
					TokenAST nt3_s = Match(ID, nt3_i);
					var nt4_s = tmp_31(nt1_s, nt2_s, nt3_s, nt4_i);
					tmp_22_s = nt4_s;
				}
				break;
			case 3:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					
					TokenAST nt2_s = Match(CLASS, nt2_i);
					TokenAST nt3_s = Match(ID, nt3_i);
					TokenAST nt4_s = Match('{', nt4_i);
					var nt5_s = start(nt5_i);
					TokenAST nt6_s = Match('}', nt6_i);
					tmp_22_s = new DeclClass    (((DeclVisibility)nt1_s), nt3_s, nt5_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case NAMESPACE:
			case PROTECTED:
			case PUBLIC:
			case PRIVATE:
			case EXTERN:
			case FUN:
			case DEL:
			case VAR:
			case CLASS:
			case -1:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_22_s;
		}
		
		DeclRoot tmp_31(IAST nt1_s, IAST nt2_s, IAST nt3_s, IAST tmp_31_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ':':
				alt = 0;
				break;
			case '=':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclRoot tmp_31_s = default(DeclRoot);
			switch (alt)
			{
			case 0:
				{
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					
					TokenAST nt4_s = Match(':', nt4_i);
					var nt5_s = type(nt5_i);
					var nt6_s = tmp_33(nt1_s, nt2_s, nt3_s, nt4_s, nt5_s, nt6_i);
					tmp_31_s = nt6_s;
				}
				break;
			case 1:
				{
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					
					TokenAST nt4_s = Match('=', nt4_i);
					var nt5_s = e(nt5_i);
					TokenAST nt6_s = Match(';', nt6_i);
					tmp_31_s = new DeclVar      (((DeclVisibility)nt1_s), ((TokenAST)nt3_s), nt5_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case NAMESPACE:
			case PROTECTED:
			case PUBLIC:
			case PRIVATE:
			case EXTERN:
			case FUN:
			case DEL:
			case VAR:
			case CLASS:
			case -1:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_31_s;
		}
		
		DeclRoot tmp_33(IAST nt1_s, IAST nt2_s, IAST nt3_s, IAST nt4_s, IAST nt5_s, IAST tmp_33_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ';':
				alt = 0;
				break;
			case '=':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclRoot tmp_33_s = default(DeclRoot);
			switch (alt)
			{
			case 0:
				{
					var nt6_i = default(IAST);
					
					TokenAST nt6_s = Match(';', nt6_i);
					tmp_33_s = new DeclVar      (((DeclVisibility)nt1_s), ((TokenAST)nt3_s), ((TypeRoot)nt5_s));
				}
				break;
			case 1:
				{
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					var nt8_i = default(IAST);
					
					TokenAST nt6_s = Match('=', nt6_i);
					var nt7_s = e(nt7_i);
					TokenAST nt8_s = Match(';', nt8_i);
					tmp_33_s = new DeclVar      (((DeclVisibility)nt1_s), ((TokenAST)nt3_s), ((TypeRoot)nt5_s), nt7_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case NAMESPACE:
			case PROTECTED:
			case PUBLIC:
			case PRIVATE:
			case EXTERN:
			case FUN:
			case DEL:
			case VAR:
			case CLASS:
			case -1:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_33_s;
		}
		
		DeclVisibility decl_visibility(IAST decl_visibility_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case FUN:
			case DEL:
			case VAR:
			case CLASS:
				alt = 0;
				break;
			case PROTECTED:
				alt = 1;
				break;
			case PUBLIC:
				alt = 2;
				break;
			case PRIVATE:
				alt = 3;
				break;
			case EXTERN:
				alt = 4;
				break;
			default:
				Error();
				break;
			}
			
			DeclVisibility decl_visibility_s = default(DeclVisibility);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					decl_visibility_s = new DeclVisibility();
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(PROTECTED, nt1_i);
					decl_visibility_s = new DeclVisibility(nt1_s);
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(PUBLIC, nt1_i);
					decl_visibility_s = new DeclVisibility(nt1_s);
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(PRIVATE, nt1_i);
					decl_visibility_s = new DeclVisibility(nt1_s);
				}
				break;
			case 4:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(EXTERN, nt1_i);
					decl_visibility_s = new DeclVisibility(nt1_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case FUN:
			case DEL:
			case VAR:
			case CLASS:
				break;
			default:
				Error();
				break;
			}
			return decl_visibility_s;
		}
		
		DeclBody decl_body(IAST decl_body_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '{':
				alt = 0;
				break;
			case '(':
				alt = 1;
				break;
			case ';':
				alt = 2;
				break;
			default:
				Error();
				break;
			}
			
			DeclBody decl_body_s = default(DeclBody);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('{', nt1_i);
					var nt2_s = stmtlist(nt2_i);
					TokenAST nt3_s = Match('}', nt3_i);
					decl_body_s = new DeclBody(nt1_s, nt2_s);;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('(', nt1_i);
					var nt2_s = e(nt2_i);
					TokenAST nt3_s = Match(')', nt3_i);
					decl_body_s = new DeclBody(nt1_s, new StmtReturn(nt1_s, nt2_s));;
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(';', nt1_i);
					decl_body_s = new DeclBody(nt1_s, null);;
				}
				break;
			}
			
			switch (Next.token)
			{
			case NAMESPACE:
			case PROTECTED:
			case PUBLIC:
			case PRIVATE:
			case EXTERN:
			case FUN:
			case DEL:
			case VAR:
			case CLASS:
			case -1:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return decl_body_s;
		}
		
		DeclArgs argse(IAST argse_i)
		{
			int alt = 0;
			switch (Next.token)
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
			
			DeclArgs argse_s = default(DeclArgs);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					argse_s = new DeclArgs();
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = args(nt1_i);
					argse_s = nt1_s;
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
			return argse_s;
		}
		
		DeclArgs args(IAST args_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ID:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			DeclArgs args_s = default(DeclArgs);
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
					nt4_i = new DeclArgs(nt1_s, nt3_s); 
					var nt4_s = tmp_3(nt4_i);
					args_s = nt4_s; 
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
			return args_s;
		}
		
		DeclArgs tmp_3(IAST tmp_3_i)
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
			
			DeclArgs tmp_3_s = default(DeclArgs);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_3_s = (DeclArgs)tmp_3_i; 
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
					nt5_i = ((DeclArgs)tmp_3_i).Add(nt2_s, nt4_s); 
					var nt5_s = tmp_3(nt5_i);
					tmp_3_s = nt5_s; 
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
			return tmp_3_s;
		}
		
		ExprVar var(IAST var_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ID:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			ExprVar var_s = default(ExprVar);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(ID, nt1_i);
					nt2_i = new ExprVar(nt1_s); 
					var nt2_s = tmp_4(nt2_i);
					var_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '(':
			case '[':
			case INC:
			case DEC:
			case '*':
			case '/':
			case '%':
			case '+':
			case '-':
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return var_s;
		}
		
		ExprVar tmp_4(IAST tmp_4_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '(':
			case '[':
			case INC:
			case DEC:
			case '*':
			case '/':
			case '%':
			case '+':
			case '-':
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
				alt = 0;
				break;
			case '.':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprVar tmp_4_s = default(ExprVar);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_4_s = (ExprVar)tmp_4_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('.', nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					nt3_i = new ExprVar(((ExprVar)tmp_4_i), nt1_s, nt2_s); 
					var nt3_s = tmp_4(nt3_i);
					tmp_4_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '(':
			case '[':
			case INC:
			case DEC:
			case '*':
			case '/':
			case '%':
			case '+':
			case '-':
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_4_s;
		}
		
		ExprRoot e(IAST e_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot e_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_oror(nt1_i);
					var nt2_s = tmp_23(nt1_s, nt2_i);
					e_s = nt2_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_s;
		}
		
		ExprRoot tmp_23(IAST nt1_s, IAST tmp_23_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '=':
				alt = 0;
				break;
			case ';':
			case ')':
			case ',':
			case ']':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_23_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt2_s = Match('=', nt2_i);
					var nt3_s = e(nt3_i);
					tmp_23_s = new ExprAss(((ExprRoot)nt1_s), nt2_s, nt3_s);
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					
					tmp_23_s = ((ExprRoot)nt1_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_23_s;
		}
		
		ExprRoot e_oror(IAST e_oror_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
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
					var nt2_s = tmp_5(nt2_i);
					e_oror_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_oror_s;
		}
		
		ExprRoot tmp_5(IAST tmp_5_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				alt = 0;
				break;
			case OROR:
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
					
					TokenAST nt1_s = Match(OROR, nt1_i);
					var nt2_s = e_andand(nt2_i);
					nt3_i = new ExprBinOp(((ExprRoot)tmp_5_i), nt1_s, nt2_s); 
					var nt3_s = tmp_5(nt3_i);
					tmp_5_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_5_s;
		}
		
		ExprRoot e_andand(IAST e_andand_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
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
					var nt2_s = tmp_6(nt2_i);
					e_andand_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_andand_s;
		}
		
		ExprRoot tmp_6(IAST tmp_6_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				alt = 0;
				break;
			case ANDAND:
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
					
					TokenAST nt1_s = Match(ANDAND, nt1_i);
					var nt2_s = e_or(nt2_i);
					nt3_i = new ExprBinOp(((ExprRoot)tmp_6_i), nt1_s, nt2_s); 
					var nt3_s = tmp_6(nt3_i);
					tmp_6_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_6_s;
		}
		
		ExprRoot e_or(IAST e_or_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
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
					var nt2_s = tmp_7(nt2_i);
					e_or_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_or_s;
		}
		
		ExprRoot tmp_7(IAST tmp_7_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				alt = 0;
				break;
			case '|':
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
					
					TokenAST nt1_s = Match('|', nt1_i);
					var nt2_s = e_xor(nt2_i);
					nt3_i = new ExprBinOp(((ExprRoot)tmp_7_i), nt1_s, nt2_s); 
					var nt3_s = tmp_7(nt3_i);
					tmp_7_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_7_s;
		}
		
		ExprRoot e_xor(IAST e_xor_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
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
					var nt2_s = tmp_8(nt2_i);
					e_xor_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_xor_s;
		}
		
		ExprRoot tmp_8(IAST tmp_8_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				alt = 0;
				break;
			case '^':
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
					
					TokenAST nt1_s = Match('^', nt1_i);
					var nt2_s = e_and(nt2_i);
					nt3_i = new ExprBinOp(((ExprRoot)tmp_8_i), nt1_s, nt2_s); 
					var nt3_s = tmp_8(nt3_i);
					tmp_8_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_8_s;
		}
		
		ExprRoot e_and(IAST e_and_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
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
					var nt2_s = tmp_9(nt2_i);
					e_and_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_and_s;
		}
		
		ExprRoot tmp_9(IAST tmp_9_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				alt = 0;
				break;
			case '&':
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
					
					TokenAST nt1_s = Match('&', nt1_i);
					var nt2_s = e_eq(nt2_i);
					nt3_i = new ExprBinOp(((ExprRoot)tmp_9_i), nt1_s, nt2_s); 
					var nt3_s = tmp_9(nt3_i);
					tmp_9_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_9_s;
		}
		
		ExprRoot e_eq(IAST e_eq_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
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
					
					var nt1_s = e_cmp(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_10(nt2_i);
					e_eq_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_eq_s;
		}
		
		ExprRoot tmp_10(IAST tmp_10_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
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
					
					TokenAST nt1_s = Match(EQ, nt1_i);
					var nt2_s = e_cmp(nt2_i);
					nt3_i = new ExprBinOp(((ExprRoot)tmp_10_i), nt1_s, nt2_s); 
					var nt3_s = tmp_10(nt3_i);
					tmp_10_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(NE, nt1_i);
					var nt2_s = e_cmp(nt2_i);
					nt3_i = new ExprBinOp(((ExprRoot)tmp_10_i), nt1_s, nt2_s); 
					var nt3_s = tmp_10(nt3_i);
					tmp_10_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_10_s;
		}
		
		ExprRoot e_cmp(IAST e_cmp_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot e_cmp_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_shift(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_11(nt2_i);
					e_cmp_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_cmp_s;
		}
		
		ExprRoot tmp_11(IAST tmp_11_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
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
					
					TokenAST nt1_s = Match(LT, nt1_i);
					var nt2_s = e_shift(nt2_i);
					nt3_i = new ExprBinOp(((ExprRoot)tmp_11_i), nt1_s, nt2_s); 
					var nt3_s = tmp_11(nt3_i);
					tmp_11_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(LE, nt1_i);
					var nt2_s = e_shift(nt2_i);
					nt3_i = new ExprBinOp(((ExprRoot)tmp_11_i), nt1_s, nt2_s); 
					var nt3_s = tmp_11(nt3_i);
					tmp_11_s = nt3_s; 
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(GT, nt1_i);
					var nt2_s = e_shift(nt2_i);
					nt3_i = new ExprBinOp(((ExprRoot)tmp_11_i), nt1_s, nt2_s); 
					var nt3_s = tmp_11(nt3_i);
					tmp_11_s = nt3_s; 
				}
				break;
			case 4:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(GE, nt1_i);
					var nt2_s = e_shift(nt2_i);
					nt3_i = new ExprBinOp(((ExprRoot)tmp_11_i), nt1_s, nt2_s); 
					var nt3_s = tmp_11(nt3_i);
					tmp_11_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case EQ:
			case NE:
			case '&':
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_11_s;
		}
		
		ExprRoot e_shift(IAST e_shift_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot e_shift_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e_add(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_12(nt2_i);
					e_shift_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
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
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_shift_s;
		}
		
		ExprRoot tmp_12(IAST tmp_12_i)
		{
			int alt = 0;
			switch (Next.token)
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
			case ';':
			case ')':
			case ',':
			case ']':
				alt = 0;
				break;
			case LSH:
				alt = 1;
				break;
			case RSH:
				alt = 2;
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
					
					TokenAST nt1_s = Match(LSH, nt1_i);
					var nt2_s = e_add(nt2_i);
					nt3_i = new ExprBinOp(((ExprRoot)tmp_12_i), nt1_s, nt2_s); 
					var nt3_s = tmp_12(nt3_i);
					tmp_12_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(RSH, nt1_i);
					var nt2_s = e_add(nt2_i);
					nt3_i = new ExprBinOp(((ExprRoot)tmp_12_i), nt1_s, nt2_s); 
					var nt3_s = tmp_12(nt3_i);
					tmp_12_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
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
			case ';':
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_12_s;
		}
		
		ExprRoot e_add(IAST e_add_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
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
					var nt2_s = tmp_13(nt2_i);
					e_add_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_add_s;
		}
		
		ExprRoot tmp_13(IAST tmp_13_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
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
					
					TokenAST nt1_s = Match('+', nt1_i);
					var nt2_s = e_mul(nt2_i);
					nt3_i = new ExprAdd(((ExprRoot)tmp_13_i), nt1_s, nt2_s); 
					var nt3_s = tmp_13(nt3_i);
					tmp_13_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('-', nt1_i);
					var nt2_s = e_mul(nt2_i);
					nt3_i = new ExprSub(((ExprRoot)tmp_13_i), nt1_s, nt2_s); 
					var nt3_s = tmp_13(nt3_i);
					tmp_13_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_13_s;
		}
		
		ExprRoot e_mul(IAST e_mul_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
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
					var nt2_s = tmp_14(nt2_i);
					e_mul_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '+':
			case '-':
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_mul_s;
		}
		
		ExprRoot tmp_14(IAST tmp_14_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '+':
			case '-':
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
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
					
					TokenAST nt1_s = Match('*', nt1_i);
					var nt2_s = e_una(nt2_i);
					nt3_i = new ExprMul(((ExprRoot)tmp_14_i), nt1_s, nt2_s); 
					var nt3_s = tmp_14(nt3_i);
					tmp_14_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('/', nt1_i);
					var nt2_s = e_una(nt2_i);
					nt3_i = new ExprDiv(((ExprRoot)tmp_14_i), nt1_s, nt2_s); 
					var nt3_s = tmp_14(nt3_i);
					tmp_14_s = nt3_s; 
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('%', nt1_i);
					var nt2_s = e_una(nt2_i);
					nt3_i = new ExprRem(((ExprRoot)tmp_14_i), nt1_s, nt2_s); 
					var nt3_s = tmp_14(nt3_i);
					tmp_14_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '+':
			case '-':
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_14_s;
		}
		
		ExprRoot e_una(IAST e_una_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
				alt = 0;
				break;
			case '-':
				alt = 1;
				break;
			case '+':
				alt = 2;
				break;
			case INC:
				alt = 3;
				break;
			case DEC:
				alt = 4;
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
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(INC, nt1_i);
					var nt2_s = e_una(nt2_i);
					e_una_s = new ExprPreInc(nt1_s, nt2_s);
				}
				break;
			case 4:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(DEC, nt1_i);
					var nt2_s = e_una(nt2_i);
					e_una_s = new ExprPreDec(nt1_s, nt2_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case '*':
			case '/':
			case '%':
			case '+':
			case '-':
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
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
			switch (Next.token)
			{
			case NUM:
				alt = 0;
				break;
			case TRUE:
				alt = 1;
				break;
			case FALSE:
				alt = 2;
				break;
			case ID:
				alt = 3;
				break;
			case '(':
				alt = 4;
				break;
			case NEW:
				alt = 5;
				break;
			case CAST:
				alt = 6;
				break;
			case FUN:
				alt = 7;
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
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(NUM, nt1_i);
					nt2_i = new ExprNum(nt1_s); 
					var nt2_s = tmp_15(nt2_i);
					e_prim_s = nt2_s; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(TRUE, nt1_i);
					nt2_i = new ExprNum(nt1_s); 
					var nt2_s = tmp_15(nt2_i);
					e_prim_s = nt2_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(FALSE, nt1_i);
					nt2_i = new ExprNum(nt1_s); 
					var nt2_s = tmp_15(nt2_i);
					e_prim_s = nt2_s; 
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = var(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_15(nt2_i);
					e_prim_s = nt2_s; 
				}
				break;
			case 4:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match('(', nt1_i);
					var nt2_s = e(nt2_i);
					TokenAST nt3_s = Match(')', nt3_i);
					nt4_i = nt2_s; 
					var nt4_s = tmp_15(nt4_i);
					e_prim_s = nt4_s; 
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
					
					TokenAST nt1_s = Match(NEW, nt1_i);
					var nt2_s = type(nt2_i);
					TokenAST nt3_s = Match('(', nt3_i);
					var nt4_s = eliste(nt4_i);
					TokenAST nt5_s = Match(')', nt5_i);
					nt6_i = new ExprNew(nt1_s, nt2_s, nt4_s); 
					var nt6_s = tmp_15(nt6_i);
					e_prim_s = nt6_s; 
				}
				break;
			case 6:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					
					TokenAST nt1_s = Match(CAST, nt1_i);
					TokenAST nt2_s = Match('(', nt2_i);
					var nt3_s = type(nt3_i);
					TokenAST nt4_s = Match(',', nt4_i);
					var nt5_s = e(nt5_i);
					TokenAST nt6_s = Match(')', nt6_i);
					nt7_i = new ExprCast(nt1_s, nt3_s, nt5_s); 
					var nt7_s = tmp_15(nt7_i);
					e_prim_s = nt7_s; 
				}
				break;
			case 7:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = lambda(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_15(nt2_i);
					e_prim_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '*':
			case '/':
			case '%':
			case '+':
			case '-':
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_prim_s;
		}
		
		ExprRoot tmp_15(IAST tmp_15_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '*':
			case '/':
			case '%':
			case '+':
			case '-':
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
				alt = 0;
				break;
			case '(':
				alt = 1;
				break;
			case '[':
				alt = 2;
				break;
			case INC:
				alt = 3;
				break;
			case DEC:
				alt = 4;
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
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match('(', nt1_i);
					var nt2_s = eliste(nt2_i);
					TokenAST nt3_s = Match(')', nt3_i);
					nt4_i = new ExprCall(((ExprRoot)tmp_15_i), nt1_s, nt2_s); 
					var nt4_s = tmp_15(nt4_i);
					tmp_15_s = nt4_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match('[', nt1_i);
					var nt2_s = elist(nt2_i);
					TokenAST nt3_s = Match(']', nt3_i);
					nt4_i = new ExprArray(((ExprRoot)tmp_15_i), nt1_s, nt2_s); 
					var nt4_s = tmp_15(nt4_i);
					tmp_15_s = nt4_s; 
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(INC, nt1_i);
					nt2_i = new ExprPostInc(((ExprRoot)tmp_15_i), nt1_s); 
					var nt2_s = tmp_15(nt2_i);
					tmp_15_s = nt2_s; 
				}
				break;
			case 4:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(DEC, nt1_i);
					nt2_i = new ExprPostDec(((ExprRoot)tmp_15_i), nt1_s); 
					var nt2_s = tmp_15(nt2_i);
					tmp_15_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '*':
			case '/':
			case '%':
			case '+':
			case '-':
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_15_s;
		}
		
		IAST lambda(IAST lambda_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case FUN:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			IAST lambda_s = default(IAST);
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
					
					TokenAST nt1_s = Match(FUN, nt1_i);
					TokenAST nt2_s = Match('(', nt2_i);
					var nt3_s = lambda_args_e(nt3_i);
					TokenAST nt4_s = Match(')', nt4_i);
					var nt5_s = lambda_type_e(nt5_i);
					var nt6_s = tmp_24(nt1_s, nt2_s, nt3_s, nt4_s, nt5_s, nt6_i);
					lambda_s = nt6_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case '(':
			case '[':
			case INC:
			case DEC:
			case '*':
			case '/':
			case '%':
			case '+':
			case '-':
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return lambda_s;
		}
		
		IAST tmp_24(IAST nt1_s, IAST nt2_s, IAST nt3_s, IAST nt4_s, IAST nt5_s, IAST tmp_24_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '(':
				alt = 0;
				break;
			case '{':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			IAST tmp_24_s = default(IAST);
			switch (alt)
			{
			case 0:
				{
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					var nt8_i = default(IAST);
					
					TokenAST nt6_s = Match('(', nt6_i);
					var nt7_s = e(nt7_i);
					TokenAST nt8_s = Match(')', nt8_i);
				}
				break;
			case 1:
				{
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					var nt8_i = default(IAST);
					
					TokenAST nt6_s = Match('{', nt6_i);
					var nt7_s = stmtlist(nt7_i);
					TokenAST nt8_s = Match('}', nt8_i);
				}
				break;
			}
			
			switch (Next.token)
			{
			case '(':
			case '[':
			case INC:
			case DEC:
			case '*':
			case '/':
			case '%':
			case '+':
			case '-':
			case LSH:
			case RSH:
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
			case ')':
			case ',':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_24_s;
		}
		
		IAST lambda_args_e(IAST lambda_args_e_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ID:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			IAST lambda_args_e_s = default(IAST);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = lambda_args(nt1_i);
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
			return lambda_args_e_s;
		}
		
		IAST lambda_args(IAST lambda_args_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ID:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			IAST lambda_args_s = default(IAST);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = lambda_arg(nt1_i);
					var nt2_s = tmp_16(nt2_i);
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
			return lambda_args_s;
		}
		
		IAST tmp_16(IAST tmp_16_i)
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
			
			IAST tmp_16_s = default(IAST);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_16_s = (IAST)tmp_16_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(',', nt1_i);
					var nt2_s = lambda_arg(nt2_i);
					var nt3_s = tmp_16(nt3_i);
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
			return tmp_16_s;
		}
		
		IAST lambda_arg(IAST lambda_arg_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ID:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			IAST lambda_arg_s = default(IAST);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(ID, nt1_i);
					var nt2_s = tmp_25(nt1_s, nt2_i);
					lambda_arg_s = nt2_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case ',':
			case ')':
				break;
			default:
				Error();
				break;
			}
			return lambda_arg_s;
		}
		
		IAST tmp_25(IAST nt1_s, IAST tmp_25_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ',':
			case ')':
				alt = 0;
				break;
			case ':':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			IAST tmp_25_s = default(IAST);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt2_s = Match(':', nt2_i);
					var nt3_s = type(nt3_i);
				}
				break;
			}
			
			switch (Next.token)
			{
			case ',':
			case ')':
				break;
			default:
				Error();
				break;
			}
			return tmp_25_s;
		}
		
		IAST lambda_type_e(IAST lambda_type_e_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '(':
			case '{':
				alt = 0;
				break;
			case ':':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			IAST lambda_type_e_s = default(IAST);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(':', nt1_i);
					var nt2_s = type(nt2_i);
				}
				break;
			}
			
			switch (Next.token)
			{
			case '(':
			case '{':
				break;
			default:
				Error();
				break;
			}
			return lambda_type_e_s;
		}
		
		ExprList eliste(IAST eliste_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ')':
				alt = 0;
				break;
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprList eliste_s = default(ExprList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					eliste_s = new ExprList();
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = elist(nt1_i);
					eliste_s = nt1_s;
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
			return eliste_s;
		}
		
		ExprList elist(IAST elist_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			ExprList elist_s = default(ExprList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = e(nt1_i);
					nt2_i = new ExprList(nt1_s); 
					var nt2_s = tmp_17(nt2_i);
					elist_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case ']':
			case ')':
				break;
			default:
				Error();
				break;
			}
			return elist_s;
		}
		
		ExprList tmp_17(IAST tmp_17_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ']':
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
			
			ExprList tmp_17_s = default(ExprList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_17_s = (ExprList)tmp_17_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(',', nt1_i);
					var nt2_s = e(nt2_i);
					nt3_i = ((ExprList)tmp_17_i).Add(nt2_s); 
					var nt3_s = tmp_17(nt3_i);
					tmp_17_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case ']':
			case ')':
				break;
			default:
				Error();
				break;
			}
			return tmp_17_s;
		}
		
		ExprRoot e_ass(IAST e_ass_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ID:
			case '(':
				alt = 0;
				break;
			case INC:
				alt = 1;
				break;
			case DEC:
				alt = 2;
				break;
			case NEW:
				alt = 3;
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
					
					var nt1_s = e_prim2(nt1_i);
					var nt2_s = tmp_26(nt1_s, nt2_i);
					e_ass_s = nt2_s;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(INC, nt1_i);
					var nt2_s = e_prim2(nt2_i);
					e_ass_s = new ExprPreInc(nt1_s, nt2_s);
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(DEC, nt1_i);
					var nt2_s = e_prim2(nt2_i);
					e_ass_s = new ExprPreDec(nt1_s, nt2_s);
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(NEW, nt1_i);
					var nt2_s = type(nt2_i);
					TokenAST nt3_s = Match('(', nt3_i);
					var nt4_s = eliste(nt4_i);
					TokenAST nt5_s = Match(')', nt5_i);
					e_ass_s = new ExprNew(nt1_s, nt2_s, nt4_s);
				}
				break;
			}
			
			switch (Next.token)
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
		
		ExprRoot tmp_26(IAST nt1_s, IAST tmp_26_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case INC:
				alt = 0;
				break;
			case DEC:
				alt = 1;
				break;
			case '=':
				alt = 2;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_26_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					
					TokenAST nt2_s = Match(INC, nt2_i);
					tmp_26_s = new ExprPostInc(((ExprRoot)nt1_s), nt2_s);
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					
					TokenAST nt2_s = Match(DEC, nt2_i);
					tmp_26_s = new ExprPostDec(((ExprRoot)nt1_s), nt2_s);
				}
				break;
			case 2:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt2_s = Match('=', nt2_i);
					var nt3_s = e(nt3_i);
					tmp_26_s = new ExprAss(((ExprRoot)nt1_s), nt2_s, nt3_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_26_s;
		}
		
		ExprRoot e_prim2(IAST e_prim2_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ID:
				alt = 0;
				break;
			case '(':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot e_prim2_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = var(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_18(nt2_i);
					e_prim2_s = nt2_s; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match('(', nt1_i);
					var nt2_s = e(nt2_i);
					TokenAST nt3_s = Match(')', nt3_i);
					nt4_i = nt2_s; 
					var nt4_s = tmp_18(nt4_i);
					e_prim2_s = nt4_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case INC:
			case DEC:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_prim2_s;
		}
		
		ExprRoot tmp_18(IAST tmp_18_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case INC:
			case DEC:
			case '=':
			case ')':
			case ';':
				alt = 0;
				break;
			case '[':
				alt = 1;
				break;
			case '(':
				alt = 2;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_18_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_18_s = (ExprRoot)tmp_18_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match('[', nt1_i);
					var nt2_s = elist(nt2_i);
					TokenAST nt3_s = Match(']', nt3_i);
					nt4_i = new ExprArray(((ExprRoot)tmp_18_i), nt1_s, nt2_s); 
					var nt4_s = tmp_18(nt4_i);
					tmp_18_s = nt4_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match('(', nt1_i);
					var nt2_s = eliste(nt2_i);
					TokenAST nt3_s = Match(')', nt3_i);
					nt4_i = new ExprCall(((ExprRoot)tmp_18_i), nt1_s, nt2_s); 
					var nt4_s = tmp_18(nt4_i);
					tmp_18_s = nt4_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case INC:
			case DEC:
			case '=':
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_18_s;
		}
		
		ExprRoot e_opz(IAST e_opz_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ';':
				alt = 0;
				break;
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot e_opz_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					e_opz_s = null;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = e(nt1_i);
					e_opz_s = nt1_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_opz_s;
		}
		
		ExprRoot e_ass_opz(IAST e_ass_opz_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ')':
			case ';':
				alt = 0;
				break;
			case INC:
			case DEC:
			case NEW:
			case ID:
			case '(':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot e_ass_opz_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					e_ass_opz_s = null;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = e_ass(nt1_i);
					e_ass_opz_s = nt1_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case ')':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return e_ass_opz_s;
		}
		
		StmtRoot stmt(IAST stmt_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
				alt = 0;
				break;
			case ';':
				alt = 1;
				break;
			case IF:
				alt = 2;
				break;
			case WHILE:
				alt = 3;
				break;
			case FOR:
				alt = 4;
				break;
			case '{':
				alt = 5;
				break;
			case RETURN:
				alt = 6;
				break;
			case VAR:
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
					
					var nt1_s = e(nt1_i);
					TokenAST nt2_s = Match(';', nt2_i);
					stmt_s = new StmtExpr(nt1_s);
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(';', nt1_i);
					stmt_s = new StmtNull(nt1_s);
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					
					TokenAST nt1_s = Match(IF, nt1_i);
					TokenAST nt2_s = Match('(', nt2_i);
					var nt3_s = e(nt3_i);
					TokenAST nt4_s = Match(')', nt4_i);
					var nt5_s = stmt(nt5_i);
					var nt6_s = tmp_27(nt1_s, nt2_s, nt3_s, nt4_s, nt5_s, nt6_i);
					stmt_s = nt6_s;
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(WHILE, nt1_i);
					TokenAST nt2_s = Match('(', nt2_i);
					var nt3_s = e(nt3_i);
					TokenAST nt4_s = Match(')', nt4_i);
					var nt5_s = stmt(nt5_i);
					stmt_s = new StmtWhile(nt1_s, nt3_s, nt5_s);
				}
				break;
			case 4:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(FOR, nt1_i);
					TokenAST nt2_s = Match('(', nt2_i);
					var nt3_s = tmp_28(nt1_s, nt2_s, nt3_i);
					stmt_s = nt3_s;
				}
				break;
			case 5:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('{', nt1_i);
					var nt2_s = stmtlist(nt2_i);
					TokenAST nt3_s = Match('}', nt3_i);
					stmt_s = nt2_s;
				}
				break;
			case 6:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(RETURN, nt1_i);
					var nt2_s = tmp_29(nt1_s, nt2_i);
					stmt_s = nt2_s;
				}
				break;
			case 7:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = defv(nt1_i);
					TokenAST nt2_s = Match(';', nt2_i);
					stmt_s = nt1_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case ELSE:
			case ';':
			case IF:
			case WHILE:
			case FOR:
			case '{':
			case RETURN:
			case VAR:
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return stmt_s;
		}
		
		StmtRoot tmp_27(IAST nt1_s, IAST nt2_s, IAST nt3_s, IAST nt4_s, IAST nt5_s, IAST tmp_27_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ';':
			case IF:
			case WHILE:
			case FOR:
			case '{':
			case RETURN:
			case VAR:
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
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
			
			StmtRoot tmp_27_s = default(StmtRoot);
			switch (alt)
			{
			case 0:
				{
					var nt6_i = default(IAST);
					
					tmp_27_s = new StmtIf(((TokenAST)nt1_s), ((ExprRoot)nt3_s), ((StmtRoot)nt5_s));
				}
				break;
			case 1:
				{
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					
					TokenAST nt6_s = Match(ELSE, nt6_i);
					var nt7_s = stmt(nt7_i);
					tmp_27_s = new StmtIf(((TokenAST)nt1_s), ((ExprRoot)nt3_s), ((StmtRoot)nt5_s), nt7_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case ELSE:
			case ';':
			case IF:
			case WHILE:
			case FOR:
			case '{':
			case RETURN:
			case VAR:
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_27_s;
		}
		
		StmtRoot tmp_28(IAST nt1_s, IAST nt2_s, IAST tmp_28_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case INC:
			case DEC:
			case NEW:
			case ID:
			case '(':
			case ';':
				alt = 0;
				break;
			case VAR:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			StmtRoot tmp_28_s = default(StmtRoot);
			switch (alt)
			{
			case 0:
				{
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					var nt8_i = default(IAST);
					var nt9_i = default(IAST);
					
					var nt3_s = e_ass_opz(nt3_i);
					TokenAST nt4_s = Match(';', nt4_i);
					var nt5_s = e_opz(nt5_i);
					TokenAST nt6_s = Match(';', nt6_i);
					var nt7_s = e_ass_opz(nt7_i);
					TokenAST nt8_s = Match(')', nt8_i);
					var nt9_s = stmt(nt9_i);
					tmp_28_s = new StmtFor(((TokenAST)nt1_s), nt3_s, nt5_s, nt7_s, nt9_s);
				}
				break;
			case 1:
				{
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					var nt8_i = default(IAST);
					var nt9_i = default(IAST);
					
					var nt3_s = defv(nt3_i);
					TokenAST nt4_s = Match(';', nt4_i);
					var nt5_s = e_opz(nt5_i);
					TokenAST nt6_s = Match(';', nt6_i);
					var nt7_s = e_ass_opz(nt7_i);
					TokenAST nt8_s = Match(')', nt8_i);
					var nt9_s = stmt(nt9_i);
					tmp_28_s = new StmtFor(((TokenAST)nt1_s), nt3_s, nt5_s, nt7_s, nt9_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case ELSE:
			case ';':
			case IF:
			case WHILE:
			case FOR:
			case '{':
			case RETURN:
			case VAR:
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_28_s;
		}
		
		StmtRoot tmp_29(IAST nt1_s, IAST tmp_29_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
				alt = 0;
				break;
			case ';':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			StmtRoot tmp_29_s = default(StmtRoot);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					var nt2_s = e(nt2_i);
					TokenAST nt3_s = Match(';', nt3_i);
					tmp_29_s = new StmtReturn(((TokenAST)nt1_s), nt2_s);
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					
					TokenAST nt2_s = Match(';', nt2_i);
					tmp_29_s = new StmtReturn(((TokenAST)nt1_s));
				}
				break;
			}
			
			switch (Next.token)
			{
			case ELSE:
			case ';':
			case IF:
			case WHILE:
			case FOR:
			case '{':
			case RETURN:
			case VAR:
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_29_s;
		}
		
		StmtDef defv(IAST defv_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case VAR:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			StmtDef defv_s = default(StmtDef);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(VAR, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					var nt3_s = tmp_30(nt1_s, nt2_s, nt3_i);
					defv_s = nt3_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case ';':
				break;
			default:
				Error();
				break;
			}
			return defv_s;
		}
		
		StmtDef tmp_30(IAST nt1_s, IAST nt2_s, IAST tmp_30_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ':':
				alt = 0;
				break;
			case '=':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			StmtDef tmp_30_s = default(StmtDef);
			switch (alt)
			{
			case 0:
				{
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt3_s = Match(':', nt3_i);
					var nt4_s = type(nt4_i);
					var nt5_s = tmp_32(nt1_s, nt2_s, nt3_s, nt4_s, nt5_i);
					tmp_30_s = nt5_s;
				}
				break;
			case 1:
				{
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt3_s = Match('=', nt3_i);
					var nt4_s = e(nt4_i);
					tmp_30_s = new StmtDef(((TokenAST)nt1_s), ((TokenAST)nt2_s), nt4_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_30_s;
		}
		
		StmtDef tmp_32(IAST nt1_s, IAST nt2_s, IAST nt3_s, IAST nt4_s, IAST tmp_32_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ';':
				alt = 0;
				break;
			case '=':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			StmtDef tmp_32_s = default(StmtDef);
			switch (alt)
			{
			case 0:
				{
					var nt5_i = default(IAST);
					
					tmp_32_s = new StmtDef(((TokenAST)nt1_s), ((TokenAST)nt2_s), ((TypeRoot)nt4_s));
				}
				break;
			case 1:
				{
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					
					TokenAST nt5_s = Match('=', nt5_i);
					var nt6_s = e(nt6_i);
					tmp_32_s = new StmtDef(((TokenAST)nt1_s), ((TokenAST)nt2_s), ((TypeRoot)nt4_s), nt6_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_32_s;
		}
		
		StmtBlock stmtlist(IAST stmtlist_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '}':
				alt = 0;
				break;
			case ';':
			case IF:
			case WHILE:
			case FOR:
			case '{':
			case RETURN:
			case VAR:
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			StmtBlock stmtlist_s = default(StmtBlock);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					stmtlist_s = new StmtBlock();
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = tmp_1(nt1_i);
					stmtlist_s = nt1_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case '}':
				break;
			default:
				Error();
				break;
			}
			return stmtlist_s;
		}
		
		StmtBlock tmp_1(IAST tmp_1_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ';':
			case IF:
			case WHILE:
			case FOR:
			case '{':
			case RETURN:
			case VAR:
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			StmtBlock tmp_1_s = default(StmtBlock);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = stmt(nt1_i);
					nt2_i = (new StmtBlock()).Add(nt1_s); 
					var nt2_s = tmp_19(nt2_i);
					tmp_1_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_1_s;
		}
		
		StmtBlock tmp_19(IAST tmp_19_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '}':
				alt = 0;
				break;
			case ';':
			case IF:
			case WHILE:
			case FOR:
			case '{':
			case RETURN:
			case VAR:
			case '-':
			case '+':
			case INC:
			case DEC:
			case NUM:
			case TRUE:
			case FALSE:
			case ID:
			case '(':
			case NEW:
			case CAST:
			case FUN:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			StmtBlock tmp_19_s = default(StmtBlock);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_19_s = (StmtBlock)tmp_19_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = stmt(nt1_i);
					nt2_i = ((StmtBlock)tmp_19_i).Add(nt1_s); 
					var nt2_s = tmp_19(nt2_i);
					tmp_19_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_19_s;
		}
		
		TypeDef typeId(IAST typeId_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ID:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			TypeDef typeId_s = default(TypeDef);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(ID, nt1_i);
					nt2_i = new TypeDef(nt1_s); 
					var nt2_s = tmp_20(nt2_i);
					typeId_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '[':
			case '*':
			case '{':
			case '(':
			case ';':
			case '=':
			case ',':
			case ')':
				break;
			default:
				Error();
				break;
			}
			return typeId_s;
		}
		
		TypeDef tmp_20(IAST tmp_20_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '[':
			case '*':
			case '{':
			case '(':
			case ';':
			case '=':
			case ',':
			case ')':
				alt = 0;
				break;
			case '.':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			TypeDef tmp_20_s = default(TypeDef);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_20_s = (TypeDef)tmp_20_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('.', nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					nt3_i = ((TypeDef)tmp_20_i).Add(nt2_s); 
					var nt3_s = tmp_20(nt3_i);
					tmp_20_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '[':
			case '*':
			case '{':
			case '(':
			case ';':
			case '=':
			case ',':
			case ')':
				break;
			default:
				Error();
				break;
			}
			return tmp_20_s;
		}
		
		TypeRoot type(IAST type_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ID:
				alt = 0;
				break;
			case INT:
				alt = 1;
				break;
			case VOID:
				alt = 2;
				break;
			case BOOL:
				alt = 3;
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
					
					var nt1_s = typeId(nt1_i);
					nt2_i = (nt1_s); 
					var nt2_s = tmp_21(nt2_i);
					type_s = nt2_s; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(INT, nt1_i);
					nt2_i = new TypeNat(nt1_s); 
					var nt2_s = tmp_21(nt2_i);
					type_s = nt2_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(VOID, nt1_i);
					nt2_i = new TypeNat(nt1_s); 
					var nt2_s = tmp_21(nt2_i);
					type_s = nt2_s; 
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(BOOL, nt1_i);
					nt2_i = new TypeNat(nt1_s); 
					var nt2_s = tmp_21(nt2_i);
					type_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '{':
			case '(':
			case ';':
			case '=':
			case ',':
			case ')':
				break;
			default:
				Error();
				break;
			}
			return type_s;
		}
		
		TypeRoot tmp_21(IAST tmp_21_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '{':
			case '(':
			case ';':
			case '=':
			case ',':
			case ')':
				alt = 0;
				break;
			case '[':
				alt = 1;
				break;
			case '*':
				alt = 2;
				break;
			default:
				Error();
				break;
			}
			
			TypeRoot tmp_21_s = default(TypeRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_21_s = (TypeRoot)tmp_21_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('[', nt1_i);
					TokenAST nt2_s = Match(']', nt2_i);
					nt3_i = new TypeArray(((TypeRoot)tmp_21_i)); 
					var nt3_s = tmp_21(nt3_i);
					tmp_21_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match('*', nt1_i);
					nt2_i = new TypePt(((TypeRoot)tmp_21_i)); 
					var nt2_s = tmp_21(nt2_i);
					tmp_21_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '{':
			case '(':
			case ';':
			case '=':
			case ',':
			case ')':
				break;
			default:
				Error();
				break;
			}
			return tmp_21_s;
		}
		
		protected override RegAcceptList CreateRegAcceptList()
		{
			var acts = new RegAcceptList();
			acts.Add(new RegAnd(new RegAnd(new RegToken('f'), new RegToken('u')), new RegToken('n')), FUN);
			acts.Add(new RegAnd(new RegAnd(new RegToken('v'), new RegToken('a')), new RegToken('r')), VAR);
			acts.Add(new RegAnd(new RegAnd(new RegToken('d'), new RegToken('e')), new RegToken('l')), DEL);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('c'), new RegToken('l')), new RegToken('a')), new RegToken('s')), new RegToken('s')), CLASS);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('p'), new RegToken('u')), new RegToken('b')), new RegToken('l')), new RegToken('i')), new RegToken('c')), PUBLIC);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('p'), new RegToken('r')), new RegToken('i')), new RegToken('v')), new RegToken('a')), new RegToken('t')), new RegToken('e')), PRIVATE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('p'), new RegToken('r')), new RegToken('o')), new RegToken('t')), new RegToken('e')), new RegToken('c')), new RegToken('t')), new RegToken('e')), new RegToken('d')), PROTECTED);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('e'), new RegToken('x')), new RegToken('t')), new RegToken('e')), new RegToken('r')), new RegToken('n')), EXTERN);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('n'), new RegToken('a')), new RegToken('m')), new RegToken('e')), new RegToken('s')), new RegToken('p')), new RegToken('a')), new RegToken('c')), new RegToken('e')), NAMESPACE);
			acts.Add(new RegAnd(new RegToken('i'), new RegToken('f')), IF);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('w'), new RegToken('h')), new RegToken('i')), new RegToken('l')), new RegToken('e')), WHILE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('e'), new RegToken('l')), new RegToken('s')), new RegToken('e')), ELSE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('r'), new RegToken('e')), new RegToken('t')), new RegToken('u')), new RegToken('r')), new RegToken('n')), RETURN);
			acts.Add(new RegAnd(new RegAnd(new RegToken('f'), new RegToken('o')), new RegToken('r')), FOR);
			acts.Add(new RegAnd(new RegAnd(new RegToken('i'), new RegToken('n')), new RegToken('t')), INT);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('v'), new RegToken('o')), new RegToken('i')), new RegToken('d')), VOID);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('b'), new RegToken('o')), new RegToken('o')), new RegToken('l')), BOOL);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('t'), new RegToken('r')), new RegToken('u')), new RegToken('e')), TRUE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('f'), new RegToken('a')), new RegToken('l')), new RegToken('s')), new RegToken('e')), FALSE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('c'), new RegToken('a')), new RegToken('s')), new RegToken('t')), CAST);
			acts.Add(new RegAnd(new RegToken('&'), new RegToken('&')), ANDAND);
			acts.Add(new RegAnd(new RegToken('|'), new RegToken('|')), OROR);
			acts.Add(new RegAnd(new RegToken('+'), new RegToken('+')), INC);
			acts.Add(new RegAnd(new RegToken('-'), new RegToken('-')), DEC);
			acts.Add(new RegToken('+'), '+');
			acts.Add(new RegToken('-'), '-');
			acts.Add(new RegToken('='), '=');
			acts.Add(new RegToken('*'), '*');
			acts.Add(new RegToken('/'), '/');
			acts.Add(new RegToken('%'), '%');
			acts.Add(new RegToken(';'), ';');
			acts.Add(new RegToken('('), '(');
			acts.Add(new RegToken(')'), ')');
			acts.Add(new RegToken('{'), '{');
			acts.Add(new RegToken('}'), '}');
			acts.Add(new RegToken('['), '[');
			acts.Add(new RegToken(']'), ']');
			acts.Add(new RegToken(','), ',');
			acts.Add(new RegToken('.'), '.');
			acts.Add(new RegToken(':'), ':');
			acts.Add(new RegToken('>'), GT);
			acts.Add(new RegAnd(new RegToken('>'), new RegToken('=')), GE);
			acts.Add(new RegToken('<'), LT);
			acts.Add(new RegAnd(new RegToken('<'), new RegToken('=')), LE);
			acts.Add(new RegAnd(new RegToken('='), new RegToken('=')), EQ);
			acts.Add(new RegAnd(new RegToken('!'), new RegToken('=')), NE);
			acts.Add(new RegAnd(new RegOr(new RegOr(new RegToken('_'), new RegTokenRange(97, 122)), new RegTokenRange(65, 90)), new RegZeroOrMore(new RegOr(new RegOr(new RegOr(new RegToken('_'), new RegTokenRange(97, 122)), new RegTokenRange(65, 90)), new RegTokenRange(48, 57)))), ID);
			acts.Add(new RegOneOrMore(new RegTokenRange(48, 57)), NUM);
			acts.Add(new RegAnd(new RegAnd(new RegToken('/'), new RegToken('/')), new RegZeroOrMore(new RegTokenOutsideRange(10, 10))));
			acts.Add(new RegAnd(new RegToken('/'), new RegToken('*')), (ref NFA.Token tk, LexReader rd, NFA nfa) => {
				for (; ; )
				{
					if (rd.Peek().ch == -1) throw new Exception("EOF in comment");
					if (rd.Read().ch == '*' && rd.Peek().ch == '/')
					{
						rd.Read();
						break;
					}
				}
				rd.SetMatch();
				rd.EndToken(out tk.strRead, out tk.fileName, out tk.line);
				return false;
			});
			acts.Add(new RegOneOrMore(new RegOr(new RegOr(new RegOr(new RegToken(' '), new RegToken(10)), new RegToken(13)), new RegToken(9))));
			return acts;
		}
	}
}
