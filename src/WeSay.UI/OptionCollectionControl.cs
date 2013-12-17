using System;
using System.Text;
using System.Windows.Forms;
using WeSay.Foundation.Options;

namespace WeSay.UI
{
	public partial class OptionCollectionControl: UserControl
	{
		private readonly OptionRefCollection _optionRefCollection;
		private OptionsList _list;
		private string _idOfPreferredWritingSystem;

		public OptionCollectionControl()
		{
			InitializeComponent();
		}

		public OptionCollectionControl(OptionRefCollection optionRefCollection,
									   OptionsList list,
									   string idOfPreferredWritingSystem)
		{
			_optionRefCollection = optionRefCollection;
			_list = list;
			_idOfPreferredWritingSystem = idOfPreferredWritingSystem;
			InitializeComponent();
			LoadDisplay();
		}

		private void LoadDisplay()
		{
			StringBuilder builder = new StringBuilder();

			foreach (string key in _optionRefCollection.Keys)
			{
				builder.AppendFormat("{0} | ", key);
			}
			_textBox.Text = builder.ToString();
		}

		private void OptionCollectionControl_BackColorChanged(object sender, EventArgs e)
		{
			_textBox.BackColor = BackColor;
		}

		private void OptionCollectionControl_Load(object sender, EventArgs e)
		{
			//read only
			TabStop = false;
			BackColor = Parent.BackColor;
		}
	}
}