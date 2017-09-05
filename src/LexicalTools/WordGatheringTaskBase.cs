using System;
using Palaso.DictionaryServices.Model;
using Palaso.Reporting;
using Palaso.WritingSystems;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;
using System.Linq;

namespace WeSay.LexicalTools
{
	public abstract class WordGatheringTaskBase: TaskBase
	{
		private readonly IWritingSystemDefinition _lexicalFormWritingSystem;
		private readonly ViewTemplate _viewTemplate;
		protected bool _glossMeaningField;

		protected WordGatheringTaskBase(ITaskConfiguration config,
										LexEntryRepository lexEntryRepository,
										ViewTemplate viewTemplate,
										TaskMemoryRepository taskMemoryRepository)
				: base( config,
						lexEntryRepository, taskMemoryRepository)
		{
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}

			_viewTemplate = viewTemplate;
			_lexicalFormWritingSystem =
				 viewTemplate.GetDefaultWritingSystemForField(Field.FieldNames.EntryLexicalForm.ToString());

			var glossField = _viewTemplate.GetField(LexSense.WellKnownProperties.Gloss);
			if (glossField == null)
			{
				_glossMeaningField = false;
			}
			else
			{
				_glossMeaningField = glossField.IsMeaningField;
			}
		}

		protected IWritingSystemDefinition GetFirstTextWritingSystemOfField(Field field)
		{
			var ids = BasilProject.Project.WritingSystems.FilterForTextIds(field.WritingSystemIds);
			if(ids.Count()==0)
			{
				throw new ConfigurationException(string.Format("The field {0} must have at least one non-audio input system.", field.DisplayName));
			}
			return BasilProject.Project.WritingSystems.Get(ids.First());
		}

		public override DashboardGroup Group
		{
			get { return DashboardGroup.Gather; }
		}

		public override ButtonStyle DashboardButtonStyle
		{
			get { return ButtonStyle.FixedAmount; }
		}

		public string WordWritingSystemId
		{
			get
			{
				VerifyTaskActivated();
				return _lexicalFormWritingSystem.Id;
			}
		}

		public IWritingSystemDefinition FormWritingSystem
		{
			get
			{
				VerifyTaskActivated();
				return _lexicalFormWritingSystem;
			}
		}
		public IWritingSystemDefinition MeaningWritingSystem
		{
			get
			{
				VerifyTaskActivated();
				if (_glossMeaningField)
				{
					return _viewTemplate.GetDefaultWritingSystemForField(LexSense.WellKnownProperties.Gloss);
				}
				else
				{
					return _viewTemplate.GetDefaultWritingSystemForField(LexSense.WellKnownProperties.Definition);
				}
			}

		}

		protected ViewTemplate ViewTemplate
		{
			get { return _viewTemplate; }
		}
	}
}