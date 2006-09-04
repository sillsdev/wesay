using System.Windows.Forms;
using WeSay.Language;
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
			 int rowCount = 0;
		   LexSense sense = (LexSense)dataObject;
		   if (_detailList.ShowField("Gloss"))
		   {
			Control c = _detailList.AddWidgetRow("Meaning", true,MakeBoundEntry(sense.Gloss, BasilProject.Project.AnalysisWritingSystemDefault), insertAtRow);
			   ++rowCount;
			   insertAtRow = _detailList.GetRowOfControl(c);
		   }
		   LexExampleSentenceLayouter exampleLayouter = new LexExampleSentenceLayouter(_detailList);

			rowCount = AddChildrenWidgets(exampleLayouter, sense.ExampleSentences, insertAtRow, rowCount);

			//add a ghost
			rowCount += exampleLayouter.AddGhost(sense.ExampleSentences, insertAtRow+rowCount);

			return rowCount;
		}

		public int AddGhost(IBindingList list)
		{
			int rowCount = 0;
			if (_detailList.ShowField("GhostGloss"))
			{
			WeSayTextBox entry = new WeSayTextBox(BasilProject.Project.AnalysisWritingSystemDefault);
			GhostBinding g=   MakeGhostBinding(list, "Gloss", BasilProject.Project.AnalysisWritingSystemDefault, entry);
				g.ReferenceControl = _detailList.AddWidgetRow("Meaning", true, entry);
				++rowCount;
			}
			return rowCount;
		}


	}
}
