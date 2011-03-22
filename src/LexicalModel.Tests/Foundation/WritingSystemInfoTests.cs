using System.Drawing;
using NUnit.Framework;
using WeSay.LexicalModel.Foundation;

namespace WeSay.LexicalModel.Tests.Foundation
{
	[TestFixture]
	public class WritingSystemInfoTests
	{
		[Test]
		public void Font_SetNull_GetReturnsGenericSansSerif()
		{
			WritingSystem ws = new WritingSystem();
			//ws.SetFont(null);
			Assert.AreEqual(FontFamily.GenericSansSerif, WritingSystemInfo.CreateFont(ws).FontFamily);
		}

		[Test]
		public void Font_SetNull_GetFontSizeIs12()
		{
			WritingSystem ws = new WritingSystem();
			//ws.SetFont(null);
			Assert.AreEqual(12, WritingSystemInfo.CreateFont(ws).Size);
		}

		[Test]
		public void Font_SetNull_GetFontNameIsIdenticalToDefaultFontName()
		{
			WritingSystem ws = new WritingSystem();
			//ws.SetFont(null);
			Assert.AreEqual(WritingSystemInfo.CreateFont(ws).Name, ws.DefaultFontName);
		}
	}
}