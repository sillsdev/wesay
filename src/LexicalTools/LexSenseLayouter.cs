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
	public class LexSenseLayouter : Layouter
	{
		public LexSenseLayouter(DetailList builder, ViewTemplate viewTemplate)
			: base(builder, viewTemplate)
		{
		}


		internal override int AddWidgets(IBindingList list, int index, int insertAtRow)
		{
			int rowCount = 0;
			LexSense sense = (LexSense)list[index];
			Field field = ActiveViewTemplate.GetField(Field.FieldNames.SenseGloss.ToString());
			if (field != null && field.Visibility == Field.VisibilitySetting.Visible)
			{
				Control entry = MakeBoundEntry(sense.Gloss, field);
				Control c = DetailList.AddWidgetRow(StringCatalog.Get("Meaning"), true, entry, insertAtRow);
				++rowCount;
				insertAtRow = DetailList.GetRowOfControl(c);
			}

			rowCount += AddCustomFields(sense, insertAtRow+rowCount);

			LexExampleSentenceLayouter exampleLayouter = new LexExampleSentenceLayouter(DetailList, ActiveViewTemplate);

			rowCount = AddChildrenWidgets(exampleLayouter, sense.ExampleSentences, insertAtRow, rowCount);

			//add a ghost
			rowCount += exampleLayouter.AddGhost(sense.ExampleSentences, insertAtRow + rowCount);

			return rowCount;
		}

		public int AddGhost(IBindingList list, bool isHeading)
		{
//            int rowCount = 0;
//           Field field;
//           //TODO: only add this if there is no empty gloss in an existing sense (we
//           //run into this with the LexFieldTask, where we don't want to see two empty gloss boxes (one a ghost)
//           if (viewTemplate.TryGetField(Field.FieldNames.SenseGloss.ToString(), out field))
//           {
//               foreach (string writingSystemId in field.WritingSystemIds)
//               {
//                   WritingSystem writingSystem = BasilProject.Project.WritingSystems[writingSystemId];
//
//                   WeSayTextBox entry = new WeSayTextBox(writingSystem);
//                   GhostBinding g = MakeGhostBinding(list, "Gloss", writingSystem, entry);
//                   g.ReferenceControl = DetailList.AddWidgetRow(StringCatalog.GetListOfType("New Meaning"), true, entry);
//                   ++rowCount;
//               }
//           }
//            return rowCount;

			int insertAtRow = -1;//////REVIEW!!!!!!!!!!!!!!!!!!
			return MakeGhostWidget(list, insertAtRow, Field.FieldNames.SenseGloss.ToString(), "Meaning", "Gloss", isHeading);

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
