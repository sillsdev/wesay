using System.Windows.Forms;
using WeSay.Data;
using WeSay.Foundation;
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
		public LexSenseLayouter(DetailList builder, ViewTemplate viewTemplate, IRecordListManager recordListManager)
			: base(builder, viewTemplate, recordListManager)
		{
		}


		internal override int AddWidgets(IBindingList list, int index, int insertAtRow)
		{
			DetailList.SuspendLayout();
			int rowCount = 0;
			LexSense sense = (LexSense)list[index];
			Field field = ActiveViewTemplate.GetField(Field.FieldNames.SenseGloss.ToString());
			if (field != null && field.DoShow )
			{
				Control glossControl = MakeBoundControl(sense.Gloss, field);
				Control glossRowControl = DetailList.AddWidgetRow(StringCatalog.Get("~Meaning"), true, glossControl, insertAtRow, false);
				++rowCount;
				insertAtRow = DetailList.GetRow(glossRowControl);
			}

			rowCount += AddCustomFields(sense, insertAtRow+rowCount);

			LexExampleSentenceLayouter exampleLayouter = new LexExampleSentenceLayouter(DetailList, ActiveViewTemplate);

			rowCount = AddChildrenWidgets(exampleLayouter, sense.ExampleSentences, insertAtRow, rowCount);

			//add a ghost
			rowCount += exampleLayouter.AddGhost(sense.ExampleSentences, insertAtRow + rowCount);
			DetailList.ResumeLayout(true);
			return rowCount;
		}

		public int AddGhost(IBindingList list, bool isHeading)
		{
//            int rowCount = 0;
//           Field field;
//           //TODO: only add this if there is no empty gloss in an existing sense (we
//           //run into this with the MissingInfoTask, where we don't want to see two empty gloss boxes (one a ghost)
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

			int insertAtRow = -1;
			return MakeGhostWidget<LexSense>(list, insertAtRow, Field.FieldNames.SenseGloss.ToString(), StringCatalog.Get("~Meaning", "This label is shown once, but has two roles.  1) it labels contains the gloss, and 2) marks the beginning of the set of fields which make up a sense. So, in english, if we labelled this 'gloss', it would describe the field well but wouldn't label the section well."), "Gloss", isHeading);

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
