using System;
using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	public class HelperForTestsRequiringDb4o: IDisposable
	{
		private string _filePath;
		private Db4oRecordListManager _manager;
		public  HelperForTestsRequiringDb4o()
		{
			_filePath = Path.GetTempFileName();
			_manager =  new Db4oRecordListManager(new DoNothingModelConfiguration(), _filePath);
			Lexicon.Init(_manager);

		}

		public void Dispose()
		{
			_manager.Dispose();
			File.Delete(_filePath);
		}
	}

	[TestFixture]
	public class MissingEntryRelationFieldFilterTests
	{
		private MissingItemFilter _missingRelationFieldFilter;
		private LexEntry _target;
		private LexEntry _source;
		private HelperForTestsRequiringDb4o _db4oTestHelper;

		[SetUp]
		public void Setup()
		{
			_db4oTestHelper = new HelperForTestsRequiringDb4o();
			_target = new LexEntry();
			_source = new LexEntry();
			Field relationField = new Field("synonyms", "LexEntry", new string[] { "vernacular" }, Field.MultiplicityType.ZeroOr1, "RelationToOneEntry");
			this._missingRelationFieldFilter = new MissingItemFilter(relationField);
		}

		[TearDown]
		public void Teardown()
		{
			_db4oTestHelper.Dispose();
		}

		[Test]
		public void FieldHasContents()
		{
			AddRelation();
			Assert.IsFalse(this._missingRelationFieldFilter.FilteringPredicate(_source));
		}

		[Test]
		public void LexEntryRelationCollectionMissingButSkipFlagged()
		{
			_source.SetFlag("intentionallyMissing_synonyms");
			Assert.IsFalse(this._missingRelationFieldFilter.FilteringPredicate(_source));
		}


		[Test]
		public void LexEntryRelationCollectionMissing()
		{
			Assert.IsTrue(this._missingRelationFieldFilter.FilteringPredicate(_source));
		}

		[Test]
		public void LexEntryRelationCollectionExistsButIsEmpty()
		{
			LexRelationCollection synonyms = _source.GetOrCreateProperty<LexRelationCollection>("synonyms");
			Assert.IsTrue(this._missingRelationFieldFilter.FilteringPredicate(_source));
		}

		private void AddRelation()
		{
			LexRelationCollection synonyms = _source.GetOrCreateProperty<LexRelationCollection>("synonyms");
			LexRelation r = new LexRelation("synonyms", _target.GetOrCreateId(true), _source);
			r.Target = _target;
			synonyms.Relations.Add(r);
		}
	}
}