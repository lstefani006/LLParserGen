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

		private Dictionary<string, string> _vars = new Dictionary<string, string>();
		public void AddDefVar(string name) { 
			if (_vars.ContainsKey(name) == true)
				throw new Exception(U.F("duplicated variable {0}", name));
			_vars[name] = NewTmp();
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

		Stack<StackData> _tk = new Stack<StackData>();
		public struct StackData {
			public StmtTk tk;
			public string lblBreak;
			public string lblContinue;
			public string varName;
		}
		public enum StmtTk {
			While, For,
			Var
		}
		public void Push(StmtTk tk, string lblBreak, string lblContinue, string vv)
		{
			StackData r = new StackData();
			r.tk = tk;
			r.lblBreak = lblBreak;
			r.lblContinue = lblContinue;
			r.varName = vv;
			_tk.Push(r);
		}
		public void Pop() { _tk.Pop(); }

		public void Break() {
			while (_tk.Count > 0) {
				var r = _tk.Pop();
				if (r.tk == StmtTk.While || r.tk == StmtTk.For)  {
					jmp(r.lblBreak);
					return;
				} else if (r.tk == StmtTk.Var) {
					ld(r.varName, 0);
				}
			}
			throw new ApplicationException("continue");
		}


		public bool GenerateCode(Fun f) {
			/*StartFunction(f);

			foreach (var v in f.args)
				AddParameter(v.Item1, v.Item2);

			int istart = _ass.Count;
			L(f.name.v);
			f.s.GenerateCode(this);
			if (f.IsVoid)
				J(OpCode.J_ret);
			int iend = _ass.Count;
			*/
			foreach (var a in f.args.args)
				this.AddArgVar(a);

			this.emit(f.name);
			f.body.GenCode(this);

			if (_ass.Count > 0) {
			
				foreach (var a in f.args.args)
					_ass[_ass.Count - 1].Out.Add(a);
			}
			Console.WriteLine("{0}", this.ToString());

			int istart = 0;
			int iend = _ass.Count;
			var live = new List<string>();

			// le var in ingresso anche se non servono più non vengono sovrascritte
			// da altre variabili (se invece si vuole ottimizzare al max si può omettere il Foreach).
			f.args.args.ForEach(v => live.Add(this.GerVar(v)));
			this.ComputeLive(istart, iend, live);

			Console.WriteLine("Codice generato per la funzione {0} {1}/{2}", f.name, istart, iend);
			Console.WriteLine("Live variables");
			Console.WriteLine(this.ToString(istart, iend));

			var gr = this.CreateGraph(istart, iend);
			Console.WriteLine("Grafo");
			Console.WriteLine(gr);

			bool ok = false;
			int k;
			for (k = 0; k < 32; ++k) {
				var col = gr.Color(k);
				if (col != null) {
					Console.WriteLine("Ci vogliono k={0} da r0 a r{1}, prossimo per var locali/parametri r{0}", k, k - 1);
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

			if (true)
			{
				// calcolo il maggiore dei registri r utilizzati (es r4)
				// c0 iniziera` con r5

				var rrr = new Dictionary<string, string>();
				for (int ci = 0; ci < 1024; ci++)
				{
					string cc = U.F("c{0}", ci);
					string ff = U.F("r{0}", ci + k);
					rrr[cc] = ff;
				}
				this.SetTemps(istart, iend, rrr);

				// ora assegno i valori del wnd
				/*foreach (MipsI t in _sw) {
					Debug.Assert(t.Window.StartsWith("c"));
					string w = rrr[t.Window];
					Debug.Assert(w.StartsWith("r"));
					int b = int.Parse(w.Substring(1), System.Globalization.CultureInfo.InvariantCulture);
					t.SetC(b);
				}*/

			}

			Console.WriteLine(this.ToString(istart, iend));
			return ok;
		}
	}
}
