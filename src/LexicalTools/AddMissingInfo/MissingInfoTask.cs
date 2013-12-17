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
		private readonly bool _isBaseFormFillingTask;
		private readonly WritingSystem _writingSystem;

		public MissingInfoTask(MissingInfoConfiguration config,
							   LexEntryRepository lexEntryRepository,
							   ViewTemplate viewTemplate)
			: base( config, lexEntryRepository)
		{
			if (config.MissingInfoField == null)
			{
				throw new ArgumentNullException("MissingInfoField");
			}
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}

			_missingInfoField = viewTemplate[config.MissingInfoField];

			_viewTemplate = CreateViewTemplateFromListOfFields(viewTemplate, config.FieldsToShow);
			MarkReadOnlyFields(config.FieldsToShowReadOnly);

			//hack until we overhaul how Tasks are setup:
			_isBaseFormFillingTask = config.FieldsToShow.Contains(LexEntry.WellKnownProperties.BaseForm);
			if (_isBaseFormFillingTask)
			{
				Field flagField = new Field();
				flagField.DisplayName = StringCatalog.Get("~This word has no Base Form",
														  "The user will click this to say that this word has no baseform.  E.g. Kindess has Kind as a baseform, but Kind has no other word as a baseform.");
				flagField.DataTypeName = "Flag";
				flagField.ClassName = "LexEntry";
				flagField.FieldName = "flag_skip_" + config.MissingInfoField;
				flagField.Enabled = true;
				_viewTemplate.Add(flagField);
			}
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
				if (_isBaseFormFillingTask)
				{
					return DashboardGroup.Refine;
				}
				return base.Group;
			}
		}



		private void MarkReadOnlyFields(string fieldsToShowReadOnly)
		{
			string[] readOnlyFields = SplitUpFieldNames(fieldsToShowReadOnly);

			for (int i = 0;i < _viewTemplate.Count;i++)
			{
				Field field = _viewTemplate[i];
				foreach (string s in readOnlyFields)
				{
					if (s == field.FieldName)
					{
						Field readOnlyVersion = new Field(field);
						readOnlyVersion.Visibility = CommonEnumerations.VisibilitySetting.ReadOnly;
						_viewTemplate.Remove(field);
						_viewTemplate.Insert(i, readOnlyVersion);
					}
				}
			}
		}

		private static ViewTemplate CreateViewTemplateFromListOfFields(IEnumerable<Field> fieldList,
																	   string fieldsToShow)
		{
			string[] fields = SplitUpFieldNames(fieldsToShow);
			ViewTemplate viewTemplate = new ViewTemplate();
			foreach (Field field in fieldList)
			{
				if (Array.IndexOf(fields, field.FieldName) >= 0)
				{
					if (field.Enabled == false)
						//make sure specified fields are shown (greg's ws-356)
					{
						Field enabledField = new Field(field);
						enabledField.Visibility = CommonEnumerations.VisibilitySetting.Visible;
						enabledField.Enabled = true;
						viewTemplate.Add(enabledField);
					}
					else
					{
						if (field.Visibility != CommonEnumerations.VisibilitySetting.Visible)
							//make sure specified fields are visible (not in 'rare mode)
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
			return fieldsToShow.Split(new char[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries);
		}

		public override void Activate()
		{
			base.Activate();

			Predicate<LexEntry> filteringPredicate =
				new MissingFieldQuery(_missingInfoField).FilteringPredicate;
			_missingInfoControl = new MissingInfoControl(GetFilteredData(),
														 ViewTemplate,
														 filteringPredicate,
														 LexEntryRepository);
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