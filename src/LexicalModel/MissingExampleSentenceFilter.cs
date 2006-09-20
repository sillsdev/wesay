using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.LexicalModel
{
	public class MissingExampleSentenceFilter : WeSay.Data.IFilter<LexEntry>
	{
		string _writingSystemId;

		public MissingExampleSentenceFilter(string writingSystemId)
		{
			_writingSystemId = writingSystemId;
		}

		#region IFilter<LexEntry> Members

		public string Key
		{
			get
			{
				return this.ToString() + _writingSystemId;
			}
		}

		public Predicate<LexEntry> Inquire
		{
			get
			{
				return Filter;
			}
		}

		private bool Filter(LexEntry entry)
		{
			if (entry == null)
			{
				return false;
			}
			bool hasSense = false;
			bool hasExample = false;
			foreach (LexSense sense in entry.Senses)
			{
				hasSense = true;
				foreach (LexExampleSentence example in sense.ExampleSentences)
				{
					hasExample = true;
					if (example.Sentence[_writingSystemId] == string.Empty)
					{
						return true;
					}
				}
			}
			return !(hasSense && hasExample);
		}
		#endregion
	}
}
