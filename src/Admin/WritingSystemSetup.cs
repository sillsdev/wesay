using System;
using System.Diagnostics;
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
			if (DesignMode)
				return;

			LoadWritingSystemListBox();
			//for checking that ids are unique
			_basicControl.WritingSystemCollection = BasilProject.Project.WritingSystems;
		}

		private void LoadWritingSystemListBox()
		{
			_wsListBox.Items.Clear();
			foreach(WeSay.Language.WritingSystem w in BasilProject.Project.WritingSystems.Values)
			{
				this._wsListBox.Items.Add(new WsDisplayProxy(w, BasilProject.Project.WritingSystems));
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

			_btnRemove.Enabled =
				(SelectedWritingSystem != BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault)
			  && (SelectedWritingSystem != BasilProject.Project.WritingSystems.VernacularWritingSystemDefault);

			_basicControl.WritingSystem = this.SelectedWritingSystem;
			_fontControl.WritingSystem = this.SelectedWritingSystem;
		}

		private WritingSystem SelectedWritingSystem
		{
			get
			{
				WsDisplayProxy proxy = _wsListBox.SelectedItem as WsDisplayProxy;
				if (proxy != null)
				{
					return proxy.WritingSystem;
				}
				else
				{
					return null;
				}
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
				this._wsListBox.Items.Add(new WsDisplayProxy(w, BasilProject.Project.WritingSystems));
				_wsListBox.SelectedItem = w;
			}
		}

		/// <summary>
		/// Called when, for example, the user changes the id of the selected ws
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _basicControl_DisplayPropertiesChanged(object sender, EventArgs e)
		{
			WritingSystem ws = sender as WritingSystem;
			PropertyValueChangedEventArgs args = e as PropertyValueChangedEventArgs;
			if (args!=null && args.ChangedItem.PropertyDescriptor.Name == "Id")
			{
				 BasilProject.Project.WritingSystems.IdOfWritingSystemChanged(ws, args.OldValue.ToString() );
			}

			//_wsListBox.Refresh(); didn't work
			//this.Refresh();   didn't work
			for (int i = 0; i < _wsListBox.Items.Count;i++ )
			{
				_wsListBox.Items[i] = _wsListBox.Items[i];
			}

			UpdateSelection();
		}


	}

	/// <summary>
	/// An item to stick in the listview which represents a ws
	/// </summary>
	public class WsDisplayProxy
	{
		private WritingSystem _writingSystem;
		private WritingSystemCollection _collection;

		public WsDisplayProxy(WritingSystem ws, WritingSystemCollection collection)
		{
			_writingSystem = ws;
			_collection = collection;
		}

		public bool IsAnalysisDefault
		{
			get
			{
				return _collection.AnalysisWritingSystemDefault == _writingSystem;
			}
			set
			{
				if (value)
				{
					_collection.AnalysisWritingSystemDefaultId = _writingSystem.Id;
				}
				else
				{
					Debug.Fail("Can't really handle setting to false.");
				}
			}
		}
		public bool IsVernacularDefault
		{
			get
			{
				return _collection.VernacularWritingSystemDefault == _writingSystem;
			}
			set
			{
				if (value)
				{
					_collection.VernacularWritingSystemDefaultId = _writingSystem.Id;
				}
				else
				{
					Debug.Fail("Can't really handle setting to false.");
				}
			}
		}

		public WritingSystem WritingSystem
		{
			get { return this._writingSystem; }
			set { this._writingSystem = value; }
		}

		public override string ToString()
		{
			string s = this._writingSystem.ToString();
			if (IsVernacularDefault )
			{
				s += " (V)";
			}
			 if (IsAnalysisDefault)
			{
				s += " (A)";
			}
			return s;
		}
	}
}
