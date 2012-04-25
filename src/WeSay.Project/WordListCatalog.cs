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
			//enhance: with wordpacks, we could/should now pull this stuff out of the wordpack folder,
			//and remove this hard-coding.  This catalog could just list the factory wordpacks (or
			//we could enumerate a folder which contians them.
			Add("SILCAWL", new WordListDescription("en", "CAWL Word List", "SIL-CAWL Word List", "Collect new words by translating from the SIL Comparative African Wordlist. This is a list of 1700 words which includes semantic domains, parts of speech, and glosses in several languages. You can use one of several languages for prompting: French, Portuguese, Swahili, Hausa, and more."
				+ Environment.NewLine + Environment.NewLine
				+"WeSay will use the top-most writing system of the definition field to choose the prompting language (only English and French are available at this time)."));
			Add("SILCAWL-MozambiqueAddendum", new WordListDescription("en", "CAWL-MOZ", "SIL-CAWL-Moz", "Collect new words by translating from this WordPack, which is a 42 word addendum to SIL-CAWL, used by SIL Mozambique. You can use English or Portuguese as the prompting language."));
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
