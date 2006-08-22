using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.LexicalModel.Tests
{
	public class PretendRecordList : System.ComponentModel.BindingList<LexEntry>
	{
		public PretendRecordList()
			: base()
		{
			LexEntry entry = this.AddNew();
			for (int i = 0; i < 200; ++i)
			{
				entry.LexicalForm["en"] = "apple " + i.ToString();
				LexSense sense = entry.Senses.AddNew();
				sense.Gloss["en"] = "red thing " + i.ToString();
				if ((i % 3)==0)
				{
					LexExampleSentence example = sense.ExampleSentences.AddNew();
					example.Sentence["en"] = "An apple a day keeps the doctor away.";
				}
				if ((i % 4)==0)
				{
					sense = entry.Senses.AddNew();
					sense.Gloss["en"] = "computer brand";
				}

			}
			entry = this.AddNew();
			entry.LexicalForm["en"] = "orange";
		}
	}
}
