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
			 if (DetailList.ShowField("Sentence"))
			 {
				 DetailList.AddWidgetRow(StringCatalog.Get("Example"), false, MakeBoundEntry(example.Sentence, BasilProject.Project.WritingSystems.VernacularWritingSystemDefault), insertAtRow);
				 ++rowCount;
			 }
			 if (DetailList.ShowField("Translation"))
			 {
				 DetailList.AddWidgetRow(StringCatalog.Get("Translation"), false, MakeBoundEntry(example.Translation, BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault),insertAtRow+1 );
				 ++rowCount;
			 }
			return rowCount;
		}

		public int AddGhost(System.ComponentModel.IBindingList list, int insertAtRow)
		{
			int rowCount = 0;
			if (DetailList.ShowField("GhostSentence"))
			{
				WeSayTextBox entry = new WeSayTextBox(BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault);
				GhostBinding g = MakeGhostBinding(list, "Sentence", BasilProject.Project.WritingSystems.VernacularWritingSystemDefault, entry);
				g.ReferenceControl = DetailList.AddWidgetRow(StringCatalog.Get("Example"), false, entry, insertAtRow);
				// entry.PrepareForFadeIn();
				++rowCount;
			}
			return rowCount;
		}
	}
}
