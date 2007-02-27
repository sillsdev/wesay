using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using WeSay.Language;
using WeSay.Project;

namespace WeSay.Setup
{
	public partial class FieldsControl : UserControl
	{
		public FieldsControl()
		{
			InitializeComponent();
			//don't want grey
			_descriptionBox.BackColor = System.Drawing.SystemColors.Window;
			_descriptionBox.ForeColor = System.Drawing.SystemColors.WindowText;
		}

		private void FieldsControl_Load(object sender, EventArgs e)
		{
			if (this.DesignMode)
				return;

			LoadInventory();
			//nb: may important to do this after loading the inventory
			this._fieldsListBox.ItemCheck += new ItemCheckEventHandler(this._fieldsListBox_ItemCheck);
			if (_fieldsListBox.Items.Count > 0)
			{
				_fieldsListBox.SelectedIndex = 0;
			}

		}

		/// <summary>
		/// Construct the list of fields to show.
		/// </summary>
		private void LoadInventory()
		{
			_fieldsListBox.Items.Clear();

			foreach (Field field in  WeSayWordsProject.Project.ViewTemplate)
			{
				this._fieldsListBox.Items.Add(field, field.Visibility == Field.VisibilitySetting.Visible);
			}
		}

		private void _fieldsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (e.NewValue== CheckState.Checked)
			{
				((Field)_fieldsListBox.SelectedItem).Visibility = Field.VisibilitySetting.Visible;
			}
			else
			{
				CurrentField.Visibility = Field.VisibilitySetting.Invisible;
			}
		}

		private Field CurrentField
		{
			get
			{
				return (Field)_fieldsListBox.SelectedItem;
			}
		}

		private void _fieldsListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			_descriptionBox.Text = CurrentField.Description;
			LoadWritingSystemBox();
			LoadAboutFieldBox();
		}


		private void LoadWritingSystemBox()
		{
			_writingSystemListBox.Items.Clear();
			foreach (WritingSystem ws in  BasilProject.Project.WritingSystems.Values)
			{
				int i= _writingSystemListBox.Items.Add(ws);
				_writingSystemListBox.SetItemChecked(i, CurrentField.WritingSystemIds.Contains(ws.Id));
			}
		}
		private void LoadAboutFieldBox()
		{
			_descriptionBox.Text = String.Format("{0} ({1}).  {2}", CurrentField.DisplayName, CurrentField.FieldName, CurrentField.Description);
		}
		private void _writingSystemListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			//happens when we are setting initial checkbox states from code
			if (_writingSystemListBox.SelectedItem == null)
				return;

			if (e.NewValue == CheckState.Checked)
			{
				CurrentField.WritingSystemIds.Add(CurrentWritingSystemId);
			}
			else
			{
				CurrentField.WritingSystemIds.Remove(CurrentWritingSystemId);
			}
		}

		private string CurrentWritingSystemId
		{
			get
			{
				return _writingSystemListBox.SelectedItem.ToString();
			}
		}
	}
}
