using Chorus;
using SIL.WritingSystems;

namespace WeSay.LexicalModel.Foundation
{
	public class ChorusWritingSystemAdaptor : IWritingSystem
	{
		private readonly WritingSystemDefinition _writingSystem;

		public ChorusWritingSystemAdaptor(WritingSystemDefinition writingSystem)
		{
			_writingSystem = writingSystem;
		}

		public void ActivateKeyboard()
		{
			_writingSystem.LocalKeyboard.Activate();
		}

		public string Name
		{
			get { return _writingSystem.Abbreviation; }
		}

		public string Code
		{
			get { return _writingSystem.LanguageTag; }
		}

		public string FontName
		{
			get { return _writingSystem.DefaultFont.Name; }
		}

		public int FontSize
		{
			get { return (int)_writingSystem.GetDefaultFontSizeOrMinimum(); }
		}
	}
}