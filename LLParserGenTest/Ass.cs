﻿using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LLParserGenTest
{
	public enum OpCode
	{
		nop,

		iadd,
		isub,
		imul,
		idiv,
		irem,
		iand,
		ior,
		ixor,
		ishl,
		ishr,

		ibeq,
		ibne,
		ibgt,
		ibge,
		iblt,
		ible,

		fadd,
		fsub,
		fmul,
		fdiv,
		frem,

		fbeq,
		fbne,
		fbgt,
		fbge,
		fblt,
		fble,

		obeq,
		obne,

		jmp,
		
		ijs,
		fjs,
		ojs,
		vjs,

		iret,
		fret,
		oret,
		vret,

		ild,
		fld,
		old,

		f2i,
		i2f,

		ildan,  // load member    ldm r4, r6, 3
		fldan,
		oldan,
		hldan,

		istan,  // store member
		fstan,
		ostan,
		hstan,


		ilda,  // load member    ldm r4, r6, 3
		flda,
		olda,
		hlda,

		ista,  // store member
		fsta,
		osta,
		hsta,

		onewobj,
		onewarray
	}

	public abstract class AssRoot : IEquatable<AssRoot>
	{

		protected U.Set<string> _in = new U.Set<string>();
		protected U.Set<string> _out = new U.Set<string>();
		protected U.Set<Label> _lbl = new U.Set<Label>();
		protected U.Set<AssRoot> _succ;

		public U.Set<AssRoot> Succ { get { return _succ; } }

		protected AssRoot(Context ctx, U.Set<Label> lbl) { _lbl = lbl; }

		public bool Equals(AssRoot other)
		{
			foreach (var a in this._lbl)
				foreach (var b in other._lbl)
					if (a == b) return true;
			return false;
		}

		public abstract void ComputeSucc(Context ctx);
		public abstract bool ComputeLive(U.Set<string> force);
		public abstract void Substitute(string temp, string reg);

		protected string InToString()
		{
			string r = "[";
			bool first = true;
			for (int i = 0; i < _in.Count; ++i)
			{
				if (first == false)
					r += ", ";
				first = false;
				r += _in[i];
			}
			r = r + "] ";

			r = r.PadRight(20);

			return r + U.F("{0,-30}", this._lbl);
		}

		public U.Set<string> In { get { return _in; } }
		public U.Set<string> Out { get { return _out; } }
		public U.Set<Label> Lbl { get { return this._lbl; } }
	}

	class OpDS : AssRoot
	{
		readonly OpCode op;
		string rd;
		ExprValue rs;

		public OpDS(Context ctx, U.Set<Label> lbl, OpCode op, string rd, ExprValue rs)
			: base(ctx, lbl)
		{
			this.op = op;
			this.rd = rd;
			this.rs = rs;

			string v = op.ToString().Substring(1);
			Debug.Assert(op == OpCode.f2i ||
				v == "ld" ||
				v == "lda" ||
				v == "ldm");
		}

		public override bool ComputeLive(U.Set<string> force)
		{
			//
			// in(s) = gen(s)  U (out(s) - kill(s))
			// in(s) = used(s) U (out(s) - def(s))
			// used = variabili usate
			// def  = variabili scritte.
			// 
			// out(s) = U in(s1)     con s1 appartente a succ(s)
			//
			// in = (out - def) u use
			//
			// out variabili vive dopo l'istruzione
			// def variabili definite (scritte) nell'istruzione
			// use variabili argomenti (lette) nell'instruzione
			//
			var rin = new U.Set<string>();
			if (force != null) rin.Add(force);
			foreach (var b in this.Succ)
				rin.Add(b.In);

			if (true)
			{
				// rd is written  ==> is not live before this instruction
				rin.Remove(this.rd);
				if (rs.IsReg) rin.Add(this.rs.Reg);
			}

			bool changed = (rin != _in);
			_in = rin;

			return changed;
		}

		public override void ComputeSucc(Context ctx)
		{
			_succ = new U.Set<AssRoot>() { };
			var succ = ctx.GetSuccOp(this);
			if (succ != null) _succ.Add(succ);
		}


		public override string ToString()
		{
			string f = Enum.GetName(typeof(OpCode), op);
			string r = U.F("{0,-6} {1}, {2}", f, rd, rs);
			return U.F("{0} {1}", InToString(), r);
		}

		public override void Substitute(string temp, string reg)
		{
			if (rd == temp) rd = reg;
			if (rs.IsReg && rs.Reg == temp) rs.SetReg(reg);
		}
	}

	class OpDSS : AssRoot
	{
		readonly OpCode op;
		string rd;
		readonly ExprValue rt;
		readonly ExprValue rs;

		public OpDSS(Context ctx, U.Set<Label> lbl, OpCode op, string rd, ExprValue rt, ExprValue rs)
			: base(ctx, lbl)
		{
			Debug.Assert(rd != null);
			Debug.Assert(rs != null);
			Debug.Assert(rt != null);

			this.op = op;

			this.rd = rd;
			this.rt = rt;
			this.rs = rs;
		}

		public override bool ComputeLive(U.Set<string> force)
		{
			//
			// in(s) = gen(s)  U (out(s) - kill(s))
			// in(s) = used(s) U (out(s) - def(s))
			// used = variabili usate
			// def  = variabili scritte.
			// 
			// out(s) = U in(s1)     con s1 appartente a succ(s)
			//
			// in = (out - def) u use
			//
			// out variabili vive dopo l'istruzione
			// def variabili definite (scritte) nell'istruzione
			// use variabili argomenti (lette) nell'instruzione
			//
			var rin = new U.Set<string>();
			if (force != null) rin.Add(force);
			foreach (var b in this.Succ)
				rin.Add(b.In);

			if (true)
			{
				// rd is written  ==> is not live before this instruction
				// rs/rt are read ==> they must be live for this instruction
				rin.Remove(rd);
				if (rs.IsReg) rin.Add(rs.Reg);
				if (rt.IsReg) rin.Add(rt.Reg);
			}

			bool changed = (rin != _in);
			_in = rin;

			return changed;
		}

		public override void ComputeSucc(Context ctx)
		{
			_succ = new U.Set<AssRoot>();
			var succ = ctx.GetSuccOp(this);
			if (succ != null) _succ.Add(succ);
		}


		public override string ToString()
		{
			string f = Enum.GetName(typeof(OpCode), op);
			string r = U.F("{0,-6} {1}, {2}, {3}", f, rd, rt, rs);
			return U.F("{0} {1}", InToString(), r);
		}

		public override void Substitute(string temp, string reg)
		{
			if (rd == temp) rd = reg;
			if (rs.IsReg && rs.Reg == temp) rs.SetReg(reg);
			if (rt.IsReg && rt.Reg == temp) rt.SetReg(reg);
		}
	}

	class OpSSS : AssRoot
	{
		readonly OpCode op;
		string rd;
		readonly ExprValue rt;
		readonly ExprValue rs;

		public OpSSS(Context ctx, U.Set<Label> lbl, OpCode op, string rd, ExprValue rt, ExprValue rs)
			: base(ctx, lbl)
		{
			Debug.Assert(rd != null);
			Debug.Assert(rs != null);
			Debug.Assert(rt != null);

			this.op = op;

			this.rd = rd;
			this.rt = rt;
			this.rs = rs;
		}

		public override bool ComputeLive(U.Set<string> force)
		{
			//
			// in(s) = gen(s)  U (out(s) - kill(s))
			// in(s) = used(s) U (out(s) - def(s))
			// used = variabili usate
			// def  = variabili scritte.
			// 
			// out(s) = U in(s1)     con s1 appartente a succ(s)
			//
			// in = (out - def) u use
			//
			// out variabili vive dopo l'istruzione
			// def variabili definite (scritte) nell'istruzione
			// use variabili argomenti (lette) nell'instruzione
			//
			var rin = new U.Set<string>();
			if (force != null) rin.Add(force);
			foreach (var b in this.Succ)
				rin.Add(b.In);

			if (true)
			{
				rin.Add(rd);
				if (rs.IsReg) rin.Add(rs.Reg);
				if (rt.IsReg) rin.Add(rt.Reg);
			}

			bool changed = (rin != _in);
			_in = rin;

			return changed;
		}

		public override void ComputeSucc(Context ctx)
		{
			_succ = new U.Set<AssRoot>();
			var succ = ctx.GetSuccOp(this);
			if (succ != null) _succ.Add(succ);
		}


		public override string ToString()
		{
			string f = Enum.GetName(typeof(OpCode), op);
			string r = U.F("{0,-6} {1}, {2}, {3}", f, rd, rt, rs);
			return U.F("{0} {1}", InToString(), r);
		}

		public override void Substitute(string temp, string reg)
		{
			if (rd == temp) rd = reg;
			if (rs.IsReg && rs.Reg == temp) rs.SetReg(reg);
			if (rt.IsReg && rt.Reg == temp) rt.SetReg(reg);
		}
	}
	class J : AssRoot
	{
		OpCode op;
		Label addr;
		string rd;

		public J(Context ctx, U.Set<Label> lbl, OpCode op, Label addr)
			: this(ctx, lbl, op, null, addr)
		{
		}
		public J(Context ctx, U.Set<Label> lbl, OpCode op, string rd, Label addr)
			: base(ctx, lbl)
		{
			Debug.Assert(op == OpCode.jmp || op == OpCode.ijs || op == OpCode.fjs || op == OpCode.ojs || op == OpCode.vjs);

			this.op = op;
			this.rd = rd;
			this.addr = addr;
		}

		public override bool ComputeLive(U.Set<string> force)
		{
			//
			// in(s) = gen(s)  U (out(s) - kill(s))
			// in(s) = used(s) U (out(s) - def(s))
			// used = variabili usate
			// def  = variabili scritte.
			// 
			// out(s) = U in(s1)     con s1 appartente a succ(s)
			//
			// in = (out - def) u use
			//
			// out variabili vive dopo l'istruzione
			// def variabili definite (scritte) nell'istruzione
			// use variabili argomenti (lette) nell'instruzione
			//
			var rin = new U.Set<string>();
			if (force != null) rin.Add(force);
			foreach (var b in this.Succ)
				rin.Add(b.In);

			if (true)
			{
				if (rd != null) rin.Remove(rd);
			}

			bool changed = (rin != _in);
			_in = rin;

			return changed;
		}

		public override void ComputeSucc(Context ctx)
		{
			switch (op)
			{
			case OpCode.jmp:
				_succ = new U.Set<AssRoot>() { ctx.GetOp(this.addr) };
				break;

			case OpCode.vjs:
			case OpCode.ijs:
			case OpCode.fjs:
			case OpCode.ojs:
				_succ = new U.Set<AssRoot>() { };
				var succ = ctx.GetSuccOp(this);
				if (succ != null) _succ.Add(succ);
				break;

			default:
				Debug.Assert(false);
				break;
			}
		}


		public override string ToString()
		{
			string f = Enum.GetName(typeof(OpCode), op);
			if (rd == null)
			{
				string r = U.F("{0,-6} {1}", f, this.addr);
				return U.F("{0} {1}", InToString(), r);
			}
			else
			{
				string r = U.F("{0,-6} {1}, {2}", f, this.rd, this.addr);
				return U.F("{0} {1}", InToString(), r);
			}
		}

		public override void Substitute(string temp, string reg)
		{
			if (rd != null && rd == temp) rd = reg;
		}
	}

	class Ret : AssRoot
	{
		readonly ExprValue rt;
		readonly OpCode op;

		public Ret(Context ctx, U.Set<Label> lbl, OpCode op, ExprValue rt)
			: base(ctx, lbl)
		{
			Debug.Assert(op == OpCode.iret || op == OpCode.fret || op == OpCode.vret || op == OpCode.oret);
			this.op = op;
			this.rt = rt;
		}

		public override bool ComputeLive(U.Set<string> force)
		{
			//
			// in(s) = gen(s)  U (out(s) - kill(s))
			// in(s) = used(s) U (out(s) - def(s))
			// used = variabili usate
			// def  = variabili scritte.
			// 
			// out(s) = U in(s1)     con s1 appartente a succ(s)
			//
			// in = (out - def) u use
			//
			// out variabili vive dopo l'istruzione
			// def variabili definite (scritte) nell'istruzione
			// use variabili argomenti (lette) nell'instruzione
			//
			var rin = new U.Set<string>();
			if (force != null) rin.Add(force);
			foreach (var b in this.Succ)
				rin.Add(b.In);

			if (true)
			{
				// rd is written  ==> is not live before this instruction
				// rs/rt are read ==> they must be live for this instruction
				if (rt != null && rt.IsReg) rin.Add(rt.Reg);
			}

			bool changed = rin != _in;
			this._in = rin;

			return changed;
		}

		public override void ComputeSucc(Context ctx)
		{
			_succ = new U.Set<AssRoot>();
		}

		public override string ToString()
		{
			string f = Enum.GetName(typeof(OpCode), op);
			string r = U.F("{0,-6} {1}", f, rt);
			return U.F("{0} {1}", InToString(), r);
		}

		public override void Substitute(string temp, string reg)
		{
			if (rt.IsReg && rt.Reg == temp) rt.SetReg(reg);
		}
	}

	class OpNew : AssRoot
	{
		readonly OpCode op;
		string rd;
		int sz;
		Label vt;

		public OpNew(Context ctx, U.Set<Label> lbl, OpCode op, string rd, int sz, Label vt)
			: base(ctx, lbl)
		{
			this.op = op;
			this.rd = rd;
			this.sz = sz;
			this.vt = vt;

			Debug.Assert(op == OpCode.onewobj || op == OpCode.onewarray);
		}

		public override bool ComputeLive(U.Set<string> force)
		{
			//
			// in(s) = gen(s)  U (out(s) - kill(s))
			// in(s) = used(s) U (out(s) - def(s))
			// used = variabili usate
			// def  = variabili scritte.
			// 
			// out(s) = U in(s1)     con s1 appartente a succ(s)
			//
			// in = (out - def) u use
			//
			// out variabili vive dopo l'istruzione
			// def variabili definite (scritte) nell'istruzione
			// use variabili argomenti (lette) nell'instruzione
			//
			var rin = new U.Set<string>();
			if (force != null) rin.Add(force);
			foreach (var b in this.Succ)
				rin.Add(b.In);

			if (true)
			{
				rin.Remove(rd);
			}

			bool changed = (rin != _in);
			_in = rin;

			return changed;
		}

		public override void ComputeSucc(Context ctx)
		{
			_succ = new U.Set<AssRoot>() { };
			var succ = ctx.GetSuccOp(this);
			if (succ != null) _succ.Add(succ);
		}


		public override string ToString()
		{
			string f = Enum.GetName(typeof(OpCode), op);
			string r = U.F("{0,-6} {1}, {2}, {3}", f, rd, sz, vt);
			return U.F("{0} {1}", InToString(), r);
		}

		public override void Substitute(string temp, string reg)
		{
			if (rd == temp) rd = reg;
		}
	}

	class Br : AssRoot
	{
		OpCode op;
		ExprValue rs;
		ExprValue rt;
		Label addr;

		public Br(Context ctx, U.Set<Label> lbl, OpCode op, ExprValue rs, ExprValue rt, Label addr)
			: base(ctx, lbl)
		{
			Debug.Assert(rs != null);
			Debug.Assert(rt != null);

			this.op = op;
			this.rs = rs;
			this.rt = rt;
			this.addr = addr;
		}

		public override bool ComputeLive(U.Set<string> force)
		{
			//
			// in(s) = gen(s)  U (out(s) - kill(s))
			// in(s) = used(s) U (out(s) - def(s))
			// used = variabili usate
			// def  = variabili scritte.
			// 
			// out(s) = U in(s1)     con s1 appartente a succ(s)
			//
			// in = (out - def) u use
			//
			// out variabili vive dopo l'istruzione
			// def variabili definite (scritte) nell'istruzione
			// use variabili argomenti (lette) nell'instruzione
			//
			//
			var rin = new U.Set<string>();
			if (force != null) rin.Add(force);
			foreach (var b in this.Succ)
				rin.Add(b.In);

			if (true)
			{
				// rs/rt are read ==> they must be live for this instruction
				if (rs.IsReg) rin.Add(this.rs.Reg);
				if (rt.IsReg) rin.Add(this.rt.Reg);
			}

			bool changed = (rin != _in);
			_in = rin;

			return changed;
		}

		public override void ComputeSucc(Context ctx)
		{
			_succ = new U.Set<AssRoot>() { ctx.GetOp(this.addr) };
			var succ = ctx.GetSuccOp(this);
			if (succ != null) _succ.Add(succ);
		}


		public override string ToString()
		{
			string f = Enum.GetName(typeof(OpCode), op);
			string r = U.F("{0,-6} {1}, {2}, {3}", f, rs, rt, addr);
			return U.F("{0} {1}", InToString(), r);
		}

		public override void Substitute(string temp, string reg)
		{
			if (rs.IsReg && rs.Reg == temp) rs.SetReg(reg);
			if (rt.IsReg && rt.Reg == temp) rt.SetReg(reg);
		}
	}


	public class Label : IEquatable<Label>
	{
		public Label(string s) { _s = s; }
		public override string ToString() { return _s; }
		string _s;

		public bool Equals(Label other) { return _s == other._s; }
	}

	class Data : AssRoot
	{
		public Data(Context ctx, U.Set<Label> lbl, int c)
			: base(ctx, lbl)
		{
			this.c = c;
		}
		public Data(Context ctx, U.Set<Label> lbl, byte c)
			: base(ctx, lbl)
		{
			this.c = c;
		}
		public Data(Context ctx, U.Set<Label> lbl, string s)
			: base(ctx, lbl)
		{
			this.c = s;
		}
		public Data(Context ctx, U.Set<Label> lbl, Label s)
			: base(ctx, lbl)
		{
			this.c = s;
		}

		readonly object c;


		public override string ToString()
		{
			string r = "";
			if (c is byte) r = U.F(".byte {0}", c);
			if (c is string) r = U.F(".string {0}", c);
			if (c is int) r = U.F(".int {0}", c);
			if (c is Label) r = U.F(".int {0}", c);
			return U.F("{0} {1}", InToString(), r);
		}



		public override void ComputeSucc(Context ctx)
		{
		}

		public override bool ComputeLive(U.Set<string> force)
		{
			return false;
		}

		public override void Substitute(string temp, string reg)
		{
		}
	}
}
