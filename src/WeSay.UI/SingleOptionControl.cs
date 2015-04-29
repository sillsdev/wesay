using System;
using System.Drawing;
using System.Windows.Forms;
using SIL.i18n;
using Palaso.Lift;
using Palaso.Lift.Options;
using SIL.UiBindings;
using SIL.Reporting;
using SIL.WritingSystems;
using WeSay.LexicalModel.Foundation;
using WeSay.UI.TextBoxes;

namespace WeSay.UI
{
	public partial class SingleOptionControl: UserControl, IBindableControl<string>
	{
		private readonly OptionsList _list;
		private readonly IWeSayComboBox _control = null;
		private readonly string _nameForLogging;
		private readonly WritingSystemDefinition _preferredWritingSystem;

		public event EventHandler ValueChanged;

		#region IBindableControl<string> Members

		public event EventHandler GoingAway;

		#endregion


		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (GoingAway != null)
			{
				GoingAway.Invoke(this, null); //shake any bindings to us loose
			}
			GoingAway = null;
			base.OnHandleDestroyed(e);
		}

		public SingleOptionControl(IValueHolder<string> optionRef, OptionsList list, string nameForLogging,
			WritingSystemDefinition preferredWritingSystem)
			: this(optionRef, list, nameForLogging, preferredWritingSystem, null)
		{

		}

		public SingleOptionControl(IValueHolder<string> optionRef, OptionsList list, string nameForLogging, WritingSystemDefinition preferredWritingSystem, IServiceProvider serviceProvider )
		{
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			_list = list;
			_nameForLogging = nameForLogging;
			_preferredWritingSystem = preferredWritingSystem;
			_control = serviceProvider.GetService(typeof(IWeSayComboBox)) as IWeSayComboBox;
			InitializeComponent();
			_control.AutoCompleteMode = AutoCompleteMode.Append;
			_control.AutoCompleteSource = AutoCompleteSource.ListItems;
			_control.Sorted = false;
			_control.MaxDropDownItems = 100;
			_control.WritingSystem = _preferredWritingSystem;
			_control.Font = WritingSystemInfo.CreateFont(_preferredWritingSystem);
			_control.Height = WritingSystemInfo.CreateFont(_preferredWritingSystem).Height + 10;
			BuildBoxes(optionRef);
		}

		public string Value
		{
			get
			{
				if (_control.SelectedItem != null)
				{
					string key = "";
					if (ConfiguredListItem(_control.SelectedItem))
					{
						key = ((Option.OptionDisplayProxy) _control.SelectedItem).Key;
					}
					else
					{
						key = (String) _control.SelectedItem;
					}
					// todo: something like this                   if (String.IsNullOrEmpty(key))
					//                    {
					//                        return null;// make "unknown" option be the same as if not set (at least as far as we can do that from here)
					//                    }
					return key;
				}
				return _control.Text;
			}
			set
			{
				for (int i = 0; i < _control.Length; i++)
				{
					String selectedItemText = "";
					if (ConfiguredListItem(_control.GetItem(i)))
					{
						selectedItemText = ((Option.OptionDisplayProxy)_control.GetItem(i)).Key;
					}
					else
					{
						selectedItemText = (String)_control.GetItem(i);
					}
					if (selectedItemText.Equals(value, StringComparison.OrdinalIgnoreCase))
					{
						_control.SelectedIndex = i;
						SetStatusColor(ConfiguredListItem(_control.GetItem(i)));
						return;
					}
				}

				//Didn't find it
				if (value.Length > 0)
				{
					_control.AddItem(value);
					_control.SelectedIndex = _control.Length - 1;
					SetStatusColor(false); //must do this before trying to change to a non-list value
				}
				else
				{
					_control.DropDownStyle = ComboBoxStyle.DropDown;
					_control.AutoCompleteMode = AutoCompleteMode.None;
					_control.Text = value;
					SetStatusColor(true);
				}

			}
		}

		private bool ConfiguredListItem(Object item)
		{
			return (item is Option.OptionDisplayProxy);
		}

		private void SetStatusColor(bool configuredItem)
		{
			if (!configuredItem)
			{
				_control.BackColor = Color.Red;
				_control.DropDownStyle = ComboBoxStyle.DropDown;
			}
			else
			{
				_control.BackColor = Color.White;
				_control.DropDownStyle = ComboBoxStyle.DropDownList;
			}
		}

		private void BuildBoxes(IValueHolder<string> optionRef)
		{
			SuspendLayout();

			Height = 0;
			const int initialPanelWidth = 200;
			SetupComboControl(optionRef);

			components.Add((Control)_control); //so it will get disposed of when we are

			Controls.Add((Control)_control);
			//Height += p.Height;
			ResumeLayout(false);
		}

		private void SetupComboControl(IValueHolder<string> selectedOptionRef)
		{

			_control.Font = (Font)StringCatalog.LabelFont.Clone();
			if (!_list.Options.Exists(delegate(Option o) { return (o.Key == string.Empty || o.Key == "unknown"); }))
			{
				MultiText unspecifiedMultiText = new MultiText();
				unspecifiedMultiText.SetAlternative(_preferredWritingSystem.Id,
													StringCatalog.Get("~unknown",
																	  "This is shown in a combo-box (list of options, like Part Of Speech) when no option has been chosen, or the user just doesn't know what to put in this field."));
				Option unspecifiedOption = new Option("unknown", unspecifiedMultiText);
				_control.AddItem(new Option.OptionDisplayProxy(unspecifiedOption,
																 _preferredWritingSystem.Id));
			}
			_list.Options.Sort(CompareItems);
			foreach (Option o in _list.Options)
			{
				_control.AddItem(o.GetDisplayProxy(_preferredWritingSystem.Id));
			}
			_control.BackColor = Color.White;

			Value = selectedOptionRef.Value;
			_control.ListCompleted();

			_control.SelectedValueChanged += OnSelectedValueChanged;

			//don't let the mousewheel do the scrolling, as it's likely an accident (http://jira.palaso.org/issues/browse/WS-34670)
			((Control)_control).MouseWheel += (sender, e) => {
				HandledMouseEventArgs he = e as HandledMouseEventArgs;
				if (he != null)
				{
					he.Handled = true;
				}
			};
		}

		private int CompareItems(Option a, Option b)
		{
			if (string.IsNullOrEmpty(a.Key)) //get the "unknown" at the top
			{
				return 1;
			}
			if (string.IsNullOrEmpty(b.Key))
			{
				return -1;
			}
			string x = a.Name.GetBestAlternative(_preferredWritingSystem.Id);
			string y = b.Name.GetBestAlternative(_preferredWritingSystem.Id);

			return String.Compare(x, y);
		}

		private void OnSelectedValueChanged(object sender, EventArgs e)
		{
			Logger.WriteMinorEvent("SingleOptionControl_SelectionChanged ({0})", _nameForLogging);
			SetStatusColor(ConfiguredListItem(_control.SelectedItem));
			if (ValueChanged != null)
			{
				ValueChanged.Invoke(this, null);
			}
		}
	}
}
