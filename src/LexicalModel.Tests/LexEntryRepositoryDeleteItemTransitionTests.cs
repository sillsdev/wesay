using NUnit.Framework;
using SIL.DictionaryServices.Model;
using SIL.IO;
using SIL.Tests.Data;
using SIL.TestUtilities;
using System;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntryRepositoryDeleteItemTransitionTests :
		IRepositoryDeleteItemTransitionTests<LexEntry>
	{
		private string _persistedFilePath;
		private TemporaryFolder _tempFolder;


		[SetUp]
		public override void SetUp()
		{
			_tempFolder = new TemporaryFolder("LexEntryRepositoryDeleteItemTransitionTests");
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
		public override void SaveItem_ItemDoesNotExist_Throws()
		{
			SetState();
			Item.Senses.Add(new LexSense());    //make Lexentry dirty
			Assert.Throws<ArgumentOutOfRangeException>(() => DataMapperUnderTest.SaveItem(Item));
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			DataMapperUnderTest.Dispose();
			DataMapperUnderTest = new LexEntryRepository(_persistedFilePath);
		}
	}
}