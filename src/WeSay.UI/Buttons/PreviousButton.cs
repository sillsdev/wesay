using System;
using System.ComponentModel;
using WeSay.UI.Properties;

namespace WeSay.UI.Buttons
{
	[Description("Previous Button Control")]
	public class PreviousButton: RoundButton
	{
		public PreviousButton()
		{
			ReallySetSize(30, 30);
		}

		protected override void OnClientSizeChanged(EventArgs e)
		{
			Image =
					Resources.LeftArrow.GetThumbnailImage(ClientSize.Width - 8,
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