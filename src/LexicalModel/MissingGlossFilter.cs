using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.LexicalModel
{
	public class MissingGlossFilter : WeSay.LexicalModel.IFilter
	{
		string _writingSystemId;

		public MissingGlossFilter(string writingSystemId)
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
			foreach (LexSense sense in entry.Senses)
			{
				hasSense = true;
				if (sense.Gloss[_writingSystemId] == string.Empty)
				{
					return true;
				}
			}
			return !hasSense;
		}
	}
}
