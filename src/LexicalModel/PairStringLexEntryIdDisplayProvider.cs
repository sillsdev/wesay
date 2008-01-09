using System.Collections.Generic;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;

namespace WeSay.LexicalModel
{
	public class PairStringLexEntryIdDisplayProvider : IDisplayStringAdaptor
	{
		private readonly CachedSortedDb4oList<string, LexEntry> _cachedSortedDb4oList;

		public PairStringLexEntryIdDisplayProvider(CachedSortedDb4oList<string, LexEntry> cachedSortedDb4oList)
		{
			_cachedSortedDb4oList = cachedSortedDb4oList;
		}


		public string GetDisplayLabel(object item)
		{
			KeyValuePair<string, long> kv = (KeyValuePair<string, long>)item;
			return kv.Key;
		}

		public string GetToolTip(object item)
		{
			KeyValuePair<string, long> kv = (KeyValuePair<string, long>)item;
			LexEntry entry = this._cachedSortedDb4oList.GetValueFromId(kv.Value);
			return entry.GetToolTipText();
		}

		#region IDisplayStringAdaptor Members

		public string GetToolTipTitle(object item)
		{
			return "";
		}

		#endregion
	}
}