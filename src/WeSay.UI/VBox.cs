using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WeSay.UI
{
	public class FlexibleHeightPanel : Panel
	{
		private int _heightDifferenceFromControllingChild;

		public FlexibleHeightPanel(int initialWidth, int heightDifferenceFromControllingChild, Control controllingChild)
		{
			Size = new Size(initialWidth, controllingChild.Height + _heightDifferenceFromControllingChild);
			Anchor = AnchorStyles.Left | AnchorStyles.Right;

			Controls.Add(controllingChild);
//            this.BackColor = Color.Wheat;
			controllingChild.Resize += new EventHandler(OnChildResize);
			_heightDifferenceFromControllingChild = heightDifferenceFromControllingChild;
		}

		void OnChildResize(object sender, EventArgs e)
		{
			Control c = (Control)sender;
			Debug.Assert(Controls.Contains(c));
			//if(c.Height > this.Height)
			{
				Height = c.Height + _heightDifferenceFromControllingChild;
				Invalidate();
			}
		}
	}

	public partial class VBox : TableLayoutPanel//, IContainerControl
	{
		/// <summary>
		/// we have this instead of just using this.Count() because  Count not implemented in Mono 1.16
		/// </summary>
		private int _rowCount = 0;

		private Control _focussedImmediateChild;

		public VBox()
		{

			InitializeComponent();
			ColumnCount = 1;// = FlowDirection.TopDown;

		//    this.CellBorderStyle = TableLayoutPanelCellBorderStyle.OutsetPartial;
		}


//        //from http://yortondotnet.blogspot.com/2007/01/when-activecontrol-is-not-active.html
//        public bool ActivateControl(Control active)
//        {
//            //If we know the active control, then focus it, otherwise focus the first child control.
//            bool retVal = false;
//            if (_activeControl != null)
//            {
//                _activeControl.Focus();
//                retVal = true;
//            }
//            else
//            {
//                if (this.ActualControls.Count > 0)
//                {
//                    this.ActualControls[0].Focus();
//                    retVal = true;
//                }
//            }
//            return retVal;
//        }

		public Control FocussedImmediateChild
		{
			get
			{
				foreach (Control child in ActualControls)
				{
					if (child.ContainsFocus)
					{
						return child;
					}
				}
				return null;
			}
			set
			{
				//Keep track of the active control ourselves by storing it in a private member, note that
				//that we only allow the active control to be set if it is actually a child of ours.
				if (Contains(value))
					_focussedImmediateChild = value;
			}
		}

//        public Control ActiveControl
//        {
//            get     //TODO: the following was just a try that didn't work
//            {
//                foreach (Control child in ActualControls)
//                {
//                    if(child.Focused)
//                    {
//                        IContainerControl c = child as IContainerControl;
//                        if (c != null)
//                        {
//                            return c.ActiveControl;
//                        }
//                    }
//                }
//                throw new ApplicationException("Could not find the ActiveControl of the VBox");
//            }
//        }

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

		/// <summary>
		/// Callers: consider control.AutoSizeMode = AutoSizeMode.GrowAndShrink if the control supports it.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="insertAtRow"></param>
		public void AddControl(Control control, int insertAtRow)
		{
			SuspendLayout();
			if (insertAtRow < 0)
			{
				insertAtRow = _rowCount;
			}
			base.Controls.Add(control);
			int i = RowToControlInsertionIndex(insertAtRow);
			Debug.Assert(i >= 0, "A negative insertion value will fail under Mono.");

			base.Controls.SetChildIndex(control, i);
			foreach (Control c in base.Controls)
			{
				c.TabIndex = base.Controls.GetChildIndex(c); //_rowCount - base.Controls.GetChildIndex(c);
			}
			++_rowCount;
			control.Resize += new EventHandler(OnChildControlResized);

			//VerticalScroll.Enabled = false;
			//HorizontalScroll.Enabled = false;

			ResumeLayout();
		}

		void OnChildControlResized(object sender, EventArgs e)
		{
			//an outermost control (eg the stack of form fields)
			//can have a height fixed to its container, not its contents
			if ((Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom )
			{
				return;
			}

			int h = 0;
			foreach (Control child in ActualControls)
			{
				h += child.Height ;
			}
			Height = h + Margin.Top + Margin.Bottom+ (5 * (ActualControls.Count - 1));
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
			return index; // (_rowCount - index) - 1;
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
			//reverse order
		   // return ((_rowCount) - row);
			return row;
		}

		protected int RowToControlIndex(int row)
		{
			Debug.Assert(row < _rowCount); // if we assert here, we are probably missing an opportunity to throw an ArgumentOutOfrange exception in a public method
			//reverse order (that's how docking works)
			return row;// RowToControlInsertionIndex(row) - 1;
		}

	}
}
