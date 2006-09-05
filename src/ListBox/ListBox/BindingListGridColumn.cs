using SourceGrid3;

namespace ListBox
{
	/// <summary>
	/// A ColumnInfo derived class used to store column informations for a BindingListGrid control. Mantains the cell used on this grid and manage the binding to the DataSource using a DataGridValueModel class.
	/// </summary>
	public class BindingListGridColumn : ColumnInfo
	{
		private System.Data.DataColumn m_DataColumn;
		private SourceGrid3.Cells.ICellVirtual m_HeaderCell;
		private SourceGrid3.Cells.ICellVirtual m_DataCell;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="grid"></param>
		public BindingListGridColumn(BindingListGrid grid):base(grid)
		{
		}

		/// <summary>
		/// Constructor. Create a BindingListGridColumn class.
		/// </summary>
		/// <param name="grid"></param>
		/// <param name="dataColumn">The DataColumn specified for this column. Can be null if not binded to a datasource Column.</param>
		/// <param name="headerCell"></param>
		/// <param name="dataCell"></param>
		public BindingListGridColumn(BindingListGrid grid, System.Data.DataColumn dataColumn, SourceGrid3.Cells.ICellVirtual headerCell, SourceGrid3.Cells.ICellVirtual dataCell):base(grid)
		{
			m_DataColumn = dataColumn;
			m_HeaderCell = headerCell;
			m_DataCell = dataCell;
		}

		/// <summary>
		/// Create a BindingListGridColumn with special cells used for RowHeader, usually used when FixedColumns is 1 for the first column.
		/// </summary>
		/// <param name="grid"></param>
		/// <returns></returns>
		public static BindingListGridColumn CreateRowHeader(BindingListGrid grid)
		{
			return new BindingListGridColumn(grid, null, new DataGridHeader(), new DataGridRowHeader());
		}

		/// <summary>
		/// Create a BindingListGridColumn class with the appropriate cells based on the type of the column.
		/// </summary>
		/// <param name="grid"></param>
		/// <param name="dataColumn"></param>
		/// <param name="editable"></param>
		/// <returns></returns>
		public static BindingListGridColumn Create(BindingListGrid grid, System.Data.DataColumn dataColumn, bool editable)
		{
			SourceGrid3.Cells.ICellVirtual cell;
			if (dataColumn.DataType == typeof(bool))
				cell = new ListBox.CheckBox(dataColumn);
			else
			{
				cell = new ListBox.Cell(dataColumn);
				cell.Editor = Utilities.CreateEditor(dataColumn.DataType);
			}

			if (cell.Editor != null) //Can be null for special DataType like Object
			{
				//cell.Editor.AllowNull = dataColumn.AllowDBNull;
				cell.Editor.AllowNull = true; //The columns now support always DbNull values because the validation is done at row level by the DataTable itself.
				cell.Editor.EnableEdit = editable;
			}

			return Create(grid, dataColumn, dataColumn.Caption, cell);
		}

		public static BindingListGridColumn Create(BindingListGrid grid, System.Data.DataColumn dataColumn, string caption, SourceGrid3.Cells.ICellVirtual cell)
		{
			return new BindingListGridColumn(grid, dataColumn, new DataGridColumnHeader(caption), cell);
		}

		public new BindingListGrid Grid
		{
			get{return (BindingListGrid)base.Grid;}
		}

		/// <summary>
		/// Gets or sets the DataColumn specified for this column. Can be null if not binded to a datasource Column.
		/// This filed is used for example to support sorting.
		/// </summary>
		public System.Data.DataColumn DataColumn
		{
			get{return m_DataColumn;}
			set{m_DataColumn = value;}
		}

		public SourceGrid3.Cells.ICellVirtual HeaderCell
		{
			get{return m_HeaderCell;}
			set{m_HeaderCell = value;}
		}

		public SourceGrid3.Cells.ICellVirtual DataCell
		{
			get{return m_DataCell;}
			set{m_DataCell = value;}
		}

		/// <summary>
		/// Gets the ICellVirtual for the current column and the specified row. Override this method to provide custom cells, based on the row informations.
		/// </summary>
		/// <param name="gridRow"></param>
		/// <returns></returns>
		public virtual SourceGrid3.Cells.ICellVirtual GetDataCell(int gridRow)
		{
			return m_DataCell;
		}
	}
}