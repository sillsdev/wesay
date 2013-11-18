using Palaso.UI.WindowsForms.Keyboarding;
using Palaso.WritingSystems;

namespace WeSay.LexicalModel.Foundation
{
	public class ChorusWritingSystemAdaptor : Chorus.IWritingSystem
	{
		private readonly IWritingSystemDefinition _writingSystem;

		public ChorusWritingSystemAdaptor(IWritingSystemDefinition writingSystem)
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