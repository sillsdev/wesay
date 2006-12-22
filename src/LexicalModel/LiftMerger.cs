using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using LiftIO;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.LexicalModel
{
	/// <summary>
	/// This class is called by the LiftParser, as it encounters each element of a lift file.
	/// There is at least one other ILexiconMerger, used in FLEX.
	///
	/// NB: this doesn't yet merge (dec 2006). Just blindly adds.
	/// </summary>
	public class LiftMerger : ILexiconMerger<LexEntry,LexSense,LexExampleSentence>, IDisposable
	{
		private Db4oDataSource _dataSource;
		private WeSay.Data.Db4oRecordList<LexEntry> _entries;


		public LiftMerger(Db4oDataSource dataSource)
		{
			_dataSource = dataSource;
			_entries = new WeSay.Data.Db4oRecordList<LexEntry>(_dataSource);
		}

		public LexEntry GetOrMakeEntry(IdentifyingInfo idInfo)
		{
			Guid guid = GetGuidOrEmptyFromIdString(idInfo.id);
			LexEntry entry = null;
			if (guid != Guid.Empty)
			{
				entry = Db4oLexQueryHelper.FindObjectFromGuid<LexEntry>(_dataSource, guid);

				if (CanSafelyPruneMerge(idInfo, entry))
				{
					return null; // no merging needed
				}
			}

			if (entry == null)
			{
				entry = new LexEntry(guid);
			}

			if (idInfo.creationTime > DateTime.MinValue)
			{
				entry.CreationTime = idInfo.creationTime;
			}

			if (idInfo.modificationTime > DateTime.MinValue)
			{
				entry.ModificationTime = idInfo.modificationTime;
			}

			return entry;
		}

		private static bool CanSafelyPruneMerge(IdentifyingInfo idInfo, LexEntry entry)
		{
			return entry != null
				&& entry.ModificationTime == idInfo.modificationTime
				&& entry.ModificationTime.Kind != DateTimeKind.Unspecified
				 && idInfo.modificationTime.Kind != DateTimeKind.Unspecified;
		}

		private Guid GetGuidOrEmptyFromIdString(string id)
		{
			try
			{
				return new Guid(id);
			}
			catch (Exception e)
			{
				//enchance: log this, we're throwing away the id they had
				return Guid.Empty;
			}
		}

		public LexSense GetOrMakeSense(LexEntry entry, IdentifyingInfo idInfo)
		{
			//nb, has no guid or dates
			return new LexSense(entry);
		}

		public LexExampleSentence GetOrMakeExample(LexSense sense, IdentifyingInfo idInfo)
		{
			//nb, has no guid or dates
			return new LexExampleSentence(sense);
		}

		public void MergeInLexemeForm(LexEntry entry, SimpleMultiText forms)
		{
			MergeIn(entry.LexicalForm, forms);
		}



		public void MergeInGloss(LexSense sense, SimpleMultiText forms)
		{
			MergeIn(sense.Gloss, forms);
		}

		public void MergeInExampleForm(LexExampleSentence example, SimpleMultiText forms)
		{
			MergeIn(example.Sentence, forms);
		}

		public void MergeInTranslationForm(LexExampleSentence example, SimpleMultiText forms)
		{
			MergeIn(example.Translation, forms);
		}

		public void MergeInDefinition(LexSense sense, SimpleMultiText simpleMultiText)
		{
			throw new NotImplementedException();
		}

		private static void MergeIn(MultiText multiText, SimpleMultiText forms)
		{
			multiText.MergeIn(MultiText.Create(forms));
		}

		 public void Dispose()
		{
			 _entries.Dispose();
		}
	}
}
