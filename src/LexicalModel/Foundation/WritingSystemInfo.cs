using System.Drawing;

namespace WeSay.LexicalModel.Foundation
{
	public class WritingSystemInfo
	{
		public static Font CreateFont(WritingSystem writingSystem)
		{
			return new Font(writingSystem.DefaultFontName, writingSystem.DefaultFontSize);
		}

		public static string IdForUnknownAnalysis = "en";
		public static string IdForUnknownVernacular = "v";

		// TODO move these into a WeSayTestUtilities or some such
		public static string AnalysisIdForTest = "PretendAnalysis";
		public static string VernacularIdForTest = "PretendVernacular";
	}
}