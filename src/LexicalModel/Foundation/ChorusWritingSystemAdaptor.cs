using Palaso.UI.WindowsForms.Keyboarding;
using Palaso.WritingSystems;

namespace WeSay.LexicalModel.Foundation
{
	public class ChorusWritingSystemAdaptor : Chorus.IWritingSystem
	{
		private readonly WritingSystemDefinition _writingSystem;

		public ChorusWritingSystemAdaptor(WritingSystemDefinition writingSystem)
		{
			_writingSystem = writingSystem;
		}

		public void ActivateKeyboard()
		{
			if (string.IsNullOrEmpty(_writingSystem.Keyboard))
			{
				KeyboardController.DeactivateKeyboard();
			}
			else
			{
				KeyboardController.ActivateKeyboard(_writingSystem.Keyboard);
			}
		}

		public string Name
		{
			get { return _writingSystem.Abbreviation; }
		}

		public string Code
		{
			get { return _writingSystem.Id; }
		}

		public string FontName
		{
			get { return _writingSystem.DefaultFontName; }
		}

		public int FontSize
		{
			get { return (int)_writingSystem.GetDefaultFontSizeOrMinimum(); }
		}
	}
}