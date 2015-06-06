#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.

using System;
using System.Text;
using System.Collections.Generic;
using LLParserLexerLib;

namespace LLParserGenTest
{
	public partial class MParser : ParserBase
	{
		public const int NAMESPACE = -2;
		public const int ID = -3;
		public const int CLASS = -4;
		public const int INTERFACE = -5;
		public const int VAR = -6;
		public const int SET = -7;
		public const int GET = -8;
		public const int FUN = -9;
		public const int NATIVE = -10;
		public const int VIRTUAL = -11;
		public const int BASE = -12;
		public const int THIS = -13;
		public const int IF = -14;
		public const int WHILE = -15;
		public const int BREAK = -16;
		public const int CONTINUE = -17;
		public const int RETURN = -18;
		public const int ELSE = -19;
		public const int OROR = -20;
		public const int ANDAND = -21;
		public const int EQ = -22;
		public const int NE = -23;
		public const int LT = -24;
		public const int LE = -25;
		public const int GT = -26;
		public const int GE = -27;
		public const int SHL = -28;
		public const int SHR = -29;
		public const int NUM = -30;
		public const int STR = -31;
		public const int CHR = -32;
		public const int FALSE = -33;
		public const int TRUE = -34;
		public const int NULL = -35;
		public const int CAST = -36;
		public const int NEW = -37;
		
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
					_token.Add(-4, "CLASS");
					_token.Add(-5, "INTERFACE");
					_token.Add(-6, "VAR");
					_token.Add(-7, "SET");
					_token.Add(-8, "GET");
					_token.Add(-9, "FUN");
					_token.Add(-10, "NATIVE");
					_token.Add(-11, "VIRTUAL");
					_token.Add(-12, "BASE");
					_token.Add(-13, "THIS");
					_token.Add(-14, "IF");
					_token.Add(-15, "WHILE");
					_token.Add(-16, "BREAK");
					_token.Add(-17, "CONTINUE");
					_token.Add(-18, "RETURN");
					_token.Add(-19, "ELSE");
					_token.Add(-20, "OROR");
					_token.Add(-21, "ANDAND");
					_token.Add(-22, "EQ");
					_token.Add(-23, "NE");
					_token.Add(-24, "LT");
					_token.Add(-25, "LE");
					_token.Add(-26, "GT");
					_token.Add(-27, "GE");
					_token.Add(-28, "SHL");
					_token.Add(-29, "SHR");
					_token.Add(-30, "NUM");
					_token.Add(-31, "STR");
					_token.Add(-32, "CHR");
					_token.Add(-33, "FALSE");
					_token.Add(-34, "TRUE");
					_token.Add(-35, "NULL");
					_token.Add(-36, "CAST");
					_token.Add(-37, "NEW");
				}
				return _token;
			}
		}
		
		DeclList start(IAST start_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case CLASS:
			case INTERFACE:
			case FUN:
			case VAR:
			case NAMESPACE:
				alt = 0;
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
					
					var nt1_s = decl_list_e(nt1_i);
					start_s = nt1_s;
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
			return start_s;
		}
		
		DeclList decl_list_e(IAST decl_list_e_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '}':
			case -1:
				alt = 0;
				break;
			case CLASS:
			case INTERFACE:
			case FUN:
			case VAR:
			case NAMESPACE:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclList decl_list_e_s = default(DeclList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					decl_list_e_s = new DeclList();
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = decl_list(nt1_i);
					decl_list_e_s = nt1_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case '}':
			case -1:
				break;
			default:
				Error();
				break;
			}
			return decl_list_e_s;
		}
		
		DeclList decl_list(IAST decl_list_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case CLASS:
			case INTERFACE:
			case FUN:
			case VAR:
			case NAMESPACE:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			DeclList decl_list_s = default(DeclList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = decl(nt1_i);
					nt2_i = new DeclList().Add(nt1_s); 
					var nt2_s = tmp_1(nt2_i);
					decl_list_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '}':
			case -1:
				break;
			default:
				Error();
				break;
			}
			return decl_list_s;
		}
		
		DeclList tmp_1(IAST tmp_1_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '}':
			case -1:
				alt = 0;
				break;
			case CLASS:
			case INTERFACE:
			case FUN:
			case VAR:
			case NAMESPACE:
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
					
					tmp_1_s = (DeclList)tmp_1_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = decl(nt1_i);
					nt2_i = ((DeclList)tmp_1_i).Add(nt1_s); 
					var nt2_s = tmp_1(nt2_i);
					tmp_1_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '}':
			case -1:
				break;
			default:
				Error();
				break;
			}
			return tmp_1_s;
		}
		
		DeclRoot decl(IAST decl_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case CLASS:
			case INTERFACE:
				alt = 0;
				break;
			case FUN:
				alt = 1;
				break;
			case VAR:
				alt = 2;
				break;
			case NAMESPACE:
				alt = 3;
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
					
					var nt1_s = decl_cls(nt1_i);
					decl_s = nt1_s;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = decl_fun(nt1_i);
					decl_s = nt1_s;
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = decl_var(nt1_i);
					decl_s = nt1_s;
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = decl_namespace(nt1_i);
					decl_s = nt1_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case CLASS:
			case INTERFACE:
			case FUN:
			case VAR:
			case NAMESPACE:
			case '}':
			case -1:
				break;
			default:
				Error();
				break;
			}
			return decl_s;
		}
		
		DeclNamespace decl_namespace(IAST decl_namespace_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case NAMESPACE:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			DeclNamespace decl_namespace_s = default(DeclNamespace);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(NAMESPACE, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match('{', nt3_i);
					var nt4_s = decl_list_e(nt4_i);
					TokenAST nt5_s = Match('}', nt5_i);
					decl_namespace_s = new DeclNamespace(nt2_s, nt4_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case CLASS:
			case INTERFACE:
			case FUN:
			case VAR:
			case NAMESPACE:
			case '}':
			case -1:
				break;
			default:
				Error();
				break;
			}
			return decl_namespace_s;
		}
		
		DeclRoot decl_cls(IAST decl_cls_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case CLASS:
				alt = 0;
				break;
			case INTERFACE:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclRoot decl_cls_s = default(DeclRoot);
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
					
					TokenAST nt1_s = Match(CLASS, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					var nt3_s = base_opz(nt3_i);
					TokenAST nt4_s = Match('{', nt4_i);
					var nt5_s = cls_members_e(nt5_i);
					TokenAST nt6_s = Match('}', nt6_i);
					decl_cls_s = new DeclClass(nt2_s, nt3_s, nt5_s);
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
					
					TokenAST nt1_s = Match(INTERFACE, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					var nt3_s = base_opz(nt3_i);
					TokenAST nt4_s = Match('{', nt4_i);
					var nt5_s = cls_members_e(nt5_i);
					TokenAST nt6_s = Match('}', nt6_i);
					decl_cls_s = new DeclInterface(nt2_s, nt3_s, nt5_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case CLASS:
			case INTERFACE:
			case FUN:
			case VAR:
			case NAMESPACE:
			case '}':
			case -1:
				break;
			default:
				Error();
				break;
			}
			return decl_cls_s;
		}
		
		RefTypeRootList base_opz(IAST base_opz_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ':':
				alt = 0;
				break;
			case '{':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			RefTypeRootList base_opz_s = default(RefTypeRootList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(':', nt1_i);
					var nt2_s = baselist(nt2_i);
					base_opz_s = nt2_s;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					base_opz_s = new RefTypeRootList();;
				}
				break;
			}
			
			switch (Next.token)
			{
			case '{':
				break;
			default:
				Error();
				break;
			}
			return base_opz_s;
		}
		
		RefTypeRootList baselist(IAST baselist_i)
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
			
			RefTypeRootList baselist_s = default(RefTypeRootList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = type_dot(nt1_i);
					nt2_i = new RefTypeRootList().Add(nt1_s); 
					var nt2_s = tmp_2(nt2_i);
					baselist_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '{':
				break;
			default:
				Error();
				break;
			}
			return baselist_s;
		}
		
		RefTypeRootList tmp_2(IAST tmp_2_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '{':
				alt = 0;
				break;
			case ',':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			RefTypeRootList tmp_2_s = default(RefTypeRootList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_2_s = (RefTypeRootList)tmp_2_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(',', nt1_i);
					var nt2_s = type_dot(nt2_i);
					nt3_i = ((RefTypeRootList)tmp_2_i).Add(nt2_s); 
					var nt3_s = tmp_2(nt3_i);
					tmp_2_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case '{':
				break;
			default:
				Error();
				break;
			}
			return tmp_2_s;
		}
		
		DeclList cls_members_e(IAST cls_members_e_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '}':
				alt = 0;
				break;
			case CLASS:
			case INTERFACE:
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
			
			switch (Next.token)
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
			switch (Next.token)
			{
			case CLASS:
			case INTERFACE:
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
			
			switch (Next.token)
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
			switch (Next.token)
			{
			case '}':
				alt = 0;
				break;
			case CLASS:
			case INTERFACE:
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
			
			switch (Next.token)
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
			switch (Next.token)
			{
			case FUN:
				alt = 0;
				break;
			case VAR:
				alt = 1;
				break;
			case CLASS:
			case INTERFACE:
				alt = 2;
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
			case 2:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = decl_cls(nt1_i);
					cls_member_s = nt1_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case CLASS:
			case INTERFACE:
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
		
		DeclRoot decl_var(IAST decl_var_i)
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
			
			DeclRoot decl_var_s = default(DeclRoot);
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
					var nt5_s = tmp_22(nt1_s, nt2_s, nt3_s, nt4_s, nt5_i);
					decl_var_s = nt5_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case CLASS:
			case INTERFACE:
			case FUN:
			case VAR:
			case NAMESPACE:
			case '}':
			case -1:
				break;
			default:
				Error();
				break;
			}
			return decl_var_s;
		}
		
		DeclRoot tmp_22(IAST nt1_s, IAST nt2_s, IAST nt3_s, IAST nt4_s, IAST tmp_22_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ';':
				alt = 0;
				break;
			case '{':
				alt = 1;
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
					var nt5_i = default(IAST);
					
					TokenAST nt5_s = Match(';', nt5_i);
					tmp_22_s = new DeclVar(((TokenAST)nt2_s), ((RefTypeRoot)nt4_s));
				}
				break;
			case 1:
				{
					var nt5_i = default(IAST);
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					
					TokenAST nt5_s = Match('{', nt5_i);
					var nt6_s = decl_var_getset_list(nt6_i);
					TokenAST nt7_s = Match('}', nt7_i);
					tmp_22_s = new DeclProp(((TokenAST)nt2_s), ((RefTypeRoot)nt4_s), nt6_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case CLASS:
			case INTERFACE:
			case FUN:
			case VAR:
			case NAMESPACE:
			case '}':
			case -1:
				break;
			default:
				Error();
				break;
			}
			return tmp_22_s;
		}
		
		DeclProp decl_var_getset_list(IAST decl_var_getset_list_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case SET:
			case GET:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			DeclProp decl_var_getset_list_s = default(DeclProp);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = decl_var_getset(nt1_i);
					var nt2_s = tmp_23(nt1_s, nt2_i);
					decl_var_getset_list_s = nt2_s;
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
			return decl_var_getset_list_s;
		}
		
		DeclProp tmp_23(IAST nt1_s, IAST tmp_23_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '}':
				alt = 0;
				break;
			case SET:
			case GET:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclProp tmp_23_s = default(DeclProp);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					
					tmp_23_s = new DeclProp(null, ((DeclPropGetSet)nt1_s), null);
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					
					var nt2_s = decl_var_getset(nt2_i);
					tmp_23_s = new DeclProp(null, ((DeclPropGetSet)nt1_s), nt2_s);
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
			return tmp_23_s;
		}
		
		DeclPropGetSet decl_var_getset(IAST decl_var_getset_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case SET:
				alt = 0;
				break;
			case GET:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclPropGetSet decl_var_getset_s = default(DeclPropGetSet);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(SET, nt1_i);
					var nt2_s = tmp_24(nt1_s, nt2_i);
					decl_var_getset_s = nt2_s;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(GET, nt1_i);
					var nt2_s = tmp_25(nt1_s, nt2_i);
					decl_var_getset_s = nt2_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case SET:
			case GET:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return decl_var_getset_s;
		}
		
		DeclPropGetSet tmp_24(IAST nt1_s, IAST tmp_24_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '{':
				alt = 0;
				break;
			case ';':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclPropGetSet tmp_24_s = default(DeclPropGetSet);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt2_s = Match('{', nt2_i);
					var nt3_s = stmtliste(nt3_i);
					TokenAST nt4_s = Match('}', nt4_i);
					tmp_24_s = new DeclPropGetSet(true, nt3_s);
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					
					TokenAST nt2_s = Match(';', nt2_i);
					tmp_24_s = new DeclPropGetSet(true, null);
				}
				break;
			}
			
			switch (Next.token)
			{
			case SET:
			case GET:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_24_s;
		}
		
		DeclPropGetSet tmp_25(IAST nt1_s, IAST tmp_25_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '{':
				alt = 0;
				break;
			case ';':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclPropGetSet tmp_25_s = default(DeclPropGetSet);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt2_s = Match('{', nt2_i);
					var nt3_s = stmtliste(nt3_i);
					TokenAST nt4_s = Match('}', nt4_i);
					tmp_25_s = new DeclPropGetSet(false, nt3_s);
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					
					TokenAST nt2_s = Match(';', nt2_i);
					tmp_25_s = new DeclPropGetSet(false, null);
				}
				break;
			}
			
			switch (Next.token)
			{
			case SET:
			case GET:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_25_s;
		}
		
		DeclFun decl_fun(IAST decl_fun_i)
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
					
					TokenAST nt1_s = Match(FUN, nt1_i);
					var nt2_s = decl_fun_mod(nt2_i);
					TokenAST nt3_s = Match(ID, nt3_i);
					TokenAST nt4_s = Match('(', nt4_i);
					var nt5_s = funargse(nt5_i);
					TokenAST nt6_s = Match(')', nt6_i);
					var nt7_s = ret_or_base(nt7_i);
					var nt8_s = tmp_26(nt1_s, nt2_s, nt3_s, nt4_s, nt5_s, nt6_s, nt7_s, nt8_i);
					decl_fun_s = nt8_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case CLASS:
			case INTERFACE:
			case FUN:
			case VAR:
			case NAMESPACE:
			case '}':
			case -1:
				break;
			default:
				Error();
				break;
			}
			return decl_fun_s;
		}
		
		DeclFun tmp_26(IAST nt1_s, IAST nt2_s, IAST nt3_s, IAST nt4_s, IAST nt5_s, IAST nt6_s, IAST nt7_s, IAST tmp_26_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '{':
				alt = 0;
				break;
			case ';':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			DeclFun tmp_26_s = default(DeclFun);
			switch (alt)
			{
			case 0:
				{
					var nt8_i = default(IAST);
					var nt9_i = default(IAST);
					var nt10_i = default(IAST);
					
					TokenAST nt8_s = Match('{', nt8_i);
					var nt9_s = stmtliste(nt9_i);
					TokenAST nt10_s = Match('}', nt10_i);
					tmp_26_s = new DeclFun(((TokenAST)nt2_s), ((TokenAST)nt3_s), ((FunArgList)nt5_s), ((RefTypeRoot_or_Base)nt7_s), nt9_s, nt10_s);
				}
				break;
			case 1:
				{
					var nt8_i = default(IAST);
					
					TokenAST nt8_s = Match(';', nt8_i);
					tmp_26_s = new DeclFun(((TokenAST)nt2_s), ((TokenAST)nt3_s), ((FunArgList)nt5_s), ((RefTypeRoot_or_Base)nt7_s), null, nt8_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case CLASS:
			case INTERFACE:
			case FUN:
			case VAR:
			case NAMESPACE:
			case '}':
			case -1:
				break;
			default:
				Error();
				break;
			}
			return tmp_26_s;
		}
		
		TokenAST decl_fun_mod(IAST decl_fun_mod_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ID:
				alt = 0;
				break;
			case NATIVE:
				alt = 1;
				break;
			case VIRTUAL:
				alt = 2;
				break;
			default:
				Error();
				break;
			}
			
			TokenAST decl_fun_mod_s = default(TokenAST);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					decl_fun_mod_s = null;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(NATIVE, nt1_i);
					decl_fun_mod_s = nt1_s;
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(VIRTUAL, nt1_i);
					decl_fun_mod_s = nt1_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case ID:
				break;
			default:
				Error();
				break;
			}
			return decl_fun_mod_s;
		}
		
		RefTypeRoot_or_Base ret_or_base(IAST ret_or_base_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ':':
				alt = 0;
				break;
			case '{':
			case ';':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			RefTypeRoot_or_Base ret_or_base_s = default(RefTypeRoot_or_Base);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(':', nt1_i);
					var nt2_s = tmp_27(nt1_s, nt2_i);
					ret_or_base_s = nt2_s;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					ret_or_base_s = null;
				}
				break;
			}
			
			switch (Next.token)
			{
			case '{':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return ret_or_base_s;
		}
		
		RefTypeRoot_or_Base tmp_27(IAST nt1_s, IAST tmp_27_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case BASE:
				alt = 0;
				break;
			case THIS:
				alt = 1;
				break;
			case ID:
				alt = 2;
				break;
			default:
				Error();
				break;
			}
			
			RefTypeRoot_or_Base tmp_27_s = default(RefTypeRoot_or_Base);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt2_s = Match(BASE, nt2_i);
					TokenAST nt3_s = Match('(', nt3_i);
					var nt4_s = e_list_opz(nt4_i);
					TokenAST nt5_s = Match(')', nt5_i);
					tmp_27_s = new RefTypeRoot_or_Base(new BaseInit(nt2_s, nt4_s));
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt2_s = Match(THIS, nt2_i);
					TokenAST nt3_s = Match('(', nt3_i);
					var nt4_s = e_list_opz(nt4_i);
					TokenAST nt5_s = Match(')', nt5_i);
					tmp_27_s = new RefTypeRoot_or_Base(new BaseInit(nt2_s, nt4_s));
				}
				break;
			case 2:
				{
					var nt2_i = default(IAST);
					
					var nt2_s = type(nt2_i);
					tmp_27_s = new RefTypeRoot_or_Base(nt2_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case '{':
			case ';':
				break;
			default:
				Error();
				break;
			}
			return tmp_27_s;
		}
		
		FunArgList funargse(IAST funargse_i)
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
			
			switch (Next.token)
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
			switch (Next.token)
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
					nt4_i = new FunArgList().Add(nt1_s, nt3_s); 
					var nt4_s = tmp_4(nt4_i);
					funargs_s = nt4_s; 
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
			return funargs_s;
		}
		
		FunArgList tmp_4(IAST tmp_4_i)
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
			
			switch (Next.token)
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
			switch (Next.token)
			{
			case IF:
				alt = 0;
				break;
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
					var nt6_s = tmp_28(nt1_s, nt2_s, nt3_s, nt4_s, nt5_s, nt6_i);
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
					var nt2_s = tmp_29(nt1_s, nt2_i);
					stmt_s = nt2_s;
				}
				break;
			}
			
			switch (Next.token)
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
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return stmt_s;
		}
		
		StmtRoot tmp_28(IAST nt1_s, IAST nt2_s, IAST nt3_s, IAST nt4_s, IAST nt5_s, IAST tmp_28_i)
		{
			int alt = 0;
			switch (Next.token)
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
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			
			StmtRoot tmp_28_s = default(StmtRoot);
			switch (alt)
			{
			case 0:
				{
					var nt6_i = default(IAST);
					
					tmp_28_s = new StmtBlock(new StmtIf(((TokenAST)nt1_s), ((ExprRoot)nt3_s), new StmtBlock(((StmtRoot)nt5_s))));
				}
				break;
			case 1:
				{
					var nt6_i = default(IAST);
					var nt7_i = default(IAST);
					
					TokenAST nt6_s = Match(ELSE, nt6_i);
					var nt7_s = stmt(nt7_i);
					tmp_28_s = new StmtBlock(new StmtIf(((TokenAST)nt1_s), ((ExprRoot)nt3_s), new StmtBlock(((StmtRoot)nt5_s)), new StmtBlock(nt7_s)));
				}
				break;
			}
			
			switch (Next.token)
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
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			case ';':
				alt = 0;
				break;
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
					
					TokenAST nt2_s = Match(';', nt2_i);
					tmp_29_s = new StmtReturn(((TokenAST)nt1_s));;
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					var nt2_s = expr(nt2_i);
					TokenAST nt3_s = Match(';', nt3_i);
					tmp_29_s = new StmtReturn(((TokenAST)nt1_s), nt2_s);;
				}
				break;
			}
			
			switch (Next.token)
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
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return tmp_29_s;
		}
		
		StmtList stmtliste(IAST stmtliste_i)
		{
			int alt = 0;
			switch (Next.token)
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
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			
			switch (Next.token)
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
			switch (Next.token)
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
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
		
		StmtList tmp_5(IAST tmp_5_i)
		{
			int alt = 0;
			switch (Next.token)
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
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			
			switch (Next.token)
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
			switch (Next.token)
			{
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			
			switch (Next.token)
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
			switch (Next.token)
			{
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
					var nt2_s = tmp_30(nt1_s, nt2_i);
					e_ass_s = nt2_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case ',':
			case ')':
			case ';':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_ass_s;
		}
		
		ExprRoot tmp_30(IAST nt1_s, IAST tmp_30_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '=':
				alt = 0;
				break;
			case ',':
			case ')':
			case ';':
			case ']':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_30_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt2_s = Match('=', nt2_i);
					var nt3_s = e_ass(nt3_i);
					tmp_30_s = new ExprAss(((ExprRoot)nt1_s), nt2_s, nt3_s);
				}
				break;
			case 1:
				{
					var nt2_i = default(IAST);
					
					tmp_30_s = ((ExprRoot)nt1_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case ',':
			case ')':
			case ';':
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_30_s;
		}
		
		ExprRoot e_oror(IAST e_oror_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			
			switch (Next.token)
			{
			case '=':
			case ',':
			case ')':
			case ';':
			case ']':
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
			switch (Next.token)
			{
			case '=':
			case ',':
			case ')':
			case ';':
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
			
			switch (Next.token)
			{
			case '=':
			case ',':
			case ')':
			case ';':
			case ']':
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
			switch (Next.token)
			{
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			
			switch (Next.token)
			{
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
			case ']':
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
			switch (Next.token)
			{
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
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
			
			switch (Next.token)
			{
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
			case ']':
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
			switch (Next.token)
			{
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			
			switch (Next.token)
			{
			case ANDAND:
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
			case ']':
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
			switch (Next.token)
			{
			case ANDAND:
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
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
			
			switch (Next.token)
			{
			case ANDAND:
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
			case ']':
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
			switch (Next.token)
			{
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			
			switch (Next.token)
			{
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
			case ']':
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
			switch (Next.token)
			{
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
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
			
			switch (Next.token)
			{
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
			case ']':
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
			switch (Next.token)
			{
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			
			switch (Next.token)
			{
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
			case ']':
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
			switch (Next.token)
			{
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
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
			
			switch (Next.token)
			{
			case '^':
			case '|':
			case ANDAND:
			case OROR:
			case '=':
			case ',':
			case ')':
			case ';':
			case ']':
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
			switch (Next.token)
			{
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			
			switch (Next.token)
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
			case ']':
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
			switch (Next.token)
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
			
			switch (Next.token)
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
			case ']':
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
			switch (Next.token)
			{
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			case ',':
			case ')':
			case ';':
			case ']':
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
			case ',':
			case ')':
			case ';':
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
			case ',':
			case ')':
			case ';':
			case ']':
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
			switch (Next.token)
			{
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			case ',':
			case ')':
			case ';':
			case ']':
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
			case ',':
			case ')':
			case ';':
			case ']':
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
			case ',':
			case ')':
			case ';':
			case ']':
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
			switch (Next.token)
			{
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			
			switch (Next.token)
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
			case ']':
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
			switch (Next.token)
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
			
			switch (Next.token)
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
			case ']':
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
			switch (Next.token)
			{
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			
			switch (Next.token)
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
			case ']':
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
			switch (Next.token)
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
			
			switch (Next.token)
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
			case ']':
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
			switch (Next.token)
			{
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			
			switch (Next.token)
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
			case NEW:
				alt = 0;
				break;
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
				alt = 1;
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
					
					var nt1_s = e_new(nt1_i);
					e_prim_s = nt1_s;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = e_prim_no_array_creation(nt1_i);
					e_prim_s = nt1_s;
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
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_prim_s;
		}
		
		ExprRoot e_prim_no_array_creation(IAST e_prim_no_array_creation_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case NUM:
				alt = 0;
				break;
			case STR:
				alt = 1;
				break;
			case CHR:
				alt = 2;
				break;
			case ID:
				alt = 3;
				break;
			case THIS:
				alt = 4;
				break;
			case BASE:
				alt = 5;
				break;
			case FALSE:
				alt = 6;
				break;
			case TRUE:
				alt = 7;
				break;
			case NULL:
				alt = 8;
				break;
			case '(':
				alt = 9;
				break;
			case CAST:
				alt = 10;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot e_prim_no_array_creation_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(NUM, nt1_i);
					nt2_i = new ExprNum(nt1_s); 
					var nt2_s = tmp_16(nt2_i);
					e_prim_no_array_creation_s = nt2_s; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(STR, nt1_i);
					nt2_i = new ExprStr(nt1_s); 
					var nt2_s = tmp_16(nt2_i);
					e_prim_no_array_creation_s = nt2_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(CHR, nt1_i);
					nt2_i = new ExprChr(nt1_s); 
					var nt2_s = tmp_16(nt2_i);
					e_prim_no_array_creation_s = nt2_s; 
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(ID, nt1_i);
					nt2_i = new ExprId(nt1_s); 
					var nt2_s = tmp_16(nt2_i);
					e_prim_no_array_creation_s = nt2_s; 
				}
				break;
			case 4:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(THIS, nt1_i);
					nt2_i = new ExprThis(nt1_s); 
					var nt2_s = tmp_16(nt2_i);
					e_prim_no_array_creation_s = nt2_s; 
				}
				break;
			case 5:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(BASE, nt1_i);
					nt2_i = new ExprThis(nt1_s); 
					var nt2_s = tmp_16(nt2_i);
					e_prim_no_array_creation_s = nt2_s; 
				}
				break;
			case 6:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(FALSE, nt1_i);
					nt2_i = new ExprBool(nt1_s, false); 
					var nt2_s = tmp_16(nt2_i);
					e_prim_no_array_creation_s = nt2_s; 
				}
				break;
			case 7:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(TRUE, nt1_i);
					nt2_i = new ExprBool(nt1_s, true); 
					var nt2_s = tmp_16(nt2_i);
					e_prim_no_array_creation_s = nt2_s; 
				}
				break;
			case 8:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(NULL, nt1_i);
					nt2_i = new ExprNull(nt1_s); 
					var nt2_s = tmp_16(nt2_i);
					e_prim_no_array_creation_s = nt2_s; 
				}
				break;
			case 9:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match('(', nt1_i);
					var nt2_s = e_add(nt2_i);
					TokenAST nt3_s = Match(')', nt3_i);
					nt4_i = nt2_s; 
					var nt4_s = tmp_16(nt4_i);
					e_prim_no_array_creation_s = nt4_s; 
				}
				break;
			case 10:
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
					var nt5_s = expr(nt5_i);
					TokenAST nt6_s = Match(')', nt6_i);
					nt7_i = new ExprCast(nt1_s, nt3_s, nt5_s); 
					var nt7_s = tmp_16(nt7_i);
					e_prim_no_array_creation_s = nt7_s; 
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
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_prim_no_array_creation_s;
		}
		
		ExprRoot tmp_16(IAST tmp_16_i)
		{
			int alt = 0;
			switch (Next.token)
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
			case ']':
				alt = 0;
				break;
			case '.':
				alt = 1;
				break;
			case '(':
				alt = 2;
				break;
			case '[':
				alt = 3;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_16_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_16_s = (ExprRoot)tmp_16_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('.', nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					nt3_i = new ExprDot(((ExprRoot)tmp_16_i), nt2_s); 
					var nt3_s = tmp_16(nt3_i);
					tmp_16_s = nt3_s; 
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match('(', nt1_i);
					var nt2_s = e_list_opz(nt2_i);
					TokenAST nt3_s = Match(')', nt3_i);
					nt4_i = new ExprFun(((ExprRoot)tmp_16_i), nt1_s, nt2_s); 
					var nt4_s = tmp_16(nt4_i);
					tmp_16_s = nt4_s; 
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match('[', nt1_i);
					var nt2_s = e_list(nt2_i);
					TokenAST nt3_s = Match(']', nt3_i);
					nt4_i = new ExprArray(((ExprRoot)tmp_16_i), nt1_s, nt2_s); 
					var nt4_s = tmp_16(nt4_i);
					tmp_16_s = nt4_s; 
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
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_16_s;
		}
		
		ExprRoot e_new(IAST e_new_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case NEW:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot e_new_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(NEW, nt1_i);
					var nt2_s = type_dot(nt2_i);
					var nt3_s = tmp_31(nt1_s, nt2_s, nt3_i);
					e_new_s = nt3_s;
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
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_new_s;
		}
		
		ExprRoot tmp_31(IAST nt1_s, IAST nt2_s, IAST tmp_31_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '(':
				alt = 0;
				break;
			case '[':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprRoot tmp_31_s = default(ExprRoot);
			switch (alt)
			{
			case 0:
				{
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt3_s = Match('(', nt3_i);
					var nt4_s = e_list_opz(nt4_i);
					TokenAST nt5_s = Match(')', nt5_i);
					tmp_31_s = new ExprNewObj(((TokenAST)nt1_s), ((RefTypeSimple)nt2_s), nt4_s);
				}
				break;
			case 1:
				{
					var nt3_i = default(IAST);
					
					var nt3_s = e_new_array(nt3_i);
					tmp_31_s = new ExprNewArray(((TokenAST)nt1_s), ((RefTypeSimple)nt2_s), nt3_s);
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
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_31_s;
		}
		
		ExprRankList e_new_array(IAST e_new_array_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '[':
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			ExprRankList e_new_array_s = default(ExprRankList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match('[', nt1_i);
					var nt2_s = e_list(nt2_i);
					TokenAST nt3_s = Match(']', nt3_i);
					nt4_i = new ExprRankList().Add(nt2_s); 
					var nt4_s = tmp_17(nt4_i);
					e_new_array_s = nt4_s; 
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
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_new_array_s;
		}
		
		ExprRankList tmp_17(IAST tmp_17_i)
		{
			int alt = 0;
			switch (Next.token)
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
			case ']':
				alt = 0;
				break;
			case '[':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprRankList tmp_17_s = default(ExprRankList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_17_s = (ExprRankList)tmp_17_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match('[', nt1_i);
					var nt2_s = e_new_array_rank_opz(nt2_i);
					TokenAST nt3_s = Match(']', nt3_i);
					nt4_i = ((ExprRankList)tmp_17_i).Add(nt2_s); 
					var nt4_s = tmp_17(nt4_i);
					tmp_17_s = nt4_s; 
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
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_17_s;
		}
		
		ExprList e_new_array_rank_opz(IAST e_new_array_rank_opz_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ']':
				alt = 0;
				break;
			case ',':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprList e_new_array_rank_opz_s = default(ExprList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					e_new_array_rank_opz_s = new ExprList().Add(null);
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					var nt1_s = e_new_array_rank(nt1_i);
					e_new_array_rank_opz_s = nt1_s;
				}
				break;
			}
			
			switch (Next.token)
			{
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_new_array_rank_opz_s;
		}
		
		ExprList e_new_array_rank(IAST e_new_array_rank_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ',':
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			ExprList e_new_array_rank_s = default(ExprList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(',', nt1_i);
					nt2_i = new ExprList().Add(null).Add(null); 
					var nt2_s = tmp_18(nt2_i);
					e_new_array_rank_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case ']':
				break;
			default:
				Error();
				break;
			}
			return e_new_array_rank_s;
		}
		
		ExprList tmp_18(IAST tmp_18_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ']':
				alt = 0;
				break;
			case ',':
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ExprList tmp_18_s = default(ExprList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_18_s = (ExprList)tmp_18_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(',', nt1_i);
					nt2_i = ((ExprList)tmp_18_i).Add(null); 
					var nt2_s = tmp_18(nt2_i);
					tmp_18_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case ']':
				break;
			default:
				Error();
				break;
			}
			return tmp_18_s;
		}
		
		ExprList e_list_opz(IAST e_list_opz_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ')':
				alt = 0;
				break;
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
			
			switch (Next.token)
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
			switch (Next.token)
			{
			case '-':
			case '+':
			case NEW:
			case NUM:
			case STR:
			case CHR:
			case ID:
			case THIS:
			case BASE:
			case FALSE:
			case TRUE:
			case NULL:
			case '(':
			case CAST:
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
					var nt2_s = tmp_19(nt2_i);
					e_list_s = nt2_s; 
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
			return e_list_s;
		}
		
		ExprList tmp_19(IAST tmp_19_i)
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
			
			ExprList tmp_19_s = default(ExprList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_19_s = (ExprList)tmp_19_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(',', nt1_i);
					var nt2_s = e_ass(nt2_i);
					nt3_i = ((ExprList)tmp_19_i).Add(nt2_s); 
					var nt3_s = tmp_19(nt3_i);
					tmp_19_s = nt3_s; 
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
			return tmp_19_s;
		}
		
		RefTypeRoot type(IAST type_i)
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
			
			RefTypeRoot type_s = default(RefTypeRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = type_dot(nt1_i);
					nt2_i = nt1_s; 
					var nt2_s = tmp_20(nt2_i);
					type_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
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
		
		RefTypeRoot tmp_20(IAST tmp_20_i)
		{
			int alt = 0;
			switch (Next.token)
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
			
			RefTypeRoot tmp_20_s = default(RefTypeRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_20_s = (RefTypeRoot)tmp_20_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match('[', nt1_i);
					var nt2_s = e_new_array_rank_opz(nt2_i);
					TokenAST nt3_s = Match(']', nt3_i);
					nt4_i = new RefTypeArray(((RefTypeRoot)tmp_20_i), nt2_s.Count); 
					var nt4_s = tmp_20(nt4_i);
					tmp_20_s = nt4_s; 
				}
				break;
			}
			
			switch (Next.token)
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
			return tmp_20_s;
		}
		
		RefTypeSimple type_dot(IAST type_dot_i)
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
			
			RefTypeSimple type_dot_s = default(RefTypeSimple);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(ID, nt1_i);
					nt2_i = new RefTypeSimple(nt1_s); 
					var nt2_s = tmp_21(nt2_i);
					type_dot_s = nt2_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case ',':
			case '(':
			case '[':
			case '{':
			case ';':
			case ')':
				break;
			default:
				Error();
				break;
			}
			return type_dot_s;
		}
		
		RefTypeSimple tmp_21(IAST tmp_21_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ',':
			case '(':
			case '[':
			case '{':
			case ';':
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
			
			RefTypeSimple tmp_21_s = default(RefTypeSimple);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_21_s = (RefTypeSimple)tmp_21_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match('.', nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					nt3_i = ((RefTypeSimple)tmp_21_i).Add(nt2_s); 
					var nt3_s = tmp_21(nt3_i);
					tmp_21_s = nt3_s; 
				}
				break;
			}
			
			switch (Next.token)
			{
			case ',':
			case '(':
			case '[':
			case '{':
			case ';':
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
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('c'), new RegToken('l')), new RegToken('a')), new RegToken('s')), new RegToken('s')), CLASS);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('i'), new RegToken('n')), new RegToken('t')), new RegToken('e')), new RegToken('r')), new RegToken('f')), new RegToken('a')), new RegToken('c')), new RegToken('e')), INTERFACE);
			acts.Add(new RegAnd(new RegAnd(new RegToken('n'), new RegToken('e')), new RegToken('w')), NEW);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('n'), new RegToken('a')), new RegToken('m')), new RegToken('e')), new RegToken('s')), new RegToken('p')), new RegToken('a')), new RegToken('c')), new RegToken('e')), NAMESPACE);
			acts.Add(new RegAnd(new RegAnd(new RegToken('g'), new RegToken('e')), new RegToken('t')), GET);
			acts.Add(new RegAnd(new RegAnd(new RegToken('s'), new RegToken('e')), new RegToken('t')), SET);
			acts.Add(new RegAnd(new RegToken('i'), new RegToken('f')), IF);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('e'), new RegToken('l')), new RegToken('s')), new RegToken('e')), ELSE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('w'), new RegToken('h')), new RegToken('i')), new RegToken('l')), new RegToken('e')), WHILE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('b'), new RegToken('r')), new RegToken('e')), new RegToken('a')), new RegToken('k')), BREAK);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('c'), new RegToken('o')), new RegToken('n')), new RegToken('t')), new RegToken('i')), new RegToken('n')), new RegToken('u')), new RegToken('e')), CONTINUE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('c'), new RegToken('a')), new RegToken('s')), new RegToken('t')), CAST);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('r'), new RegToken('e')), new RegToken('t')), new RegToken('u')), new RegToken('r')), new RegToken('n')), RETURN);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('t'), new RegToken('r')), new RegToken('u')), new RegToken('e')), TRUE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('f'), new RegToken('a')), new RegToken('l')), new RegToken('s')), new RegToken('e')), FALSE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('n'), new RegToken('u')), new RegToken('l')), new RegToken('l')), NULL);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('t'), new RegToken('h')), new RegToken('i')), new RegToken('s')), THIS);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('b'), new RegToken('a')), new RegToken('s')), new RegToken('e')), BASE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('n'), new RegToken('a')), new RegToken('t')), new RegToken('i')), new RegToken('v')), new RegToken('e')), NATIVE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('v'), new RegToken('i')), new RegToken('r')), new RegToken('t')), new RegToken('u')), new RegToken('a')), new RegToken('l')), VIRTUAL);
			acts.Add(new RegToken('.'), '.');
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
			acts.Add(new RegAnd(new RegToken('/'), new RegToken('*')), (ref NFA.Token tk, LexReader rd, NFA nfa) => {
				for (;;)
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
			acts.Add(new RegToken('"'), (ref NFA.Token tk, LexReader rd, NFA nfa) => {
				var sb = new StringBuilder();
				sb.Append('"');
				for (;;)
				{
					int ch = rd.Read().ch;
					if (ch == -1) throw new Exception("EOF in string");
					if (ch == '"')
						break;
					if (ch == '\\')
						ch = rd.Read().ch;
					else
						sb.Append((char)ch);
				}
				sb.Append('"');
				
				rd.SetMatch();
				rd.EndToken(out tk.strRead, out tk.fileName, out tk.line);
				tk.strRead = sb.ToString();
				tk.token = STR;
				return true;
			});
			acts.Add(new RegToken(39), (ref NFA.Token tk, LexReader rd, NFA nfa) => {
				int ch = rd.Read().ch;
				if (ch == -1) throw new Exception("EOF in char");
				if (rd.Read().ch != '\'') throw new Exception("unterminated char");
				
				rd.SetMatch();
				rd.EndToken(out tk.strRead, out tk.fileName, out tk.line);
				tk.strRead = ((char)ch).ToString();
				tk.token = CHR;
				return true;
			});
			return acts;
		}
	}
}
