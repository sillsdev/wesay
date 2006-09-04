using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	/// <summary>
	/// <see cref="Layouter"/>
	/// </summary>
	public class LexEntryLayouter : Layouter
	{
		public LexEntryLayouter(DetailList builder)
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
			LexEntry entry = (LexEntry)dataObject;

			_detailList.AddWidgetRow("Word", true, MakeBoundEntry(entry.LexicalForm, BasilProject.Project.VernacularWritingSystemDefault), insertAtRow);
			LexSenseLayouter layouter = new LexSenseLayouter(_detailList);
			foreach (LexSense sense in entry.Senses)
			{
				rowCount+= layouter.AddWidgets(sense, insertAtRow + rowCount);
			}
			//add a ghost
			 rowCount+= layouter.AddGhost(entry.Senses);

			return rowCount;
		}
	}
}