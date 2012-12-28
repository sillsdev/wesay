using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeSay.TestUtilities
{
	public class ConfigFileContentForTests
	{
		public static string GetConfigFileSnippetContainingFieldWithWritingSystems(string writingSystemLabel1, string writingSystemLabel2)
		{
			return String.Format(
@"<components>
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
					<id>{0}</id>
					<id>{1}</id>
				</writingSystems>
			</field>
		</fields>
		<id>Default View Template</id>
	</viewTemplate>
</components>".Replace("'", "\""), writingSystemLabel1, writingSystemLabel2);
		}

		public static string GetConfigFileSnippetContainingMissingInfoTaskWithWritingSystems(string writingSystemLabelToMatch1, string writingSystemLabelToMatch2, string requiredWritingSystemLabel1, string requiredWritingSystemLabel2)
		{
			return String.Format(
@"<tasks>
	<task
		taskName='Dashboard'
		visible='true' />
	<task
		taskName='Dictionary'
		visible='true' />
	<task
		taskName='AddMissingInfo'
		visible='true'>
		<label>Meanings</label>
		<longLabel>Add Meanings</longLabel>
		<description>Add meanings (senses) to entries where they are missing.</description>
		<field>definition</field>
		<showFields>definition</showFields>
		<readOnly>semantic-domain-ddp4</readOnly>
		<writingSystemsToMatch>{0}, {1}</writingSystemsToMatch>
		<writingSystemsWhichAreRequired>{2}, {3}</writingSystemsWhichAreRequired>
	</task>
</tasks>".Replace("'", "\""), writingSystemLabelToMatch1, writingSystemLabelToMatch2, requiredWritingSystemLabel1, requiredWritingSystemLabel2);
		}

		public static string GetConfigFileContainingSfmExporterAddinWithWritingSystems(string englishWritingSystemLabel,
																				string nationalWritingSystemLabel,
																				string regionalWritingSystemLabel,
																				string vernacularWritingSystemLabel)
		{
			return String.Format(
@"<addins>
	<addin
		id='Export To SFM'
		showInWeSay='True'>
		<SfmTransformSettings>
			<SfmTagConversions />
			<EnglishLanguageWritingSystemId>{0}</EnglishLanguageWritingSystemId>
			<NationalLanguageWritingSystemId>{1}</NationalLanguageWritingSystemId>
			<RegionalLanguageWritingSystemId>{2}</RegionalLanguageWritingSystemId>
			<VernacularLanguageWritingSystemId>{3}</VernacularLanguageWritingSystemId>
		</SfmTransformSettings>
	</addin>
</addins>".Replace("'", "\""), englishWritingSystemLabel, nationalWritingSystemLabel, regionalWritingSystemLabel, vernacularWritingSystemLabel);
		}

		public static string WrapContentInConfigurationTags(string contentToWrap)
		{
			return
			"<?xml version='1.0' encoding='utf-8'?><configuration version='8'>" + Environment.NewLine +
				contentToWrap + Environment.NewLine +
			"</configuration>";
		}

		public static string GetCompleteV8ConfigFile(string wsId1, string wsId2, string wsId3)
		{
			return WrapContentInConfigurationTags(GetConfigFileSnippetContainingFieldWithWritingSystems(wsId1, wsId2) +
								  GetConfigFileSnippetContainingMissingInfoTaskWithWritingSystems(wsId1, wsId2, wsId3, wsId1));
		}
	}
}
