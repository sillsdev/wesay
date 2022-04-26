using NUnit.Framework;
using SIL.DictionaryServices.Model;
using SIL.Tests.Data;
using SIL.TestUtilities;
using System.Collections.Generic;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntryRepositoryCreateItemTransitionTests :
		IRepositoryCreateItemTransitionTests<LexEntry>
	{
		private TemporaryFolder _tempFolder;
		private string _persistedFilePath;

		public LexEntryRepositoryCreateItemTransitionTests()
		{
			_hasPersistOnCreate = false;
		}

		[SetUp]
		public override void SetUp()
		{
			_tempFolder = new TemporaryFolder(GetType().Name);
			_persistedFilePath = _tempFolder.GetPathForNewTempFile(false);
			DataMapperUnderTest = new LexEntryRepository(_persistedFilePath);
		}

		[TearDown]
		public override void TearDown()
		{
			DataMapperUnderTest.Dispose();
			_tempFolder.Dispose();
		}

		[Test]
		public void SaveItem_LexEntryIsDirtyIsFalse()
		{
			SetState();
			DataMapperUnderTest.SaveItem(Item);
			Assert.IsFalse(Item.IsDirty);
		}

		[Test]
		public void SaveItems_LexEntryIsDirtyIsFalse()
		{
			SetState();
			var itemsToBeSaved = new List<LexEntry> {Item};
			DataMapperUnderTest.SaveItems(itemsToBeSaved);
			Assert.IsFalse(Item.IsDirty);
		}

		[Test]
		public void Constructor_LexEntryIsDirtyIsTrue()
		{
			SetState();
			Assert.IsTrue(Item.IsDirty);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			DataMapperUnderTest.Dispose();
			DataMapperUnderTest = new LexEntryRepository(_persistedFilePath);
		}
	}
}