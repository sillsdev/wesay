using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Palaso.TestUtilities;
using WeSay.Project.ConfigMigration.WritingSystem;

namespace WeSay.Project.Tests.ConfigMigration.WritingSystem
{
	[TestFixture]
	public class WritingSystemsMigratorTests
	{
		private class TestEnvironment : IDisposable
		{
			private TemporaryFolder _folder;

			public TestEnvironment()
			{
				_folder = new TemporaryFolder("WritingSystemsMigratorTests");
			}

			public string ProjectPath
			{
				get { return _folder.Path; }
			}

			public void Dispose()
			{
				_folder.Dispose();
			}

			public string WritingSystemsPath
			{
				get { return Path.Combine(ProjectPath, "WritingSystems"); }
			}

			public string WritingSystemsOldPrefsFilePath
			{
				get { return Path.Combine(ProjectPath, "WritingSystemPrefs.xml"); }
			}

			public string WritingSystemFilePath(string tag)
			{
				return Path.Combine(WritingSystemsPath, String.Format("{0}.ldml", tag));
			}

			public void WriteToPrefsFile(string language)
			{
				File.WriteAllText(WritingSystemsOldPrefsFilePath, WritingSystemPrefsFileContent.SingleWritingSystemForLanguage(language));
			}
		}

		[Test]
		public void MigrateIfNeeded_HasPrefsFile_LdmlLastestVersion()
		{
			using (var e = new TestEnvironment())
			{
				e.WriteToPrefsFile("qaa-x-test");
				var migrator = new WritingSystemsMigrator(e.ProjectPath);
				migrator.MigrateIfNeeded();

				AssertThatXmlIn.File(e.WritingSystemFilePath("qaa-x-test")).HasAtLeastOneMatchForXpath("/blah");

			}
		}
	}
}
