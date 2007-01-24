using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation;
using System;

namespace WeSay.UI
{
	public partial class SingleOptionControl : Widget, IBindableControl<string>
	{
		private OptionsList _list;
		private ComboBox _control = new ComboBox();

		public  event EventHandler ValueChanged;

		#region IBindableControl<string> Members

		public event EventHandler GoingAway;

		#endregion

		public SingleOptionControl()
		{
			InitializeComponent();
		}

		public SingleOptionControl(OptionRef optionRef, OptionsList list)
		{
			_list = list;
			InitializeComponent();
		   //doesn't all old, non-valid values to be shown (can't set the text):  ComboBoxStyle.DropDownList;
			_control.DropDownStyle = ComboBoxStyle.DropDown ;
			_control.AutoCompleteMode = AutoCompleteMode.Append;
			_control.AutoCompleteSource = AutoCompleteSource.ListItems;
			BuildBoxes(optionRef);
		}

		public string Value
		{
			get
			{
				//review
				if (_control.SelectedItem != null)
				{
					return (_control.SelectedItem as Option).Key;
				}
				else
				{
					return _control.Text; // situation where the value isn't currently a member of the approved list
				}
			}
			set
			{
				if (value != null && value.Length == 0)
				{
					_control.SelectedIndex = -1; //enhance: have a default value
					return;
				}

				foreach (Option option in _list.Options)
				{
					if (option.Name.Equals(value))
					{
						_control.SelectedItem = option;
					   return;
					}
				}

				//Didn't find it

				_control.Text = value;
				SetStatusColor();
			}
		}

		private void SetStatusColor()
		{
			if (this.Value != null && Value.Length > 0 &&_control.SelectedIndex == -1)
			{
				_control.BackColor = Color.Red;
			}
			else
			{
				_control.BackColor = Color.White;
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
			}
			this.Value = optionRef.Value;

			_control.SelectedValueChanged += new System.EventHandler(OnSelectedValueChanged);
			_control.Validating += new System.ComponentModel.CancelEventHandler(_control_Validating);
	   }

		void _control_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//don't allow entering things that aren't options
			e.Cancel = !(_control.SelectedIndex > -1 || _control.Text=="");
		}

		void OnSelectedValueChanged(object sender, System.EventArgs e)
		{
			SetStatusColor();
			if (ValueChanged != null)
			{
				ValueChanged.Invoke(this,null);
			}
		}
	}
}
