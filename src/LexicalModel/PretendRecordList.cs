namespace WeSay.LexicalModel.Tests
{
	public class PretendRecordList : WeSay.Data.InMemoryBindingList<LexEntry>
	{
		public PretendRecordList()
			: base()
		{
			LexEntry entry = AddNew();
			for (int i = 0; i < 4; ++i)
			{
				entry.LexicalForm["th"] = "apple " + i.ToString();
				LexSense sense = (LexSense) entry.Senses.AddNew();
				sense.Gloss["en"] = "red thing " + i.ToString();
				if ((i % 3)==0)
				{
					LexExampleSentence example = (LexExampleSentence) sense.ExampleSentences.AddNew();
					example.Sentence["th"] = "An apple a day keeps the doctor away.";
				}
				if ((i % 4)==0)
				{
					sense = (LexSense) entry.Senses.AddNew();
					sense.Gloss["en"] = "computer brand";
				}

			}
			entry = AddNew();
			entry.LexicalForm["th"] = "orange";
		}
	}
}
