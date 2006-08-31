using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public partial class EntryDetailControl : WeSay.UI.DetailList
	{

		private LexEntry _record;

		public EntryDetailControl()
		{
		}

		public LexEntry DataSource{
			get
			{
				return _record;
			}
			set
			{
				this.SuspendLayout();
				this.Clear();
				_record = value;
				if (_record != null)
				{
					LexEntryLayouter layout = new LexEntryLayouter(this);
					layout.AddWidgets(_record);
				}

				this.ResumeLayout(true);
			}
		}
	}
}
