

using System.Collections.Generic;
using LLParserLexerLib;

namespace LLProtoBuff
{
	#region list
	public class AList<C, T> : List<T>, IAST where C : AList<C, T>
	{
		public AList() : base() { }
		public AList(T d) : base() { this.Add(d); }
		public new C Add(T d) { base.Add(d); return (C)this; }
	}
	public class DeclList : AList<DeclList, DeclRoot>
	{
		public DeclList(DeclRoot d) : base(d)
		{
		}
	}
	public class EnumList : AList<EnumList, EnumType>
	{
		public EnumList(EnumType d) : base(d)
		{
		}
	}
	public class FieldList : AList<FieldList, FieldRoot>
	{
		public FieldList(FieldRoot d) : base(d)
		{
		}
	}
	public class ServiceList : AList<ServiceList, Service>
	{
		public ServiceList(Service d) : base(d)
		{
		}
	}

	public class OneOfList : AList<OneOfList, Optional>
	{
		public OneOfList(Optional d) : base(d) { }
	}
	#endregion

	public abstract class DeclRoot : IAST
	{
		public virtual bool IsSyntax => false;
		public virtual bool IsPackage => false;
		public virtual bool IsMessage => false;
		public virtual bool IsEnum => false;
	}

	public class EnumType : IAST
	{
		public readonly TokenAST ID;
		public readonly TokenAST NUM;

		public EnumType(TokenAST nt1_s, TokenAST nt3_s) { this.ID = nt1_s; this.NUM = nt3_s; }
	}

	public class FieldType : IAST { }

	public class EnumDecl : DeclRoot, IAST
	{
		public override bool IsEnum => true;
		public readonly TokenAST ID;
		public readonly EnumList List;
		public EnumDecl(TokenAST nt2_s, EnumList nt4_s)
		{
			this.ID = nt2_s;
			this.List = nt4_s;
		}
	}
	public class ServiceDecl : DeclRoot, IAST
	{
		private TokenAST nt2_s;
		private ServiceList nt4_s;

		public ServiceDecl(TokenAST nt2_s, ServiceList nt4_s)
		{
			this.nt2_s = nt2_s;
			this.nt4_s = nt4_s;
		}
	}
	public class Service : IAST
	{
		private TokenAST nt2_s;
		private TokenAST nt4_s;
		private TokenAST nt8_s;

		public Service(TokenAST nt2_s, TokenAST nt4_s, TokenAST nt8_s)
		{
			this.nt2_s = nt2_s;
			this.nt4_s = nt4_s;
			this.nt8_s = nt8_s;
		}
	}

	public class FieldRoot : IAST
	{
		public virtual bool IsOptional => false;
		public virtual bool IsOneOf => false;
		public virtual bool IsRepeated => false;
	}
	public class OneOf : FieldRoot
	{
		public override bool IsOneOf => true;
		public readonly TokenAST ID;
		public readonly OneOfList List;
		public OneOf(TokenAST ID, OneOfList List)
		{
			this.ID = ID;
			this.List = List;
		}

	}
	public class Optional : FieldRoot
	{
		public override bool IsOptional => true;
		public readonly TokenAST TYPE;
		public readonly TokenAST ID;
		public readonly TokenAST NUM;

		public Optional(TokenAST TYPE, TokenAST ID, TokenAST NUM)
		{
			this.TYPE = TYPE;
			this.ID = ID;
			this.NUM = NUM;
		}
	}
	public class Repeated : FieldRoot
	{
		public override bool IsRepeated => true;
		public readonly TokenAST TYPE;
		public readonly TokenAST ID;
		public readonly TokenAST NUM;

		public Repeated(FieldRoot f)
		{
			if (!f.IsOptional) throw new SyntaxError("only type");
			this.TYPE = ((Optional) f).TYPE;
			this.ID = ((Optional)f).ID;
			this.NUM = ((Optional)f).NUM;
		}
	}

	class PackageDecl : DeclRoot, IAST
	{
		public override bool IsPackage => true;
		public readonly TokenAST Str;
		public PackageDecl(TokenAST nt2_s) => this.Str = nt2_s;
	}

	class ImportDecl : DeclRoot, IAST
	{
		private TokenAST nt2_s;

		public ImportDecl(TokenAST nt2_s)
		{
			this.nt2_s = nt2_s;
		}
	}
	class MessageDecl : DeclRoot, IAST
	{
		public override bool IsMessage => true;
		public readonly TokenAST ID;
		public readonly FieldList Fields;

		public MessageDecl(TokenAST nt2_s, FieldList nt4_s)
		{
			this.ID = nt2_s;
			this.Fields = nt4_s;
		}
	}

	class SyntaxDecl : DeclRoot, IAST
	{
		public override bool IsSyntax => true;
		public readonly TokenAST ID;
		public SyntaxDecl(TokenAST nt3_s) => this.ID = nt3_s;
	}

}