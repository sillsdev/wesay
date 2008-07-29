using System.Collections.Generic;

namespace WeSay.Data.Tests
{
	public class TestMultiple
	{
		public TestMultiple()
		{
			_strings = new List<string>();
			_keyValuePairs = new List<KeyValuePair<string, string>>();
		}

		public readonly List<string> _strings;

		public List<string> Strings
		{
			get { return this._strings; }
		}

		public List<KeyValuePair<string, string>> KeyValuePairs
		{
			get { return this._keyValuePairs; }
		}

		public readonly List<KeyValuePair<string, string>> _keyValuePairs;

		public string String
		{
			set { this._string = value; }
			get { return this._string; }
		}

		public string _string;
	}
}