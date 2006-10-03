using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WeSay.UI
{
	public partial class VBox : UserControl
	{

		/// <summary>
		/// we have this instead of just using this.Count() because  Count not implemented in Mono 1.16
		/// </summary>
		private int _rowCount = 0;


		public VBox()
		{
			InitializeComponent();
		}

		public int Count
		{
			get
			{
				return _rowCount;
			}
		}

		/// <summary>
		/// I want to hide this from clients who would try to touch my controls directly
		/// </summary>
		///doesn't work to hide       protected new ControlCollection Controls
		public static new ControlCollection Controls
		{
			get
			{
				throw new InvalidOperationException("Please don't access my Controls property externally.\n For testing use GetControlOfRow and GetEditControlFromReferenceControl. Subclasses can use ActualControls");
			}
		}

		protected ControlCollection ActualControls
		{
			get
			{
				return base.Controls;
			}
		}

		public void Clear()
		{
			_rowCount = 0;
			base.Controls.Clear();
		}

		public void AddControlToBottom(Control control)
		{
			AddControl(control, -1);
		}

		public void AddControl(Control control, int insertAtRow)
		{
			if (insertAtRow < 0)
			{
				insertAtRow = _rowCount;
			}
			control.Dock = DockStyle.Top;
			base.Controls.Add(control);
			int i = RowToControlInsertionIndex(insertAtRow);
			Debug.Assert(i >= 0, "A negative insertion value will fail under Mono.");

			base.Controls.SetChildIndex(control, i);

			++_rowCount;
		}

		public int GetRowOfControl(Control control)
		{
			return (_rowCount - base.Controls.GetChildIndex(control)) - 1;
		}

		/// <summary>
		/// for unit testing
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public Control GetControlOfRow(int row)
		{
			return (base.Controls[RowToControlIndex(row)]);
		}

		private int RowToControlInsertionIndex(int row)
		{
			//reverse order (that's how docking works)
			return ((_rowCount) - row);
		}

		protected int RowToControlIndex(int row)
		{
			//reverse order (that's how docking works)
			return RowToControlInsertionIndex(row) - 1;
		}
	}
}
