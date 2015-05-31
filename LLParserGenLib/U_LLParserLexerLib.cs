﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace LLParserLexerLib
{

	public class SyntaxError : Exception
	{
		public SyntaxError(string msg)
			: base(msg)
		{
		}
		public SyntaxError(string fn, int line, string fmt, params object[] args)
			: base(U.F("{0}({1}): {2}", fn, line, U.F(fmt, args)))
		{
		}
		public SyntaxError(ISourceTrackable sc, string fmt, params object[] args)
			: base(U.F("{0}({1}): {2}", sc != null ? sc.fileName : "", sc != null ? sc.lineNu : 0, U.F(fmt, args)))
		{
		}
	}

	public interface ISourceTrackable
	{
		string fileName { get; }
		int  lineNu { get; }
	}
	public interface IAST
	{
	}

	public class ListAST<T> : IAST, IEnumerable<T>
	{
		public ListAST() { }
		public ListAST(T a) { _s.Add(a); }
		public ListAST<T> Add(T a) { _s.Add(a); return this; }
		public IEnumerator<T> GetEnumerator() { return _s.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return _s.GetEnumerator(); }
		List<T> _s = new List<T>();
		public int Count { get { return _s.Count; } }
		public T this[int i] { get { return _s[i]; } }
		public T[] ToArray() { return _s.ToArray(); }
	}

	[Serializable]
	public class SourceTrackable : ISourceTrackable
	{
		public SourceTrackable(string fileName, int lineNu) { this._fileName = fileName; this._lineNu = lineNu; }
		public SourceTrackable(ISourceTrackable sc) { this._fileName = sc.fileName; this._lineNu = sc.lineNu; }
		public readonly string _fileName;
		public readonly int _lineNu;

		public string fileName { get { return _fileName; } }
		public int lineNu { get { return _lineNu; } }

		public override string ToString() { return TrackMsg; }

		public string TrackMsg {
			get {
				if (_fileName != null && _lineNu > 0)
					return U.F("{0}({1}):", _fileName, _lineNu);
				if (_fileName != null && _lineNu == 0)
					return U.F("{0}:", _fileName);
				if (_fileName == null && _lineNu > 0)
					return U.F("<unknown file>({0}):", _lineNu);
				return "";
			}
		}
	}

	[Serializable]
	public class TokenAST : SourceTrackable, IAST
	{
		public TokenAST(string fileName, int lineNu, int ch, string id, string v) : base(fileName, lineNu) { this.token = ch; this.tokenStr = id;  this.strRead = v; }
		public TokenAST(ISourceTrackable sc, int tk, string id, string v) : this(sc.fileName, sc.lineNu, tk, id, v) {}
		public TokenAST(ISourceTrackable sc, char tk) : base(sc) { this.token = tk; this.strRead = U.F("{0}", tk); this.tokenStr = this.strRead; }

		public readonly int token;         // quello che ho letto tradotto in token
		public readonly string strRead;          // es while (quello che ha letto) come stringa
		public readonly string tokenStr;   // la rappresentazione in stringa del token. es "WHILE" quando token=WHILE

		public override string ToString()
		{
			if (tokenStr == null)
				return U.F("{0}: {1} - \"{2}\"", this.TrackMsg, this.token, strRead);
			else
				return U.F("{0}: {1} - \"{2}\"", this.TrackMsg, this.tokenStr, strRead);
		}
	}

	public abstract class ParserBase
	{
		LexReader _rd;
		NFA _nfa;
		TokenAST _next;

		public void init(NFA nfa, LexReader rd)
		{
			this._nfa = nfa;
			this._rd = rd;
		}
		public void init(LexReader rd)
		{
			this._rd = rd;
		}


		abstract public Dictionary<int, string> Token
		{
			get;
		}

		protected string GetToken(int ch)
		{
			if (Token.ContainsKey(ch)) return Token[ch];
			if (ch >= 32 && ch < 128) return U.F("'{0}'", (char)ch);
			return U.F("{0}", ch);
		}

		protected void Error()
		{
			throw new SyntaxError(Next.fileName, Next.lineNu, "unexpected token '{0}' '{1}'", GetToken(Next.token), Next.strRead);
		}

		protected abstract RegAcceptList CreateRegAcceptList();

		protected ParserBase(int state)
		{
			this._nfa = new NFA();
			var acts = CreateRegAcceptList();
			this._nfa.Add(state, acts);
		}
		protected ParserBase()
		{
			this._nfa = null;
			this._rd = null;
		}

		protected virtual TokenAST Next
		{
			get
			{
				if (_next == null)
				{
					var t = _nfa.ReadToken(_rd);
					string tokenStr;
					if (Token.TryGetValue(t.token, out tokenStr) == false) tokenStr = null;
					_next = new TokenAST(t.fileName, t.line, t.token, tokenStr, t.strRead);
				}
				return _next;
			}
		}
		protected virtual TokenAST Match(int ch, IAST v)
		{
			if (Next.token != ch)
				throw new SyntaxError(_next.fileName, _next.lineNu, "expected char '{0}' read {1}", (char)ch, Next.ToString());

			var ret = _next;
			_next = null;
			return ret;
		}
	}
}
