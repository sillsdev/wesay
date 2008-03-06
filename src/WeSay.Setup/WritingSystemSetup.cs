using System;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Reporting;
using WeSay.Language;
using WeSay.Project;

namespace WeSay.Setup
{
	public partial class WritingSystemSetup : ConfigurationControlBase
	{
		public WritingSystemSetup():base("set up fonts, keyboards, and sorting")
		{
			InitializeComponent();
			Resize += new EventHandler(WritingSystemSetup_Resize);
		}

		private void WritingSystemSetup_Resize(object sender, EventArgs e)
		{
			//this is part of dealing with .net not adjusting stuff well for different dpis
			splitContainer1.Dock = DockStyle.None;
			splitContainer1.Width = Width - 25;
		}


		public void WritingSystemSetup_Load(object sender, EventArgs e)
		{
			if (DesignMode)
			{
				return;
			}

			LoadWritingSystemListBox();
			//for checking that ids are unique
			_basicControl.WritingSystemCollection = BasilProject.Project.WritingSystems;
		}

		private void LoadWritingSystemListBox()
		{
			_wsListBox.Items.Clear();
			foreach (WritingSystem w in BasilProject.Project.WritingSystems.Values)
			{
				_wsListBox.Items.Add(new WsDisplayProxy(w, BasilProject.Project.WritingSystems));
			}
			_wsListBox.Sorted = true;
			if (_wsListBox.Items.Count > 0)
			{
				_wsListBox.SelectedIndex = 0;
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
			_tabControl.Visible = SelectedWritingSystem != null;
			if (SelectedWritingSystem == null)
			{
				Refresh();
				return;
			}

			_btnRemove.Enabled = true;
//                (SelectedWritingSystem != BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault)
//              && (SelectedWritingSystem != BasilProject.Project.WritingSystems.VernacularWritingSystemDefault);
			_basicControl.WritingSystem = SelectedWritingSystem;
			_sortControl.WritingSystem = SelectedWritingSystem;
			_fontControl.WritingSystem = SelectedWritingSystem;
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
			if (SelectedWritingSystem != null &&
				BasilProject.Project.WritingSystems.ContainsKey(SelectedWritingSystem.Id))
			{
				BasilProject.Project.WritingSystems.Remove(SelectedWritingSystem.Id);
				LoadWritingSystemListBox();
				UpdateSelection();
			}
		}

		private void _btnAddWritingSystem_Click(object sender, EventArgs e)
		{
			WritingSystem w = null;
			string[] keys = {"xx", "x1", "x2", "x3"};
			foreach (string s in keys)
			{
				if (!BasilProject.Project.WritingSystems.ContainsKey(s))
				{
					w = new WritingSystem(s, new Font("Doulos SIL", 12));
					break;
				}
			}
			if (w == null)
			{
				ErrorReport.ReportNonFatalMessage("Could not produce a unique ID.");
			}
			else
			{
				BasilProject.Project.WritingSystems.Add(w.Id, w);
				WsDisplayProxy item = new WsDisplayProxy(w, BasilProject.Project.WritingSystems);
				_wsListBox.Items.Add(item);
				_wsListBox.SelectedItem = item;
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
			if (args != null && args.ChangedItem.PropertyDescriptor.Name == "Id")
			{
				string oldId = args.OldValue.ToString();
				WeSayWordsProject.Project.MakeWritingSystemIdChange(ws, oldId);
//                Reporting.ErrorReporter.ReportNonFatalMessage(
//                    "Currently, WeSay does not make a corresponding change to the id of this writing system in your LIFT xml file.  Please do that yourself, using something like NotePad to search for lang=\"{0}\" and change to lang=\"{1}\"",
//                    ws.Id, oldId);
			}

			//_wsListBox.Refresh(); didn't work
			//this.Refresh();   didn't work
			for (int i = 0; i < _wsListBox.Items.Count; i++)
			{
				_wsListBox.Items[i] = _wsListBox.Items[i];
			}
			UpdateSelection();
			if (args != null && args.ChangedItem.PropertyDescriptor.Name == "Id")
			{
				LoadWritingSystemListBox();
				foreach (WsDisplayProxy o in _wsListBox.Items)
				{
					if (o.WritingSystem == ws)
					{
						_wsListBox.SelectedItem = o;
						break;
					}
				}
			}
		}

		//private void MakeWritingSystemIdChange(string oldId, string newId)
		//{
		//    foreach (Field field in WeSayWordsProject.Project.DefaultViewTemplate)
		//    {
		//        field.ChangeWritingSystemId(oldId, newId);
		//    }
		//}
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

//        public bool IsAnalysisDefault
//        {
//            get
//            {
//                return _collection.AnalysisWritingSystemDefault == _writingSystem;
//            }
//            set
//            {
//                if (value)
//                {
//                    _collection.AnalysisWritingSystemDefaultId = _writingSystem.Id;
//                }
//                else
//                {
//                    Debug.Fail("Can't really handle setting to false.");
//                }
//            }
//        }
//        public bool IsVernacularDefault
//        {
//            get
//            {
//                return _collection.VernacularWritingSystemDefault == _writingSystem;
//            }
//            set
//            {
//                if (value)
//                {
//                    _collection.VernacularWritingSystemDefaultId = _writingSystem.Id;
//                }
//                else
//                {
//                    Debug.Fail("Can't really handle setting to false.");
//                }
//            }
//        }

		public WritingSystem WritingSystem
		{
			get { return _writingSystem; }
			set { _writingSystem = value; }
		}

		public override string ToString()
		{
			string s = _writingSystem.ToString();
//            if (IsVernacularDefault )
//            {
//                s += " (V)";
//            }
//             if (IsAnalysisDefault)
//            {
//                s += " (A)";
//            }


			switch (s)
			{
				default:
					if (s == WritingSystem.IdForUnknownVernacular)
						s += " (Change to your Vernacular)";
					break;
				case "fr":
					s += " (French)";
					break;
				case "id":
					s += " (Indonesian)";
					break;
				case "tpi":
					s += " (Tok Pisin)";
					break;
				case "th":
					s += " (Thai)";
					break;
				case "es":
					s += " (Spanish)";
					break;
				case "en":
					s += " (English)";
					break;
				case "my":
					s += " (Burmese)";
					break;
			}

			return s;
		}
	}
}