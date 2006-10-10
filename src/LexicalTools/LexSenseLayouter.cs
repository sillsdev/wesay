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
	public class LexSenseLayouter : Layouter
	{
		public LexSenseLayouter(DetailList builder, FieldInventory fieldInventory)
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
		   LexSense sense = (LexSense)list[index];
		   Field field;
		   if (FieldInventory.TryGetField(Field.FieldNames.SenseGloss.ToString(), out field))
		   {
			   foreach (string writingSystemId in field.WritingSystemIds)
			   {
				   WritingSystem writingSystem = BasilProject.Project.WritingSystems[writingSystemId];
				   Control entry = MakeBoundEntry(sense.Gloss, writingSystem);
				   Control c = DetailList.AddWidgetRow(StringCatalog.Get("Meaning"), true, entry, insertAtRow);
				   ++rowCount;
				   insertAtRow = DetailList.GetRowOfControl(c);
			   }
		   }
		   LexExampleSentenceLayouter exampleLayouter = new LexExampleSentenceLayouter(DetailList, FieldInventory);

			rowCount = AddChildrenWidgets(exampleLayouter, sense.ExampleSentences, insertAtRow, rowCount);

			//add a ghost
			rowCount += exampleLayouter.AddGhost(sense.ExampleSentences, insertAtRow+rowCount);

			return rowCount;
		}

		public int AddGhost(IBindingList list)
		{
			int rowCount = 0;
		   Field field;
		   //TODO: only add this if there is no empty gloss in an existing sense (we
		   //run into this with the LexFieldTask, where we don't want to see two empty gloss boxes (one a ghost)
		   if (FieldInventory.TryGetField(Field.FieldNames.SenseGloss.ToString(), out field))
		   {
			   foreach (string writingSystemId in field.WritingSystemIds)
			   {
				   WritingSystem writingSystem = BasilProject.Project.WritingSystems[writingSystemId];

				   WeSayTextBox entry = new WeSayTextBox(writingSystem);
				   GhostBinding g = MakeGhostBinding(list, "Gloss", writingSystem, entry);
				   g.ReferenceControl = DetailList.AddWidgetRow(StringCatalog.Get("New Meaning"), true, entry);
				   ++rowCount;
			   }
		   }
			return rowCount;
		}

		public static bool HasSenseWithEmptyGloss(IBindingList list)
		{
			foreach (LexSense sense in list)
			{
				if (sense.Gloss.Count == 0)
				{
					return true;
				}
			}
			return false;
		}

	}
}
