using System.Windows.Forms;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;
using System.ComponentModel;
namespace WeSay.LexicalTools
{
	/// <summary>
	/// <see cref="Layouter"/>
	/// </summary>
	public class LexExampleSentenceLayouter : Layouter
	{
		public LexExampleSentenceLayouter(DetailList  builder, ViewTemplate viewTemplate)
			: base(builder, viewTemplate)
		{
		}

		internal override int AddWidgets(IBindingList list, int index, int insertAtRow)
		{
			DetailList.SuspendLayout();
			int rowCount = 0;
			LexExampleSentence example = (LexExampleSentence)list[index];

			Field field = ActiveViewTemplate.GetField(Field.FieldNames.ExampleSentence.ToString());
			if (field != null && field.DoShow )
			{
				Control entry = MakeBoundControl(example.Sentence, field);
				DetailList.AddWidgetRow(StringCatalog.Get("Example"), false, entry, insertAtRow+rowCount, false);
				++rowCount;
			}

			field = ActiveViewTemplate.GetField(Field.FieldNames.ExampleTranslation.ToString());
			if (field != null && field.DoShow)
			{
				 Control entry = MakeBoundControl(example.Translation, field);
				DetailList.AddWidgetRow(StringCatalog.Get("Translation"), false, entry, insertAtRow+rowCount, false);
				++rowCount;
			}

			rowCount += AddCustomFields(example, insertAtRow + rowCount);
			DetailList.ResumeLayout(false);
			return rowCount;
		}

		public int AddGhost(System.ComponentModel.IBindingList list, int insertAtRow)
		{
			return MakeGhostWidget<LexExampleSentence>(list, insertAtRow, Field.FieldNames.ExampleSentence.ToString(), "Example", "Sentence", false);
		}


	}
}
