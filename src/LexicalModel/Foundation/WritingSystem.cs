using System;
using System.Drawing;
using Palaso.WritingSystems;

namespace WeSay.LexicalModel.Foundation
{
	public class WritingSystem : WritingSystemDefinition
	{
		public bool IsUnicodeEncoded { get; set; } // TODO Introduce IsUnicodeEncoded to palaso wsd.

		public object CustomSortRules { get; set; }

		// TODO this is only used in tests: should we care about conversion to/from string?
		public new string SortUsing { get; set; }

		// TODO add this to palaso
		public static WritingSystem FromRFC5646(string rftTag)
		{
			throw new NotImplementedException();
		}

		public static WritingSystemDefinition VoiceFromRFC5646(string empty)
		{
			throw new NotImplementedException();
		}

		// TODO Move to palaso wsd
		public void SortUsingOtherLanguage(string fr)
		{
			throw new NotImplementedException();
		}

		// TODO move to palaso wsd
		public void SortUsingCustomICU(string lastPrimaryIgnorable)
		{
			throw new NotImplementedException();
		}
	}
}