using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Autofac;
using Palaso.Reporting;
using WeSay.ConfigTool.Tasks;

namespace WeSay.ConfigTool
{
	public partial class SettingsControl: UserControl
	{
		private readonly List<ConfigurationControlBase> _areaControls;

		public SettingsControl(IComponentContext context)
		{
			this.Disposed += OnDisposed;
			_areaControls = new List<ConfigurationControlBase>();

			InitializeComponent();

			var m = context.Resolve<Tasks.TaskListPresentationModel>();
			_tasksButton.Tag = m.View;
			_areaControls.Add((ConfigurationControlBase) _tasksButton.Tag);

			_writingSystemButton.Tag = context.Resolve<WritingSystemSetup>();
			_areaControls.Add((ConfigurationControlBase) _writingSystemButton.Tag);

			_fieldsButton.Tag = context.Resolve<FieldsControl>();
			_areaControls.Add((ConfigurationControlBase) _fieldsButton.Tag);

			_interfaceLanguageButton.Tag = context.Resolve<InterfaceLanguageControl>();
			_areaControls.Add((ConfigurationControlBase) _interfaceLanguageButton.Tag);

			_actionsButton.Tag = context.Resolve <ActionsControl>();
			_areaControls.Add((ConfigurationControlBase) _actionsButton.Tag);

			_backupButton.Tag = context.Resolve<BackupPlanControl>();
			_areaControls.Add((ConfigurationControlBase) _backupButton.Tag);

//            _chorusButton.Tag = context.Resolve<ChorusControl>();
//            _areaControls.Add((ConfigurationControlBase)_chorusButton.Tag);

			_optionsListButton.Tag = context.Resolve<OptionListControl>();
			_areaControls.Add((ConfigurationControlBase) _optionsListButton.Tag);

			SetStyle(ControlStyles.ResizeRedraw, true); //makes OnPaint work

		}

		private void OnDisposed(object sender, EventArgs e)
		{
			DisposeConfigurationControl(_tasksButton.Tag);
			DisposeConfigurationControl(_writingSystemButton.Tag);
			DisposeConfigurationControl(_fieldsButton.Tag);
			DisposeConfigurationControl(_interfaceLanguageButton.Tag);
			DisposeConfigurationControl(_actionsButton.Tag);
			DisposeConfigurationControl(_backupButton.Tag);
			DisposeConfigurationControl(_optionsListButton.Tag);
		}

		private void DisposeConfigurationControl(object controlToDispose)
		{
			ConfigurationControlBase control = controlToDispose as ConfigurationControlBase;
			control.Dispose();
		}


		///*something left over from this class's predaccesor, which may be useful*/
		////seems to help with some, not with others
		//private void TryToFixScaling(Control c)
		//{
		//    //this is part of dealing with .net not adjusting stuff well for different dpis
		//    c.Dock = DockStyle.None;
		//    c.Size = new Size(Width, Height - 25);
		//    // c.BackColor = System.Drawing.Color.Crimson;
		//}
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
			ToolStripButton button = (ToolStripButton) sender;
			button.Checked = true;

			if (currentItem != null)
			{
				_areaPanel.Controls.Remove((Control) (currentItem.Tag));
			}
			ConfigurationControlBase c = button.Tag as ConfigurationControlBase;
			if (c != null)
			{
				_areaPanel.Controls.Add(c);
				c.Location = new Point(10, _areaHeader.Height + 20);
				c.Width = _areaPanel.Width - 20;
				c.Height = _areaPanel.Height - (15 + c.Top);
				c.SetOtherStuff();

				_areaHeader.Text = "";
				_areaHeader.Font = new Font("Tahoma", 10F, FontStyle.Bold);
				_areaHeader.AppendText(button.Text + ": ");
				_areaHeader.SelectionFont = new Font("Tahoma", 10F, FontStyle.Regular);
				_areaHeader.AppendText(c.Header);
				FieldsControl fc = button.Tag as FieldsControl;
				if (fc != null)
				{
					fc.RefreshFieldDisplayNames();
				}
				c.Focus();

				UsageReporter.SendNavigationNotice("settings/"+c.NameForUsageReporting);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			foreach (ToolStripButton item in _areasToolStrip.Items)
			{
				if (item.Checked) {}
			}

			Rectangle r = new Rectangle(_areaPanel.Left - 1,
										_areaPanel.Top - 1,
										2 + _areaPanel.Width,
										2 + _areaPanel.Height);

			ControlPaint.DrawBorder(e.Graphics, r, Color.LightGray, ButtonBorderStyle.Solid);
		}

		private void OnLoad(object sender, EventArgs e)
		{
			foreach (ConfigurationControlBase control in _areaControls)
			{
				control.PreLoad();
			}
			OnAreaButton_Click(_tasksButton, null);
		}

		private void _areaPanel_Paint(object sender, PaintEventArgs e)
		{
			int y = 3 + _areaHeader.Height;
			e.Graphics.DrawLine(Pens.LightGray, 0, y, _areaPanel.Width, y);
		}


	}
}