using System.Drawing;
using Palaso.WritingSystems;

namespace WeSay.LexicalModel.Foundation
{
	public class WritingSystemInfo
	{
		public static Font CreateFont(WritingSystemDefinition writingSystem)
		{
			float size = writingSystem.DefaultFontSize > 0 ? writingSystem.DefaultFontSize : 12;
			return new Font(writingSystem.DefaultFontName, size);
		}

		public static string IdForUnknownAnalysis = "en";
		public static string IdForUnknownVernacular = "qaa";

		// TODO move these into a WeSayTestUtilities or some such
		public static string AnalysisIdForTest = "en";
		public static string VernacularIdForTest = "th";
	}
}