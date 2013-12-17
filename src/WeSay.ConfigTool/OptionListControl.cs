using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Palaso.i18n;
using Palaso.Lift;
using Palaso.Lift.Options;
using Palaso.Reporting;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI.TextBoxes;

namespace WeSay.ConfigTool
{
	public partial class OptionListControl: ConfigurationControlBase
	{
		private OptionsList _currentList;
		private Option _currentOption;
		private Field _currentField;
		/// <summary>
		/// the new ones change their key as their names are editted. Old ones don't.
		/// </summary>
		private readonly List<Option> _newlyCreatedOptions = new List<Option>();

		public OptionListControl(ILogger logger)
			: base("set up choices for option fields", logger,"optionLists")
		{
			InitializeComponent();
			VisibleChanged += OptionListControl_VisibleChanged;
			WeSayWordsProject.Project.EditorsSaveNow += OnEditorSaveNow;
		}

		void OnNameChanged(object sender, EventArgs e)
		{
			UpdateKeyLabel();
		}



		private void OnEditorSaveNow(object sender, EventArgs e)
		{
			SaveEditsToCurrentItem();
		}

		private void OptionListControl_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible && _listBox.Items.Count > 0)
			{
				LoadFieldChooser();

				//things, like active fields, may have been changed in a different tab

				if (_listBox.SelectedItem != null)
				{
					if (_listBox.SelectedIndex == 0)
					{
						_listBox.SelectedIndex = _listBox.Items.Count - 1;
					}
				}
				AdjustLocations();
			}
		}

		private void LoadFieldChooser()
		{
			_fieldChooser.Items.Clear();
			foreach (Field field in WeSayWordsProject.Project.DefaultViewTemplate)
			{
				if (field.DataTypeName == Field.BuiltInDataType.Option.ToString() ||
					field.DataTypeName == Field.BuiltInDataType.OptionCollection.ToString())
				{
					_fieldChooser.Items.Add(field);
				}
			}
			if (_currentField != null)
			{
				_fieldChooser.SelectedItem = _currentField;
			}
		}

		private void OnLoad(object sender, EventArgs e)
		{
			Field field = GetPOSOrFieldWithOptionsList();
			if (null == field)
			{
				// _listDescriptionLabel.Text = "This project does not have any fields which use option lists.";
				splitContainer1.Visible = false;
				UpdateDisplay();
				return;
			}
			LoadFieldChooser();
			LoadList(field);
		}

		private void LoadList(Field field)
		{
			try
			{
				_currentField = field;
				_currentList = WeSayWordsProject.Project.GetOptionsList(_currentField, true);

				_listBox.Items.Clear();
				foreach (Option option in _currentList.Options)
				{
					_listBox.Items.Add(option.GetDisplayProxy(PreferredWritingSystem));
				}
				if (_listBox.Items.Count > 0)
				{
					_listBox.SelectedIndex = 0;
				}
				_fieldChooser.SelectedItem = field;
				UpdateDisplay();
			}
			catch (ConfigurationException e)
			{
				ErrorReport.NotifyUserOfProblem(e.Message);
			}
		}

		private string PreferredWritingSystem
		{
			get
			{
				string preferredWritingSystem = "en";
				if (_currentField.WritingSystemIds.Count > 0)
				{
					preferredWritingSystem = _currentField.WritingSystemIds[0];
				}
				return preferredWritingSystem;
			}
		}

		private static Field GetPOSOrFieldWithOptionsList()
		{
			foreach (Field field in WeSayWordsProject.Project.DefaultViewTemplate)
			{
				if (field.FieldName == "POS")
				{
					return field;
				}
			}

			foreach (Field field in WeSayWordsProject.Project.DefaultViewTemplate)
			{
				if (!String.IsNullOrEmpty(field.OptionsListFile))
				{
					return field;
				}
			}
			return null;
		}

		private void OnSelectedIndexChanged(object sender, EventArgs e)
		{
			if (_listBox.SelectedIndex > -1 &&
				((Option.OptionDisplayProxy) _listBox.SelectedItem).UnderlyingOption !=
				_currentOption)
			{
				SaveEditsToCurrentItem();
				var proxy = (Option.OptionDisplayProxy) _listBox.SelectedItem;
				splitContainer1.Panel2.Controls.Remove(_nameMultiTextControl);

				_currentOption = proxy.UnderlyingOption;
				var m = new MultiTextControl(
					_currentField.WritingSystemIds,
					_currentOption.Name,
					_currentField.FieldName,
					false,
					BasilProject.Project.WritingSystems,
					CommonEnumerations.VisibilitySetting.Visible,
					_currentField.IsSpellCheckingEnabled, false, null
				);
				m.SizeChanged += OnNameControlSizeChanged;
				m.Bounds = _nameMultiTextControl.Bounds;
				m.Top = _nameLabel.Top;
				m.BorderStyle = BorderStyle.FixedSingle;
				m.Anchor = _nameMultiTextControl.Anchor;
				m.TextChanged += OnNameChanged;

				_nameMultiTextControl = m;
				splitContainer1.Panel2.Controls.Add(m);

				/* user can't change this anymore. It was confusing to people an probably not needed
				_keyText.TextChanged -= OnKeyTextChanged;
				_keyText.Text = proxy.UnderlyingOption.Key;
				_keyText.TextChanged += OnKeyTextChanged;
				*/

				var justTextBoxes = from z in m.TextBoxes where z is WeSayTextBox select z;
				foreach (WeSayTextBox box in justTextBoxes)
				{
					var binding = new TextBinding(
						_currentOption.Name,
						box.WritingSystem.Id,
						box
					);
					//hooking on to this is more reliable, sequence-wise, than directly wiring to m.TextChanged
					//JH (2011): Maybe so, but this doesn't fire until they tab away, and that's too late.
					binding.DataTarget.PropertyChanged += DataTarget_PropertyChanged;
				}
				AdjustLocations();
				UpdateDisplay();
			}
		}

		private void OnNameControlSizeChanged(object sender, EventArgs e)
		{
			AdjustLocations();
		}

		private void AdjustLocations()
		{
			_keyLabel.Left = _nameMultiTextControl.Left;
			_keyLabel.Top = _nameMultiTextControl.Bottom + 20;
			_keyLabel.Width = _nameMultiTextControl.Width;
		}

		private void SaveEditsToCurrentItem()
		{
			if (_currentOption == null)
			{
				return;
			}
			_currentOption.Key = ProposedKey;
		}

		protected string ProposedKey
		{
			get
			{
				if (_newlyCreatedOptions.Contains(_currentOption))
				{
					//we prefer a major language for the key
					var key =
						_nameMultiTextControl.GetMultiText().GetBestAlternativeString(new[]
																						  {
																							  "en", "fr", "th", "es", "pt",
																							  "id"
																						  });
					if (!string.IsNullOrEmpty(key))
						return key;
				}
				return _currentOption.Key;
			}
		}

		private void DataTarget_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateKeyLabel();
			//will automatically change as we type, if the key isn't set yet

			//this weirdness is because I couldn't get the list item to update
			// with any of the normal, documented means (e.g. _listBox.Text)
			_listBox.Items[_listBox.SelectedIndex] = _listBox.SelectedItem;
			UserModifiedList();
		}

		private void UpdateKeyLabel()
		{
			_keyLabel.Text = "Id in LIFT file: " + ProposedKey;
		}

		private void _btnAdd_Click(object sender, EventArgs e)
		{
			_listBox.Focus();   //This is a hack to get the TextBinding to update by losing focus :-(
			var newOption = new Option();
			_newlyCreatedOptions.Add(newOption);
			_currentList.Options.Add(newOption);

			int index = _listBox.Items.Add(newOption.GetDisplayProxy(PreferredWritingSystem));
			_listBox.SelectedIndex = index;
			UpdateDisplay();
			if (_nameMultiTextControl.TextBoxes.Count == 0)
			{
			}
			_nameMultiTextControl.TextBoxes[0].Focus();
			UserModifiedList();
		}

		private void _btnDelete_Click(object sender, EventArgs e)
		{
			_listBox.Focus();   //This is a hack to get the TextBinding to update by losing focus :-(
			_currentList.Options.Remove(_currentOption);
			_listBox.Items.Remove(_listBox.SelectedItem);

			if (_listBox.Items.Count > 0)
			{
				_listBox.SelectedIndex = 0;
			}
			UpdateDisplay();
			UserModifiedList();
		}

		private void UserModifiedList()
		{
			WeSayWordsProject.Project.MarkOptionListAsUpdated(_currentList);
		}

		private void UpdateDisplay()
		{
			_btnDelete.Enabled = _listBox.SelectedItem != null && _listBox.Items.Count>1;
			UpdateKeyLabel();
			_nameMultiTextControl.Visible = _listBox.SelectedItem != null;
			_btnAdd.Enabled = (null != _currentField) ;
		}

		private void OnFieldChooser_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_fieldChooser.SelectedItem != null)
			{
				var f = _fieldChooser.SelectedItem as Field;
				LoadList(f);
			}
		}

	}
}