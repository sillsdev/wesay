using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using Palaso.TestUtilities;
using WeSay.Project.ConfigMigration.WeSayConfig;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class ConfigFileTests
	{
		[Test]
		public void ChangeWritingSystemId_IdExists_IsChanged()
		{
			WeSayWordsProject.InitializeForTests();
			string pathToConfigFile = Path.GetTempFileName();
			File.WriteAllText(pathToConfigFile,
								  GetV7ConfigFileContent());
			ConfigFile configFile = new ConfigFile(pathToConfigFile);
			configFile.ReplaceWritingSystemId("hi-u", "hi-Zxxx-x-audio");
			string newFileContent = File.ReadAllText(pathToConfigFile);
			Assert.IsFalse(newFileContent.Contains("hi-u"));
			Assert.IsTrue(newFileContent.Contains("hi-Zxxx-x-audio"));
		}

		[Test]
		public void ChangeWritingSystemId_DoesnotExist_NoChange()
		{
			WeSayWordsProject.InitializeForTests();
			string pathToConfigFile = Path.GetTempFileName();
			File.WriteAllText(pathToConfigFile,
								  GetV7ConfigFileContent());
			ConfigFile configFile = new ConfigFile(pathToConfigFile);
			configFile.ReplaceWritingSystemId("hi-up", "hi-Zxxx-x-audio");
			string newFileContent = File.ReadAllText(pathToConfigFile);
			Assert.IsFalse(newFileContent.Contains("hi-Zxxx-x-audio"));
		}

		[Test]
		public void DefaultConfigFile_DoesntNeedMigrating()
		{
			XmlDocument configFile = new XmlDocument();
			configFile.Load(WeSayWordsProject.PathToDefaultConfig);
			XmlNode versionNode = configFile.SelectSingleNode("configuration/@version");
			Assert.AreEqual(ConfigFile.LatestVersion, Convert.ToInt32(versionNode.Value));
		}

		private string GetV7ConfigFileContent()
		{
			return
				@"<?xml version='1.0' encoding='utf-8'?>
<configuration version='7'>
  <components>
	<viewTemplate>
	  <fields>
		<field>
		  <className>LexEntry</className>
		  <dataType>MultiText</dataType>
		  <displayName>Word</displayName>
		  <enabled>True</enabled>
		  <fieldName>EntryLexicalForm</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>False</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>Visible</visibility>
		  <writingSystems>
			<id>hi-u</id>
			<id>tap-Zxxx-x-audio</id>
			<id>lalaa</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexEntry</className>
		  <dataType>MultiText</dataType>
		  <displayName>Citation Form</displayName>
		  <enabled>False</enabled>
		  <fieldName>citation</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>False</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>hi-u</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexSense</className>
		  <dataType>MultiText</dataType>
		  <displayName>Definition (Meaning)</displayName>
		  <enabled>True</enabled>
		  <fieldName>definition</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>True</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>Visible</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexSense</className>
		  <dataType>MultiText</dataType>
		  <displayName>Gloss</displayName>
		  <enabled>False</enabled>
		  <fieldName>gloss</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>True</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexEntry</className>
		  <dataType>MultiText</dataType>
		  <displayName>Literal Meaning</displayName>
		  <enabled>False</enabled>
		  <fieldName>literal-meaning</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>True</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>PalasoDataObject</className>
		  <dataType>MultiText</dataType>
		  <displayName>Note</displayName>
		  <enabled>True</enabled>
		  <fieldName>note</fieldName>
		  <multiParagraph>True</multiParagraph>
		  <spellCheckingEnabled>True</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexSense</className>
		  <dataType>Picture</dataType>
		  <displayName>Picture</displayName>
		  <enabled>True</enabled>
		  <fieldName>Picture</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>False</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexSense</className>
		  <dataType>Option</dataType>
		  <displayName>PartOfSpeech</displayName>
		  <enabled>True</enabled>
		  <fieldName>POS</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>False</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <optionsListFile>PartsOfSpeech.xml</optionsListFile>
		  <visibility>Visible</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexExampleSentence</className>
		  <dataType>MultiText</dataType>
		  <displayName>Example Sentence</displayName>
		  <enabled>True</enabled>
		  <fieldName>ExampleSentence</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>True</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>Visible</visibility>
		  <writingSystems>
			<id>hi-u</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexExampleSentence</className>
		  <dataType>MultiText</dataType>
		  <displayName>Example Translation</displayName>
		  <enabled>False</enabled>
		  <fieldName>ExampleTranslation</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>True</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>Visible</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexSense</className>
		  <dataType>OptionCollection</dataType>
		  <displayName>Sem Dom</displayName>
		  <enabled>True</enabled>
		  <fieldName>semantic-domain-ddp4</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>False</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <optionsListFile>Ddp4.xml</optionsListFile>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>en</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexEntry</className>
		  <dataType>RelationToOneEntry</dataType>
		  <displayName>Base Form</displayName>
		  <enabled>False</enabled>
		  <fieldName>BaseForm</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>False</spellCheckingEnabled>
		  <multiplicity>ZeroOr1</multiplicity>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>hi-u</id>
		  </writingSystems>
		</field>
		<field>
		  <className>LexEntry</className>
		  <dataType>RelationToOneEntry</dataType>
		  <displayName>Cross Reference</displayName>
		  <enabled>False</enabled>
		  <fieldName>confer</fieldName>
		  <multiParagraph>False</multiParagraph>
		  <spellCheckingEnabled>False</spellCheckingEnabled>
		  <multiplicity>ZeroOrMore</multiplicity>
		  <visibility>NormallyHidden</visibility>
		  <writingSystems>
			<id>hi-u</id>
		  </writingSystems>
		</field>
	  </fields>
	  <id>Default View Template</id>
	</viewTemplate>
  </components>
  <tasks>
	<task taskName='Dashboard' visible='true' />
	<task taskName='Dictionary' visible='true' />
	<task taskName='AddMissingInfo' visible='false'>
	  <label>Meanings</label>
	  <longLabel>Add Meanings</longLabel>
	  <description>Add meanings (senses) to entries where they are missing.</description>
	  <field>definition</field>
	  <showFields>definition</showFields>
	  <readOnly>semantic-domain-ddp4</readOnly>
	  <writingSystemsToMatch />
	  <writingSystemsWhichAreRequired />
	</task>
	<task taskName='AddMissingInfo' visible='false'>
	  <label>Parts of Speech</label>
	  <longLabel>Add Parts of Speech</longLabel>
	  <description>Add parts of speech to senses where they are missing.</description>
	  <field>POS</field>
	  <showFields>POS</showFields>
	  <readOnly>definition, ExampleSentence</readOnly>
	  <writingSystemsToMatch />
	  <writingSystemsWhichAreRequired />
	</task>
	<task taskName='AddMissingInfo' visible='false'>
	  <label>Example Sentences</label>
	  <longLabel>Add Example Sentences</longLabel>
	  <description>Add example sentences to senses where they are missing.</description>
	  <field>ExampleSentence</field>
	  <showFields>ExampleSentence</showFields>
	  <readOnly>definition</readOnly>
	  <writingSystemsToMatch />
	  <writingSystemsWhichAreRequired />
	</task>
	<task taskName='AddMissingInfo' visible='false'>
	  <label>Base Forms</label>
	  <longLabel>Add Base Forms</longLabel>
	  <description>Identify the 'base form' word that this word is built from. In the printed dictionary, the derived or variant words can optionally be shown as subentries of their base forms.</description>
	  <field>BaseForm</field>
	  <showFields>BaseForm</showFields>
	  <readOnly />
	  <writingSystemsToMatch />
	  <writingSystemsWhichAreRequired />
	</task>
	<task taskName='AdvancedHistory' visible='false' />
	<task taskName='NotesBrowser' visible='false' />
	<task taskName='GatherWordList' visible='false'>
	  <wordListFileName>SILCAWL.lift</wordListFileName>
	  <wordListWritingSystemId>en</wordListWritingSystemId>
	</task>
	<task taskName='GatherWordsBySemanticDomains' visible='true'>
	  <semanticDomainsQuestionFileName>Ddp4Questions-en.xml</semanticDomainsQuestionFileName>
	  <showMeaningField>False</showMeaningField>
	</task>
  </tasks>
  <addins>
	<addin id='SendReceiveAction' showInWeSay='True' />
  </addins>
</configuration>"
					.Replace("'", "\"");
		}
	}
}
