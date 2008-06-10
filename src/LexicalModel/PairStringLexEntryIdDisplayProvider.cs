using System.Collections.Generic;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;

namespace WeSay.LexicalModel
{
	public class PairStringLexEntryIdDisplayProvider : IDisplayStringAdaptor
	{
		private readonly IRecordListManager _recordListManager;

		public PairStringLexEntryIdDisplayProvider(IRecordListManager recordListManager)
		{
			_recordListManager = recordListManager;
		}


		public string GetDisplayLabel(object item)
		{
			KeyValuePair<string, long> kv = (KeyValuePair<string, long>)item;
			return kv.Key;
		}

		public string GetToolTip(object item)
		{
			KeyValuePair<string, long> kv = (KeyValuePair<string, long>)item;
			LexEntry entry = this._recordListManager.GetItem<LexEntry>(kv.Value);
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