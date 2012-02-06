using System.IO;
using System.Diagnostics;
using NUnit.Framework;
using WeSay.AddinLib;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.TestUtilities;
using Addin.Transform.PdfDictionary;

namespace Addin.Transform.Tests
{
	[TestFixture]
	public class PdfMakerTests
	{
		public PdfMaker _addin;
		private LexEntryRepository _lexEntryRepository;
		private string _filePath;
		private string _liftFilePath;
		private string _pdfPath;

		[SetUp]
		public void Setup()
		{
			WeSayProjectTestHelper.InitializeForTests();
			_filePath = Path.GetTempFileName();
			_liftFilePath = Path.GetTempFileName();
			_lexEntryRepository = new LexEntryRepository(_filePath);

			_addin = new PdfMaker();
			_addin.LaunchAfterTransform = false;
		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(_addin.PathToOutput))
			{
				File.Delete(_addin.PathToOutput);
			}
			_lexEntryRepository.Dispose();
			File.Delete(_filePath);
			File.Delete(_liftFilePath);
		}

		[Test]
		public void CreatePDF()
		{
			LaunchAddin();
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void CreateAndOpen()
		{
			LaunchAddin();
			Process.Start(_pdfPath);
		}


		private void LaunchAddin()
		{
			ProjectInfo projectinfo = WeSayWordsProject.Project.GetProjectInfoForAddin();
			_pdfPath = Path.Combine(projectinfo.PathToExportDirectory,
											  projectinfo.Name + ".pdf");
			_addin.Launch(null, projectinfo);
			Assert.IsTrue(File.Exists(_pdfPath));
			bool succeeded = (new FileInfo(pdfPath).Length > 0);
			Assert.IsTrue(succeeded);
		}
	}
}
