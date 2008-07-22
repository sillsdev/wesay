using System;
using System.ComponentModel;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Data;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public class MissingInfoTask : TaskBase, ISetupIndices
	{
		private MissingInfoControl _missingInfoControl;
		private readonly IRecordListManager _recordListManager;
		private readonly IFilter<LexEntry> _filter;
		private readonly ViewTemplate _viewTemplate;
		private bool _dataHasBeenRetrieved;
		private LexEntrySortHelper _sortHelper;
		private bool _isBaseFormFillingTask;


		public MissingInfoTask(IRecordListManager recordListManager,
			IFilter<LexEntry> filter,
					string label,
					string longLabel,
					string description,
					string remainingCountText,
					string referenceCountText,
					ViewTemplate viewTemplate)
			: base(label, longLabel, description, remainingCountText, referenceCountText, false, recordListManager)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}

			_recordListManager = recordListManager;
			_filter = filter;
			_viewTemplate = viewTemplate;



			RegisterIndicesNow(viewTemplate);
		}

		public void RegisterIndicesNow(ViewTemplate viewTemplate)
		{
			WritingSystem listWritingSystem = BasilProject.Project.WritingSystems.UnknownVernacularWritingSystem;
			// use the master view Template instead of the one for this task. (most likely the one for this
			// task doesn't have the EntryLexicalForm field specified but the Master (Default) one will
			Field field = WeSayWordsProject.Project.DefaultViewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
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

			if (_recordListManager is Db4oRecordListManager)
			{
				_sortHelper =
						new LexEntrySortHelper(((Db4oRecordListManager)_recordListManager).DataSource,
											   listWritingSystem,
											   true);
			}
			else
			{
				_sortHelper = new LexEntrySortHelper(listWritingSystem, true);
			}
			_recordListManager.Register(_filter, _sortHelper);
			_recordListManager.Register(new AllItems<LexEntry>(), _sortHelper);
		}


		//Instead, use ISetupIndices
		public override bool MustBeActivatedDuringPreCache
		{
			get { return false; }
		}

		public MissingInfoTask(IRecordListManager recordListManager,
			IFilter<LexEntry> filter,
					string label,
					string longLabel,
					string description,
					ViewTemplate viewTemplate)
			: this(recordListManager, filter, label, longLabel, description, string.Empty, string.Empty, viewTemplate) {}

		public MissingInfoTask(IRecordListManager recordListManager,
			IFilter<LexEntry> filter,
					string label,
					string description,
					ViewTemplate viewTemplate)
			: this(recordListManager, filter, label, label, description, string.Empty, string.Empty, viewTemplate) { }

		/// <summary>
		/// Creates a generic Lexical Field editing task
		/// </summary>
		/// <param name="recordListManager">The recordListManager that will provide the data</param>
		/// <param name="filter">The filter that should be used to filter the data</param>
		/// <param name="label">The task label</param>
		/// <param name="longLabel">Slightly longer task label (for ToolTips)</param>
		/// <param name="description">The task description</param>
		/// <param name="remainingCountText">Text describing the remaining count</param>
		/// <param name="referenceCountText">Text describing the reference count</param>
		/// <param name="viewTemplate">The base viewTemplate</param>
		/// <param name="fieldsToShow">The fields to show from the base Field Inventory</param>
		public MissingInfoTask(IRecordListManager recordListManager,
							IFilter<LexEntry>  filter,
							string label,
							string longLabel,
							string description,
							string remainingCountText,
							string referenceCountText,
							ViewTemplate viewTemplate,
							string fieldsToShow)
			:this(recordListManager, filter, label, longLabel, description, remainingCountText, referenceCountText, viewTemplate)
		{
			if (fieldsToShow == null)
			{
				throw new ArgumentNullException("fieldsToShow");
			}
			_viewTemplate = CreateViewTemplateFromListOfFields(viewTemplate, fieldsToShow);

			//hack until we overhaul how Tasks are setup:
			_isBaseFormFillingTask = filter is MissingItemFilter && fieldsToShow.Contains(LexEntry.WellKnownProperties.BaseForm);
			if (_isBaseFormFillingTask)
			{
				MissingItemFilter f = filter as MissingItemFilter;
				Field flagField = new Field();
				flagField.DisplayName = StringCatalog.Get("~This word has no Base Form", "The user will click this to say that this word has no baseform.  E.g. Kindess has Kind as a baseform, but Kind has no other word as a baseform.");
				flagField.DataTypeName = "Flag";
				flagField.ClassName = "LexEntry";
				flagField.FieldName = "flag_skip_" + ((MissingItemFilter) filter).FieldName;
				flagField.Enabled = true;
				_viewTemplate.Add(flagField);
			}
		}

		public MissingInfoTask(IRecordListManager recordListManager,
					IFilter<LexEntry>  filter,
					string label,
					string longLabel,
					string description,
					ViewTemplate viewTemplate,
					string fieldsToShow)
			:this(recordListManager, filter, label, longLabel, description, string.Empty, string.Empty, viewTemplate, fieldsToShow) {}

		public MissingInfoTask(IRecordListManager recordListManager,
			IFilter<LexEntry> filter,
			string label,
			string description,
			ViewTemplate viewTemplate,
			string fieldsToShow)
			: this(recordListManager, filter, label, label, description, string.Empty, string.Empty, viewTemplate, fieldsToShow) { }

		public override WeSay.Foundation.Dashboard.DashboardGroup Group
		{
			get
			{
				if (_isBaseFormFillingTask)
				{
					return WeSay.Foundation.Dashboard.DashboardGroup.Refine;
				}
				return base.Group;
			}
		}
		public MissingInfoTask(IRecordListManager recordListManager,
			IFilter<LexEntry>  filter,
					string label,
					string longLabel,
					string description,
					string remainingCountText,
					string referenceCountText,
					ViewTemplate viewTemplate,
					string fieldsToShowEditable,
					string fieldsToShowReadOnly)
			: this(recordListManager, filter, label, longLabel, description, remainingCountText,
				   referenceCountText, viewTemplate, fieldsToShowEditable+" "+fieldsToShowReadOnly)
		{
			MarkReadOnlyFIelds(fieldsToShowReadOnly);
		}

		public MissingInfoTask(IRecordListManager recordListManager,
			IFilter<LexEntry>  filter,
					string label,
					string longLabel,
					string description,
					ViewTemplate viewTemplate,
					string fieldsToShowEditable,
					string fieldsToShowReadOnly)
			:this(recordListManager, filter, label, longLabel, description, string.Empty, string.Empty,
				  viewTemplate, fieldsToShowEditable, fieldsToShowReadOnly) {}

		public MissingInfoTask(IRecordListManager recordListManager,
			IFilter<LexEntry> filter,
					string label,
					string description,
					ViewTemplate viewTemplate,
					string fieldsToShowEditable,
					string fieldsToShowReadOnly)
			: this(recordListManager, filter, label, label, description, string.Empty, string.Empty,
				  viewTemplate, fieldsToShowEditable, fieldsToShowReadOnly) { }


		private void MarkReadOnlyFIelds(string fieldsToShowReadOnly)
		{
			string[] readOnlyFields = SplitUpFieldNames(fieldsToShowReadOnly);

			for (int i = 0; i < _viewTemplate.Count; i++)
			{
				Field field = _viewTemplate[i];
				foreach (string s in readOnlyFields)
				{
					if(s==field.FieldName)
					{
						Field readOnlyVersion = new Field(field);
						readOnlyVersion.Visibility = WeSay.Foundation.CommonEnumerations.VisibilitySetting.ReadOnly;
						_viewTemplate.Remove(field);
						_viewTemplate.Insert(i, readOnlyVersion);
					}
				}
			}
		}

		static private ViewTemplate CreateViewTemplateFromListOfFields(ViewTemplate baseViewTemplate, string fieldsToShow)
		{
			string[] fields = SplitUpFieldNames(fieldsToShow);
			ViewTemplate viewTemplate = new ViewTemplate();
			foreach (Field field in baseViewTemplate)
			{
				if(Array.IndexOf(fields, field.FieldName) >= 0)
				{
					if (field.Enabled == false)//make sure specified fields are shown (greg's ws-356)
					{
						Field enabledField = new Field(field);
						enabledField.Enabled = true;
						viewTemplate.Add(enabledField);
					}
					else
					{
						if (field.Visibility != CommonEnumerations.VisibilitySetting.Visible)//make sure specified fields are visible (not in 'rare mode)
						{
							Field visibleField = new Field(field);
							visibleField.Visibility = CommonEnumerations.VisibilitySetting.Visible;
							viewTemplate.Add(visibleField);
						}
						else
						{
							viewTemplate.Add(field);
						}
					}


				}
			}
			return viewTemplate;
		}

		private static string[] SplitUpFieldNames(string fieldsToShow)
		{
			return fieldsToShow.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public override void Activate()
		{
			base.Activate();
			IBindingList allRecords;
			if (RecordListManager is Db4oRecordListManager)
			{
				allRecords = ((Db4oRecordListManager) RecordListManager).GetSortedList(_sortHelper);
			}
			else
			{
				allRecords = RecordListManager.GetListOfTypeFilteredFurther(new AllItems<LexEntry>(), _sortHelper);
			}


			_missingInfoControl = new MissingInfoControl(DataSource, ViewTemplate, _filter.FilteringPredicate, RecordListManager);
			_missingInfoControl.SelectedIndexChanged += new EventHandler(OnRecordSelectionChanged);
		}

		void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			RecordListManager.GoodTimeToCommit();
		}

		public override void Deactivate()
		{
			base.Deactivate();
		   if (_missingInfoControl != null)
		   {
			   _missingInfoControl.SelectedIndexChanged -= new EventHandler(OnRecordSelectionChanged);
			   _missingInfoControl.Dispose();
		   }
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

		protected override int ComputeCount(bool returnResultEvenIfExpensive)
		{
			if (_dataHasBeenRetrieved || returnResultEvenIfExpensive)
			{
				return DataSource.Count;
			}
			return CountNotComputed;
		}

		protected override int ComputeReferenceCount()
		{
			//TODO: Make this correct for Examples.  Currently it counts all words which
			//gives an incorrect progress indicator when not all words have meanings
			return RecordListManager.GetListOfType<LexEntry>().Count;
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
			get
			{
				return _viewTemplate;
			}
		}
	}
}
