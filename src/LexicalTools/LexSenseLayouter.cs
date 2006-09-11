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


		public override int AddWidgets(IBindingList list, int index)
		{
			return AddWidgets(list, index, -1);
		}

		internal override int AddWidgets(IBindingList list, int index, int insertAtRow)
		{
			 int rowCount = 0;
		   LexSense sense = (LexSense)list[index];
		   if (_detailList.ShowField("Gloss"))
		   {
			Control c = _detailList.AddWidgetRow(StringCatalog.Get("Meaning") + " " + (index+1).ToString(), true,MakeBoundEntry(sense.Gloss, BasilProject.Project.AnalysisWritingSystemDefault), insertAtRow);
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
			GhostBinding g= MakeGhostBinding(list, "Gloss", BasilProject.Project.AnalysisWritingSystemDefault, entry);
			g.ReferenceControl = _detailList.AddWidgetRow(StringCatalog.Get("New Meaning"), true, entry);
			++rowCount;
			}
			return rowCount;
		}

		public bool HasSenseWithEmptyGloss(IBindingList list)
		{
			foreach (LexSense sense in list)
			{
				if (sense.Gloss.Count == 0)
				{
					return true;
				}
			}
			return false;
		}

	}
}
