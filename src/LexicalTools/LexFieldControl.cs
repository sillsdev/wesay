using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WeSay.UI;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools
{
	public partial class LexFieldControl : UserControl
	{
		LexEntry _record;
		public LexFieldControl()
		{
			InitializeComponent();
		}

		public LexEntry DataSource
		{
			get
			{
				return _record;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_record = value;
				_lexicalEntryView.Rtf = _record.ToRtf();
			}
		}

	}
}
