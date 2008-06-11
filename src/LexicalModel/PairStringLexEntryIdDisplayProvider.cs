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
			RecordToken kv = (RecordToken)item;
			return kv.DisplayString;
		}

		public string GetToolTip(object item)
		{
			RecordToken recordToken = (RecordToken)item;
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