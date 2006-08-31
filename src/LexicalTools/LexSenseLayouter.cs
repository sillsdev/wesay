using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.UI;
using System.ComponentModel;

namespace WeSay.LexicalTools
{
	/// <summary>
	/// <see cref="Layouter"/>
	/// </summary>
	public class LexSenseLayouter : Layouter
	{
		public LexSenseLayouter(DetailList builder)
			: base(builder)
		{
		}

		public override int AddWidgets(object dataObject)
		{
			return AddWidgets(dataObject, -1);
		}

		internal override int AddWidgets(object dataObject, int insertAtRow)
		{
			 int rowCount = 1;
		   LexSense sense = (LexSense)dataObject;

			Control c = _detailList.AddWidgetRow("Meaning", true,MakeBoundEntry(sense.Gloss, "en"), insertAtRow);
			insertAtRow = _detailList.GetRowOfControl(c);

			LexExampleSentenceLayouter exampleLayouter = new LexExampleSentenceLayouter(_detailList);
			foreach (LexExampleSentence example in sense.ExampleSentences)
			{
				rowCount += exampleLayouter.AddWidgets(example, insertAtRow + rowCount);
			}

			//add a ghost
			rowCount += exampleLayouter.AddGhost(sense.ExampleSentences, insertAtRow+rowCount);

			return rowCount;
		}

		public int AddGhost(IBindingList list)
		{
			WeSayTextBox entry = new WeSayTextBox();
			GhostBinding g=   MakeGhostBinding(list, "Gloss", "en", entry);
			g.ReferenceControl = _detailList.AddWidgetRow("Meaning", true, entry);
			return 1;
		}


	}
}
