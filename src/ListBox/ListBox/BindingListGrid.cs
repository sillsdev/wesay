using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using SourceGrid3;

namespace ListBox
{
	/// <summary>
	/// A grid control that support load from a System.Data.DataView class, usually used for data binding.
	/// </summary>
	public class BindingListGrid : GridVirtual
	{
		private IBindingList _list;
//        private int _selectedRowIndex;
		public event EventHandler SelectedIndexChanged;

		public BindingListGrid()
		{
//            this._selectedRowIndex = 0;
			FixedRows = 0;
			FixedColumns = 0;

			//			Controller.AddController(new DataGridCellController());

			Selection.SelectionMode = GridSelectionMode.Row;
			//Controller.AddController(new NewRowController());
			Selection.EnableMultiSelection = false;

			Selection.FocusStyle = SourceGrid3.FocusStyle.RemoveFocusCellOnLeave;
			Selection.FocusRowLeaving += new RowCancelEventHandler(Selection_FocusRowLeaving);

			//nb: this comes at a time that the Selection.GetRowsIndex() is empty,
			//so it's not too useful!
			//ESA: I disagree this event is necessary in order to handle key presses in addition to mouse activity.
			Selection.Changed += new EventHandler(Selection_Changed);

			Selection.FocusBackColor = Color.FromArgb(75, 49, 106, 197);
		}

		void Selection_Changed(object sender, EventArgs e)
		{
			Selection selection = (Selection)sender;
			if(selection.ActivePosition.Column != Position.c_EmptyIndex &&
			   selection.ActivePosition.Row != Position.c_EmptyIndex)
			{
				if (SelectedIndexChanged != null)
				{
					SelectedIndexChanged.Invoke(this, e);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !IsDisposed)
			{
				Unbind();
			}
			base.Dispose(disposing);
		}


		/// <summary>
		/// the index into the binding list of the currently selected item.
		/// </summary>
		public int SelectedIndex
		{
			//jdh
			get
			{
				//return this._selectedRowIndex;
				int[] r = Selection.GetRowsIndex();
				if (r == null || r.Length == 0)
					return 0; //hack. Having start up problems if I dont return a valid index when the grid doesn't have a selection yet.
				//for some reason does grow Debug.Assert(r.Length == 1, "Don't expect multiple selected rows");
				return r[0];
			}
			set
			{
				if (_list == null)
				{
					return; //the designer will intialize us with this.
				}
				Debug.Assert(value < _list.Count);
			  //  this._selectedRowIndex = value;
				Selection.SelectRow(value, true);
				Selection.FocusRow(value);

				if (SelectedIndexChanged != null)
				{
					SelectedIndexChanged.Invoke(this, null);
				}

			}
		}
		/// <summary>
		/// the object at the index into the binding list of the currently selected item.
		/// </summary>
		public object SelectedObject
		{
			get
			{
				if (SelectedIndex < -1)
					return null;
				return _list[SelectedIndex];
			}
		}

		/// <summary>
		/// Change the cell currently under the mouse
		/// </summary>
		public override void ChangeMouseDownCell(Position p_MouseDownCell, Position p_MouseCell)
		{
			//hatton overroad this to implement a selection event
			base.ChangeMouseDownCell(p_MouseDownCell, p_MouseCell);
			if (p_MouseDownCell.Row > -1 && p_MouseDownCell.Row < _list.Count)
			{
				SelectedIndex = p_MouseDownCell.Row;
			}
		}

		/// <summary>
		/// Method used to create the rows object, in this class of type DataGridRows.
		/// </summary>
		protected override RowsBase CreateRowsObject()
		{
			return new DataGridRows(this);
		}

		/// <summary>
		/// Method used to create the columns object, in this class of type DataGridColumns.
		/// </summary>
		protected override ColumnsBase CreateColumnsObject()
		{
			return new DataGridColumns(this);
		}

		//		#region Cell Identifier abstract methods
		//		/// <summary>
		//		/// An abstract method that must return an object to identify a Position in the Grid. See also IdentifierToPosition. The identifier for Position.Empty must be null.
		//		/// The object returned must follow these rules:
		//		/// //The identifier of the same position is always the same
		//		/// object.Equals(Grid.IdentifierToPosition(new Position(x1, y1)), Grid.IdentifierToPosition(new Position(x1, y1))) == true
		//		/// //The identifier of a different position is always defferent
		//		/// object.Equals(Grid.IdentifierToPosition(new Position(x1, y1)), Grid.IdentifierToPosition(new Position(x2, y2))) == false
		//		/// </summary>
		//		/// <param name="position"></param>
		//		/// <returns></returns>
		//		public override object PositionToCellIdentifier(Position position)
		//		{
		//			if (position.IsEmpty())
		//				return null;
		//			else
		//				return new CellIdentifier(DataSource, Rows.IndexToDataSourceRow(position.Row), Columns.IndexToDataSourceColumn(position.Column), position.Row, position.Column);
		//		}
		//		/// <summary>
		//		/// An abstract method that must return a valid position if the identifier is valid, otherwise Position.Empty. The identifier object must be created with PositionToCellIdentifier. See also PositionToCellIdentifier. A null identifier must return Position.Empty.
		//		/// </summary>
		//		/// <param name="identifier"></param>
		//		/// <returns></returns>
		//		public override Position IdentifierToPosition(object identifier)
		//		{
		//			if (identifier == null)
		//				return Position.Empty;
		//			else
		//			{
		//				CellIdentifier cellId = ((CellIdentifier)identifier);
		//				if (object.Equals(cellId.DataSource, DataSource))
		//				{
		//					int col = cellId.ColumnIndex;
		//					int row = cellId.RowIndex;
		//					Position pos = new Position(cellId.RowIndex, cellId.ColumnIndex);
		//					if (cellId.Column != null)
		//						col = Columns.DataSourceColumnToIndex(cellId.Column);
		//					if (cellId.Row != null)
		//						row = Rows.DataSourceRowToIndex(cellId.Row);
		//
		//					return new Position(row, col);
		//				}
		//				else
		//					return Position.Empty;
		//			}
		//		}
		//		private class CellIdentifier
		//		{
		//			/// <summary>
		//			///
		//			/// </summary>
		//			/// <param name="dataSource"></param>
		//			/// <param name="row"></param>
		//			/// <param name="column"></param>
		//			/// <param name="rowIndex">Only used if row is null, otherwise is set to -1</param>
		//			/// <param name="columnIndex">Only used if column is null, otherwise is set to -1</param>
		//			public CellIdentifier(System.Data.DataView dataSource, System.Data.DataRowView row, System.Data.DataColumn column, int rowIndex, int columnIndex)
		//			{
		//				mDataSource = dataSource;
		//				mRow = row;
		//				mColumn = column;
		//				if (mRow == null)
		//					mRowIndex = rowIndex;
		//				if (mColumn == null)
		//					mColumnIndex = columnIndex;
		//			}
		//			private System.Data.DataView mDataSource;
		//			private System.Data.DataColumn mColumn;
		//			private System.Data.DataRowView mRow;
		//			private int mColumnIndex = -1;
		//			private int mRowIndex = -1;
		//
		//			public System.Data.DataView DataSource
		//			{
		//				get{return mDataSource;}
		//			}
		//			public System.Data.DataRowView Row
		//			{
		//				get{return mRow;}
		//			}
		//			public System.Data.DataColumn Column
		//			{
		//				get{return mColumn;}
		//			}
		//			/// <summary>
		//			/// Only used when Row is null.
		//			/// </summary>
		//			public int RowIndex
		//			{
		//				get{return mRowIndex;}
		//			}
		//			/// <summary>
		//			/// Only used when Column is null.
		//			/// </summary>
		//			public int ColumnIndex
		//			{
		//				get{return mColumnIndex;}
		//			}
		//
		//			public override int GetHashCode()
		//			{
		//				return mColumnIndex;
		//			}
		//
		//			public override bool Equals(object obj)
		//			{
		//				CellIdentifier other = obj as CellIdentifier;
		//				if (obj == null)
		//					return false;
		//				else
		//					return object.Equals(mDataSource, other.mDataSource) &&
		//						object.Equals(mColumn, other.mColumn) && object.Equals(mRow, other.mRow) &&
		//						mColumnIndex == other.mColumnIndex && mRowIndex == other.mRowIndex;
		//			}
		//		}
		//		#endregion


		//	private System.Data.DataView m_DataView;

		/// <summary>
		/// Gets or sets the DataView used for data binding.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IBindingList DataSource
		{
			get { return _list; }
			set
			{
				Unbind();
				_list = value;
				if (_list != null)
					Bind();

				if (_list.Count > 0)
				{
					SelectedIndex = 0;
				}
			}
		}

		protected virtual void Unbind()
		{
			//			if (m_DataView != null)
			//			{
			//				m_DataView.ListChanged -= new ListChangedEventHandler(m_DataView_ListChanged);
			//			}
			if (DataSource != null)
			{
				DataSource.ListChanged -= new ListChangedEventHandler(BindingListGrid_ListChanged);
			}
			//JDH this leads to accessing disposed dataSource: Rows.RowsChanged();
		}

		void BindingListGrid_ListChanged(object sender, ListChangedEventArgs e)
		{
			 Rows.RowsChanged();
			 InvalidateCells();
			 if (Selection.IsEmpty() && _list.Count > 0)
			{
				if (e.NewIndex == _list.Count)
				{
					//just deleted the last item in the list, so select the one above it
					Selection.SelectRow(e.NewIndex -1, true); ;
				}
				else
				{
					Selection.SelectRow(0, true); ;
				}
			  //  SelectedIndex = 0;
			}
		}

		protected virtual void Bind()
		{
			if (Columns.Count == 0)
				CreateColumns();

			DataSource.ListChanged += new ListChangedEventHandler(BindingListGrid_ListChanged);
			Rows.RowsChanged();
		}

		/// <summary>
		/// Gets the rows information as a DataGridRows object.
		/// </summary>
		public new DataGridRows Rows
		{
			get { return (DataGridRows)base.Rows; }
		}

		/// <summary>
		/// Gets the columns informations as a DataGridColumns object.
		/// </summary>
		public new DataGridColumns Columns
		{
			get { return (DataGridColumns)base.Columns; }
		}


		/// <summary>
		/// Gets a specified Cell by its row and column.
		/// </summary>
		/// <param name="p_iRow"></param>
		/// <param name="p_iCol"></param>
		/// <returns></returns>
		public override SourceGrid3.Cells.ICellVirtual GetCell(int p_iRow, int p_iCol)
		{
			try
			{
				if (_list != null)
				{
					if (p_iRow < FixedRows)
						return Columns[p_iCol].HeaderCell;
					else
						return Columns[p_iCol].GetDataCell(p_iRow);
				}
				else
					return null;
			}
			catch (Exception err)
			{
				System.Diagnostics.Debug.Assert(false, err.Message);
				return null;
			}
		}

		protected override void OnSortingRangeRows(SortRangeRowsEventArgs e)
		{

		}

		public void CreateColumns()
		{
			Columns.Clear();
			if (DataSource != null)
			{
				System.Data.DataColumn col = new System.Data.DataColumn();
				Columns.Insert(0, BindingListGridColumn.Create(this, col, false));
			}
		}

		/// <summary>
		/// Gets or sets the selected DataRowView.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public System.Data.DataRowView[] SelectedDataRows
		{
			get
			{
				//if (m_DataView == null)
				//    return new System.Data.DataRowView[0];

				//int[] rowsSel = Selection.GetRowsIndex();

				//int count = 0;
				//for (int i = 0; i < rowsSel.Length; i++)
				//{
				//    System.Data.DataRowView rowView = Rows.IndexToDataSourceRow(rowsSel[i]);
				//    if (rowView != null)
				//        count++;
				//}

				//System.Data.DataRowView[] dataRows = new System.Data.DataRowView[count];
				//int indexRows = 0;
				//for (int i = 0; i < rowsSel.Length; i++)
				//{
				//    System.Data.DataRowView rowView = Rows.IndexToDataSourceRow(rowsSel[i]);
				//    if (rowView != null)
				//    {
				//        dataRows[indexRows] = rowView;
				//        indexRows++;
				//    }
				//}
				//return dataRows;
				return new System.Data.DataRowView[0];
			}
			set
			{
				//ORIGINAL
				//if (m_DataView != null && value != null)
				//{
				//    for (int i = 0; i < value.Length; i++)
				//    {
				//        for (int r = FixedRows; r < Rows.Count; r++)
				//        {
				//            System.Data.DataRowView rowView = Rows.IndexToDataSourceRow(r);

				//            if (rowView != null && rowView.Row == value[i].Row)
				//            {
				//                Selection.SelectRow(r, true);
				//                break;
				//            }
				//        }
				//    }
				//}
			}
		}

		public override void OnGridKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnGridKeyDown(e);

			//			if (e.KeyCode == System.Windows.Forms.Keys.Delete &&
			//				_list != null &&
			//				//_list.AllowDelete &&
			//				e.Handled == false &&
			//				mDeleteRowsWithDeleteKey)
			//			{
			//				System.Data.DataRowView[] rows = SelectedDataRows;
			//				if (rows != null && rows.Length > 0)
			//					DeleteSelectedRows();
			//
			//				e.Handled = true;
			//			}
			//			else if (e.KeyCode == System.Windows.Forms.Keys.Escape &&
			//				e.Handled == false &&
			//				mCancelEditingWithEscapeKey)
			//			{
			//				EndEditingRow(true);
			//
			//				e.Handled = true;
			//			}
		}



		/// <summary>
		/// Delete all the selected rows.
		/// </summary>
		/// <returns>Returns true if one or more row is deleted otherwise false.</returns>
		public virtual bool DeleteSelectedRows()
		{
			return false;
		}

		/// <summary>
		/// AutoSize the columns based on the visible range and autosize te rows based on the first row. (because there is only one height available)
		/// </summary>
		public override void AutoSize()
		{
			Columns.AutoSizeView();


// original code from sourceGrid           if (Rows.Count > 1)
//                Rows.AutoSizeRow(1);
//            else if (Rows.Count > 0)
//                Rows.AutoSizeRow(0);

			//hack that won't last us long. We really need to base this on the font, not some string.
			bool autosized = false;
			foreach (object o in _list)
			{
				string s = o.ToString();
				if (s == null || s == String.Empty)
					continue;
				Rows.AutoSizeRow(_list.IndexOf(o));
				autosized = true;
				break;
			}
			if(autosized == false)
			{
				Rows.RowHeight = 40;
			}
		}

		private void Selection_FocusRowLeaving(object sender, RowCancelEventArgs e)
		{
			if (e.Row == EditingRow)
			{
				try
				{
					EndEditingRow(false);
				}
				catch (Exception exc)
				{
					OnUserException(new ExceptionEventArgs(new EndEditingException(exc)));

					e.Cancel = true;
				}
			}
		}

		/// <summary>
		/// Check if the specified row is the active row (focused), return false if it is not the active row. Then call the BeginEdit on the associated DataRowView. Add a row to the DataView if required. Returns true if the method sucesfully call the BeginEdit and set the EditingRow property.
		/// </summary>
		/// <param name="gridRow"></param>
		/// <returns></returns>
		public bool BeginEditRow(int gridRow)
		{
			return true;
		}


		private struct EditingInfo
		{
			public EditingInfo(System.Data.DataRowView dataRow, int editingRow)
			{
				mEditingDataRow = dataRow;
				mEditingRow = editingRow;
			}
			/// <summary>
			/// Nota: Devo uasre la stessa istanza della DataView sia per il BeginEdit che per l'EndEdit, e questa viene ricreata ad ogni chiamata all'indexer della DataView, deve quindi essere mantenuto in memoria quello in editing.
			/// </summary>
			public System.Data.DataRowView mEditingDataRow;
			public int mEditingRow;
		}
		private EditingInfo mEditingInfo = new EditingInfo(null, -1);
		/// <summary>
		/// Gets the currently editing row. Null if no row is in editing.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public System.Data.DataRowView EditingDataRow
		{
			get { return mEditingInfo.mEditingDataRow; }
		}
		/// <summary>
		/// Gets the currently editing row. Null if no row is in editing.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int EditingRow
		{
			get { return mEditingInfo.mEditingRow; }
		}


		/// <summary>
		/// Calls the CancelEdit or the EndEdit on the editing Row and set to null the editing row.
		/// </summary>
		/// <param name="cancel"></param>
		public void EndEditingRow(bool cancel)
		{
			if (EditingDataRow != null)
			{
				if (cancel)
				{
					EditingDataRow.CancelEdit();
				}

				//These lines can throw an error if the row is not valid
				EditingDataRow.EndEdit();

				mEditingInfo = new EditingInfo(null, -1);
			}
		}
	}


}
