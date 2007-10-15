using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class MissingEntryRelationFieldFilterTests
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_db4oTestHelper = new HelperForTestsRequiringDb4o();
			_target = new LexEntry();
			_source = new LexEntry();
			IRecordList<LexEntry> lexEntries = this._db4oTestHelper.RecordListManager.GetListOfType<LexEntry>();
			lexEntries.Add(_target);
			lexEntries.Add(_source);

			Field relationField =
					new Field("synonyms",
							  "LexEntry",
							  new string[] {"vernacular"},
							  Field.MultiplicityType.ZeroOr1,
							  "RelationToOneEntry");
			_missingRelationFieldFilter = new MissingItemFilter(relationField);
		}

		[TearDown]
		public void Teardown()
		{
			_db4oTestHelper.Dispose();
		}

		#endregion

		private MissingItemFilter _missingRelationFieldFilter;
		private LexEntry _target;
		private LexEntry _source;
		private HelperForTestsRequiringDb4o _db4oTestHelper;

		private void AddRelation()
		{
			LexRelationCollection synonyms =
					_source.GetOrCreateProperty<LexRelationCollection>("synonyms");
			LexRelation r = new LexRelation("synonyms", _target.GetOrCreateId(true), _source);
			r.Target = _target;
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