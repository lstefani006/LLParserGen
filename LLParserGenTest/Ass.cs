using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LLParserGenTest
{
	public enum OpCode {
		X_sti, X_stiu,
		X_ldi,
		X_ldiu,
		J_ret,
		J_js,

	
		I_lw,
		I_lh,
		I_lb,
		I_lhu,
		I_lbu,
		I_sw,
		I_sh,
		I_sb,
		I_lui,
		I_beq,
		I_bne,
		I_bgt,
		I_bge,
		I_blt,
		I_ble,

	
		I_mvw,
		I_lr,
		I_sr,
	}

	abstract class MipsAssembly {

		protected U.Set<string> _in = new U.Set<string>();
		protected U.Set<string> _out = new U.Set<string>();
		protected static U.Set<string> S_lbl = new U.Set<string>();
		protected U.Set<string> _lbl = new U.Set<string>();
		protected U.Set<int> _prev = new U.Set<int>();

		public U.Set<int> Succ = new U.Set<int>();

		public static void SetLabel(string lbl) {
			S_lbl.Add(lbl);
		}

		protected MipsAssembly() {
			_lbl = S_lbl;
			S_lbl = new U.Set<string>();
		}

		public abstract bool ComputeLive(U.Set<string> rout);
		public abstract void Substitute(string temp, string reg);
		public abstract U.Set<int> GetSucc(int pc, Context ctx);

		protected string InToString() {
			string r = "[";
			bool first = true;
			for (int i = 0; i < _in.Count; ++i) {
				if (first == false)
					r += ", ";
				first = false;
				r += _in[i];
			}
			r = r + "] ";

			r = r.PadRight(35);

			return r + U.F("{0,-6}", this._lbl);
		}

		public U.Set<string> In { get { return _in; } }
		public U.Set<string> Out { get { return _out; } }
		public U.Set<string> Lbl { get { return this._lbl; } }
	}

	class MipsX : MipsAssembly {
		OpCode op;
		string rd;
		string rt;
		string rs;

		public MipsX(OpCode op, string rd, string rt, string rs) {
			this.op = op;

			this.rd = rd;
			this.rt = rt;
			this.rs = rs;
		}

		public override bool ComputeLive(U.Set<string> prev) {
			// in = (out - def) u use
			//
			// out variabili vive dopo l'istruzione
			// def variabili definite (scritte) nell'istruzione
			// use variabili argomenti (lette) nell'instruzione

			var rout = new U.Set<string>(prev);
			var rin = new U.Set<string>(prev);

			if (op == OpCode.X_sti || op == OpCode.X_stiu) {
				// rd e` un ingresso non una uscita !!!
				rin.Add(rs);
				rin.Add(rt);
				rin.Add(rd);
			} else {
				rin.Remove(rd);
				rin.Add(rs);
				rin.Add(rt);
			}

			bool changed = false;
			if (rin != _in || rout != _out)
				changed = true;

			_in = rin;
			_out = rout;

			return changed;
		}

		public override U.Set<int> GetSucc(int pc, Context ctx) {
			return new U.Set<int>() { pc + 1 };
		}


		public override string ToString() {
			string f = Enum.GetName(typeof(OpCode), op).Substring(2);
			switch (op) {
			case OpCode.X_sti:
			case OpCode.X_stiu:
				{
					string r = U.F("{0,-6} [{2}, #{4}*{3}, #{5}], {1}", f, rd, rt, rs/*, 1 << kk, imm*/);
					return U.F("{0} {1}", InToString(), r);
				}
			case OpCode.X_ldi:
			case OpCode.X_ldiu:
				{
					string r = U.F("{0,-6} {1}, [{2}, #{4}*{3}, #{5}]", f, rd, rt, rs/*, 1 << kk, imm*/);
					return U.F("{0} {1}", InToString(), r);
				}

			default:
				Debug.Assert(false);
				return null;
			}
		}

		public override void Substitute(string temp, string reg) {
			if (rd == temp) rd = reg;
			if (rs == temp) rs = reg;
			if (rt == temp) rt = reg;
		}

	}

	class MipsR : MipsAssembly {
		OpCode op;
		string rd, rs, rt;

		public MipsR(OpCode op, string rd, string rs, string rt) {
			this.op = op;

			this.rd = rd;
			this.rs = rs;
			this.rt = rt;
		}


		public override bool ComputeLive(U.Set<string> prev) {
			var rout = new U.Set<string>(prev);
			var rin = new U.Set<string>(prev);

			rin.Remove(rd);
			rin.Add(rs);
			rin.Add(rt);

			bool changed = false;
			if (_in != rin || _out != rout)
				changed = true;

			_in = rin;
			_out = rout;

			return changed;
		}
		/*
		public override string ToString() {
			string f = Enum.GetName(typeof(Func_R), func);
			string r;

			switch (func) {
			case Func_R.sll:
			case Func_R.srl:
			case Func_R.sra:
				r = U.F("{0,-6} {1}, {2}, #{3}", f, rd, rt, shmat);
				break;

			case Func_R.jr:
			case Func_R.js:
				r = U.F("{0,-6} {1}", f, rd);
				break;

			default:
				r = U.F("{0,-6} {1}, {2}, {3}", f, rd, rt, rs);
				break;
			}

			return U.F("{0} {1}", InToString(), r);
		}
*/

		public override void Substitute(string temp, string reg) {
			if (rd == temp) rd = reg;
			if (rs == temp) rs = reg;
			if (rt == temp) rt = reg;
		}

		public override U.Set<int> GetSucc(int pc, Context ctx) {
			//if (func == Func_R.jr)
			//	Debug.Assert(false, "Non si puo` calcolare il next di un j r2");

			// notare che invece Func_R.js ha con succ pc+1
			return new U.Set<int>() { pc + 1 };
		}
	}

	class MipsI : MipsAssembly, IEquatable<MipsI> {
		OpCode op;
		string rs, rt;
		int C;
		string lbl;

		public MipsI(OpCode op, string rs, string rt, int C) {
			this.op = op;
			this.rs = rs;
			this.rt = rt;
			this.C = C;
		}

		public MipsI(OpCode op, string rs, string rt, string lbl) {
			this.op = op;
			this.rs = rs;
			this.rt = rt;
			this.lbl = lbl;
		}

		public override bool ComputeLive(U.Set<string> prev) {
			var rout = new U.Set<string>(prev);
			var rin = new U.Set<string>(prev);
			switch (op) {
			case OpCode.I_sw:
			case OpCode.I_sh:
			case OpCode.I_sb:
				rin.Add(rs);
				rin.Add(rt);
				break;

			case OpCode.I_beq:
			case OpCode.I_bne:
			case OpCode.I_bgt:
			case OpCode.I_bge:
			case OpCode.I_blt:
			case OpCode.I_ble:
				rin.Add(rs);
				rin.Add(rt);
				break;

			case OpCode.I_mvw:
				break;

			case OpCode.I_lr:
				rin.Add(rs);
				rin.Add(lbl);
				rin.Remove(rt);
				break;

			case OpCode.I_sr:
				rin.Add(rt);
				rin.Add(rs);
				rin.Remove(lbl);
				break;

			default:
				rin.Remove(rt);
				rin.Add(rs);
				break;
			}
			bool changed = false;
			if (_in != rin || _out != rout)
				changed = true;

			_in = rin;
			_out = rout;

			return changed;
		}

		public override U.Set<int> GetSucc(int pc, Context ctx) {
			var ret = new U.Set<int>() { pc + 1 };
			if (op == OpCode.I_beq || op == OpCode.I_bne ||
			     op == OpCode.I_bgt || op == OpCode.I_bge ||
			     op == OpCode.I_blt || op == OpCode.I_ble) {
				if (this.lbl != null)
					ret.Add(ctx.GetAddrFromLabel(this.lbl));
				else
					ret.Add(this.C);
			}

			return ret;
		}

		public override string ToString() {
			string s = Enum.GetName(typeof(OpCode), op);
			s = s.Remove(0, 2); // tolgo I_

			string r;
			switch (op) {
			case OpCode.I_lw:
			case OpCode.I_lh:
			case OpCode.I_lb:
			case OpCode.I_lhu:
			case OpCode.I_lbu:
					// U[rt] = *(uint*)((byte*)mem + U[rs] + (uint)C);
				r = U.F("lw     {2}, [{0}, {1}]", rs, C, rt);
				break;

			case OpCode.I_sw:
			case OpCode.I_sh:
			case OpCode.I_sb:
					// *(uint*)((byte*)mem + U[rs] + (uint)C) = U[rt];
				r = U.F("sw     [{0}, {1}], {2}", rs, C, rt);
				break;

			case OpCode.I_lui:
				r = U.F("lui    {0} #{1}", rt, C);
				break;

			case OpCode.I_beq:
			case OpCode.I_bne:
			case OpCode.I_bgt:
			case OpCode.I_bge:
			case OpCode.I_blt:
			case OpCode.I_ble:
				{
					if (this.lbl == null)
						r = U.F("{0,-6} {1}, {2}, {3}", s, rt, rs, C);
					else
						r = U.F("{0,-6} {1}, {2}, {3}", s, rt, rs, lbl);
				}
				break;

			case OpCode.I_mvw:
				{
					r = U.F("{0,-6} {1}", s, this.C);
				}
				break;

			case OpCode.I_lr:
				if (this.lbl == null)
					r = U.F("{0,-6} {1}, r[{2}, {3}]", s, rt, this.C, rs);
				else
					r = U.F("{0,-6} {1}, r[{2}, {3}]", s, rt, this.lbl, rs);
				break;
			case OpCode.I_sr:
				if (this.lbl == null)
					r = U.F("{0,-6} r[{2}, {3}], {1}", s, rt, this.C, rs);
				else
					r = U.F("{0,-6} r[{2}, {3}], {1}", s, rt, this.lbl, rs);
				break;

			default:
				{
					if (this.lbl == null)
						r = U.F("{0,-6} {1}, {2}, #{3}", s, rt, rs, C);
					else
						r = U.F("{0,-6} {1}, {2}, #{3}", s, rt, rs, lbl);
				}
				break;
			}
			return U.F("{0} {1}", InToString(), r);
		}

		public override void Substitute(string temp, string reg) {
			if (rs == temp) rs = reg;
			if (rt == temp) rt = reg;
		}

		public void SetC(int C) {
			this.C = C;
			this.lbl = null;
		}

		public bool Equals(MipsI other) {
			return this == other;
		}

		public string Window { get { return this.lbl; } }
	}

	class MipsJ : MipsAssembly, IEquatable<MipsJ> {
		OpCode op;
		int C;
		string lbl;
		U.Set<string> rUsed;
		U.Set<string> rOut;

		public MipsJ(OpCode op, int C) {
			this.op = op;
			this.C = C;
		}

		public MipsJ(OpCode op, string lbl, U.Set<string> rUsed, U.Set<string> rOut) {
			this.op = op;
			this.lbl = lbl;
			this.rUsed = rUsed;
			this.rOut = rOut;
		}

		public override bool ComputeLive(U.Set<string> prev) {
			if (op == OpCode.J_ret) {
				//prev.Add(MipsAssembly.regZero);
				//prev.Add(MipsAssembly.regWin);
				//prev.Add(MipsAssembly.regJs);
				//prev.Add(MipsAssembly.regReserved);
			}

			var rout = new U.Set<string>(prev);
			var rin = new U.Set<string>(prev);

			if ((object)rOut != null)
				foreach (var r in this.rOut)
					rin.Remove(r);

			if ((object)rUsed != null)
				foreach (var r in this.rUsed)
					rin.Add(r);




			bool changed = false;
			if (_in != rin || _out != rout)
				changed = true;

			_in = rin;
			_out = rout;
			return changed;
		}

		public override string ToString() {
			string r = Enum.GetName(typeof(OpCode), this.op).Remove(0, 2);
			if (op == OpCode.J_ret) {
				return U.F("{0} {1,-6}", InToString(), r);
			} else {
				if (string.IsNullOrEmpty(this.lbl))
					return U.F("{0} {1,-6} {2}", InToString(), r, C);
				else
					return U.F("{0} {1,-6} {2}", InToString(), r, lbl);
			}
		}

		public override void Substitute(string temp, string reg) {
		}

		public override U.Set<int> GetSucc(int pc, Context ctx) {
			if (op == OpCode.J_ret) return new U.Set<int>();
			if (op == OpCode.J_js) return new U.Set<int>() { pc + 1 };
			return new U.Set<int>() { ctx.GetAddrFromLabel(this.lbl) };
		}


		public bool Equals(MipsJ other) {
			return this == other;
		}
	}
}


