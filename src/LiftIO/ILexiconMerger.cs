using System;
using System.Collections.Specialized;

namespace LiftIO
{
	public struct IdentifyingInfo
	{
		public string id;
		public DateTime creationTime;
		public DateTime modificationTime;

		public override string ToString()
		{
			string s= id + ";";
			if (DateTime.MinValue != creationTime)
			{
				s += creationTime;
			}
			s += ";";
			if (DateTime.MinValue != modificationTime)
			{
				s += modificationTime;
			}
			s += ";";

			return s;
		}
   }

	/// <summary>
	/// Use with the LiftParser (but concievably other drivers). Allows the same parser
	/// to push LIFT data into multiple systems, e.g. WeSay and FLEx.  Also decoouples
	/// different versions of the lift-specific parser from the model-specific stuff,
	/// so either can change or have multiple implementations.
	/// </summary>
	public interface ILexiconMerger<TEntry, TSense, TExample>
	{
		TEntry GetOrMakeEntry(IdentifyingInfo idInfo);
		TSense GetOrMergeSense(TEntry entry, IdentifyingInfo idInfo);
		TExample GetOrMergeExample(TSense sense, IdentifyingInfo idInfo);
		void MergeInLexemeForm(TEntry entry, StringDictionary multiText);
		void MergeInGloss(TSense sense, StringDictionary multiText);
		void MergeInExampleForm(TExample example, StringDictionary multiText);
		void MergeInTranslationForm(TExample example, StringDictionary multiText);
   }
}