using System.Windows.Forms;
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

		public int AddWidgets(object dataObject, int insertAtRow)
		{
			 int rowCount = 2;
			 LexExampleSentence example = (LexExampleSentence)dataObject;
			_detailList.AddWidgetRow("Example", false, MakeBoundEntry(example.Sentence, "th"),insertAtRow );
			_detailList.AddWidgetRow("Translation", false, MakeBoundEntry(example.Translation, "en"),insertAtRow+1 );

			return rowCount;
		}

		public int AddGhost(System.ComponentModel.IBindingList list, int insertAtRow)
		{
			TextBox entry = new TextBox();
			GhostBinding g = MakeGhostBinding(list, "Sentence", "th", entry);
			g.ReferenceControl = _detailList.AddWidgetRow("Example", false, entry, insertAtRow);
			return 1;
		}

		protected override void OnGhostBindingTriggered(GhostBinding sender, object newDataTarget, System.EventArgs args)
		{
			AddWidgets(newDataTarget, sender.ReferenceControl);
		}

		private void AddWidgets(object dataObject, Control refControl)
		{
			int row = _detailList.GetRowOfControl(refControl);
			AddWidgets(dataObject, row);
		}

	}
}
