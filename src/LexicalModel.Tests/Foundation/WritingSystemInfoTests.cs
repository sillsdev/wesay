using System.Drawing;
using NUnit.Framework;
using SIL.WritingSystems;
using WeSay.LexicalModel.Foundation;

namespace WeSay.LexicalModel.Tests.Foundation
{
	[TestFixture]
	public class WritingSystemInfoTests
	{
#if WS_FIX
// Does this go away?
		[Test]
		public void CreateFont_Default_GetReturnsGenericSansSerif()
		{
			var ws = new WritingSystemDefinition();
			Assert.AreEqual(FontFamily.GenericSansSerif, WritingSystemInfo.CreateFont(ws).FontFamily);
		}
#endif

		[Test]
		public void CreateFont_Default_GetFontSizeIs12()
		{
			var ws = new WritingSystemDefinition()
			{
				DefaultFont = new FontDefinition("Arial")
			};
			Assert.AreEqual(12, WritingSystemInfo.CreateFont(ws).Size);
		}

		[Test]
		public void CreateFont_WithFontName_NameSetToFontName()
		{
			var ws = new WritingSystemDefinition
						 {
							 DefaultFont = new FontDefinition((FontFamily.GenericSerif.Name))
						 };
			// Assert the precondition
			Assert.AreNotEqual(FontFamily.GenericSansSerif.Name, ws.DefaultFont.Name);
			// Assert the test
			Assert.AreEqual(WritingSystemInfo.CreateFont(ws).Name, ws.DefaultFont.Name);
		}
	}
}