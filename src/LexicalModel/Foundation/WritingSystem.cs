using System;
using System.ComponentModel;
using System.Drawing;
using Palaso.WritingSystems;

namespace WeSay.LexicalModel.Foundation
{
	public class WritingSystem : WritingSystemDefinition
	{
		public bool IsUnicodeEncoded { get; set; } // TODO Introduce IsUnicodeEncoded to palaso wsd.

		// TODO add this to palaso
		public static WritingSystem FromRFC5646(string tag)
		{
			var x = new WritingSystem();
			x.ISO639 = tag;
			return x;

		}

		// TODO add this to palaso
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

		// TODO move to palaso wsd
		public void SortUsingCustomSimple(string p0)
		{
			throw new NotImplementedException();
		}
	}
}