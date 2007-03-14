using System.Windows.Forms;
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
			int rowCount = 0;
			LexExampleSentence example = (LexExampleSentence)list[index];

			Field field = ActiveViewTemplate.GetField(Field.FieldNames.ExampleSentence.ToString());
			if (field != null && field.Visibility == Field.VisibilitySetting.Visible)
			{
				Control entry = MakeBoundControl(example.Sentence, field);
				DetailList.AddWidgetRow(StringCatalog.Get("Example"), false, entry, insertAtRow+rowCount);
				++rowCount;
			}

			field = ActiveViewTemplate.GetField(Field.FieldNames.ExampleTranslation.ToString());
			if (field != null && field.Visibility == Field.VisibilitySetting.Visible)
			{
				 Control entry = MakeBoundControl(example.Translation, field);
				DetailList.AddWidgetRow(StringCatalog.Get("Translation"), false, entry, insertAtRow+rowCount);
				++rowCount;
			}

			rowCount += AddCustomFields(example, insertAtRow + rowCount);

			return rowCount;
		}

		public int AddGhost(System.ComponentModel.IBindingList list, int insertAtRow)
		{
			return MakeGhostWidget(list, insertAtRow, Field.FieldNames.ExampleSentence.ToString(), "Example", "Sentence", false);
		}


	}
}
