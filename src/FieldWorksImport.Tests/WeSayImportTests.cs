using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Cellar;
using SIL.FieldWorks.FDO.Cellar.Generated;
using SIL.FieldWorks.FDO.Ling;
using SIL.FieldWorks.FDO.Ling.Generated;
using SIL.FieldWorks.Common.COMInterfaces;
using NUnit.Framework;

namespace WeSay.FieldWorks
{
	/// <summary>
	/// Test WeSay-format import
	/// </summary>
	[TestFixture]
	public class WeSayImportTests // merge code doesn't work with in-mem cache : InMemoryFdoTestBase
	{
		protected FdoCache m_cache;


		[Test]
		public void LoadWeSay()
		{
			Importer importer = new Importer(m_cache);
			int count = m_cache.LanguageProject.LexicalDatabaseOA.EntriesOC.Count;
			importer.ImportWeSayFile(GetSampleFilePath("WeSaySample1.xml"));
			Assert.AreEqual(1+count, m_cache.LanguageProject.LexicalDatabaseOA.EntriesOC.Count);
			importer.ImportWeSayFile(GetSampleFilePath("WeSaySample1.xml"));
			Assert.AreEqual(1+count, m_cache.LanguageProject.LexicalDatabaseOA.EntriesOC.Count, "should have merged");
	   }

	   [NUnit.Framework.TearDown]
	   public void TestCleanUp()
	   {
		   if (m_cache != null)
		   {
			   FwKernelLib.UndoResult ures = 0;
			   while (m_cache.CanUndo)
			   {
				   m_cache.Undo(out ures);
				   if (ures == FwKernelLib.UndoResult.kuresFailed || ures == FwKernelLib.UndoResult.kuresError)
					   Assert.Fail("ures should not be == " + ures.ToString());
			   }
		   }

		   if (m_cache != null)
		   {
			   m_cache.Dispose();
			   m_cache = null;
		   }
	   }

	   [SetUp]
	   public void Setup()
	   {
		   System.Collections.Hashtable htCacheOptions = new System.Collections.Hashtable();
		   htCacheOptions.Add("db", "TestLangProj");
		   m_cache = FdoCache.Create(htCacheOptions);

	   }


	   protected string GetSampleFilePath(string file)
	   {
		   string dir = @"C:\WeSay\src\FieldWorksImport.Tests\TestData";
		   return Path.Combine(dir, file);
	   }
   }
}
