using System;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public abstract class WordGatheringTaskBase : TaskBase
	{
		private readonly WritingSystem _lexicalFormWritingSystem;
		protected WordGatheringTaskBase(string label, string description, bool isPinned,
										IRecordListManager recordListManager, ViewTemplate viewTemplate)
			: base(label, description, isPinned, recordListManager)
		{
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
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
	}

	public class EntryDisplayProxy
	{
		private readonly string _writingSystemId;
		private LexEntry _entry;
		public EntryDisplayProxy(LexEntry entry, string writingSystemId)
		{
			_writingSystemId = writingSystemId;
			_entry = entry;
		}

		public LexEntry Entry
		{
			get { return _entry; }
		}

		public override string ToString()
		{
			return _entry.LexicalForm.GetBestAlternative(_writingSystemId, "*");
		}
	}
}