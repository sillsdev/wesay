using System.IO;
using NUnit.Framework;
using WeSay.Foundation.Tests.TestHelpers;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class TaskMemoryRepositoryTests
	{
		[Test]
		public void CreateAndDispose_NoExistingFile_LeavesFile()
		{
			using(var dir = new TemporaryFolder("TaskMemoryRepositoryTests"))
			{
				using(var repo = TaskMemoryRepository.CreateOrLoadTaskMemoryRepository("foo",dir.FolderPath))
				{
					Assert.IsNotNull(repo);
				}
				Assert.IsTrue(File.Exists(dir.Combine("foo"+TaskMemoryRepository.FileExtensionWithDot)));
			}
		}


		[Test]
		public void FindOrCreateSettingsByTaskId_NotFound_CreatesNew()
		{
			using (var dir = new TemporaryFolder("TaskMemoryRepositoryTests"))
			{
				using (var repo = TaskMemoryRepository.CreateOrLoadTaskMemoryRepository("foo", dir.FolderPath))
				{
					Assert.IsNotNull(repo.FindOrCreateSettingsByTaskId("NewGuy"));
				}
			}
		}

		[Test]
		public void FindOrCreateSettingsByTaskId_IsFound_GivesOldMemory()
		{
			using (var dir = new TemporaryFolder("TaskMemoryRepositoryTests"))
			{
				using (var repo = TaskMemoryRepository.CreateOrLoadTaskMemoryRepository("foo", dir.FolderPath))
				{
					repo.FindOrCreateSettingsByTaskId("NewGuy").Set("color", "blue");
				}
				using (var repo = TaskMemoryRepository.CreateOrLoadTaskMemoryRepository("foo", dir.FolderPath))
				{
					Assert.AreEqual( "blue", repo.FindOrCreateSettingsByTaskId("NewGuy").Get("color", "blah"));
				}
			}
		}


		[Test]
		public void Set_NotFound_SetsToValue()
		{
			var memory = new TaskMemory();
			memory.Set("color", "red");
			Assert.AreEqual("red", memory.Get("color", "blue"));
		}

		[Test]
		public void Set_Found_ChangesToValue()
		{
			var memory = new TaskMemory();
			memory.Set("color", "orange");
			memory.Set("color", "red");
			Assert.AreEqual("red", memory.Get("color", "blue"));
		}

		[Test]
		public void Get_NotFound_GivesDefault()
		{
			var memory = new TaskMemory();
			Assert.AreEqual("red", memory.Get("color", "red"));
		}

		[Test]
		public void Get_Found_GivesCorrectValue()
		{
			var memory = new TaskMemory();
			memory.Set("color", "blue");
			Assert.AreEqual("blue", memory.Get("color", "red"));
		}
	}
}
