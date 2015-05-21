using System;
using Palaso.DictionaryServices.Model;
using SIL.Reporting;
using SIL.WritingSystems;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;
using System.Linq;

namespace WeSay.LexicalTools
{
	public abstract class WordGatheringTaskBase: TaskBase
	{
		private readonly WritingSystemDefinition _lexicalFormWritingSystem;
		private readonly ViewTemplate _viewTemplate;

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
		}

		protected WritingSystemDefinition GetFirstTextWritingSystemOfField(Field field)
		{
			var ids = BasilProject.Project.WritingSystems.FilterForTextLanguageTags(field.WritingSystemIds);
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

		public string WordWritingSystemLanguageTag
		{
			get
			{
				VerifyTaskActivated();
				return _lexicalFormWritingSystem.LanguageTag;
			}
		}

		public WritingSystemDefinition FormWritingSystem
		{
			get
			{
				VerifyTaskActivated();
				return _lexicalFormWritingSystem;
			}
		}
		public WritingSystemDefinition MeaningWritingSystem
		{
			get
			{
				VerifyTaskActivated();
				return _viewTemplate.GetDefaultWritingSystemForField(LexSense.WellKnownProperties.Definition);
			}

		}

		protected ViewTemplate ViewTemplate
		{
			get { return _viewTemplate; }
		}
	}
}