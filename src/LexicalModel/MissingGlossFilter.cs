using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.LexicalModel
{
	public class MissingGlossFilter : WeSay.Data.IFilter<LexEntry>
	{
		string _writingSystemId;

		public MissingGlossFilter(string writingSystemId)
		{
			_writingSystemId = writingSystemId;
		}

		#region IFilter<LexEntry> Members

		public string Key
		{
			get
			{
				return this.ToString() + "writingSystemId";
			}
		}

		public Predicate<LexEntry> Inquire
		{
			get
			{
				return Filter;
			}
		}

		#endregion
		private bool Filter(LexEntry entry)
		{
			if (entry == null)
			{
				return false;
			}

			bool hasSense = false;
			foreach (LexSense sense in entry.Senses)
			{
				hasSense = true;
				if (sense.Gloss[_writingSystemId].Length == 0)
				{
					return true;
				}
			}
			return !hasSense;
		}
	}
}
