using System;
using System.IO;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SIL.Reporting;
using SIL.TestUtilities;
using WeSay.Project.LocalizedList;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class LocalizedListParserTests
	{
		private IServiceLocator Context { get; set; }
		[SetUp]
		public void Setup()
		{
			ErrorReport.IsOkToInteractWithUser = false;
			WeSayWordsProject.PreventBackupForTests = true;
		}

		[TearDown]
		public void TearDown()
		{
		}

		[Test]
		public void ParseSemDomXMLFile()
		{
			using (var projectDirectory = new TemporaryFolder())
			{
				//setting up a minimal WeSay project with a config file that contains an id for a nonexistent writing system
				var project = new WeSayWordsProject();
				var localizedList = new LocalizedListParser();
				localizedList.ApplicationCommonDirectory = BasilProject.ApplicationTestDirectory;
				localizedList.PathToWeSaySpecificFilesDirectoryInProject = projectDirectory.Path;
				localizedList.SemanticDomainWs = "en";
				Assert.AreEqual(106, localizedList.ReadListFile());
				Assert.AreEqual(106, localizedList.Keys.Count);
				Assert.AreEqual(106, localizedList.QuestionDictionary.Count);
				Assert.AreEqual(9, localizedList.QuestionDictionary[localizedList.Keys[1]].Count);
				Assert.AreEqual("(1) What words are used to refer to the sky? (sky, firmament, canopy, vault)",
					localizedList.QuestionDictionary[localizedList.Keys[1]][0]);
			}
		}
		[Test]
		public void ParseLocalizedListXMLFile()
		{
			using (var projectDirectory = new TemporaryFolder())
			{
				//setting up a minimal WeSay project with a config file that contains an id for a nonexistent writing system
				var project = new WeSayWordsProject();
				var localizedList = new LocalizedListParser();
				localizedList.ApplicationCommonDirectory = BasilProject.ApplicationTestDirectory;
				localizedList.PathToWeSaySpecificFilesDirectoryInProject = projectDirectory.Path;
				localizedList.SemanticDomainWs = "fr";
				Assert.AreEqual(1792, localizedList.ReadListFile());
				Assert.AreEqual(1792, localizedList.Keys.Count);
				Assert.AreEqual(1792, localizedList.QuestionDictionary.Count);
				Assert.AreEqual(9, localizedList.QuestionDictionary[localizedList.Keys[1]].Count);
				Assert.AreEqual("(1) Quels sont les mots ou expressions qui font référence au ciel ? (ciel, firmament, céleste, voûte céleste, l’azur (poétique))",
					localizedList.QuestionDictionary[localizedList.Keys[1]][0]);
			}
		}
		[Test]
		public void ReadListFile_NonExistantSemanticDomainFile_Throws()
		{
			using (var projectDirectory = new TemporaryFolder())
			{
				if (File.Exists(Path.Combine(BasilProject.GetPretendProjectDirectory(), "SemDom.xml")))
				{
					File.Delete(Path.Combine(BasilProject.GetPretendProjectDirectory(), "SemDom.xml"));
				}
				//setting up a minimal WeSay project with a config file that contains an id for a nonexistent writing system
				var project = new WeSayWordsProject();
				var localizedList = new LocalizedListParser();
				localizedList.ApplicationCommonDirectory = BasilProject.GetPretendProjectDirectory();
				localizedList.PathToWeSaySpecificFilesDirectoryInProject = projectDirectory.Path;
				localizedList.SemanticDomainWs = "en";
				Assert.Throws<ApplicationException>(() => localizedList.ReadListFile());
			}
		}
	}
}
