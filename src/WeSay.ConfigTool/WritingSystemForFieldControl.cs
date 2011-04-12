using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Enchant;
using Palaso.Reporting;
using Palaso.WritingSystems;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;

namespace WeSay.ConfigTool
{
	public partial class WritingSystemForFieldControl: UserControl
	{
		private Field _field;

		private class WritingSystemListBoxAdaptor
		{
			private readonly WritingSystemDefinition _ws;
			private readonly bool _hasSpellCheckerInstalled;

			public WritingSystemListBoxAdaptor(WritingSystemDefinition ws, bool hasSpellCheckerInstalled)
			{
				_ws = ws;
				_hasSpellCheckerInstalled = hasSpellCheckerInstalled;
			}

			public WritingSystemDefinition WritingSystem
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
				if (HasSpellCheckerInstalled)
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
					foreach (WritingSystemDefinition ws in BasilProject.Project.WritingSystems.AllWritingSystems)
					{
						try
						{
							if (broker.DictionaryExists(ws.SpellCheckingId))
							{
								writingSystemIdsWithSpellCheckingInstalled.Add(ws.Id);
							}
						}
						catch (Exception)  //WS-1296 where (sometimes) a bogus looking id killed Enchant
						{
							//ErrorReport.NotifyUserOfProblem(new ShowOncePerSessionBasedOnExactMessagePolicy(), "There was a problem asking the Enchant Spelling system about '{0}'.", ws.Id);

							//These enchant errors are just too common & useless.  Let's not even report on them.
						}
					}
				}
			}
			catch (DllNotFoundException)
			{
				//If Enchant is not installed we expect an exception.
			}
			catch (Exception e)//there are other errors we can get from the enchange binding
			{
				ErrorReport.NotifyUserOfProblem(new ShowOncePerSessionBasedOnExactMessagePolicy(),
												"The Enchant Spelling engine encountered an error: " + e.Message);
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
					LoadWritingSystemBox();
					// refresh these since they might have changed on another tab
				}
			}
			get { return _field; }
		}

		private void _writingSystemListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			//happens when we are setting initial checkbox states from code
			if (_writingSystemListBox.SelectedItem == null)
			{
				return;
			}

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
			for (int i = 0;i < _writingSystemListBox.Items.Count;i++)
			{
				if (_writingSystemListBox.GetItemChecked(i) || i == aboutToBeCheckedItemIndex)
				{
					WritingSystemDefinition ws =
							((WritingSystemListBoxAdaptor) _writingSystemListBox.Items[i]).
									WritingSystem;
					CurrentField.WritingSystemIds.Add(ws.Id);
				}
			}
		}

		private string CurrentWritingSystemId
		{
			get
			{
				return
						((WritingSystemListBoxAdaptor) _writingSystemListBox.SelectedItem).
								WritingSystem.ToString();
			}
		}

		private void _writingSystemListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			RefreshMoveButtons();
		}

		private void OnBtnMoveUpClick(object sender, EventArgs e)
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

		private void OnBtnMoveDownClick(object sender, EventArgs e)
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
			btnMoveUp.Enabled = _writingSystemListBox.SelectedIndex > 0;
			btnMoveDown.Enabled = _writingSystemListBox.SelectedIndex <
								  _writingSystemListBox.Items.Count - 1;
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
			List<string> writingSystemIdsWithSpellCheckingInstalled =
					GetWritingSystemIdsWithSpellCheckingInstalled();

			_writingSystemListBox.Items.Clear();
			IList<WritingSystemDefinition> writingSystems =
					BasilProject.Project.WritingSystemsFromIds(CurrentField.WritingSystemIds);
			foreach (WritingSystemDefinition ws in writingSystems)
			{
				bool hasSpellCheckerInstalled =
						writingSystemIdsWithSpellCheckingInstalled.Contains(ws.Id);
				int i =
						_writingSystemListBox.Items.Add(new WritingSystemListBoxAdaptor(ws,
																						hasSpellCheckerInstalled));
				_writingSystemListBox.SetItemChecked(i, true);
			}
			foreach (WritingSystemDefinition ws in BasilProject.Project.WritingSystems.AllWritingSystems)
			{
				if (!CurrentField.WritingSystemIds.Contains(ws.Id))
				{
					bool hasSpellCheckerInstalled =
							writingSystemIdsWithSpellCheckingInstalled.Contains(ws.Id);
					int i =
							_writingSystemListBox.Items.Add(new WritingSystemListBoxAdaptor(ws,
																							hasSpellCheckerInstalled));
					_writingSystemListBox.SetItemChecked(i, false);
				}
			}
		}

		private void WritingSystemForFieldControl_VisibleChanged(object sender, EventArgs e)
		{
			LoadWritingSystemBox(); // choices might have changed
		}
	}
}