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
			this.components = new System.ComponentModel.Container();
			InitializeComponent();
			this.BackColor = System.Drawing.Color.LightYellow;
			//does nothing this.Margin = new Padding(900);

		   //seems to be ignored this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		  //  this.DoubleBuffered = true;
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

		void OnChildResize(object sender, EventArgs e)
		{
			this.SuspendLayout();
			int h = 0;
			foreach (Control control in ActualControls)
			{
				h += control.Height;
			}
			this.Height = h;
			//hack need to goose it some better way...
			this.Visible = false;
			this.Visible = true;
//   doesn't do it
			//this.SuspendLayout();
//            this.ResumeLayout(true);
		  //doesn't do it  this.Refresh();

			this.ResumeLayout();

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
			this.components.Add(control);//dispose when we are
			int i = RowToControlInsertionIndex(insertAtRow);
			Debug.Assert(i >= 0, "A negative insertion value will fail under Mono.");

			base.Controls.SetChildIndex(control, i);
			foreach (Control c in base.Controls)
			{
				c.TabIndex = _rowCount - base.Controls.GetChildIndex(c);
			}
			++_rowCount;

			control.Resize += new EventHandler(OnChildResize);

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
			if(0 > row || row >= _rowCount)
			{
				throw new ArgumentOutOfRangeException("row", row, "row must be between 0 and Count-1 inclusive");
			}
			return (base.Controls[RowToControlIndex(row)]);
		}

		private int RowToControlInsertionIndex(int row)
		{
			//reverse order (that's how docking works)
			return ((_rowCount) - row);
		}

		protected int RowToControlIndex(int row)
		{
			Debug.Assert(row < _rowCount); // if we assert here, we are probably missing an opportunity to throw an ArgumentOutOfrange exception in a public method
			//reverse order (that's how docking works)
			return RowToControlInsertionIndex(row) - 1;
		}

	}
}
