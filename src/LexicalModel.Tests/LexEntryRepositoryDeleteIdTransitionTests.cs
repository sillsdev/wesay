using NUnit.Framework;
using SIL.DictionaryServices.Model;
using SIL.IO;
using SIL.Tests.Data;
using SIL.TestUtilities;
using System;

namespace WeSay.LexicalModel.Tests
{
	//"FailsDueToSomeTeamCityProblemWhenInvokeFromWeSayTest" is about the inherited tests
	//which check for an ArgumentOutOfRangeException, which started failing on TeamCity
	//only, when we upgraded to nunit 2.5.  These problems might hopefully go away when we move
	//to a newer TeamCity.
	[TestFixture, Category("FailsDueToSomeTeamCityProblemWhenInvokeFromWeSayTest")]
	public class LexEntryRepositoryDeleteIdTransitionTests :
		IRepositoryDeleteIdTransitionTests<LexEntry>
	{
		private string _persistedFilePath;
		private TemporaryFolder _tempFolder;

		[SetUp]
		public override void SetUp()
		{
			_tempFolder = new TemporaryFolder("LexEntryRepositoryDeleteIdTransitionTests");
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
			Item.Senses.Add(new LexSense());
			Assert.Throws<ArgumentOutOfRangeException>(() => DataMapperUnderTest.SaveItem(Item));
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			DataMapperUnderTest.Dispose();
			DataMapperUnderTest = new LexEntryRepository(_persistedFilePath);
		}
	}
}