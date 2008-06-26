using System;
using System.Drawing;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Dashboard;
using WeSay.LexicalModel;
using WeSay.LexicalTools.Properties;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public class DictionaryTask : TaskBase
	{
		private DictionaryControl _dictionaryControl;
		private readonly ViewTemplate _viewTemplate;
		//TODO: Use short label when migrating to new dashboard
		private static readonly string kTaskLabel = "~Dictionary";
		private static readonly string kTaskLongLabel = "~Dictionary Browse && Edit";

		public DictionaryTask(IRecordListManager recordListManager,
							  ViewTemplate viewTemplate)
			: base(kTaskLongLabel, kTaskLongLabel, string.Empty, true, recordListManager)
		{
#if JustForCodeScanner
			StringCatalog.Get(kTaskLabel,
							  "The label for the task that lets you see all entries, search for entries, and edit various fields.  We don't like the English name very much, so feel free to call this something very different in the language you are translating to.");
			StringCatalog.Get(kTaskLongLabel,
							  "The long label for the task that lets you see all entries, search for entries, and edit various fields.  We don't like the English name very much, so feel free to call this something very different in the language you are translating to.");
#endif
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
				RegisterWithCache(_viewTemplate);
				base.Activate();
				_dictionaryControl = new DictionaryControl(RecordListManager, ViewTemplate);
				_dictionaryControl.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
			}
			catch (Palaso.Reporting.ConfigurationException)
			{
				IsActive = false;
				throw;
			}
		}

		/// <summary>
		/// This is static and public so we can keep the cache current with what this needs
		/// even if we are running in ServerMode
		/// </summary>
		/// <param name="viewTemplate"></param>
		public override void RegisterWithCache(ViewTemplate viewTemplate)
		{
			Field field = viewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
			if (field != null)
			{
				Lexicon.RegisterFieldWithCache(field.WritingSystems, true);
			}
			field = viewTemplate.GetField(LexSense.WellKnownProperties.Gloss);
			if (field != null)
			{
				Lexicon.RegisterFieldWithCache(field.WritingSystems, false);
			}

			Lexicon.RegisterHeadwordListWithCache(viewTemplate.HeadwordWritingSystems);

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

		public override void GoToUrl(string url)
		{
			_dictionaryControl.GoToEntry(GetEntryFromUrl(url));
		}

		private string GetEntryFromUrl(string url)
		{
			return url;
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

		public override string Description
		{
			get
			{
				return String.Format(StringCatalog.Get("~See all {0} {1} words.", "The description of the 'Dictionary' task.  In place of the {0} will be the number of words in the dictionary.  In place of the {1} will be the name of the project."), DataSource.Count, BasilProject.Project.Name);
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

		protected override int ComputeCount(bool returnResultEvenIfExpensive)
		{
			return DataSource.Count;
		}

		protected override int ComputeReferenceCount()
		{
			return CountNotRelevant;
		}

		public override ButtonStyle DashboardButtonStyle
		{
			get { return ButtonStyle.IconFixedWidth; }
		}

		public override Image DashboardButtonImage
		{
			get { return Resources.blueDictionary; }
		}
	}
}