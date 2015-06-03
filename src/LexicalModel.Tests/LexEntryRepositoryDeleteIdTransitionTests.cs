using System;
using NUnit.Framework;
using SIL.DictionaryServices.Model;
using Palaso.TestUtilities;
using SIL.IO;
using SIL.Tests.Data;

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
		private TempFile _persistedFilePath;
		private TemporaryFolder _tempFolder;

		[SetUp]
		public override void SetUp()
		{
			_tempFolder = new TemporaryFolder("LexEntryRepositoryDeleteIdTransitionTests");
			_persistedFilePath = _tempFolder.GetNewTempFile(false);
			DataMapperUnderTest = new LexEntryRepository(_persistedFilePath.Path);
		}

		[TearDown]
		public override void TearDown()
		{
			DataMapperUnderTest.Dispose();
			_tempFolder.Delete();
		}

		[Test]
		public override void SaveItem_ItemDoesNotExist_Throws()
		{
			SetState();
			Item.Senses.Add(new LexSense());
			Assert.Throws<ArgumentOutOfRangeException>(() =>DataMapperUnderTest.SaveItem(Item));
		}

		protected override void CreateNewRepositoryFromPersistedData()
		{
			DataMapperUnderTest.Dispose();
			DataMapperUnderTest = new LexEntryRepository(_persistedFilePath.Path);
		}
	}
}