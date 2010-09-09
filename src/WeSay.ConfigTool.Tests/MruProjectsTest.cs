using System;
using System.IO;
using NUnit.Framework;

namespace WeSay.ConfigTool.Tests
{
	[TestFixture]
	public class MruProjectsTests
	{
		private MruProjects _mruProjects;

		[SetUp]
		public void Setup()
		{
			_mruProjects = new MruProjects();
		}

		[TearDown]
		public void TearDown() {}

		private class TempFile: IDisposable
		{
			private readonly string fileName;

			public TempFile()
			{
				fileName = Path.GetTempFileName();
			}

			public string FileName
			{
				get { return fileName; }
			}

			public void Dispose()
			{
				File.Delete(FileName);
			}
		}

		[Test]
		public void AddNewPath_PathExists_PathAtTopOfList()
		{
			using (TempFile existingFile = new TempFile())
			{
				_mruProjects.AddNewPath(existingFile.FileName);
				string[] mruPaths = _mruProjects.Paths;
				Assert.AreEqual(1, mruPaths.Length);
				Assert.AreEqual(existingFile.FileName, mruPaths[0]);
			}
		}

		[Test]
		public void AddNewPath_PathDoesNotExist_ReturnsFalse()
		{
			string nonExistentFileName = Path.GetRandomFileName();
			Assert.AreEqual(false, _mruProjects.AddNewPath(nonExistentFileName));
		}

		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof (ArgumentNullException))]
		public void AddNewPath_NullPath_Throws()
		{
			_mruProjects.AddNewPath(null);
		}

		[Test]
		public void GetPaths_AddTwoFiles_BothFilesInListInInverseOrder()
		{
			using (TempFile firstFileIn = new TempFile(), secondFileIn = new TempFile())
			{
				_mruProjects.AddNewPath(firstFileIn.FileName);
				_mruProjects.AddNewPath(secondFileIn.FileName);
				string[] mruProjectsPaths = _mruProjects.Paths;
				foreach (string path in mruProjectsPaths)
				{
					Console.WriteLine(path);
				}
				Assert.AreEqual(2, mruProjectsPaths.Length);
				Assert.AreEqual(secondFileIn.FileName, mruProjectsPaths[0]);
				Assert.AreEqual(firstFileIn.FileName, mruProjectsPaths[1]);
			}
		}

		[Test]
		public void SetPaths_Null_ClearsPaths()
		{
			using (TempFile file1 = new TempFile(), file2 = new TempFile())
			{
				_mruProjects.Paths = new string[] {file1.FileName, file2.FileName};
				_mruProjects.Paths = null;
				Assert.IsNotNull(_mruProjects.Paths);
				foreach (string path in _mruProjects.Paths)
				{
					Console.WriteLine(path);
				}
				//Assert.AreEqual(0, _mruProjects.Paths.Length);
				Assert.IsEmpty(_mruProjects.Paths);
			}
		}

		[Test]
		public void SetPaths_InitializeWithValues_ValuesWereInitialized()
		{
			using (TempFile file1 = new TempFile(), file2 = new TempFile(), file3 = new TempFile())
			{
				_mruProjects.Paths = new string[] {file1.FileName, file2.FileName, file3.FileName};
				Assert.AreEqual(3, _mruProjects.Paths.Length);
				Assert.AreEqual(file1.FileName, _mruProjects.Paths[0]);
				Assert.AreEqual(file2.FileName, _mruProjects.Paths[1]);
				Assert.AreEqual(file3.FileName, _mruProjects.Paths[2]);
			}
		}

		[Test]
		public void
				AddNewPath_AddPathThatIsAlreadyInMruPaths_PathIsRemovedFromOldPositionAndMovedToTopPosition
				()
		{
			using (TempFile file1 = new TempFile(), file2 = new TempFile(), file3 = new TempFile())
			{
				_mruProjects.Paths = new string[] {file1.FileName, file2.FileName, file3.FileName};
				_mruProjects.AddNewPath(file2.FileName);
				string[] mruPaths = _mruProjects.Paths;
				Assert.AreEqual(3, mruPaths.Length);
				Assert.AreEqual(file2.FileName, mruPaths[0]);
				Assert.AreEqual(file1.FileName, mruPaths[1]);
				Assert.AreEqual(file3.FileName, mruPaths[2]);
			}
		}

		[Test]
		public void SetPaths_MultipleInstancesOfSamePath_OnlyMostRecentInstanceIsStored()
		{
			using (TempFile file1 = new TempFile(), file2 = new TempFile(), file3 = new TempFile())
			{
				_mruProjects.Paths = new string[]
										 {
												 file1.FileName, file2.FileName, file1.FileName,
												 file3.FileName
										 };
				Assert.AreEqual(3, _mruProjects.Paths.Length);
				Assert.AreEqual(file1.FileName, _mruProjects.Paths[0]);
			}
		}
	}
}