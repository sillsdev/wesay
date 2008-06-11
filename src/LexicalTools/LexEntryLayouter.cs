using System.Windows.Forms;
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
	public class LexEntryLayouter : Layouter
	{

		public LexEntryLayouter(DetailList builder, ViewTemplate viewTemplate, LexEntryRepository recordListManager)
			: base(builder, viewTemplate, recordListManager)
		{
		}

		 public int AddWidgets(LexEntry entry)
		{
			return AddWidgets(entry, -1);
		}

		internal override int AddWidgets(IBindingList list, int index, int insertAtRow)
		{
			return AddWidgets((LexEntry)list[index], insertAtRow);
		}

		internal int AddWidgets(LexEntry entry, int insertAtRow)
		{
			DetailList.SuspendLayout();
			int rowCount = 0;
			Field field = ActiveViewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
			if (field != null && field.GetDoShow(entry.LexicalForm, this.ShowNormallyHiddenFields))
			{
				Control formControl = MakeBoundControl(entry.LexicalForm, field);
				DetailList.AddWidgetRow(StringCatalog.Get(field.DisplayName), true, formControl, insertAtRow, false);
				insertAtRow = DetailList.GetRow(formControl);
				++rowCount;
			}
			rowCount += AddCustomFields(entry, insertAtRow+rowCount);

			LexSenseLayouter layouter = new LexSenseLayouter(DetailList, ActiveViewTemplate, RecordListManager);
			layouter.ShowNormallyHiddenFields = ShowNormallyHiddenFields;
			rowCount = AddChildrenWidgets(layouter, entry.Senses, insertAtRow, rowCount);
			//add a ghost
			rowCount += layouter.AddGhost(entry.Senses, true);

			DetailList.ResumeLayout();
			return rowCount;
		}
	}
}