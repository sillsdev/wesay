using System;
using System.Collections.Generic;
using System.Text;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public class LexSenseLayouter : Layouter
	{
		public LexSenseLayouter(DetailList builder)
			: base(builder)
		{
		}

		public override int AddWidgets(object dataObject)
		{
			int rowCount = 1;
			LexSense sense = (LexSense)dataObject;

			_builder.AddWidgetRow("Meaning", true,MakeBoundEntry(sense.Gloss, "en"));

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
			_builder.AddWidgetRow("Meaning", false, MakeGhostEntry(list, "Gloss", "en"));
			return 1;
		}


		protected override void binding_Triggered(object newGuy, System.EventArgs args)
		{
		   // AddWidgets(newGuy);//todo:: insert these at right spot
			_builder.AddLabelRow("test: ");


			//and how to add a new ghost? or can we keep the old one, but clear him out?
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
