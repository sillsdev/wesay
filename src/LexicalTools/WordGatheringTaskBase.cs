using System;
using Palaso.Reporting;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;
using System.Linq;

namespace WeSay.LexicalTools
{
	public abstract class WordGatheringTaskBase: TaskBase
	{
		private readonly WritingSystem _lexicalFormWritingSystem;
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
			Field lexicalFormField =
					viewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
			WritingSystemCollection writingSystems = BasilProject.Project.WritingSystems;
			if (lexicalFormField == null || lexicalFormField.WritingSystemIds.Count < 1)
			{
				_lexicalFormWritingSystem = writingSystems.UnknownVernacularWritingSystem;
			}
			else
			{
				_lexicalFormWritingSystem = GetFirstTestWritingSystemOfField(lexicalFormField);
			}
		}

		protected WritingSystem GetFirstTestWritingSystemOfField(Field field)
		{
			var ids = field.GetTextOnlyWritingSystemIds(BasilProject.Project.WritingSystems);
			if(ids.Count()==0)
			{
				throw new ConfigurationException(string.Format("The field {0} must have at least one non-audio writing system.", field.DisplayName));
			}
			return BasilProject.Project.WritingSystems[ids.First()];
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

		public WritingSystem WritingSystemUserIsTypingIn
		{
			get
			{
				VerifyTaskActivated();
				return _lexicalFormWritingSystem;
			}
		}

		protected ViewTemplate ViewTemplate
		{
			get { return _viewTemplate; }
		}
	}
}