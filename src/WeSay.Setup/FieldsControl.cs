using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Reporting;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.Project;

namespace WeSay.Setup
{
	public partial class FieldsControl : ConfigurationControlBase
	{

		public FieldsControl()
			: base("set up the fields for the dictionary")
		{
			InitializeComponent();
			//don't want grey
			_descriptionBox.BackColor = SystemColors.Window;
			_descriptionBox.ForeColor = SystemColors.WindowText;
			this.Resize +=new EventHandler(FieldsControl_Resize);
		}




		private void FieldsControl_Load(object sender, EventArgs e)
		{
			if (DesignMode)
				return;

			LoadInventory();
			//nb: may important to do this after loading the inventory
			this._fieldsListBox.ItemCheck += new ItemCheckEventHandler(this.OnFieldsListBox_ItemCheck);

		}


		/// <summary>
		/// Construct the list of fields to show.
		/// </summary>
		private void LoadInventory()
		{
			_fieldsListBox.Items.Clear();

			foreach (Field field in  WeSayWordsProject.Project.DefaultViewTemplate)
			{
				this._fieldsListBox.Items.Add(field, field.Enabled);
			}

			if (_fieldsListBox.Items.Count > 0)
			{
				_fieldsListBox.SelectedIndex = 0;
			}
		}


		private void OnFieldsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (_fieldsListBox.SelectedItem == null)//this gets called during population of the list,too
			{
				return;
			}
			if (e.NewValue== CheckState.Checked)
			{
				((Field) _fieldsListBox.SelectedItem).Enabled = true;
			}
			else if (e.NewValue== CheckState.Unchecked)
			{
				if(CurrentField.CanOmitFromMainViewTemplate)
				{
					CurrentField.Enabled = false;
				}
				else
				{
					e.NewValue = CheckState.Checked; //revert
				}
			}
		}

		private Field CurrentField
		{
			get
			{
				if (_fieldsListBox.SelectedItem == null && _fieldsListBox.Items.Count>0)
				{
					_fieldsListBox.SelectedItem = _fieldsListBox.Items[0];
				}
				return _fieldsListBox.SelectedItem as Field;
			}
		}

		private void OnSelectedFieldChanged(object sender, EventArgs e)
		{
			if (CurrentField == null)
			{
				return;
			}
			_btnDeleteField.Enabled = CurrentField.UserCanDeleteOrModify;
			_descriptionBox.Text = CurrentField.Description;

			LoadAboutFieldBox();

			//todo(WS-364): this is too blunt. They should be able to edit the display name
		//    _fieldPropertyGrid.Enabled = CurrentField.UserCanDeleteOrModify;

		//    _fieldPropertyGrid.SelectedObject = CurrentField;
			_fieldSetupControl.CurrentField = CurrentField;
		}



		private void LoadAboutFieldBox()
		{
			_descriptionBox.Text = CurrentField.Description;
		//    _descriptionBox.Text = String.Format("{0} ({1}).  {2}", CurrentField.DisplayName, CurrentField.FieldName, CurrentField.Description);
		}


//        private void groupBox2_SizeChanged(object sender, EventArgs e)
//        {
//            _descriptionBox.MaximumSize  = new Size(groupBox1.Width - 30,groupBox1.Height -30);
//        }

		private void OnAddField_Click(object sender, EventArgs e)
		{
			Field f = new Field(MakeUniqueFieldName(), "LexEntry", WeSayWordsProject.Project.WritingSystems.Keys);
			WeSayWordsProject.Project.DefaultViewTemplate.Fields.Add(f);
			LoadInventory();
			_tabControl.SelectedTab = _setupTab;
			MakeFieldTheSelectedOne(f);
		}
		private string MakeUniqueFieldName()
		{
			string baseName = Field.NewFieldNamePrefix;
			for (int count = 0; count<1000 ; count++)
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
			return WeSayWordsProject.Project.DefaultViewTemplate.Fields.Find(delegate(Field f)
																				  {
																					  return f.FieldName == name;
																				  });
		}

		private void MakeFieldTheSelectedOne(Field f)
		{
			foreach (object o in _fieldsListBox.Items )
			{
				if (o == f)
				{
					_fieldsListBox.SelectedItem = o;
					break;
				}
			}
		}

		private void OnDeleteField_Click(object sender, EventArgs e)
		{
			if (CurrentField ==null)
				return;
			if (!CurrentField.UserCanDeleteOrModify)
				return;

			int index = _fieldsListBox.SelectedIndex;
			WeSayWordsProject.Project.DefaultViewTemplate.Fields.Remove(CurrentField);
			LoadInventory();
			if(_fieldsListBox.Items.Count>0)
			{
				_fieldsListBox.SelectedIndex = index -1;//select the item before the deleted one
			}
		}

		private void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
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
					ErrorReport.ReportNonFatalMessage(
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

				List<Field> fields = WeSayWordsProject.Project.DefaultViewTemplate.Fields.FindAll(delegate(Field f)
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
					ErrorReport.ReportNonFatalMessage(
						"The field '{0}' with DisplayName '{1}' on class '{2}' is already using that name. Please choose another one.",
						f.FieldName, f.DisplayName, f.ClassName);
					CurrentField.FieldName = (string)e.OldValue;
					return;
				}

				WeSayWordsProject.Project.MakeFieldNameChange(CurrentField, (string) e.OldValue);
			}
		}

		private void FieldsControl_Resize(object sender, EventArgs e)
		{
			try//I've seen this crash when window is really small
			{
				//this is part of dealing with .net not adjusting stuff well for different dpis
				splitContainer1.Dock = DockStyle.None;
				splitContainer1.Width = this.Width - 20;
			}
			catch (Exception)
			{
				//swallow
			}
		}



	}

}
