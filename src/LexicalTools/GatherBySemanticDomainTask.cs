using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public class GatherBySemanticDomainTask : TaskBase
	{
		private readonly string _wordListFileName;
		private GatherBySemanticDomainsControl  _gatherControl;
		private List<string> _domains;
		private List<string> _questions;

		public GatherBySemanticDomainTask(IRecordListManager recordListManager, string label, string description, string wordListFileName)
			: base(label, description, false, recordListManager)
		{
			_wordListFileName = wordListFileName;
			_domains = null;
			_questions = null;
		}

		private void LoadList(string listName, List<string> list)
		{
			list.Clear();
			string path = Path.Combine(WeSayWordsProject.Project.PathToWeSaySpecificFilesDirectoryInProject, listName);
			if (!File.Exists(path))
			{
				path = Path.Combine(WeSayWordsProject.Project.ApplicationCommonDirectory, listName);
			}
			using (TextReader r = File.OpenText(path))
			{
				do
				{
					string s = r.ReadLine();
					if (s == null)
					{
						break;
					}
					list.Add(s);
				} while (true);
			}
		}

		/// <summary>
		/// The GatherWordListControl associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public override Control Control
		{
			get
			{
				return _gatherControl;
			}
		}

		public override void Activate()
		{
			if (_domains == null)
			{
				_domains = new List<string>();
				LoadList(_wordListFileName, _domains);
				_questions = new List<string>();
				LoadList("Q" + _wordListFileName, _questions);
			}
			base.Activate();
			_gatherControl = new GatherBySemanticDomainsControl();
			_gatherControl.Domains = _domains;
			_gatherControl.Questions = _questions;
		   // _gatherControl.Records = RecordListManager.GetListOfType<LexEntry>();

		  //  _gatherControl.WordAdded += new EventHandler(_gatherControl_WordAdded);
		}

		void _gatherControl_WordAdded(object sender, EventArgs e)
		{
			this.RecordListManager.GoodTimeToCommit();
		}

		public override void Deactivate()
		{
			base.Deactivate();
		   // _gatherControl.WordAdded -= new EventHandler(_gatherControl_WordAdded);
			_gatherControl.Dispose();
			_gatherControl = null;
			this.RecordListManager.GoodTimeToCommit();
		}
	}
}
