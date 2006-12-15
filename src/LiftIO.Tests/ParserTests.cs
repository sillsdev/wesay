using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using NUnit.Framework;
using System.Xml;

namespace LiftIO.Tests
{
	[TestFixture]
	public class ParserTests
	{
		private TestLiftMerger _merger;
		private LiftParser<object, object, object> _parser;
		private XmlDocument _doc;
	   // private List<Exception> _parsingErrors;
		public StringBuilder _results;


		[SetUp]
		public void Setup()
		{
			//_parsingErrors = new List<Exception>();
			_doc = new XmlDocument();
			_results = new StringBuilder();
			_merger = new TestLiftMerger(_results);
			_parser = new LiftParser<object, object, object>(_merger);
			_parser.ParsingError += new EventHandler<LiftParser<object, object,object>.ErrorArgs>(OnParsingError);
		}

		void OnParsingError(object sender, LiftParser<object, object, object>.ErrorArgs e)
		{
			_results.AppendFormat("Error");
			//_parsingErrors.Add(e._exception);
		}

		[TearDown]
		public void TearDown()
		{


		}


		[Test]
		public void EmptyLiftOk()
		{
			ParseAndCheck("<lift/>","");
		}

		[Test]
		public void EntryMissingIdNonFatal()
		{
			ParseAndCheck("<lift><entry/></lift>", "GetOrMakeEntry(;;;)");
		}


		[Test]
		public void EmptyEntriesOk()
		{
			ParseAndCheck("<lift><entry/><entry/></lift>", "GetOrMakeEntry(;;;)GetOrMakeEntry(;;;)");
		}

		[Test]
		public void EntryWithId()
		{
			Guid g = Guid.NewGuid();
			ParseEntryAndCheck(string.Format("<entry id=\"{0}\" />", g.ToString()),
				string.Format("GetOrMakeEntry({0};;;)", g.ToString()));
		}

		[Test]
		public void EntryWithoutId()
		{
			ParseEntryAndCheck("<entry/>", "GetOrMakeEntry(;;;)");
		}

		[Test]
		public void FormMissingLangGeneratesNonFatalError()
		{
			ParseEntryAndCheck("<entry><lex><form/></lex></entry>", "GetOrMakeEntry(;;;)Error");
		}


		[Test]
		public void EmptyFormOk()
		{
			ParseEntryAndCheck("<entry><lex><form lang='x'/></lex></entry>", "GetOrMakeEntry(;;;)MergeInLexemeForm(,x=|)");
			//Assert.AreEqual(1, _parsingErrors.Count);
		}

		[Test]
		public void EntryWithLexemeForm()
		{
			ParseEntryAndCheck("<entry><lex><form lang='x'>hello</form><form lang='y'>bye</form></lex></entry>", "GetOrMakeEntry(;;;)MergeInLexemeForm(,x=hello|y=bye|)");
		}

		[Test]
		public void EntryWithDates()
		{
			string created = "2003-08-07T08:42:42+07:00";
			string mod ="2005-01-01T01:11:11+01:00";
			ParseEntryAndCheck(string.Format("<entry id='foo' dateCreated='{0}' dateModified='{1}'></entry>", created, mod),
				String.Format("GetOrMakeEntry(foo;{0};{1};)", created,mod));
		}

		[Test]
		public void EntryWithSense()
		{
			ParseEntryAndCheck(string.Format("<entry><sense></sense></entry>"),
				"GetOrMakeEntry(;;;)GetOrMergeSense(,)");
		}

		[Test]
		public void SenseWithGloss()
		{
			ParseEntryAndCheck(string.Format("<entry><sense><gloss><form lang='x'>hello</form></gloss></sense></entry>"),
				"GetOrMakeEntry(;;;)GetOrMergeSense(,)MergeInGloss(,x=hello|)");
		}
		[Test]
		public void SenseWithDefintition()
		{
			ParseEntryAndCheck(string.Format("<entry><sense><def><form lang='x'>hello</form></def></sense></entry>"),
				"GetOrMakeEntry(;;;)GetOrMergeSense(,)MergeInDefinition(,x=hello|)");
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
				"GetOrMakeEntry(;;;)GetOrMergeSense(,)");
		}

		[Test, Ignore("Not implemented")]
		public void SenseWithEmptyGrammi()
		{
			ParseEntryAndCheck(string.Format("<entry><sense><grammi></grammi></sense></entry>"),
				"GetOrMakeEntry(;;;)GetOrMergeSense(,)");
		}

		[Test]
		public void SenseWithExample()
		{
			ParseEntryAndCheck(string.Format("<entry><sense><example><form lang='x'>hello</form></example></sense></entry>"),
				"GetOrMakeEntry(;;;)GetOrMergeSense(,)GetOrMergeExample(,)MergeInExampleForm(,x=hello|)");
		}

		[Test]
		public void ExampleWithTranslation()
		{
			ParseEntryAndCheck(string.Format("<entry><sense><example><translation><form lang='x'>hello</form></translation></example></sense></entry>"),
				"GetOrMakeEntry(;;;)GetOrMergeSense(,)GetOrMergeExample(,)MergeInTranslationForm(,x=hello|)");
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

		private void ParseEntryAndCheck(string content, string expectedResults)
		{
			_doc.LoadXml(content);
			_parser.ReadEntry(_doc.FirstChild);
			Assert.AreEqual(expectedResults, _results.ToString());
		}
	}

	class TestLiftMerger : ILexiconMerger<object, object, object>
	{
		public StringBuilder _results;

		public TestLiftMerger(StringBuilder results)
		{
			_results = results;
		}

		public object GetOrMakeEntry(IdentifyingInfo idInfo)
		{
			_results.AppendFormat("GetOrMakeEntry({0})",idInfo);
			return null;
		}

		public void MergeInLexemeForm(object entry, StringDictionary forms)
		{
			_results.AppendFormat("MergeInLexemeForm({0},{1})", entry, GetStingFromMultiText(forms));
	   }

		private static string GetStingFromMultiText(StringDictionary forms)
		{
			string s="";
			foreach (string key in forms.Keys)
			{
				s += string.Format("{0}={1}|", key, forms[key]);
			}
			return s;
		}

		public object GetOrMergeSense(object entry, IdentifyingInfo idInfo)
		{
			_results.AppendFormat("GetOrMergeSense({0},{1})", entry, idInfo);
			return null;
		}

		public object GetOrMergeExample(object sense, IdentifyingInfo idInfo)
		{
			_results.AppendFormat("GetOrMergeExample({0},{1})", sense, idInfo);
			return null;
		}


		public void MergeInGloss(object sense, StringDictionary forms)
		{
			_results.AppendFormat("MergeInGloss({0},{1})", sense, GetStingFromMultiText(forms));
		}

		public void MergeInExampleForm(object example, StringDictionary forms)
		{
			_results.AppendFormat("MergeInExampleForm({0},{1})", example, GetStingFromMultiText(forms));
		}

		public void MergeInTranslationForm(object example, StringDictionary forms)
		{
			_results.AppendFormat("MergeInTranslationForm({0},{1})", example, GetStingFromMultiText(forms));
		}
	}

}