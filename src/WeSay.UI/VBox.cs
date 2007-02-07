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
			//Debug.WriteLine("VBox " + Name + "   Clearing");
			_rowCount = 0;

			while(base.Controls.Count>0)
			{
			  //  Debug.WriteLine("  VBoxClear() calling dispose on " + base.Controls[0].Name);
				base.Controls[0].Dispose();
			}
			base.Controls.Clear();
		   // Debug.WriteLine("VBox " + Name + "   Clearing DONE");
		}

		public void AddControlToBottom(Control control)
		{
			AddControl(control, -1);
		}

		public void AddControl(Control control, int insertAtRow)
		{
			SuspendLayout();
			if (insertAtRow < 0)
			{
				insertAtRow = _rowCount;
			}
			control.Dock = DockStyle.Top;
			base.Controls.Add(control);
			int i = RowToControlInsertionIndex(insertAtRow);
			Debug.Assert(i >= 0, "A negative insertion value will fail under Mono.");

			base.Controls.SetChildIndex(control, i);
			foreach (Control c in base.Controls)
			{
				c.TabIndex = _rowCount - base.Controls.GetChildIndex(c);
			}
			++_rowCount;
			ResumeLayout();
		}

		public int GetRowOfControl(Control control)
		{
			int index = base.Controls.GetChildIndex(control,false);
			if(index == -1)
			{
				index = 0;
				foreach (Control childControl in base.Controls)
				{
					if(HasDescendentControl(childControl, control))
					{
						break;
					}
					++index;
				}
				if(index == base.Controls.Count)
				{
					throw new ArgumentException("Control is not an owned descendant.");
				}
			}
			Debug.Assert(index < _rowCount);
			return (_rowCount - index) - 1;
		}

		private static bool HasDescendentControl(Control current, Control control)
		{
			foreach (Control child in current.Controls)
			{
				if (child == control || HasDescendentControl(child, control))
				{
					return true;
				}
			}
			return false;
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
			Debug.Assert(row <= _rowCount);
			//reverse order (that's how docking works)
			return ((_rowCount) - row);
		}

		protected int RowToControlIndex(int row)
		{
			Debug.Assert(row < _rowCount);
			//reverse order (that's how docking works)
			return RowToControlInsertionIndex(row) - 1;
		}

	}
}
