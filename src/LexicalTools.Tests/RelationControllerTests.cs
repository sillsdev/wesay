using System.Windows.Forms;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.UI.AutoCompleteTextBox;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class RelationControllerTests
	{
		private LexEntry _target;
		private LexEntry _source;
		private HelperForTestsRequiringDb4o _db4oTestHelper;
		private LexRelationType _synonymsRelationType;
		private LexRelationType _synonymRelationType;
		private Field _synonymRelationField;
		private Field _synonymsRelationField;

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.InitializeForTests();

			_db4oTestHelper = new HelperForTestsRequiringDb4o();
			Db4oLexModelHelper.Initialize(_db4oTestHelper.RecordListManager.DataSource.Data);

			_target = CreateEntry("one", "single item");
			_source = CreateEntry("single", "one item");

			IRecordList<LexEntry> recordList = this._db4oTestHelper.RecordListManager.GetListOfType<LexEntry>();

			recordList.Add(_target);
			recordList.Add(_source);
			recordList.Add(CreateEntry("verde", "green"));
			recordList.Add(CreateEntry("amarelho", "yellow"));
			recordList.Add(CreateEntry("azul", "blue"));

			this._synonymsRelationField = new Field("synonyms", "LexEntry", new string[] { "vernacular" }, Field.MultiplicityType.ZeroOrMore, "RelationToOneEntry");
			this._synonymsRelationType = new LexRelationType("synonyms", LexRelationType.Multiplicities.Many, LexRelationType.TargetTypes.Sense);

			this._synonymRelationField = new Field("synonym", "LexEntry", new string[] { "vernacular" }, Field.MultiplicityType.ZeroOr1, "RelationToOneEntry");
			this._synonymRelationType = new LexRelationType("synonym", LexRelationType.Multiplicities.One, LexRelationType.TargetTypes.Sense);
		}

		[TearDown]
		public void Teardown()
		{
			_db4oTestHelper.Dispose();
		}


		[Test]
		public void CreateWidget_Many()
		{

			RelationController.CreateWidget(_source,
								_synonymsRelationType,
								_synonymsRelationField,
								_db4oTestHelper.RecordListManager,
								delegate { });

		}

		[Test]
		public void CreateWidget_Single_Empty()
		{

			RelationController.CreateWidget(_source,
								_synonymRelationType,
								_synonymRelationField,
								_db4oTestHelper.RecordListManager,
								delegate
								{
								});

		}


		[Test]
		public void CreateWidget_Single_ExistingEmpty()
		{
			AddRelation(this._source, this._synonymRelationField.FieldName, string.Empty);

			RelationController.CreateWidget(_source,
								_synonymRelationType,
								_synonymRelationField,
								_db4oTestHelper.RecordListManager,
								delegate
								{
								});

		}

		[Test]
		public void CreateWidget_Single_ExistingRelationToNonExistantRecord_ControlDisplaysId()
		{
			AddRelation(this._source, this._synonymRelationField.FieldName, "NonExistantId");

			Control c = RelationController.CreateWidget(_source,
								_synonymRelationType,
								_synonymRelationField,
								_db4oTestHelper.RecordListManager,
								delegate
								{
								});
			Assert.AreEqual("NonExistantId", c.Text);
		}


		[Test]
		public void CreateWidget_Single_ExistingRelationToExistantRecord_ControlDisplaysLexicalUnit()
		{
			AddRelation(this._source, this._synonymRelationField.FieldName, _target.Id);

			Control c = RelationController.CreateWidget(_source,
								_synonymRelationType,
								_synonymRelationField,
								_db4oTestHelper.RecordListManager,
								delegate
								{
								});

			Assert.AreEqual(_target.LexicalForm["vernacular"], c.Text);

		}

		[Test]
		public void ChangeBoundRelation_Single_ToEmpty()
		{
			LexRelation relation = AddRelation(this._source, this._synonymRelationField.FieldName, _target.Id);

			Control c = RelationController.CreateWidget(_source,
								_synonymRelationType,
								_synonymRelationField,
								_db4oTestHelper.RecordListManager,
								delegate
								{
								});
			c.Text = string.Empty;

			Assert.AreEqual(string.Empty, relation.Key);
		}

		[Test]
		public void ChangeBoundRelation_Single_ToNonExistant_NoRelation()
		{
			LexRelation relation = AddRelation(this._source, this._synonymRelationField.FieldName, _target.Id);

			Control c = RelationController.CreateWidget(_source,
								_synonymRelationType,
								_synonymRelationField,
								_db4oTestHelper.RecordListManager,
								delegate
								{
								});
			c.Text = "NonExistantId";

			Assert.AreEqual(string.Empty, relation.Key);
		}

		[Test]
		public void ChangeBoundRelation_Single_Existing()
		{
			LexRelation relation = AddRelation(this._source, this._synonymRelationField.FieldName, "something");

			Control c = RelationController.CreateWidget(_source,
								_synonymRelationType,
								_synonymRelationField,
								_db4oTestHelper.RecordListManager,
								delegate
								{
								});
			c.Text = _target.LexicalForm["vernacular"];

			Assert.AreEqual(_target.Id, relation.Key);
		}

		[Test]
		public void ChangeBoundRelation_Single_ToNonExistantCreate_CreatesRelation()
		{
			LexRelation relation = AddRelation(this._source, this._synonymRelationField.FieldName, _target.Id);

			Control c = RelationController.CreateWidget(_source,
								_synonymRelationType,
								_synonymRelationField,
								_db4oTestHelper.RecordListManager,
								delegate
								{
								});
			c.Text = "new";

			AutoCompleteWithCreationBox<object, LexEntry> picker = (AutoCompleteWithCreationBox<object, LexEntry>) c;
			picker.CreateNewObjectFromText();

			LexEntry newEntry = Lexicon.FindFirstLexEntryMatchingId(relation.Key);
			Assert.IsNotNull(newEntry);
			Assert.AreEqual("new", newEntry.LexicalForm["vernacular"]);
		}

		[Test]
		public void TriggerFindApproximate_Single()
		{
			AddRelation(this._source, this._synonymRelationField.FieldName, _target.Id);

			Control c = RelationController.CreateWidget(_source,
								_synonymRelationType,
								_synonymRelationField,
								_db4oTestHelper.RecordListManager,
								delegate
								{
								});
			Form form = new Form();
			form.Controls.Add(c);
			AutoCompleteWithCreationBox<object, LexEntry> picker = (AutoCompleteWithCreationBox<object, LexEntry>)c;
			picker.Box.Paste("text");
		}

		private static LexEntry CreateEntry(string lexemeForm, string meaning)
		{
			LexEntry entry = new LexEntry();
			entry.LexicalForm.SetAlternative("vernacular", lexemeForm);

			LexSense sense = (LexSense)entry.Senses.AddNew();
			sense.Gloss[
				WeSayWordsProject.Project.DefaultViewTemplate.GetField("SenseGloss").WritingSystemIds[0]] =
				meaning;
			return entry;
		}

		private static LexRelation AddRelation(WeSayDataObject source, string fieldName, string targetId)
		{
			LexRelationCollection relationColection =
					source.GetOrCreateProperty<LexRelationCollection>(
							fieldName);

			LexRelation relation = new LexRelation(fieldName,
												   targetId,
												   source);
			relationColection.Relations.Add(relation);
			return relation;
		}

	}

}