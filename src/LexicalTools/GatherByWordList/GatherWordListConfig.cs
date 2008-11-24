using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using WeSay.Project;

namespace WeSay.LexicalTools.GatherByWordList
{
	public interface IGatherWordListConfig : ITaskConfiguration
	{
		string wordListFileName
		{
			get;
		}
		string wordListWritingSystemId
		{
			get;
		}
	}


	public class GatherWordListConfig : TaskConfigurationBase, IGatherWordListConfig, ITaskConfiguration , ICareThatWritingSystemIdChanged
	{
		private readonly WordListCatalog _catalog;

		public GatherWordListConfig(string xml, WordListCatalog catalog)
		{
			_catalog = catalog;
			_xmlDoc = new XmlDocument();
			_xmlDoc.LoadXml(xml);
		}


		public string wordListFileName
		{
			get
			{
				return GetStringFromConfigNode("wordListFileName");
			}
		}
		public string wordListWritingSystemId
		{
			get
			{
				return GetStringFromConfigNode("wordListWritingSystemId");
			}
		}

		private WordListDescription WordList
		{
			get { return _catalog[wordListFileName]; }
		}

		public string Label
		{
			get { return WordList.Label; }
		}

		public string LongLabel
		{
			get { return WordList.LongLabel; }
		}

		public string Description
		{
			get { return WordList.Description; }
		}

		public string RemainingCountText
		{
			get { return "Remaining Words:"; }
		}

		public string ReferenceCountText
		{
			get { return string.Empty; }
		}

		public bool IsPinned
		{
			get { return false; }
		}



		public bool IsOptional
		{
			get { throw new System.NotImplementedException(); }
		}

		public static IGatherWordListConfig CreateForTests(string wordListFileName, string wordListWritingSystemId)
		{
			string x = String.Format(@"   <task taskName='AddMissingInfo' visible='true'>
					  <wordListFileName>{0}</wordListFileName>
					  <wordListWritingSystemId>{1}</wordListWritingSystemId>
					</task>
				", wordListFileName, wordListWritingSystemId);

			return new GatherWordListConfig(x, new WordListCatalog());

		}

		/// <summary>
		/// this catalog can eventually come by inspecting word lists found in a folder or something
		/// ideally, the files containing the lists would be self describing (and be lift files, with
		/// all the meta data for each entry).
		/// </summary>
		public class WordListCatalog : Dictionary<string, WordListDescription >
		{
			public WordListCatalog()
			{
				//SILCAWL
				Add("DuerksenWords.txt", new WordListDescription("en", "SILCA Word List", "Gather words using the SIL Comparative African Wordlist", "Collect new words by translating from words in another language. This is a list of 1700 words."));
				Add("PNGWords.txt", new WordListDescription("en", "PNG Word List", "Gather words using the PNG Word List", "Collect new words by translating from words in another language. This is a list of 900 words and phrases."));
			}
		}

		public class WordListDescription
		{
			public string WritingSystemId { get; set; }
			public string Label { get; set; }
			public string LongLabel { get; set; }
			public string Description { get; set; }

			public WordListDescription(string writingSystemId, string label, string longLabel, string description)
			{
				WritingSystemId = writingSystemId;
				Label = label;
				LongLabel = longLabel;
				Description = description;
			}
		}

		public void WritingSystemIdChanged(string from, string to)
		{
			  //TODO, when we become writeable
		}
	}
}