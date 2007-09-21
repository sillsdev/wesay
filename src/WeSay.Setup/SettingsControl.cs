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
				this.Controls.Remove((Control) (currentItem.Tag));
			}
			ConfigurationControlBase c = button.Tag as ConfigurationControlBase;
			if (c != null)
			{
				this.Controls.Add(c);
				c.Left = _areaHeader.Left-12;//this indents the header text to match the first label in the control
				c.Top = _areaHeader.Bottom + 4  ;
				c.Height = _areasToolStrip.Bottom - (c.Top+15);
				c.Width = _areaHeader.Width+13;
				c.SetOtherStuff();

				_areaHeader.Text = "";
				this._areaHeader.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
				//this._areaHeader.SelectionFont = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
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
			ConfigurationControlBase c = currentItem.Tag as ConfigurationControlBase;

			Rectangle r = new Rectangle(c.Left, _areaHeader.Top - 12, c.Width-1, 12+c.Top - _areaHeader.Top);
//           ControlPaint.DrawBorder(e.Graphics, r, Color.Red,
//                                        System.Windows.Forms.ButtonBorderStyle.Solid);
			e.Graphics.FillRectangle(SystemBrushes.Window, r);
			e.Graphics.DrawRectangle(Pens.LightGray, r);
		}

		private void OnLoad(object sender, EventArgs e)
		{
			OnAreaButton_Click(_tasksButton, null);
		}
	}
}