using System.IO;
using NUnit.Framework;
using WeSay.LexicalModel;

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
			string sourcePath = CombinePathParts(new string[] { "..", "..", "SampleProjects", "Thai", "WeSay", "thai5000.lift.xml"});
			string destPath = CombinePathParts(new string[] { "..", "..", "SampleProjects", "Thai", "WeSay", "thai5000.words"});

			if (System.IO.File.Exists(destPath))
			{
				System.IO.File.Delete(destPath);
			}

			using (Db4oDataSource ds = new WeSay.Data.Db4oDataSource(destPath))
			{
				using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
				{
					LiftImporter importer = new LiftImporter(entries);
					importer.ReadFile(sourcePath);
				}
			}
		}
	}
}
