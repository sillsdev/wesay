using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using SIL.WritingSystems;
using WeSay.LexicalModel.Foundation;
using Enumerable=SIL.Linq.Enumerable;

namespace WeSay.ConfigTool.Tasks
{
	public partial class WritingSystemFilterControl : UserControl
	{
		public WritingSystemFilterControl()
		{
			InitializeComponent();
		}

		private IList<string> _selectedItemIds;
		private string _labelWhenEmpty;

		public void Init(IEnumerable<WritingSystemDefinition> writingSystems, IList<string> selectedItemIds, string labelWhenEmpty)
		{
			_selectedItemIds = selectedItemIds;
			_writingSystemList.DropDownItems.Clear();
			_labelWhenEmpty = labelWhenEmpty;

			foreach (var writingSystem in writingSystems)
			{
				ToolStripMenuItem item = new ToolStripMenuItem(writingSystem.Id);
				item.CheckOnClick = true;
				item.Checked = selectedItemIds.Contains(writingSystem.Id);
				item.Tag = writingSystem;
				_writingSystemList.DropDownItems.Add(item);
				item.CheckedChanged += new EventHandler(OnItem_CheckedChanged);
			}
			SetMenuLabel();
		}

		void OnItem_CheckedChanged(object sender, EventArgs e)
		{
			SetMenuLabel();
			_selectedItemIds.Clear();
			Enumerable.ForEach(SelectedItemIds, id =>_selectedItemIds.Add(id));
		}

		public IEnumerable<string> SelectedItemIds
		{
			get
			{
				foreach (ToolStripMenuItem item in _writingSystemList.DropDownItems)
				{
					if(item.Checked)
						yield return ((WritingSystemDefinition)item.Tag).Id;
				}
			}
		}

		private void SetMenuLabel()
		{
			if (SelectedItemIds.Count() > 0)
			{
				_writingSystemList.Text = SelectedItemIds.Aggregate((a, b) => a + ", " + b);
			}
			else
			{
				_writingSystemList.Text = _labelWhenEmpty;
			}
		}
	}
}
