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

		protected WordGatheringTaskBase(string label,
										string longLabel,
										string description,
										string remainingCountText,
										string referenceCountText,
										bool isPinned,
										LexEntryRepository lexEntryRepository,
										ViewTemplate viewTemplate)
				: base(label, longLabel, description, remainingCountText, referenceCountText, isPinned, lexEntryRepository)
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
				_lexicalFormWritingSystem =
						writingSystems.UnknownVernacularWritingSystem;
			}
			else
			{
				_lexicalFormWritingSystem = writingSystems[lexicalFormField.WritingSystemIds[0]];
			}
		}

		protected WordGatheringTaskBase(string label,
										string longLabel,
										string description,
										bool isPinned,
										LexEntryRepository lexEntryRepository,
										ViewTemplate viewTemplate)
				: this(label, longLabel, description, string.Empty, string.Empty, isPinned, lexEntryRepository, viewTemplate) { }

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