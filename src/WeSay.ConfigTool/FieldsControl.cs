using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Palaso.i18n;
using Palaso.Reporting;
using Palaso.WritingSystems;
using WeSay.ConfigTool.Properties;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.ConfigTool
{
	public partial class FieldsControl: ConfigurationControlBase
	{
		private bool _loading = false;

		public FieldsControl(ILogger logger): base("set up the fields for the dictionary", logger)
		{
			InitializeComponent();
			_btnAddField.Image = Resources.genericLittleNewButton;
			_btnDeleteField.Image = Resources.GenericLittleDeletionButton;
			_fieldSetupControl.DescriptionOfFieldChanged +=
					_fieldSetupControl_DescriptionOfFieldChanged;
		}

		private void _fieldSetupControl_DescriptionOfFieldChanged(object sender, EventArgs e)
		{
			_fieldsListBox.SelectedItems[0].ToolTipText = CurrentField.Description;
			ReportEdit(StringCatalog.Get("Set description of field '{0}'", "Checkin description when setting the description of field in WeSay Configuration Tool."), CurrentField.Key);
		}

		private void FieldsControl_Load(object sender, EventArgs e)
		{
			if (DesignMode)
			{
				return;
			}

			LoadInventory();
			//nb: may important to do this after loading the inventory
			_fieldsListBox.ItemCheck += OnFieldsListBox_ItemCheck;
			_fieldSetupControl.ClassOfFieldChanged += OnClassOfFieldChanged;
			_fieldSetupControl.DisplayNameOfFieldChanged += OnNameOfFieldChanged;
		}

		private void OnNameOfFieldChanged(object sender, EventArgs e)
		{
			if (_fieldsListBox.SelectedItems.Count == 0)
			{
				return;
			}

			var f = (Field) _fieldsListBox.SelectedItems[0].Tag;
			_fieldsListBox.SelectedItems[0].Text = f.DisplayName;
		}

		private void OnClassOfFieldChanged(object sender, EventArgs e)
		{
			Field f = CurrentField;
			ViewTemplate.MoveToLastInClass(f);
			LoadInventory(); // show it in its new location
			MakeFieldTheSelectedOne(f);
			ReportEdit(
					StringCatalog.Get("Set class of field '{0}'",
									  "Checkin description when setting the kind of field in WeSay Configuration Tool."),
					f.Key);

		}

		/// <summary>
		/// Construct the list of fields to show.
		/// </summary>
		private void LoadInventory()
		{
			_loading = true;
			_fieldsListBox.Items.Clear();

			foreach (Field field in  ViewTemplate)
			{
				var item = new ListViewItem(field.DisplayName);
				item.Tag = field;
				item.Text = field.DisplayName;
				item.Checked = field.Enabled;
				item.ToolTipText = field.Description;
				foreach (ListViewGroup group in _fieldsListBox.Groups)
				{
					if (field.ClassName == group.Name)
					{
						item.Group = group;
					}
				}
				_fieldsListBox.Items.Add(item);
			}

			if (_fieldsListBox.Items.Count > 0)
			{
				_fieldsListBox.Items[0].Selected = true;
			}
			_loading = false;
		}

		private static ViewTemplate ViewTemplate
		{
			get { return WeSayWordsProject.Project.DefaultViewTemplate; }
		}

		private void OnFieldsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (CurrentField == null) //this gets called during population of the list,too
			{
				return;
			}

			//nb: this is not necessarily the Current Field!  you can click check boxes without selecting a different item
			var touchedField = (Field) _fieldsListBox.Items[e.Index].Tag;
			if (e.NewValue == CheckState.Checked)
			{
				touchedField.Enabled = true;
				//((Field) _fieldsListBox.SelectedItem).Enabled = true;
				ReportEdit(StringCatalog.Get("Enabled field '{0}'",
									  "Checkin description when enabling a field in WeSay Configuration Tool."), touchedField.Key);
			}
			else if (e.NewValue == CheckState.Unchecked)
			{
				if (touchedField.CanOmitFromMainViewTemplate)
				{
					touchedField.Enabled = false;
					_logger.WriteConciseHistoricalEvent(StringCatalog.Get("Disabled field '{0}'", "Checkin description when disabling a field in WeSay Configuration Tool."), touchedField.Key);
				}
				else
				{
					e.NewValue = CheckState.Checked; //revert
				}
			}
		}

		private void ReportEdit(string localizedMessageTemplate, params object[] stringArgs)
		{
			if (!_loading)
			{
				_logger.WriteConciseHistoricalEvent(localizedMessageTemplate, stringArgs);
			}
		}

		//        class FieldListBoxWrapper
		//        {
		//            public Field _field;
		//            private static StringDictionary _labels;
		//
		//            public FieldListBoxWrapper(Field f)
		//            {
		//                _field = f;
		//                if (_labels == null)
		//                {
		//                    _labels = new StringDictionary();
		//                    PopulateLabelDictionary();
		//                }
		//            }
		//
		//            private static void PopulateLabelDictionary()
		//            {
		//                _labels.Add("LexEntry", "Entry");
		//                _labels.Add("LexSense", "Sense");
		//                _labels.Add("LexExampleSentence", "Example");
		//            }
		//
		//            public override string ToString()
		//            {
		//                return string.Format("{0} ({1})", _field.DisplayName, _labels[_field.ClassName]);
		//            }
		//        }

		private Field CurrentField
		{
			get
			{
				if (_fieldsListBox.SelectedItems.Count == 0 && _fieldsListBox.Items.Count > 0)
				{
					_fieldsListBox.Items[0].Selected = true;
					//did not select in time for the return statement
					return _fieldsListBox.Items[0].Tag as Field;
					//_fieldsListBox.SelectedIndices.Add(0);
				}
				return (_fieldsListBox.SelectedItems[0].Tag) as Field;
			}
		}

		//        private void groupBox2_SizeChanged(object sender, EventArgs e)
		//        {
		//            _descriptionBox.MaximumSize  = new Size(groupBox1.Width - 30,groupBox1.Height -30);
		//        }

		private void OnAddField_Click(object sender, EventArgs e)
		{
			// TODO inject the IWritingSystemStore in the ctor
			var writingSystemStore = WeSayWordsProject.Project.WritingSystems;
			var writingSystemIds = writingSystemStore.AllWritingSystems.Select(ws => ws.Id);

			var f = new Field(
				MakeUniqueFieldName(),
				"LexEntry",
				writingSystemIds);
			ViewTemplate.Fields.Add(f);
			LoadInventory();
			_tabControl.SelectedTab = _setupTab;
			MakeFieldTheSelectedOne(f);
			ReportEdit(StringCatalog.Get("Added field", "Checkin description When adding a new field in WeSay Configuration Tool."));
		}

		private static string MakeUniqueFieldName()
		{
			string baseName = Field.NewFieldNamePrefix;
			for (int count = 0;count < 1000;count++)
			{
				string check = baseName;
				if (count > 0)
				{
					check += count.ToString();
				}
				if (null == FindFieldWithFieldName(check))
				{
					return check;
				}
			}
			//if can't find a  unique name (this will never happen)
			return baseName;
		}

		private static Field FindFieldWithFieldName(string name)
		{
			return ViewTemplate.Fields.Find(f => f.FieldName == name);
		}

		private void MakeFieldTheSelectedOne(Field f)
		{
			//            _fieldsListBox.SelectedIndices.Clear();
			//            _fieldsListBox.SelectedIndices.Add(GetItemOfField(f).Index);
			GetItemOfField(f).Selected = true;
		}

		private ListViewItem GetItemOfField(Field f)
		{
			foreach (ListViewItem item in _fieldsListBox.Items)
			{
				if (item.Tag == f)
				{
					return item;
				}
			}
			throw new ApplicationException(
					string.Format("Could not find the field {0} in th list view", f.DisplayName));
		}

		private void OnDeleteField_Click(object sender, EventArgs e)
		{
			if (CurrentField == null)
			{
				return;
			}
			if (!CurrentField.UserCanDeleteOrModify)
			{
				return;
			}

			int index = _fieldsListBox.SelectedIndices[0];
			ViewTemplate.Fields.Remove(CurrentField);
			ReportEdit(StringCatalog.Get("Remove field '{0}'", "Checkin description when deleting a field in WeSay Configuration Tool."), CurrentField.Key);

			LoadInventory();
			if (_fieldsListBox.Items.Count > 0)
			{
				_fieldsListBox.SelectedIndices.Clear();
				int indexToSelect = index == 0 ? 0 : index - 1;

				_fieldsListBox.Items[indexToSelect].Selected = true;
				//_fieldsListBox.SelectedIndices.Add(index - 1);//select the item before the deleted one
			}
		}

		/* private void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.Label == "DataTypeName")
			{
				//catch the case where the user is changing the type, but
				//there is already existing data in the other type

				bool found = false;
				if (CurrentField.DataTypeName == Field.BuiltInDataType.MultiText.ToString())
				{
					//we can't go from a option or option collection to multitext, if there is already data
					found =
						WeSayWordsProject.Project.LiftHasMatchingElement("trait", "name", CurrentField.FieldName);
				}
				else if (CurrentField.DataTypeName == Field.BuiltInDataType.Option.ToString())
				{
					//we can't go from an option collection to to a simple option, if there is already data
					if ((string)e.OldValue == Field.BuiltInDataType.OptionCollection.ToString())
					{
						found =
							WeSayWordsProject.Project.LiftHasMatchingElement("trait", "name",
																					 CurrentField.FieldName);
					}
					//we can't go from a multitext to to a simple option, if there is already data
					found = found ||
						WeSayWordsProject.Project.LiftHasMatchingElement("field", "tag", CurrentField.FieldName);
				}
				else if (CurrentField.DataTypeName == Field.BuiltInDataType.OptionCollection.ToString())
				{
					//we can't go from a multitext to to a option collection, if there is already data
					found =
						WeSayWordsProject.Project.LiftHasMatchingElement("field", "tag", CurrentField.FieldName);
				}

				if (found)
				{
					ErrorReport.NotifyUserOfProblem(
						"Sorry, WeSay cannot change the type of this field to '{0}', because there is existing data in the LIFT file of the old type, '{1}'",
						CurrentField.DataTypeName, e.OldValue);
					CurrentField.DataTypeName = (string)e.OldValue;
				}
			}

			if (e.ChangedItem.Label == "FieldName")
			{
				if (CurrentField.FieldName == String.Empty)
				{
   //                 e.ChangedItem.PropertyDescriptor.SetValue(_fieldPropertyGrid.SelectedObject, e.OldValue);
					return;
				}

				List<Field> fields = ViewTemplate.Fields.FindAll(delegate(Field f)
																					   {
																						   return f.FieldName == CurrentField.FieldName;
																					   });
				if (fields.Count > 1)
				{
					Field f = fields[0];
					if(f==CurrentField)
					{
						f = fields[1];
					}
					ErrorReport.NotifyUserOfProblem(
						"The field '{0}' with DisplayName '{1}' on class '{2}' is already using that name. Please choose another one.",
						f.FieldName, f.DisplayName, f.ClassName);
					CurrentField.FieldName = (string)e.OldValue;
					return;
				}

				WeSayWordsProject.Project.MakeFieldNameChange(CurrentField, (string) e.OldValue);
			}
		}*/

		private void OnSelectedFieldChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			btnMoveUp.Enabled = false;
			btnMoveDown.Enabled = false;

			if (!e.IsSelected)
			{
				return;
			}
			if (CurrentField == null)
			{
				return;
			}
			_btnDeleteField.Enabled = CurrentField.UserCanDeleteOrModify;
			//(WS-364): this is too blunt. They should be able to edit the display name
			//    _fieldPropertyGrid.Enabled = CurrentField.UserCanDeleteOrModify;

			//    _fieldPropertyGrid.SelectedObject = CurrentField;
			_fieldSetupControl.CurrentField = CurrentField;

			btnMoveUp.Enabled = CurrentField.UserCanRelocate &&
								!ViewTemplate.IsFieldFirstInClass(CurrentField);
			btnMoveDown.Enabled = CurrentField.UserCanRelocate &&
								  !ViewTemplate.IsFieldLastInClass(CurrentField);
		}

		private void btnMoveUp_Click(object sender, EventArgs e)
		{
			Field f = CurrentField;
			ViewTemplate.MoveUpInClass(CurrentField);
			LoadInventory();
			MakeFieldTheSelectedOne(f);
			ReportEdit(StringCatalog.Get("Adjusted field order", "Checkin description when moving a field in WeSay Configuration Tool."));
		}

		private void btnMoveDown_Click(object sender, EventArgs e)
		{
			Field f = CurrentField;
			ViewTemplate.MoveDownInClass(CurrentField);
			LoadInventory();
			MakeFieldTheSelectedOne(f);
			ReportEdit(StringCatalog.Get("Adjusted field order", "Checkin description when moving a field in WeSay Configuration Tool."));

		}
	}
}