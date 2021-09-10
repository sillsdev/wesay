using System;
using System.Windows.Forms;

namespace WeSay.UI.Buttons
{
	public partial class DeleteButton : UserControl
	{
		private bool _active = true;

		public DeleteButton()
		{
			InitializeComponent();
			_button.Click += OnButtonClick;
		}

		private void OnButtonClick(object sender, EventArgs e)
		{
			if (_active)
			{
				OnClick(e);
			}
		}

		public bool Active
		{
			get { return _active; }
			set
			{
				if (value == _active)
				{
					return;
				}
				_active = value;
				_button.Image = _active ? Properties.Resources.DeleteIcon : Properties.Resources.DeleteIconBw;
			}
		}

		public string ToolTip
		{
			get { return toolTip1.GetToolTip(_button); }
			set { toolTip1.SetToolTip(_button, value); }
		}

		public void PerformClick()
		{
			OnButtonClick(this, new EventArgs());
		}
	}
}
