
using System;
using System.IO;
using System.Xml;
using Addin.Transform.OpenOffice;
using WeSay.Project;
using WeSay.Project.Tests;
using WeSay.AddinLib;
using WeSay.LexicalModel;
using LiftIO.Validation;
using ICSharpCode.SharpZipLib.Zip;

using Palaso.TestUtilities;

using NUnit.Framework;

namespace Addin.Transform.Tests
{
	[TestFixture]
	public class OdfTransformerTests
	{
		public OpenOfficeAddin _addin;
		private ProjectDirectorySetupForTesting _testProject;
		private WeSayWordsProject _project;
		private ProjectInfo _projectInfo;
		private bool succeeded = false;

		[SetUp]
		public void Setup()
		{
			string xmlOfEntries = @" <entry id='foo1'>
						<lexical-unit><form lang='v'><text>hello</text></form></lexical-unit>
				</entry>";
			_testProject = new WeSay.Project.Tests.ProjectDirectorySetupForTesting(xmlOfEntries);
			_project = _testProject.CreateLoadedProject();
			_projectInfo = _project.GetProjectInfoForAddin();
			_addin = new OpenOfficeAddin();
			_addin.LaunchAfterExport= false;
		}

		[TearDown]
		public void TearDown()
		{
			if (succeeded)
			{
				_project.Dispose();
				_testProject.Dispose();
			}
		}

		[Test]
		public void TestOpenDocumentExport()
		{
			_addin.Launch(null,  _projectInfo);
			string odtFile =Path.Combine(_projectInfo.PathToExportDirectory, _projectInfo.Name + ".odt");
			Assert.IsTrue(File.Exists(odtFile));
			succeeded = (new FileInfo(odtFile).Length > 0);
			Assert.IsTrue(succeeded);
			string odtContent =Path.Combine(_projectInfo.PathToExportDirectory, "content.xml");
			string odtStyles =Path.Combine(_projectInfo.PathToExportDirectory, "styles.xml");

			XmlNamespaceManager nsManager = new XmlNamespaceManager(new NameTable());
			nsManager.AddNamespace("text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
			nsManager.AddNamespace("style","urn:oasis:names:tc:opendocument:xmlns:style:1.0" );
			AssertThatXmlIn.File(odtContent).HasAtLeastOneMatchForXpath("//text:p", nsManager);
			AssertThatXmlIn.File(odtStyles).HasAtLeastOneMatchForXpath("//style:font-face", nsManager);

			ZipFile odtZip = new ZipFile(odtFile);
			ZipEntry manifest = odtZip.GetEntry("META-INF/manifest.xml");
			Assert.IsNotNull(manifest);
			odtZip.Close();
		}
	}
}
