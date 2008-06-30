using System.Collections.Generic;
using WeSay.Foundation;

namespace WeSay.LexicalModel
{
	public class WeSayDataObjectLabelAdaptor: IDisplayStringAdaptor
	{
		private readonly IList<string> _writingSystemIds;
		//review: should this really be an ordered collection of preferred choices?

		public WeSayDataObjectLabelAdaptor(IList<string> writingSystemIds)
		{
			_writingSystemIds = writingSystemIds;
		}

		#region IDisplayStringAdaptor Members

		public string GetDisplayLabel(object item)
		{
			if (item is LexEntry)
			{
				return ((LexEntry) item).LexicalForm.GetBestAlternativeString(_writingSystemIds);
			}
			if (item is LexSense)
			{
				LexSense sense = (LexSense) item;
				return
						GetDisplayLabel(sense.Parent) + "." +
						sense.Gloss.GetBestAlternativeString(_writingSystemIds);
			}
			return "Program error";
		}

		public string GetToolTip(object item)
		{
			if (item is LexEntry)
			{
				return ((LexEntry) item).GetToolTipText();
			}
			if (item is LexSense)
			{
				//LexSense sense = (LexSense) item;
				return "What to show for senses?";
			}
			return "Program error";
		}

		public string GetToolTipTitle(object item)
		{
			return "";
		}

		#endregion
	}
}