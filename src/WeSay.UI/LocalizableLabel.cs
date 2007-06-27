using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.UI
{
	public partial class LocalizableLabel : Label
	{
		private bool _alreadyChanging;

		public LocalizableLabel()
		{
			InitializeComponent();
		}

		private void LocalizableLable_FontChanged(object sender, EventArgs e)
		{
		   if(_alreadyChanging)
		   {
			   return;
		   }
			_alreadyChanging = true;

			this.Font = new Font(StringCatalog.LabelFont.Name, this.Font.Size, this.Font.Style);

			_alreadyChanging = false;
		}

		private void LocalizableLabel_TextChanged(object sender, EventArgs e)
		{
		   if(_alreadyChanging)
		   {
			   return;
		   }
			_alreadyChanging = true;

			this.Text = StringCatalog.Get(this.Text);

			_alreadyChanging = false;

		}


	}
}
