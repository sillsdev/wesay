using System;
using System.Collections.Generic;
using System.Text;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public class LexSenseLayouter : Layouter
	{
		public LexSenseLayouter(TableBuilder builder)
			: base(builder)
		{
		}

		public override int AddWidgets(object dataObject)
		{
			int rowCount = 1;
			LexSense sense = (LexSense)dataObject;

			_builder.AddWidgetRow("Meaning: ", MakeBoundEntry(sense.Gloss, "en"));

			LexExampleSentenceLayouter exampleLayouter = new LexExampleSentenceLayouter(_builder);
			foreach (LexExampleSentence example in sense.ExampleSentences)
			{
			   rowCount+= exampleLayouter.AddWidgets(example);
			}
		  //  _builder.AddWidgetRow("meaning: ", senseTableBuilder.BuildTable());

			return rowCount;
		}

		public int AddGhost(System.ComponentModel.BindingList<LexSense> list)
		{
			_builder.AddWidgetRow("(Meaning): ", MakeGhostEntry(list, "Gloss", "en"));

			return 1;
		}

//        public override int AddWidgets(object dataObject)
//        {
//            int rowCount = 1;
//            LexSense sense = (LexSense)dataObject;
//
//            TableBuilder senseTableBuilder = new TableBuilder();
//            senseTableBuilder.AddWidgetRow("gloss: ", MakeBoundEntry(sense.Gloss, "en"));
//
//            LexExampleSentenceLayouter exampleLayouter = new LexExampleSentenceLayouter(senseTableBuilder);
//            foreach (LexExampleSentence example in sense.ExampleSentences)
//            {
//                rowCount += exampleLayouter.AddWidgets(example);
//            }
//            _builder.AddWidgetRow("meaning: ", senseTableBuilder.BuildTable());
//
//            return rowCount;
//        }
	}
}
