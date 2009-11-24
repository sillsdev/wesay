using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Palaso.I8N;
using Palaso.Lift.Model;
using Palaso.Reporting;
using Palaso.Lift;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	/// <summary>
	/// <see cref="Layouter"/>
	/// </summary>
	public class LexExampleSentenceLayouter: Layouter
	{
		public LexExampleSentenceLayouter(DetailList builder, ViewTemplate viewTemplate,
			IServiceProvider serviceProvider)
				: base(builder, viewTemplate, null, serviceProvider)
		{
		}

		internal override int AddWidgets(PalasoDataObject wsdo, int insertAtRow)
		{
			LexExampleSentence example = (LexExampleSentence) wsdo;

			DetailList.SuspendLayout();
			int rowCount = 0;
			try
			{
				Field field =
						ActiveViewTemplate.GetField(Field.FieldNames.ExampleSentence.ToString());
				if (field != null && field.GetDoShow(example.Sentence, ShowNormallyHiddenFields))
				{
					Control control = MakeBoundControl(example.Sentence, field);
					DetailList.AddWidgetRow(
							StringCatalog.Get("~Example",
											  "This is the field containing an example sentence of a sense of a word."),
							false,
							control,
							insertAtRow,
							false);
					++rowCount;
					insertAtRow = DetailList.GetRow(control);
				}

				field = ActiveViewTemplate.GetField(Field.FieldNames.ExampleTranslation.ToString());
				if (field != null && field.GetDoShow(example.Translation, ShowNormallyHiddenFields))
				{
					Control entry = MakeBoundControl(example.Translation, field);
					DetailList.AddWidgetRow(
							StringCatalog.Get("~Translation",
											  "This is the field for putting in a translation of an example sentence."),
							false,
							entry,
							insertAtRow + rowCount,
							false);
					++rowCount;
				}

				rowCount += AddCustomFields(example, insertAtRow + rowCount);
			}
			catch (ConfigurationException e)
			{
				ErrorReport.NotifyUserOfProblem(e.Message);
			}

			DetailList.ResumeLayout();
			return rowCount;
		}

		public int AddGhost(LexSense sense, IList<LexExampleSentence> list, int insertAtRow)
		{
			return MakeGhostWidget(sense, list,
								   insertAtRow,
								   Field.FieldNames.ExampleSentence.ToString(),
								   StringCatalog.Get("~Example",
													 "This is the field containing an example sentence of a sense of a word."),
								   "Sentence",
								   false);
		}
	}
}