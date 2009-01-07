using System;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public abstract class WordGatheringTaskBase: TaskBase
	{
		private readonly WritingSystem _lexicalFormWritingSystem;
		private readonly ViewTemplate _viewTemplate;

		protected WordGatheringTaskBase(ITaskConfiguration config,
										LexEntryRepository lexEntryRepository,
										ViewTemplate viewTemplate)
				: base( config,
						lexEntryRepository)
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
				_lexicalFormWritingSystem = writingSystems[lexicalFormField.WritingSystemIds[0]];
			}
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

		public WritingSystem WordWritingSystem
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