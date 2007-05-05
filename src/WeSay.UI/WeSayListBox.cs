using System;
using System.ComponentModel;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.UI
{
	public partial class WeSayListBox : ListBox
	{
		private WritingSystem _writingSystem;

		public WeSayListBox()
		{
			InitializeComponent();
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystem WritingSystem
		{
			get
			{
				if (_writingSystem == null)
				{
					throw new InvalidOperationException("WritingSystem must be initialized prior to use.");
				}
				return _writingSystem;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_writingSystem = value;
				Font = value.Font;
				if (value.RightToLeft)
				{
					RightToLeft = RightToLeft.Yes;
				}
				else
				{
					RightToLeft = RightToLeft.No;
				}
			}
		}

	}
}
