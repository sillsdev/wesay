
using System;
using System.IO;
using System.Xml;
using Palaso.Reporting;
using Palaso.TestUtilities;
using Addin.Transform.OpenOffice;
using WeSay.Project;
using WeSay.Project.Tests;
using WeSay.AddinLib;
using WeSay.LexicalModel;
using LiftIO.Validation;
using ICSharpCode.SharpZipLib.Zip;

using NUnit.Framework;

namespace Addin.Transform.Tests
{
	[TestFixture]
	public class OdfTransformerTests
	{
		private class EnvironmentForTest : IDisposable
		{
			private ProjectDirectorySetupForTesting _testProject;
			private WeSayWordsProject _project;
			private ProjectInfo _projectInfo;

			public EnvironmentForTest()
			{
				ErrorReport.IsOkToInteractWithUser = false;
				string xmlOfEntries = @" <entry id='foo1'>
							<lexical-unit><form lang='v'><text>hello</text></form></lexical-unit>
					</entry>";
				_testProject = new WeSay.Project.Tests.ProjectDirectorySetupForTesting(xmlOfEntries);
				_project = _testProject.CreateLoadedProject();
				_projectInfo = _project.GetProjectInfoForAddin();

				string sourceTemplateDir = Path.Combine(_projectInfo.PathToApplicationRootDirectory, String.Format("..{0}..{0}templates", Path.DirectorySeparatorChar));
				TestUtilities.DeleteFolderThatMayBeInUse(OutputTemplateDir);
				CopyFolder(sourceTemplateDir, OutputTemplateDir);
			}

			public ProjectInfo ProjectInfo
			{
				get { return _projectInfo; }
			}

			private string OutputTemplateDir
			{
				get { return Path.Combine(_projectInfo.PathToApplicationRootDirectory, "templates"); }
			}

			public string OdtFile
			{
				get { return Path.Combine(_projectInfo.PathToExportDirectory, _projectInfo.Name + ".odt"); }
			}

			public string OdtContent
			{
				get { return Path.Combine(_projectInfo.PathToExportDirectory, "content.xml"); }
			}

			public string OdtStyles
			{
				get { return Path.Combine(_projectInfo.PathToExportDirectory, "styles.xml"); }
			}

			public void Dispose ()
			{
				_project.Dispose();
				_testProject.Dispose();
				TestUtilities.DeleteFolderThatMayBeInUse(OutputTemplateDir);
			}

			private static void CopyFolder(string sourceFolder, string destFolder)
			{
				if (!Directory.Exists( destFolder ))
					Directory.CreateDirectory( destFolder );
				string[] files = Directory.GetFiles( sourceFolder );
				foreach (string file in files)
				{
					string name = Path.GetFileName( file );
					string dest = Path.Combine( destFolder, name );
					File.Copy( file, dest );
				}
				string[] folders = Directory.GetDirectories( sourceFolder );
				foreach (string folder in folders)
				{
					string name = Path.GetFileName( folder );
					string dest = Path.Combine( destFolder, name );
					CopyFolder( folder, dest );
				}
			}
		}



		[Test]
		public void TestOpenDocumentExport()
		{
			using (var e = new EnvironmentForTest())
			{
				var addin = new OpenOfficeAddin();
				addin.LaunchAfterExport= false;

				addin.Launch(null,  e.ProjectInfo);
				Assert.IsTrue(File.Exists(e.OdtFile));
				bool succeeded = (new FileInfo(e.OdtFile).Length > 0);
				Assert.IsTrue(succeeded);

				XmlNamespaceManager nsManager = new XmlNamespaceManager(new NameTable());
				nsManager.AddNamespace("text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
				nsManager.AddNamespace("style","urn:oasis:names:tc:opendocument:xmlns:style:1.0" );
				AssertThatXmlIn.File(e.OdtContent).HasAtLeastOneMatchForXpath("//text:p", nsManager);
				AssertThatXmlIn.File(e.OdtStyles).HasAtLeastOneMatchForXpath("//style:font-face", nsManager);

				ZipFile odtZip = new ZipFile(e.OdtFile);
				ZipEntry manifest = odtZip.GetEntry("META-INF/manifest.xml");
				Assert.IsNotNull(manifest);
				odtZip.Close();
			}
		}
	}
}
