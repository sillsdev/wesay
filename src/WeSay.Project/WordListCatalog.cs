using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Project
{
	/// <summary>
	/// this catalog can eventually come by inspecting word lists found in a folder or something
	/// ideally, the files containing the lists would be self describing (and be lift files, with
	/// all the meta data for each entry).
	/// </summary>
	public class WordListCatalog : Dictionary<string, WordListDescription>
	{
		public WordListCatalog()
		{
			Add("SILCAWL.lift", new WordListDescription("en", "SIL CAWL ", "Gather words using the SIL Comparative African Wordlist", "Collect new words by translating from the SIL Comparative African Wordlist. This is a list of 1700 words which includes semantic domains, parts of speech, and french glosses."));
		 //   Add("DuerksenWords.txt", new WordListDescription("en", "Old SILCA Word List", "Gather words using the SIL Comparative African Wordlist", "Collect new words by translating from the SIL Comparative African Wordlist. This is a list of 1700 words."));
		 //   Add("PNGWords.txt", new WordListDescription("en", "PNG Word List", "Gather words using the PNG Word List", "Collect new words by translating from words from this PNG wordlist. This is a list of 900 words and phrases."));
		}

		public WordListDescription GetOrAddWordList(string fileName)
		{
			if (!this.ContainsKey(fileName))
			{
				Add(fileName, new WordListDescription("en", fileName, "Custom Word List", string.Format("Words from {0}", fileName)));
			}

			return this[fileName];
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
}
