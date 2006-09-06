using System;
using System.Collections;
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
			int rowCount = 0;
			LexEntry entry = (LexEntry)dataObject;
			if (_detailList.ShowField("LexicalForm"))
			{
				_detailList.AddWidgetRow(StringCatalog.Get("Word"), true, MakeBoundEntry(entry.LexicalForm, BasilProject.Project.VernacularWritingSystemDefault), insertAtRow);
				++rowCount;
			}
			LexSenseLayouter layouter = new LexSenseLayouter(_detailList);
			rowCount = AddChildrenWidgets(layouter, entry.Senses, insertAtRow, rowCount);
			//add a ghost
			rowCount += layouter.AddGhost(entry.Senses);

			return rowCount;
		}
	}
}