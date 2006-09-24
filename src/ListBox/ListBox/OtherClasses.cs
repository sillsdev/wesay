using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SourceGrid3;

namespace ListBox
{
	#region Columns
	public class DataGridColumns : ColumnInfoCollection
	{
		public DataGridColumns(BindingListGrid grid)
			: base(grid)
		{
		}

		public new BindingListGrid Grid
		{
			get { return (BindingListGrid)base.Grid; }
		}

		public new BindingListGridColumn this[int index]
		{
			get { return (BindingListGridColumn)base[index]; }
		}

		public void Insert(int index, BindingListGridColumn bindingListGridColumn)
		{
			base.InsertRange(index, new SourceGrid3.ColumnInfo[] { bindingListGridColumn });
		}

		/// <summary>
		/// Return the DataColumn object for a given grid column index. Return null if not applicable, for example if the column index requested is a FixedColumns of an unbound column
		/// </summary>
		/// <param name="gridColumnIndex"></param>
		/// <returns></returns>
		public System.Data.DataColumn IndexToDataSourceColumn(int gridColumnIndex)
		{
			return Grid.Columns[gridColumnIndex].DataColumn;
		}
		/// <summary>
		/// Returns the index for a given DataColumn. -1 if not valid.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		public int DataSourceColumnToIndex(System.Data.DataColumn column)
		{
			for (int i = 0; i < Grid.Columns.Count; i++)
			{
				if (Grid.Columns[i].DataColumn == column)
					return i;
			}

			return -1;
		}
	}

	#endregion

	#region Models

	public class BindingListGridRowHeaderModel : SourceGrid3.Cells.Models.IValueModel
	{
		public BindingListGridRowHeaderModel()
		{
		}
		#region IValueModel Members
		public object GetValue(CellContext cellContext)
		{
			  return null;
		}

		public void SetValue(CellContext cellContext, object p_Value)
		{
			throw new NotSupportedException();
		}
		#endregion
	}
	#endregion

	#region Rows
	/// <summary>
	/// This class implements a RowsSimpleBase class using a DataView bound mode for row count.
	/// </summary>
	public class DataGridRows : RowsSimpleBase
	{
		public DataGridRows(BindingListGrid grid)
			: base(grid)
		{
		}

		public new BindingListGrid Grid
		{
			get { return (BindingListGrid)base.Grid; }
		}

		/// <summary>
		/// Gets the number of row of the current DataView. Usually this value is automatically calculated and cannot be changed manually.
		/// </summary>
		public override int Count
		{
			get
			{
				if (Grid.DataSource != null)
					//					if (Grid.DataSource.AllowNew)
					//						return Grid.DataSource.Count + Grid.FixedRows + 1;
					//					else
					return Grid.DataSource.Count + Grid.FixedRows;
				else
					return Grid.FixedRows;
			}
		}

		//		/// <summary>
		//		/// Returns the DataView index for the specified grid row index.
		//		/// </summary>
		//		/// <param name="gridRowIndex"></param>
		//		/// <returns></returns>
		//		public int IndexToDataSourceIndex(int gridRowIndex)
		//		{
		//			int dataIndex = gridRowIndex - Grid.FixedRows;
		//			return dataIndex;
		//		}
		//		/// <summary>
		//		/// Returns the DataRowView object for a given grid row index. Return null if not applicable, for example if the DataSource is null or if the row index requested is a FixedRows
		//		/// </summary>
		//		/// <param name="gridRowIndex"></param>
		//		/// <returns></returns>
		//		public System.Data.DataRowView IndexToDataSourceRow(int gridRowIndex)
		//		{
		//			int dataIndex = IndexToDataSourceIndex(gridRowIndex);
		//
		//			//Verifico che l'indice sia valido, perchè potrei essere in un caso in cui le righe non combaciano (magari a seguito di un refresh non ancora completo)
		//			if (Grid.DataSource != null &&
		//				dataIndex >= 0 && dataIndex < Grid.DataSource.Count)
		//				return Grid.DataSource[dataIndex];
		//			else
		//				return null;
		//		}
		//
		//		/// <summary>
		//		/// Returns the index for a given DataRowView. -1 if not valid.
		//		/// </summary>
		//		/// <param name="row"></param>
		//		/// <returns></returns>
		//		public int DataSourceRowToIndex(System.Data.DataRowView row)
		//		{
		//			//TODO Da ottimizzare, non essendoci un metodo per cercare l'indice della DataView (ma solo della DataTable devo ciclare su tutti gli indici)
		//			if (Grid.DataSource != null)
		//			{
		//				for (int i = 0; i < Grid.DataSource.Count; i++)
		//				{
		//					if (Grid.DataSource[i].Row == row.Row)
		//						return i;
		//				}
		//			}
		//
		//			return -1;
		//		}
	}
	#endregion


	#region Cells
	/// <summary>
	/// A cell header used for the columns. Usually used in the HeaderCell property of a BindingListGridColumn.
	/// </summary>
	public class DataGridColumnHeader : SourceGrid3.Cells.Virtual.ColumnHeader
	{
		public DataGridColumnHeader(string pCaption)
		{
			Model.AddModel(new SourceGrid3.Cells.Models.ValueModel(pCaption));
		}
	}

	/// <summary>
	/// A cell used as left row selector. Usually used in the DataCell property of a BindingListGridColumn. If FixedColumns is grater than 0 and the columns are automatically created then the first column is created of this type.
	/// </summary>
	public class DataGridRowHeader : SourceGrid3.Cells.Virtual.RowHeader
	{
		public DataGridRowHeader()
		{
			Model.AddModel(new BindingListGridRowHeaderModel());
			ResizeEnabled = false;
		}
	}

	/// <summary>
	/// A cell used for the top/left cell when using DataGridRowHeader.
	/// </summary>
	public class DataGridHeader : SourceGrid3.Cells.Virtual.Header
	{
		public DataGridHeader()
		{
			Model.AddModel(new SourceGrid3.Cells.Models.NullValueModel());
		}
	}
	#endregion

	#region Controller
	public class DataGridCellController : SourceGrid3.Cells.Controllers.ControllerBase
	{
		public override void OnValueChanging(CellContext sender, DevAge.ComponentModel.ValueEventArgs e)
		{
			base.OnValueChanging(sender, e);

			//BeginEdit on the row, set the Cancel = true if failed to start editing.
			bool success = ((BindingListGrid)sender.Grid).BeginEditRow(sender.Position.Row);
			if (success == false)
				throw new SourceGridException("Failed to editing row " + sender.Position.Row.ToString());
		}

		public override void OnEditStarting(CellContext sender, CancelEventArgs e)
		{
			base.OnEditStarting(sender, e);

			//BeginEdit on the row, set the Cancel = true if failed to start editing.
			bool success = ((BindingListGrid)sender.Grid).BeginEditRow(sender.Position.Row);
			e.Cancel = !success;
		}
	}
	#endregion


	[Serializable]
	public class EndEditingException : SourceGridException
	{
		public EndEditingException(Exception innerException)
			:
			base(innerException.Message, innerException)
		{
		}
		protected EndEditingException(System.Runtime.Serialization.SerializationInfo p_Info, System.Runtime.Serialization.StreamingContext p_StreamingContext)
			:
			base(p_Info, p_StreamingContext)
		{
		}
	}
}
