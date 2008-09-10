using System;
using System.ComponentModel;
using WeSay.UI.Properties;

namespace WeSay.UI.Buttons
{
	[Description("Next Button Control")]
	public class NextButton: RoundButton
	{
		protected override void OnClientSizeChanged(EventArgs e)
		{
			Image = Resources.RightArrow.GetThumbnailImage(ClientSize.Width - 8,
														   ClientSize.Height - 8,
														   ReturnFalse,
														   IntPtr.Zero);
			base.OnClientSizeChanged(e);
		}

		private static bool ReturnFalse()
		{
			return false;
		}
	}
}