using System;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools
{
	public partial class EntryDetailControl : WeSay.UI.DetailList
	{
		private readonly FieldInventory _fieldInventory;

		public EntryDetailControl(FieldInventory fieldInventory)
			: base()
		{
			if(fieldInventory == null)
			{
				throw new ArgumentNullException();
			}
			_fieldInventory = fieldInventory;
		}
		private LexEntry _record;

		public LexEntry DataSource{
			get
			{
				return _record;
			}
			set
			{
				_record = value;

				Refresh();
			}
		}

		public override void Refresh()
		{
			SuspendLayout();
			Clear();
			if (_record != null)
			{
				LexEntryLayouter layout = new LexEntryLayouter(this, _fieldInventory);
				layout.AddWidgets(_record);
			}

			ResumeLayout(true);
			base.Refresh();
		}
	}
}
