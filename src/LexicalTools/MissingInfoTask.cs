using System;
using System.Windows.Forms;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Data;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public class MissingInfoTask : TaskBase
	{
		private MissingInfoControl _missingInfoControl;
		private readonly IFilter<LexEntry> _filter;
		private readonly ViewTemplate _viewTemplate;
		private bool _dataHasBeenRetrieved;
		private LexEntrySortHelper _sortHelper;


		public MissingInfoTask(IRecordListManager recordListManager,
					IFilter<LexEntry> filter,
					string label,
					string description,
					ViewTemplate viewTemplate)
			: base(label, description, false, recordListManager)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}

			_filter = filter;
			_viewTemplate = viewTemplate;


			WritingSystem listWritingSystem = BasilProject.Project.WritingSystems.UnknownVernacularWritingSystem;
			Field field = viewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
			if (field != null)
			{
				if (field.WritingSystems.Count > 0)
				{
					listWritingSystem = field.WritingSystems[0];
				}
				else
				{
					MessageBox.Show(String.Format("There are no writing systems enabled for the Field '{0}'", field.FieldName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);//review
				}
			}

			if (recordListManager is Db4oRecordListManager)
			{
				_sortHelper =
						new LexEntrySortHelper(((Db4oRecordListManager)recordListManager).DataSource,
											   listWritingSystem.Id,
											   true);
			}
			else
			{
				_sortHelper = new LexEntrySortHelper(listWritingSystem.Id, true);
			}

			recordListManager.Register(filter, _sortHelper);
		}

		/// <summary>
		/// Creates a generic Lexical Field editing task
		/// </summary>
		/// <param name="recordListManager">The recordListManager that will provide the data</param>
		/// <param name="filter">The filter that should be used to filter the data</param>
		/// <param name="label">The task label</param>
		/// <param name="description">The task description</param>
		/// <param name="viewTemplate">The base viewTemplate</param>
		/// <param name="fieldsToShow">The fields to show from the base Field Inventory</param>
		public MissingInfoTask(IRecordListManager recordListManager,
							IFilter<LexEntry> filter,
							string label,
							string description,
							ViewTemplate viewTemplate,
							string fieldsToShow)
			:this(recordListManager, filter, label, description, viewTemplate)
		{
			if (fieldsToShow == null)
			{
				throw new ArgumentNullException("fieldsToShow");
			}
			_viewTemplate = FilterviewTemplate(viewTemplate, fieldsToShow);
		}

		static private ViewTemplate FilterviewTemplate(ViewTemplate baseViewTemplate, string fieldsToShow)
		{
			string[] fields = fieldsToShow.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			ViewTemplate viewTemplate = new ViewTemplate();
			foreach (Field field in baseViewTemplate)
			{
				if(Array.IndexOf(fields, field.FieldName) >= 0)
				{
					viewTemplate.Add(field);
				}
			}
			return viewTemplate;
		}

		public override void Activate()
		{
			base.Activate();
			_missingInfoControl = new MissingInfoControl(DataSource, ViewTemplate, _filter.FilteringPredicate);
			_missingInfoControl.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			RecordListManager.GoodTimeToCommit();
		}

		public override void Deactivate()
		{
			base.Deactivate();
		   _missingInfoControl.SelectedIndexChanged -= new EventHandler(OnRecordSelectionChanged);
			_missingInfoControl.Dispose();
			_missingInfoControl = null;
			RecordListManager.GoodTimeToCommit();
		}

		/// <summary>
		/// The MissingInfoControl associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public override Control Control
		{
			get
			{
				return _missingInfoControl;
			}
		}

		public override string Status
		{
			get
			{
				if (_dataHasBeenRetrieved)
				{
					return DataSource.Count.ToString();
				}
				return "-";// String.Empty;
			}
		}
		public override string ExactStatus
		{
			get
			{
				return DataSource.Count.ToString();
			}
		}
		public IRecordList<LexEntry> DataSource
		{
			get
			{
				IRecordList<LexEntry> data = RecordListManager.GetListOfTypeFilteredFurther(_filter, _sortHelper);
				_dataHasBeenRetrieved = true;
				return data;
			}
		}

		public ViewTemplate ViewTemplate
		{
			get { return this._viewTemplate; }
		}
	}
}
