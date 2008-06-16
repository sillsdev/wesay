using System.Collections.Generic;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;

namespace WeSay.LexicalModel
{
	public class PairStringLexEntryIdDisplayProvider : IDisplayStringAdaptor
	{
		private readonly LexEntryRepository _lexEntryRepository;

		public PairStringLexEntryIdDisplayProvider(LexEntryRepository lexEntryRepository)
		{
			_lexEntryRepository = lexEntryRepository;
		}


		public string GetDisplayLabel(object item)
		{
			RecordToken<LexEntry> kv = (RecordToken<LexEntry>)item;
			return kv.DisplayString;
		}

		public string GetToolTip(object item)
		{
			RecordToken<LexEntry> recordToken = (RecordToken<LexEntry>)item;
			LexEntry entry = _lexEntryRepository.GetItem(recordToken);
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