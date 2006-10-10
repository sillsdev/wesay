using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
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
			this._fieldsListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._fieldsListBox_ItemCheck);

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
				AdminWindow.SharedFieldInventory = MakeMasterInventory();
			}
			FieldInventory oldInventory = GetUsersExistingInventory();

			foreach (Field field in AdminWindow.SharedFieldInventory)
			{
				bool showCheckMark = oldInventory.Contains(field.FieldName);
				this._fieldsListBox.Items.Add(field, showCheckMark);
			}
		}

		private static FieldInventory MakeMasterInventory()
		{
			FieldInventory masterInventory = new FieldInventory();
			masterInventory.Add(MakeField(Field.FieldNames.EntryLexicalForm.ToString(), "Word"));
			masterInventory.Add(MakeField(Field.FieldNames.SenseGloss.ToString(), "Gloss"));
			masterInventory.Add(MakeField(Field.FieldNames.ExampleSentence.ToString(), "Example Sentence"));
			masterInventory.Add(MakeField(Field.FieldNames.ExampleTranslation.ToString(), "Translation"));
			return masterInventory;
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

		private static Field MakeField(string name, string displayName)
		{
			Field field = new Field();
			field.FieldName = name;
			field.DisplayName = displayName;
			field.Visibility = Field.VisibilitySetting.Invisible;
			return field;
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
				((Field)_fieldsListBox.SelectedItem).Visibility = Field.VisibilitySetting.Invisible;
			}
		}
	}
}
