using System.IO;
using NUnit.Framework;
using TestUtilities;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingEntryRelationFieldFilterTests
	{
		private LexEntryRepository _lexEntryRepository;
		private TemporaryFolder _tempFolder;
		private string _filePath;

		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_tempFolder = new TemporaryFolder();
			_filePath = _tempFolder.GetTemporaryFile();
			_lexEntryRepository = new LexEntryRepository(_filePath);

			_target = _lexEntryRepository.CreateItem();
			_source = _lexEntryRepository.CreateItem();

			Field relationField = new Field("synonyms",
											"LexEntry",
											new string[] {"vernacular"},
											Field.MultiplicityType.ZeroOr1,
											"RelationToOneEntry");
			_missingRelationFieldFilter = new MissingFieldQuery(relationField);
		}

		[TearDown]
		public void Teardown()
		{
			_lexEntryRepository.Dispose();
			File.Delete(_filePath);
		}

		#endregion

		private MissingFieldQuery _missingRelationFieldFilter;
		private LexEntry _target;
		private LexEntry _source;

		private void AddRelation()
		{
			LexRelationCollection synonyms =
					_source.GetOrCreateProperty<LexRelationCollection>("synonyms");
			LexRelation r = new LexRelation("synonyms", _target.GetOrCreateId(true), _source);
			r.SetTarget(_target);
			synonyms.Relations.Add(r);
		}

		[Test]
		public void FieldHasContents()
		{
			AddRelation();
			Assert.IsFalse(_missingRelationFieldFilter.FilteringPredicate(_source));
		}

		[Test]
		public void LexEntryRelationCollectionExistsButIsEmpty()
		{
			_source.GetOrCreateProperty<LexRelationCollection>("synonyms");
			Assert.IsTrue(_missingRelationFieldFilter.FilteringPredicate(_source));
		}

		[Test]
		public void LexEntryRelationCollectionMissing()
		{
			Assert.IsTrue(_missingRelationFieldFilter.FilteringPredicate(_source));
		}

		[Test]
		public void LexEntryRelationCollectionMissingButSkipFlagged()
		{
			_source.SetFlag("flag_skip_synonyms");
			Assert.IsFalse(_missingRelationFieldFilter.FilteringPredicate(_source));
		}
	}
}