using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.UI;
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

		public override int AddWidgets(object dataObject)
		{
			return AddWidgets(dataObject, -1);
		}

		internal override int AddWidgets(object dataObject, int insertAtRow)
		{
			 int rowCount = 2;
			 LexExampleSentence example = (LexExampleSentence)dataObject;
			 _detailList.AddWidgetRow("Example", false, MakeBoundEntry(example.Sentence, BasilProject.Project.VernacularWritingSystemDefault), insertAtRow);
			_detailList.AddWidgetRow("Translation", false, MakeBoundEntry(example.Translation, BasilProject.Project.AnalysisWritingSystemDefault),insertAtRow+1 );

			return rowCount;
		}

		public int AddGhost(System.ComponentModel.IBindingList list, int insertAtRow)
		{
			WeSayTextBox entry = new WeSayTextBox(BasilProject.Project.AnalysisWritingSystemDefault);
			GhostBinding g = MakeGhostBinding(list, "Sentence", BasilProject.Project.VernacularWritingSystemDefault, entry);
			g.ReferenceControl = _detailList.AddWidgetRow("Example", false, entry, insertAtRow);
		   // entry.PrepareForFadeIn();
			return 1;
		}
	}
}
