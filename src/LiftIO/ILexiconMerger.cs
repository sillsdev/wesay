using System;
using System.Collections.Specialized;
using System.Text;

namespace LiftIO
{
	public struct IdentifyingInfo
	{
		public string id;
		public DateTime creationTime;
		public DateTime modificationTime;

		static public string LiftTimeFormat = "yyyy-MM-ddThh:mm:sszzzz";
		static public string LiftDateOnlyFormat = "yyyy-MM-dd";

		public override string ToString()
		{
			string s= id + ";";
			if (DateTime.MinValue != creationTime)
			{
				s += creationTime.ToString(LiftTimeFormat);
			}
			s += ";";
			if (DateTime.MinValue != modificationTime)
			{
				s += modificationTime.ToString(LiftTimeFormat);
			}
			s += ";";

			return s;
		}
   }

	public class SimpleMultiText : StringDictionary
	{
		public override string ToString()
		{
			StringBuilder b = new StringBuilder();
			foreach (string key in Keys)
			{
				b.AppendFormat("{0}={1}|", key, this[key]);
			}
			return b.ToString();
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
		TSense GetOrMakeSense(TEntry entry, IdentifyingInfo idInfo);
		TExample GetOrMakeExample(TSense sense, IdentifyingInfo idInfo);
		void MergeInLexemeForm(TEntry entry, SimpleMultiText multiText);
		void MergeInGloss(TSense sense, SimpleMultiText multiText);
		void MergeInExampleForm(TExample example, SimpleMultiText multiText);
		void MergeInTranslationForm(TExample example, SimpleMultiText multiText);

		void MergeInDefinition(TSense sense, SimpleMultiText simpleMultiText);
	}
}