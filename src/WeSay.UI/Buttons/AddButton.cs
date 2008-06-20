using System;
using System.ComponentModel;
using WeSay.UI.Properties;

namespace WeSay.UI.Buttons
{
	[Description("Add Button Control")]
	public class AddButton: TagButton
	{
		protected override void OnClientSizeChanged(EventArgs e)
		{
			Image =
					Resources.Plus.GetThumbnailImage(ClientSize.Height - 8,
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