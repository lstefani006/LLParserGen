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
		private readonly List<AssRoot> _ass;
		private readonly DeclGlobal _dg;

		public string NewTmp()
		{
			return U.F("T{0}", ++_cnt_tmp);
		}

		public string NewLbl()
		{
			return U.F("${0}", ++_cnt_lbl);
		}

		public Context(DeclGlobal dg)
		{
			_dg = dg;
			_ass = new List<AssRoot>();
		}

		public void Dispose()
		{
		}

		public void add(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			e();
			/***/if (rs.IsInt) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.iadd, rd, rs, rt));
			else if (rs.IsDbl) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.fadd, rd, rs, rt));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void sub(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			e();
			/***/if (rs.IsInt) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.isub, rd, rs, rt));
			else if (rs.IsDbl) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.fsub, rd, rs, rt));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void mul(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			e();
			/***/if (rs.IsInt) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.imul, rd, rs, rt));
			else if (rs.IsDbl) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.fmul, rd, rs, rt));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void rem(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			e();
			/***/if (rs.IsInt) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.irem, rd, rs, rt));
			else if (rs.IsDbl) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.frem, rd, rs, rt));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void div(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			e();
			/***/if (rs.IsInt) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.idiv, rd, rs, rt));
			else if (rs.IsDbl) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.fdiv, rd, rs, rt));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void or_(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			e();
			/***/if (rs.IsInt) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.ior, rd, rs, rt));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void xor(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			e();
			/***/if (rs.IsInt) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.ixor, rd, rs, rt));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void and(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			e();
			/***/if (rs.IsInt) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.iand, rd, rs, rt));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void shl(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			e();
			/***/if (rs.IsInt) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.ishl, rd, rs, rt));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void shr(string rd, ExprValue rs, ExprValue rt)
		{
			Debug.Assert(rs.Type == rt.Type);
			e();
			/***/if (rs.Type.IsInt) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.ishr, rd, rs, rt));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void jmp(string addr)
		{
			e();
			_ass.Add(new J(this, emitNextLbl, OpCode.jmp, addr));
			emitNextLbl = null;
		}

		public void js(string rd, string addr, ExprType et)
		{
			e();
			/***/if (et == null) _ass.Add(new J(this, emitNextLbl, OpCode.vjs, rd, addr));
			else if (et.IsVoid) _ass.Add(new J(this, emitNextLbl, OpCode.vjs, rd, addr));
			else if (et.IsInt) _ass.Add(new J(this, emitNextLbl, OpCode.ijs, rd, addr));
			else if (et.IsBool) _ass.Add(new J(this, emitNextLbl, OpCode.ijs, rd, addr));
			else if (et.IsDbl) _ass.Add(new J(this, emitNextLbl, OpCode.fjs, rd, addr));
			else if (et.IsObj) _ass.Add(new J(this, emitNextLbl, OpCode.ojs, rd, addr));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void ret(ExprValue rt)
		{
			e();
			/***/if (rt == null) _ass.Add(new Ret(this, emitNextLbl, OpCode.vret, rt));
			else if (rt.IsVoid) _ass.Add(new Ret(this, emitNextLbl, OpCode.vret, rt));
			else if (rt.IsInt) _ass.Add(new Ret(this, emitNextLbl, OpCode.iret, rt));
			else if (rt.IsBool) _ass.Add(new Ret(this, emitNextLbl, OpCode.iret, rt));
			else if (rt.IsDbl) _ass.Add(new Ret(this, emitNextLbl, OpCode.fret, rt));
			else if (rt.IsObj) _ass.Add(new Ret(this, emitNextLbl, OpCode.oret, rt));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void beq(ExprValue rs, ExprValue rt, string addr)
		{
			e();
			/***/if (rs.IsInt) _ass.Add(new Br(this, emitNextLbl, OpCode.ibeq, rs, rt, addr));
			else if (rs.IsBool) _ass.Add(new Br(this, emitNextLbl, OpCode.ibeq, rs, rt, addr));
			else if (rs.IsDbl) _ass.Add(new Br(this, emitNextLbl, OpCode.fbeq, rs, rt, addr));
			else if (rs.IsObj) _ass.Add(new Br(this, emitNextLbl, OpCode.obeq, rs, rt, addr));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void bne(ExprValue rs, ExprValue rt, string addr)
		{
			e();
			/***/if (rs.IsInt) _ass.Add(new Br(this, emitNextLbl, OpCode.ibne, rs, rt, addr));
			else if (rs.IsBool) _ass.Add(new Br(this, emitNextLbl, OpCode.ibne, rs, rt, addr));
			else if (rs.IsDbl) _ass.Add(new Br(this, emitNextLbl, OpCode.fbne, rs, rt, addr));
			else if (rs.IsObj) _ass.Add(new Br(this, emitNextLbl, OpCode.obne, rs, rt, addr));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void blt(ExprValue rs, ExprValue rt, string addr)
		{
			e();
			/***/if (rs.IsInt) _ass.Add(new Br(this, emitNextLbl, OpCode.iblt, rs, rt, addr));
			else if (rs.IsDbl) _ass.Add(new Br(this, emitNextLbl, OpCode.fblt, rs, rt, addr));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void ble(ExprValue rs, ExprValue rt, string addr)
		{
			e();
			/***/if (rs.IsInt) _ass.Add(new Br(this, emitNextLbl, OpCode.ible, rs, rt, addr));
			else if (rs.IsDbl) _ass.Add(new Br(this, emitNextLbl, OpCode.fble, rs, rt, addr));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void bgt(ExprValue rs, ExprValue rt, string addr)
		{
			e();
			/***/if (rs.IsInt) _ass.Add(new Br(this, emitNextLbl, OpCode.ibgt, rs, rt, addr));
			else if (rs.IsDbl) _ass.Add(new Br(this, emitNextLbl, OpCode.fbgt, rs, rt, addr));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void bge(ExprValue rs, ExprValue rt, string addr)
		{
			e();
			/***/if (rs.IsInt) _ass.Add(new Br(this, emitNextLbl, OpCode.ibge, rs, rt, addr));
			else if (rs.IsDbl) _ass.Add(new Br(this, emitNextLbl, OpCode.fbge, rs, rt, addr));
			else Debug.Assert(false);
			emitNextLbl = null;
		}
		public void ld(string rs, int n)
		{
			ld(rs, new ExprValue(n));
		}

		public void ld(string rs, ExprValue c)
		{
			e();
			/***/if (c.IsInt) _ass.Add(new OpDS(this, emitNextLbl, OpCode.ild, rs, c));
			else if (c.IsBool) _ass.Add(new OpDS(this, emitNextLbl, OpCode.ild, rs, c));
			else if (c.IsDbl) _ass.Add(new OpDS(this, emitNextLbl, OpCode.fld, rs, c));
			else if (c.IsObj) _ass.Add(new OpDS(this, emitNextLbl, OpCode.old, rs, c));
			else Debug.Assert(false);
			emitNextLbl = null;
		}
		public void i2d(string rd, string rs)
		{
			e();
			_ass.Add(new OpDS(this, emitNextLbl, OpCode.i2f, rd, new ExprValue(rs, TypeSimple.Int)));
			emitNextLbl = null;
		}
		public void d2i(string rd, string rs)
		{
			e();
			_ass.Add(new OpDS(this, emitNextLbl, OpCode.f2i, rd, new ExprValue(rs, TypeSimple.Dbl)));
			emitNextLbl = null;
		}

		public void ldm(string rd, ExprValue rs, int offset, ExprType et)
		{
			e();
			/***/if (et.IsInt) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.ildm, rd, rs, new ExprValue(offset)));
			else if (et.IsBool) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.ildm, rd, rs, new ExprValue(offset)));
			else if (et.IsDbl) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.fldm, rd, rs, new ExprValue(offset)));
			else if (et.IsObj) _ass.Add(new OpDSS(this, emitNextLbl, OpCode.oldm, rd, rs, new ExprValue(offset)));
			else Debug.Assert(false);
			emitNextLbl = null;
		}
		public void stm(string rd, int offset, ExprValue rs)
		{
			e();
			/***/if (rs.IsInt) _ass.Add(new OpSSS(this, emitNextLbl, OpCode.istm, rd, new ExprValue(offset), rs));
			else if (rs.IsBool)_ass.Add(new OpSSS(this, emitNextLbl, OpCode.istm, rd, new ExprValue(offset), rs));
			else if (rs.IsDbl)_ass.Add(new OpSSS(this, emitNextLbl, OpCode.fstm, rd, new ExprValue(offset), rs));
			else if (rs.IsObj)_ass.Add(new OpSSS(this, emitNextLbl, OpCode.ostm, rd, new ExprValue(offset), rs));
			else Debug.Assert(false);
			emitNextLbl = null;
		}

		public void newobj(string rd, int size, int vt)
		{
			e();
			_ass.Add(new OpNew(this, emitNextLbl, OpCode.onew, rd, size, vt));
			emitNextLbl = null;
		}

	
		
		U.Set<string> emitNextLbl;

		public void emit(string lbl)
		{
			if (emitNextLbl == null) emitNextLbl = new U.Set<string>();
			emitNextLbl.Add(lbl);
		}

		private void e()
		{
			if (emitNextLbl == null)
			{
				emitNextLbl = new U.Set<string>();
				emitNextLbl.Add(NewLbl());
			}
		}

		private void ComputeLive(int istart, int iend, List<string> alwaysLive)
		{
			// capisco per ogni istruzione quale sono
			// le istruzioni che la seguono.
			// puo` essere quella immediatamente successiva 
			// o un jmp da qualche parte nel codice.
			for (int i = istart; i < iend; ++i)
				_ass[i].ComputeSucc(this);

			bool changed;
			do
			{
				changed = false;
				for (int i = iend - 1; i >= istart; --i)
				{
					var c = _ass[i];

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

		public string ToString(int istart, int iend)
		{
			string r = "";
			for (int i = istart; i < iend; ++i)
			{
				var v = _ass[i];
				r += v.ToString() + "\n";
			}

			if (true)
			{
				r += "[";
				bool first = true;
				foreach (var v in _ass[_ass.Count - 1].Out)
				{
					if (first == false) r += ", ";
					first = false;
					r += v;
				}
				r += "]\n";
			}

			return r;
		}

		public override string ToString()
		{
			return ToString(0, _ass.Count);
		}

		private Graph CreateGraph(int istart, int iend)
		{
			Graph gr = new Graph();

			// creo i nodi
			for (int i = istart; i < iend; ++i)
			{
				var c = _ass[i];
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
				var c = _ass[i];

				for (int j = 0; j < c.In.Count; ++j)
				{
					Debug.Assert(gr.ExistsNode(c.In[j]) == true);

					for (int k = j + 1; k < c.In.Count; ++k)
						if (gr.ExistsNode(c.In[k]) == true)
							gr.AddEdge(c.In[j], c.In[k]);
				}
			}

			var rr = _ass[iend - 1].Out;
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

		private void SetTemps(int istart, int iend, Dictionary<string, string> regs)
		{
			for (int i = istart; i < iend; ++i)
			{
				var stmt = _ass[i];
				foreach (var t in regs)
					stmt.Substitute(t.Key, t.Value);
			}
		}


		public AssRoot GetSuccOp(AssRoot ass)
		{
			for (int i = 0; i < _ass.Count; ++i)
				if (_ass[i] == ass && i + 1 < _ass.Count)
					return _ass[i + 1];
			return null;
		}

		public AssRoot GetOp(string lbl)
		{
			for (var i = 0; i < _ass.Count; ++i)
				if (_ass[i].Lbl.Contains(lbl))
					return _ass[i];
			return null;
		}


		private bool GenerateCode(DeclList member)
		{
			foreach (var f in member)
			{
				var df = f as DeclFun;
				var dc = f as DeclClass;
				
				if (df != null)
				{
					if (GenerateCode(df) == false)
						return false;
				}
				else if (dc != null)
				{
					if (GenerateCode(dc.members) == false)
						return false;
				}
			}
			return true;
		}

		public bool GenerateCode()
		{
			return GenerateCode(_dg.members);
		}

		public bool GenerateCode(DeclFun f)
		{
			bool debug = false;
			bool optimize = true;

			using (var fctx = new FunctionContex(this, f))
			{
				if (f.Father is DeclClass)
				{
					fctx.AddArgVar(new TokenAST(f.name.fileName, f.name.lineNu, MParser.THIS, "this", "this"), new TypeSimple(f.Father.name.v));

					if (((DeclClass)f.Father).name.v == f.name.v)
					{
						if (f.ret != new TypeSimple(f.name.v))
							throw new SyntaxError(f.lastCurly, "constructor must return object of the enclosed class");
					}
				}

				int regAllocati = f.args.Count;
				foreach (var a in f.args)
					fctx.AddArgVar(a.ArgName, a.ArgType);

				int istart = _ass.Count;
				this.emit(f.AssName);
				bool unreachable = f.body.GenCode(fctx);
				if (unreachable == false)
				{
					if (f.ret.IsVoid == false)
						throw new SyntaxError(f.lastCurly, "missing return stmt");
					fctx.Context.ret(null);
				}
				int iend = _ass.Count;

				// if (debug) Console.WriteLine("{0}", this.ToString(istart, iend));


				// le var in ingresso anche se non servono più non vengono sovrascritte
				// da altre variabili (se invece si vuole ottimizzare al max si può omettere il Foreach).
				var live = new List<string>();
				if (optimize == false) f.args.ForEach(v => live.Add(fctx.GetVar(v.ArgName).Reg));
				this.ComputeLive(istart, iend, live);
				if (debug)
				{
					Console.WriteLine("Codice generato per la funzione {0} {1}/{2}", f.name, istart, iend);
					Console.WriteLine("Live variables");
					Console.WriteLine(this.ToString(istart, iend));
				}

				var gr = this.CreateGraph(istart, iend);
				if (debug)
				{
					Console.WriteLine("Grafo");
					Console.WriteLine(gr);
				}

				bool ok = false;
				for (int k = regAllocati; k < 32; ++k)
				{
					var col = gr.Color(k);
					if (col != null)
					{
						Console.WriteLine("Ci vogliono k={0} da r0 a r{1}, prossimo per push/js r{0}", k, k - 1);
						var regs = col.GetRegs();
						this.SetTemps(istart, iend, regs);
						ok = true;
						break;
					}
				}

				if (ok == false)
				{
					Console.WriteLine("Cannot allocate registers");
					return false;
				}

				Console.WriteLine(this.ToString(istart, iend));
				Console.WriteLine("#####################");
				return ok;
			}
		}

		public DeclFun GetFun(string name, List<TypeRoot> args)
		{
			foreach (var f in _dg.members)
			{
				if (f.name.v != name) continue;
				var fun = f as DeclFun;
				if (fun == null) continue;
				if (fun.args.Count != args.Count) continue;
				bool ok = true;
				for (int i = 0; i < args.Count; ++i)
				{
					if (args[i] != fun.args[i].ArgType) { ok = false; break; }
				}
				if (ok) return fun;
			}
			return null;
		}

		public DeclClass GetClass(string name)
		{
			foreach (var f in _dg.members)
				if (f.name.v == name)
				{
					var c = f as DeclClass;
					if (c != null)
						return c;
				}
			return null;
		}
		public DeclFun GetFun(string className, string fun, List<TypeRoot> args)
		{
			var dc = this.GetClass(className);
			if (dc == null)
				return null;

			foreach (var m in dc.members)
			{
				if (m is DeclFun == false) continue;
				var df = (DeclFun)m;
				if (df.name.v != fun) continue;
				if (df.args.Count != args.Count) continue;
				bool ok = true;
				for (int i = 0; i < args.Count; ++i)
				{
					if (args[i] != df.args[i].ArgType) { ok = false; break; }
				}
				if (ok)
					return df;
			}
			return null;
		}
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

		public string NewLbl() { return _ctx.NewLbl(); }
		public string NewTmp() { return _ctx.NewTmp(); }

		public void emit(string lbl) { _ctx.emit(lbl); }


		Dictionary<string, ExprValue> _localVars = new Dictionary<string, ExprValue>();
		int _nvar = 0;

		public void AddArgVar(TokenAST name, TypeRoot type)
		{
			if (_localVars.ContainsKey(name.v) == true)
				throw new SyntaxError(name, "duplicated param '{0}'", name.v);
			_localVars[name.v] = new ExprValue(U.F("r{0}", _nvar++), type);
		}


		public void AddDefVar(TokenAST name, TypeRoot ty)
		{
			if (_localVars.ContainsKey(name.v) == true)
				throw new SyntaxError(name, "duplicated variable '{0}'", name.v);
			_localVars[name.v] = new ExprValue(_ctx.NewTmp(), ty);
		}

		public void UnDefVar(TokenAST name)
		{
			Debug.Assert(_localVars.ContainsKey(name.v));
			_localVars.Remove(name.v);
		}

		// bisogna confondere var con fun.
		// .... 
		public ExprValue GetVar(TokenAST name)
		{
			if (_localVars.ContainsKey(name.v) == true)
				return _localVars[name.v];

			if (fun.Father is DeclGlobal)
			{
				DeclGlobal dg = (DeclGlobal)fun.Father;
				foreach (var m in dg.members)
				{
					DeclFun f = m as DeclFun;
					if (f == null) continue;
					if (f.name.v == name.v)
						return new ExprValue(new ExprType(new TypeFun(dg, f.name.v)));
				}
			}

			throw new SyntaxError(name, "variable '{0}' not found", name.v);
		}


		public DeclFun GetFun(string name, List<TypeRoot> args)
		{
			return this.Context.GetFun(name, args);
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
			public string lblBreak;
			public string lblContinue;
			public TokenAST varName;
		}

		public void Push(StmtTk tk, string lblBreak, string lblContinue, TokenAST vv)
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
