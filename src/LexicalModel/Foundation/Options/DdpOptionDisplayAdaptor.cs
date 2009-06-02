using System;

namespace WeSay.Foundation.Options
{
	[CLSCompliant (false)]
	public class DdpOptionDisplayAdaptor : OptionDisplayAdaptor
	{
		public DdpOptionDisplayAdaptor(OptionsList allOptions, string preferredWritingSystemId)
			: base(allOptions, preferredWritingSystemId)
		{
		}
		public override string  GetDisplayLabel(object item)
		{
			Option option = item as Option; // _allOptions.GetOptionFromKey((string)item);
			//prefix with the domain number
			return option.Abbreviation+" "+base.GetDisplayLabel(item);
		}


		/* This doesn't work.
		 * We would like to just type in "1.1" and have it select that domain.
		 * However, until the auto text box differentiates between what we have typed
		 * and what we might like to complete it as, this doesn't work.
		 *
		 * You type "1" and it enters "1 Universe"... if you were about to type "1.1",
		 * you're out of luck!
		 *
		 * public override Option GetValueFromForm(string form)
		 {
			 Option x = base.GetValueFromForm(form);
			 if(x==null)
			 {
				 var y = _allOptions.Options.FirstOrDefault(o => o.Abbreviation.GetBestAlternative(_preferredWritingSystemId) == form);
				 if (y!=null)
					 return y;
			 }
			 return x;
		 }*/
	}
}
