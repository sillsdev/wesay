using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WeSay.AddinLib
{
	public partial class ActionItemControl : UserControl//, IControlForListBox
	{
		private IWeSayAddin _addin;
		public event EventHandler Launch;

		public ActionItemControl()
		{
			InitializeComponent();
			_description.ForeColor = Color.Blue;
		}

		public ActionItemControl(IWeSayAddin addin) : this()
		{
			_addin = addin;
			_actionName.Text = addin.Name;
			_description.Text = addin.ShortDescription;
			if (addin.ButtonImage != null)
			{
				_launchButton.Image = addin.ButtonImage;
			}
			if (!addin.Available)
			{
				_actionName.ForeColor = System.Drawing.Color.Gray;
				_description.ForeColor = System.Drawing.Color.Gray;
				_launchButton.Enabled = false;
			}
		}

		public void Draw(Graphics graphics, Rectangle bounds)
		{
			//this.InvokePaint(this, new PaintEventArgs(graphics, bounds));
		}

		public int GetHeight(int width)
		{
			return this.Height;

		}

		private void _launchButton_Click(object sender, EventArgs e)
		{
			if (Launch != null)
			{
				Launch.Invoke(_addin,null);
			}
		}

	}
}
