using System.Drawing;
using Palaso.WritingSystems;

namespace WeSay.LexicalModel.Foundation
{

	public class WritingSystem : WritingSystemDefinition
	{
		public bool IsUnicode { get; private set; } // Rename  TODO Introduce IsUnicodeEncoded to palaso wsd.

		public bool IsAudio { get; set; } // Rename to IsVoice

		public bool RightToLeft { get; set; }

		public string KeyboardName { get; set; }

		public Font Font
		{
			get
			{
				return new Font(DefaultFontName, DefaultFontSize);
			}
		}


	}
}