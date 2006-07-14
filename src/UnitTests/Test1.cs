using NUnit.Framework;
using com.db4o;
using System.Diagnostics;
using System.IO;
using WeSay.Core;
using System.Xml;
using System.Collections.Generic;
using WeSay.LexicalModel;

namespace WeSay.UnitTests
{
  using System.Diagnostics;
  public class ProcessMemory
  {
	public static long WorkingSet {
	  get {
		return Process.GetCurrentProcess().WorkingSet64;
	  }
	}
	public static void Write(string s) {
	  Debug.WriteLine(ProcessMemory.WorkingSet + " " + s);
	}
  }
	public class TestBase
	{
		protected static string GetDbFilePath(string name)
		{
			return @"c:\WeSay\src\unittests\TestData\" + name;
		}
	}

	[TestFixture]
	public class Test1 : TestBase
	{
		protected string _outputDir = @"c:\WeSay\output\tests\";

		[SetUp]
		public void Setup()
		{
			System.IO.Directory.CreateDirectory(_outputDir );
		}

		[Test]
		public void LoadThai5000FromTextXML()
		{
			ObjectContainer database = GetBlankDb("thai5000.yap");
			try
			{
				XmlDocument document = new XmlDocument();
				document.Load(@"c:\WeSay\src\unittests\TestData\thai5000.xml");
				foreach (XmlNode node in document.SelectNodes("test/entry"))
				{
					LexicalEntry entry = new LexicalEntry();
					entry.LoadFromTestXml(node);

					database.Set(entry);
				}

			}
			finally
			{
				database.Close();
			}
		}

		[Test]
		public void ImportWeSayXML()
		{
			ObjectContainer database = GetBlankDb("pwo.yap");
			try
			{
				XmlDocument document = new XmlDocument();
				document.Load(@"c:\WeSay\src\unittests\TestData\pwo.xws");
				foreach (XmlNode node in document.SelectNodes("lexicon/entry"))
				{
					LexicalEntry entry = WeSayLexicalImporter.LoadFromWeSayXml(node);
					database.Set(entry);
				}
			}
			finally
			{
				database.Close();
			}
		}

		[Test]
		public void Read()
		{
			ObjectContainer database = GetDb("thai5000.yap");
			try
			{
				ProcessMemory.Write("Start of READ");

				com.db4o.query.Query query = database.Query();
				//query.Constrain(typeof(UtteranceHistory));
				//query.Descend("_utteranceKey").Constrain(this.GUID.ToString());
				com.db4o.config.Configuration config = Db4o.Configure();
				//config.ActivationDepth(0);

				ProcessMemory.Write("After open");

				ObjectSet result = query.Execute();
				foreach (LexicalEntry entry in result)
				{
				}
				ProcessMemory.Write("After Touching all");
			}
			finally
			{
				database.Close();
			}
			ProcessMemory.Write("After db closed");
		}

		private ObjectContainer GetDb(string name)
		{
			ObjectContainer db;
			string s = GetDbFilePath(name);
			db = Db4oFactory.OpenFile(s);

			Assert.IsNotNull(db);
			com.db4o.config.Configuration config = Db4o.Configure();
			//            SetupIndices(config);

			return db;
		}

		private ObjectContainer GetBlankDb(string name)
		{
			ObjectContainer db;
			string s = GetDbFilePath(name);
			System.IO.File.Delete(s);
			db = Db4oFactory.OpenFile(s);

			Assert.IsNotNull(db);
			com.db4o.config.Configuration config = Db4o.Configure();
			//            SetupIndices(config);

			return db;
		}



		private static void SetupIndices(com.db4o.config.Configuration config)
		{
			LexicalEntry dummy = new LexicalEntry();
			com.db4o.config.ObjectClass oc = config.ObjectClass(dummy);
			com.db4o.config.ObjectField of = oc.ObjectField("_lexicalForm");
			of.Indexed(true);
		}

		[Test]
		public void ExportThai5000()
		{
			using (LexiconModel model = new LexiconModel(GetDbFilePath("thai5000.yap")))
			{
				WeSayExporter exporter = new WeSayExporter(model);
				string path = _outputDir+"wordsExport.xml";
				if(File.Exists(path))
					System.IO.File.Delete(path);
				exporter.Export(path);
				Assert.IsTrue(File.Exists(path),"Could not find the file that was supposed to be exported.");
				XmlDocument document = new XmlDocument();
				document.Load(path);
				Assert.AreEqual(model.Count, document.SelectNodes("lexicon/entry").Count, "The resulting file had the wrong number of words in it.");

				path = _outputDir+"wordsExport.zip";
			   System.IO.File.Delete(path);
			   exporter.ExportToZip(path);
			   Assert.IsTrue(File.Exists(path),"Could not find the zip file that was supposed to be exported.");
			}
		}

		[Test]
		public void Filter()
		{
			using (LexiconModel lexiconModel = new LexiconModel(GetDbFilePath("thai5000.yap")))
			{

				Assert.Less(0, lexiconModel.Count);

				lexiconModel.Filtered = true;
				Assert.Less(0, lexiconModel.Count);
				Assert.AreEqual(329, lexiconModel.Count);
			}
		}
	}
}
