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
			entry.LexicalForm["en"] = "apple";
			LexSense sense =entry.Senses.AddNew();
			sense.Gloss["en"] = "red thing";
			LexExampleSentence example = sense.ExampleSentences.AddNew();
			example.Sentence["en"] = "An apple a day keeps the doctor away.";
			 sense =entry.Senses.AddNew();
			sense.Gloss["en"] = "computer brand";

			 entry = this.AddNew();
			entry.LexicalForm["en"] = "orange";
		}
	}
}
