using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using WeSay.Foundation;
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
			RefreshMoveButtons();
		}

		private void RefreshMoveButtons() {
			this.btnMoveUp.Enabled = this._writingSystemListBox.SelectedIndex > 0;
			this.btnMoveDown.Enabled = this._writingSystemListBox.SelectedIndex < this._writingSystemListBox.Items.Count - 2;
		}

		private void FieldsControl_Load(object sender, EventArgs e)
		{
			if (DesignMode)
				return;

			LoadInventory();
			//nb: may important to do this after loading the inventory
			this._fieldsListBox.ItemCheck += new ItemCheckEventHandler(this.OnFieldsListBox_ItemCheck);
			if (_fieldsListBox.Items.Count > 0)
			{
				_fieldsListBox.SelectedIndex = 0;
			}

		}
		protected override void OnVisibleChanged(EventArgs e)
		{
			if (Visible)
			{
				LoadWritingSystemBox(); // refresh these since they might have changed on another tab
			}
			base.OnVisibleChanged(e);
		}

		/// <summary>
		/// Construct the list of fields to show.
		/// </summary>
		private void LoadInventory()
		{
			_fieldsListBox.Items.Clear();

			foreach (Field field in  WeSayWordsProject.Project.ViewTemplate)
			{
				this._fieldsListBox.Items.Add(field, field.Visibility == CommonEnumerations.VisibilitySetting.Visible);
			}
		}


		private void OnFieldsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (e.NewValue== CheckState.Checked)
			{
				((Field)_fieldsListBox.SelectedItem).Visibility = CommonEnumerations.VisibilitySetting.Visible;
			}
			else
			{
				CurrentField.Visibility = CommonEnumerations.VisibilitySetting.Invisible;
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
			foreach (WritingSystem ws in CurrentField.WritingSystems)
			{
				int i = _writingSystemListBox.Items.Add(ws);
				_writingSystemListBox.SetItemChecked(i, true);

			}
			foreach (WritingSystem ws in BasilProject.Project.WritingSystems.Values)
			{
				if(!CurrentField.WritingSystemIds.Contains(ws.Id))
				{
					int i= _writingSystemListBox.Items.Add(ws);
					_writingSystemListBox.SetItemChecked(i, false);
				}
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
				SaveWritingSystemIdsForField(e.Index);
				//CurrentField.WritingSystemIds.Add(CurrentWritingSystemId);
			}
			else
			{
				CurrentField.WritingSystemIds.Remove(CurrentWritingSystemId);
			}
		}

		private void SaveWritingSystemIdsForField()
		{
			SaveWritingSystemIdsForField(-1);
		}


		private void SaveWritingSystemIdsForField(int aboutToBeCheckedItemIndex) {
			CurrentField.WritingSystemIds.Clear();
			for (int i = 0; i < this._writingSystemListBox.Items.Count; i++)
			{
				if(this._writingSystemListBox.GetItemChecked(i) ||
						i == aboutToBeCheckedItemIndex)
				{
					WritingSystem ws = (WritingSystem) this._writingSystemListBox.Items[i];
					CurrentField.WritingSystemIds.Add(ws.Id);
				}
			}
		}

		private string CurrentWritingSystemId
		{
			get
			{
				return _writingSystemListBox.SelectedItem.ToString();
			}
		}

		void _writingSystemListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			RefreshMoveButtons();
		}

		void OnBtnMoveUpClick(object sender, System.EventArgs e)
		{
			object item = _writingSystemListBox.SelectedItem;
			int index = _writingSystemListBox.SelectedIndex;
			if (item == null || index < 1)
			{
				return;
			}
			// remove and put back
			bool isChecked = _writingSystemListBox.GetItemChecked(index);
			_writingSystemListBox.Items.RemoveAt(index);
			--index;
			_writingSystemListBox.Items.Insert(index, item);
			_writingSystemListBox.SetItemChecked(index, isChecked);
			_writingSystemListBox.SelectedIndex = index;
			if (isChecked)
			{
				SaveWritingSystemIdsForField();
			}
			RefreshMoveButtons();
		}

		void OnBtnMoveDownClick(object sender, System.EventArgs e)
		{
			object item = _writingSystemListBox.SelectedItem;
			int index = _writingSystemListBox.SelectedIndex;
			if (item == null || index > _writingSystemListBox.Items.Count - 2)
			{
				return;
			}
			// remove and put back
			bool isChecked = _writingSystemListBox.GetItemChecked(index);
			_writingSystemListBox.Items.RemoveAt(index);
			++index;
			_writingSystemListBox.Items.Insert(index, item);
			_writingSystemListBox.SetItemChecked(index, isChecked);
			_writingSystemListBox.SelectedIndex = index;
			if (isChecked)
			{
				SaveWritingSystemIdsForField();
			}
			RefreshMoveButtons();
		}
	}
}
