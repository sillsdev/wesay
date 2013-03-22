using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.i18n;
using Palaso.i18n;
using Palaso.Reporting;
using WeSay.UI.Buttons;
using WeSay.UI.TextBoxes;

namespace WeSay.UI
{
	/// <summary>
	/// This control displays a simple list of {label, control} pairs, one per row.
	/// It supports dynamically removing and inserting new rows, to support
	/// "ghost" fields
	/// </summary>
	public partial class DetailList: TableLayoutPanel, IMessageFilter, ILocalizableControl
	{
		/// <summary>
		/// Can be used to track which data item the user is currently editting, to,
		/// for example, hilight that piece in a preview control
		/// </summary>
		public event EventHandler<CurrentItemEventArgs> ChangeOfWhichItemIsInFocus = delegate { };

		private readonly int _indexOfLabel; // todo to const?
		private readonly int _indexOfWidget = 1; // todo to const?

		private bool _disposed;
		private readonly StackTrace _stackAtConstruction;

		public EventHandler MouseEnteredBounds;
		public EventHandler MouseLeftBounds;
		private bool _mouseIsInBounds;

		public DetailList()
		{
#if DEBUG
			_stackAtConstruction = new StackTrace();
#endif
			InitializeComponent();
			Application.AddMessageFilter(this);

			Name = "DetailList"; //for  debugging
			ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 0));
			ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));
			Dock = DockStyle.Fill;
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;

			MouseClick += OnMouseClick;

			//CellPaint += OnCellPaint;
			//var rand = new Random();
			//BackColor = Color.FromArgb(255, rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
		}

		public float LabelColumnWidth
		{
			get { return ColumnStyles[0].Width; }
			set
			{
				SuspendLayout();
				if (value != LabelColumnWidth)
				{
					ColumnStyles[0].Width = value;
				}
				foreach (var detailList in GetChildDetailLists())
				{
					detailList.LabelColumnWidth = value;
				}
				ResumeLayout();
			}

		}
		private List<DetailList> GetChildDetailLists()
		{
			var lists = new List<DetailList>();
			for (int row = 0; row < RowCount; row++)
			{
				var control = GetFirstControlInRow(row);
				if (control is DetailList)
				{
					var detailList = ((DetailList)control);
					lists.Add(detailList);
				}
			}
			return lists;
		}


		public int WidestLabelWidthWithMargin
		{
			get
			{
				var labels = new List<Control>();
				AppendControlsFromEachFieldRow(0, labels);
				return labels.Select(label => ((Label)label).GetPreferredSize(new Size(1000, 1000)).Width + label.Margin.Left + label.Margin.Right).Concat(new[] { 0 }).Max();
			}
		}

		private void OnCellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			var bounds = new Rectangle(e.CellBounds.Location, new Size(e.CellBounds.Width, e.CellBounds.Height - 10));
			e.Graphics.DrawRectangle(new Pen(Color.FromArgb(BackColor.ToArgb() ^ 0x808080)), bounds);
		}

		private void OnMouseClick(object sender, MouseEventArgs e)
		{
			Select();
			_clicked = true;
		}
		public void AddDetailList(DetailList detailList, int insertAtRow)
		{
			if (insertAtRow >= RowCount)
			{
				RowCount = insertAtRow + 1;
			}
			SetColumnSpan(detailList, 3);
			RowStyles.Add(new RowStyle(SizeType.AutoSize));
			detailList.LabelColumnWidth = LabelColumnWidth;
			detailList.MouseWheel += OnChildWidget_MouseWheel;
			detailList.Margin = new Padding(0, detailList.Margin.Top, 0, detailList.Margin.Bottom);
			Controls.Add(detailList, _indexOfLabel, insertAtRow);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			//we don't want to have focus, ourselves
			if (RowCount >0)
				MoveInsertionPoint(0);
		}
		/// <summary>
		/// Forces scroll bar to only have vertical scroll bar and not horizontal scroll bar by
		/// allowing enough space for the scroll bar to be added in (even though it then
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaddingChanged(EventArgs e)
		{
			VerifyNotDisposed();
			base.OnPaddingChanged(e);
			Padding = new Padding(Padding.Left,
								  Padding.Top,
								  Math.Max(Padding.Right, 20),
								  Padding.Bottom);
		}

		public Control AddWidgetRow(string label, bool isHeader, Control control)
		{
			return AddWidgetRow(label, isHeader, control, RowCount, false);
		}

		public int Count
		{
			get { return RowCount; }
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
			} //set
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

			RowCount = 0;
			RowStyles.Clear();
			while (Controls.Count > 0)
			{
				//  Debug.WriteLine("  VBoxClear() calling dispose on " + base.Controls[0].Name);
				Controls[0].Dispose();
			}
			Controls.Clear();
			// Debug.WriteLine("VBox " + Name + "   Clearing DONE");
		}

		public Control AddWidgetRow(string fieldLabel,
									bool isHeader,
									Control editWidget,
									int insertAtRow,
									bool isGhostField)
		{
			//Debug.WriteLine(String.Format("AddWidgetRow({0}, header={1}, , row={2}", fieldLabel, isHeader, insertAtRow));
			RowStyles.Add(new RowStyle(SizeType.AutoSize));

			if (insertAtRow >= RowCount)
			{
				RowCount = insertAtRow + 1;
			}
			else
			{
				if (insertAtRow == -1)
				{
					insertAtRow = RowCount;
				}
				else
				{
					// move down to make space for new row
					for (int row = RowCount;row > insertAtRow;row--)
					{
						for (int col = 0;col < ColumnCount;col++)
						{
							Control c = GetControlFromPosition(col, row - 1);
							if (c != null)
							{
								SetCellPosition(c, new TableLayoutPanelCellPosition(col, row));
								c.TabIndex = row;
							}
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
			//label.Font =StringCatalog.ModifyFontForLocalization(label.Font);
			label.Text = fieldLabel;
			label.AutoSize = true;

			int beforeHeadingPadding = (isHeader && insertAtRow != 0) ? 18 : 0;
			//        label.Top = 3 + beforeHeadingPadding;
			label.Margin = new Padding(label.Margin.Left,
									   beforeHeadingPadding,
									   label.Margin.Right,
									   label.Margin.Bottom);

			if (isGhostField)
			{
				label.ForeColor = Color.Gray;
			}

			Controls.Add(label, _indexOfLabel, insertAtRow);

			editWidget.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

			editWidget.KeyDown += OnEditWidget_KeyDown;
			editWidget.MouseWheel += OnChildWidget_MouseWheel;

			Debug.Assert(GetControlFromPosition(_indexOfWidget, insertAtRow) == null);

			//test
			editWidget.TabIndex = insertAtRow;

			editWidget.Margin = new Padding(editWidget.Margin.Left,
											beforeHeadingPadding,
											editWidget.Margin.Right,
											editWidget.Margin.Bottom);

			// At this point, multitext controls were being displayed on the screen.
			// We weren't able to get around this by simply using SuspendLayout and ResumeLayout
			//
			// http://msdn.microsoft.com/msdnmag/issues/06/03/WindowsFormsPerformance/
			// tells why:
			// "If the handles are created you will notice the difference, even if you use
			// SuspendLayout and ResumeLayout calls. SuspendLayout only prevents Windows
			// Forms OnLayout from being called. It will not prevent messages about size
			// changes from being sent and processed."
			//
			// we eventually get around this by making control invisible while it laysout
			// and then making it visible again. (See EntryViewControl.cs:RefreshEntryDetail)
			Controls.Add(editWidget, _indexOfWidget, insertAtRow);

			return editWidget;
		}

		private void OnChildWidget_MouseWheel(object sender, MouseEventArgs e)
		{
			OnMouseWheel(e);
		}

		private void OnEditWidget_KeyDown(object sender, KeyEventArgs e)
		{
			VerifyNotDisposed();
			OnKeyDown(e);
		}

		public void MoveInsertionPoint(int row)
		{
#if (!DEBUG) // not worth crashing over
			try
			{
#endif
			if (0 > row || row >= RowCount)
			{
				throw new ArgumentOutOfRangeException("row",
													  row,
													  "row must be between 0 and Count-1 inclusive");
			}

			Control c = GetControlFromPosition(1, row);

			Control tb;

			if (c is MultiTextControl)
			{
				MultiTextControl multText = (MultiTextControl) c;
				tb = multText.TextBoxes[0];
				tb.Focus();
				if(tb is WeSayTextBox)
					((WeSayTextBox) tb).Select(1000, 0); //go to end
			}
			else if (c is WeSayTextBox)
			{
				c.Focus();
				if(c is WeSayTextBox)
					((WeSayTextBox) c).Select(1000, 0); //go to end
			}
			else
			{
				c.Focus();
			}
#if (!DEBUG) // not worth crashing over
			}
			catch (Exception)
			{
			}
#endif
		}

		public void OnBinding_ChangeOfWhichItemIsInFocus(object sender, CurrentItemEventArgs e)
		{
			VerifyNotDisposed();
			ChangeOfWhichItemIsInFocus(sender, e);
		}

		/// <summary>
		/// for tests
		/// </summary>
		public Control GetEditControlFromRow(int fieldIndex)
		{
			var labels = new List<Control>();
			AppendControlsFromEachFieldRow(1, labels);
			return labels[fieldIndex];
		}

		/// <summary>
		/// for tests
		/// </summary>
		public Label GetLabelControlFromRow(int fieldIndex)
		{
			var labels = new List<Control>();
			AppendControlsFromEachFieldRow(0, labels);
			return (Label) labels[fieldIndex];
		}

		/// <summary>
		/// for tests
		/// </summary>
		public DeleteButton GetDeleteButton(int fieldIndex)
		{
			var deleteButtons = new List<Control>();
			AppendControlsFromEachFieldRow(2, deleteButtons);
			return (DeleteButton)deleteButtons[fieldIndex];
		}

		private void AppendControlsFromEachFieldRow(int columnIndex, List<Control> controls)
		{
			for (int row = 0; row < RowCount; row++)
			{
				var control = GetFirstControlInRow(row);
				if (control is DetailList)
				{
					var detailList = ((DetailList)control);
					detailList.AppendControlsFromEachFieldRow(columnIndex, controls);
				}
				else
				{
					controls.Add(GetControlFromPosition(columnIndex, row));
				}
			}
		}

		/// <summary>
		/// Tests
		/// </summary>
		/// <returns></returns>
		public int FieldCount
		{
			get
			{
				int fieldcount = 0;
				for (int row = 0; row < RowCount; row++)
				{
					var control = GetFirstControlInRow(row);
					if (control is DetailList)
					{
						fieldcount += ((DetailList) control).FieldCount;
					}
					else
					{
						fieldcount++;
					}
				}
				return fieldcount;
			}
		}

		private Control GetFirstControlInRow(int row)
		{
			return GetControlFromPosition(0, row);
		}

		~DetailList()
		{
			if (!_disposed)
			{
				string trace = "Was not recorded.";
				if (_stackAtConstruction != null)
				{
					trace = _stackAtConstruction.ToString();
				}
				throw new InvalidOperationException("Disposed not explicitly called on " +
													GetType().FullName + ".  Stack at creation was " +
													trace);
			}
		}

		protected void VerifyNotDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

		bool _clicked = false;

		//This message filter is used to determine wether the mouse in hovering over the DetailList or one
		//of its children. MouseLeave unfortunately fires when the mouse moves over a child control. This
		//is not behavior that we want which is why we are adding the MouseEnteredBounds and MouseLeftBounds
		//events
		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg != 0x0200) return false;
			if (_disposed) return false;
			var controlGettingMessage = FromHandle(m.HWnd);
			if (controlGettingMessage == null) return false;
			//if (!_clicked) return false;
			int x = m.LParam.ToInt32() & 0x0000FFFF;
			int y = (int)((m.LParam.ToInt32() & 0xFFFF0000) >> 16);
			var posRelativeToControl = new Point(x, y);
			var screenPos = controlGettingMessage.PointToScreen(posRelativeToControl);
			var posRelativeToThis = PointToClient(screenPos);

			//Console.WriteLine("MouseCoords: {0} {1} BoundsUpperLeft: {2}, {3}, {4}, {5}", posRelativeToThis.X, posRelativeToThis.Y, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);

			var mouseInBounds = ClientRectangle.Contains(posRelativeToThis);
			if (_mouseIsInBounds != mouseInBounds)
			{
				if (mouseInBounds)
				{
					if (MouseEnteredBounds != null)
					{
						MouseEnteredBounds(this, new EventArgs());
					}
					_mouseIsInBounds = true;
				}
				else
				{
					if (MouseLeftBounds != null)
					{
						MouseLeftBounds(this, new EventArgs());
					}
					_mouseIsInBounds = false;
				}
			}
			return false;
		}

		public void BeginWiring()
		{
			//do nothing
		}

		public void EndWiring()
		{
			if (!(Parent is DetailList))
			{
				//at this point the labels are all localized thanks to the localization helper
				//which means all of the label sizes have changed. We need to change the size of the first column accordingly
				LabelColumnWidth = WidestLabelWidthWithMargin;
			}
		}

		public bool ShouldModifyFont { get; private set; }
	}
}