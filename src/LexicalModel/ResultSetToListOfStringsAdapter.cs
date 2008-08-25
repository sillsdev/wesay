using System.Collections;
using System.Collections.Generic;
using WeSay.Data;

namespace WeSay.LexicalModel
{
	public class ResultSetToListOfStringsAdapter : IEnumerable<string>
	{
		private ResultSet<LexEntry> _results;
		private string _fieldLabel;

		// TODO: This can be generic and expressed as an adapter for IEnumerable<RecordToken<T>>
		public ResultSetToListOfStringsAdapter(string fieldLabel, ResultSet<LexEntry> results)
		{
			_results = results;
			FieldLabel = fieldLabel;
		}

		public ResultSet<LexEntry> Items
		{
			get { return _results; }
			set { _results = value; }
		}

		public string FieldLabel
		{
			get { return _fieldLabel; }
			set { _fieldLabel = value; }
		}

		public IEnumerator<string> GetEnumerator()
		{
			foreach (RecordToken<LexEntry> token in Items)
			{
				yield return token[FieldLabel].ToString();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
