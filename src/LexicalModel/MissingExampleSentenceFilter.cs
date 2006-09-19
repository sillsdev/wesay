using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.LexicalModel
{
	public class MissingExampleSentenceFilter : WeSay.LexicalModel.IFilter
	{
		string _writingSystemId;

		public MissingExampleSentenceFilter(string writingSystemId)
		{
			_writingSystemId = writingSystemId;
		}

		public Predicate<object> Inquire
		{
			get
			{
				return Filter;
			}
		}

		private bool Filter(object o)
		{
			LexEntry entry = o as LexEntry;
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
	}
}
