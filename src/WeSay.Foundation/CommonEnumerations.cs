using System;
using System.Collections.Generic;
using System.Text;


namespace WeSay.Foundation
{
	public class CommonEnumerations
	{
		public enum VisibilitySetting { Visible, ReadOnly, NormallyHidden, /*legacy, so the xml parser doesn't choke*/ Invisible};
	}
}
