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
		   if (DetailList.ShowField("Gloss"))
		   {
			Control c = DetailList.AddWidgetRow(StringCatalog.Get("Meaning") + " " + (index+1).ToString(), true,MakeBoundEntry(sense.Gloss, BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault), insertAtRow);
			   ++rowCount;
			   insertAtRow = DetailList.GetRowOfControl(c);
		   }
		   LexExampleSentenceLayouter exampleLayouter = new LexExampleSentenceLayouter(DetailList);

			rowCount = AddChildrenWidgets(exampleLayouter, sense.ExampleSentences, insertAtRow, rowCount);

			//add a ghost
			rowCount += exampleLayouter.AddGhost(sense.ExampleSentences, insertAtRow+rowCount);

			return rowCount;
		}

		public int AddGhost(IBindingList list)
		{
			int rowCount = 0;
			if (DetailList.ShowField("GhostGloss"))
			{
			WeSayTextBox entry = new WeSayTextBox(BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault);
			GhostBinding g= MakeGhostBinding(list, "Gloss", BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault, entry);
			g.ReferenceControl = DetailList.AddWidgetRow(StringCatalog.Get("New Meaning"), true, entry);
			++rowCount;
			}
			return rowCount;
		}

		public static bool HasSenseWithEmptyGloss(IBindingList list)
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
