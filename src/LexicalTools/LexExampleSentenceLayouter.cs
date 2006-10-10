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

			Field field;
			if (FieldInventory.TryGetField(Field.FieldNames.ExampleSentence.ToString(), out field))
			{
				foreach (string writingSystemId in field.WritingSystemIds)
				{
					WritingSystem writingSystem = BasilProject.Project.WritingSystems[writingSystemId];
					Control entry = MakeBoundEntry(example.Sentence, writingSystem);
					DetailList.AddWidgetRow(StringCatalog.Get("Example"), false, entry, insertAtRow+rowCount);
					++rowCount;
				}
			}

			if (FieldInventory.TryGetField(Field.FieldNames.ExampleTranslation.ToString(), out field))
			{
				foreach (string writingSystemId in field.WritingSystemIds)
				{
					WritingSystem writingSystem = BasilProject.Project.WritingSystems[writingSystemId];
					Control entry = MakeBoundEntry(example.Translation, writingSystem);
					DetailList.AddWidgetRow(StringCatalog.Get("Translation"), false, entry, insertAtRow+rowCount);
					++rowCount;
				}
			}

			return rowCount;
		}

		public int AddGhost(System.ComponentModel.IBindingList list, int insertAtRow)
		{
			int rowCount = 0;
			Field field;
			if (FieldInventory.TryGetField(Field.FieldNames.ExampleSentence.ToString(), out field))
			{
				foreach (string writingSystemId in field.WritingSystemIds)
				{
					WritingSystem writingSystem = BasilProject.Project.WritingSystems[writingSystemId];
					WeSayTextBox entry = new WeSayTextBox(writingSystem);
					GhostBinding g = MakeGhostBinding(list, "Sentence", writingSystem, entry);
					g.ReferenceControl = DetailList.AddWidgetRow(StringCatalog.Get("New Example"), false, entry, insertAtRow+rowCount);
					// entry.PrepareForFadeIn();
					++rowCount;
				}
			}

			return rowCount;
		}
	}
}
