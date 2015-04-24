using NUnit.Framework;
using Palaso.DictionaryServices.Model;
using SIL.Tests.Data;
using Palaso.TestUtilities;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntryRepositoryDeleteAllItemsTransitionTests :
		IRepositoryDeleteAllItemsTransitionTests<LexEntry>
	{
		private string _persistedFilePath;
		private TemporaryFolder _tempFolder;

		[SetUp]
		public override void SetUp()
		{
			_tempFolder = new TemporaryFolder("LexEntryRepositoryDeleteAllItemsTransitionTests");
			_persistedFilePath = _tempFolder.GetTemporaryFile();
			DataMapperUnderTest = new LexEntryRepository(_persistedFilePath);
		}

		[TearDown]
		public override void TearDown()
		{
			DataMapperUnderTest.Dispose();
			_tempFolder.Delete();
		}

		protected override void RepopulateRepositoryFromPersistedData()
		{
			DataMapperUnderTest.Dispose();
			DataMapperUnderTest = new LexEntryRepository(_persistedFilePath);
		}
	}
}