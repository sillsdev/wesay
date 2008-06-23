using System.IO;
using NUnit.Framework;
using WeSay.Project;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexRelationTests
	{
		private string _filePath;
		private LexEntryRepository _lexEntryRepository;

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.InitializeForTests();

			_filePath = Path.GetTempFileName();
			_lexEntryRepository = new LexEntryRepository(_filePath);
		}

		[TearDown]
		public void TearDown()
		{
			_lexEntryRepository.Dispose();
			File.Delete(_filePath);
		}

		[Test]
		public void Construct_TargetIdNull_TargetIdIsEmptyString()
		{
			LexSense sense = new LexSense();
			LexRelationType synonymRelationType =
					new LexRelationType("synonym",
										LexRelationType.Multiplicities.Many,
										LexRelationType.TargetTypes.Sense);

			LexRelation relation = new LexRelation(synonymRelationType.ID, null, sense);
			Assert.AreEqual(null, relation.GetTarget(_lexEntryRepository));
			Assert.AreEqual(string.Empty, relation.Key);
		}

		[Test]
		public void TargetId_SetNull_GetStringEmpty()
		{
			LexSense sense = new LexSense();
			LexRelationType synonymRelationType =
					new LexRelationType("synonym",
										LexRelationType.Multiplicities.Many,
										LexRelationType.TargetTypes.Sense);

			LexRelation relation = new LexRelation(synonymRelationType.ID, "something", sense);
			relation.Key = null;
			Assert.AreEqual(null, relation.GetTarget(_lexEntryRepository));
			Assert.AreEqual(string.Empty, relation.Key);
		}
	}
}