using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace WeSay.ConfigTool
{
	public class SplitContainer2: SplitContainer, ISupportInitialize
	{
		new public void BeginInit()
		{
#if !__MonoCS__
			base.BeginInit();
#endif
		}
		new public void EndInit()
		{
#if !__MonoCS__
			base.EndInit();
#endif
		}
	}
}
