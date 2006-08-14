using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.IListTreeView
{
	public delegate GLib.Value GetValueStrategy<T>(T instance, int column);

	public class IListTreeModelConfiguration<T>
	{
		IList<T> _data;
		List<GLib.GType> _columnTypes;
		GetValueStrategy<T> _getValueStrategy;

		public IList<T> DataSource
		{
			get
			{
				if (_data == null)
				{
					throw new InvalidOperationException("DataSource has never been set");
				}
				return _data;
			}
			set
			{
				if (_data == null)
				{
					throw new ArgumentNullException();
				}
				_data = value;
			}
		}
		public List<GLib.GType> ColumnTypes
		{
			get
			{
				if (_columnTypes == null)
				{
					throw new InvalidOperationException("ColumnTypes has never been set");
				}
				return _columnTypes;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_columnTypes = value;
			}
		}
		public GetValueStrategy<T> GetValueStrategy
		{
			get
			{
				if (_getValueStrategy == null)
				{
					throw new InvalidOperationException("GetValueStrategy has never been set");
				}

				return _getValueStrategy;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_getValueStrategy = value;
			}
		}
	}
}
