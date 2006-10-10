using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.Admin
{
	public partial class FieldsControl : UserControl
	{
		public FieldsControl()
		{
			InitializeComponent();
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
		/// <remarks>
		/// The algorithm here is to fill the list with all of the fields from the master inventory.
		/// If a field is also found in the users existing inventory, turn on the checkbox,
		/// and set all of the writing systems to match what the user had before.
		/// Any fields that are in the user's inventory which are no longer in the
		/// master inventory will be thrown away.
		/// </remarks>
		private void LoadInventory()
		{
			_fieldsListBox.Items.Clear();

			if (AdminWindow.SharedFieldInventory == null)
			{
				AdminWindow.SharedFieldInventory = FieldInventory.MakeMasterInventory(BasilProject.Project.WritingSystems);
			}
			FieldInventory oldInventory = GetUsersExistingInventory();

			FieldInventory.ModifyMasterFromUser(AdminWindow.SharedFieldInventory, oldInventory);
			foreach (Field field in AdminWindow.SharedFieldInventory)
			{
				this._fieldsListBox.Items.Add(field, field.Visibility == Field.VisibilitySetting.Visible);
			}
	  }




		private static FieldInventory GetUsersExistingInventory()
		{
			FieldInventory oldInventory = new FieldInventory();
			try
			{
				XmlDocument projectDoc = GetProjectDoc();
				if (projectDoc != null)
				{
					XmlNode inventoryNode = projectDoc.SelectSingleNode("tasks/components/fieldInventory");
					oldInventory.LoadFromString(inventoryNode.OuterXml);
				}
			}
			catch (Exception error)
			{
				MessageBox.Show("There may have been a problem reading the master task inventory xml. " + error.Message);
			}
			return oldInventory;
		}



		private static XmlDocument GetProjectDoc()
		{
			XmlDocument projectDoc = null;
			if (File.Exists(WeSayWordsProject.Project.PathToProjectTaskInventory))
			{
				try
				{
					projectDoc = new XmlDocument();
					projectDoc.Load(WeSayWordsProject.Project.PathToProjectTaskInventory);
				}
				catch (Exception e)
				{
					MessageBox.Show("There was a problem reading the task xml. " + e.Message);
					projectDoc = null;
				}
			}
			return projectDoc;
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
