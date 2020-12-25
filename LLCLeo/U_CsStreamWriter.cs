﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System;

public partial class U
{
	public class CsStreamWriter
	{
		public CsStreamWriter(TextWriter sw)
		{
			_sw = sw;
			_tab = 0;
			_last = "";
			_sb = new StringBuilder();

			_stmt = new List<Regex>
				{
					new Regex(@"^if \(.*\)$"),
					new Regex(@"^else$"),
					new Regex(@"^else if \(.*\)$"),
					new Regex(@"^for \(.*\)$"),
					new Regex(@"^foreach \(.*\)$"),
					new Regex(@"^while \(.*\)$"),
					new Regex(@"^do$")
				};
		}

		public CsStreamWriter WriteLine()
		{
			WriteLine("");
			return this;
		}
		public CsStreamWriter WriteLine(string s)
		{
			_sb.Append(s);
			s = _sb.ToString(); 
			_sb = new StringBuilder();

			if (s == "{")
			{
				Tab();
				_sw.WriteLine("{");
				_tab += 1;
			}
			else if (s == "}" || s == "};")
			{
				_tab -= 1;
				Tab();
				_sw.WriteLine(s);
			}
			else if (s.StartsWith("case") || s.StartsWith("default") || s == "public:" || s == "private:" || s == "protected:")
			{
				_tab -= 1;
				Tab();
				_sw.WriteLine(s);
				_tab += 1;
			}
			else if (MatchLast())
			{
				_tab += 1;
				Tab();
				_sw.WriteLine(s);
				_tab -= 1;
			}
			else
			{
				Tab();
				_sw.WriteLine(s);
			}

			_last = s;
			return this;
		}
		public CsStreamWriter WriteLine(string s, params object[] a)
		{
			string v = string.Format(CultureInfo.InvariantCulture, s, a);
			WriteLine(v);
			return this;
		}

		public CsStreamWriter WriteComma<T>(List<T> v, Func<T, string> f)
		{
			if (v.Count == 0) return this;
			if (v.Count >= 1) Write(f(v[0]));
			for (int i = 1; i < v.Count; ++i)
			{
				Write(", ");
				Write(f(v[i]));
			}
			return this;
		}
		public CsStreamWriter WriteComma<T>(string pre, List<T> v, Func<T, string> f)
		{
			Write(pre);
			for (int i = 0; i < v.Count; ++i)
			{
				Write(", ");
				Write(f(v[i]));
			}
			return this;
		}


		public CsStreamWriter Write(string s)
		{
			_sb.Append(s);
			return this;
		}
		public CsStreamWriter Write(string s, params object [] a)
		{
			_sb.AppendFormat(CultureInfo.InvariantCulture, s, a);
			return this;
		}

		public void SetTab(int i)
		{
			_tab += i;
		}

		private readonly TextWriter _sw;
		private int _tab;
		private readonly List<Regex> _stmt;
		private string _last;
		private StringBuilder _sb;

		private void Tab()
		{
			for (int i = 0; i < _tab; ++i)
				_sw.Write("\t");
		}
		private bool MatchLast()
		{
			// il primo della lista _stmt che ha .Match().success esce con true, altrimenti si esce con false.
			return _stmt.Any(m => m.Match(_last).Success);
		}
	}
}
