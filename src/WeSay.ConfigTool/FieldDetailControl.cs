using System;
using System.IO;
using System.Windows.Forms;
using Enchant;
using Palaso.DictionaryServices.Model;
using Palaso.Lift;
using Palaso.Reporting;
using WeSay.Project;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.ConfigTool
{
	public partial class FieldDetailControl: UserControl
	{
		private Field _field;
		private bool _loading;

		public event EventHandler ClassOfFieldChanged;
		public event EventHandler DisplayNameOfFieldChanged;
		public event EventHandler DescriptionOfFieldChanged = delegate { };

		public FieldDetailControl()
		{
			InitializeComponent();
			if (DesignMode)
			{
				return;
			}

			toolTip1.SetToolTip(_displayName, toolTip1.GetToolTip(_displayLabel));
			toolTip1.SetToolTip(_fieldName, toolTip1.GetToolTip(_fieldNameLabel));
			toolTip1.SetToolTip(_description, toolTip1.GetToolTip(_descriptionLabel));
			toolTip1.SetToolTip(_optionsFileName, toolTip1.GetToolTip(_optionListFileLabel));
			toolTip1.SetToolTip(_dataTypeCombo, toolTip1.GetToolTip(_displayLabel));
			toolTip1.SetToolTip(_classNameCombo, toolTip1.GetToolTip(_classLabel));
			toolTip1.SetToolTip(_normallyHidden, toolTip1.GetToolTip(_normallyHiddenLabel));
			toolTip1.SetToolTip(_enableSpelling, toolTip1.GetToolTip(_enableSpellingLabel));
			if (IsEnchantInstalled())
			{
				spellingNotEnabledWarning.Visible = false;
			}
		}

		private static bool IsEnchantInstalled()
		{
			bool enchantInstalled = true;
			try
			{
				using (new Broker()) {}
			}
			catch
			{
				enchantInstalled = false;
			}
			return enchantInstalled;
		}

		public Field CurrentField
		{
			set
			{
				if (value == _field)
				{
					// refresh these since they might have changed on another tab
					_writingSystemsControl.CurrentField = value;
					return;
				}

				_loading = true;
				_field = value;
				_fieldName.Text = _field.FieldName;
				_displayName.Text = _field.DisplayName;
				_optionsFileName.Text = _field.OptionsListFile;
				_description.Text = _field.Description;
				_enableSpelling.Checked = _field.IsSpellCheckingEnabled;
				_normallyHidden.Checked = _field.Visibility ==
										  CommonEnumerations.VisibilitySetting.NormallyHidden;
				_multiParagraph.Checked = _field.IsMultiParagraph;

				FillClassNameCombo();
				FillDataTypeCombo();
				_writingSystemsControl.CurrentField = value;
				UpdateDisplay();
				_displayName.SelectAll();
				_displayName.Focus();
				_loading = false;
			}
		}

		private static void AddComboItem(ComboBox combo,
										 object currentValue,
										 object choiceValue,
										 string choiceLabel)
		{
			int i = combo.Items.Add(new ComboItemProxy(choiceValue, choiceLabel));
			if (choiceValue.ToString() == currentValue.ToString())
					//tostring defeats the generality, but the == failed without it
			{
				combo.SelectedIndex = i;
			}
		}

		private static void SelectComboItem(ComboBox combo, string desiredValueName)
		{
			foreach (ComboItemProxy proxy in combo.Items)
			{
				if (proxy.UnderlyingValue.ToString() == desiredValueName)
						//tostring defeats the generality, but the == failed without it
				{
					combo.SelectedItem = proxy;
					break;
				}
			}
		}

		public class ComboItemProxy
		{
			private readonly string _label;
			private object _value;

			public ComboItemProxy(object value, string label)
			{
				_label = label;
				_value = value;
			}

			public Object UnderlyingValue
			{
				get { return _value; }
				set { _value = value; }
			}

			public override string ToString()
			{
				return _label;
			}
		}

		private void FillClassNameCombo()
		{
			_classNameCombo.Items.Clear();
			AddComboItem(_classNameCombo, _field.ClassName, "LexEntry", "Entry/Lexeme/Word");
			AddComboItem(_classNameCombo, _field.ClassName, "LexSense", "Sense");
			AddComboItem(_classNameCombo, _field.ClassName, "LexExampleSentence", "Example Sentence");
		}

		private void FillDataTypeCombo()
		{
			_dataTypeCombo.Items.Clear();
			AddComboItem(_dataTypeCombo,
						 _field.DataTypeName,
						 Field.BuiltInDataType.MultiText,
						 "Text in one or more input systems");
			AddComboItem(_dataTypeCombo,
						 _field.DataTypeName,
						 Field.BuiltInDataType.Option,
						 "Choice from a list of options");
			AddComboItem(_dataTypeCombo,
						 _field.DataTypeName,
						 Field.BuiltInDataType.OptionCollection,
						 "Multiple choices from a list of options");
			AddComboItem(_dataTypeCombo,
						 _field.DataTypeName,
						 Field.BuiltInDataType.Picture,
						 "Picture");
			AddComboItem(_dataTypeCombo,
						 _field.DataTypeName,
						 Field.BuiltInDataType.RelationToOneEntry,
						 "Relation to a single entry");
		}

		private void UpdateDisplay()
		{
			if (_field.FieldName == LexSense.WellKnownProperties.SemanticDomainDdp4)
			{
				_optionListFileLabel.Visible = false;
			}
			else
			{
				_optionListFileLabel.Visible = _optionsFileName.Visible = _field.ShowOptionListStuff;
			}
			bool isFieldMultiText = _field.DataTypeName ==
									Field.BuiltInDataType.MultiText.ToString();
			_enableSpellingLabel.Visible = _enableSpelling.Visible = isFieldMultiText;

			if (String.IsNullOrEmpty(_fieldName.Text))
			{
				_fieldName.Text = _displayName.Text;
			}
			_optionsFileName.Enabled = _field.UserCanDeleteOrModify;
			_fieldName.Enabled = _field.UserCanDeleteOrModify;
			_classNameCombo.Enabled = _field.UserCanDeleteOrModify;
			_dataTypeCombo.Enabled = _field.UserCanDeleteOrModify;
			_description.Enabled = _field.UserCanDeleteOrModify;
			_normallyHidden.Enabled = _field.CanOmitFromMainViewTemplate;
			_multiParagraphLabel.Visible =
			_multiParagraph.Visible = _field.DataTypeName == Field.BuiltInDataType.MultiText.ToString();
			_multiParagraph.Enabled = _field.UserCanDeleteOrModify;
			_writingSystemsControlLabel.Visible =
			_writingSystemsControl.Visible = _field.UserCanModifyWritingSystems;
			_enableSpelling.Visible = _field.UserCanModifySpellCheckFeature;

		}

		private void OnLeaveDisplayName(object sender, EventArgs e)
		{
			if (_fieldName.Text.StartsWith(Field.NewFieldNamePrefix) &&
				_displayName.Text.Trim().Length > 0)
			{
				_fieldName.Text = _displayName.Text;
			}
			UpdateDisplay();
		}

		private void OnDisplayName_TextChanged(object sender, EventArgs e)
		{
			_field.DisplayName = _displayName.Text.Trim();

			if (DisplayNameOfFieldChanged != null && !_loading)
			{
				DisplayNameOfFieldChanged.Invoke(this, null);
			}
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
				if(!WeSayWordsProject.Project.MakeFieldNameChange(_field, oldValue))
				{
					//we couldn't do it, so back out
					_field.FieldName = oldValue;
					_fieldName.Text = _field.FieldName;
				}
			}
		}

		private void _description_TextChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{

				_field.Description = _description.Text.Trim();
				DescriptionOfFieldChanged.Invoke(this, e);
			}
		}

		private void _normallyHidden_CheckedChanged(object sender, EventArgs e)
		{
			if (_normallyHidden.Checked)
			{
				_field.Visibility = CommonEnumerations.VisibilitySetting.NormallyHidden;
			}
			else
			{
				_field.Visibility = CommonEnumerations.VisibilitySetting.Visible;
			}
		}



		private void _enableSpelling_CheckedChanged(object sender, EventArgs e)
		{
			_field.IsSpellCheckingEnabled = _enableSpelling.Checked;
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
				s = s.Remove(i, 1);
			}

			_optionsFileName.Text = s;
			_field.OptionsListFile = _optionsFileName.Text;
		}

		private string GetDefaultOptionsListFileName()
		{
			return _fieldName.Text + ".xml";
		}

		private void _classNameCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_loading)
			{
				return;
			}

			if (CheckClassNameChange())
			{
				ComboItemProxy proxy = _classNameCombo.SelectedItem as ComboItemProxy;
				if (proxy == null)
				{
					return;
				}
				_field.ClassName = proxy.UnderlyingValue.ToString();
				UpdateDisplay();
				if (ClassOfFieldChanged != null && !_loading)
				{
					ClassOfFieldChanged.Invoke(this, null);
				}
			}
			else //revert
			{
				_loading = true; //don't check this
				SelectComboItem(_classNameCombo, _field.ClassName);
				_loading = false;
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <returns>false if the change was rejected</returns>
		private bool CheckClassNameChange()
		{
			bool validChange = true;
			string newClassName =
				((ComboItemProxy) _classNameCombo.SelectedItem).UnderlyingValue.ToString();
			if (_field.DataTypeName == Field.BuiltInDataType.Picture.ToString()
				&& (newClassName != "LexSense"))
				{
					ErrorReport.NotifyUserOfProblem("Sorry, WeSay cannot set the type of this field to '{0}'. Pictures are only supported on Sense",
						newClassName);
					validChange = false;
				}
			return validChange;
		}

		private void OnDataTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				if (CheckDataTypeChange())
				{
					ComboItemProxy proxy = _dataTypeCombo.SelectedItem as ComboItemProxy;
					if (proxy == null)
					{
						return;
					}
					_field.DataTypeName = proxy.UnderlyingValue.ToString();
					if (_field.ShowOptionListStuff)
					{
						if (_optionsFileName.Text.Trim().Length == 0)
						{
							_optionsFileName.Text = GetDefaultOptionsListFileName();
						}
					}
				}
				else //revert
				{
					_loading = true; //don't check this
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
			string newDataTypeName =
					((ComboItemProxy) _dataTypeCombo.SelectedItem).UnderlyingValue.ToString();
			string oldDataTypeName = _field.DataTypeName;

			bool conflictFound = false;
			if (newDataTypeName == Field.BuiltInDataType.MultiText.ToString())
			{
				//we can't go from a option or option collection to multitext, if there is already data
				conflictFound = WeSayWordsProject.Project.LiftHasMatchingElement("trait",
																				 "name",
																				 _field.FieldName);
			}
			else if (newDataTypeName == Field.BuiltInDataType.Option.ToString())
			{
				//we can't go from an option collection to to a simple option, if there is already data
				if (_field.DataTypeName == Field.BuiltInDataType.OptionCollection.ToString())
				{
					conflictFound = WeSayWordsProject.Project.LiftHasMatchingElement("trait",
																					 "name",
																					 _field.
																							 FieldName);
				}
				//we can't go from a multitext to to a simple option, if there is already data
				conflictFound = conflictFound ||
								WeSayWordsProject.Project.LiftHasMatchingElement("field",
																				 "type",
																				 _field.FieldName);
			}
			else if (newDataTypeName == Field.BuiltInDataType.OptionCollection.ToString())
			{
				//we can't go from a multitext to to a option collection, if there is already data
				conflictFound = WeSayWordsProject.Project.LiftHasMatchingElement("field",
																				 "type",
																				 _field.FieldName);
			}
			else if (newDataTypeName == Field.BuiltInDataType.Picture.ToString()
				&& (_field.ClassName != "LexSense"))
			{
				ErrorReport.NotifyUserOfProblem("Sorry, WeSay cannot set the type of this field to '{0}'. Pictures are only supported on Sense",
					newDataTypeName);
				return false;
			}

			if (conflictFound)
			{
				ErrorReport.NotifyUserOfProblem(
						"Sorry, WeSay cannot change the type of this field to '{0}', because there is existing data in the LIFT file of the old type, '{1}'",
						newDataTypeName,
						oldDataTypeName);
			}
			return !conflictFound;
		}


		private void _multiParagraph_CheckedChanged(object sender, EventArgs e)
		{
			if (_multiParagraph.Checked)
			{
				_field.IsMultiParagraph = true;
			}
			else
			{
				_field.IsMultiParagraph = false;
			}
		}
	}
}