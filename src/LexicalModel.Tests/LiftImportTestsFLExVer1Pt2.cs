using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LiftImportTestsFLExVer1Pt2 : LiftImportTestsBase
	{
		protected override LiftImporter CreateImporter()
		{
			return new LiftImporterFlexVer1Pt2();
		}

		[Test]
		public void LoadKalabaFromFLEx1pt2()
		{
			IList<LexEntry> entries = new List<LexEntry>();
			string path = Path.Combine(WeSay.Project.WeSayWordsProject.Project.ApplicationTestDirectory, "kalabaFlex1pt2.lift");
			LiftImporter importer = LiftImporter.ReadFile(entries, path, null);
			Assert.IsInstanceOfType(typeof(LiftImporterFlexVer1Pt2), importer);
			Assert.Greater(entries.Count,60);
		}
	}
}