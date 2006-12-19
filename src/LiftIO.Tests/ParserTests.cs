using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using LiftIO;
using NMock2;
using NMock2.Syntax;
using NUnit.Framework;
using System.Xml;

namespace LiftIO.Tests
{
	[TestFixture]
	public class ParserTests
	{
		private ILexiconMerger<Mute, Mute, Mute> _merger;
		private LiftParser<Mute, Mute, Mute> _parser;
		private XmlDocument _doc;
		public StringBuilder _results;
		private Mockery _mocks;
		private List<LiftParser<Mute, Mute, Mute>.ErrorArgs> _parsingErrors;


		[SetUp]
		public void Setup()
		{
			//_parsingErrors = new List<Exception>();
			_doc = new XmlDocument();
			_mocks = new Mockery();
			_merger = _mocks.NewMock<ILexiconMerger<Mute, Mute, Mute>>();
			_parser = new LiftParser<Mute, Mute, Mute>(_merger);
			_parsingErrors = new List<LiftParser<Mute, Mute, Mute>.ErrorArgs>();
			_parser.ParsingError += new EventHandler<LiftParser<Mute, Mute,Mute>.ErrorArgs>(OnParsingError);
		}

		void OnParsingError(object sender, LiftParser<Mute, Mute, Mute>.ErrorArgs e)
		{
			_parsingErrors.Add(e);
		}

		[TearDown]
		public void TearDown()
		{


		}


		[Test]
		public void EmptyLiftOk()
		{
			SimpleCheckGetOrMakeEntry("<lift/>", 0);
		}

		[Test]
		public void EntryMissingIdNonFatal()
		{
			SimpleCheckGetOrMakeEntry("<lift><entry/></lift>", 1);
		}

		[Test]
		public void EmptyEntriesOk()
		{
			SimpleCheckGetOrMakeEntry("<lift><entry/><entry/></lift>", 2);
		}

		private void SimpleCheckGetOrMakeEntry(string content, int times)
		{
			_doc.LoadXml(content);
			using (_mocks.Ordered)
			{
				Expect.Exactly(times).On(_merger)
					.Method("GetOrMakeEntry")
					.WithAnyArguments()
					.Will(Return.Value(null));
			}
			_parser.ReadFile(_doc);
			_mocks.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void EntryWithId()
		{
			Guid g = Guid.NewGuid();
			ExpectMergeInLexemeForm();
			ParseEntryAndCheck(string.Format("<entry id=\"{0}\" />", g.ToString()),
				 string.Format("{0};;;",g.ToString()));
//
//            _doc.LoadXml(string.Format("<entry id=\"{0}\" />", g.ToString()));
//            using (_mocks.Ordered)
//            {
//                string idInfoString = g.ToString();
//                Expect.Exactly(1).On(_merger)
//                    .Method("GetOrMakeEntry")
//                    .With(Has.ToString(new NMock2.Matchers.StringContainsMatcher(idInfoString)))
//                    .Will(Return.Value(null));
//            }
//            _parser.ReadEntry(_doc.FirstChild);
//            _mocks.VerifyAllExpectationsHaveBeenMet();
		}

		private void ParseEntryAndCheck(string content, string expectedIdString)
		{
			ExpectGetOrMakeEntry(expectedIdString);

			_doc.LoadXml(content);
			_parser.ReadEntry(_doc.FirstChild);
			_mocks.VerifyAllExpectationsHaveBeenMet();

		}


		private void ParseEntryAndCheck(string content)
		{
			_doc.LoadXml(content);
			_parser.ReadEntry(_doc.FirstChild);
			_mocks.VerifyAllExpectationsHaveBeenMet();
		}

		private void ExpectGetOrMakeEntry(string expectedIdString)
		{
			Expect.Exactly(1).On(_merger)
				.Method("GetOrMakeEntry")
			  //  .With(Is.Anything)
				.With(Has.ToString(Is.EqualTo(expectedIdString)))
				.Will(Return.Value(new Mute()));
		}

		private void ExpectGetOrMakeEntry()
		{
			Expect.Exactly(1).On(_merger)
				.Method("GetOrMakeEntry")
				.Will(Return.Value(new Mute()));
		}

		private void ExpectGetOrMakeSense()
		{
			Expect.Exactly(1).On(_merger)
				.Method("GetOrMakeSense")
				.Will(Return.Value(new Mute()));
		}

		private void ExpectGetOrMakeExample()
		{
			Expect.Exactly(1).On(_merger)
				.Method("GetOrMakeExample")
				.Will(Return.Value(new Mute()));
		}

		private void ExpectMergeInLexemeForm(string exactMultiTextToString)
		{
			Expect.Exactly(1).On(_merger)
				.Method("MergeInLexemeForm")
				.With(Is.Anything, Is.Same(exactMultiTextToString));
		}
		private void ExpectMergeInLexemeForm()
		{
			Expect.Exactly(1).On(_merger)
				.Method("MergeInLexemeForm");
		}
		private void ExpectMergeGloss()
		{
			Expect.Exactly(1).On(_merger)
				.Method("MergeInGloss");
		}
		private void ExpectMergeDefinition()
		{
			Expect.Exactly(1).On(_merger)
				.Method("MergeInDefinition");
		}
		[Test]
		public void EntryWithoutId()
		{
			ExpectMergeInLexemeForm();
			ParseEntryAndCheck("<entry/>", ";;;");
		}


		[Test]
		public void FormMissingLangGeneratesNonFatalError()
		{
			ExpectGetOrMakeEntry();
			ExpectMergeInLexemeForm();
			ParseEntryAndCheck("<entry><lex><form/></lex></entry>");
			Assert.AreEqual(1, _parsingErrors.Count);
		}


		[Test]
		public void EmptyFormOk()
		{
		   using (_mocks.Ordered)
			{
				ExpectGetOrMakeEntry(";;;");
				ExpectMergeInLexemeForm();
			}
			ParseEntryAndCheck("<entry><lex><form lang='x'/></lex></entry>");
		}

		[Test]
		public void EntryWithLexemeForm()
		{
			ExpectGetOrMakeEntry();
			ExpectMultiTextMergeIn("LexemeForm", Has.Property("Count", Is.EqualTo(2)));
			ParseEntryAndCheck("<entry><lex><form lang='x'>hello</form><form lang='y'>bye</form></lex></entry>");
 //           ParseEntryAndCheck("<entry><lex><form lang='x'>hello</form><form lang='y'>bye</form></lex></entry>", "GetOrMakeEntry(;;;)MergeInLexemeForm(m,x=hello|y=bye|)");
		}

		 private void ExpectEmptyMultiTextMergeIn(string MultiTextPropertyName)
		{
			Expect.Exactly(1).On(_merger)
							.Method("MergeIn" + MultiTextPropertyName)
							.With(Is.Anything, Has.Property("Count",Is.EqualTo(0)));

		}

		private void ExpectMultiTextMergeIn(string MultiTextPropertyName, string value)
		{
			 Expect.Exactly(1).On(_merger)
							.Method("MergeIn" + MultiTextPropertyName)
							.With(Is.Anything, Has.ToString(Is.EqualTo(value)));
	   }

		private void ExpectMultiTextMergeIn(string MultiTextPropertyName, NMock2.Matcher multiTextMatcher)
		{
			Expect.Exactly(1).On(_merger)
						   .Method("MergeIn" + MultiTextPropertyName)
						   .With(Is.Anything, multiTextMatcher);
		}

		[Test]
		public void EntryWithLexemeForm_NoFormTag()
		{
			ExpectGetOrMakeEntry();
			ExpectMultiTextMergeIn("LexemeForm", "??=hello|");
			ParseEntryAndCheck("<entry><lex>hello</lex></entry>");
			//            ParseEntryAndCheck("<entry><lex>hello</lex></entry>","GetOrMakeEntry(;;;)MergeInLexemeForm(m,??=hello)");
		}

		[Test]
		public void NonLiftDateError()
		{
			TryDateFormat("last tuesday");
			TryDateFormat("2005-01-01T01:11:11");
			TryDateFormat("1/2/2003");
			Assert.AreEqual(3, _parsingErrors.Count);
		}

		private void TryDateFormat(string created)
		{
			ExpectGetOrMakeEntry();
			ExpectMergeInLexemeForm();
			ParseEntryAndCheck(
				string.Format("<entry id='foo' dateCreated='{0}'></entry>", created));
		}

		[Test]
		public void DateWithoutTimeOk()
		{
			ExpectGetOrMakeEntry();
			ExpectMergeInLexemeForm();
			ParseEntryAndCheck("<entry id='foo' dateCreated='2005-01-01'></entry>");
			Assert.AreEqual(0, _parsingErrors.Count);
		}

		[Test]
		public void EntryWithDates()
		{
			string created = "2003-08-07T08:42:42+07:00";
			string mod = "2005-01-01T01:11:11+07:00";
			ExpectGetOrMakeEntry(String.Format("foo;{0};{1};", created, mod));
			ExpectEmptyMultiTextMergeIn("LexemeForm");
			ParseEntryAndCheck(
				string.Format("<entry id='foo' dateCreated='{0}' dateModified='{1}'></entry>", created, mod));

			 //   String.Format("GetOrMakeEntry(foo;{0};{1};)", created,mod));
//            ParseEntryAndCheck(string.Format("<entry id='foo' dateCreated='{0}' dateModified='{1}'></entry>", created, mod),
//                String.Format("GetOrMakeEntry(foo;{0};{1};)", created,mod));
		}

		[Test]
		public void EntryWithSense()
		{
			ExpectGetOrMakeEntry();
			ExpectMergeInLexemeForm();
			ExpectGetOrMakeSense();
			ExpectMergeGloss();
			ExpectMergeDefinition();
		   ParseEntryAndCheck(string.Format("<entry><sense></sense></entry>"));
		}

		[Test]
		public void SenseWithGloss()
		{
			ExpectGetOrMakeEntry();
			ExpectMergeInLexemeForm();
			ExpectGetOrMakeSense();
			ExpectMultiTextMergeIn("Gloss","x=hello|");
			ExpectMergeDefinition();
			ParseEntryAndCheck(string.Format("<entry><sense><gloss><form lang='x'>hello</form></gloss></sense></entry>"));
//            ParseEntryAndCheck(string.Format("<entry><sense><gloss><form lang='x'>hello</form></gloss></sense></entry>"),
//                "GetOrMakeEntry(;;;)GetOrMakeSense(m,)MergeInGloss(m,x=hello|)");
		}
		[Test]
		public void SenseWithDefintition()
		{
			ExpectGetOrMakeEntry();
			ExpectMergeInLexemeForm();
			ExpectGetOrMakeSense();
			ExpectMergeGloss();
			ExpectMultiTextMergeIn("Definition", "x=hello|");

			ParseEntryAndCheck(string.Format("<entry><sense><def><form lang='x'>hello</form></def></sense></entry>"));

			//    "GetOrMakeEntry(;;;)GetOrMakeSense(m,)MergeInDefinition(m,x=hello|)");
		}

		[Test, Ignore("Not implemented")]
		public void SenseWithSemanticDomains()
		{
			ParseEntryAndCheck(string.Format("<entry><sense></sense></entry>"),
				"");
		}

		[Test, Ignore("Not implemented")]
		public void SenseWithGrammi()
		{
			ParseEntryAndCheck(string.Format("<entry><sense><grammi></grammi></sense></entry>"),
				"GetOrMakeEntry(;;;)GetOrMakeSense(m,)");
		}

		[Test, Ignore("Not implemented")]
		public void SenseWithEmptyGrammi()
		{
			ParseEntryAndCheck(string.Format("<entry><sense><grammi></grammi></sense></entry>"),
				"GetOrMakeEntry(;;;)GetOrMakeSense(m,)");
		}

		[Test]
		public void SenseWithExample()
		{
			ExpectGetOrMakeEntry();
			ExpectMergeInLexemeForm();
			ExpectGetOrMakeSense();
			ExpectMergeGloss();
			ExpectMergeDefinition();
			ExpectGetOrMakeExample();
			ExpectMultiTextMergeIn("ExampleForm", "x=hello|");
			ExpectMultiTextMergeIn("TranslationForm", "");

			ParseEntryAndCheck(
				string.Format("<entry><sense><example><form lang='x'>hello</form></example></sense></entry>"));
		}

		[Test]
		public void ExampleWithTranslation()
		{
			ExpectGetOrMakeEntry();
			ExpectMergeInLexemeForm();
			ExpectGetOrMakeSense();
			ExpectMergeGloss();
			ExpectMergeDefinition();
			ExpectGetOrMakeExample();
			ExpectMultiTextMergeIn("ExampleForm", "");
			ExpectMultiTextMergeIn("TranslationForm", "x=hello|");

			ParseEntryAndCheck("<entry><sense><example><translation><form lang='x'>hello</form></translation></example></sense></entry>");
			//    "GetOrMakeEntry(;;;)GetOrMakeSense(m,)GetOrMakeExample(m,)MergeInTranslationForm(m,x=hello|)");
		}

		/*
		 *
		/// <summary>
		/// when I wrote the flex exporter, lift did not yet implement semantic domain
		/// </summary>
		[Test, Ignore("Not yet implemented in WeSay")]
		public void SemanticDomainTraitIsBroughtInCorrectly()
		{
			_doc.LoadXml("<trait range=\"semantic-domain\" value=\"6.5.1.1\"/>");
			//TODO   _importer.ReadTrait(_doc.SelectSingleNode("wrap"));
		}

		/// <summary>
		/// when I wrote the flex exporter, lift did not yet implement part of speech
		/// </summary>
		[Test, Ignore("Not yet implemented in WeSay")]
		public void GrammiWithTextLabel()
		{
			_doc.LoadXml("<sense><grammi type=\"conc\"/></sense>");
			//TODO   _importer.ReadSense(_doc.SelectSingleNode("sense"));
		}

		/// <summary>
		/// when I wrote the flex exporter, lift did not yet implement part of speech
		/// </summary>
		[Test, Ignore("Not yet implemented in WeSay")]
		public void GrammiWithEmptyLabel()
		{
			_doc.LoadXml("<sense><grammi type=\"\"/></sense>");
			//TODO   _importer.ReadSense(_doc.SelectSingleNode("sense"));
		}


		 * */

		private void ParseAndCheck(string content, string expectedResults)
		{
			_doc.LoadXml(content);
			_parser.ReadFile(_doc);
			Assert.AreEqual(expectedResults, _results.ToString());
		}

//        private void ParseEntryAndCheck(string content, string expectedResults)
//        {
//            _doc.LoadXml(content);
//            _parser.ReadEntry(_doc.FirstChild);
//            Assert.AreEqual(expectedResults, _results.ToString());
//        }
	}

	public class Mute
	{
		public override string ToString()
		{
			return "m";
		}
	}
/*
	class TestLiftMerger : ILexiconMerger<Mute, Mute, Mute>
	{
		public StringBuilder _results;

		public TestLiftMerger(StringBuilder results)
		{
			_results = results;
		}

		public Mute GetOrMakeEntry(IdentifyingInfo idInfo)
		{
			_results.AppendFormat("GetOrMakeEntry({0})",idInfo);
			return new Mute();
		}

		public void MergeInLexemeForm(Mute entry, SimpleMultiText forms)
		{
			_results.AppendFormat("MergeInLexemeForm({0},{1})", entry, GetStingFromMultiText(forms));
	   }

		private static string GetStingFromMultiText(SimpleMultiText forms)
		{
			string s="";
			foreach (string key in forms.Keys)
			{
				s += string.Format("{0}={1}|", key, forms[key]);
			}
			return s;
		}

		public Mute GetOrMakeSense(Mute entry, IdentifyingInfo idInfo)
		{
			_results.AppendFormat("GetOrMakeSense({0},{1})", entry, idInfo);
			return new Mute();
		}

		public Mute GetOrMakeExample(Mute sense, IdentifyingInfo idInfo)
		{
			_results.AppendFormat("GetOrMakeExample({0},{1})", sense, idInfo);
			return new Mute();
		}


		public void MergeInGloss(Mute sense, SimpleMultiText forms)
		{
			_results.AppendFormat("MergeInGloss({0},{1})", sense, GetStingFromMultiText(forms));
		}

		public void MergeInExampleForm(Mute example, SimpleMultiText forms)
		{
			_results.AppendFormat("MergeInExampleForm({0},{1})", example, GetStingFromMultiText(forms));
		}

		public void MergeInTranslationForm(Mute example, SimpleMultiText forms)
		{
			_results.AppendFormat("MergeInTranslationForm({0},{1})", example, GetStingFromMultiText(forms));
		}
	}*/

}