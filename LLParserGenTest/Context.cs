using System;
using System.Diagnostics;
using System.Collections.Generic;
using LLParserLexerLib;

namespace LLParserGenTest
{
	public class Context : IDisposable
	{
		private int _cnt_tmp;
		private int _cnt_lbl;
		public readonly List<AssRoot> _code;
		public readonly List<AssRoot> _data;
		private readonly DeclGlobal _dg;

		public string NewTmp()
		{
			return U.F("T{0}", ++_cnt_tmp);
		}

		public Label NewLbl()
		{
			return new Label(U.F("${0}", ++_cnt_lbl));
		}

		public Context(DeclGlobal dg)
		{
			_dg = dg;
			_code = new List<AssRoot>();
			_data = new List<AssRoot>();
		}

		public void Dispose()
		{
		}

		public void add(string rd, ExprValue rs, ExprValue rt)
		{
			OpCode op = OpCode.nop;
			switch (rt.TypeBase)
			{
			case TypeBase.Double: op = OpCode.fadd; break;
			case TypeBase.Int: op = OpCode.iadd; break;
			case TypeBase.Bool:
			case TypeBase.Obj:
			case TypeBase.Void:
			default: op = OpCode.nop; Debug.Assert(false); break;
			}
			startCode();
			_code.Add(new OpDSS(this, codeNextLbl, op, rd, rs, rt));
			codeNextLbl = null;
		}

		public void sub(string rd, ExprValue rs, ExprValue rt)
		{
			OpCode op = OpCode.nop;
			switch (rt.TypeBase)
			{
			case TypeBase.Double: op = OpCode.fsub; break;
			case TypeBase.Int: op = OpCode.isub; break;
			case TypeBase.Bool:
			case TypeBase.Obj:
			case TypeBase.Void:
			default: op = OpCode.nop; Debug.Assert(false); break;
			}
			startCode();
			_code.Add(new OpDSS(this, codeNextLbl, op, rd, rs, rt));
			codeNextLbl = null;
		}

		public void mul(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			startCode();
			/***/if (rs.TypeBase == TypeBase.Int) _code.Add(new OpDSS(this, codeNextLbl, OpCode.imul, rd, rs, rt));
			else if (rs.TypeBase == TypeBase.Double) _code.Add(new OpDSS(this, codeNextLbl, OpCode.fmul, rd, rs, rt));
			else Debug.Assert(false);
			codeNextLbl = null;
		}

		public void rem(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			startCode();
			/***/if (rs.TypeBase == TypeBase.Int) _code.Add(new OpDSS(this, codeNextLbl, OpCode.irem, rd, rs, rt));
			else if (rs.TypeBase == TypeBase.Double) _code.Add(new OpDSS(this, codeNextLbl, OpCode.frem, rd, rs, rt));
			else Debug.Assert(false);
			codeNextLbl = null;
		}

		public void div(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			startCode();
			/***/if (rs.TypeBase == TypeBase.Int) _code.Add(new OpDSS(this, codeNextLbl, OpCode.idiv, rd, rs, rt));
			else if (rs.TypeBase == TypeBase.Double) _code.Add(new OpDSS(this, codeNextLbl, OpCode.fdiv, rd, rs, rt));
			else Debug.Assert(false);
			codeNextLbl = null;
		}

		public void or_(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			startCode();
			/***/if (rs.TypeBase == TypeBase.Int) _code.Add(new OpDSS(this, codeNextLbl, OpCode.ior, rd, rs, rt));
			else Debug.Assert(false);
			codeNextLbl = null;
		}

		public void xor(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			startCode();
			/***/if (rs.TypeBase == TypeBase.Int) _code.Add(new OpDSS(this, codeNextLbl, OpCode.ixor, rd, rs, rt));
			else Debug.Assert(false);
			codeNextLbl = null;
		}

		public void and(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			startCode();
			/***/if (rs.TypeBase == TypeBase.Int) _code.Add(new OpDSS(this, codeNextLbl, OpCode.iand, rd, rs, rt));
			else Debug.Assert(false);
			codeNextLbl = null;
		}

		public void shl(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			startCode();
			/***/if (rs.TypeBase == TypeBase.Int) _code.Add(new OpDSS(this, codeNextLbl, OpCode.ishl, rd, rs, rt));
			else Debug.Assert(false);
			codeNextLbl = null;
		}

		public void shr(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			startCode();
			/***/if (rs.TypeBase == TypeBase.Int) _code.Add(new OpDSS(this, codeNextLbl, OpCode.ishr, rd, rs, rt));
			else Debug.Assert(false);
			codeNextLbl = null;
		}

		public void jmp(Label addr)
		{
			startCode();
			_code.Add(new J(this, codeNextLbl, OpCode.jmp, addr));
			codeNextLbl = null;
		}

		public void js(string rd, Label addr, ExprType et)
		{
			startCode();
			if (et == null)
				_code.Add(new J(this, codeNextLbl, OpCode.vjs, rd, addr));
			else
			{
				switch (et.TypeBase)
				{
				case TypeBase.Bool: _code.Add(new J(this, codeNextLbl, OpCode.ijs, rd, addr)); break;
				case TypeBase.Int: _code.Add(new J(this, codeNextLbl, OpCode.ijs, rd, addr)); break;
				case TypeBase.Char: _code.Add(new J(this, codeNextLbl, OpCode.ijs, rd, addr)); break;
				case TypeBase.Double: _code.Add(new J(this, codeNextLbl, OpCode.fjs, rd, addr)); break;
				case TypeBase.Obj: _code.Add(new J(this, codeNextLbl, OpCode.ojs, rd, addr)); break;
				case TypeBase.Void: _code.Add(new J(this, codeNextLbl, OpCode.vjs, rd, addr)); break;
				default: Debug.Assert(false); break;
				}
			}
			codeNextLbl = null;
		}

		public void ret(ExprValue rt)
		{
			OpCode op = OpCode.vret;
			if (rt != null)
			{
				switch (rt.TypeBase)
				{
				case TypeBase.Bool: op = OpCode.iret; break;
				case TypeBase.Int: op = OpCode.iret; break;
				case TypeBase.Char: op = OpCode.iret; break;
				case TypeBase.Double: op = OpCode.fret; break;
				case TypeBase.Obj: op = OpCode.oret; break;
				case TypeBase.Void: op = OpCode.vret; break;
				default: op = OpCode.nop; Debug.Assert(false); break;
				}
			}

			startCode();
			_code.Add(new Ret(this, codeNextLbl, op, rt));
			codeNextLbl = null;
		}

		public void beq(ExprValue rs, ExprValue rt, Label addr)
		{
			OpCode op = OpCode.nop;
			switch (rt.TypeBase)
			{
			case TypeBase.Bool: op = OpCode.ibeq; break;
			case TypeBase.Char: op = OpCode.ibeq; break;
			case TypeBase.Int: op = OpCode.ibeq; break;
			case TypeBase.Double: op = OpCode.fbeq; break;
			case TypeBase.Obj: 
			case TypeBase.Void: 
			default: op = OpCode.nop; Debug.Assert(false); break;
			}
			startCode();
			_code.Add(new Br(this, codeNextLbl, op, rs, rt, addr));
			codeNextLbl = null;
		}

		public void bne(ExprValue rs, ExprValue rt, Label addr)
		{
			OpCode op = OpCode.nop;
			switch (rt.TypeBase)
			{
			case TypeBase.Bool: op = OpCode.ibne; break;
			case TypeBase.Int: op = OpCode.ibne; break;
			case TypeBase.Char: op = OpCode.ibne; break;
			case TypeBase.Double: op = OpCode.fbne; break;
			case TypeBase.Obj:
			case TypeBase.Void:
			default: op = OpCode.nop; Debug.Assert(false); break;
			}
			startCode();
			_code.Add(new Br(this, codeNextLbl, op, rs, rt, addr));
			codeNextLbl = null;
		}

		public void blt(ExprValue rs, ExprValue rt, Label addr)
		{
			OpCode op = OpCode.nop;
			switch (rt.TypeBase)
			{
			case TypeBase.Bool: op = OpCode.iblt; break;
			case TypeBase.Int: op = OpCode.iblt; break;
			case TypeBase.Char: op = OpCode.iblt; break;
			case TypeBase.Double: op = OpCode.fblt; break;
			case TypeBase.Obj:
			case TypeBase.Void:
			default: op = OpCode.nop; Debug.Assert(false); break;
			}
			startCode();
			_code.Add(new Br(this, codeNextLbl, op, rs, rt, addr));
			codeNextLbl = null;
		}

		public void ble(ExprValue rs, ExprValue rt, Label addr)
		{
			OpCode op = OpCode.nop;
			switch (rt.TypeBase)
			{
			case TypeBase.Bool: op = OpCode.ible; break;
			case TypeBase.Int: op = OpCode.ible; break;
			case TypeBase.Char: op = OpCode.ible; break;
			case TypeBase.Double: op = OpCode.fble; break;
			case TypeBase.Obj:
			case TypeBase.Void:
			default: op = OpCode.nop; Debug.Assert(false); break;
			}
			startCode();
			_code.Add(new Br(this, codeNextLbl, op, rs, rt, addr));
			codeNextLbl = null;
		}

		public void bgt(ExprValue rs, ExprValue rt, Label addr)
		{
			OpCode op = OpCode.nop;
			switch (rt.TypeBase)
			{
			case TypeBase.Bool: op = OpCode.ibgt; break;
			case TypeBase.Int: op = OpCode.ibgt; break;
			case TypeBase.Char: op = OpCode.ibgt; break;
			case TypeBase.Double: op = OpCode.fbgt; break;
			case TypeBase.Obj:
			case TypeBase.Void:
			default: op = OpCode.nop; Debug.Assert(false); break;
			}
			startCode();
			_code.Add(new Br(this, codeNextLbl, op, rs, rt, addr));
			codeNextLbl = null;
		}

		public void bge(ExprValue rs, ExprValue rt, Label addr)
		{
			OpCode op = OpCode.nop;
			switch (rt.TypeBase)
			{
			case TypeBase.Bool: op = OpCode.ibge; break;
			case TypeBase.Int: op = OpCode.ibge; break;
			case TypeBase.Char: op = OpCode.ibge; break;
			case TypeBase.Double: op = OpCode.fbge; break;
			case TypeBase.Obj:
			case TypeBase.Void:
			default: op = OpCode.nop; Debug.Assert(false); break;
			}
			startCode();
			_code.Add(new Br(this, codeNextLbl, op, rs, rt, addr));
			codeNextLbl = null;
		}
		public void ld(string rs, int n)
		{
			ld(rs, new ExprValue(n));
		}

		public void ld(string rs, ExprValue c)
		{
			OpCode op;
			switch (c.TypeBase)
			{
			case TypeBase.Bool: op = OpCode.ild; break;
			case TypeBase.Int: op = OpCode.ild; break;
			case TypeBase.Char: op = OpCode.ild; break;
			case TypeBase.Double: op = OpCode.fld; break;
			case TypeBase.Obj: op = OpCode.old; break;
			default: op = OpCode.nop;  Debug.Assert(false); break;
			}

			startCode();
			_code.Add(new OpDS(this, codeNextLbl, op, rs, c));
			codeNextLbl = null;
		}
		public void i2d(string rd, string rs)
		{
			startCode();
			_code.Add(new OpDS(this, codeNextLbl, OpCode.i2f, rd, new ExprValue(rs, TypeSimple.Int)));
			codeNextLbl = null;
		}
		public void d2i(string rd, string rs)
		{
			startCode();
			_code.Add(new OpDS(this, codeNextLbl, OpCode.f2i, rd, new ExprValue(rs, TypeSimple.Dbl)));
			codeNextLbl = null;
		}

		public void ldm(string rd, ExprValue rs, int offset, ExprType et)
		{
			startCode();
			switch (et.TypeBase)
			{
			case TypeBase.Int: _code.Add(new OpDSS(this, codeNextLbl, OpCode.ildm, rd, rs, new ExprValue(offset))); break;
			case TypeBase.Bool: _code.Add(new OpDSS(this, codeNextLbl, OpCode.ildm, rd, rs, new ExprValue(offset))); break;
			case TypeBase.Char: _code.Add(new OpDSS(this, codeNextLbl, OpCode.ildm, rd, rs, new ExprValue(offset))); break;
			case TypeBase.Double: _code.Add(new OpDSS(this, codeNextLbl, OpCode.fldm, rd, rs, new ExprValue(offset))); break;
			case TypeBase.Obj: _code.Add(new OpDSS(this, codeNextLbl, OpCode.oldm, rd, rs, new ExprValue(offset))); break;
			case TypeBase.Handle: _code.Add(new OpDSS(this, codeNextLbl, OpCode.hldm, rd, rs, new ExprValue(offset))); break;
			default: Debug.Assert(false); break;
			}
			codeNextLbl = null;
		}
		public void stm(string rd, int offset, ExprValue rs)
		{
			startCode();
			/***/if (rs.TypeBase == TypeBase.Int) _code.Add(new OpSSS(this, codeNextLbl, OpCode.istm, rd, new ExprValue(offset), rs));
			else if (rs.TypeBase == TypeBase.Bool) _code.Add(new OpSSS(this, codeNextLbl, OpCode.istm, rd, new ExprValue(offset), rs));
			else if (rs.TypeBase == TypeBase.Char) _code.Add(new OpSSS(this, codeNextLbl, OpCode.istm, rd, new ExprValue(offset), rs));
			else if (rs.TypeBase == TypeBase.Double) _code.Add(new OpSSS(this, codeNextLbl, OpCode.fstm, rd, new ExprValue(offset), rs));
			else if (rs.TypeBase == TypeBase.Obj) _code.Add(new OpSSS(this, codeNextLbl, OpCode.ostm, rd, new ExprValue(offset), rs));
			else if (rs.TypeBase == TypeBase.Handle) _code.Add(new OpSSS(this, codeNextLbl, OpCode.hstm, rd, new ExprValue(offset), rs));
			else Debug.Assert(false);
			codeNextLbl = null;
		}

		public void newobj(string rd, int size, Label vt)
		{
			startCode();
			_code.Add(new OpNew(this, codeNextLbl, OpCode.onew, rd, size, vt));
			codeNextLbl = null;
		}


		public void putByte(byte c)
		{
			startData();
			_data.Add(new Data(this, dataNextLbl, c));
			dataNextLbl = null;
		}
		public void putString(string c)
		{
			startData();
			_data.Add(new Data(this, dataNextLbl, c));
			dataNextLbl = null;
		}
		public void putInt(int c)
		{
			startData();
			_data.Add(new Data(this, dataNextLbl, c));
			dataNextLbl = null;
		}
		public void putInt(Label lbl)
		{
			startData();
			_data.Add(new Data(this, dataNextLbl, lbl));
			dataNextLbl = null;
		}

		U.Set<Label> codeNextLbl;
		U.Set<Label> dataNextLbl;

		public void codeLbl(Label lbl)
		{
			if (codeNextLbl == null) codeNextLbl = new U.Set<Label>();
			codeNextLbl.Add(lbl);
		}
		public void dataLbl(Label lbl)
		{
			if (dataNextLbl == null) dataNextLbl = new U.Set<Label>();
			dataNextLbl.Add(lbl);
		}

		/// <summary>
		/// inizio una nuova istruzione
		/// </summary>
		private void startCode()
		{
			if (codeNextLbl != null) return;
			codeNextLbl = new U.Set<Label>();
			codeNextLbl.Add(NewLbl());
		}
		/// <summary>
		/// inizio una nuova istruzione
		/// </summary>
		private void startData()
		{
			if (dataNextLbl != null) return;
			dataNextLbl = new U.Set<Label>();
			dataNextLbl.Add(NewLbl());
		}
		public void ComputeLive(int istart, int iend, List<string> alwaysLive)
		{
			// capisco per ogni istruzione quale sono
			// le istruzioni che la seguono.
			// puo` essere quella immediatamente successiva 
			// o un jmp da qualche parte nel codice.
			for (int i = istart; i < iend; ++i)
				_code[i].ComputeSucc(this);

			bool changed;
			do
			{
				changed = false;
				for (int i = iend - 1; i >= istart; --i)
				{
					var c = _code[i];

					// per ogni ret devo aggiungere il codice che mi tiene viva le var in uscita
					U.Set<string> force = new U.Set<string>();
					if (i == iend - 1 || c is Ret)
						alwaysLive.ForEach(force.Add);

					// force contiene la lista dei registri che devono rimanere vivi
					if (c.ComputeLive(force))
						changed = true;
				}
			} while (changed);
		}

		public string CodeToString(int istart, int iend)
		{
			string r = "";
			for (int i = istart; i < iend; ++i)
			{
				var v = _code[i];
				r += v.ToString() + "\n";
			}

			if (true)
			{
				r += "[";
				bool first = true;
				foreach (var v in _code[_code.Count - 1].Out)
				{
					if (first == false) r += ", ";
					first = false;
					r += v;
				}
				r += "]\n";
			}

			return r;
		}
		public string DataToString(int istart, int iend)
		{
			string r = "";
			for (int i = istart; i < iend; ++i)
			{
				var v = _data[i];
				r += v.ToString() + "\n";
			}

			if (true)
			{
				r += "[";
				bool first = true;
				if (_data.Count > 0)
				{
					foreach (var v in _data[_data.Count - 1].Out)
					{
						if (first == false) r += ", ";
						first = false;
						r += v;
					}
				}
				r += "]\n";
			}

			return r;
		}

		public override string ToString()
		{
			return CodeToString(0, _code.Count) + DataToString(0, _data.Count);
		}

		public Graph CreateGraph(int istart, int iend)
		{
			Graph gr = new Graph();

			// creo i nodi
			for (int i = istart; i < iend; ++i)
			{
				var c = _code[i];
				foreach (var n in c.In)
				{
					if (gr.ExistsNode(n) == false)
					{
						// se inizia con T e' un registro da allocare
						// see inizia con "r" e' un registro gia' allocato
						Debug.Assert(n.StartsWith("r") || n.StartsWith("T"));
						string reg = n.StartsWith("r") ? n : null;
						gr.CreateNode(n, reg);
					}
				}
			}

			// creo gli archi 
			for (int i = istart; i < iend; ++i)
			{
				var c = _code[i];

				for (int j = 0; j < c.In.Count; ++j)
				{
					Debug.Assert(gr.ExistsNode(c.In[j]) == true);

					for (int k = j + 1; k < c.In.Count; ++k)
						if (gr.ExistsNode(c.In[k]) == true)
							gr.AddEdge(c.In[j], c.In[k]);
				}
			}

			var rr = _code[iend - 1].Out;
			for (int j = 0; j < rr.Count; ++j)
			{
				Debug.Assert(gr.ExistsNode(rr[j]) == true);

				for (int k = j + 1; k < rr.Count; ++k)
				{
					if (gr.ExistsNode(rr[k]))
						gr.AddEdge(rr[j], rr[k]);
				}
			}

			return gr;
		}

		public void SetTemps(int istart, int iend, Dictionary<string, string> regs)
		{
			for (int i = istart; i < iend; ++i)
			{
				var stmt = _code[i];
				foreach (var t in regs)
					stmt.Substitute(t.Key, t.Value);
			}
		}

		public AssRoot GetSuccOp(AssRoot ass)
		{
			for (int i = 0; i < _code.Count; ++i)
				if (_code[i] == ass && i + 1 < _code.Count)
					return _code[i + 1];
			return null;
		}

		public AssRoot GetOp(Label lbl)
		{
			for (var i = 0; i < _code.Count; ++i)
				if (_code[i].Lbl.Contains(lbl))
					return _code[i];
			return null;
		}

	//	public DeclFun GetFun(string name, List<TypeRoot> args)
	//	{
	//		foreach (var f in _dg.members)
	//		{
	//			if (f.name.strRead != name) continue;
	//			var fun = f as DeclFun;
	//			if (fun == null) continue;
	//			if (fun.args.Count != args.Count) continue;
	//			bool ok = true;
	//			for (int i = 0; i < args.Count; ++i)
	//			{
	//				if (args[i] != fun.args[i].ArgType) { ok = false; break; }
	//			}
	//			if (ok) return fun;
	//		}
	//		return null;
	//	}

	//	public DeclClass GetClass(string name)
	//	{
	//		foreach (var f in _dg.members)
	//			if (f.name.strRead == name)
	//			{
	//				var c = f as DeclClass;
	//				if (c != null)
	//					return c;
	//			}
	//		return null;
	//	}

	//	public DeclFun GetFun(string className, string fun, List<TypeRoot> args)
	//	{
	//		var dc = this.GetClass(className);
	//		if (dc == null)
	//			return null;

	//		foreach (var m in dc.members)
	//		{
	//			if (m is DeclFun == false) continue;
	//			var df = (DeclFun)m;
	//			if (df.name.strRead != fun) continue;
	//			if (df.args.Count != args.Count) continue;
	//			bool ok = true;
	//			for (int i = 0; i < args.Count; ++i)
	//			{
	//				if (args[i] != df.args[i].ArgType) { ok = false; break; }
	//			}
	//			if (ok)
	//				return df;
	//		}
	//		return null;
	//	}
	}

	public class FunctionContex : IDisposable
	{
		readonly Context _ctx;
		public readonly DeclFun fun;

		public FunctionContex(Context ctx, DeclFun fun)
		{
			this._ctx = ctx;
			this.fun = fun;
		}

		public void Dispose()
		{
		}

		public Context Context { get { return _ctx; } }

		public Label NewLbl() { return _ctx.NewLbl(); }
		public string NewTmp() { return _ctx.NewTmp(); }

		public void emit(Label lbl) { _ctx.codeLbl(lbl); }


		Dictionary<string, ExprValue> _localVars = new Dictionary<string, ExprValue>();
		int _nvar = 0;

		public void AddArgVar(TokenAST name, TypeRoot type)
		{
			if (_localVars.ContainsKey(name.strRead) == true)
				throw new SyntaxError(name, "duplicated param '{0}'", name.strRead);
			_localVars[name.strRead] = new ExprValue(U.F("r{0}", _nvar++), type);
		}


		public void AddDefVar(TokenAST name, TypeRoot ty)
		{
			if (_localVars.ContainsKey(name.strRead) == true)
				throw new SyntaxError(name, "duplicated variable '{0}'", name.strRead);
			_localVars[name.strRead] = new ExprValue(_ctx.NewTmp(), ty);
		}

		public void UnDefVar(TokenAST name)
		{
			Debug.Assert(_localVars.ContainsKey(name.strRead));
			_localVars.Remove(name.strRead);
		}

		// bisogna confondere var con fun.
		// .... 
		public ExprValue GetVar(TokenAST name)
		{
			if (_localVars.ContainsKey(name.strRead) == true)
				return _localVars[name.strRead];

			if (fun.Father is DeclGlobal)
			{
				DeclGlobal dg = (DeclGlobal)fun.Father;
				foreach (var m in dg.members)
				{
					DeclFun f = m as DeclFun;
					if (f == null) continue;
					if (f.name.strRead == name.strRead)
						//return new ExprValue(new ExprType(new TypeFun(dg, f.name.strRead)));
						Debug.Assert(false);
				}
			}

			throw new SyntaxError(name, "variable '{0}' not found", name.strRead);
		}


		public enum StmtTk
		{
			Block,
			While,
			For,
			Var
		}

		List<StackData> _tk = new List<StackData>();

		public struct StackData
		{
			public StmtTk tk;
			public Label lblBreak;
			public Label lblContinue;
			public TokenAST varName;
		}

		public void Push(StmtTk tk, Label lblBreak, Label lblContinue, TokenAST vv)
		{
			StackData r = new StackData();
			r.tk = tk;
			r.lblBreak = lblBreak;
			r.lblContinue = lblContinue;
			r.varName = vv;
			_tk.Add(r);
		}

		public void Pop(StmtTk tk)
		{
			int i = _tk.Count - 1;
			while (_tk[i].tk != tk)
			{
				if (_tk[i].varName != null)
				{
					//Context.xor(this.GerVar(_tk[i].varName), new ExprValue(this.GerVar(_tk[i].varName)), new ExprValue(this.GerVar(_tk[i].varName)));
					this.UnDefVar(_tk[i].varName);
				}
				_tk.RemoveRange(i, 1);
				i -= 1;
			}
			_tk.RemoveRange(i, 1);
		}

		public bool Break()
		{
			int i = _tk.Count - 1;
			while (i >= 0 && _tk[i].lblBreak == null)
			{
				if (_tk[i].varName != null)
				{
					//Context.xor(this.GerVar(_tk[i].varName), new ExprValue(this.GerVar(_tk[i].varName)), new ExprValue(this.GerVar(_tk[i].varName)));
				}
				i -= 1;
			}
			if (i < 0) return false;
			this.Context.jmp(_tk[i].lblBreak);
			return true;
		}

		public bool Continue()
		{
			int i = _tk.Count - 1;
			while (i >= 0 && _tk[i].lblContinue == null)
			{
				if (_tk[i].varName != null)
				{
					//Context.xor(this.GerVar(_tk[i].varName), new ExprValue(this.GerVar(_tk[i].varName)), new ExprValue(this.GerVar(_tk[i].varName)));
				}
				i -= 1;
			}
			if (i < 0) return false;
			this.Context.jmp(_tk[i].lblContinue);
			return true;
		}

		public void Return(ExprValue rr)
		{
			for (int i = _tk.Count - 1; i >= 0; --i)
			{
				if (_tk[i].varName != null)
				{
					if (rr != null && rr.IsReg && rr.Reg == this.GetVar(_tk[i].varName).Reg) continue;
					//Context.xor(this.GerVar(_tk[i].varName), new ExprValue(this.GerVar(_tk[i].varName)), new ExprValue(this.GerVar(_tk[i].varName)));
				}
			}
		}
	}
}
