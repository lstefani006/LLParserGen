using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace LLParserGenTest
{
	public class Context : IDisposable
	{
		private int _cnt_tmp;
		private int _cnt_lbl;
		private readonly List<AssRoot> _ass = new List<AssRoot>();
		public string NewTmp() { return U.F("T{0}", ++_cnt_tmp); }
		public string NewLbl() { return U.F("${0}", ++_cnt_lbl); }

		public Context() { }
		public void Dispose() { }

		public void add(string rd, ExprValue rs, ExprValue rt)   { e(); _ass.Add(new Op_rxx(this, emitNextLbl, OpCode.I_add, rd, rs, rt)); emitNextLbl = null; }
		public void sub(string rd, ExprValue rs, ExprValue rt)   { e(); _ass.Add(new Op_rxx(this, emitNextLbl, OpCode.I_sub, rd, rs, rt)); emitNextLbl = null; }
		public void mul(string rd, ExprValue rs, ExprValue rt)   { e(); _ass.Add(new Op_rxx(this, emitNextLbl, OpCode.I_mul, rd, rs, rt)); emitNextLbl = null; }
		public void rem(string rd, ExprValue rs, ExprValue rt)   { e(); _ass.Add(new Op_rxx(this, emitNextLbl, OpCode.I_rem, rd, rs, rt)); emitNextLbl = null; }
		public void div(string rd, ExprValue rs, ExprValue rt)   { e(); _ass.Add(new Op_rxx(this, emitNextLbl, OpCode.I_div, rd, rs, rt)); emitNextLbl = null; }
		public void or_(string rd, ExprValue rs, ExprValue rt)   { e(); _ass.Add(new Op_rxx(this, emitNextLbl, OpCode.I__or, rd, rs, rt)); emitNextLbl = null; }
		public void xor(string rd, ExprValue rs, ExprValue rt)   { e(); _ass.Add(new Op_rxx(this, emitNextLbl, OpCode.I_xor, rd, rs, rt)); emitNextLbl = null; }
		public void and(string rd, ExprValue rs, ExprValue rt)   { e(); _ass.Add(new Op_rxx(this, emitNextLbl, OpCode.I_and, rd, rs, rt)); emitNextLbl = null; }
		public void shl(string rd, ExprValue rs, ExprValue rt)   { e(); _ass.Add(new Op_rxx(this, emitNextLbl, OpCode.I_shl, rd, rs, rt)); emitNextLbl = null; }
		public void shr(string rd, ExprValue rs, ExprValue rt)   { e(); _ass.Add(new Op_rxx(this, emitNextLbl, OpCode.I_shr, rd, rs, rt)); emitNextLbl = null; }

		public void jmp(string addr)                             { e(); _ass.Add(new AssJ(this, emitNextLbl, OpCode.J_jmp, addr));         emitNextLbl = null; }
		public void js(string addr)                              { e(); _ass.Add(new AssJ(this, emitNextLbl, OpCode.J_js, addr));          emitNextLbl = null; }
		public void ret(ExprValue rt)                            { e(); _ass.Add(new Op_Ret(this, emitNextLbl, rt)); emitNextLbl = null; }

		public void beq(ExprValue rs, ExprValue rt, string addr) { e(); _ass.Add(new Br_xx(this, emitNextLbl, OpCode.B_beq_rr, rs, rt, addr)); emitNextLbl = null; }
		public void bne(ExprValue rs, ExprValue rt, string addr) { e(); _ass.Add(new Br_xx(this, emitNextLbl, OpCode.B_bne_rr, rs, rt, addr)); emitNextLbl = null; }
		public void blt(ExprValue rs, ExprValue rt, string addr) { e(); _ass.Add(new Br_xx(this, emitNextLbl, OpCode.B_blt_rr, rs, rt, addr)); emitNextLbl = null; }
		public void ble(ExprValue rs, ExprValue rt, string addr) { e(); _ass.Add(new Br_xx(this, emitNextLbl, OpCode.B_ble_rr, rs, rt, addr)); emitNextLbl = null; }
		public void bgt(ExprValue rs, ExprValue rt, string addr) { e(); _ass.Add(new Br_xx(this, emitNextLbl, OpCode.B_bgt_rr, rs, rt, addr)); emitNextLbl = null; }
		public void bge(ExprValue rs, ExprValue rt, string addr) { e(); _ass.Add(new Br_xx(this, emitNextLbl, OpCode.B_bge_rr, rs, rt, addr)); emitNextLbl = null; }

		public void ld(string rs, int c) { e(); _ass.Add(new AssLdConst(this, emitNextLbl, rs, c)); emitNextLbl = null; }

		U.Set<string> emitNextLbl;
		public void emit(string lbl) { if (emitNextLbl == null) emitNextLbl = new U.Set<string>(); emitNextLbl.Add(lbl); }
		private void e() { if (emitNextLbl == null) { emitNextLbl = new U.Set<string>(); emitNextLbl.Add(NewLbl()); } }

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

					// cerco gli stmt 
					U.Set<string> rout = new U.Set<string>();
					foreach (var t in c.Succ)
						rout = rout + t.In;

					if (i == iend - 1)  // TODO oppure return oppure halt oppure throw
						alwaysLive.ForEach(rout.Add);
						
					bool b = c.ComputeLive(rout);
					if (b) changed = true;
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
					if (gr.ExistsNode(n) == false) {
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

				for (int k = j + 1; k < rr.Count; ++k) {
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



		public bool GenerateCode(FunList fl) {
			foreach (var f in fl)
				if (GenerateCode(f) == false)
					return false;
			return true;
		}
		public bool GenerateCode(Fun f) {

			bool debug = false;

			var fctx = new FunctionContex(this);

			foreach (var a in f.args.args)
				fctx.AddArgVar(a);

			int istart = _ass.Count;
			this.emit(f.name);
			f.body.GenCode(fctx);

			if (_ass.Count > 0) {
				foreach (var a in f.args.args)
					_ass[_ass.Count - 1].Out.Add(a);
			}

			if (debug) Console.WriteLine("{0}", this.ToString());

			int iend = _ass.Count;
			var live = new List<string>();

			// le var in ingresso anche se non servono più non vengono sovrascritte
			// da altre variabili (se invece si vuole ottimizzare al max si può omettere il Foreach).
			f.args.args.ForEach(v => live.Add(fctx.GerVar(v)));
			this.ComputeLive(istart, iend, live);

			if (debug) {
				Console.WriteLine("Codice generato per la funzione {0} {1}/{2}", f.name, istart, iend);
				Console.WriteLine("Live variables");
				Console.WriteLine(this.ToString(istart, iend));
			}
			var gr = this.CreateGraph(istart, iend);

			if (debug) {
				Console.WriteLine("Grafo");
				Console.WriteLine(gr);
			}

			bool ok = false;
			int k;
			for (k = 0; k < 32; ++k) {
				var col = gr.Color(k);
				if (col != null) {
					if (k > 0)
						Console.WriteLine("Ci vogliono k={0} da r0 a r{1}, prossimo per var locali/parametri r{0}", k, k - 1);
					else 
						Console.WriteLine("Ci vogliono k={0}", k);
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
			return ok;
		}
	}
	public class FunctionContex {

		readonly Context ctx;

		public Context Context { get { return ctx; } }


		public string NewLbl() { return ctx.NewLbl(); }
		public void emit(string lbl) {
			ctx.emit(lbl);
		}

		public FunctionContex(Context ctx)
		{
			this.ctx = ctx;
		}
		private Dictionary<string, string> _vars = new Dictionary<string, string>();
		public void AddDefVar(string name) { 
			if (_vars.ContainsKey(name) == true)
				throw new Exception(U.F("duplicated variable {0}", name));
			_vars[name] = ctx.NewTmp();
		}
		public void UnDefVar(string name) {
			Debug.Assert(_vars.ContainsKey(name));
			_vars.Remove(name);
		}
		int nvar = 0;
		public void AddArgVar(string name) { 
			if (_vars.ContainsKey(name) == true)
				throw new Exception(U.F("duplicated variable {0}", name));
			_vars[name] = U.F("r{0}", nvar++); 
		}
		public string GerVar(string name) { 
			if (_vars.ContainsKey(name) == false)
				throw new Exception(U.F("variable {0} not found", name));
			return _vars[name];
		}

		public enum StmtTk {
			Block,
			While, For,
			Var
		}


		List<StackData> _tk = new List<StackData>();
		public struct StackData {
			public StmtTk tk;
			public string lblBreak;
			public string lblContinue;
			public string varName;
		}
		public void Push(StmtTk tk, string lblBreak, string lblContinue, string vv)
		{
			StackData r = new StackData();
			r.tk = tk;
			r.lblBreak = lblBreak;
			r.lblContinue = lblContinue;
			r.varName = vv;
			_tk.Add(r);
		}

		public void Pop(StmtTk tk) { 
			int i = _tk.Count - 1;
			while (_tk[i].tk != tk) {
				if (_tk[i].varName != null) {
					Context.xor(this.GerVar(_tk[i].varName), new ExprValue(this.GerVar(_tk[i].varName)), new ExprValue(this.GerVar(_tk[i].varName)));
					this.UnDefVar(_tk[i].varName);
				}		
				_tk.RemoveRange(i, 1);
				i -= 1;
			}
			_tk.RemoveRange(i, 1);
		}

		public void Break() {
			int i = _tk.Count - 1;
			while (_tk[i].lblBreak == null) {
				if (_tk[i].varName != null)
					Context.xor(this.GerVar(_tk[i].varName), new ExprValue(this.GerVar(_tk[i].varName)), new ExprValue(this.GerVar(_tk[i].varName)));
				i -= 1;
			}
			this.Context.jmp(_tk[i].lblBreak);
		}
		public void Continue() {
			int i = _tk.Count - 1;
			while (_tk[i].lblContinue == null) {
				if (_tk[i].varName != null)
					Context.xor(this.GerVar(_tk[i].varName), new ExprValue(this.GerVar(_tk[i].varName)), new ExprValue(this.GerVar(_tk[i].varName)));
				i -= 1;
			}
			this.Context.jmp(_tk[i].lblContinue);
		}
		public void Return(ExprValue rr) {
			for (int i = _tk.Count - 1; i >= 0; --i) {
				if (_tk[i].varName != null) {
					if (rr != null && rr.IsReg && rr.reg == this.GerVar(_tk[i].varName)) continue;
					Context.xor(this.GerVar(_tk[i].varName), new ExprValue(this.GerVar(_tk[i].varName)), new ExprValue(this.GerVar(_tk[i].varName)));
				}
			}
		}
	}
}
