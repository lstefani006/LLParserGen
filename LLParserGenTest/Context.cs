using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace LLParserGenTest
{
	public class Context : IDisposable
	{
		private int _cnt_tmp;
		private int _cnt_lbl;
		private List<AssRoot> _ass = new List<AssRoot>();
		public string NewTmp() { return U.F("T{0}", ++_cnt_tmp); }
		public string NewLbl() { return U.F("L{0}", ++_cnt_lbl); }

		public Context() { }
		public void Dispose() { }

		public void add(string rd, string rs, string rt) { e(); _ass.Add(new AssI(this, emitNextLbl, OpCode.I_add, rd, rs, rt)); emitNextLbl = null; }
		public void sub(string rd, string rs, string rt) { e(); _ass.Add(new AssI(this, emitNextLbl, OpCode.I_sub, rd, rs, rt)); emitNextLbl = null; }
		public void mul(string rd, string rs, string rt) { e(); _ass.Add(new AssI(this, emitNextLbl, OpCode.I_mul, rd, rs, rt)); emitNextLbl = null; }
		public void div(string rd, string rs, string rt) { e(); _ass.Add(new AssI(this, emitNextLbl, OpCode.I_div, rd, rs, rt)); emitNextLbl = null; }

		public void jmp(string addr) { e(); _ass.Add(new AssJ(this, emitNextLbl, OpCode.J_jmp, addr)); emitNextLbl = null; }
		public void beq(string rs, string rt, string addr) { e(); _ass.Add(new AssB(this, emitNextLbl, OpCode.B_beq, rs, rt, addr)); emitNextLbl = null; }
		public void bne(string rs, string rt, string addr) { e(); _ass.Add(new AssB(this, emitNextLbl, OpCode.B_bne, rs, rt, addr)); emitNextLbl = null; }
		public void blt(string rs, string rt, string addr) { e(); _ass.Add(new AssB(this, emitNextLbl, OpCode.B_blt, rs, rt, addr)); emitNextLbl = null; }
		public void ble(string rs, string rt, string addr) { e(); _ass.Add(new AssB(this, emitNextLbl, OpCode.B_ble, rs, rt, addr)); emitNextLbl = null; }
		public void bgt(string rs, string rt, string addr) { e(); _ass.Add(new AssB(this, emitNextLbl, OpCode.B_bgt, rs, rt, addr)); emitNextLbl = null; }
		public void bge(string rs, string rt, string addr) { e(); _ass.Add(new AssB(this, emitNextLbl, OpCode.B_bge, rs, rt, addr)); emitNextLbl = null; }

		public void ld(string rs, int c) { e(); _ass.Add(new AssLd(this, emitNextLbl, rs, c)); emitNextLbl = null; }

		U.Set<string> emitNextLbl;
		public void emit(string lbl) { if (emitNextLbl == null) emitNextLbl = new U.Set<string>(); emitNextLbl.Add(lbl); }
		private void e() { if (emitNextLbl == null) { emitNextLbl = new U.Set<string>(); emitNextLbl.Add(NewLbl()); } }

		private Dictionary<string, string> _vars = new Dictionary<string, string>();
		private int vars_n = 0;
		public void AddDefVar(string name) { 
			if (_vars.ContainsKey(name) == true)
				throw new Exception(U.F("duplicated variable {0}", name));
			_vars[name] = U.F("r{0}", ++vars_n); 
		}
		public string GerVar(string name) { 
			if (_vars.ContainsKey(name) == false)
				throw new Exception(U.F("variable {0} not found", name));
			return _vars[name];
		}


		private void ComputeLive(int istart, int iend)
		{
			// capisco per ogni istruzione quale sono
			// le istruzioni che la seguono.
			// puo` essere quella immediatamente successiva 
			// o un jmp da qualche parte nel codice.
			for (int i = istart; i < iend; ++i)
				_ass[i].ComputeSucc(this);

			if (true)
			{
				for (var i = istart; i < iend; ++i)
				{
					var c = _ass[i];
					Console.WriteLine("{0,-3} {1, -10} {2}", i, c.Succ, c.ToString());
				}
			}

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
					bool b = c.ComputeLive(rout);
					if (b) changed = true;

					//Console.WriteLine(this);
				}
				//Console.WriteLine(this);
			} while (changed);
			//Console.WriteLine(this.ToString(istart, iend));
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

		private Graph ComputeGraph(int istart, int iend)
		{
			Graph gr = new Graph();

			for (int i = istart; i < iend; ++i)
			{
				var c = _ass[i];

				for (int j = 0; j < c.In.Count; ++j)
				{
					if (c.In[j].StartsWith("r"))
					{
						bool giaFissato = c.In[j].StartsWith("r");
						if (gr.ExistsNode(c.In[j]) == false)
							gr.CreateNode(c.In[j], giaFissato);
					}
				}
			}


			for (int i = istart; i < iend; ++i)
			{
				var c = _ass[i];

				for (int j = 0; j < c.In.Count; ++j)
				{
					if (gr.ExistsNode(c.In[j]) == true)
					{
						for (int k = j + 1; k < c.In.Count; ++k)
							if (gr.ExistsNode(c.In[k]) == true)
								gr.AddEdge(c.In[j], c.In[k]);
					}
				}
			}

			var rr = _ass[iend - 1].Out;
			for (int j = 0; j < rr.Count; ++j)
			{
				if (gr.ExistsNode(rr[j]) == true)
				{
					for (int k = j + 1; k < rr.Count; ++k)
					{
						if (gr.ExistsNode(rr[k]))
							gr.AddEdge(rr[j], rr[k]);
					}
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
				if (_ass[i] == ass)
					return _ass[i + 1];
			Debug.Assert(false);
			return null;
		}
		public AssRoot GetOp(string lbl)
		{
			for (var i = 0; i < _ass.Count; ++i)
				if (_ass[i].Lbl.Contains(lbl))
					return _ass[i];
			return null;
		}


		public bool GenerateCode(/*Function f*/ StmtRoot s)
		{
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

			s.GenCode(this);

			Console.WriteLine("####");
			Console.WriteLine("{0}", this.ToString());

			return true;

			int istart = 0;
			int iend = _ass.Count; ;

			this.ComputeLive(istart, iend);

			//Console.WriteLine("Codice generato per la funzione {0} {1}/{2}", f.name, istart, iend);
			Console.WriteLine("Live variables");
			Console.WriteLine(this.ToString(istart, iend));

			var gr = this.ComputeGraph(istart, iend);

			Console.WriteLine("Grafo");
			Console.WriteLine(gr);

			bool ok = false;
			int k;
			for (k = 0; k < 32; ++k)
				if (gr.Color(k))
				{
					Console.WriteLine("Ci vogliono k={0} da r0 a r{1}, prossimo per var locali/parametri r{0}", k, k - 1);
					var regs = gr.GetRegs();
					this.SetTemps(istart, iend, regs);
					ok = true;
					break;
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

			return ok;
		}
	}
}