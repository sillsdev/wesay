using System.Windows.Forms;
using Palaso.Reporting;
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
	public class LexExampleSentenceLayouter : Layouter
	{
		public LexExampleSentenceLayouter(DetailList  builder, ViewTemplate viewTemplate)
			: base(builder, viewTemplate, null)
		{
		}

		internal override int AddWidgets(IBindingList list, int index, int insertAtRow)
		{
			DetailList.SuspendLayout();
			int rowCount = 0;
			try
			{
				LexExampleSentence example = (LexExampleSentence) list[index];

				Field field = ActiveViewTemplate.GetField(Field.FieldNames.ExampleSentence.ToString());
				if (field != null && field.GetDoShow(example.Sentence, this.ShowNormallyHiddenFields))
				{
					Control control = MakeBoundControl(example.Sentence, field);
					DetailList.AddWidgetRow(
						StringCatalog.Get("~Example",
										  "This is the field containing an example sentence of a sense of a word."),
						false,
						control, insertAtRow, false);
					++rowCount;
					insertAtRow = DetailList.GetRow(control);
				}

				field = ActiveViewTemplate.GetField(Field.FieldNames.ExampleTranslation.ToString());
				if (field != null && field.GetDoShow(example.Translation, this.ShowNormallyHiddenFields))
				{
					Control entry = MakeBoundControl(example.Translation, field);
					DetailList.AddWidgetRow(
						StringCatalog.Get("~Translation",
										  "This is the field for putting in a translation of an example sentence."),
						false,
						entry, insertAtRow + rowCount, false);
					++rowCount;
				}

				rowCount += AddCustomFields(example, insertAtRow + rowCount);
			}
			catch (ConfigurationException e)
			{
				Palaso.Reporting.ErrorReport.ReportNonFatalMessage(e.Message);
			}

			DetailList.ResumeLayout(true);
			return rowCount;
		}

		public int AddGhost(System.ComponentModel.IBindingList list, int insertAtRow)
		{
			return MakeGhostWidget<LexExampleSentence>(list, insertAtRow, Field.FieldNames.ExampleSentence.ToString(), StringCatalog.Get("~Example", "This is the field containing an example sentence of a sense of a word."), "Sentence", false);
		}


	}
}
