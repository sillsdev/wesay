using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Language;
using WeSay.UI;

namespace WeSay.Admin
{
	public partial class WritingSystemSetup : UserControl
	{
		public WritingSystemSetup()
		{
			InitializeComponent();
		}

		private void WritingSystemSetup_Load(object sender, EventArgs e)
		{
			LoadWritingSystemListBox();
		}

		private void LoadWritingSystemListBox()
		{
			_wsListBox.Items.Clear();
			foreach(WeSay.Language.WritingSystem w in BasilProject.Project.WritingSystems.Values)
			{
				this._wsListBox.Items.Add(w);
			}
			if (this._wsListBox.Items.Count > 0)
			{
				this._wsListBox.SelectedIndex = 0;
			}
		}

		private void _wsListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateSelection();
		}

		/// <summary>
		/// nb: seperate from the event handler because the handler isn't called if the last item is deleted
		/// </summary>
		private void UpdateSelection()
		{
			this._tabControl.Visible = this.SelectedWritingSystem != null;
			if (this.SelectedWritingSystem == null)
			{
				this.Refresh();
				return;
			}
			_basicControl.WritingSystem = this.SelectedWritingSystem;
			_fontControl.WritingSystem = this.SelectedWritingSystem;
		}

		private WritingSystem SelectedWritingSystem
		{
			get
			{
				return _wsListBox.SelectedItem as WritingSystem;
			}
		}

		private void _btnRemove_Click(object sender, EventArgs e)
		{
			if(SelectedWritingSystem!=null && BasilProject.Project.WritingSystems.ContainsKey(SelectedWritingSystem.Id))
			{
				BasilProject.Project.WritingSystems.Remove(SelectedWritingSystem.Id);
				LoadWritingSystemListBox();
				UpdateSelection();
			}
		}

		private void _btnAddWritingSystem_Click(object sender, EventArgs e)
		{
			WritingSystem w =null;
			string[] keys = { "xx", "x1", "x2", "x3" };
			foreach (string s in keys)
			{
				if (!BasilProject.Project.WritingSystems.ContainsKey(s))
				{
					w= new WritingSystem(s, new Font("Doulos SIL", 12));
					break;
				}
			}
			if (w == null)
			{
				MessageBox.Show("Could not produce a unique ID.");
			}
			else
			{
				BasilProject.Project.WritingSystems.Add(w.Id,w);
				this._wsListBox.Items.Add(w);
				_wsListBox.SelectedItem = w;
			}
		}
	}
}
