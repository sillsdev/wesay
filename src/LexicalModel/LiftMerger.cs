using System;
using System.Collections.Specialized;
using LiftIO;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	/// <summary>
	/// This class is called by the LiftParser, as it encounters each element of a lift file.
	/// There is at least one other ILexiconMerger, used in FLEX.
	///
	/// NB: this doesn't yet merge (dec 2006). Just blindly adds.
	/// </summary>
	public class LiftMerger : ILexiconMerger<LexEntry,LexSense,LexExampleSentence>
	{
		public LexEntry GetOrMakeEntry(IdentifyingInfo idInfo)
		{
			LexEntry e = new LexEntry(GetGuidFromIdString(idInfo.id));

			if (idInfo.creationTime > DateTime.MinValue)
			{
				e.CreationDate = idInfo.creationTime;
			}

			if (idInfo.modificationTime > DateTime.MinValue)
			{
				e.ModifiedDate = idInfo.modificationTime;
			}

			return e;
		}

		private Guid GetGuidFromIdString(string id)
		{
			try
			{
			   return new Guid(id);
			}
			catch (Exception e)
			{
				//enchance: log this, we're throwing away the id they had
				return new Guid();
			}
		}

		public LexSense GetOrMergeSense(LexEntry entry, IdentifyingInfo idInfo)
		{
			//nb, has no guid or dates
			return new LexSense(entry);
		}

		public LexExampleSentence GetOrMergeExample(LexSense sense, IdentifyingInfo idInfo)
		{
			//nb, has no guid or dates
			return new LexExampleSentence(sense);
		}

		public void MergeInLexemeForm(LexEntry entry, StringDictionary forms)
		{
			MergeIn(entry.LexicalForm, forms);
		}

		private static void MergeIn(MultiText multiText, StringDictionary forms)
		{
			multiText.MergeIn(MultiText.Create(forms));
		}

		public void MergeInGloss(LexSense sense, StringDictionary forms)
		{
			MergeIn(sense.Gloss, forms);
		}

		public void MergeInExampleForm(LexExampleSentence example, StringDictionary forms)
		{
			MergeIn(example.Sentence, forms);
		}

		public void MergeInTranslationForm(LexExampleSentence example, StringDictionary forms)
		{
			MergeIn(example.Translation, forms);
		}
	}
}
