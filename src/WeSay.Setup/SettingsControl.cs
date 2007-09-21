using System;
using System.Drawing;
using System.Windows.Forms;

namespace WeSay.Setup
{
	public partial class SettingsControl : UserControl
	{
		public SettingsControl()
		{
			InitializeComponent();
			_tasksButton.Tag = new TaskListControl();
			_writingSystemButton.Tag = new WritingSystemSetup();
			_fieldsButton.Tag = new FieldsControl();
			_interfaceLanguageButton.Tag = new InterfaceLanguageControl();
			_actionsButton.Tag = new ActionsControl();
			_backupButton.Tag = new BackupPlanControl();
			_optionsListButton.Tag = new OptionListControl();
			SetStyle(ControlStyles.ResizeRedraw,true);//makes OnPaint work
		}

		/*something left over from this class's predaccesor, which may be useful*/
		//seems to help with some, not with others
		private void TryToFixScaling(Control c)
		{
			//this is part of dealing with .net not adjusting stuff well for different dpis
			c.Dock = DockStyle.None;
			c.Size = new Size(this.Width, this.Height - 25);
			// c.BackColor = System.Drawing.Color.Crimson;
		}

		private void OnAreaButton_Click(object sender, EventArgs e)
		{
			ToolStripButton currentItem = null;
			foreach (ToolStripButton item in _areasToolStrip.Items)
			{
				if (item.Checked)
				{
					currentItem = item;
				}
				item.Checked = false;
			}
			ToolStripButton button = (ToolStripButton)sender;
			button.Checked = true;

		   if (currentItem != null)
			{
				this._areaPanel.Controls.Remove((Control) (currentItem.Tag));
			}
			ConfigurationControlBase c = button.Tag as ConfigurationControlBase;
			if (c != null)
			{
				this._areaPanel.Controls.Add(c);
				c.Location = new Point(10, _areaHeader.Height +20);
				c.Width = _areaPanel.Width - 20;
				c.Height = _areaPanel.Height - (15+c.Top);
				c.SetOtherStuff();

				_areaHeader.Text = "";
				this._areaHeader.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
				_areaHeader.AppendText(button.Text+": ");
				 this._areaHeader.SelectionFont = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular);
				_areaHeader.AppendText(c.Header);

			 }
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			ToolStripButton currentItem = null;
			foreach (ToolStripButton item in _areasToolStrip.Items)
			{
				if (item.Checked)
				{
					currentItem = item;
				}
			}

			Rectangle r = new Rectangle(_areaPanel.Left - 1, _areaPanel.Top-1 , 2 + _areaPanel.Width, 2+ _areaPanel.Height);

			ControlPaint.DrawBorder(e.Graphics, r, Color.LightGray, ButtonBorderStyle.Solid);
		}

		private void OnLoad(object sender, EventArgs e)
		{
			OnAreaButton_Click(_tasksButton, null);
		}

		private void _areaPanel_Paint(object sender, PaintEventArgs e)
		{
			int y = 3 + _areaHeader.Height;
			e.Graphics.DrawLine(Pens.LightGray, 0, y, _areaPanel.Width, y);
		}
	}
}