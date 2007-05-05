using System;
using System.Drawing;
using System.ComponentModel;
using WeSay.UI.Properties;

namespace WeSay.UI.Buttons
{

	[Description("Previous Button Control")]
	public class PreviousButton : RoundButton
	{
		protected override void OnClientSizeChanged(EventArgs e)
		{
			Image = Resources.LeftArrow.GetThumbnailImage(ClientSize.Width - 4, ClientSize.Height - 4, ReturnFalse, IntPtr.Zero);
			base.OnClientSizeChanged(e);
		}

		static private bool ReturnFalse() { return false; }

	}
}
