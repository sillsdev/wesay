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
			RecordToken kv = (RecordToken)item;
			return kv.DisplayString;
		}

		public string GetToolTip(object item)
		{
			RecordToken kv = (RecordToken)item;
			LexEntry entry = this._recordListManager.GetItem<LexEntry>(kv.Id);
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