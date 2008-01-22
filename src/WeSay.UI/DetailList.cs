using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation;

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
		public event EventHandler<CurrentItemEventArgs> ChangeOfWhichItemIsInFocus = delegate{};

		private readonly int _indexOfLabel = 0;
		private readonly int _indexOfWidget = 1;

		private bool _disposed = false;
		private StackTrace _stackAtConstruction;
		private static int _instanceCountForDebugging=0;

		public DetailList()
		{
			++_instanceCountForDebugging;
			if(_instanceCountForDebugging >1)
			{//not necessarily bad, just did this while looking into ws-554
				Palaso.Reporting.Logger.WriteEvent("Detail List Count ={0}", _instanceCountForDebugging);
#if DEBUG
				Debug.Assert(_instanceCountForDebugging < 3,"ws-554 reproduction?");
#endif
			}

#if DEBUG

			_stackAtConstruction = new StackTrace();
#endif
			InitializeComponent();
			SetStyle(ControlStyles.OptimizedDoubleBuffer |
					 ControlStyles.AllPaintingInWmPaint |
					 ControlStyles.UserPaint,
					 true);

			Name = "DetailList";//for  debugging


		   if (_indexOfLabel == 0)
		   {
			 ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			 ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
		   }
		   else
		   {
			 ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			 ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
		   }
		}



		protected override void OnPaddingChanged(EventArgs e)
		{
			VerifyNotDisposed();
			base.OnPaddingChanged(e);
			Padding =
					new Padding(Math.Max(Padding.Left, 20),
								Padding.Top,
								Padding.Right,
								Padding.Bottom);
		}

		public Control AddWidgetRow(string label, bool isHeader, Control control)
		{
			return AddWidgetRow(label, isHeader, control, RowCount, false);
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

		public Control AddWidgetRow(string fieldLabel, bool isHeader, Control editWidget, int insertAtRow, bool isGhostField)
		{
			//Debug.WriteLine(String.Format("AddWidgetRow({0}, header={1}, , row={2}", fieldLabel, isHeader, insertAtRow));
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
							SetCellPosition(c, new TableLayoutPanelCellPosition(col, row));
							c.TabIndex = row;
						}
					}
				}
				RowCount++;
			}

			Label label = new Label();
			if (isHeader)
			{
				label.Font = new Font(StringCatalog.LabelFont /* label.Font*/, FontStyle.Bold);
			}
			label.Font =StringCatalog.ModifyFontForLocalization(label.Font);
			label.Text = fieldLabel;
			label.Size = new Size(75, 50);
			label.AutoSize = true;

			int beforeHeadingPadding = isHeader ? 8 : 0;
			label.Top = 3 + beforeHeadingPadding;

			if (isGhostField)
			{
				label.ForeColor = System.Drawing.Color.Gray;
			}

			Controls.Add(label, _indexOfLabel, insertAtRow);
			editWidget.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

			editWidget.KeyDown += new KeyEventHandler(OnEditWidget_KeyDown);

			Debug.Assert(GetControlFromPosition(_indexOfWidget, insertAtRow) == null);

			//test
			editWidget.TabIndex = insertAtRow;

			// At this point, multitext controls were being displayed on the screen.
			// We weren't able to get around this by simply using SuspendLayout and ResumeLayout
			// But finally made MultiText default to have a size of 1,1.
			Controls.Add(editWidget, _indexOfWidget, insertAtRow);

		   return editWidget;
		}

		private void OnEditWidget_KeyDown(object sender, KeyEventArgs e)
		{
			VerifyNotDisposed();
			OnKeyDown(e);
		}

		public void MoveInsertionPoint(int row)
		{
			try
			{
				if (0 > row || row >= RowCount)
				{
					throw new ArgumentOutOfRangeException("row",
														  row,
														  "row must be between 0 and Count-1 inclusive");
				}
				//            Panel p = (Panel)ActualControls[RowToControlIndex(row)];
				//            Control c = GetEditControlFromReferenceControl(p);
				Control c = GetControlFromPosition(1, row);

				WeSayTextBox tb;

				if (c is MultiTextControl)
				{
					MultiTextControl multText = (MultiTextControl) c;
					tb = multText.TextBoxes[0];
					tb.Focus();
					tb.Select(1000, 0); //go to end
				}
				else if (c is WeSayTextBox)
				{
					tb = (WeSayTextBox) c;
					tb.Focus();
					tb.Select(1000, 0); //go to end
				}
				else
				{
					c.Focus();
				}
			}
			catch(Exception error)
			{
#if DEBUG // not worth crashing over
				throw;
#endif
			}
		}

		public void OnBinding_ChangeOfWhichItemIsInFocus(object sender, CurrentItemEventArgs e)
		{
			VerifyNotDisposed();
			ChangeOfWhichItemIsInFocus(sender, e);
		}

		/// <summary>
		/// for tests
		/// </summary>
		public Control GetEditControlFromRow(int row)
		{
			return GetControlFromPosition(_indexOfWidget, row);
		}

		/// <summary>
		/// for tests
		/// </summary>
		public Label GetLabelControlFromRow(int row)
		{
			return (Label)GetControlFromPosition(_indexOfLabel, row);
		}

		~DetailList()
		{
			if (!this._disposed)
			{
				string trace = "Was not recorded.";
				if (_stackAtConstruction != null)
				{
					trace = _stackAtConstruction.ToString();
			}
				throw new InvalidOperationException("Disposed not explicitly called on " + GetType().FullName + ".  Stack at creation was "+trace);
		}
		}




		protected void VerifyNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

	}
}
