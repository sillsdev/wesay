using System;
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
	public class LexExampleSentenceLayouter : Layouter
	{
		public LexExampleSentenceLayouter(DetailList  builder)
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
			 LexExampleSentence example = (LexExampleSentence)list[index];
			 if (_detailList.ShowField("Sentence"))
			 {
				 _detailList.AddWidgetRow(StringCatalog.Get("Example"), false, MakeBoundEntry(example.Sentence, BasilProject.Project.VernacularWritingSystemDefault), insertAtRow);
				 ++rowCount;
			 }
			 if (_detailList.ShowField("Translation"))
			 {
				 _detailList.AddWidgetRow(StringCatalog.Get("Translation"), false, MakeBoundEntry(example.Translation, BasilProject.Project.AnalysisWritingSystemDefault),insertAtRow+1 );
				 ++rowCount;
			 }
			return rowCount;
		}

		public int AddGhost(System.ComponentModel.IBindingList list, int insertAtRow)
		{
			int rowCount = 0;
			if (_detailList.ShowField("GhostSentence"))
			{
				WeSayTextBox entry = new WeSayTextBox(BasilProject.Project.AnalysisWritingSystemDefault);
				GhostBinding g = MakeGhostBinding(list, "Sentence", BasilProject.Project.VernacularWritingSystemDefault, entry);
				g.ReferenceControl = _detailList.AddWidgetRow(StringCatalog.Get("Example"), false, entry, insertAtRow);
				// entry.PrepareForFadeIn();
				++rowCount;
			}
			return rowCount;
		}
	}
}
