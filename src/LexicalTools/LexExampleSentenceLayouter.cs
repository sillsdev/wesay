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
		public LexExampleSentenceLayouter(DetailList  builder, FieldInventory fieldInventory)
			: base(builder, fieldInventory)
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

			Field field = FieldInventory.GetField(Field.FieldNames.ExampleSentence.ToString());
			if (field != null && field.Visibility == Field.VisibilitySetting.Visible)
			{
				Control entry = MakeBoundEntry(example.Sentence, field);
				DetailList.AddWidgetRow(StringCatalog.Get("Example"), false, entry, insertAtRow+rowCount);
				++rowCount;
			}

			field = FieldInventory.GetField(Field.FieldNames.ExampleTranslation.ToString());
			if (field != null && field.Visibility == Field.VisibilitySetting.Visible)
			{
				 Control entry = MakeBoundEntry(example.Translation, field);
				DetailList.AddWidgetRow(StringCatalog.Get("Translation"), false, entry, insertAtRow+rowCount);
				++rowCount;
			}

			return rowCount;
		}

		public int AddGhost(System.ComponentModel.IBindingList list, int insertAtRow)
		{
			return MakeGhostWidget(list, insertAtRow, Field.FieldNames.ExampleSentence.ToString(), "New Example", "Sentence");
		}


	}
}
