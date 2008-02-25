using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
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
			int rowCount = 0;
			DetailList.SuspendLayout();
			try
			{

				LexSense sense = (LexSense) list[index];
#if GlossMeaning
				Field field = ActiveViewTemplate.GetField(Field.FieldNames.SenseGloss.ToString());
				if (field != null && field.GetDoShow(sense.Gloss, this.ShowNormallyHiddenFields))
				{
					Control meaningControl = MakeBoundControl(sense.Gloss, field);
#else
					Field field = ActiveViewTemplate.GetField(LexSense.WellKnownProperties.Definition);
					if (field != null && field.GetDoShow(sense.Definition, this.ShowNormallyHiddenFields))
					{
					Control meaningControl = MakeBoundControl(sense.Definition, field);
#endif
					string label = StringCatalog.Get("~Meaning");
					LexEntry entry = sense.Parent as LexEntry;
					if (entry != null) // && entry.Senses.Count > 1)
					{
						label += " " + (entry.Senses.IndexOf(sense) + 1);
					}
					Control meaningRowControl = DetailList.AddWidgetRow(label, true, meaningControl, insertAtRow, false);
					++rowCount;
					insertAtRow = DetailList.GetRow(meaningRowControl);
				}

//#if GlossMeaning
//#else
//                Field glossfield = ActiveViewTemplate.GetField(Field.FieldNames.SenseGloss.ToString());
//                if (glossfield != null && glossfield.GetDoShow(sense.Gloss, this.ShowNormallyHiddenFields))
//                {
//                    Control control = MakeBoundControl(sense.Gloss, glossfield);
//                    DetailList.AddWidgetRow(
//                        StringCatalog.Get("~Gloss",
//                                          "This is the field that normally has just a single word translation, not a full definition. Mostly used with interlinear text displays."),
//                        false,
//                        control, insertAtRow + rowCount, false);
//                    ++rowCount;
//                    insertAtRow = DetailList.GetRow(control);
//                }
//#endif

				rowCount += AddCustomFields(sense, insertAtRow + rowCount);

				LexExampleSentenceLayouter exampleLayouter =
					new LexExampleSentenceLayouter(DetailList, ActiveViewTemplate);
				exampleLayouter.ShowNormallyHiddenFields = ShowNormallyHiddenFields;

				rowCount = AddChildrenWidgets(exampleLayouter, sense.ExampleSentences, insertAtRow, rowCount);

				//add a ghost for another example if we don't have one or we're in the "show all" mode
			   //removed because of its effect on the Add Examples task, where
				//we'd like to be able to add more than one
				//if (ShowNormallyHiddenFields || sense.ExampleSentences.Count == 0)
				{
					rowCount += exampleLayouter.AddGhost(sense.ExampleSentences, insertAtRow + rowCount);
				}
			}
			catch (ConfigurationException e)
			{
				Palaso.Reporting.ErrorReport.ReportNonFatalMessage(e.Message);
			}
			DetailList.ResumeLayout();
			return rowCount;
		}

		public int AddGhost(IBindingList list, bool isHeading)
		{
			int insertAtRow = -1;
			string label = GetLabelForMeaning(list);
#if GlossMeaning
			return MakeGhostWidget<LexSense>(list, insertAtRow, Field.FieldNames.SenseGloss.ToString(), label, "Gloss", isHeading);
#else
			return MakeGhostWidget<LexSense>(list, insertAtRow, LexSense.WellKnownProperties.Definition, label, "Definition", isHeading);
#endif
		}

		private string GetLabelForMeaning(IBindingList list)
		{
			string label = StringCatalog.Get("~Meaning", "This label is shown once, but has two roles.  1) it labels the defintion field, and 2) marks the beginning of the set of fields which make up a sense. So, in english, if we labelled this 'definition', it would describe the field well but wouldn't label the section well.");
			if(list.Count > 0)
			{
				label += " "+(list.Count + 1);
			}
			return label;
		}

		protected override void UpdateGhostLabel(IBindingList list, int rowOfGhost)
		{
			DetailList.GetLabelControlFromRow(rowOfGhost).Text = GetLabelForMeaning(list);
		}
	}
}
