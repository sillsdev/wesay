using System.Collections.Generic;
using NUnit.Framework;
using Palaso.Data;
using WeSay.Data;
using WeSay.Data.Tests;
using Palaso.TestUtilities;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntryRepositoryCreateItemTransitionTests :
		IRepositoryCreateItemTransitionTests<LexEntry>
	{
		private TemporaryFolder _tempFolder;
		private string _persistedFilePath;

		[SetUp]
		public override void SetUp()
		{
			_tempFolder = new TemporaryFolder();
			_persistedFilePath = _tempFolder.GetTemporaryFile();
			DataMapperUnderTest = new LexEntryRepository(_persistedFilePath);
		}

		[TearDown]
		public override void TearDown()
		{
			DataMapperUnderTest.Dispose();
			_tempFolder.Delete();
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
			List<LexEntry> itemsToBeSaved = new List<LexEntry>();
			itemsToBeSaved.Add(Item);
			DataMapperUnderTest.SaveItems(itemsToBeSaved);
			Assert.IsFalse(Item.IsDirty);
		}

		[Test]
		public void Constructor_LexEntryIsDirtyIsTrue()
		{
			SetState();
			Assert.IsTrue(Item.IsDirty);
		}

		protected override void  GetItemsMatchingQuery_QueryWithShow_ReturnAllItemsMatchingQuery_v()
		{
			Item.LexicalForm["de"] = "Sonne";
			QueryAdapter<LexEntry> query = new QueryAdapter<LexEntry>();
			query.Show("LexicalForm");
			ResultSet<LexEntry> resultsOfQuery = DataMapperUnderTest.GetItemsMatching(query);
			Assert.AreEqual(1, resultsOfQuery.Count);
			Assert.AreEqual("Sonne", resultsOfQuery[0]["LexicalForm"].ToString());
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			DataMapperUnderTest.Dispose();
			DataMapperUnderTest = new LexEntryRepository(_persistedFilePath);
		}
	}
}