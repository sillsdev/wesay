using System.Globalization;
using System.IO;
using System.Xml;
using NUnit.Framework;
using WeSay.Language;
using System.Collections;

namespace WeSay.Language.Tests
{
	[TestFixture]
	public class WritingSystemTests
	{

	   [SetUp]
		public void Setup()
		{
		}


		[Test]
		public void NoSetupDefaultFont()
		{
			WritingSystem ws = new WritingSystem("xx", new System.Drawing.Font("Times", 33));
			Assert.AreEqual(33, ws.Font.Size);
		}




		//NB: there are related tests on the basilproject
	}
}
