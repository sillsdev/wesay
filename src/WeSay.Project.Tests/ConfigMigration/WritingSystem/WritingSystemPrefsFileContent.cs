using System;

namespace WeSay.Project.Tests.ConfigMigration.WritingSystem
{
	public class WritingSystemPrefsFileContent
	{

		public static string GetSingleWritingSystemXmlForLanguage(string language)
		{
			return GetSingleWritingSystemXml(
				language, language, "", "", "Arial", 11, false, language, language, true, false
			);
		}

		public static string GetSingleWritingSystemXml(
			string id,
			string abbreviation,
			string sortUsing,
			string customSortRules,
			string fontName,
			int fontSize,
			bool rightToleft,
			string spellCheckingId,
			string keyboard,
			bool isUnicode,
			bool isAudio
			)
		{
			string sortRulesXml = String.Empty;
			if (!String.IsNullOrEmpty(customSortRules))
			{
				sortRulesXml = String.Format("<CustomSortRules>{0}</CustomSortRules>", customSortRules);
			}
			return String.Format(
				@"<WritingSystem>
	  <Abbreviation>{0}</Abbreviation>
	  {1}
	  <FontName>{2}</FontName>
	  <FontSize>{3}</FontSize>
	  <Id>{4}</Id>
	  <IsAudio>{5}</IsAudio>
	  <IsUnicode>{6}</IsUnicode>
	  <WindowsKeyman>{7}</WindowsKeyman>
	  <RightToLeft>{8}</RightToLeft>
	  <SortUsing>{9}</SortUsing>
	  <SpellCheckingId>{10}</SpellCheckingId>
	</WritingSystem>".Replace("'", "\""),
				abbreviation, sortRulesXml, fontName, fontSize, id, isAudio,
				isUnicode, keyboard, rightToleft, sortUsing, spellCheckingId
				);
		}

		public static string SingleWritingSystem(
			string id,
			string abbreviation,
			string sortUsing,
			string customSortRules,
			string fontName,
			int fontSize,
			bool rightToleft,
			string spellCheckingId,
			string keyboard,
			bool isUnicode,
			bool isAudio
			)
		{
			string singleWritingsystemXml = GetSingleWritingSystemXml(id, abbreviation, sortUsing, customSortRules,
																	  fontName, fontSize, rightToleft, spellCheckingId,
																	  keyboard, isUnicode, isAudio);
			return WrapWritingSystemXmlWithCollectionXml(singleWritingsystemXml);
		}

		public static string WrapWritingSystemXmlWithCollectionXml(string writingSystemXml)
		{
			return "<?xml version='1.0' encoding='utf-8'?><WritingSystemCollection><members>" +
					writingSystemXml +
					"</members></WritingSystemCollection>";

		}

		public static string SingleWritingSystemForLanguage(string language)
		{
			return SingleWritingSystem(
				language, language, "", "", "Arial", 11, false, language, language, true, false
			);
		}



		public static string TwoWritingSystems(string language1, string language2)
		{
			string ws1Xml = GetSingleWritingSystemXml(
				language1, language1, "", "", "Arial", 11, false, language1, language1, true, false
			);
			string ws2Xml = GetSingleWritingSystemXml(
				language2, language2, "", "", "Arial", 11, false, language2, language2, true, false
			);
			return WrapWritingSystemXmlWithCollectionXml(String.Concat(ws1Xml, ws2Xml));
		}
	}
}