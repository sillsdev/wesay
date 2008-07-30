using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Data.Tests;

namespace WeSay.LexicalModel.Tests
{
	internal static class LiftFileInitializer
	{
		public static string MakeFile()
		{
			string liftfileName = Path.GetTempFileName();
			File.WriteAllText(liftfileName,
							  @"<?xml version='1.0' encoding='utf-8'?>
				<lift
					version='0.12'
					producer='WeSay 1.0.0.0'>
					<entry
						id='Sonne_c753f6cc-e07c-4bb1-9e3c-013d09629111'
						dateCreated='2008-07-01T06:29:23Z'
						dateModified='2008-07-01T06:29:57Z'
						guid='c753f6cc-e07c-4bb1-9e3c-013d09629111'>
						<lexical-unit>
							<form
								lang='v'>
								<text>Sonne</text>
							</form>
						</lexical-unit>
						<sense
							id='33d60091-ba96-4204-85fe-9d15a24bd5ff'>
							<trait
								name='SemanticDomainDdp4'
								value='1 Universe, creation' />
						</sense>
					</entry>
				</lift>");
			return liftfileName;
		}
	}

	[TestFixture]
	public class LiftRepositoryStateUnitializedTests: IRepositoryStateUnitializedTests<LexEntry>
	{
		private string _persistedFilePath;

		[SetUp]
		public void Setup()
		{
			_persistedFilePath = Path.GetRandomFileName();
			_persistedFilePath = Path.GetFullPath(_persistedFilePath);
			RepositoryUnderTest = new LiftRepository(_persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_persistedFilePath);
		}

		[Test, Ignore("Locking needs to be implemented in LiftRepository!")]
		[ExpectedException(typeof(IOException))]
		public void Constructor_FileIsWriteableAfterRepositoryIsCreated_Throws()
		{
			using (File.OpenWrite(_persistedFilePath))
			{
			}
		}

		[Test]
		[ExpectedException(typeof(IOException))]
		public void Constructor_FileIsNotWriteableWhenRepositoryIsCreated_Throws()
		{
			using (File.OpenWrite(_persistedFilePath))
			{
				using(LiftRepository repository = new LiftRepository(_persistedFilePath))
				{
				}
			}
		}

		[Test]
		public void Constructor_FileDoesNotExist_EmptyLiftFileIsCreated()
		{
			string nonExistentFileToBeCreated = Path.GetTempPath() + Path.GetRandomFileName();
			using (new LiftRepository(nonExistentFileToBeCreated))
			{
				string fileContent = File.ReadAllText(nonExistentFileToBeCreated);
				const string emptyLiftFileContent =
						@"<?xml version=""1.0"" encoding=""utf-8""?>
<lift version=""0.12"" producer=""WeSay 1.0.0.0"" />";
				Assert.AreEqual(emptyLiftFileContent, fileContent);
			}
		}

		[Test]
		public void Constructor_FileIsEmpty_MakeFileAnEmptyLiftFile()
		{
			string emptyFileToBeFilled = Path.GetTempFileName();
			using (new LiftRepository(emptyFileToBeFilled))
			{
				string fileContent = File.ReadAllText(emptyFileToBeFilled);
				const string emptyLiftFileContent =
						@"<?xml version=""1.0"" encoding=""utf-8""?>
<lift version=""0.12"" producer=""WeSay 1.0.0.0"" />";
				Assert.AreEqual(emptyLiftFileContent, fileContent);
			}
		}
	}

	[TestFixture]
	public class LiftRepositoryCreatedFromPersistedData:
			IRepositoryPopulateFromPersistedTests<LexEntry>
	{
		private string _persistedFilePath;

		[SetUp]
		public void Setup()
		{
			_persistedFilePath = LiftFileInitializer.MakeFile();
			RepositoryUnderTest = new LiftRepository(_persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_persistedFilePath);
		}

		[Test]
		protected override void  LastModified_IsSetToMostRecentItemInPersistedDatasLastModifiedTime_v()
		{
			SetState();
			Assert.AreEqual(Item.ModificationTime, RepositoryUnderTest.LastModified);
		}

		[Test]
		protected override void  GetItemMatchingQuery_QueryWithShow_ReturnsAllItemsAndFieldsMatchingQuery_v()       {
			SetState();
			Query query = new Query(typeof (LexEntry)).Show("LexicalForm");
			ResultSet<LexEntry> resultsOfQuery = RepositoryUnderTest.GetItemsMatching(query);
			Assert.AreEqual(1, resultsOfQuery.Count);
			Assert.AreEqual("Sonne", resultsOfQuery[0]["LexicalForm"].ToString());
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			RepositoryUnderTest = new LiftRepository(_persistedFilePath);
		}
	}

	[TestFixture]
	public class LiftRepositoryCreateItemTransitionTests:
			IRepositoryCreateItemTransitionTests<LexEntry>
	{
		private string _persistedFilePath;

		[SetUp]
		public void Setup()
		{
			_persistedFilePath = Path.GetRandomFileName();
			_persistedFilePath = Path.GetFullPath(_persistedFilePath);
			RepositoryUnderTest = new LiftRepository(_persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_persistedFilePath);
		}

		[Test]
		protected override void  GetItemsMatchingQuery_QueryWithShow_ReturnAllItemsMatchingQuery_v()
		{
			Item.LexicalForm["de"] = "Sonne";
			Query query = new Query(typeof (LexEntry)).Show("LexicalForm");
			ResultSet<LexEntry> resultsOfQuery = RepositoryUnderTest.GetItemsMatching(query);
			Assert.AreEqual(1, resultsOfQuery.Count);
			Assert.AreEqual("Sonne", resultsOfQuery[0]["LexicalForm"].ToString());
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			RepositoryUnderTest = new LiftRepository(_persistedFilePath);
		}
	}

	[TestFixture]
	public class LiftRepositoryDeleteItemTransitionTests:
			IRepositoryDeleteItemTransitionTests<LexEntry>
	{
		private string _persistedFilePath;

		[SetUp]
		public void Setup()
		{
			_persistedFilePath = Path.GetRandomFileName();
			_persistedFilePath = Path.GetFullPath(_persistedFilePath);
			RepositoryUnderTest = new LiftRepository(_persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_persistedFilePath);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			RepositoryUnderTest = new LiftRepository(_persistedFilePath);
		}
	}

	[TestFixture]
	public class LiftRepositoryDeleteIdTransitionTests: IRepositoryDeleteIdTransitionTests<LexEntry>
	{
		private string _persistedFilePath;

		[SetUp]
		public void Setup()
		{
			_persistedFilePath = Path.GetRandomFileName();
			_persistedFilePath = Path.GetFullPath(_persistedFilePath);
			RepositoryUnderTest = new LiftRepository(_persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
			File.Delete(_persistedFilePath);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			RepositoryUnderTest = new LiftRepository(_persistedFilePath);
		}
	}

	[TestFixture]
	public class LiftRepositoryDeleteAllItemsTransitionTests:
			IRepositoryDeleteAllItemsTransitionTests<LexEntry>
	{
		private string _persistedFilePath;

		[SetUp]
		public void Setup()
		{
			_persistedFilePath = Path.GetRandomFileName();
			_persistedFilePath = Path.GetFullPath(_persistedFilePath);
			RepositoryUnderTest = new LiftRepository(_persistedFilePath);
		}

		[TearDown]
		public void Teardown()
		{
			RepositoryUnderTest.Dispose();
		}

		protected override void RepopulateRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			RepositoryUnderTest = new LiftRepository(_persistedFilePath);
		}
	}

	[TestFixture]
	public class LiftFileAlreadyLockedTest
	{
		private string _persistedFilePath;
		private FileStream _fileStream;

		[SetUp]
		public void SetUp()
		{
			_persistedFilePath = Path.GetRandomFileName();
			_persistedFilePath = Path.GetFullPath(_persistedFilePath);
			_fileStream = File.OpenWrite(_persistedFilePath);
		}

		[TearDown]
		public void TearDown()
		{
			_fileStream.Close();
			File.Delete(_persistedFilePath);
		}

		[Test]
		[ExpectedException(typeof (IOException))]
		public void LockedFile_Throws()
		{
			Assert.IsTrue(_fileStream.CanWrite);
			LiftRepository liftRepository = new LiftRepository(_persistedFilePath);
		}
	}

	[TestFixture]
	public class LiftFileNotLockedTest
	{
		[Test]
		public void UnlockedLiftFile_ConstructorDoesNotThrow()
		{
			string persistedFilePath = Path.GetRandomFileName();
			persistedFilePath = Path.GetFullPath(persistedFilePath);
			// Confirm that the file is writable.
			FileStream fileStream = File.OpenWrite(persistedFilePath);
			Assert.IsTrue(fileStream.CanWrite);
			// Close it before creating the LiftRepository.
			fileStream.Close();
			// LiftRepository constructor shouldn't throw an IOException.
			using (LiftRepository liftRepository = new LiftRepository(persistedFilePath)) {}
			Assert.IsTrue(true); // Constructor didn't throw.
			File.Delete(persistedFilePath);
		}
	}

	[TestFixture]
	public class LiftMigrationTests
	{
		[Test]
		public void MigrateIfNeeded_LiftIsLockedByProject_LockedAgainAfterMigration()
		{
			Assert.Fail("NYI");
			//CreateLiftFileAndRepositoryForTesting("0.10");
			//LiftPreparer preparer = new LiftPreparer(_liftFilePath);
			//Assert.IsTrue(preparer.MigrateIfNeeded(), "MigrateIfNeeded Failed");
			//Assert.IsTrue(_liftRepository.IsLiftFileLocked);
		}
	}
}