#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.

using System;
using System.Text;
using LLProtoBuff;
using System.Collections.Generic;
using LLParserLexerLib;

namespace PB
{
	public partial class MParser : ParserBase
	{
		public const int IMPORT = -2;
		public const int STR = -3;
		public const int SYNTAX = -4;
		public const int PACKAGE = -5;
		public const int ID = -6;
		public const int MESSAGE = -7;
		public const int ENUM = -8;
		public const int SERVICE = -9;
		public const int RPC = -10;
		public const int RETURNS = -11;
		public const int NUM = -12;
		public const int REPEATED = -13;
		public const int ONEOF = -14;
		public const int DOUBLE = -15;
		public const int FLOAT = -16;
		public const int INT32 = -17;
		public const int INT64 = -18;
		public const int UINT32 = -19;
		public const int UINT64 = -20;
		public const int SINT32 = -21;
		public const int SINT64 = -22;
		public const int FIXED32 = -23;
		public const int FIXED64 = -24;
		public const int SFIXED32 = -25;
		public const int SFIXED64 = -26;
		public const int BOOL = -27;
		public const int STRING = -28;
		public const int BYTES = -29;
		
		Dictionary<int, string> _token;
		public override Dictionary<int, string> Token
		{
			get
			{
				if (_token == null)
				{
					_token = new Dictionary<int, string>();
					_token.Add(-1, "EOF");
					_token.Add(-2, "IMPORT");
					_token.Add(-3, "STR");
					_token.Add(-4, "SYNTAX");
					_token.Add(-5, "PACKAGE");
					_token.Add(-6, "ID");
					_token.Add(-7, "MESSAGE");
					_token.Add(-8, "ENUM");
					_token.Add(-9, "SERVICE");
					_token.Add(-10, "RPC");
					_token.Add(-11, "RETURNS");
					_token.Add(-12, "NUM");
					_token.Add(-13, "REPEATED");
					_token.Add(-14, "ONEOF");
					_token.Add(-15, "DOUBLE");
					_token.Add(-16, "FLOAT");
					_token.Add(-17, "INT32");
					_token.Add(-18, "INT64");
					_token.Add(-19, "UINT32");
					_token.Add(-20, "UINT64");
					_token.Add(-21, "SINT32");
					_token.Add(-22, "SINT64");
					_token.Add(-23, "FIXED32");
					_token.Add(-24, "FIXED64");
					_token.Add(-25, "SFIXED32");
					_token.Add(-26, "SFIXED64");
					_token.Add(-27, "BOOL");
					_token.Add(-28, "STRING");
					_token.Add(-29, "BYTES");
				}
				return _token;
			}
		}
		
		DeclList start(IAST start_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case IMPORT:
			case SYNTAX:
			case PACKAGE:
			case MESSAGE:
			case ENUM:
			case SERVICE:
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
					
					var nt1_s = decl_list(nt1_i);
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
		
		DeclList decl_list(IAST decl_list_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case IMPORT:
			case SYNTAX:
			case PACKAGE:
			case MESSAGE:
			case ENUM:
			case SERVICE:
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
					nt2_i = new DeclList(nt1_s); 
					var nt2_s = tmp_1(nt2_i);
					decl_list_s = nt2_s; 
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
			return decl_list_s;
		}
		
		DeclList tmp_1(IAST tmp_1_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case -1:
				alt = 0;
				break;
			case IMPORT:
			case SYNTAX:
			case PACKAGE:
			case MESSAGE:
			case ENUM:
			case SERVICE:
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
			case IMPORT:
				alt = 0;
				break;
			case SYNTAX:
				alt = 1;
				break;
			case PACKAGE:
				alt = 2;
				break;
			case MESSAGE:
				alt = 3;
				break;
			case ENUM:
				alt = 4;
				break;
			case SERVICE:
				alt = 5;
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
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(IMPORT, nt1_i);
					TokenAST nt2_s = Match(STR, nt2_i);
					TokenAST nt3_s = Match(';', nt3_i);
					decl_s = new ImportDecl(nt2_s);
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match(SYNTAX, nt1_i);
					TokenAST nt2_s = Match('=', nt2_i);
					TokenAST nt3_s = Match(STR, nt3_i);
					TokenAST nt4_s = Match(';', nt4_i);
					decl_s = new SyntaxDecl(nt3_s);
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					
					TokenAST nt1_s = Match(PACKAGE, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match(';', nt3_i);
					decl_s = new PackageDecl(nt2_s);
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(MESSAGE, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match('{', nt3_i);
					var nt4_s = message_list(nt4_i);
					TokenAST nt5_s = Match('}', nt5_i);
					decl_s = new MessageDecl(nt2_s, nt4_s);
				}
				break;
			case 4:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(ENUM, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match('{', nt3_i);
					var nt4_s = enum_list(nt4_i);
					TokenAST nt5_s = Match('}', nt5_i);
					decl_s = new EnumDecl(nt2_s, nt4_s);
				}
				break;
			case 5:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(SERVICE, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match('{', nt3_i);
					var nt4_s = service_list(nt4_i);
					TokenAST nt5_s = Match('}', nt5_i);
					decl_s = new ServiceDecl(nt2_s, nt4_s) ;
				}
				break;
			}
			
			switch (Next.token)
			{
			case IMPORT:
			case SYNTAX:
			case PACKAGE:
			case MESSAGE:
			case ENUM:
			case SERVICE:
			case -1:
				break;
			default:
				Error();
				break;
			}
			return decl_s;
		}
		
		ServiceList service_list(IAST service_list_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case RPC:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			ServiceList service_list_s = default(ServiceList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = service(nt1_i);
					nt2_i = new ServiceList(nt1_s); 
					var nt2_s = tmp_2(nt2_i);
					service_list_s = nt2_s; 
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
			return service_list_s;
		}
		
		ServiceList tmp_2(IAST tmp_2_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '}':
				alt = 0;
				break;
			case RPC:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			ServiceList tmp_2_s = default(ServiceList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_2_s = (ServiceList)tmp_2_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = service(nt1_i);
					nt2_i = ((ServiceList)tmp_2_i).Add(nt1_s); 
					var nt2_s = tmp_2(nt2_i);
					tmp_2_s = nt2_s; 
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
			return tmp_2_s;
		}
		
		Service service(IAST service_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case RPC:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			Service service_s = default(Service);
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
					var nt11_i = default(IAST);
					
					TokenAST nt1_s = Match(RPC, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match('(', nt3_i);
					TokenAST nt4_s = Match(ID, nt4_i);
					TokenAST nt5_s = Match(')', nt5_i);
					TokenAST nt6_s = Match(RETURNS, nt6_i);
					TokenAST nt7_s = Match('(', nt7_i);
					TokenAST nt8_s = Match(ID, nt8_i);
					TokenAST nt9_s = Match(')', nt9_i);
					TokenAST nt10_s = Match('{', nt10_i);
					TokenAST nt11_s = Match('}', nt11_i);
					service_s = new Service(nt2_s, nt4_s, nt8_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case RPC:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return service_s;
		}
		
		EnumList enum_list(IAST enum_list_i)
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
			
			EnumList enum_list_s = default(EnumList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = enum_decl(nt1_i);
					nt2_i = new EnumList(nt1_s); 
					var nt2_s = tmp_3(nt2_i);
					enum_list_s = nt2_s; 
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
			return enum_list_s;
		}
		
		EnumList tmp_3(IAST tmp_3_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '}':
				alt = 0;
				break;
			case ID:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			EnumList tmp_3_s = default(EnumList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_3_s = (EnumList)tmp_3_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = enum_decl(nt1_i);
					nt2_i = ((EnumList)tmp_3_i).Add(nt1_s); 
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
		
		EnumType enum_decl(IAST enum_decl_i)
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
			
			EnumType enum_decl_s = default(EnumType);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					
					TokenAST nt1_s = Match(ID, nt1_i);
					TokenAST nt2_s = Match('=', nt2_i);
					TokenAST nt3_s = Match(NUM, nt3_i);
					TokenAST nt4_s = Match(';', nt4_i);
					enum_decl_s = new EnumType(nt1_s, nt3_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case ID:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return enum_decl_s;
		}
		
		FieldList message_list(IAST message_list_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case REPEATED:
			case ONEOF:
			case ID:
			case DOUBLE:
			case FLOAT:
			case INT32:
			case INT64:
			case UINT32:
			case UINT64:
			case SINT32:
			case SINT64:
			case FIXED32:
			case FIXED64:
			case SFIXED32:
			case SFIXED64:
			case BOOL:
			case STRING:
			case BYTES:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			FieldList message_list_s = default(FieldList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = field(nt1_i);
					nt2_i = new FieldList(nt1_s); 
					var nt2_s = tmp_4(nt2_i);
					message_list_s = nt2_s; 
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
			return message_list_s;
		}
		
		FieldList tmp_4(IAST tmp_4_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '}':
				alt = 0;
				break;
			case REPEATED:
			case ONEOF:
			case ID:
			case DOUBLE:
			case FLOAT:
			case INT32:
			case INT64:
			case UINT32:
			case UINT64:
			case SINT32:
			case SINT64:
			case FIXED32:
			case FIXED64:
			case SFIXED32:
			case SFIXED64:
			case BOOL:
			case STRING:
			case BYTES:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			FieldList tmp_4_s = default(FieldList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_4_s = (FieldList)tmp_4_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = field(nt1_i);
					nt2_i = ((FieldList)tmp_4_i).Add(nt1_s); 
					var nt2_s = tmp_4(nt2_i);
					tmp_4_s = nt2_s; 
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
			return tmp_4_s;
		}
		
		FieldRoot field(IAST field_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ID:
			case DOUBLE:
			case FLOAT:
			case INT32:
			case INT64:
			case UINT32:
			case UINT64:
			case SINT32:
			case SINT64:
			case FIXED32:
			case FIXED64:
			case SFIXED32:
			case SFIXED64:
			case BOOL:
			case STRING:
			case BYTES:
				alt = 0;
				break;
			case REPEATED:
				alt = 1;
				break;
			case ONEOF:
				alt = 2;
				break;
			default:
				Error();
				break;
			}
			
			FieldRoot field_s = default(FieldRoot);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					var nt1_s = type(nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match('=', nt3_i);
					TokenAST nt4_s = Match(NUM, nt4_i);
					TokenAST nt5_s = Match(';', nt5_i);
					field_s = new Optional(nt1_s, nt2_s, nt4_s);
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					TokenAST nt1_s = Match(REPEATED, nt1_i);
					var nt2_s = field(nt2_i);
					field_s = new Repeated(nt2_s);
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					TokenAST nt1_s = Match(ONEOF, nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match('{', nt3_i);
					var nt4_s = oneof_list(nt4_i);
					TokenAST nt5_s = Match('}', nt5_i);
					field_s = new OneOf(nt2_s, nt4_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case REPEATED:
			case ONEOF:
			case ID:
			case DOUBLE:
			case FLOAT:
			case INT32:
			case INT64:
			case UINT32:
			case UINT64:
			case SINT32:
			case SINT64:
			case FIXED32:
			case FIXED64:
			case SFIXED32:
			case SFIXED64:
			case BOOL:
			case STRING:
			case BYTES:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return field_s;
		}
		
		OneOfList oneof_list(IAST oneof_list_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ID:
			case DOUBLE:
			case FLOAT:
			case INT32:
			case INT64:
			case UINT32:
			case UINT64:
			case SINT32:
			case SINT64:
			case FIXED32:
			case FIXED64:
			case SFIXED32:
			case SFIXED64:
			case BOOL:
			case STRING:
			case BYTES:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			OneOfList oneof_list_s = default(OneOfList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = oneof(nt1_i);
					nt2_i = new OneOfList(nt1_s); 
					var nt2_s = tmp_5(nt2_i);
					oneof_list_s = nt2_s; 
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
			return oneof_list_s;
		}
		
		OneOfList tmp_5(IAST tmp_5_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case '}':
				alt = 0;
				break;
			case ID:
			case DOUBLE:
			case FLOAT:
			case INT32:
			case INT64:
			case UINT32:
			case UINT64:
			case SINT32:
			case SINT64:
			case FIXED32:
			case FIXED64:
			case SFIXED32:
			case SFIXED64:
			case BOOL:
			case STRING:
			case BYTES:
				alt = 1;
				break;
			default:
				Error();
				break;
			}
			
			OneOfList tmp_5_s = default(OneOfList);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					tmp_5_s = (OneOfList)tmp_5_i; 
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					
					var nt1_s = oneof(nt1_i);
					nt2_i = ((OneOfList)tmp_5_i).Add(nt1_s); 
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
		
		Optional oneof(IAST oneof_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ID:
			case DOUBLE:
			case FLOAT:
			case INT32:
			case INT64:
			case UINT32:
			case UINT64:
			case SINT32:
			case SINT64:
			case FIXED32:
			case FIXED64:
			case SFIXED32:
			case SFIXED64:
			case BOOL:
			case STRING:
			case BYTES:
				alt = 0;
				break;
			default:
				Error();
				break;
			}
			
			Optional oneof_s = default(Optional);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					var nt2_i = default(IAST);
					var nt3_i = default(IAST);
					var nt4_i = default(IAST);
					var nt5_i = default(IAST);
					
					var nt1_s = type(nt1_i);
					TokenAST nt2_s = Match(ID, nt2_i);
					TokenAST nt3_s = Match('=', nt3_i);
					TokenAST nt4_s = Match(NUM, nt4_i);
					TokenAST nt5_s = Match(';', nt5_i);
					oneof_s = new Optional(nt1_s, nt2_s, nt4_s);
				}
				break;
			}
			
			switch (Next.token)
			{
			case ID:
			case DOUBLE:
			case FLOAT:
			case INT32:
			case INT64:
			case UINT32:
			case UINT64:
			case SINT32:
			case SINT64:
			case FIXED32:
			case FIXED64:
			case SFIXED32:
			case SFIXED64:
			case BOOL:
			case STRING:
			case BYTES:
			case '}':
				break;
			default:
				Error();
				break;
			}
			return oneof_s;
		}
		
		TokenAST type(IAST type_i)
		{
			int alt = 0;
			switch (Next.token)
			{
			case ID:
				alt = 0;
				break;
			case DOUBLE:
				alt = 1;
				break;
			case FLOAT:
				alt = 2;
				break;
			case INT32:
				alt = 3;
				break;
			case INT64:
				alt = 4;
				break;
			case UINT32:
				alt = 5;
				break;
			case UINT64:
				alt = 6;
				break;
			case SINT32:
				alt = 7;
				break;
			case SINT64:
				alt = 8;
				break;
			case FIXED32:
				alt = 9;
				break;
			case FIXED64:
				alt = 10;
				break;
			case SFIXED32:
				alt = 11;
				break;
			case SFIXED64:
				alt = 12;
				break;
			case BOOL:
				alt = 13;
				break;
			case STRING:
				alt = 14;
				break;
			case BYTES:
				alt = 15;
				break;
			default:
				Error();
				break;
			}
			
			TokenAST type_s = default(TokenAST);
			switch (alt)
			{
			case 0:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(ID, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 1:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(DOUBLE, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 2:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(FLOAT, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 3:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(INT32, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 4:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(INT64, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 5:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(UINT32, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 6:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(UINT64, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 7:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(SINT32, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 8:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(SINT64, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 9:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(FIXED32, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 10:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(FIXED64, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 11:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(SFIXED32, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 12:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(SFIXED64, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 13:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(BOOL, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 14:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(STRING, nt1_i);
					type_s = nt1_s;
				}
				break;
			case 15:
				{
					var nt1_i = default(IAST);
					
					TokenAST nt1_s = Match(BYTES, nt1_i);
					type_s = nt1_s;
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
			return type_s;
		}
		
		protected override RegAcceptList CreateRegAcceptList()
		{
			var acts = new RegAcceptList();
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('s'), new RegToken('y')), new RegToken('n')), new RegToken('t')), new RegToken('a')), new RegToken('x')), SYNTAX);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('f'), new RegToken('l')), new RegToken('o')), new RegToken('a')), new RegToken('t')), FLOAT);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('d'), new RegToken('o')), new RegToken('u')), new RegToken('b')), new RegToken('l')), new RegToken('e')), DOUBLE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('i'), new RegToken('n')), new RegToken('t')), new RegToken('3')), new RegToken('2')), INT32);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('i'), new RegToken('n')), new RegToken('t')), new RegToken('6')), new RegToken('4')), INT64);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('u'), new RegToken('i')), new RegToken('n')), new RegToken('t')), new RegToken('3')), new RegToken('2')), UINT32);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('u'), new RegToken('i')), new RegToken('n')), new RegToken('t')), new RegToken('6')), new RegToken('4')), UINT64);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('s'), new RegToken('i')), new RegToken('n')), new RegToken('t')), new RegToken('3')), new RegToken('2')), SINT32);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('s'), new RegToken('i')), new RegToken('n')), new RegToken('t')), new RegToken('6')), new RegToken('4')), SINT64);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('f'), new RegToken('i')), new RegToken('x')), new RegToken('e')), new RegToken('d')), new RegToken('3')), new RegToken('2')), FIXED32);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('f'), new RegToken('i')), new RegToken('x')), new RegToken('e')), new RegToken('d')), new RegToken('6')), new RegToken('4')), FIXED64);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('s'), new RegToken('f')), new RegToken('i')), new RegToken('x')), new RegToken('e')), new RegToken('d')), new RegToken('3')), new RegToken('2')), SFIXED32);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('s'), new RegToken('f')), new RegToken('i')), new RegToken('x')), new RegToken('e')), new RegToken('d')), new RegToken('6')), new RegToken('4')), SFIXED64);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('b'), new RegToken('o')), new RegToken('o')), new RegToken('l')), BOOL);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('s'), new RegToken('t')), new RegToken('r')), new RegToken('i')), new RegToken('n')), new RegToken('g')), STRING);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('b'), new RegToken('y')), new RegToken('t')), new RegToken('e')), new RegToken('s')), BYTES);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegToken('e'), new RegToken('n')), new RegToken('u')), new RegToken('m')), ENUM);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('r'), new RegToken('e')), new RegToken('p')), new RegToken('e')), new RegToken('a')), new RegToken('t')), new RegToken('e')), new RegToken('d')), REPEATED);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('o'), new RegToken('n')), new RegToken('e')), new RegToken('o')), new RegToken('f')), ONEOF);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('p'), new RegToken('a')), new RegToken('c')), new RegToken('k')), new RegToken('a')), new RegToken('g')), new RegToken('e')), PACKAGE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('i'), new RegToken('m')), new RegToken('p')), new RegToken('o')), new RegToken('r')), new RegToken('t')), IMPORT);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('s'), new RegToken('y')), new RegToken('n')), new RegToken('t')), new RegToken('a')), new RegToken('x')), SYNTAX);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('m'), new RegToken('e')), new RegToken('s')), new RegToken('s')), new RegToken('a')), new RegToken('g')), new RegToken('e')), MESSAGE);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('s'), new RegToken('e')), new RegToken('r')), new RegToken('v')), new RegToken('i')), new RegToken('c')), new RegToken('e')), SERVICE);
			acts.Add(new RegAnd(new RegAnd(new RegToken('r'), new RegToken('p')), new RegToken('c')), RPC);
			acts.Add(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegAnd(new RegToken('r'), new RegToken('e')), new RegToken('t')), new RegToken('u')), new RegToken('r')), new RegToken('n')), new RegToken('s')), RETURNS);
			acts.Add(new RegToken('='), '=');
			acts.Add(new RegToken(';'), ';');
			acts.Add(new RegToken('{'), '{');
			acts.Add(new RegToken('}'), '}');
			acts.Add(new RegToken('('), '(');
			acts.Add(new RegToken(')'), ')');
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
			return acts;
		}
	}
}
