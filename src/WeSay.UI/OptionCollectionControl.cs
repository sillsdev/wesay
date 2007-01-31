using System.Text;
using System.Windows.Forms;
using WeSay.Foundation;

namespace WeSay.UI
{
	public partial class OptionCollectionControl : UserControl
	{
		private OptionRefCollection _optionRefCollection;
		private OptionsList _list;
		private string _idOfPreferredWritingSystem;

		public OptionCollectionControl()
		{
			InitializeComponent();
		}


		public OptionCollectionControl(OptionRefCollection optionRefCollection, OptionsList list, string idOfPreferredWritingSystem)
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

		private void button1_Click(object sender, System.EventArgs e)
		{

			_optionRefCollection.Keys.Add(_list.Options[0].Key);
			_optionRefCollection.NotifyPropertyChanged();
			LoadDisplay();
			this.Refresh();
		}

		private void OptionCollectionControl_BackColorChanged(object sender, System.EventArgs e)
		{
			this._textBox.BackColor = this.BackColor;
		}

		private void OptionCollectionControl_Load(object sender, System.EventArgs e)
		{
			//read only
			this.TabStop = false;
			this.BackColor = this.Parent.BackColor;
		}

	}
}
