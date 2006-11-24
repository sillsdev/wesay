using System;
using System.Collections.Generic;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public class MissingGlossFilter : WeSay.Data.IFilter<LexEntry>
	{
		IList<string> _writingSystemIds;

		public MissingGlossFilter(Field field)
		{
			if(field == null)
			{
				throw new ArgumentNullException();
			}
			if (field.FieldName != Field.FieldNames.SenseGloss.ToString())
			{
				throw new ArgumentOutOfRangeException("field", "should be Gloss field definition");
			}
			_writingSystemIds = field.WritingSystemIds;
		}

		public MissingGlossFilter(FieldInventory fieldInventory)
		{
			if (fieldInventory == null)
			{
				throw new ArgumentNullException();
			}
			if (!fieldInventory.Contains(Field.FieldNames.SenseGloss.ToString()))
			{
				throw new ArgumentOutOfRangeException("field", "should contain Gloss field definition");
			}
			Field field;
			if (!fieldInventory.TryGetField(Field.FieldNames.SenseGloss.ToString(), out field))
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
				return IsMissingGloss;
			}
		}

		#endregion
		private bool IsMissingGloss(LexEntry entry)
		{
			if (entry == null)
			{
				return false;
			}

			bool hasSense = false;
			foreach (LexSense sense in entry.Senses)
			{
				hasSense = true;
				foreach (string writingSystemId in _writingSystemIds)
				{
					if (sense.Gloss[writingSystemId].Length == 0)
					{
						return true;
					}
				}
			}
			return !hasSense;
		}
	}
}
