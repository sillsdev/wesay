using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WeSay.UI
{

	public partial class VBox : TableLayoutPanel//, IContainerControl
	{
		/// <summary>
		/// we have this instead of just using this.Count() because  Count not implemented in Mono 1.16
		/// </summary>
		private int _rowCount = 0;

		public VBox()
		{

			InitializeComponent();
			ColumnCount = 1;// = FlowDirection.TopDown;
		//    this.CellBorderStyle = TableLayoutPanelCellBorderStyle.OutsetPartial;
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
		public static new Control.ControlCollection Controls
		{
			get
			{
				throw new InvalidOperationException("Please don't access my Controls property externally.\n For testing use GetControlOfRow and GetEditControlFromReferenceControl. Subclasses can use ActualControls");
			}
		}

		protected TableLayoutControlCollection ActualControls
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

			int i = /*RowToControlInsertionIndex(*/insertAtRow;//);
			Debug.Assert(i >= 0, "A negative insertion value will fail under Mono.");

			base.Controls.SetChildIndex(control, i);
			foreach (Control c in base.Controls)
			{
				c.TabIndex = base.Controls.GetChildIndex(c); //_rowCount - base.Controls.GetChildIndex(c);
			}
			++_rowCount;
			// control.Resize += new EventHandler(OnChildControlResized);

			//VerticalScroll.Enabled = false;
			//HorizontalScroll.Enabled = false;

			ResumeLayout();
		}
	}
}
