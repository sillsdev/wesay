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
	public partial class DetailList : VBox
	{
		/// <summary>
		/// Can be used to track which data item the user is currently editting, to,
		/// for example, hilight that piece in a preview control
		/// </summary>
		public event EventHandler<CurrentItemEventArgs> CurrentItemChanged = delegate
																			 {
																			 };

		private int _indexOfLabel = 0;
		private int _indexOfTextBox = 1;

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
		}


		public Control AddWidgetRow(string label, bool isHeader, Control control)
		{
			return AddWidgetRow(label, isHeader, control, Count);
		}


		public Control AddWidgetRow(string fieldLabel, bool isHeader, Control editWidget, int insertAtRow)
		{
			Panel panel = AddRowPanel(editWidget, fieldLabel, isHeader);

			AddControl(panel, insertAtRow);

			return panel;
		}

		class TestPanel : Panel
		{
			protected override void Dispose(bool disposing)
			{
				Debug.WriteLine("Disposing "+Name+"   Disposing="+disposing);
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
			int verticalPadding = 0;
			label.Top = verticalPadding+3+top;

			editWidget.Top = verticalPadding + top;
			editWidget.Width = 5;
			FixUpForMono(editWidget);
			editWidget.Left = label.Width + 10;
			editWidget.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			editWidget.KeyDown += new KeyEventHandler(editWidget_KeyDown);

			Panel panel = new TestPanel();
			panel.SuspendLayout();
			panel.Name = fieldLabel+"_panel of detailList";
			//            panel.Size = new Size(100, 10+editWidget.Height+(editWidget.Top-6) );//careful.. if width is too small, then editwidget grows to much.  Weird.
			panel.Size = new Size(100, 10+editWidget.Height+(editWidget.Top-6) );//careful.. if width is too small, then editwidget grows to much.  Weird.
			panel.Controls.Add(label);
			panel.Controls.Add(editWidget);
			panel.ResumeLayout(false);
			return panel;
		}

		void editWidget_KeyDown(object sender, KeyEventArgs e)
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

		public void OnBindingCurrentItemChanged(object sender, CurrentItemEventArgs e)
		{
			CurrentItemChanged(sender, e);
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
