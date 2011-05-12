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
	}
}