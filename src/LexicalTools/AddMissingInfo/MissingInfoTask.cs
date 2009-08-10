using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Palaso.Data;
using Palaso.Misc;
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
		private readonly Field _missingInfoField;
		private readonly ViewTemplate _viewTemplate;
		private readonly MissingInfoConfiguration _config;
		private readonly TaskMemory _taskMemory;
		private MissingInfoControl _missingInfoControl;
		private bool _dataHasBeenRetrieved;

		public MissingInfoTask(MissingInfoConfiguration config,
							   LexEntryRepository lexEntryRepository,
							   ViewTemplate defaultViewTemplate,
								TaskMemoryRepository taskMemoryRepository)
			: base( config, lexEntryRepository, taskMemoryRepository)
		{
			Guard.AgainstNull(config.MissingInfoFieldName, "MissingInfoFieldName");
			Guard.AgainstNull(defaultViewTemplate, "viewTemplate");
			Debug.Assert(config.WritingSystemsWeWantToFillInArray == null ||
						 config.WritingSystemsWeWantToFillInArray.Length == 0 ||
						 !string.IsNullOrEmpty(config.WritingSystemsWeWantToFillInArray[0]));

			_config = config;
			_taskMemory = taskMemoryRepository.FindOrCreateSettingsByTaskId(config.TaskName);


			_missingInfoField = defaultViewTemplate[config.MissingInfoFieldName];

			_viewTemplate = config.CreateViewTemplate(defaultViewTemplate);
		 }

		private WritingSystem GetLexicalUnitWritingSystem()
		{
			//NB: don't replace these ugly static uses with the _viewTemplate we were given... that won't have what we're looking for here

			var ws = BasilProject.Project.WritingSystems.UnknownVernacularWritingSystem;
			// use the master view Template instead of the one for this task. (most likely the one for this
			// task doesn't have the EntryLexicalForm field specified but the Master (Default) one will
			Field fieldDefn = WeSayWordsProject.Project.DefaultViewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
			if (fieldDefn != null)
			{
				if (fieldDefn.WritingSystemIds.Count > 0)
				{
					ws = BasilProject.Project.WritingSystems[fieldDefn.WritingSystemIds[0]];
				}
				else
				{
					throw new ConfigurationException("There are no writing systems enabled for the Field '{0}'",
													 fieldDefn.FieldName);
				}
			}
			return ws;
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

			_missingInfoControl = new MissingInfoControl(GetFilteredData(),
														 ViewTemplate,
														 GetQuery().FilteringPredicate,
														 LexEntryRepository,
														 _taskMemory.CreateNewSection("view"));
			_missingInfoControl.TimeToSaveRecord += OnSaveRecord;
		}

		private void OnSaveRecord(object sender, EventArgs e)
		{
			SaveRecord();
		}

		private void SaveRecord()
		{
				   if (_missingInfoControl != null && _missingInfoControl.CurrentEntry != null)
				{
					LexEntryRepository.SaveItem(_missingInfoControl.CurrentEntry);
				}

		}

		public override void Deactivate()
		{
			SaveRecord();
			base.Deactivate();
			if (_missingInfoControl != null)
			{
				_missingInfoControl.TimeToSaveRecord -= OnSaveRecord;
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
					GetQuery(), _missingInfoField, GetLexicalUnitWritingSystem());
			_dataHasBeenRetrieved = true;
			return data;
		}

		private MissingFieldQuery GetQuery()
		{
			return new MissingFieldQuery(_missingInfoField, _config.WritingSystemsWeWantToFillInArray,
										   _config.WritingSystemsWhichAreRequiredArray);

		}

		public ViewTemplate ViewTemplate
		{
			get { return _viewTemplate; }
		}
	}
}
