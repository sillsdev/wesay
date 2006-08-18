using System;
using System.Collections;
using System.Collections.Generic;

namespace WeSay.TreeViewIList
{
	public delegate object GetValueStrategyDelegate(object instance, int column);

	public class TreeModelIListConfiguration
	{
		IList _data;
		IList<GLib.GType> _columnTypes;
		GetValueStrategyDelegate _getValueStrategy;

		public IList DataSource
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
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_data = value;
			}
		}
		public IList<GLib.GType> ColumnTypes
		{
			get
			{
				if (_columnTypes == null)
				{
					_columnTypes = new List<GLib.GType>();
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
		public GetValueStrategyDelegate GetValueStrategy
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
