using System;
using System.Collections.Generic;
using System.IO;
using LiftIO;
using LiftIO.Parsing;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LiftMergerTests : ILiftMergerTestSuite
	{
		private LiftMerger _merger;
		protected Db4oDataSource _dataSource;
		protected Db4oRecordList<LexEntry> _entries;
		private string _tempFile;

		[SetUp]
		public void Setup()
		{
			WeSayWordsProject.InitializeForTests();

			_tempFile = Path.GetTempFileName();
			_dataSource = new Db4oDataSource(_tempFile);
			_entries = new Db4oRecordList<LexEntry>(_dataSource);
			Db4oLexModelHelper.Initialize(_dataSource.Data);

			_merger = new LiftMerger(_dataSource, _entries);
		}

		protected void RefreshEntriesList()
		{
			_entries.Dispose();
			_entries = new Db4oRecordList<LexEntry>(_dataSource);
		}

		[TearDown]
		public void TearDown()
		{
			_merger.Dispose();
			_entries.Dispose();
			_dataSource.Dispose();
			File.Delete(_tempFile);
		}

		[Test]
		public void NewEntryGetsId()
		{
			Extensible extensibleInfo = new Extensible();
			extensibleInfo.Id = "foo";
			LexEntry e = _merger.GetOrMakeEntry(extensibleInfo, 0);
			Assert.AreEqual(extensibleInfo.Id, e.Id);
			_merger.FinishEntry(e);
			RefreshEntriesList();
			Assert.AreEqual(1, _entries.Count);
		}

		[Test]
		public  void NewEntryGetsGuid()
		{
			Extensible extensibleInfo = new Extensible();
			extensibleInfo.Guid = Guid.NewGuid();
			LexEntry e= _merger.GetOrMakeEntry(extensibleInfo, 0);
			Assert.AreEqual(extensibleInfo.Guid, e.Guid);
			_merger.FinishEntry(e);
			RefreshEntriesList();
			Assert.AreEqual(1, _entries.Count);
		}


		[Test]
		public void NewEntryGetsDates()
		{
			Extensible extensibleInfo = new Extensible();
			extensibleInfo.CreationTime = DateTime.Parse("2/2/1969  12:15:12").ToUniversalTime();
			extensibleInfo.ModificationTime = DateTime.Parse("10/11/1968  12:15:12").ToUniversalTime();
			LexEntry e = _merger.GetOrMakeEntry(extensibleInfo, 0);
			Assert.AreEqual(extensibleInfo.CreationTime, e.CreationTime);
			Assert.AreEqual(extensibleInfo.ModificationTime, e.ModificationTime);
			_merger.FinishEntry(e);
			RefreshEntriesList();
			Assert.AreEqual(1, _entries.Count);
		}

		[Test]
		public  void NewEntryWithTextIdIgnoresIt()
		{
			Extensible extensibleInfo = new Extensible();
			extensibleInfo.Id = "hello";
			LexEntry e = _merger.GetOrMakeEntry(extensibleInfo, 0);
			//no attempt is made to use that id
			Assert.IsNotNull(e.Guid);
			Assert.AreNotSame(Guid.Empty, e.Guid);
		}

		[Test]
		public  void NewEntryTakesGivenDates()
		{
			Extensible extensibleInfo = new Extensible();
			extensibleInfo = AddDates(extensibleInfo);

			LexEntry e = _merger.GetOrMakeEntry(extensibleInfo, 0);
			Assert.AreEqual(extensibleInfo.CreationTime, e.CreationTime);
			Assert.AreEqual(extensibleInfo.ModificationTime, e.ModificationTime);
		}

		[Test]
		public  void NewEntryNoDatesUsesNow()
		{
			LexEntry e = MakeSimpleEntry();
			Assert.IsTrue(TimeSpan.FromTicks(DateTime.UtcNow.Ticks - e.CreationTime.Ticks).Seconds < 2);
			Assert.IsTrue(TimeSpan.FromTicks(DateTime.UtcNow.Ticks - e.ModificationTime.Ticks).Seconds < 2);
		}

		private LexEntry MakeSimpleEntry()
		{
			Extensible extensibleInfo = new Extensible();
			return _merger.GetOrMakeEntry(extensibleInfo, 0);
		}


		[Test]
		public  void EntryGetsEmptyLexemeForm()
		{
			LexEntry e = MakeSimpleEntry();
			_merger.MergeInLexemeForm(e, new LiftMultiText());
			Assert.AreEqual(0, e.LexicalForm.Count);
		}

		[Test]
		public void EntryGetsNote()
		{
			LexEntry e = MakeSimpleEntry();
			_merger.MergeInNote(e, null, MakeBasicLiftMultiText());
			AssertPropertyHasExpectedMultiText(e, WeSayDataObject.WellKnownProperties.Note);
			MultiText m = e.GetProperty<MultiText>(WeSayDataObject.WellKnownProperties.Note);
			Assert.IsTrue(m.ContainsAlternative("ws-one"));
			Assert.IsTrue(m.ContainsAlternative("ws-two"));
			Assert.AreEqual("uno",m["ws-one"]);
			Assert.AreEqual("dos",m["ws-two"]);
		}

		[Test]
		public void TypeOfNoteEmbedded()
		{
			LexEntry e = MakeSimpleEntry();
			_merger.MergeInNote(e, "red", MakeBasicLiftMultiText());
			MultiText mt = e.GetProperty<MultiText>(WeSayDataObject.WellKnownProperties.Note);
			Assert.AreEqual("(red) uno", mt["ws-one"]);
			Assert.AreEqual("(red) dos", mt["ws-two"]);
		}

		[Test]
		public void SenseGetsGrammi()
		{
			LexSense sense = new LexSense();
			_merger.MergeInGrammaticalInfo(sense, "red", null);
			OptionRef optionRef = sense.GetProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech);
			Assert.IsNotNull(optionRef);
			Assert.AreEqual("red", optionRef.Value);
		}

		[Test]
		public void GrammiGetsFlagTrait()
		{
			LexSense sense = new LexSense();
			_merger.MergeInGrammaticalInfo(sense, "red", new List<Trait>(new Trait[]{new Trait("flag", "1")}));
			OptionRef optionRef = sense.GetProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech);
			Assert.IsTrue(optionRef.IsStarred);
		}

		[Test]
		public void SenseGetsExample()
		{
			LexSense sense = new LexSense();
			Extensible x = new Extensible();
			LexExampleSentence ex = _merger.GetOrMakeExample(sense, x);
			Assert.IsNotNull(ex);
			_merger.MergeInExampleForm(ex, MakeBasicLiftMultiText());
			Assert.AreEqual(2, ex.Sentence.Forms.Length);
			Assert.AreEqual("dos", ex.Sentence["ws-two"]);
		}


		[Test]
		public void SenseGetsRelation()
		{
			LexSense sense = new LexSense();
			_merger.MergeInRelation(sense, "synonym", "foo", null);
			LexRelationCollection synonyms= sense.GetProperty<LexRelationCollection>("synonym");
			LexRelation relation = synonyms.Relations[0];
			Assert.AreEqual("synonym", relation.FieldId);
			Assert.AreEqual("foo", relation.Key);
		}

		[Test]
		public void ExampleSourcePreserved()
		{
			LexExampleSentence ex = new LexExampleSentence();
			_merger.MergeInSource(ex, "fred");

			Assert.AreEqual("fred", ex.GetProperty<OptionRef>(LexExampleSentence.WellKnownProperties.Source).Value);
		}

		[Test]
		public void SenseGetsDef()
		{
			LexSense sense = new LexSense();
			_merger.MergeInDefinition(sense, MakeBasicLiftMultiText());
			AssertPropertyHasExpectedMultiText(sense, LexSense.WellKnownProperties.Definition);
		}

		[Test]
		public void SenseGetsNote()
		{
			LexSense sense = new LexSense();
			_merger.MergeInNote(sense, null, MakeBasicLiftMultiText());
			AssertPropertyHasExpectedMultiText(sense, WeSayDataObject.WellKnownProperties.Note);
		}

		[Test]
		public void SenseGetsId()
		{
			Extensible extensibleInfo = new Extensible();
			extensibleInfo.Id = "foo";
			LexSense s = _merger.GetOrMakeSense(new LexEntry(),extensibleInfo) ;
			Assert.AreEqual(extensibleInfo.Id, s.Id);
		}

		[Test]
		public void MultipleNotesCombined()
		{
			LexSense sense = new LexSense();
			_merger.MergeInNote(sense, null, MakeBasicLiftMultiText());
			LiftMultiText secondNote = new LiftMultiText();
			secondNote.Add("ws-one", "UNO");
			secondNote.Add("ws-three", "tres");
			_merger.MergeInNote(sense, null, secondNote);

			MultiText mt = sense.GetProperty<MultiText>(LexSense.WellKnownProperties.Note);
			Assert.AreEqual(3, mt.Forms.Length);
			Assert.AreEqual("uno || UNO", mt["ws-one"]);
		}

//        [Test]
//        public void MergingIntoEmptyMultiTextWithFlags()
//        {
//            LiftMultiText lm = new LiftMultiText();
//            lm.Add("one", "uno");
//            lm.Add("two", "dos");
//            lm.Traits.Add(new Trait("one", "flag", "1"));
//
//            MultiText m = new MultiText();
//            MultiText.Create(lm as System.Collections.Generic.Dictionary<string,string>, List<)
//            LexSense sense = new LexSense();
//            LiftMultiText text = MakeBasicLiftMultiText();
//            text.Traits.Add(new Trait("ws-one", "flag", "1"));
//            _merger.MergeInGloss(sense, text);
//
//            Assert.IsTrue(sense.Gloss.GetAnnotationOfAlternativeIsStarred("ws-one"));
//        }

		[Test]
		public void GlossGetsFlag()
		{
			LexSense sense = new LexSense();
			LiftMultiText text = MakeBasicLiftMultiText();
			AddAnnotationToLiftMultiText(text, "ws-one", "flag", "1");
			_merger.MergeInGloss(sense, text);
			Assert.IsTrue(sense.Gloss.GetAnnotationOfAlternativeIsStarred("ws-one"));
			Assert.IsFalse(sense.Gloss.GetAnnotationOfAlternativeIsStarred("ws-two"));

			text = MakeBasicLiftMultiText();
			AddAnnotationToLiftMultiText(text, "ws-one", "flag", "0");
			_merger.MergeInGloss(sense, text);
			Assert.IsFalse(sense.Gloss.GetAnnotationOfAlternativeIsStarred("ws-one"));
		}

		[Test]
		public void LexicalUnitGetsFlag()
		{
			LexEntry entry = MakeSimpleEntry();
			LiftMultiText text = MakeBasicLiftMultiText();
			AddAnnotationToLiftMultiText(text, "ws-one", "flag", "1");
			_merger.MergeInLexemeForm(entry, text);
			Assert.IsTrue(entry.LexicalForm.GetAnnotationOfAlternativeIsStarred("ws-one"));
			Assert.IsFalse(entry.LexicalForm.GetAnnotationOfAlternativeIsStarred("ws-two"));
		}

		private static void AddAnnotationToLiftMultiText(LiftMultiText text, string languageHint, string name, string value)
		{
			Annotation annotation = new Annotation(name, value, default(DateTime), null);
			annotation.LanguageHint = languageHint;
			text.Annotations.Add(annotation);
		}

		[Test]
		public void MultipleGlossesCombined()
		{
			LexSense sense = new LexSense();
			_merger.MergeInGloss(sense, MakeBasicLiftMultiText());
			LiftMultiText secondGloss = new LiftMultiText();
			secondGloss.Add("ws-one", "UNO");
			secondGloss.Add("ws-three", "tres");
			_merger.MergeInGloss(sense, secondGloss);

			//MultiText mt = sense.GetProperty<MultiText>(LexSense.WellKnownProperties.Note);
			Assert.AreEqual(3, sense.Gloss.Forms.Length);
			Assert.AreEqual("uno; UNO", sense.Gloss["ws-one"]);
		}

		private static void AssertPropertyHasExpectedMultiText(WeSayDataObject dataObject, string name)
		{
			//must match what is created by MakeBasicLiftMultiText()
			MultiText mt = dataObject.GetProperty<MultiText>(name);
			Assert.AreEqual(2, mt.Forms.Length);
			Assert.AreEqual("dos", mt["ws-two"]);
		}

		private static LiftMultiText MakeBasicLiftMultiText()
		{
			LiftMultiText forms = new LiftMultiText();
			forms.Add("ws-one", "uno");
			forms.Add("ws-two", "dos");
			return forms;
		}



		#region ILiftMergerTestSuite Members

		[Test, Ignore("not yet")]
		public void NewWritingSystemAlternativeHandled()
		{
		}

		#endregion

		[Test]
		public  void EntryGetsLexemeFormWithUnheardOfLanguage()
		{
			LexEntry e = MakeSimpleEntry();
			LiftMultiText forms = new LiftMultiText();
			forms.Add("x99", "hello");
			_merger.MergeInLexemeForm(e, forms);
			Assert.AreEqual("hello", e.LexicalForm["x99"]);
		}

		[Test]
		public  void NewEntryGetsLexemeForm()
		{
			LexEntry e = MakeSimpleEntry();
			LiftMultiText forms = new LiftMultiText();
			forms.Add("x", "hello");
			forms.Add("y", "bye");
			_merger.MergeInLexemeForm(e, forms);
			Assert.AreEqual(2, e.LexicalForm.Count);
		}


		[Test]
		public void EntryWithCitation()
		{
			LexEntry entry = MakeSimpleEntry();
			LiftMultiText forms = new LiftMultiText();
			forms.Add("x", "hello");
			forms.Add("y", "bye");
			_merger.MergeInCitationForm(entry, forms);

			MultiText citation = entry.GetProperty<MultiText>(LexEntry.WellKnownProperties.Citation);
			Assert.AreEqual(2, citation.Forms.Length);
			Assert.AreEqual("hello", citation["x"]);
			Assert.AreEqual("bye", citation["y"]);

		}

		[Test]
		public  void EntryWithChildren()
		{
			Extensible extensibleInfo = new Extensible();
			LexEntry e = MakeSimpleEntry();
			LexSense s= _merger.GetOrMakeSense(e, extensibleInfo);

			LexExampleSentence ex = _merger.GetOrMakeExample(s, new Extensible());
			ex.Sentence["foo"] = "this is a sentence";
			ex.Translation["aa"] = "aaaa";
			_merger.FinishEntry(e);
			CheckCompleteEntry(e);

			RefreshEntriesList();
			Assert.AreEqual(1, _entries.Count);

			//now check it again, from the list
			CheckCompleteEntry(_entries[0]);
		}

		private static void CheckCompleteEntry(LexEntry entry)
		{
			Assert.AreEqual(1, entry.Senses.Count);
			LexSense sense = (LexSense)entry.Senses[0];
			Assert.AreEqual(1, sense.ExampleSentences.Count);
			LexExampleSentence  example = (LexExampleSentence)sense.ExampleSentences[0];
			Assert.AreEqual("this is a sentence", example.Sentence["foo"]);
			Assert.AreEqual("aaaa", example.Translation["aa"]);
			Assert.AreEqual(entry, sense.Parent);
			Assert.AreEqual(entry, example.Parent.Parent);
		}

		[Test, Ignore("Haven't implemented protecting modified dates of, e.g., the entry as you add/merge its children.")]
		public  void ModifiedDatesRetained()
		{
		}


		[Test]
		public  void ChangedEntryFound()
		{
#if merging
			Guid g = Guid.NewGuid();
			Extensible extensibleInfo = CreateFullextensibleInfo(g);

			LexEntry e = new LexEntry(null, g);
			e.Senses.AddNew();
			e.Senses.AddNew();
			e.CreationTime = extensibleInfo.CreationTime;
			e.ModificationTime = new DateTime(e.CreationTime.Ticks + 100, DateTimeKind.Utc);
			_entries.Add(e);

			LexEntry found = _merger.GetOrMakeEntry(extensibleInfo);
			_merger.FinishEntry(found);
			Assert.AreSame(found, e);
			Assert.AreEqual(2, found.Senses.Count);

			//this is a temp side track
			Assert.AreEqual(1, _entries.Count);
			Extensible xInfo = CreateFullextensibleInfo(Guid.NewGuid());
			LexEntry x = _merger.GetOrMakeEntry(xInfo);
			_merger.FinishEntry(x);
			RefreshEntriesList();
			Assert.AreEqual(2, _entries.Count);
#endif
		}

		[Test]
		public  void UnchangedEntryPruned()
		{
#if merging
			Guid g = Guid.NewGuid();
			Extensible extensibleInfo = CreateFullextensibleInfo( g);

			LexEntry e = new LexEntry(null, g);
			e.CreationTime = extensibleInfo.CreationTime;
			e.ModificationTime = extensibleInfo.ModificationTime;
			_entries.Add(e);

			LexEntry found = _merger.GetOrMakeEntry(extensibleInfo);
			Assert.IsNull(found);
#endif
		}

		[Test]
		public  void EntryWithIncomingUnspecifiedModTimeNotPruned()
		{
			Guid g = Guid.NewGuid();
			Extensible extensibleInfo = CreateFullextensibleInfo(g);
			LexEntry e = new LexEntry(null, g);
			e.CreationTime = extensibleInfo.CreationTime;
			e.ModificationTime = extensibleInfo.ModificationTime;
			_entries.Add(e);

		   //strip out the time
			extensibleInfo.ModificationTime = Extensible.ParseDateTimeCorrectly("2005-01-01");
			Assert.AreEqual(DateTimeKind.Utc ,extensibleInfo.ModificationTime.Kind );

			LexEntry found = _merger.GetOrMakeEntry(extensibleInfo, 0);
			Assert.IsNotNull(found);
		}

		[Test]
		public void ParseDateTimeCorrectly()
		{
			 Assert.AreEqual(DateTimeKind.Utc, Extensible.ParseDateTimeCorrectly("2003-08-07T08:42:42Z").Kind);
			 Assert.AreEqual(DateTimeKind.Utc, Extensible.ParseDateTimeCorrectly("2005-01-01T01:11:11+8:00").Kind);
			 Assert.AreEqual(DateTimeKind.Utc, Extensible.ParseDateTimeCorrectly("2005-01-01").Kind);
			 Assert.AreEqual("00:00:00", Extensible.ParseDateTimeCorrectly("2005-01-01").TimeOfDay.ToString());

		}

		[Test, Ignore("Haven't implemented this.")]
		public  void MergingSameEntryLackingGuidId_TwiceFindMatch()
		{
		}

		private static Extensible AddDates(Extensible extensibleInfo)
		{
			extensibleInfo.CreationTime = Extensible.ParseDateTimeCorrectly("2003-08-07T08:42:42Z");
			extensibleInfo.ModificationTime = Extensible.ParseDateTimeCorrectly("2005-01-01T01:11:11+8:00");
			return extensibleInfo;
		}

		private static Extensible CreateFullextensibleInfo(Guid g)
		{
			Extensible extensibleInfo = new Extensible();
			extensibleInfo.Guid = g;
			extensibleInfo = AddDates(extensibleInfo);
			return extensibleInfo;
		}


		[Test]
		public void ExpectedAtomicTraitOnEntry()
		{
			_merger.ExpectedOptionTraits.Add("flub");
			LexEntry e = MakeSimpleEntry();
			_merger.MergeInTrait(e, new Trait("flub", "dub"));
			Assert.AreEqual(1, e.Properties.Count);
			Assert.AreEqual("flub", e.Properties[0].Key);
			OptionRef option = e.GetProperty<OptionRef>("flub");
			Assert.AreEqual("dub", option.Value);
		}

		[Test]
		public void UnexpectedAtomicTraitRetained()
		{
			LexEntry e = MakeSimpleEntry();
			_merger.MergeInTrait(e, new Trait("flub", "dub"));
			Assert.AreEqual(1, e.Properties.Count);
			Assert.AreEqual("flub", e.Properties[0].Key);
			OptionRefCollection option = e.GetProperty<OptionRefCollection>("flub");
			Assert.IsTrue(option.Contains("dub"));
		}

		[Test]
		public void ExpectedCollectionTrait()
		{
			_merger.ExpectedOptionCollectionTraits.Add("flub");
			LexEntry e = MakeSimpleEntry();
			_merger.MergeInTrait(e, new Trait("flub", "dub"));
			_merger.MergeInTrait(e, new Trait("flub", "stub"));
			Assert.AreEqual(1, e.Properties.Count);
			Assert.AreEqual("flub", e.Properties[0].Key);
			OptionRefCollection options = e.GetProperty<OptionRefCollection>("flub");
			Assert.AreEqual(2, options.Count);
			Assert.IsTrue(options.Contains("dub"));
			Assert.IsTrue(options.Contains("stub"));
		}

		[Test]
		public void UnexpectedAtomicCollectionRetained()
		{
			LexEntry e = MakeSimpleEntry();
			_merger.MergeInTrait(e, new Trait("flub", "dub"));
			_merger.MergeInTrait(e, new Trait("flub", "stub"));
			Assert.AreEqual(1, e.Properties.Count);
			Assert.AreEqual("flub", e.Properties[0].Key);
			OptionRefCollection option = e.GetProperty<OptionRefCollection>("flub");
			Assert.IsTrue(option.Contains("dub"));
			Assert.IsTrue(option.Contains("stub"));

		}

		[Test]
		public void ExpectedCustomField()
		{
			LexEntry e = MakeSimpleEntry();
			LiftMultiText t = new LiftMultiText();
			t["z"] = new LiftString("dub");
			_merger.MergeInField(e, "flub", default(DateTime), default(DateTime), t, null);
			Assert.AreEqual(1, e.Properties.Count);
			Assert.AreEqual("flub", e.Properties[0].Key);
			MultiText mt = e.GetProperty<MultiText>("flub");
			Assert.AreEqual("dub", mt["z"]);
		}

		[Test]
		public void UnexpectedCustomFieldRetained()
		{
			LexEntry e = MakeSimpleEntry();
			LiftMultiText t = new LiftMultiText();
			t["z"] = new LiftString("dub");
			_merger.MergeInField(e, "flub", default(DateTime), default(DateTime), t, null);
			Assert.AreEqual(1, e.Properties.Count);
			Assert.AreEqual("flub", e.Properties[0].Key);
			MultiText mt = e.GetProperty<MultiText>("flub");
			Assert.AreEqual("dub", mt["z" ]);
		}

		[Test]
		public void EntryGetsFlag()
		{
			LexEntry e = MakeSimpleEntry();
			_merger.MergeInTrait(e, new Trait("flag_skip_BaseForm", null) );
			Assert.IsTrue(e.GetHasFlag("flag_skip_BaseForm"));
		}


		[Test]
		public void SenseGetsPictureNoCaption()
		{
			Extensible extensibleInfo = new Extensible();
			LexEntry e = MakeSimpleEntry();
			LexSense s = _merger.GetOrMakeSense(e, extensibleInfo);

			_merger.MergeInPicture(s, "testPicture.png", null);
			PictureRef pict = s.GetProperty<PictureRef>("Picture");
			Assert.AreEqual("testPicture.png", pict.Value);
			Assert.IsNull(pict.Caption);
		}
		[Test]
		public void SenseGetsPictureWithCaption()
		{
			Extensible extensibleInfo = new Extensible();
			LexEntry e = MakeSimpleEntry();
			LexSense s = _merger.GetOrMakeSense(e, extensibleInfo);

			LiftMultiText caption = new LiftMultiText();
			caption["aa"] = new LiftString("acaption");
			_merger.MergeInPicture(s, "testPicture.png", caption);
			PictureRef pict = s.GetProperty<PictureRef>("Picture");
			Assert.AreEqual("testPicture.png", pict.Value);
			Assert.AreEqual("acaption", pict.Caption["aa"]);
		}
	}

}