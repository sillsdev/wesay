using System;
using System.Collections.Generic;
using NUnit.Framework;
using Palaso.Tests.Data;
using Palaso.TestUtilities;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntryRepositoryCreatedFromPersistedData :
		IRepositoryPopulateFromPersistedTests<LexEntry>
	{
		private TemporaryFolder _tempFolder;
		private string _persistedFilePath;

		[SetUp]
		public override void SetUp()
		{
			_tempFolder = new TemporaryFolder("LexEntryRepositoryCreatedFromPersistedData");
			_persistedFilePath = LiftFileInitializer.MakeFile(_tempFolder.GetTemporaryFile());
			DataMapperUnderTest = new LexEntryRepository(_persistedFilePath);
		}

		[TearDown]
		public override void TearDown()
		{
			DataMapperUnderTest.Dispose();
			_tempFolder.Delete();
		}

		[Test]
		public override void SaveItem_LastModifiedIsChangedToLaterTime()
		{
			SetState();
			DateTime modifiedTimePreSave = DataMapperUnderTest.LastModified;
			MakeItemDirty(Item);
			DataMapperUnderTest.SaveItem(Item);
			Assert.Greater(DataMapperUnderTest.LastModified, modifiedTimePreSave);
		}

		[Test]
		public override void SaveItems_LastModifiedIsChangedToLaterTime()
		{
			SetState();
			List<LexEntry> itemsToSave = new List<LexEntry>();
			itemsToSave.Add(Item);
			DateTime modifiedTimePreSave = DataMapperUnderTest.LastModified;
			MakeItemDirty(Item);
			DataMapperUnderTest.SaveItems(itemsToSave);
			Assert.Greater(DataMapperUnderTest.LastModified, modifiedTimePreSave);
		}

		private static void MakeItemDirty(LexEntry Item)
		{
			Item.LexicalForm["de"] = "Sonne";
		}

		protected override void  LastModified_IsSetToMostRecentItemInPersistedDatasLastModifiedTime_v()
		{
			SetState();
			Assert.AreEqual(Item.ModificationTime, DataMapperUnderTest.LastModified);
		}

		[Test]
		public void Constructor_LexEntryIsDirtyIsFalse()
		{
			SetState();
			Assert.IsFalse(Item.IsDirty);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			DataMapperUnderTest.Dispose();
			DataMapperUnderTest = new LexEntryRepository(_persistedFilePath);
		}
	}
}