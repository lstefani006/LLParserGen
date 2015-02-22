using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace LLParserGenTest
{
	public class Context : IDisposable {

		public void Dispose() {}

		public Context() {}

		public void add(string rd, string rs, string rt) { _ass.Add(new MipsX(OpCode.I_add, rd, rs, rt)); }

		private List<AssRoot> _ass = new List<AssRoot>();

		private void ComputeLive(int istart, int iend) {
			// capisco per ogni istruzione quale sono
			// le istruzioni che la seguono.
			// puo` essere quella immediatamente successiva 
			// o un jmp da qualche parte nel codice.
			for (int i = istart; i < iend; ++i)
				_ass[i].ComputeSucc(i, this);


			if (false) {
				for (var i = istart; i < iend; ++i) {
					var c = _ass[i];
					Console.WriteLine("{0,-3} {1, -10} {2}", i, c.Succ, c.ToString());
				}
			}

			bool changed;
			do {
				changed = false;
				for (int i = iend - 1; i >= istart; --i) {
					var c = _ass[i];

					// cerco gli stmt 
					U.Set<string> rout = new U.Set<string>();
					foreach (var t in c.Succ)
						rout = rout + _ass[t].In;

					bool b = c.ComputeLive(rout);
					if (b) changed = true;

					//Console.WriteLine(this);
				}
				//Console.WriteLine(this);
			} while (changed);
			//Console.WriteLine(this.ToString(istart, iend));
		}

		public string ToString(int istart, int iend) {
			string r = "";
			for (int i = istart; i < iend; ++i) {
				var v = _ass[i];
				r += v.ToString() + "\n";
			}

			if (true) {
				r += "[";
				bool first = true;
				foreach (var v in _ass[_ass.Count - 1].Out) {
					if (first == false) r += ", ";
					first = false;
					r += v;
				}
				r += "]\n";
			}

			return r;
		}

		public override string ToString() {
			return ToString(0, _ass.Count);
		}

		private Graph ComputeGraph(int istart, int iend, string prefix) {
			Graph gr = new Graph();

			for (int i = istart; i < iend; ++i) {
				var c = _ass[i];

				for (int j = 0; j < c.In.Count; ++j) {
					if (c.In[j].StartsWith(prefix) || c.In[j].StartsWith("r")) {
						bool giaFissato = c.In[j].StartsWith("r");

						if (gr.ExistsNode(c.In[j]) == false)
							gr.CreateNode(c.In[j], giaFissato);
					}
				}
			}


			for (int i = istart; i < iend; ++i) {
				var c = _ass[i];

				for (int j = 0; j < c.In.Count; ++j) {
					if (gr.ExistsNode(c.In[j]) == true) {
						for (int k = j + 1; k < c.In.Count; ++k)
							if (gr.ExistsNode(c.In[k]) == true)
								gr.AddEdge(c.In[j], c.In[k]);
					}
				}
			}

			var rr = _ass[iend - 1].Out;
			for (int j = 0; j < rr.Count; ++j) {
				if (gr.ExistsNode(rr[j]) == true) {
					for (int k = j + 1; k < rr.Count; ++k) {
						if (gr.ExistsNode(rr[k]))
							gr.AddEdge(rr[j], rr[k]);
					}
				}
			}

			return gr;
		}

		private void SetTemps(int istart, int iend, Dictionary<string, string> regs) {
			for (int i = istart; i < iend; ++i) {
				var stmt = _ass[i];
				foreach (var t in regs)
					stmt.Substitute(t.Key, t.Value);
			}
		}


		public int GetAddrFromLabel(string lbl) {
			for (int i = 0; i < _ass.Count; ++i)
				if (_ass[i].Lbl.Contains(lbl))
					return i;
			return _ass.Count;
		}

		public bool GenerateCode(/*Function f*/) {
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
			int istart = 0;
			int iend = 0;

			this.ComputeLive(istart, iend);

			//Console.WriteLine("Codice generato per la funzione {0} {1}/{2}", f.name, istart, iend);
			Console.WriteLine("Live variables");
			Console.WriteLine(this.ToString(istart, iend));

			var gr = this.ComputeGraph(istart, iend, "$");

			Console.WriteLine("Grafo");
			Console.WriteLine(gr);

			bool ok = false;
			int k;
			for (k = 0; k < 32; ++k)
				if (gr.Color(k)) {
					Console.WriteLine("Ci vogliono k={0} da r0 a r{1}, prossimo per var locali/parametri r{0}", k, k - 1);
					var regs = gr.GetRegs();
					this.SetTemps(istart, iend, regs);
					ok = true;
					break;
				}

			if (ok == false) {
				Console.WriteLine("Cannot allocate registers");
				return false;
			}

			if (true) {
				// calcolo il maggiore dei registri r utilizzati (es r4)
				// c0 iniziera` con r5

				var rrr = new Dictionary<string, string>();
				for (int ci = 0; ci < 1024; ci++) {
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