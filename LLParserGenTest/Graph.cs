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

		public NodeReg CreateNode(string reg, bool giaAssegnato) {
			Debug.Assert(ExistsNode(reg) == false);

			NodeReg n = new NodeReg(reg, giaAssegnato);
			_nodes.Add(n);
			return n;
		}

		public NodeReg GetNode(string reg) {
			var nn = _nodes.Find(r => r.Name == reg);
			if (nn != null)
				return nn;
			Debug.Assert(false);
			return null;
		}

		public bool ExistsNode(string reg) {
			return _nodes.Find(r => r.Name == reg) != null;
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


		public bool Color(int k) {
			var st = new Stack<string>();
			return Color(k, st);
		}

		bool Color(int k, Stack<string> st) {
			// rimuovo dal grafo un nodo nodo
			// il nodo da rimuovere deve avere meno di k vicini
			Graph gr = this;
			while (gr._nodes.Count > 0) {
				NodeReg nd = gr._nodes.Find(n => n.Neighbors.Count < k);
				if (nd == null) {
					// proviamo con l'optimistic coloring
					// scegliamo un nodo qualunque, sperando che i suoi vicini
					// vengano assegnati a registri in comune.

					// cerco il nodo nel grafo che ha meno vicini
					var q = from nn in gr._nodes
					        orderby nn.Neighbors.Count ascending
					        select nn;

					nd = q.FirstOrDefault();
					if (nd == null)
						return false;
				}

				st.Push(nd.Name);
				gr = gr.Remove(nd);
			}

			while (st.Count > 0) {
				var nd = st.Pop();

				// se inizia per r e` un registro gia` assegnato
				// i registri da assegnare iniziano per Tn
				if (nd.StartsWith("r"))
					continue;

				// assegno un registro non assegnato gia` ai vicini
				// rg = lista dei registri assegnati ai vicini
				U.Set<string> rg = new U.Set<string>();
				foreach (var nv in this.GetNode(nd).Neighbors)
					if (nv.Reg != null)
						rg.Add(nv.Reg);

				if (rg.Count >= k) {
					// siamo nel caso dell'optimistic coloring, ma ci e` andata male
					return false;
				}

				// fra tutti i registri disponibili prendo il primo
				// non assegnato ai vicini.
				for (int i = 0; i < 32; ++i) {
					string r = U.F("r{0}", i);
					if (rg.Contains(r) == false) {
						this.GetNode(nd).Reg = r;
						break;
					}
				}

				Debug.Assert(this.GetNode(nd).Reg != null);
			}

			return true;
		}

		Graph Remove(NodeReg src) {
			Graph gr = new Graph();
			foreach (var s in this._nodes) {
				if (s == src) continue;

				// creo il nodo nel nuovo grafo
				if (gr.ExistsNode(s.Name) == false)
					gr.CreateNode(s.Name, s.Name.StartsWith("r"));

				// e tutti gli altri nodi, purche non siano il nodo di ingresso.
				foreach (var s_neighbor in s.Neighbors) {
					if (s_neighbor.Name != src.Name) {
						if (gr.ExistsNode(s_neighbor.Name) == false)
							gr.CreateNode(s_neighbor.Name, s.Name.StartsWith("r"));
						gr.AddEdge(s.Name, s_neighbor.Name);
					}
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
		public NodeReg(string rg, bool giaFissato) {
			this._name = rg;
			this._neighbors = new List<NodeReg>();

			if (giaFissato)
				this.Reg = rg;
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

		readonly string _name;
		List<NodeReg> _neighbors;
	};
}