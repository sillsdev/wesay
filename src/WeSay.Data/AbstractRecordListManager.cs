using System;

namespace WeSay.Data
{
	public class DeletedItemEventArgs:EventArgs
	{
		private readonly object _itemDeleted;
		public DeletedItemEventArgs(object itemDeleted)
		{
			_itemDeleted = itemDeleted;
		}
		public object ItemDeleted
		{
			get { return this._itemDeleted; }
		}
	}
}
