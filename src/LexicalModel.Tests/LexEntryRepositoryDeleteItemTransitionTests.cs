using System;
using NUnit.Framework;
using WeSay.Data.Tests;
using WeSay.Foundation.Tests.TestHelpers;

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
			_tempFolder = new TemporaryFolder();
			_persistedFilePath = _tempFolder.GetTemporaryFile();
			RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
		}

		[TearDown]
		public override void TearDown()
		{
			RepositoryUnderTest.Dispose();
			_tempFolder.Delete();
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public override void SaveItem_ItemDoesNotExist_Throws()
		{
			SetState();
			Item.Senses.Add(new LexSense());    //make Lexentry dirty
			RepositoryUnderTest.SaveItem(Item);
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			RepositoryUnderTest.Dispose();
			RepositoryUnderTest = new LexEntryRepository(_persistedFilePath);
		}
	}
}