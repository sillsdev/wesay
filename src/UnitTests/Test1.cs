using NUnit.Framework;
using com.db4o;
using System.Diagnostics;
using WeSay.Core;
using System.Xml;
using System.Collections.Generic;

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

  [TestFixture]
  public class DataTest
  {
	[Test]
	public void Main() {
	  using (LexiconModel lexiconModel = new LexiconModel(@"c:\WeSay\src\unittests\thai5000.yap")) {
		Assert.Less(0, lexiconModel.Count);

		LexicalEntry currentLexicalEntry = lexiconModel.CurrentLexicalEntry;
		Assert.IsNotNull(currentLexicalEntry);
		Assert.IsTrue(lexiconModel.Contains(currentLexicalEntry));
		lexiconModel.CurrentLexicalEntry = lexiconModel[2000];
		Assert.AreNotEqual(currentLexicalEntry, lexiconModel.CurrentLexicalEntry);

		lexiconModel.Filtered = true;
		Assert.Less(0, lexiconModel.Count);
		Assert.IsNotNull(lexiconModel.CurrentLexicalEntry);
		Assert.AreEqual(currentLexicalEntry, lexiconModel.CurrentLexicalEntry);
		Assert.AreEqual(329, lexiconModel.Count);
	  }
	}
  }

  [TestFixture]
  public class Test1
  {
	[Test]
	public void Load() {
	  ObjectContainer database = GetBlankDb("thai5000.yap");
	  try {
		XmlDocument document = new XmlDocument();
		document.Load(@"c:\WeSay\src\unittests\thai5000.xml");
		foreach (XmlNode node in document.SelectNodes("test/entry")) {
		  LexicalEntry entry = new LexicalEntry();
		  entry.LoadFromTestXml(node);

		  database.Set(entry);
		}
	  }
	  finally {
		database.Close();
	  }
	}

	[Test]
	public void Read() {
	  ObjectContainer database = GetDb("thai5000.yap");
	  try {
		ProcessMemory.Write("Start of READ");

		com.db4o.query.Query query = database.Query();
		//query.Constrain(typeof(UtteranceHistory));
		//query.Descend("_utteranceKey").Constrain(this.GUID.ToString());
		com.db4o.config.Configuration config = Db4o.Configure();
		//config.ActivationDepth(0);

		ProcessMemory.Write("After open");

		ObjectSet result = query.Execute();
		foreach (LexicalEntry entry in result) {
		}
		ProcessMemory.Write("After Touching all");
	  }
	  finally {
		database.Close();
	  }
	  ProcessMemory.Write("After db closed");
	}

	private ObjectContainer GetDb(string name) {
	  ObjectContainer db;
	  string s = GetDbFilePath(name);
	  db = Db4oFactory.OpenFile(s);

	  Assert.IsNotNull(db);
	  com.db4o.config.Configuration config = Db4o.Configure();
	  //            SetupIndices(config);

	  return db;
	}

	private ObjectContainer GetBlankDb(string name) {
	  ObjectContainer db;
	  string s = GetDbFilePath(name);
	  System.IO.File.Delete(s);
	  db = Db4oFactory.OpenFile(s);

	  Assert.IsNotNull(db);
	  com.db4o.config.Configuration config = Db4o.Configure();
	  //            SetupIndices(config);

	  return db;
	}

	private static string GetDbFilePath(string name) {
	  return @"c:\WeSay\src\unittests\" + name;
	}

	private static void SetupIndices(com.db4o.config.Configuration config) {
	  LexicalEntry dummy = new LexicalEntry();
	  com.db4o.config.ObjectClass oc = config.ObjectClass(dummy);
	  com.db4o.config.ObjectField of = oc.ObjectField("_lexicalForm");
	  of.Indexed(true);
	}
  }
}
