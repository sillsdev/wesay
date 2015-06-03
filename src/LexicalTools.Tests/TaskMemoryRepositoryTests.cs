using System.IO;
using NUnit.Framework;
using SIL.TestUtilities;


namespace WeSay.LexicalTools.Tests
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
		public void PersistThenGet_TwoTasksWithSameKey_GivesCorrectValue()
		{
			using (var dir = new TemporaryFolder("TaskMemoryRepositoryTests"))
			{
				using (var repo = TaskMemoryRepository.CreateOrLoadTaskMemoryRepository("foo", dir.FolderPath))
				{
					repo.FindOrCreateSettingsByTaskId("one").Set("common", "forOne");
					repo.FindOrCreateSettingsByTaskId("two").Set("common", "forTwo");
					Assert.AreEqual("forOne", repo.FindOrCreateSettingsByTaskId("one").Get("common", "blah"));
					Assert.AreEqual("forTwo", repo.FindOrCreateSettingsByTaskId("two").Get("common", "blah"));
				}
				//now reopen and verify
				using (var repo = TaskMemoryRepository.CreateOrLoadTaskMemoryRepository("foo", dir.FolderPath))
				{
					Assert.AreEqual("forOne", repo.FindOrCreateSettingsByTaskId("one").Get("common", "blah"));
					Assert.AreEqual("forTwo", repo.FindOrCreateSettingsByTaskId("two").Get("common", "blah"));
				}
			}
		}

		[Test]
		public void FindOrCreateSettingsByTaskId_FileCorrupt_CreatesNew()
		{
			using (var dir = new TemporaryFolder("TaskMemoryRepositoryTests"))
			{
				File.WriteAllText(dir.Combine("foo" + TaskMemoryRepository.FileExtensionWithDot), "I am corrupt");
				using (var seeIfWeCanLoad = TaskMemoryRepository.CreateOrLoadTaskMemoryRepository("foo", dir.FolderPath))
				{
				}
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


		[Test]
		public void GetInt_NotReallyAnInteger_GivesDefaultValue()
		{
			var memory = new TaskMemory();
			memory.Set("number", "blue");
			Assert.AreEqual(3, memory.Get("number", 3));
		}

	}
}