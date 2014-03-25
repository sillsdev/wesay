using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		}

		internal override int AddWidgets(PalasoDataObject wsdo, int insertAtRow)
		{
			LexExampleSentence example = (LexExampleSentence) wsdo;
			FirstRow = insertAtRow;

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

			DetailList.ResumeLayout(false);
			LastRow = insertAtRow + rowCount - 1;	// want index of last row owned, not a limit value
			return rowCount;
		}

		/// <summary>
		/// Create a new LexExampleSentenceLayouter for a new LexExampleSentence created from a ghost,
		/// and insert it at the right place in the Layouter tree.
		/// </summary>
		protected override Layouter CreateAndInsertNewLayouter(int row, PalasoDataObject wsdo)
		{
			var newLayouter = new LexExampleSentenceLayouter(DetailList, row, ActiveViewTemplate, _serviceProvider,
				wsdo as LexExampleSentence)
			{
				ShowNormallyHiddenFields = ShowNormallyHiddenFields,
				Deletable = false,
				ParentLayouter = ParentLayouter
			};
			var idx = ParentLayouter.ChildLayouts.IndexOf(this);
			if (idx >= 0)
				ParentLayouter.ChildLayouts.Insert(idx, newLayouter);
			else
				ParentLayouter.ChildLayouts.Add(newLayouter);
			return newLayouter;
		}

		protected override void AdjustLayoutRowsAfterGhostTrigger(int rowCount)
		{
			// Get the relevant LexSenseLayouter (either "this" or its parent).  Either way,
			// its rows have already been adjusted but following senses (if any) need to have
			// their rows adjusted.
			var senseLayouter = ParentLayouter;
			Debug.Assert(senseLayouter is LexSenseLayouter);
			senseLayouter.LastRow = LastRow;
			Layouter parentEntryLayouter = senseLayouter;
			while (parentEntryLayouter.ParentLayouter != null)
				parentEntryLayouter = parentEntryLayouter.ParentLayouter;
			Debug.Assert(parentEntryLayouter is LexEntryLayouter);
			int first = parentEntryLayouter.ChildLayouts.IndexOf(senseLayouter);
			Debug.Assert(first >= 0);
			for (int i = first+1; i < parentEntryLayouter.ChildLayouts.Count; ++i)
			{
				var layouter = parentEntryLayouter.ChildLayouts[i];
				layouter.FirstRow = layouter.FirstRow + rowCount;
				layouter.LastRow = layouter.LastRow + rowCount;
			}
			// REVIEW: do we need to adjust rows for LexExampleSentenceLayouts?  I don't think they're ever used,
			// since example sentences cannot be deleted and enabling the delete button is the only use of these
			// row numbers.
		}

		public int AddGhost(LexSense sense, IList<LexExampleSentence> list, int insertAtRow)
		{
			return MakeGhostWidget(sense, list,
								   Field.FieldNames.ExampleSentence.ToString(),
								   StringCatalog.Get("~Example",
													 "This is the field containing an example sentence of a sense of a word."),
								   "Sentence",
								   false,
								   insertAtRow);
		}

	}
}
