using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;

namespace WeSay.LexicalModel
{
	public class PairStringLexEntryIdDisplayProvider : IDisplayStringAdaptor
	{
		public string GetDisplayLabel(object item)
		{
			RecordToken<LexEntry> kv = (RecordToken<LexEntry>)item;
			return kv.DisplayString;
		}

		public string GetToolTip(object item)
		{
			RecordToken<LexEntry> recordToken = (RecordToken<LexEntry>)item;
			LexEntry entry = recordToken.RealObject;
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