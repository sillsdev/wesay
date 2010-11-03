using System;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Code;
using Palaso.i18n;
using Palaso.Code;
using Palaso.Reporting;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;

namespace WeSay.ConfigTool
{
	public partial class WritingSystemSetup: ConfigurationControlBase
	{

		public WritingSystemSetup(ILogger logger)
			: base("set up fonts, keyboards, and sorting", logger)
		{
			InitializeComponent();
			Resize += WritingSystemSetup_Resize;
			_basicControl.Logger = logger;
			_fontControl.Logger = logger;
			_sortControl.Logger = logger;
			_sortingPage.Enter += OnSortingPageEntered;
		}

		private void OnSortingPageEntered(object sender, EventArgs e)
		{
			_sortControl.UpdateFontInChildControlsIfNecassary();
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
			_wsListBox.BeginUpdate();
			_wsListBox.Items.Clear();
			_wsListBox.Sorted = false; // Required for Mono which keeps on sorting during the Add which slows things down.
			foreach (WritingSystem w in BasilProject.Project.WritingSystems.Values)
			{
				_wsListBox.Items.Add(new WsDisplayProxy(w));
			}
			_wsListBox.Sorted = true;
			_wsListBox.EndUpdate();
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
			WritingSystem selectedWritingSystem = SelectedWritingSystem;
			_tabControl.Visible = selectedWritingSystem != null;
			_btnRemove.Enabled = EnableRemove;
			if (selectedWritingSystem == null)
			{
				Console.WriteLine("WritingSystemSetup.UpdateSelection selected is null");
				Invalidate();
				//Refresh();
				return;
			}

			//                (SelectedWritingSystem != BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault)
			//              && (SelectedWritingSystem != BasilProject.Project.WritingSystems.VernacularWritingSystemDefault);
			_basicControl.WritingSystem = selectedWritingSystem;
			_sortControl.WritingSystem = selectedWritingSystem;
			_fontControl.WritingSystem = selectedWritingSystem;

			_sortingPage.Enabled = !selectedWritingSystem.IsAudio;
			_fontsPage.Enabled = !selectedWritingSystem.IsAudio;
		}

		private bool EnableRemove
		{
			get
			{
				return SelectedWritingSystem != null
					   && SelectedWritingSystem.Id != WritingSystem.IdForUnknownAnalysis //once they delete english, lots of things are going to break
					   && _wsListBox.Items.Count > 1; //don't let them delete the last one
		   }
		}

		private WritingSystem SelectedWritingSystem
		{
			get
			{
				var proxy = _wsListBox.SelectedItem as WsDisplayProxy;
				if (proxy != null)
				{
					return proxy.WritingSystem;
				}
				return null;
			}
		}

		private void _btnRemove_Click(object sender, EventArgs e)
		{
			if (SelectedWritingSystem != null &&
				BasilProject.Project.WritingSystems.ContainsKey(SelectedWritingSystem.Id))
			{
				var doomedId = SelectedWritingSystem.Id;
				BasilProject.Project.WritingSystems.Remove(SelectedWritingSystem.Id);
				LoadWritingSystemListBox();
				UpdateSelection();
				_logger.WriteConciseHistoricalEvent(StringCatalog.Get("Removed writing system '{0}'", "Checkin Description in WeSay Config Tool used when you remove a writing system."), doomedId);

			}
		}

		private void _btnAddWritingSystem_Click(object sender, EventArgs e)
		{
			if(IsDefaultVernacularStillUnidentified)
			{
				MessageBox.Show(
					"Before creating new writing systems, please change the ID of the 'v' writing system to match the Ethnologue/ISO 693 code of this language.");
				return;
			}
			WritingSystem w = null;
			string[] keys = {"xx", "x1", "x2", "x3"};
			foreach (string s in keys)
			{
				if (!BasilProject.Project.WritingSystems.ContainsKey(s))
				{
					Font font;
					try
					{
						font = new Font("Doulos SIL", 12);
					}
					catch(Exception )
					{
					   font = new Font(SystemFonts.DefaultFont.SystemFontName, 12);
					}

					w = new WritingSystem(s, font);
					break;
				}
			}
			if (w == null)
			{
				ErrorReport.NotifyUserOfProblem("Could not produce a unique ID.");
			}
			else
			{
				BasilProject.Project.WritingSystems.Add(w.Id, w);
				var item = new WsDisplayProxy(w);
				_wsListBox.Items.Add(item);
				_wsListBox.SelectedItem = item;
			}

			_logger.WriteConciseHistoricalEvent(StringCatalog.Get("Added writing system", "Checkin Description in WeSay Config Tool used when you add a writing system."));

		}

		private bool IsDefaultVernacularStillUnidentified
		{
			get
			{
				foreach (WritingSystem w in BasilProject.Project.WritingSystems.Values)
				{
					if(w.Id == WritingSystem.IdForUnknownVernacular)
						return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Called when, for example, the user changes the id of the selected ws
		/// </summary>
		private void OnWritingSystemIdChanged(WritingSystem writingSystem, WritingSystem oldWritingSystemValues)
		{
			using (Detect.Reentry(this, "OnWritingSystemIdChanged").AndThrow())
			{
				if (writingSystem.Id != oldWritingSystemValues.Id)
				{
					Console.WriteLine("WritingSystemSetup.OnWritingSystemIdChanged changing to {0}", writingSystem.Id);
					if (!WeSayWordsProject.Project.MakeWritingSystemIdChange(writingSystem, oldWritingSystemValues.Id))
					{
					   throw new ApplicationException(String.Format("Could not change Writingsystem Id {0} to {1} in Lift file.", oldWritingSystemValues.Id, writingSystem.Id));
					}
					//                Reporting.ErrorReporter.NotifyUserOfProblem(
					//                    "Currently, WeSay does not make a corresponding change to the id of this writing system in your LIFT xml file.  Please do that yourself, using something like NotePad to search for lang=\"{0}\" and change to lang=\"{1}\"",
					//                    ws.Id, oldId);
				}

				// Update the list box
				var p = _wsListBox.SelectedItem as WsDisplayProxy;
				_wsListBox.BeginUpdate();
				_wsListBox.Sorted = false; // Force a re-sort, there doesn't seem to be a method available to do this.
				_wsListBox.Sorted = true;
				_wsListBox.SelectedItem = p;
				_wsListBox.EndUpdate();
				_wsListBox.Invalidate();
			}
		}

		string AppendOrNot(string idComponent)
		{
			string stringToAppend = "";
			if(!String.IsNullOrEmpty(idComponent))
			{
				stringToAppend = '-' + idComponent;
			}
			return stringToAppend;
		}

		private void OnIsAudioChanged(object sender, EventArgs e)
		{
			UpdateSelection();
		}
	}

	/// <summary>
	/// An item to stick in the listview which represents a ws
	/// </summary>
	public class WsDisplayProxy : Object
	{
		private WritingSystem _writingSystem;

		public WsDisplayProxy(WritingSystem ws)
		{
			_writingSystem = ws;
		}

		public WritingSystem WritingSystem
		{
			get { return _writingSystem; }
			set { _writingSystem = value; }
		}

		public override string ToString()
		{
			string s = _writingSystem.ToString();

			switch (s)
			{
				default:
					if (s == WritingSystem.IdForUnknownVernacular)
					{
						s += " (Change to your Vernacular)";
					}
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