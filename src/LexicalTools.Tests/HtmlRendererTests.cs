
using System;
using NUnit.Framework;

using WeSay.Project;
using WeSay.UI;
using WeSay.LexicalModel;
using Palaso.DictionaryServices.Model;
using Palaso.TestUtilities;

namespace WeSay.LexicalTools.Tests
{


	[TestFixture]
	public class HtmlRendererTests
	{
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			WeSayWordsProject.InitializeForTests();

		}

		[Test]
		public void TestCase()
		{
			string lexicalForm = "test";
			string definition = "definition";
			string exampleSentence = "Test sentence";
			string exampleTranslation = "Translated sentence";
			LexEntryRepository lexEntryRepository = (LexEntryRepository)
				WeSayWordsProject.Project.ServiceLocator.GetService(typeof(LexEntryRepository));
			ViewTemplate viewTemplate = (ViewTemplate)
						WeSayWordsProject.Project.ServiceLocator.GetService(typeof(ViewTemplate));

			string wsA = viewTemplate.GetHeadwordWritingSystemIds()[0];
			string wsB = viewTemplate.GetDefaultWritingSystemForField("Definition").Id;

			LexEntry entry = lexEntryRepository.CreateItem();
			entry.LexicalForm[wsA] = lexicalForm;
			LexSense sense = new LexSense();
			sense.Definition[wsB] = definition;
			LexExampleSentence example = new LexExampleSentence();
			example.Sentence[wsA] = exampleSentence;
			example.Translation[wsB] = exampleTranslation;
			sense.ExampleSentences.Add(example);
			entry.Senses.Add(sense);

			CurrentItemEventArgs currentItem = null;

			string html = HtmlRenderer.ToHtml(entry, currentItem, lexEntryRepository,  viewTemplate);

			Assert.IsTrue(html.Contains(lexicalForm));
			Assert.IsTrue(html.Contains(exampleSentence));
			Assert.IsTrue(html.Contains(exampleTranslation));
			Assert.IsTrue(html.Contains(definition));
			AssertThatXmlIn.String(html).HasSpecifiedNumberOfMatchesForXpath("/html/body", 1);
			AssertThatXmlIn.String(html).HasAtLeastOneMatchForXpath("//@lang");
		}
	}
}
