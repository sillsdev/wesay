using System.IO;
using System.Xml;
using Addin.Transform.OpenOffice;
using WeSay.Project;
using WeSay.Project.Tests;
using WeSay.AddinLib;
using ICSharpCode.SharpZipLib.Zip;

using Palaso.TestUtilities;

using NUnit.Framework;

namespace Addin.Transform.Tests
{
	[TestFixture]
	public class OdfTransformerTests
	{
		private OpenOfficeAddin _addin;
		private ProjectDirectorySetupForTesting _testProject;
		private WeSayWordsProject _project;
		private ProjectInfo _projectInfo;
		private bool _succeeded;

		[SetUp]
		public void Setup()
		{
			const string xmlOfEntries = @" <entry id='foo1'>
						<lexical-unit><form lang='qaa'><text>hello</text></form></lexical-unit>
				</entry>";
			_testProject = new ProjectDirectorySetupForTesting(xmlOfEntries);
			_project = _testProject.CreateLoadedProject();
			_projectInfo = _project.GetProjectInfoForAddin();
			_addin = new OpenOfficeAddin();
			_addin.LaunchAfterExport= false;
			_succeeded = false;
		}

		[TearDown]
		public void TearDown()
		{
			if (_succeeded)
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
			_succeeded = (new FileInfo(odtFile).Length > 0);
			Assert.IsTrue(_succeeded);
			string odtContent =Path.Combine(_projectInfo.PathToExportDirectory, "content.xml");
			string odtStyles =Path.Combine(_projectInfo.PathToExportDirectory, "styles.xml");

			var nsManager = new XmlNamespaceManager(new NameTable());
			nsManager.AddNamespace("text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
			nsManager.AddNamespace("style","urn:oasis:names:tc:opendocument:xmlns:style:1.0" );
			AssertThatXmlIn.File(odtContent).HasAtLeastOneMatchForXpath("//text:p", nsManager);
			AssertThatXmlIn.File(odtStyles).HasAtLeastOneMatchForXpath("//style:font-face", nsManager);

			var odtZip = new ZipFile(odtFile);
			ZipEntry manifest = odtZip.GetEntry("META-INF/manifest.xml");
			Assert.IsNotNull(manifest);
			odtZip.Close();
		}
	}
}
