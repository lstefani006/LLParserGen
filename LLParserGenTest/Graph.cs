using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace LLParserGenTest
{

	class Graph {
		public Graph() {
		}

		readonly List<NodeReg> _nodes = new List<NodeReg>();

		public NodeReg CreateNode(string name, string reg) {
			Debug.Assert(ExistsNode(name) == false);

			NodeReg n = new NodeReg(name, reg);
			_nodes.Add(n);
			return n;
		}

		public NodeReg GetNode(string name) {
			var nn = _nodes.Find(r => r.Name == name);
			Debug.Assert(nn != null);
			return nn;
		}

		public bool ExistsNode(string name) {
			return _nodes.Find(r => r.Name == name) != null;
		}

		public void AddEdge(string a, string b) {
			Debug.Assert(ExistsNode(a));
			Debug.Assert(ExistsNode(b));

			NodeReg na = GetNode(a);
			NodeReg nb = GetNode(b);

			na.AddEdge(nb);
			nb.AddEdge(na);
		}

		public override string ToString() {
			string r = "";
			foreach (var n in _nodes) {
				r += n.ToString();
				r += "\n";
			}
			return r;
		}


		public Graph Color(int k) {
			var st = new Stack<string>();
			return Color(k, st);
		}

		Graph Color(int k, Stack<string> st) {

			// riduco il grafo this scegliendo un nodo da togliere dallo stesso
			// grafo ed ottenendo un grafo più piccolo.... finchè non ci
			// sono più nodi da togliere.
			if (true) {
				// rimuovo dal grafo un nodo nodo
				// il nodo da rimuovere deve avere meno di k vicini
				Graph gr = this;
				while (gr._nodes.Count > 0) {
					NodeReg nd = gr._nodes.Find(n => n.Neighbors.Count < k);
					if (nd == null) {
						// ci è andata male con la ricerca del nodo con meno di k vicini.
						//
						// proviamo con l'optimistic coloring
						// scegliamo un nodo qualunque, sperando che i suoi vicini
						// vengano assegnati a registri in comune.
						//
						// cerco il nodo nel grafo che ha meno vicini
						var q = from nn in gr._nodes
						       orderby nn.Neighbors.Count ascending
						       select nn;

						nd = q.FirstOrDefault();
						if (nd == null)
							return null;
					}

					// metto il nodo rimosso nello stack
					st.Push(nd.Name);
					// creo un nuovo grafo rimuovendo il nodo
					// il nuovo grafo è una copia profonda del vecchio grafo
					gr = gr.Remove(nd);
				}
			}

			if (true) {
				// a questo punto provo a colorare i nodi.
				// a meno che un nodo non sia già colorato
				// scelgo un colore non utilizzati dai suoi vicini.
				// Questa operazione può fallire non caso di optimistic coloring
				Graph gr = this.Clone();
				while (st.Count > 0) {
					var nodeName = st.Pop();

					var nd = gr.GetNode(nodeName);

					// se inizia per r e` un registro gia` assegnato
					// i registri da assegnare iniziano per Tn
					if (nd.Reg != null)
						continue;

					// determino la lista <rg> dei registri gia' assegnati ai vicini
					U.Set<string> rg = new U.Set<string>();
					foreach (var nv in nd.Neighbors)
						if (nv.Reg != null)
							rg.Add(nv.Reg);

					// se i registri gia' assegnati superano il numero dei registri disponibili
					// vuol dire che non abbiamo posto per un nuovo registro....
					if (rg.Count >= k) {
						// siamo nel caso dell'optimistic coloring, ma ci e` andata male

						// resetto i nodi colorati.
						return null;
					}

					// fra tutti i registri disponibili 
					// (ossia tra quelli non assegnati gia' ai vicini) 
					// prendo il primo disponibile
					for (int i = 0; i < k; ++i) {
						string r = U.F("r{0}", i);
						if (rg.Contains(r) == false) {
							nd.Reg = r;
							break;
						}
					}

					Debug.Assert(nd.Reg != null);
				}

				return gr;
			}
		}

		/// <summary>
		/// creo un nuovo grafo ottenuto dal grafo corrente rimuovendo il nono src
		/// </summary>
		/// <param name="nd">Nodo da rimuovere.</param>
		Graph Remove(NodeReg nd) {
			Graph gr = new Graph();
			foreach (var s in this._nodes) {
				if (s.Name == nd.Name) continue;

				// creo il nodo nel nuovo grafo
				if (gr.ExistsNode(s.Name) == false)
					gr.CreateNode(s.Name, s.OriReg);

				// e tutti gli altri nodi, purche non siano il nodo di ingresso.
				foreach (var t in s.Neighbors) {
					if (t.Name != nd.Name) {
						if (gr.ExistsNode(t.Name) == false)
							gr.CreateNode(t.Name, t.OriReg);
						gr.AddEdge(s.Name, t.Name);
					}
				}
			}
			return gr;
		}

		/// <summary>
		/// creo un nuovo grafo copia profonda
		/// </summary>
		Graph Clone() {
			Graph gr = new Graph();
			foreach (var s in this._nodes) {
				// creo il nodo nel nuovo grafo
				if (gr.ExistsNode(s.Name) == false)
					gr.CreateNode(s.Name, s.OriReg);

				foreach (var t in s.Neighbors) {
					if (gr.ExistsNode(t.Name) == false)
						gr.CreateNode(t.Name, t.OriReg);
						gr.AddEdge(s.Name, t.Name);
				}
			}
			return gr;
		}


		public Dictionary<string, string> GetRegs() {
			Dictionary<string, string> ret = new Dictionary<string, string>();
			foreach (var n in _nodes)
				if (n.Name != n.Reg)
					ret[n.Name] = n.Reg;
			return ret;
		}
	}

	class NodeReg {
		public NodeReg(string name, string reg) {
			this._name = name;
			this._oriReg = reg;
			this._neighbors = new List<NodeReg>();
			this.Reg = reg;
		}
			
		public void AddEdge(NodeReg nd) {
			if (_neighbors.Contains(nd) == false)
				_neighbors.Add(nd);
		}

		public override string ToString() {
			string r = this.Name;
			if (this.Reg != null && this.Name != this.Reg) r += "/" + this.Reg;
			r += " :";
			foreach (var k in this._neighbors) {
				r += " " + k.Name;
				if (k.Reg != null && k.Reg != k.Name) r += "/" + k.Reg;
			}
			return r;
		}

		public List<NodeReg> Neighbors { get { return _neighbors; } }

		public string Name { get { return _name; } }

		public string Reg {
			get;
			set;
		}
		public string OriReg {
			get { return _oriReg; }
		}

		readonly string _name;
		readonly string _oriReg;
		readonly List<NodeReg> _neighbors;
	};
}
