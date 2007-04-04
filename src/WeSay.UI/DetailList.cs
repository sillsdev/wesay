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
	public partial class DetailList : VBoxFlow
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
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint
				| ControlStyles.UserPaint, true);

			Name = "DetailList";//for  debugging
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				//this.Margin = new Padding(5, 5, 5, 5); //not implemented inn mono 1.16
			}
			_fadeInTimer.Enabled = false;
			_fadeInTimer.Interval = 500;

		   this.AutoScroll = true; //but we need to make sure children are never wider than we are
//            this.VerticalScroll.Enabled = true;
//            this.VerticalScroll.Visible = true;
//
//
//            this.VerticalScroll.Maximum = 1000;
//            this.VerticalScroll.Minimum = 0;
//            this.VerticalScroll.Value = this.VerticalScroll.Minimum;
//            this.VerticalScroll.LargeChange = 30;
//            this.VerticalScroll.SmallChange = 10;
//            this.Scroll += new ScrollEventHandler(OnScroll);

			this.HScroll = false;
		}

		void OnScroll(object sender, ScrollEventArgs e)
		{
		}


		public Control AddWidgetRow(string label, bool isHeader, Control control)
		{
			return AddWidgetRow(label, isHeader, control, Count);
		}


		public Control AddWidgetRow(string fieldLabel, bool isHeader, Control editWidget, int insertAtRow)
		{
			//Debug.WriteLine(String.Format("AddWidgetRow({0}, header={1}, , row={2}", fieldLabel, isHeader, insertAtRow));
			Panel panel = AddRowPanel(editWidget, fieldLabel, isHeader);
			AddControl(panel, insertAtRow);
			return panel;
		}

		class TestPanel : Panel
		{
			protected override void Dispose(bool disposing)
			{
				//Debug.WriteLine("Disposing "+Name+"   Disposing="+disposing);
				base.Dispose(disposing);
			}
		}

		private Panel AddRowPanel(Control editWidget, string fieldLabel, bool isHeader)
		{
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
			label.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			int verticalPadding = 0;
			label.Top = verticalPadding+3+top;

			editWidget.Top = verticalPadding + top;
			int padding = 2;
			editWidget.Left = label.Left + label.Width + padding;
			editWidget.Width = (this.Width - editWidget.Left) - 20;
			FixUpForMono(editWidget);
			editWidget.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			editWidget.KeyDown += new KeyEventHandler(OnEditWidget_KeyDown);

//            FlexibleHeightPanel panel = new FlexibleHeightPanel(100, 10 + (editWidget.Top/*-6*/), editWidget);
			FlexibleHeightPanel panel = new FlexibleHeightPanel(this.Width, 10, editWidget);
			panel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top ;
		   // panel.SuspendLayout();
			panel.Name = fieldLabel+"_panel of detailList";
			//            panel.Size = new Size(100, 10+editWidget.Height+(editWidget.Top-6) );//careful.. if width is too small, then editwidget grows to much.  Weird.
		   // panel.Size = new Size(100, 10+editWidget.Height+(editWidget.Top-6) );//careful.. if width is too small, then editwidget grows to much.  Weird.
			panel.Controls.Add(label);
		   // panel.ResumeLayout(false);


			UpdateScrollBar();


			return panel;
		}

		private void UpdateScrollBar()
		{
			if (ActualControls.Count > 0)
			{
//                this.VerticalScroll.Maximum = ActualControls[ActualControls.Count - 1].Bottom;
//                this.VerticalScroll.Minimum = this.Height;
//                this.VerticalScroll.Value = this.VerticalScroll.Minimum;
			}
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

		//private int AddHorizontalRule(Panel parentPanel, bool isHeader, bool isFirstInList)
		//{

		//    Panel line = new Panel();
		//    line.BorderStyle = BorderStyle.None;//let it provide some padding
		//    line.ForeColor = System.Drawing.SystemColors.Control;//make it invisible
		//    line.Left = 2;
		//    line.Width = parentPanel.Width - 4;
		//    if (isHeader && !isFirstInList)
		//    {
		//        line.Top = 5; // <---- defines the padding
		//       // label.Font = new Font(label.Font, FontStyle.Bold);
		//        line.BackColor = System.Drawing.Color.LightGray;
		//        line.Height = 1;
		//     //   parentPanel.Height += 5;
		//    }
		//    else
		//    {   //it's now a placeholder to keep indexing simple
		//        line.Top = 0;
		//        line.Height = 0;
		//    }
		//    line.Anchor = AnchorStyles.Left | AnchorStyles.Right;
		//    //line.Dock = DockStyle.Top;
		//    parentPanel.Controls.Add(line);

		//    //Panel padding = new Panel();
		//    //padding.BackColor = System.Drawing.Color.Red;
		//    //padding.Height = 10;
		//    //padding.Top = 0;
		//    //parentPanel.Controls.Add(padding);

		//    return line.Bottom + 4;
		//}


		private void _fadeInTimer_Tick(object sender, EventArgs e)
		{
			foreach (Control c in ActualControls)
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
			if(0 > row || row >= Count)
			{
				throw new ArgumentOutOfRangeException("row", row, "row must be between 0 and Count-1 inclusive");
			}
			Panel p = (Panel)ActualControls[RowToControlIndex(row)];
			Control c = GetEditControlFromReferenceControl(p);
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
		public Control GetEditControlFromReferenceControl(Control rowControl)
		{
			return rowControl.Controls[_indexOfTextBox];
		}


		/// <summary>
		/// for tests
		/// </summary>
		public Label GetLabelControlFromReferenceControl(Control rowControl)
		{
			return (Label) rowControl.Controls[_indexOfLabel];
		}
	}
}
