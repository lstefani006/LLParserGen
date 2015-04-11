using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LLParserGenTest
{
	/*
	 * r0-r14-rp
	 * 
	 * add r2, r3, r4
	 * add r2, r3, 232
	 * add r2, 53, r4
	 *  
	 * ld  r3, 345
	 * ld  r3, b1[r4]
	 * ld  r3, b1[r4 + off]
	 * ld  r3, b1[r4 + off + r5*1]
	 * ld  r3, b2[r4 + off + r5*2]
	 * ld  r3, b4[r4 + off + r5*4]
	 * 
	 * beq r2, r5,  lbl
	 * beq r2, 434, lbl
	 * beq 34, r4,  lbl
	 * br  lbl
	 * js  lbl
	 * js  obj-addr, id-int, off
	 * 
	 */
	public enum OpCode {
		/*
		X_sti, X_stiu,
		X_ldi,
		X_ldiu,
		J_ret,
		J_js,
		*/

		I_add,
		I_sub,
		I_mul,
		I_div,
		I_rem,
		I_and,
		I__or,
		I_xor,
		I_shl,
		I_shr,

		I_add_rc,
		I_sub_rc,
		I_mul_rc,
		I_div_rc,

		I_add_cr,
		I_sub_cr,
		I_mul_cr,
		I_div_cr,

		J_jmp,
		J_js,
		J_ret,

		B_beq_rr,
		B_bne_rr,
		B_bgt_rr,
		B_bge_rr,
		B_blt_rr,
		B_ble_rr,

		B_beq_rc,
		B_bne_rc,
		B_bgt_rc,
		B_bge_rc,
		B_blt_rc,
		B_ble_rc,

		B_beq_cr,
		B_bne_cr,
		B_bgt_cr,
		B_bge_cr,
		B_blt_cr,
		B_ble_cr,

		L_ld

	
		/*
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
		*/
	}

	public abstract class AssRoot : IEquatable<AssRoot> {

		protected U.Set<string> _in = new U.Set<string>();
		protected U.Set<string> _out = new U.Set<string>();
		protected U.Set<string> _lbl = new U.Set<string>();

		public U.Set<AssRoot> _succ;

		public U.Set<AssRoot> Succ { get { return _succ; } }

		protected AssRoot(Context ctx, U.Set<string> lbl) {
			_lbl = lbl;
		}

		public bool Equals(AssRoot other) {
			foreach (var a in this._lbl)
				foreach (var b in other._lbl)
					if (a == b) return true;
			return false;
		}

		public abstract void ComputeSucc(Context ctx);

		public abstract bool ComputeLive(U.Set<string> prev);

		public abstract void Substitute(string temp, string reg);

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

	class Op_rxx : AssRoot {
		readonly OpCode op;
		string rd;
		readonly ExprValue rt;
		readonly ExprValue rs;

		public Op_rxx(Context ctx, U.Set<string> lbl, OpCode op, string rd, ExprValue rt, ExprValue rs)
			: base(ctx, lbl) {
			Debug.Assert(op == OpCode.I_add || op == OpCode.I_sub || op == OpCode.I_mul || op == OpCode.I_div);
			Debug.Assert(rd != null);
			Debug.Assert(rs != null);
			Debug.Assert(rt != null);

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

			if (true) {
				// rd is written  ==> is not live before this instruction
				// rs/rt are read ==> they must be live for this instruction
				rin.Remove(rd);
				if (rs.IsReg) rin.Add(rs.reg);
				if (rt.IsReg) rin.Add(rt.reg);
			}

			bool changed = (rin != _in || rout != _out);
			_in = rin;
			_out = rout;

			return changed;
		}

		public override void ComputeSucc(Context ctx) {
			_succ = new U.Set<AssRoot>();
			var succ = ctx.GetSuccOp(this);
			if (succ != null) _succ.Add(succ);
		}


		public override string ToString() {
			string f = Enum.GetName(typeof(OpCode), op).Substring(2);
			var idx = f.IndexOf('_');
			if (idx > 0) f = f.Substring(0, idx);
			string r = U.F("{0,-6} {1}, {2}, {3}", f, rd, rt, rs);
			return U.F("{0} {1}", InToString(), r);
		}

		public override void Substitute(string temp, string reg) {
			if (rd == temp) rd = reg;
			if (rs.IsReg && rs.reg == temp) rs.SetReg(reg);
			if (rt.IsReg && rt.reg == temp) rt.SetReg(reg);
		}
	}

	class AssJ : AssRoot {
		OpCode op;
		string addr;
		string rd;

		public AssJ(Context ctx, U.Set<string> lbl, OpCode op, string addr)
			: this(ctx, lbl, op, null, addr) {
		}
		public AssJ(Context ctx, U.Set<string> lbl, OpCode op, string rd, string addr)
			: base(ctx, lbl) {
			Debug.Assert(op == OpCode.J_jmp || op == OpCode.J_js);

			this.op = op;
			this.rd = rd;
			this.addr = addr;
		}

		public override bool ComputeLive(U.Set<string> prev) {
			// in = (out - def) u use
			//
			// out variabili vive dopo l'istruzione
			// def variabili definite (scritte) nell'istruzione
			// use variabili argomenti (lette) nell'instruzione

			var rout = new U.Set<string>(prev);
			var rin = new U.Set<string>(prev);

			if (true) {
				if (rd != null) rin.Remove(rd);
			}

			bool changed = (rin != _in || rout != _out);
			_in = rin;
			_out = rout;

			return changed;
		}

		public override void ComputeSucc(Context ctx) {
			switch (op) {
			case OpCode.J_jmp: 
				_succ = new U.Set<AssRoot>() { ctx.GetOp(this.addr) }; 
				break;

			case OpCode.J_js: 
				_succ = new U.Set<AssRoot>() { }; 
				var succ = ctx.GetSuccOp(this);
				if (succ != null) _succ.Add(succ);
				break;

			default: 
				Debug.Assert(false); 
				break;
			}
		}


		public override string ToString() {
			string f = Enum.GetName(typeof(OpCode), op).Substring(2);
			if (rd == null) {
				string r = U.F("{0,-6} {1}", f, this.addr);
				return U.F("{0} {1}", InToString(), r);
			} else {
				string r = U.F("{0,-6} {1}, {2}", f, this.rd, this.addr);
				return U.F("{0} {1}", InToString(), r);
			}
		}

		public override void Substitute(string temp, string reg) {
			if (rd != null && rd == temp) rd = reg;
		}
	}

	class Op_Ret: AssRoot {
		readonly ExprValue rt;
		readonly OpCode op;

		public Op_Ret(Context ctx, U.Set<string> lbl, ExprValue rt)
			: base(ctx, lbl) {
			this.op = OpCode.J_ret;
			this.rt = rt;
		}

		public override bool ComputeLive(U.Set<string> prev) {
			// in = (out - def) u use
			//
			// out variabili vive dopo l'istruzione
			// def variabili definite (scritte) nell'istruzione
			// use variabili argomenti (lette) nell'instruzione

			var rout = new U.Set<string>(prev);
			var rin = new U.Set<string>(prev);

			if (true) {
				// rd is written  ==> is not live before this instruction
				// rs/rt are read ==> they must be live for this instruction
				if (rt.IsReg) rin.Add(rt.reg);
			}

			bool changed = (rin != _in || rout != _out);
			_in = rin;
			_out = rout;

			return changed;
		}

		public override void ComputeSucc(Context ctx) {
			_succ = new U.Set<AssRoot>();
		}
	
		public override string ToString() {
			string f = Enum.GetName(typeof(OpCode), op).Substring(2);
			var idx = f.IndexOf('_');
			if (idx > 0) f = f.Substring(0, idx);
			string r = U.F("{0,-6} {1}", f, rt);
			return U.F("{0} {1}", InToString(), r);
		}

		public override void Substitute(string temp, string reg) {
			if (rt.IsReg && rt.reg == temp) rt.SetReg(reg);
		}
	}
	class Br_xx : AssRoot {
		OpCode op;
		ExprValue rs;
		ExprValue rt;
		string addr;

		public Br_xx(Context ctx, U.Set<string> lbl, OpCode op, ExprValue rs, ExprValue rt, string addr)
			: base(ctx, lbl) {
			Debug.Assert(Enum.GetName(typeof(OpCode), op).StartsWith("B_"));
			Debug.Assert(rs != null);
			Debug.Assert(rt != null);

			this.op = op;
			this.rs = rs;
			this.rt = rt;
			this.addr = addr;
		}

		public override bool ComputeLive(U.Set<string> prev) {
			// in = (out - def) u use
			//
			// out variabili vive dopo l'istruzione
			// def variabili definite (scritte) nell'istruzione
			// use variabili argomenti (lette) nell'instruzione

			var rout = new U.Set<string>(prev);
			var rin = new U.Set<string>(prev);

			if (true) {
				// rs/rt are read ==> they must be live for this instruction
				if (rs.IsReg) rin.Add(this.rs.reg);
				if (rt.IsReg) rin.Add(this.rt.reg);
			}

			bool changed = (rin != _in || rout != _out);
			_in = rin;
			_out = rout;

			return changed;
		}

		public override void ComputeSucc(Context ctx) {
			_succ = new U.Set<AssRoot>() { ctx.GetOp(this.addr) };
			var succ = ctx.GetSuccOp(this);
			if (succ != null) _succ.Add(succ);
		}


		public override string ToString() {
			string f = Enum.GetName(typeof(OpCode), op).Substring(2);
			var idx = f.IndexOf('_');
			if (idx > 0) f = f.Substring(0, idx);
			string r = U.F("{0,-6} {1}, {2}, {3}", f, rs, rt, addr);
			return U.F("{0} {1}", InToString(), r);
		}

		public override void Substitute(string temp, string reg) {
			if (rs.IsReg && rs.reg == temp) rs.SetReg(reg);
			if (rs.IsReg && rt.reg == temp) rt.SetReg(reg);
		}
	}

	class Br_cr : AssRoot {
		OpCode op;
		int c;
		string rt;
		string addr;

		public Br_cr(Context ctx, U.Set<string> lbl, OpCode op, int c, string rt, string addr)
			: base(ctx, lbl) {
			Debug.Assert(Enum.GetName(typeof(OpCode), op).StartsWith("B_"));

			this.op = op;

			this.c = c;
			this.rt = rt;
			this.addr = addr;
		}

		public override bool ComputeLive(U.Set<string> prev) {
			// in = (out - def) u use
			//
			// out variabili vive dopo l'istruzione
			// def variabili definite (scritte) nell'istruzione
			// use variabili argomenti (lette) nell'instruzione

			var rout = new U.Set<string>(prev);
			var rin = new U.Set<string>(prev);

			if (true) {
				// rd is written  ==> is not live before this instruction
				// rs/rt are read ==> they must be live for this instruction
				rin.Add(rt);
			}

			bool changed = (rin != _in || rout != _out);
			_in = rin;
			_out = rout;

			return changed;
		}

		public override void ComputeSucc(Context ctx) {
			_succ = new U.Set<AssRoot>() { ctx.GetOp(this.addr) };
			var succ = ctx.GetSuccOp(this);
			if (succ != null) _succ.Add(succ);
		}


		public override string ToString() {
			string f = Enum.GetName(typeof(OpCode), op).Substring(2);
			var idx = f.IndexOf('_');
			if (idx > 0) f = f.Substring(0, idx);
			string r = U.F("{0,-6} {1}, {2}, {3}", f, c, rt, addr);
			return U.F("{0} {1}", InToString(), r);
		}

		public override void Substitute(string temp, string reg) {
			if (rt == temp) rt = reg;
		}
	}

	class Br_rc : AssRoot {
		OpCode op;
		string rs;
		int c;
		string addr;

		public Br_rc(Context ctx, U.Set<string> lbl, OpCode op, string rs, int c, string addr)
			: base(ctx, lbl) {
			Debug.Assert(Enum.GetName(typeof(OpCode), op).StartsWith("B_"));

			this.op = op;

			this.rs = rs;
			this.c = c;
			this.addr = addr;
		}

		public override bool ComputeLive(U.Set<string> prev) {
			// in = (out - def) u use
			//
			// out variabili vive dopo l'istruzione
			// def variabili definite (scritte) nell'istruzione
			// use variabili argomenti (lette) nell'instruzione

			var rout = new U.Set<string>(prev);
			var rin = new U.Set<string>(prev);

			if (true) {
				// rd is written  ==> is not live before this instruction
				// rs/rt are read ==> they must be live for this instruction
				rin.Add(rs);
			}

			bool changed = (rin != _in || rout != _out);
			_in = rin;
			_out = rout;

			return changed;
		}

		public override void ComputeSucc(Context ctx) {
			_succ = new U.Set<AssRoot>() { ctx.GetOp(this.addr) };
			var succ = ctx.GetSuccOp(this);
			if (succ != null) _succ.Add(succ);
		}


		public override string ToString() {
			string f = Enum.GetName(typeof(OpCode), op).Substring(2);
			var idx = f.IndexOf('_');
			if (idx > 0) f = f.Substring(0, idx);
			string r = U.F("{0,-6} {1}, {2}, {3}", f, rs, c, addr);
			return U.F("{0} {1}", InToString(), r);
		}

		public override void Substitute(string temp, string reg) {
			if (rs == temp) rs = reg;
		}
	}

	class AssLdConst : AssRoot {
		string rd;
		int c;

		public AssLdConst(Context ctx, U.Set<string> lbl, string rd, int c)
			: base(ctx, lbl) {
			this.rd = rd;
			this.c = c;
		}

		public override bool ComputeLive(U.Set<string> prev) {
			// in = (out - def) u use
			//
			// out variabili vive dopo l'istruzione
			// def variabili definite (scritte) nell'istruzione
			// use variabili argomenti (lette) nell'instruzione

			var rout = new U.Set<string>(prev);
			var rin = new U.Set<string>(prev);

			if (true) {
				// rd is written  ==> is not live before this instruction
				rin.Remove(rd);
			}

			bool changed = (rin != _in || rout != _out);
			_in = rin;
			_out = rout;

			return changed;
		}

		public override void ComputeSucc(Context ctx) {
			_succ = new U.Set<AssRoot>() { };
			var succ = ctx.GetSuccOp(this);
			if (succ != null) _succ.Add(succ);
		}


		public override string ToString() {
			string r = U.F("{0,-6} {1}, {2}", "ld ", rd, c);
			return U.F("{0} {1}", InToString(), r);
		}

		public override void Substitute(string temp, string reg) {
			if (rd == temp) rd = reg;
		}
	}
	#if MIPS
	class MipsX : AssRoot {
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
			} else if (op == OpCode.X_ldi || op == OpCode.X_ldiu) {
				rin.Remove(rd);
				rin.Add(rs);
				rin.Add(rt);
			} else
				Debug.Assert(false);

			bool changed = (rin != _in || rout != _out);
			_in = rin;
			_out = rout;

			return changed;
		}

		public override void ComputeSucc(int pc, Context ctx) {
			_succ = new U.Set<int>() { pc + 1 };
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

	class MipsR : AssRoot {
		OpCode op;
		string rd, rs, rt;

		public MipsR(OpCode op, string rd, string rs, string rt) {
			this.op = op;

			// $d = $s + $t
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

		public override void ComputeSucc(int pc, Context ctx) {
			//if (func == Func_R.jr)
			//	Debug.Assert(false, "Non si puo` calcolare il next di un j r2");

			// notare che invece Func_R.js ha con succ pc+1
			_succ = new U.Set<int>() { pc + 1 };
		}
	}

	class MipsI : AssRoot, IEquatable<MipsI> {
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
			case OpCode.I_sw:  // sw ==> Memory[$s + C] = $t
			case OpCode.I_sh:
			case OpCode.I_sb:
				rin.Add(rs);
				rin.Add(rt);
				break;

			case OpCode.I_beq:  // if($s == $t) go to PC+4+4*C
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

			bool changed = (_in != rin || _out != rout);
			_in = rin;
			_out = rout;
			return changed;
		}

		public override void ComputeSucc(int pc, Context ctx) {
			var ret = new U.Set<int>() { pc + 1 };
			if (op == OpCode.I_beq || op == OpCode.I_bne ||
			     op == OpCode.I_bgt || op == OpCode.I_bge ||
			     op == OpCode.I_blt || op == OpCode.I_ble) {
				if (this.lbl != null)
					ret.Add(ctx.GetAddrFromLabel(this.lbl));
				else
					ret.Add(this.C);
			}

			_succ = ret;
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

	class MipsJ : AssRoot, IEquatable<MipsJ> {
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


			// quidipende dall'istruzione

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

		public override void ComputeSucc(int pc, Context ctx) {
			if (op == OpCode.J_ret) _succ = new U.Set<int>();
			else if (op == OpCode.J_js) _succ = new U.Set<int>() { pc + 1 };
			else _succ = new U.Set<int>() { ctx.GetAddrFromLabel(this.lbl) };
		}


		public bool Equals(MipsJ other) {
			return this == other;
		}
	}
#endif
}


