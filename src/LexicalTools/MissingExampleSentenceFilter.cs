using System;
using System.Collections.Generic;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public class MissingExampleSentenceFilter : WeSay.Data.IFilter<LexEntry>
	{
		IList<string> _writingSystemIds;

		public MissingExampleSentenceFilter(Field field)
		{
			if (field == null)
			{
				throw new ArgumentNullException();
			}
			if (field.FieldName != Field.FieldNames.ExampleSentence.ToString())
			{
				throw new ArgumentOutOfRangeException("field", "should be Sentence field definition");
			}
			_writingSystemIds = field.WritingSystemIds;
		}

		public MissingExampleSentenceFilter(ViewTemplate viewTemplate)
		{
			if (viewTemplate == null)
			{
				throw new ArgumentNullException();
			}
			Field field;
			if (!viewTemplate.TryGetField(Field.FieldNames.ExampleSentence.ToString(), out field))
			{
				throw new ArgumentOutOfRangeException("field", "should contain Sentence field definition");
			}
			_writingSystemIds = field.WritingSystemIds;
		}

		#region IFilter<LexEntry> Members

		public string Key
		{
			get
			{
				string key = ToString();
				foreach (string writingSystemId in _writingSystemIds)
				{
					key += writingSystemId;
				}
				return key;
			}
		}

		public Predicate<LexEntry> FilteringPredicate
		{
			get
			{
				return IsMissingExampleSentence;
			}
		}

		private bool IsMissingExampleSentence(LexEntry entry)
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
					foreach (string writingSystemId in _writingSystemIds)
					{
						if (example.Sentence[writingSystemId].Length == 0)
						{
							return true;
						}
					}
				}
			}
			return !(hasSense && hasExample);
		}
		#endregion
	}
}
