using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.UI;
namespace WeSay.LexicalTools
{
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
			_builder.AddWidgetRow("Example", false, MakeBoundEntry(example.Sentence, "th"),insertAtRow );
			_builder.AddWidgetRow("Translation", false, MakeBoundEntry(example.Translation, "en"),insertAtRow+1 );

			return rowCount;
		}

		public int AddGhost(System.ComponentModel.IBindingList list, int insertAtRow)
		{
			TextBox entry = new TextBox();
			GhostBinding g = MakeGhostBinding(list, "Sentence", "th", entry);
			g.ReferenceControl = _builder.AddWidgetRow("Example", false, entry, insertAtRow);
			return 1;
		}

		protected override void OnGhostBindingTriggered(GhostBinding sender, object newGuy, System.EventArgs args)
		{
			AddWidgets(newGuy, sender.ReferenceControl);
		}

		private void AddWidgets(object dataObject, Control refControl)
		{
			int row = _builder.GetRowOfControl(refControl);
			AddWidgets(dataObject, row);
		}

	}
}
