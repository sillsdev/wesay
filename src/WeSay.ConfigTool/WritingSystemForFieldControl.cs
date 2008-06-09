using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Enchant;
using WeSay.Language;
using WeSay.Project;

namespace WeSay.ConfigTool
{
	public partial class WritingSystemForFieldControl : UserControl
	{
		private Field _field;
		private class WritingSystemListBoxAdaptor
		{
			private readonly WritingSystem _ws;
			private readonly bool _hasSpellCheckerInstalled;
			public WritingSystemListBoxAdaptor(WritingSystem ws, bool hasSpellCheckerInstalled)
			{
				this._ws = ws;
				this._hasSpellCheckerInstalled = hasSpellCheckerInstalled;
			}

			public WritingSystem WritingSystem
			{
				get { return _ws; }
			}

			public bool HasSpellCheckerInstalled
			{
				get { return _hasSpellCheckerInstalled; }
			}

			public override string ToString()
			{
				string displayString = WritingSystem.ToString();
				if(HasSpellCheckerInstalled)
				{
					displayString += " (Has spell checker)";
				}
				return displayString;
			}
		}
		public WritingSystemForFieldControl()
		{
			InitializeComponent();
		}

		private static List<string> GetWritingSystemIdsWithSpellCheckingInstalled()
		{
			List<string> writingSystemIdsWithSpellCheckingInstalled = new List<string>();
			try
			{
				using (Broker broker = new Broker())
				{
					foreach (WritingSystem ws in BasilProject.Project.WritingSystems.Values)
					{
						if (broker.DictionaryExists(ws.Id))
						{
							writingSystemIdsWithSpellCheckingInstalled.Add(ws.Id);
						}
					}
				}
			}
			catch (DllNotFoundException)
			{
				//If Enchant is not installed we expect an exception.
			}
			return writingSystemIdsWithSpellCheckingInstalled;
		}

		public Field CurrentField
		{
			set
			{
				_field = value;
				if (Visible)
				{
					LoadWritingSystemBox(); // refresh these since they might have changed on another tab
				}
			}
			get { return _field; }
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


		private void SaveWritingSystemIdsForField(int aboutToBeCheckedItemIndex)
		{
			CurrentField.WritingSystemIds.Clear();
			for (int i = 0; i < this._writingSystemListBox.Items.Count; i++)
			{
				if (this._writingSystemListBox.GetItemChecked(i) ||
						i == aboutToBeCheckedItemIndex)
				{
					WritingSystem ws = ((WritingSystemListBoxAdaptor)this._writingSystemListBox.Items[i]).WritingSystem;
					CurrentField.WritingSystemIds.Add(ws.Id);
				}
			}
		}

		private string CurrentWritingSystemId
		{
			get
			{
				return ((WritingSystemListBoxAdaptor)_writingSystemListBox.SelectedItem).WritingSystem.ToString();
			}
		}

		void _writingSystemListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			RefreshMoveButtons();
		}

		void OnBtnMoveUpClick(object sender, EventArgs e)
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

		void OnBtnMoveDownClick(object sender, EventArgs e)
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

		private void RefreshMoveButtons()
		{
			this.btnMoveUp.Enabled = this._writingSystemListBox.SelectedIndex > 0;
			this.btnMoveDown.Enabled = this._writingSystemListBox.SelectedIndex < this._writingSystemListBox.Items.Count - 1;
		}

		private void WritingSystemForFieldControl_Load(object sender, EventArgs e)
		{
			RefreshMoveButtons();

		}

		private void LoadWritingSystemBox()
		{
			if (CurrentField == null)
			{
				return;
			}
			List<string> writingSystemIdsWithSpellCheckingInstalled = GetWritingSystemIdsWithSpellCheckingInstalled();

			_writingSystemListBox.Items.Clear();
			foreach (WritingSystem ws in CurrentField.WritingSystems)
			{
				bool hasSpellCheckerInstalled = writingSystemIdsWithSpellCheckingInstalled.Contains(ws.Id);
				int i = _writingSystemListBox.Items.Add(new WritingSystemListBoxAdaptor(ws, hasSpellCheckerInstalled));
				_writingSystemListBox.SetItemChecked(i, true);
			}
			foreach (WritingSystem ws in BasilProject.Project.WritingSystems.Values)
			{
				if (!CurrentField.WritingSystemIds.Contains(ws.Id))
				{
					bool hasSpellCheckerInstalled = writingSystemIdsWithSpellCheckingInstalled.Contains(ws.Id);
					int i = _writingSystemListBox.Items.Add(new WritingSystemListBoxAdaptor(ws, hasSpellCheckerInstalled));
					_writingSystemListBox.SetItemChecked(i, false);
				}
			}

		}

		private void WritingSystemForFieldControl_VisibleChanged(object sender, EventArgs e)
		{
			LoadWritingSystemBox();// choices might have changed
		}
	}
}
