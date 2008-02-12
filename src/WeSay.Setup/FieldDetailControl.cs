using System;
using System.IO;
using System.Windows.Forms;
using Palaso.Reporting;
using WeSay.Foundation;
using WeSay.Project;

namespace WeSay.Setup
{
	public partial class FieldDetailControl : UserControl
	{
		private Field _field;
		private bool _loading;

		public event EventHandler ClassOfFieldChanged;

		public FieldDetailControl()
		{
			InitializeComponent();
			this.toolTip1.SetToolTip(this._displayName, toolTip1.GetToolTip(_displayLabel));
			this.toolTip1.SetToolTip(this._fieldName,  toolTip1.GetToolTip(_fieldNameLabel));
			this.toolTip1.SetToolTip(this._description,  toolTip1.GetToolTip(_descriptionLabel));
			this.toolTip1.SetToolTip(this._optionsFileName,  toolTip1.GetToolTip(_optionListFileLabel));
			this.toolTip1.SetToolTip(this._dataTypeCombo,  toolTip1.GetToolTip(_displayLabel));
			this.toolTip1.SetToolTip(this._classNameCombo, toolTip1.GetToolTip(_classLabel));
			 this.toolTip1.SetToolTip(this._normallyHidden, toolTip1.GetToolTip(_normallyHiddenLabel));
	   }

		public Field CurrentField
		{
			set
			{
				if(value == _field )
					return;

				_loading = true;
			   _field = value;
				_fieldName.Text = _field.FieldName;
				_displayName.Text = _field.DisplayName;
				_optionsFileName.Text = _field.OptionsListFile;
				_description.Text = _field.Description;

				_normallyHidden.Checked = _field.Visibility == CommonEnumerations.VisibilitySetting.NormallyHidden;

				FillClassNameCombo();
				FillDataTypeCombo();
				_writingSystemsControl.CurrentField = value;
				UpdateDisplay();
				_displayName.SelectAll();
				_displayName.Focus();
				_loading = false;
			}
		}


		private void AddComboItem(ComboBox combo, object currentValue, object choiceValue, string choiceLabel)
		{
			int i = combo.Items.Add(new ComboItemProxy(choiceValue, choiceLabel));
			if (choiceValue.ToString() == currentValue.ToString()) //tostring defeats the generality, but the == failed without it
			{
				combo.SelectedIndex = i;
			}
		}

		private void SelectComboItem(ComboBox combo, string desiredValueName)
		{
			foreach (ComboItemProxy proxy in combo.Items)
			{
				if (proxy.UnderlyingValue.ToString() == desiredValueName) //tostring defeats the generality, but the == failed without it
				{
					combo.SelectedItem = proxy;
					break;
				}
			}
		}

		public class ComboItemProxy
		{
			private string _label;
			private object _value;

			public ComboItemProxy(object value, string label)
			{
				_label = label;
				_value = value;
			}


			public Object UnderlyingValue
			{
				get
				{
					return _value;
				}
				set
				{
					_value = value;
				}
			}

			public override string ToString()
			{
				return _label;
			}
		}

		private void FillClassNameCombo()
		{
			_classNameCombo.Items.Clear();
			AddComboItem(_classNameCombo, _field.ClassName,"LexEntry", "Entry/Lexeme/Word");
			AddComboItem(_classNameCombo, _field.ClassName, "LexSense", "Sense");
			AddComboItem(_classNameCombo, _field.ClassName, "LexExampleSentence", "Example Sentence");
		}

		private void FillDataTypeCombo()
		{
			_dataTypeCombo.Items.Clear();
			AddComboItem(_dataTypeCombo, _field.DataTypeName, Field.BuiltInDataType.MultiText, "Text in one or more writing systems");
			AddComboItem(_dataTypeCombo, _field.DataTypeName, Field.BuiltInDataType.Option,"Choice from a list of options");
			AddComboItem(_dataTypeCombo, _field.DataTypeName,  Field.BuiltInDataType.OptionCollection, "Multiple choices from a list of options");
			AddComboItem(_dataTypeCombo, _field.DataTypeName,  Field.BuiltInDataType.Picture, "Picture");
			AddComboItem(_dataTypeCombo, _field.DataTypeName, Field.BuiltInDataType.RelationToOneEntry, "Relation to a single entry");

		}

		private void UpdateDisplay()
		{
			_optionListFileLabel.Visible = _optionsFileName.Visible = _field.ShowOptionListStuff;
			if(String.IsNullOrEmpty(_fieldName.Text))
			{
				_fieldName.Text = _displayName.Text;
			}
			_optionsFileName.Enabled = _field.UserCanDeleteOrModify;
			_fieldName.Enabled = _field.UserCanDeleteOrModify;
			_classNameCombo.Enabled = _field.UserCanDeleteOrModify;
			_dataTypeCombo.Enabled = _field.UserCanDeleteOrModify;
			_description.Enabled = _field.UserCanDeleteOrModify;

			_normallyHidden.Enabled = _field.CanOmitFromMainViewTemplate;

		}

		private void OnLeaveDisplayName(object sender, EventArgs e)
		{
			if (_fieldName.Text.StartsWith(Field.NewFieldNamePrefix) && _displayName.Text.Trim().Length > 0)
			{
				_fieldName.Text = _displayName.Text;
			}
			UpdateDisplay();
		}

		private void _displayName_TextChanged(object sender, EventArgs e)
		{
			_field.DisplayName = _displayName.Text.Trim();
		}

		private void _fieldName_TextChanged(object sender, EventArgs e)
		{

			string oldValue = _field.FieldName;
			_fieldName.Text = Field.MakeFieldNameSafe(_fieldName.Text);
			_field.FieldName = _fieldName.Text;
			if (string.IsNullOrEmpty(_field.FieldName))
			{
				_field.FieldName = oldValue;
				return;
			}
			if (_field.FieldName != oldValue)
			{
				WeSayWordsProject.Project.MakeFieldNameChange(_field, oldValue);
			}
		}

		private void _description_TextChanged(object sender, EventArgs e)
		{
			_field.Description = _description.Text.Trim();

		}


		private void _normallyHidden_CheckedChanged(object sender, EventArgs e)
		{
			if(_normallyHidden.Checked)
			{
				_field.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			}
			else
			{
				_field.Visibility = CommonEnumerations.VisibilitySetting.Visible;
			}
		}

		private void _optionsFileName_TextChanged(object sender, EventArgs e)
		{
			string s = _optionsFileName.Text.Trim();

//            if (s.Length < 1)
//            {
//                s = GetDefaultOptionsListFileName();
//            }


			while (s.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
			{
				int i = s.IndexOfAny(Path.GetInvalidFileNameChars());
				s=s.Remove(i, 1);
			}

			_optionsFileName.Text = s;
			_field.OptionsListFile = _optionsFileName.Text;
		}



		private string GetDefaultOptionsListFileName()
		{
			return _fieldName.Text+".xml";
		}

		private void _classNameCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_loading)
				return;

			ComboItemProxy proxy = _classNameCombo.SelectedItem as ComboItemProxy;
			if (proxy == null)
				return;
			_field.ClassName = proxy.UnderlyingValue.ToString();
			UpdateDisplay();
			if (ClassOfFieldChanged != null)
			{
				ClassOfFieldChanged.Invoke(this, null);
			}
		}

		private void OnDataTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loading )
			{
				if (CheckDataTypeChange())
				{
					ComboItemProxy proxy = _dataTypeCombo.SelectedItem as ComboItemProxy;
					if (proxy == null)
						return;
					_field.DataTypeName = proxy.UnderlyingValue.ToString();
					if (_field.ShowOptionListStuff)
					{
						if (_optionsFileName.Text.Trim().Length == 0)
						{
							_optionsFileName.Text = GetDefaultOptionsListFileName();
						}
					}
				}
				else//revert
				{
					_loading = true;//don't check this
					SelectComboItem(_dataTypeCombo, _field.DataTypeName);
					_loading = false;
				}
			}

			UpdateDisplay();
		}

		/// <summary>
		///
		/// </summary>
		/// <returns>false if the change was rejected</returns>
		private bool CheckDataTypeChange()
		{
			string newDataTypeName = ((ComboItemProxy) _dataTypeCombo.SelectedItem).UnderlyingValue.ToString();
			string oldDataTypeName = _field.DataTypeName;

			bool conflictFound = false;
			if (newDataTypeName == Field.BuiltInDataType.MultiText.ToString())
			{
				//we can't go from a option or option collection to multitext, if there is already data
				conflictFound =
					WeSayWordsProject.Project.LiftHasMatchingElement("trait", "name", _field.FieldName);
			}
			else if (newDataTypeName == Field.BuiltInDataType.Option.ToString())
			{
				//we can't go from an option collection to to a simple option, if there is already data
				if ((string)_field.DataTypeName  == Field.BuiltInDataType.OptionCollection.ToString())
				{
					conflictFound =
						WeSayWordsProject.Project.LiftHasMatchingElement("trait", "name",
																				 _field.FieldName);
				}
				//we can't go from a multitext to to a simple option, if there is already data
				conflictFound = conflictFound ||
					WeSayWordsProject.Project.LiftHasMatchingElement("field", "tag", _field.FieldName);
			}
			else if (newDataTypeName == Field.BuiltInDataType.OptionCollection.ToString())
			{
				//we can't go from a multitext to to a option collection, if there is already data
				conflictFound =
					WeSayWordsProject.Project.LiftHasMatchingElement("field", "tag", _field.FieldName);
			}

			if (conflictFound)
			{
				ErrorReport.ReportNonFatalMessage(
					"Sorry, WeSay cannot change the type of this field to '{0}', because there is existing data in the LIFT file of the old type, '{1}'",
					newDataTypeName, oldDataTypeName);
			}
			return !conflictFound;
		}



	}
}
