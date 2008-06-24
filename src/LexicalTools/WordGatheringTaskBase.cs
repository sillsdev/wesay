using System;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Dashboard;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public abstract class WordGatheringTaskBase : TaskBase
	{
		private readonly WritingSystem _lexicalFormWritingSystem;
		private readonly ViewTemplate _viewTemplate;

		protected WordGatheringTaskBase(string label, string longLabel, string description, string remainingCountText,
										string referenceCountText, bool isPinned, IRecordListManager recordListManager,
										ViewTemplate viewTemplate)
			: base(label, longLabel, description, remainingCountText, referenceCountText, isPinned, recordListManager)
		{
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}

			_viewTemplate = viewTemplate;
			Field lexicalFormField = viewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
			if (lexicalFormField == null || lexicalFormField.WritingSystems.Count < 1)
			{
				_lexicalFormWritingSystem = BasilProject.Project.WritingSystems.UnknownVernacularWritingSystem;
			}
			else
			{
				_lexicalFormWritingSystem = lexicalFormField.WritingSystems[0];
			}
		}

		protected WordGatheringTaskBase(string label, string longLabel, string description, bool isPinned,
										IRecordListManager recordListManager, ViewTemplate viewTemplate)
			: this(label, longLabel, description, string.Empty, string.Empty, isPinned, recordListManager, viewTemplate) {}

		public  override WeSay.Foundation.Dashboard.DashboardGroup Group
		{
			get { return WeSay.Foundation.Dashboard.DashboardGroup.Gather; }
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
			get { return this._viewTemplate; }
		}
	}
}