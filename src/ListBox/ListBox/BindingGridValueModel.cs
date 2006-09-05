using System.Collections;
using SourceGrid3;

namespace ListBox
{
	/// <summary>
	/// A Model of type IValueModel used for binding the value to a specified column of a DataView. Used for the BindingListGrid control.
	/// </summary>
	public class BindingGridValueModel : SourceGrid3.Cells.Models.IValueModel
	{
		public BindingGridValueModel()
		{
		}

		#region IValueModel Members

		public object GetValue(CellContext cellContext)
		{
			BindingListGrid grid = (BindingListGrid)cellContext.Grid;
			IList list = (IList)((BindingListGrid)cellContext.Grid).DataSource;

			object o = list[cellContext.Position.Row];
			return o;
		}

		public void SetValue(CellContext cellContext, object p_Value)
		{

		}
		#endregion
	}
}