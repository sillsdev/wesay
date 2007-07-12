using System;
using System.Windows.Forms;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public class DictionaryTask : TaskBase
	{
		private DictionaryControl _dictionaryControl;
		private readonly ViewTemplate _viewTemplate;

		public DictionaryTask(IRecordListManager recordListManager,
							ViewTemplate viewTemplate)
			: base("Dictionary", string.Empty, true, recordListManager)
		{
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
			_viewTemplate = viewTemplate;
		}

		public override void Activate()
		{
			try
			{
				base.Activate();
				_dictionaryControl = new DictionaryControl(RecordListManager, ViewTemplate);
				_dictionaryControl.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
			}
			catch (Reporting.ConfigurationException)
			{
				IsActive = false;
				throw;
			}
		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			RecordListManager.GoodTimeToCommit();
		}

		public override void Deactivate()
		{
			base.Deactivate();
			_dictionaryControl.SelectedIndexChanged -= new EventHandler(OnRecordSelectionChanged);
			_dictionaryControl.Dispose();
			_dictionaryControl = null;
			RecordListManager.GoodTimeToCommit();
		}

		/// <summary>
		/// The entry detail control associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public override Control Control
		{
			get
			{
				return _dictionaryControl;
			}
		}

		public override string Status
		{
			get
			{
				return DataSource.Count.ToString();
			}
		}

		public override string Description
		{
			get
			{
				return String.Format(StringCatalog.Get("~See all {0} {1} words.", "The description of the 'Dictionary' task.  In place of the {0} will be the number of words in the dictionary.  In place of the {1} will be the name of the project."), DataSource.Count, BasilProject.Project.Name);
			}
		}


		/// <summary>
		/// Gives a sense of the overall size of the task versus what's left to do
		/// </summary>
		public override int ReferenceCount
		{
			get
			{
				return -1; //not relevant
			}
		}

		public IRecordList<LexEntry> DataSource
		{
			get
			{
				IRecordList<LexEntry> data = RecordListManager.GetListOfType<LexEntry>();
				return data;
			}
		}

		public ViewTemplate ViewTemplate
		{
			get { return this._viewTemplate; }
		}
	}
}