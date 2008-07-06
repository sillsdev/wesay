using System;
using System.IO;
using NUnit.Framework;
using WeSay.Foundation.Tests;

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
		public void TearDown()
		{
		}

//        private class TempFile : IDisposable
//        {
//            private readonly string fileName;
//
//            public TempFile()
//            {
//                fileName = Path.GetTempFileName();
//            }
//
//            public string FolderPath
//            {
//                get { return fileName; }
//            }
//
//            public void Dispose()
//            {
//                File.Delete(FolderPath);
//            }
//        }

		[Test]
		public void AddNewPath_PathExists_PathAtTopOfList()
		{
			using (TempFile existingFile = new TempFile())
			{
				_mruProjects.AddNewPath(existingFile.FilePath);
				string[] mruPaths = _mruProjects.Paths;
				Assert.AreEqual(1, mruPaths.Length);
				Assert.AreEqual(existingFile.FilePath, mruPaths[0]);
			}
		}

		[Test]
		public void AddNewPath_PathDoesNotExist_ReturnsFalse()
		{
			string nonExistentFileName = Path.GetRandomFileName();
			Assert.AreEqual(false, _mruProjects.AddNewPath(nonExistentFileName));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void AddNewPath_NullPath_Throws()
		{
			_mruProjects.AddNewPath(null);
		}

		[Test]
		public void GetPaths_AddTwoFiles_BothFilesInListInInverseOrder()
		{
			using (TempFile firstFileIn = new TempFile(),
							secondFileIn = new TempFile())
			{
				_mruProjects.AddNewPath(firstFileIn.FilePath);
				_mruProjects.AddNewPath(secondFileIn.FilePath);
				string[] mruProjectsPaths = _mruProjects.Paths;
				foreach (string path in mruProjectsPaths)
				{
					Console.WriteLine(path);
				}
				Assert.AreEqual(2, mruProjectsPaths.Length);
				Assert.AreEqual(secondFileIn.FilePath, mruProjectsPaths[0]);
				Assert.AreEqual(firstFileIn.FilePath, mruProjectsPaths[1]);
			}
		}

		[Test]
		public void SetPaths_Null_ClearsPaths()
		{
			using (TempFile file1 = new TempFile(),
						   file2 = new TempFile())
			{
				_mruProjects.Paths = new string[] {file1.FilePath, file2.FilePath};
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
			using (TempFile file1 = new TempFile(),
						   file2 = new TempFile(),
						   file3 = new TempFile())
			{
				_mruProjects.Paths = new string[] {file1.FilePath, file2.FilePath, file3.FilePath};
				Assert.AreEqual(3, _mruProjects.Paths.Length);
				Assert.AreEqual(file1.FilePath, _mruProjects.Paths[0]);
				Assert.AreEqual(file2.FilePath, _mruProjects.Paths[1]);
				Assert.AreEqual(file3.FilePath, _mruProjects.Paths[2]);
			}
		}

		[Test]
		public void AddNewPath_AddPathThatIsAlreadyInMruPaths_PathIsRemovedFromOldPositionAndMovedToTopPosition()
		{
			using (TempFile file1 = new TempFile(),
						   file2 = new TempFile(),
						   file3= new TempFile())
			{
				_mruProjects.Paths = new string[] {file1.FilePath,
												   file2.FilePath,
												   file3.FilePath};
				_mruProjects.AddNewPath(file2.FilePath);
				string[] mruPaths = _mruProjects.Paths;
				Assert.AreEqual(3, mruPaths.Length);
				Assert.AreEqual(file2.FilePath, mruPaths[0]);
				Assert.AreEqual(file1.FilePath, mruPaths[1]);
				Assert.AreEqual(file3.FilePath, mruPaths[2]);
			}
		}

		[Test]
		public void SetPaths_MultipleInstancesOfSamePath_OnlyMostRecentInstanceIsStored()
		{
			using (TempFile file1 = new TempFile(),
							file2 = new TempFile(),
							file3= new TempFile())
			{
				_mruProjects.Paths = new string[]{ file1.FilePath,
												   file2.FilePath,
												   file1.FilePath,
												   file3.FilePath};
				Assert.AreEqual(3,_mruProjects.Paths.Length);
				Assert.AreEqual(file1.FilePath, _mruProjects.Paths[0]);

			}
		}

	}
}