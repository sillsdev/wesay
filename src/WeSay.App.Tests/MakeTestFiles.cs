using System.IO;
using System.Xml;
using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class MakeSampleFiles
	{
		/// <summary>
		/// This is to get us good paths on linux, as well as windows
		/// </summary>
		/// <param name="parts"></param>
		/// <returns></returns>
		private string CombinePathParts(string[] parts)
		{
			string s="";
			foreach (string part in parts)
			{
				s = Path.Combine(s, part);
			}
			return s;
		}

		[Test]
		[Ignore("Run this explicity to setup the sample data. It should not be required for any tests.")]
		public void MakeThai5000SampleFromLift()
		{
			string sourcePath = CombinePathParts(new string[] { "..", "..", "SampleProjects", "Thai",  "thai5000.lift.xml"});
			string destPath = CombinePathParts(new string[] { "..", "..", "SampleProjects", "Thai",  "thai5000.words"});

			if (File.Exists(destPath))
			{
				File.Delete(destPath);
			}

			using (Db4oDataSource ds = new Db4oDataSource(destPath))
			{
				Db4oLexModelHelper.Initialize(ds.Data);
				using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
				{
					entries.WriteCacheSize = 0;//don't commit all the time
					Assert.Fail("this has fallen out of repair. Needs to be hooked up to new merger");
		//            LiftImporter.ReadFile(entries, sourcePath, null);
				}
			}
		}
	}
}
