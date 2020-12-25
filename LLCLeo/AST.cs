
using LLParserLexerLib;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LLCLeo
{
	public class DeclRootList : DeclRoot, IEnumerable<DeclRoot>
	{
		public readonly List<DeclRoot> c = new List<DeclRoot>();
		public DeclRootList(DeclRoot d) : base(d.tk) { Add(d); }
		public DeclRootList Add(DeclRoot a) { c.Add(a); return this; }

		public IEnumerator<DeclRoot> GetEnumerator() => c.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => c.GetEnumerator();

		public override void CRelativeName(string path, List<(string, DeclRoot)> v)
		{
			foreach (var a in this.c)
				a.CRelativeName(path, v);
		}


		public override string CName => "LL";
	}
	public abstract class DeclRoot : IAST
	{
		public DeclRoot container;
		public readonly TokenAST tk;
		protected DeclRoot(TokenAST tk) { this.tk = tk; }

		public abstract string CName { get; }
		public virtual void CRelativeName(string path, List<(string CName, DeclRoot Decl)> v) { }

	}
	public class DeclFunction : DeclRoot
	{
		public readonly DeclVisibility vs;
		public readonly DeclArgs ag;
		public readonly TypeRoot ret;
		public readonly DeclBody s;

		public DeclFunction(DeclVisibility vs, TokenAST name, DeclArgs ag, TypeRoot tr, DeclBody s) : base(name)
		{
			this.vs = vs;
			this.ag = ag;
			this.ret = tr;
			this.s = s;
		}

		public override string CName => container.CName + "__" + tk.strRead;
	}
	public class DeclDelegate : DeclRoot
	{
		public readonly DeclVisibility vs;
		public readonly DeclArgs args;
		public readonly TypeRoot ret;
		public DeclDelegate(DeclVisibility vs, TokenAST tk, DeclArgs ag, TypeRoot ret) : base(tk)
		{
			this.vs = vs;
			this.args = ag;
			this.ret = ret;
		}
		public override string CName => container.CName + "__" + tk.strRead;
		public override void CRelativeName(string path, List<(string, DeclRoot)> v) { 
			if (path.Length > 0) path += "."; 
			v.Add((path + tk.strRead, this)); 
		}
	}
	public class DeclArgs : List<(TokenAST var, TypeRoot type)>, IAST
	{
		public DeclArgs() { }
		public DeclArgs(TokenAST var, TypeRoot type) { Add(var, type); }
		public DeclArgs Add(TokenAST var, TypeRoot type) { base.Add((var, type)); return this; }
	}
	public class DeclVar : DeclRoot
	{
		public readonly DeclVisibility vs;
		public readonly TypeRoot type;
		public readonly ExprRoot e;
		public DeclVar(DeclVisibility vs, TokenAST v, ExprRoot e) : base(v) { this.vs = vs; this.e = e; }
		public DeclVar(DeclVisibility vs, TokenAST v, TypeRoot type, ExprRoot e = null) : base(v) { this.vs = vs; this.type = type; this.e = e; }

		public override string CName
		{
			get
			{
				if (this.container != null && this.container is DeclClass)
					return tk.strRead;
				else
					return container.CName + "__" + tk.strRead;
			}
		}
	}
	public class DeclVisibility : IAST
	{
		public readonly TokenAST tk;
		public DeclVisibility() { }
		public DeclVisibility(TokenAST tk) { this.tk = tk; }
	}
	public class DeclBody
	{
		public readonly TokenAST tk;
		public readonly StmtRoot s;
		public DeclBody(TokenAST tk, StmtRoot s) { this.tk = tk; this.s = s; }
	}


	public class DeclNative : DeclRoot
	{
		string _s;
		public DeclNative(string s) : base(null)
		{
			_s = s;
		}
		public override string CName => _s;
		public override void CRelativeName(string path, List<(string,DeclRoot)> v) { if (path.Length > 0) path += "."; v.Add((path + _s, this)); }
	}
	public class DeclClass : DeclRoot
	{
		public readonly DeclVisibility vs;
		public readonly TokenAST v;
		public readonly DeclRootList s;
		public DeclClass(DeclVisibility vs, TokenAST tk, DeclRootList s) : base(tk) { this.vs = vs; this.s = s; }
		public override string CName => container.CName + "__" + tk.strRead;
		public override void CRelativeName(string path, List<(string CName, DeclRoot Decl)> v)
		{
			if (path.Length > 0) path += ".";
			path += tk.strRead;
			v.Add((path, this));
			foreach (var a in this.s)
				a.CRelativeName(path, v);
		}
	}
	public class DeclNamespace : DeclRoot
	{
		public readonly DeclRootList s;
		public DeclNamespace(TokenAST tk, DeclRootList s) : base(tk) { this.s = s; }
		public override string CName => container.CName + "__" + tk.strRead;
		public override void CRelativeName(string path, List<(string CName, DeclRoot Decl)> v)
		{
			if (path.Length > 0) path += ".";
			path += tk.strRead;
			v.Add((path, this));
			foreach (var a in this.s)
				a.CRelativeName(path, v);
		}
	}

	////////////////////////////////////////////////////////////////////////

	public class ExprRoot : IAST
	{
		public readonly TokenAST tk;
		public ExprRoot(TokenAST tk) { this.tk = tk; }
	}
	class ExprBinOp : ExprRoot
	{
		public readonly ExprRoot a;
		public readonly ExprRoot b;
		public ExprBinOp(ExprRoot a, TokenAST tk, ExprRoot b) : base(tk) { this.a = a; this.b = b; }
	}
	class ExprAss : ExprRoot
	{
		public readonly ExprRoot a;
		public readonly ExprRoot b;
		public ExprAss(ExprRoot a, TokenAST tk, ExprRoot b) : base(tk) { this.a = a; this.b = b; }
	}
	class ExprArray : ExprRoot
	{
		public readonly ExprRoot e;
		public readonly ExprList el;
		public ExprArray(ExprRoot e, TokenAST tk, ExprList el) : base(tk) { this.e = e; this.el = el; }
	}
	class ExprVar : ExprRoot
	{
		public readonly ExprVar a;
		public readonly TokenAST c;
		public ExprVar(TokenAST tk) : base(tk) { }
		public ExprVar(ExprVar a, TokenAST tk = null, TokenAST c = null) : base(tk) { this.a = a; this.c = c; }
	}
	class ExprSub : ExprBinOp { public ExprSub(ExprRoot a, TokenAST tk, ExprRoot b) : base(a, tk, b) { } }
	class ExprAdd : ExprBinOp { public ExprAdd(ExprRoot a, TokenAST tk, ExprRoot b) : base(a, tk, b) { } }
	class ExprMul : ExprBinOp { public ExprMul(ExprRoot a, TokenAST tk, ExprRoot b) : base(a, tk, b) { } }
	class ExprDiv : ExprBinOp { public ExprDiv(ExprRoot a, TokenAST tk, ExprRoot b) : base(a, tk, b) { } }
	class ExprRem : ExprBinOp { public ExprRem(ExprRoot a, TokenAST tk, ExprRoot b) : base(a, tk, b) { } }
	class ExprPreInc : ExprRoot { public ExprRoot a; public ExprPreInc(TokenAST tk, ExprRoot a) : base(tk) { this.a = a; } }
	class ExprPreDec : ExprRoot { public ExprRoot a; public ExprPreDec(TokenAST tk, ExprRoot a) : base(tk) { this.a = a; } }
	class ExprPostInc : ExprRoot { public ExprRoot a; public ExprPostInc(ExprRoot a, TokenAST tk) : base(tk) { this.a = a; } }
	class ExprPostDec : ExprRoot { public ExprRoot a; public ExprPostDec(ExprRoot a, TokenAST tk) : base(tk) { this.a = a; } }
	class ExprPlus : ExprRoot { public ExprPlus(TokenAST tk, ExprRoot v) : base(tk) { } }
	class ExprCast : ExprRoot
	{
		public readonly TypeRoot tr;
		public readonly ExprRoot e;
		public ExprCast(TokenAST tk, TypeRoot tr, ExprRoot e) : base(tk) { this.tr = tr; this.e = e; }
	}
	class ExprCall : ExprRoot
	{
		public readonly ExprRoot e;
		public readonly ExprList pe;
		public ExprCall(ExprRoot e, TokenAST tk, ExprList pe) : base(tk) { this.e = e; this.pe = pe; }
	}
	class ExprNeg : ExprRoot { public readonly ExprRoot e; public ExprNeg(TokenAST tk, ExprRoot e) : base(tk) { this.e = e; } }
	class ExprNum : ExprRoot { public ExprNum(TokenAST tk) : base(tk) { } }
	class ExprNew : ExprRoot
	{
		public readonly TypeRoot tr;
		public readonly ExprList e;
		public ExprNew(TokenAST tk, TypeRoot tr, ExprList e) : base(tk) { this.tr = tr; this.e = e; }
	}

	class ExprList : List<ExprRoot>, IAST
	{
		public ExprList() { }
		public ExprList(ExprRoot e) { Add(e); }
		public new ExprList Add(ExprRoot e) { base.Add(e); return this; }
	}

	////////////////////////////////////////////////////////////////////////

	public abstract class TypeRoot : IAST
	{
		public DeclRoot _container;

		public readonly TokenAST tk;
		public TypeRoot(TokenAST tk) { this.tk = tk; }
		public abstract string CName { get; }
		public abstract DeclRoot Container { get; set; }
	}
	public class TypeNat : TypeRoot
	{
		public TypeNat(TokenAST tk) : base(tk) { }
		public override string CName => tk.strRead;
		public override DeclRoot Container { get => _container; set { _container = value; } }
	}
	public class TypeDef : TypeRoot
	{
		List<TokenAST> _tk = new List<TokenAST>();
		public TypeDef(TokenAST tk) : base(tk) { _tk.Add(tk); }
		public TypeDef Add(TokenAST tk) { _tk.Add(tk); return this; }

		public override string CName
		{
			get
			{
				var s = "";
				foreach (var t in _tk) if (s == "") s += t.strRead; else s += "." + t.strRead;
				return s;
			}
		}
		public override DeclRoot Container { get => _container; set { _container = value; } }
	}
	public class TypeArray : TypeRoot
	{
		public readonly TypeRoot tr;
		public TypeArray(TypeRoot tr) : base(tr.tk) { this.tr = tr; }

		public override string CName => tr.CName;
		public override DeclRoot Container { get => _container; set { _container = value; if (tr != null) tr.Container = value; } }
	}

	public class TypePt : TypeRoot
	{
		public readonly TypeRoot tr;
		public TypePt(TypeRoot tr) : base(tr.tk) { this.tr = tr; }

		public override string CName => tr.CName;
		public override DeclRoot Container { get => _container; set { _container = value; if (tr != null) tr.Container = value; } }
	}

	////////////////////////////////////////////////////////////////////////

	public abstract class StmtRoot : IAST
	{
		public readonly TokenAST tk;
		public StmtRoot(TokenAST tk) { this.tk = tk; }
	}
	public class StmtWhile : StmtRoot
	{
		public readonly ExprRoot e;
		public readonly StmtRoot s;
		public StmtWhile(TokenAST tk, ExprRoot e, StmtRoot s) : base(tk) { this.e = e; this.s = s; }
	}
	public class StmtReturn : StmtRoot
	{
		public readonly ExprRoot e;
		public StmtReturn(TokenAST tk, ExprRoot e = null) : base(tk) { this.e = e; }
	}
	public class StmtNull : StmtRoot { public StmtNull(TokenAST tk) : base(tk) { } }
	public class StmtIf : StmtRoot
	{
		public readonly ExprRoot e;
		public readonly StmtRoot a;
		public readonly StmtRoot b;
		public StmtIf(TokenAST tk, ExprRoot e, StmtRoot a, StmtRoot b = null) : base(tk)
		{
			this.e = e;
			this.a = a;
			this.b = b;
		}
	}
	public class StmtFor : StmtRoot
	{
		public readonly ExprRoot a;
		public readonly StmtDef d;
		public readonly ExprRoot b;
		public readonly ExprRoot c;
		public readonly StmtRoot s;
		public StmtFor(TokenAST tk, ExprRoot a, ExprRoot b, ExprRoot c, StmtRoot s) : base(tk)
		{
			this.a = a;
			this.b = b;
			this.c = c;
			this.s = s;
		}
		public StmtFor(TokenAST tk, StmtDef d, ExprRoot b, ExprRoot c, StmtRoot s) : base(tk)
		{
			this.d = d;
			this.b = b;
			this.c = c;
			this.s = s;
		}
	}
	public class StmtExpr : StmtRoot
	{
		public readonly ExprRoot e;
		public StmtExpr(ExprRoot e) : base(null) { this.e = e; }
	}
	public class StmtDef : StmtRoot
	{
		public readonly TokenAST v;
		public readonly TypeRoot tr;
		public readonly ExprRoot e;
		public StmtDef(TokenAST tk, TokenAST v, ExprRoot e) : base(tk) { this.v = v; this.e = e; }
		public StmtDef(TokenAST tk, TokenAST v, TypeRoot tr, ExprRoot e = null) : base(tk) { this.v = v; this.tr = tr; this.e = e; }
	}
	public class StmtBlock : StmtRoot
	{
		public readonly List<StmtRoot> lst = new List<StmtRoot>();
		public StmtBlock() : base(null) { }
		public StmtBlock Add(StmtRoot s) { lst.Add(s); return this; }

	}
}
