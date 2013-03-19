using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Palaso.DictionaryServices.Model;
using Palaso.i18n;
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
		public LexExampleSentenceLayouter(DetailList parentDetailList, int parentRow, ViewTemplate viewTemplate,
			IServiceProvider serviceProvider, LexExampleSentence exampleToLayout)
			: base(parentDetailList, parentRow, viewTemplate, null, serviceProvider, exampleToLayout)
		{
			DetailList.Name = "LexExampleSentenceDetailList";
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

		public int AddGhost(LexSense sense, IList<LexExampleSentence> list)
		{
			return MakeGhostWidget(sense, list,
								   Field.FieldNames.ExampleSentence.ToString(),
								   StringCatalog.Get("~Example",
													 "This is the field containing an example sentence of a sense of a word."),
								   "Sentence",
								   false);
		}

	}
}