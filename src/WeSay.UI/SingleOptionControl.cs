using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation;
using System;

namespace WeSay.UI
{
	public partial class SingleOptionControl : Widget
	{
		private Option _value;
		private OptionsList _list;
		private ComboBox _control = new ComboBox();

		public event EventHandler ValueChanged;

		public SingleOptionControl()
		{
			InitializeComponent();
		}

		public SingleOptionControl(OptionRef optionRef, OptionsList list)
		{
			_list = list;
			InitializeComponent();
			BuildBoxes(optionRef);
		}
		public Option Value
		{
			get
			{
				return _control.SelectedItem as Option;
			}
			set
			{
				_value = value;
			}
		}
		private void BuildBoxes(OptionRef optionRef)
		{
			SuspendLayout();

			Height = 0;
			const int initialPanelWidth = 200;
			SetupComboControl(initialPanelWidth, optionRef);

			this.components.Add(_control);//so it will get disposed of when we are

			Panel p = new Panel();
			p.Controls.Add(_control);
			p.Size = new Size(initialPanelWidth, _control.Height + 0);

			FlagButton flagButton = MakeFlagButton(p.Size);
			p.Controls.Add(flagButton);
			this.components.Add(flagButton);//so it will get disposed of when we are
			this.Controls.Add(p);
			Height += p.Height;
			ResumeLayout(false);
		}

		private void SetupComboControl(int initialPanelWidth, OptionRef optionRef)
		{
			foreach (Option o in _list.Options)
			{
				_control.Items.Add(o);
				if (optionRef.Value != null && optionRef.Value.Guid == o.Guid)
				{
					_control.SelectedItem = o;
				}
			}

			_control.SelectedValueChanged += new System.EventHandler(OnSelectedValueChanged);
		}

		void OnSelectedValueChanged(object sender, System.EventArgs e)
		{
			if (ValueChanged != null)
			{
				ValueChanged.Invoke(this,null);
			}
		}
	}
}
