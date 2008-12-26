using System.Windows.Forms;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	/// <summary>
	/// <see cref="Layouter"/>
	/// </summary>
	public class LexEntryLayouter: Layouter
	{
		public LexEntryLayouter(DetailList builder,
								ViewTemplate viewTemplate,
								LexEntryRepository lexEntryRepository)
				: base(builder, viewTemplate, lexEntryRepository) {}

		public int AddWidgets(LexEntry entry)
		{
			return AddWidgets(entry, -1);
		}

		internal override int AddWidgets(WeSayDataObject wsdo, int insertAtRow)
		{
			return AddWidgets((LexEntry) wsdo, insertAtRow);
		}

		internal int AddWidgets(LexEntry entry, int insertAtRow)
		{
			 SetUpLayoutInfoServiceProvider(entry);

			DetailList.SuspendLayout();
			int rowCount = 0;
			Field field = ActiveViewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
			if (field != null && field.GetDoShow(entry.LexicalForm, ShowNormallyHiddenFields))
			{
				Control formControl = MakeBoundControl(entry.LexicalForm, field);
				DetailList.AddWidgetRow(StringCatalog.Get(field.DisplayName),
										true,
										formControl,
										insertAtRow,
										false);
				insertAtRow = DetailList.GetRow(formControl);
				++rowCount;
			}
			rowCount += AddCustomFields(entry, insertAtRow + rowCount);

			var rowCountBeforeSenses = rowCount;
			LexSenseLayouter layouter = new LexSenseLayouter(DetailList,
															  ActiveViewTemplate,
															  RecordListManager);
			layouter.ShowNormallyHiddenFields = ShowNormallyHiddenFields;
			rowCount = AddChildrenWidgets(layouter, entry.Senses, insertAtRow, rowCount);

			//see: WS-1120 Add option to limit "add meanings" task to the ones that have a semantic domain
			//also: WS-639 (jonathan_coombs@sil.org) In Add meanings, don't show extra meaning slots just because a sense was created for the semantic domain
			var ghostingRule = ActiveViewTemplate.GetGhostingRuleForField(LexEntry.WellKnownProperties.Sense);
			if (rowCountBeforeSenses == rowCount || ghostingRule.ShowGhost)
			{
				rowCount += layouter.AddGhost(entry.Senses, true);
			}

			DetailList.ResumeLayout();
			return rowCount;
		}

		private void SetUpLayoutInfoServiceProvider(LexEntry entry)
		{
			Field lexicalUnitField = ActiveViewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
			string entryName = entry.LexicalForm.GetBestAlternativeString(lexicalUnitField.WritingSystemIds);
			_serviceProvider = new LayoutInfoProvider(entryName);
		}
	}
}