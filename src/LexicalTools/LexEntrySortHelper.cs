using System;
using System.Collections.Generic;
using System.Globalization;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public class LexEntrySortHelper: IDb4oSortHelper<string, LexEntry>
	{
		Db4oDataSource _db4oData;
		string _writingSystemId;
		ViewTemplate _viewTemplate;

		public LexEntrySortHelper(Db4oDataSource db4oData,
								  ViewTemplate viewTemplate,
								  string writingSystemId)
		{
			_db4oData = db4oData;
			_viewTemplate = viewTemplate;
			_writingSystemId = writingSystemId;
		}

		#region IDb4oSortHelper<string,LexEntry> Members

		public IComparer<string> KeyComparer
		{
			get
			{
				StringComparer comparer;
				try
				{
					comparer = StringComparer.Create(CultureInfo.GetCultureInfo(_writingSystemId),
													 false);
				}
				catch
				{
					comparer = StringComparer.InvariantCulture;
				}

				return comparer;
			}
		}

		public List<KeyValuePair<string, long>> GetKeyIdPairs()
		{
			if (IsLexicalFormWritingSystem)
			{
				return KeyToEntryIdInitializer.GetLexicalFormToEntryIdPairs(_db4oData,
																			_writingSystemId);
			}
			else
			{
				return KeyToEntryIdInitializer.GetGlossToEntryIdPairs(_db4oData,
																	  _writingSystemId);
			}
		}

		public IEnumerable<string> GetKeys(LexEntry item)
		{
			List<string> keys = new List<string>();
			if (IsLexicalFormWritingSystem)
			{
				keys.Add(item.LexicalForm.GetAlternative(_writingSystemId));
			}
			else
			{
				foreach (LexSense sense in item.Senses)
				{
					keys.Add(sense.Gloss.GetAlternative(_writingSystemId));
				}
			}
			return keys;
		}

		private bool IsLexicalFormWritingSystem
		{
			get
			{
				return _viewTemplate.GetField("EntryLexicalForm").WritingSystemIds.Contains(_writingSystemId);
			}
		}

		public string Name
		{
			get
			{
				return "LexEntry sorted by " + _writingSystemId;
			}
		}

		#endregion
	}
}
