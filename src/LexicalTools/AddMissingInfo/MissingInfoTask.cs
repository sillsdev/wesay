using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools.AddMissingInfo
{
	public class MissingInfoTask: TaskBase
	{
		private MissingInfoControl _missingInfoControl;
		private readonly Field _missingInfoField;
		private readonly ViewTemplate _viewTemplate;
		private bool _dataHasBeenRetrieved;
		//private readonly bool _isBaseFormFillingTask;
		private readonly WritingSystem _writingSystem;
		private MissingInfoConfiguration _config;
		private TaskMemory _taskMemory;

		public MissingInfoTask(MissingInfoConfiguration config,
							   LexEntryRepository lexEntryRepository,
							   ViewTemplate defaultViewTemplate,
								TaskMemoryRepository taskMemoryRepository)
			: base( config, lexEntryRepository)
		{
			_config = config;
			_taskMemory = taskMemoryRepository.FindOrCreateSettingsByTaskId(config.TaskName);

			Guard.AgainstNull(config.MissingInfoField, "MissingInfoField");
			Guard.AgainstNull(defaultViewTemplate, "viewTemplate");

			_missingInfoField = defaultViewTemplate[config.MissingInfoField];

			_viewTemplate = config.CreateViewTemplate(defaultViewTemplate);

			_writingSystem = BasilProject.Project.WritingSystems.UnknownVernacularWritingSystem;
			// use the master view Template instead of the one for this task. (most likely the one for this
			// task doesn't have the EntryLexicalForm field specified but the Master (Default) one will
			Field fieldDefn =
				WeSayWordsProject.Project.DefaultViewTemplate.GetField(
					Field.FieldNames.EntryLexicalForm.ToString());
			if (fieldDefn != null)
			{
				if (fieldDefn.WritingSystemIds.Count > 0)
				{
					_writingSystem = BasilProject.Project.WritingSystems[fieldDefn.WritingSystemIds[0]];
				}
				else
				{
					throw new ConfigurationException("There are no writing systems enabled for the Field '{0}'",
													 fieldDefn.FieldName);
				}
			}
		}


		public override DashboardGroup Group
		{
			get
			{
				return _config.Group;
			}
		}





		public override void Activate()
		{
			base.Activate();

			Predicate<LexEntry> filteringPredicate =
				new MissingFieldQuery(_missingInfoField).FilteringPredicate;
			_missingInfoControl = new MissingInfoControl(GetFilteredData(),
														 ViewTemplate,
														 filteringPredicate,
														 LexEntryRepository,
														 _taskMemory.CreateNewSection("view"));
			_missingInfoControl.SelectedIndexChanged += OnRecordSelectionChanged;
		}

		private void OnRecordSelectionChanged(object sender, EventArgs e)
		{
			LexEntryRepository.SaveItem(_missingInfoControl.CurrentEntry);
		}

		public override void Deactivate()
		{
			if (_missingInfoControl != null && _missingInfoControl.CurrentEntry!=null)
			{
				LexEntryRepository.SaveItem(_missingInfoControl.CurrentEntry);
			}
			base.Deactivate();
			if (_missingInfoControl != null)
			{
				_missingInfoControl.SelectedIndexChanged -= OnRecordSelectionChanged;
				_missingInfoControl.Dispose();
			}
			_missingInfoControl = null;
		}

		/// <summary>
		/// The MissingInfoControl associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public override Control Control
		{
			get { return _missingInfoControl; }
		}

		protected override int ComputeCount(bool returnResultEvenIfExpensive)
		{
			if (_dataHasBeenRetrieved || returnResultEvenIfExpensive)
			{
				return GetFilteredData().Count;
			}
			return CountNotComputed;
		}

		protected override int ComputeReferenceCount()
		{
			//TODO: Make this correct for Examples.  Currently it counts all words which
			//gives an incorrect progress indicator when not all words have meanings
			return LexEntryRepository.CountAllItems();
		}

		public ResultSet<LexEntry> GetFilteredData()
		{
			ResultSet<LexEntry> data =
				LexEntryRepository.GetEntriesWithMissingFieldSortedByLexicalUnit(
					_missingInfoField, _writingSystem);
			_dataHasBeenRetrieved = true;
			return data;
		}

		public ViewTemplate ViewTemplate
		{
			get { return _viewTemplate; }
		}
	}
}
