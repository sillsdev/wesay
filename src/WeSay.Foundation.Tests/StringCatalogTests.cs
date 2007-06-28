using System.IO;
using NUnit.Framework;
using WeSay.Foundation.Tests;
using WeSay.Language;
using WeSay.Project;

namespace WeSay.Language.Tests
{
	[TestFixture]
	public class StringCatalogTests
	{
		private string _poFile = Path.GetTempFileName();

		[SetUp]
		public void Setup()
		{
			BasilProject.InitializeForTests();
			StreamWriter writer = File.CreateText(_poFile);
			writer.Write(TestResources.poStrings);
			writer.Close();
		}

		[TearDown]
		public void TearDown()
		{
			File.Delete(_poFile);
		}

		[Test]
		public void NotTranslated()
		{
			StringCatalog catalog = new StringCatalog(_poFile,null);
			Assert.AreEqual("justid", catalog["justid"]);
		}

		[Test]
		public void NotListedAtAll()
		{
			StringCatalog catalog = new StringCatalog(_poFile,null);
			Assert.AreEqual("notinthere", catalog["notinthere"]);
		}

		[Test]
		public void Normal()
		{
			StringCatalog catalog = new StringCatalog(_poFile,null);
			Assert.AreEqual("deng", catalog["red"]);
		}
	}
}
