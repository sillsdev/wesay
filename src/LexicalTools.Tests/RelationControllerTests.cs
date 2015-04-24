using System.Windows.Forms;
using NUnit.Framework;
using SIL.Data;
using Palaso.DictionaryServices.Model;
using Palaso.Lift;
using SIL.Reporting;
using Palaso.TestUtilities;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;
using WeSay.TestUtilities;
using WeSay.UI.AutoCompleteTextBox;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class RelationControllerTests
	{
		private LexEntry _target;
		private LexEntry _source;
		private LexEntryRepository _lexEntryRepository;
		private TemporaryFolder _tempFolder;
		private string _filePath;
		private LexRelationType _synonymsRelationType;
		private LexRelationType _singleSynonymRelationType;
		private Field _singleSynonymRelationField;
		private Field _synonymsRelationField;

		[SetUp]
		public void Setup()
		{
			WeSayProjectTestHelper.InitializeForTests();

			_tempFolder = new TemporaryFolder();
			_filePath = _tempFolder.GetTemporaryFile();
			_lexEntryRepository = new LexEntryRepository(_filePath);

			_target = CreateEntry("one", "single item");
			_source = CreateEntry("single", "one item");

			CreateEntry("verde", "green");
			CreateEntry("amarelho", "yellow");
			CreateEntry("azul", "blue");

			_synonymsRelationField = new Field("synonyms",
											   "LexEntry",
											   new string[] { WritingSystemsIdsForTests.OtherIdForTest },
											   Field.MultiplicityType.ZeroOrMore,
											   "RelationToOneEntry");
			_synonymsRelationType = new LexRelationType("synonyms",
														LexRelationType.Multiplicities.Many,
														LexRelationType.TargetTypes.Sense);

			_singleSynonymRelationField = new Field("synonym",
											  "LexEntry",
											  new string[] { WritingSystemsIdsForTests.OtherIdForTest },
											  Field.MultiplicityType.ZeroOr1,
											  "RelationToOneEntry");
			_singleSynonymRelationType = new LexRelationType("synonym",
													   LexRelationType.Multiplicities.One,
													   LexRelationType.TargetTypes.Sense);
		}

		[TearDown]
		public void Teardown()
		{
			_lexEntryRepository.Dispose();
			_tempFolder.Delete();
		}

		[Test]
		public void NoWritingSystemsEnabledOnField_ThrowsConfigurationException()
		{
				_singleSynonymRelationField.WritingSystemIds.Clear();
				Assert.Throws<ConfigurationException>(() => RelationController.CreateWidget(_source,
												_singleSynonymRelationType,
												_singleSynonymRelationField,
												_lexEntryRepository,
												delegate { }));
		}

		[Test]
		public void CreateWidget_Many()
		{
			RelationController.CreateWidget(_source,
											_synonymsRelationType,
											_synonymsRelationField,
											_lexEntryRepository,
											delegate { });
		}

		[Test]
		public void CreateWidget_Single_Empty()
		{
			RelationController.CreateWidget(_source,
											_singleSynonymRelationType,
											_singleSynonymRelationField,
											_lexEntryRepository,
											delegate { });
		}

		[Test]
		public void CreateWidget_Single_ExistingEmpty()
		{
			AddRelation(_source, _singleSynonymRelationField.FieldName, string.Empty);

			RelationController.CreateWidget(_source,
											_singleSynonymRelationType,
											_singleSynonymRelationField,
											_lexEntryRepository,
											delegate { });
		}

		[Test]
		public void CreateWidget_Single_ExistingRelationToNonExistantRecord_ControlDisplaysId()
		{
			AddRelation(_source, _singleSynonymRelationField.FieldName, "NonExistantId");

			Control c = RelationController.CreateWidget(_source,
														_singleSynonymRelationType,
														_singleSynonymRelationField,
														_lexEntryRepository,
														delegate { });
			Assert.AreEqual("NonExistantId", c.Text);
		}

		[Test]
		public void CreateWidget_Single_ExistingRelationToExistantRecord_ControlDisplaysLexicalUnit()
		{
			AddRelation(_source, _singleSynonymRelationField.FieldName, _target.Id);

			Control c = RelationController.CreateWidget(_source,
														_singleSynonymRelationType,
														_singleSynonymRelationField,
														_lexEntryRepository,
														delegate { });

			Assert.AreEqual(_target.LexicalForm[WritingSystemsIdsForTests.OtherIdForTest], c.Text);
		}

		[Test]
		public void ChangeBoundRelation_Single_ToEmpty()
		{
			LexRelation relation = AddRelation(_source, _singleSynonymRelationField.FieldName, _target.Id);

			Control c = RelationController.CreateWidget(_source,
														_singleSynonymRelationType,
														_singleSynonymRelationField,
														_lexEntryRepository,
														delegate { });
			c.Text = string.Empty;

			Assert.AreEqual(string.Empty, relation.Key);
		}

		[Test]
		public void ChangeBoundRelation_Single_ToNonExistant_NoRelation()
		{
			LexRelation relation = AddRelation(_source, _singleSynonymRelationField.FieldName, _target.Id);

			Control c = RelationController.CreateWidget(_source,
														_singleSynonymRelationType,
														_singleSynonymRelationField,
														_lexEntryRepository,
														delegate { });
			c.Text = "NonExistantId";

			Assert.AreEqual(string.Empty, relation.Key);
		}

		[Test]
		public void ChangeBoundRelation_Single_Existing()
		{
			LexRelation relation = AddRelation(_source, _singleSynonymRelationField.FieldName, "something");

			Control c = RelationController.CreateWidget(_source,
														_singleSynonymRelationType,
														_singleSynonymRelationField,
														_lexEntryRepository,
														delegate { });
			c.Text = _target.LexicalForm[WritingSystemsIdsForTests.OtherIdForTest];

			Assert.AreEqual(_target.Id, relation.Key);
		}

		[Test]
		public void ChangeBoundRelation_Single_ToNonExistantCreate_CreatesRelation()
		{
			LexRelation relation = AddRelation(_source, _singleSynonymRelationField.FieldName, _target.Id);

			Control c = RelationController.CreateWidget(_source,
														_singleSynonymRelationType,
														_singleSynonymRelationField,
														_lexEntryRepository,
														delegate { });
			c.Text = "new";

			AutoCompleteWithCreationBox<RecordToken<LexEntry>, string> picker =
					(AutoCompleteWithCreationBox<RecordToken<LexEntry>, string>) c;
			picker.CreateNewObjectFromText();

			LexEntry newEntry = _lexEntryRepository.GetLexEntryWithMatchingId(relation.Key);
			Assert.IsNotNull(newEntry);
			Assert.AreEqual("new", newEntry.LexicalForm[WritingSystemsIdsForTests.OtherIdForTest]);
		}

		[Test]
		public void TriggerFindApproximate_Single()
		{
			AddRelation(_source, _singleSynonymRelationField.FieldName, _target.Id);

			Control c = RelationController.CreateWidget(_source,
														_singleSynonymRelationType,
														_singleSynonymRelationField,
														_lexEntryRepository,
														delegate { });
			Form form = new Form();
			form.Controls.Add(c);
			AutoCompleteWithCreationBox<RecordToken<LexEntry>, string> picker =
					(AutoCompleteWithCreationBox<RecordToken<LexEntry>, string>) c;
			((WeSayAutoCompleteTextBox)(picker.Box)).Paste("text");
		}

		private LexEntry CreateEntry(string lexemeForm, string meaning)
		{
			LexEntry entry = _lexEntryRepository.CreateItem();

			entry.LexicalForm.SetAlternative(WritingSystemsIdsForTests.OtherIdForTest, lexemeForm);

			LexSense sense = new LexSense();
			entry.Senses.Add(sense);
			sense.Gloss[
					WeSayWordsProject.Project.DefaultViewTemplate.GetField(
							LexSense.WellKnownProperties.Definition).WritingSystemIds[0]] = meaning;
			_lexEntryRepository.SaveItem(entry);
			return entry;
		}

		private static LexRelation AddRelation(PalasoDataObject source,
											   string fieldName,
											   string targetId)
		{
			LexRelationCollection relationColection =
					source.GetOrCreateProperty<LexRelationCollection>(fieldName);

			LexRelation relation = new LexRelation(fieldName, targetId, source);
			relationColection.Relations.Add(relation);
			return relation;
		}
	}
}
