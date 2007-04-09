using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WeSay.UI
{
	/// <summary>
	/// This control displays a simple list of {label, control} pairs, one per row.
	/// It supports dynamically removing and inserting new rows, to support
	/// "ghost" fields
	/// </summary>
	public partial class DetailList : TableLayoutPanel
	{
		/// <summary>
		/// Can be used to track which data item the user is currently editting, to,
		/// for example, hilight that piece in a preview control
		/// </summary>
		public event EventHandler<CurrentItemEventArgs> ChangeOfWhichItemIsInFocus = delegate
																			 {
																			 };

		private int _indexOfLabel = 1;
		private int _indexOfTextBox = 0;

		public DetailList()
		{
			InitializeComponent();
			SetStyle(ControlStyles.OptimizedDoubleBuffer |
					 ControlStyles.AllPaintingInWmPaint |
					 ControlStyles.UserPaint,
					 true);

			Name = "DetailList";//for  debugging
			_fadeInTimer.Enabled = false;
			_fadeInTimer.Interval = 500;

		   AutoScroll = true; //but we need to make sure children are never wider than we are
		   HScroll = false;
			VerticalScroll.Enabled = true;
			VerticalScroll.Visible = true;
		   ColumnCount = 2;
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
		   Padding = new Padding(20, 0,0,0);

		   ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
		   ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

	   }

		protected override void OnPaddingChanged(EventArgs e)
		{
			base.OnPaddingChanged(e);
			Padding = new Padding(Math.Max(Padding.Left, 20), Padding.Top, Padding.Right, Padding.Bottom);
		}

		public Control AddWidgetRow(string label, bool isHeader, Control control)
		{
			return AddWidgetRow(label, isHeader, control, RowCount);
		}

		public int Count
		{
			get
			{
				return RowCount;
			}
		}

		public Control FocussedImmediateChild
		{
			get
			{
				foreach (Control child in Controls)
				{
					if (child.ContainsFocus)
					{
						return child;
					}
				}
				return null;
			}
			//set
			//{
			//    //Keep track of the active control ourselves by storing it in a private member, note that
			//    //that we only allow the active control to be set if it is actually a child of ours.
			//    if (Contains(value))
			//        _focussedImmediateChild = value;
			//}
		}

		public void Clear()
		{
			//Debug.WriteLine("VBox " + Name + "   Clearing");

			while (Controls.Count > 0)
			{
				//  Debug.WriteLine("  VBoxClear() calling dispose on " + base.Controls[0].Name);
				Controls[0].Dispose();
			}
			Controls.Clear();
			// Debug.WriteLine("VBox " + Name + "   Clearing DONE");
			RowCount = 0;
			RowStyles.Clear();
		}

		public Control AddWidgetRow(string fieldLabel, bool isHeader, Control editWidget, int insertAtRow)
		{
			//Debug.WriteLine(String.Format("AddWidgetRow({0}, header={1}, , row={2}", fieldLabel, isHeader, insertAtRow));
			return /*Panel panel = */AddRowPanel(editWidget, fieldLabel, isHeader, insertAtRow);
			//AddControl(panel, insertAtRow);
			//return panel;
		}
		//protected override void OnClientSizeChanged(EventArgs e)
		//{
		//    base.OnClientSizeChanged(e);
		//    int widthDifference = Width - ClientSize.Width;
		//    if(widthDifference > 0)
		//    {
		//        foreach (Control control in this.ActualControls)
		//        {
		//            AdjustWidthsOfControlAndDescendants(control, widthDifference);
		//        }
		//    }
		//}

		//static private void AdjustWidthsOfControlAndDescendants(Control control, int widthDifference) {
		//    if((control.Anchor & (AnchorStyles.Left | AnchorStyles.Right))
		//       == (AnchorStyles.Left | AnchorStyles.Right))
		//    {
		//        control.Width -= widthDifference;
		//        foreach (Control child in control.Controls)
		//        {
		//            AdjustWidthsOfControlAndDescendants(child, widthDifference);
		//        }
		//    }
		//}

		private Control AddRowPanel(Control editWidget, string fieldLabel, bool isHeader, int insertAtRow)
		{
			RowStyles.Add(new RowStyle(SizeType.AutoSize));

			if (insertAtRow >= RowCount)
			{
			   RowCount = insertAtRow+1;
			}
			else
			{
				if(insertAtRow == -1)
				{
					insertAtRow = RowCount;
				}
				else
				{
					// move down to make space for new row
					for (int row = RowCount; row > insertAtRow; row--)
					{
						for (int col = 0; col < ColumnCount; col++)
						{
							Control c = GetControlFromPosition(col, row - 1);
							//if (row == RowCount - 1)
							//{
							//    SetRow(c, -1);
							//}
							//else
							//{
							this.SetCellPosition(c, new TableLayoutPanelCellPosition(col, row));
							//}
						}
					}
				}
				RowCount++;
			}


			int top = 0;// AddHorizontalRule(panel, isHeader, _rowCount == 0);
			if (isHeader)
			{
				int beforeHeadingPadding = 8;
				top = beforeHeadingPadding;
			}
			Label label = new Label();
			if (isHeader)
			{
				label.Font = new Font(label.Font, FontStyle.Bold);
			}
			label.Text = fieldLabel;
			label.Size = new Size(75, 50);
			label.AutoSize = true;
//            label.Anchor = AnchorStyles.Left | AnchorStyles.Top;
//            label.Dock = DockStyle.Left;
			int verticalPadding = 0;
			label.Top = verticalPadding+3+top;


			Controls.Add(label, 0, insertAtRow);


//            editWidget.Top = verticalPadding + top;
//            int padding = 2;
//            editWidget.Left = label.Left + label.Width + padding;
//            editWidget.Width = (ClientSize.Width - editWidget.Left) - 20;
//            editWidget.Width = GetColumnWidths()[1];
			FixUpForMono(editWidget);
			//editWidget.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			editWidget.Dock = DockStyle.Fill;

			editWidget.KeyDown += new KeyEventHandler(OnEditWidget_KeyDown);

			Debug.Assert(GetControlFromPosition(1, insertAtRow) == null);
			Controls.Add(editWidget, 1, insertAtRow);

//            FlexibleHeightPanel panel = new FlexibleHeightPanel(100, 10 + (editWidget.Top/*-6*/), editWidget);

//            FlexibleHeightPanel panel = new FlexibleHeightPanel(ClientSize.Width, 10, editWidget);
			//panel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top ;
//            panel.Dock = DockStyle.Fill;
//            panel.Name = fieldLabel+"_panel of detailList";
//            panel.Controls.Add(label);

//            return panel;
			return editWidget;
		}

		void OnEditWidget_KeyDown(object sender, KeyEventArgs e)
		{
			OnKeyDown(e);
		}

		private static void FixUpForMono(Control control)
		{
// in mono 1.16,  controls with was something like zero,
			//such that the the child width was something like -3.
			//we tried various things to work around this and couldn't get anything smart to work.
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				//didn't work
				//ResumeLayout(true);
				//this.Refresh();

				////for mono debugging. Indications were that this was coming out negative on mono.
				if (control.Width < 0)
					control.Width = 400;

				//didn't help    control.Dock = DockStyle.Fill;
			}
		}


		private void _fadeInTimer_Tick(object sender, EventArgs e)
		{
			foreach (Control c in Controls)
			{
				if (c.Controls.Count < 2)
					continue;
//                WeSayTextBox tb = c.Controls[_indexOfTextBox] as WeSayTextBox;
//                if (tb == null)
//                    continue;
//
//                tb.FadeInSomeMore((Label) c.Controls[_indexOfLabel]);
			}
		}

		public void MoveInsertionPoint(int row)
		{
			if(0 > row || row >= RowCount)
			{
				throw new ArgumentOutOfRangeException("row", row, "row must be between 0 and Count-1 inclusive");
			}
//            Panel p = (Panel)ActualControls[RowToControlIndex(row)];
//            Control c = GetEditControlFromReferenceControl(p);
			Control c = GetControlFromPosition(1, row);

			WeSayTextBox tb;

			if (c is MultiTextControl)
			{
				MultiTextControl multText = (MultiTextControl)c;
				tb = multText.TextBoxes[0];
				tb.Focus();
				tb.Select(1000, 0);//go to end
			}
			else if (c is WeSayTextBox)
			{
				tb = (WeSayTextBox)c;
				tb.Focus();
				tb.Select(1000, 0);//go to end
			}
			else
			{
				c.Focus();
			}
		}

		public void OnBinding_ChangeOfWhichItemIsInFocus(object sender, CurrentItemEventArgs e)
		{
			ChangeOfWhichItemIsInFocus(sender, e);
		}

		/// <summary>
		/// for tests
		/// </summary>
		public Control GetEditControlFromRow(int row)
		{
			return GetControlFromPosition(1, row);
		}


		/// <summary>
		/// for tests
		/// </summary>
		public Label GetLabelControlFromRow(int row)
		{
			return (Label) GetControlFromPosition(0, row);
		}
	}
}
