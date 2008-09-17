using TestUtilities;
using NUnit.Framework;
using System.IO;

namespace TestUtilities.Tests
{
	[TestFixture]
	public class TemporaryFolderTests
	{
		private TemporaryFolder _temporaryFolder;

		[Test]
		public void Constructor_CreatesTemporarySubDirectory()
		{
			TemporaryFolder temporaryFolder = new TemporaryFolder();
			Assert.IsTrue(Directory.Exists(temporaryFolder.FolderPath));
			temporaryFolder.Delete();
		}

		[Test]
		public void Constructor_Path_CreatesTemporarySubDirectoryAtPath()
		{
			TemporaryFolder temporaryFolder = new TemporaryFolder(Path.GetTempPath());
			Assert.IsTrue(Directory.Exists(temporaryFolder.FolderPath));
			temporaryFolder.Delete();
		}

		[Test]
		public void Constructor_PathDirectoryName_CreatesTemporarySubDirectoryAtPathWithGivenName()
		{
			TemporaryFolder temporaryFolder = new TemporaryFolder(Path.GetTempPath(),"Test");
			Assert.IsTrue(Directory.Exists(temporaryFolder.FolderPath));
			temporaryFolder.Delete();
		}

		[Test]
		public void Constructor_TemporarySubDirectoryAlreadyExistsAndHasFilesInIt_EmptyTheTemporarySubDirectory()
		{
			TemporaryFolder temporaryFolder = new TemporaryFolder(Directory.GetCurrentDirectory(), "NonStandard");
			string pathToFile = Path.Combine(temporaryFolder.FolderPath, Path.GetRandomFileName());
			FileStream file = File.Create(pathToFile);
			file.Close();
			TemporaryFolder temporaryFolderUsingSameDirectory = new TemporaryFolder(Directory.GetCurrentDirectory(), "NonStandard");
			Assert.AreEqual(0, Directory.GetFiles(temporaryFolder.FolderPath).Length);
			temporaryFolder.Delete();
		}

		[Test]
		public void GetTemporaryFile_FileExistsInTemporarySubdirectory()
		{
			TemporaryFolder temporaryFolder = new TemporaryFolder();
			string pathToFile = temporaryFolder.GetTemporaryFile();
			Assert.IsTrue(File.Exists(pathToFile));
			temporaryFolder.Delete();
		}

		[Test]
		public void GetTemporaryFile_Name_FileWithNameExistsInTemporarySubdirectory()
		{
			TemporaryFolder temporaryFolder = new TemporaryFolder();
			string pathToFile = temporaryFolder.GetTemporaryFile("fileName");
			Assert.IsTrue(File.Exists(pathToFile));
			Assert.AreEqual(pathToFile, Path.Combine(temporaryFolder.FolderPath, "fileName"));
			temporaryFolder.Delete();
		}

		[Test]
		public void GetTemporaryFile_CalledTwice_BothFilesFoundInSameTemporarySubdirectory()
		{
			TemporaryFolder temporaryFolder = new TemporaryFolder();
			temporaryFolder.GetTemporaryFile();
			temporaryFolder.GetTemporaryFile();
			Assert.AreEqual(2, Directory.GetFiles(temporaryFolder.FolderPath).Length);
			temporaryFolder.Delete();
		}

		[Test]
		public void Delete_RemovesTemporarySubDirectory()
		{
			TemporaryFolder temporaryFolder = new TemporaryFolder(Directory.GetCurrentDirectory(), "NonStandard");
			temporaryFolder.Delete();
			Assert.IsFalse(Directory.Exists(temporaryFolder.FolderPath));
		}

		[Test]
		public void Delete_FileInDirectory_RemovesTemporaryDirectory()
		{
			TemporaryFolder temporaryFolder = new TemporaryFolder(Directory.GetCurrentDirectory(), "NonStandard");
			string pathToFile = Path.Combine(temporaryFolder.FolderPath, Path.GetRandomFileName());
			FileStream file = File.Create(pathToFile);
			file.Close();
			temporaryFolder.Delete();
			Assert.IsFalse(Directory.Exists(temporaryFolder.FolderPath));
		}

		[Test]
		public void Delete_SubDirectoriesInDirectory_RemovesTemporaryDirectory()
		{
			TemporaryFolder temporaryFolder = new TemporaryFolder(Directory.GetCurrentDirectory(), "NonStandard");
			string pathToSubdirectory = Path.Combine(temporaryFolder.FolderPath, Path.GetRandomFileName());
			Directory.CreateDirectory(pathToSubdirectory);
			temporaryFolder.Delete();
			Assert.IsFalse(Directory.Exists(temporaryFolder.FolderPath));
		}
	}
}
