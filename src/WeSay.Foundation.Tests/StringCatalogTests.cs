using System;
using System.Drawing;
using System.IO;
using NUnit.Framework;
using WeSay.Foundation;
using WeSay.Foundation.Tests;
using WeSay.Project;

namespace WeSay.Foundation.Tests
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
			StringCatalog catalog = new StringCatalog(_poFile, null, 9);
			Assert.AreEqual("justid", catalog["justid"]);
		}

		[Test]
		public void NotListedAtAll()
		{
			StringCatalog catalog = new StringCatalog(_poFile, null, 9);
			Assert.AreEqual("notinthere", catalog["notinthere"]);
		}

		[Test]
		public void Normal()
		{
			StringCatalog catalog = new StringCatalog(_poFile,null, 9);
			Assert.AreEqual("deng", catalog["red"]);
		}

		[Test]
		public void FontsScaleUp()
		{
			StringCatalog catalog = new StringCatalog(_poFile, "Onyx", 30);
			Font normal = new Font(System.Drawing.FontFamily.GenericSerif, 20);
			Font localized = StringCatalog.ModifyFontForLocalization(normal);
			Assert.AreEqual(41,Math.Floor(localized.SizeInPoints));
		}
		[Test]
		public void FontsChanged()
		{
			StringCatalog catalog = new StringCatalog(_poFile, "Arial", 30);
			Font normal = new Font(System.Drawing.FontFamily.GenericSerif, 20);
			Font localized = StringCatalog.ModifyFontForLocalization(normal);
			Assert.AreEqual("Arial", localized.FontFamily.Name);
		}
	}
}
