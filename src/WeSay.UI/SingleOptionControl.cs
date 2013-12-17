using System;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;
using WeSay.Foundation.Options;

namespace WeSay.UI
{
	public partial class SingleOptionControl: UserControl, IBindableControl<string>
	{
		private readonly OptionsList _list;
		private readonly ComboBox _control = new ComboBox();
		private readonly string _idOfPreferredWritingSystem;
		private readonly string _nameForLogging;

		public event EventHandler ValueChanged;

		#region IBindableControl<string> Members

		public event EventHandler GoingAway;

		#endregion

		public SingleOptionControl(string nameForLogging)
		{
			InitializeComponent();
			_idOfPreferredWritingSystem = "es";
			_nameForLogging = nameForLogging;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (GoingAway != null)
			{
				GoingAway.Invoke(this, null); //shake any bindings to us loose
			}
			GoingAway = null;
			base.OnHandleDestroyed(e);
		}

		public SingleOptionControl(IValueHolder<string> optionRef,
								   OptionsList list,
								   string idOfPreferredWritingSystem,
								   string nameForLogging)
		{
			_list = list;
			_nameForLogging = nameForLogging;
			_idOfPreferredWritingSystem = idOfPreferredWritingSystem;
			InitializeComponent();
			//doesn't allow old, non-valid values to be shown (can't set the text):  ComboBoxStyle.DropDownList;
			_control.AutoCompleteMode = AutoCompleteMode.Append;
			_control.AutoCompleteSource = AutoCompleteSource.ListItems;
			_control.Sorted = false;
			_control.MaxDropDownItems = 100;
			BuildBoxes(optionRef);
		}

		public string Value
		{
			get
			{
				//review
				if (_control.SelectedItem != null)
				{
					string key = ((Option.OptionDisplayProxy) _control.SelectedItem).Key;
					// todo: something like this                   if (String.IsNullOrEmpty(key))
					//                    {
					//                        return null;// make "unknown" option be the same as if not set (at least as far as we can do that from here)
					//                    }
					return key;
				}
				else
				{
					return _control.Text;
					// situation where the value isn't currently a member of the approved list
				}
			}
			set
			{
				//if (value != null && value.Length == 0)
				//{
				//    _control.SelectedIndex = -1; //enhance: have a default value
				//    SetStatusColor();
				//    return;
				//}

				foreach (Option.OptionDisplayProxy proxy in _control.Items)
				{
					if (proxy.Key.Equals(value))
					{
						_control.SelectedItem = proxy;
						SetStatusColor();
						return;
					}
				}

				//Didn't find it

				_control.DropDownStyle = ComboBoxStyle.DropDown;
				//allow abberant old value NB: don't remove just because SetStatusCOlor looks like it will set this
				//this was needed to fix ws-115, which appear to be related to changing the
				//DropDownStyle + having autocomplete on.  Sadly, it means a bad (red) key can't be fixed just by typing. Must
				//select a good value.
				_control.AutoCompleteMode = AutoCompleteMode.None;

				_control.Text = value;
				SetStatusColor(); //must do this before trying to change to a non-list value
			}
		}

		private void SetStatusColor()
		{
			if (Value != null && Value.Length > 0 && _control.SelectedIndex == -1)
			{
				_control.BackColor = Color.Red;
				_control.DropDownStyle = ComboBoxStyle.DropDown; //allow abberant old value
			}
			else
			{
				_control.BackColor = Color.White;
				_control.DropDownStyle = ComboBoxStyle.DropDownList; // can't type
			}
		}

		private void BuildBoxes(IValueHolder<string> optionRef)
		{
			SuspendLayout();

			Height = 0;
			const int initialPanelWidth = 200;
			SetupComboControl(optionRef);

			components.Add(_control); //so it will get disposed of when we are

			Panel p = new Panel();
			p.Controls.Add(_control);
			p.Size = new Size(initialPanelWidth, _control.Height + 0);

			//            FlagButton flagButton = MakeFlagButton(p.Size);
			//            p.Controls.Add(flagButton);
			//            this.components.Add(flagButton);//so it will get disposed of when we are

			//            //TODO: THIS IS TRANSITIONAL CODE... AnnotationWidget should probably become a full control (or go away)
			//            AnnotationWidget aw = new AnnotationWidget(multiText, writingSystem.Id);
			//            Control annotationControl = aw.MakeControl(p.Size);
			//            p.Controls.Add(annotationControl);
			//            this.components.Add(annotationControl);//so it will get disposed of when we are

			Controls.Add(p);
			Height += p.Height;
			ResumeLayout(false);
		}

		private void SetupComboControl(IValueHolder<string> selectedOptionRef)
		{
			if (!_list.Options.Exists(delegate(Option o) { return (o.Key == string.Empty); }))
			{
				MultiText unspecifiedMultiText = new MultiText();
				unspecifiedMultiText.SetAlternative(_idOfPreferredWritingSystem,
													StringCatalog.Get("~unknown",
																	  "This is shown in a combo-box (list of options, like Part Of Speech) when no option has been chosen, or the user just doesn't know what to put in this field."));
				Option unspecifiedOption = new Option(string.Empty, unspecifiedMultiText);
				_control.Items.Add(new Option.OptionDisplayProxy(unspecifiedOption,
																 _idOfPreferredWritingSystem));
			}
			_list.Options.Sort(CompareItems);
			foreach (Option o in _list.Options)
			{
				/* this won't work.  It doesn't give us a way to select which ws to display, as it will always
					draw from ToString().  We can change which property (e.g. DisplayName(), but that doesn't
					give us a way to choose the ws either.
					_control.Items.Add(o);
				*/
				_control.Items.Add(o.GetDisplayProxy(_idOfPreferredWritingSystem));
			}

			Value = selectedOptionRef.Value;

			_control.SelectedValueChanged += OnSelectedValueChanged;
			//            _control.Validating += new System.ComponentModel.CancelEventHandler(_control_Validating);
		}

		private int CompareItems(Option a, Option b)
		{
			if (string.IsNullOrEmpty(a.Key)) //get the "unknown" at the top
			{
				return 1;
			}
			else if (string.IsNullOrEmpty(b.Key))
			{
				return -1;
			}
			string x = a.Name.GetBestAlternative(_idOfPreferredWritingSystem);
			string y = b.Name.GetBestAlternative(_idOfPreferredWritingSystem);

			return String.Compare(x, y);
		}

		//void _control_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		//{
		//    //don't allow entering things that aren't options
		//      e.Cancel = !(_control.SelectedIndex > -1 || _control.Text=="");
		//}

		private void OnSelectedValueChanged(object sender, EventArgs e)
		{
			Logger.WriteMinorEvent("SingleOptionControl_SelectionChanged ({0})", _nameForLogging);
			SetStatusColor();
			if (ValueChanged != null)
			{
				ValueChanged.Invoke(this, null);
			}
		}
	}
}