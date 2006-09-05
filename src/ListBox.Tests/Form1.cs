using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ListBox
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			//this._grid.SelectedDataRows


			BindingList<X> l = new BindingList<X>();
			for (int i = 0; i < 1000; i++)
				l.Add(new X(i));

			this._grid.DataSource = l;
			_grid.BackColor = Color.AliceBlue;
			_grid.AutoStretchColumnsToFitWidth = true;
			_grid.AutoSize();
			_grid.Columns.StretchToFit();
		}

		private void _grid_SelectionChanged(object sender, System.EventArgs e)
		{
			MessageBox.Show(_grid.SelectedIndex.ToString());
		}
	}

	public class X
	{
		private int _i;
		public X(int i)
		{
			_i = i;
		}

		public int Number
		{
			get
			{
			   // System.Diagnostics.Debug.Assert(_i < 30);
				return _i;
			}
			set { _i = value; }
		}

		public override string ToString()
		{
			return Number.ToString();
		}
	}
}