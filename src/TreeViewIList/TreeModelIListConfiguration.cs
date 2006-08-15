using System;
using System.Collections;
using System.Collections.Generic;

namespace WeSay.TreeViewIList
{
	public delegate GLib.Value GetValueStrategyDelegate(object instance, int column);


	public class GetValueHelper{
		[System.Runtime.InteropServices.DllImport("libgobject-2.0-0.dll")]
		static extern void g_value_init(ref GLib.Value val, IntPtr type);

		static public GLib.Value Convert<T>(T o)
		{
			GLib.Value value = GLib.Value.Empty;
			GLib.GType gtype = GLib.GType.None;

			Type type = o.GetType();
			switch(Type.GetTypeCode(type)){
				case TypeCode.Boolean:
					gtype = GLib.GType.Boolean;
					break;
				case TypeCode.Byte:
					gtype = GLib.GType.UChar;
					break;
				case TypeCode.Char:
					gtype = GLib.GType.Char;
					break;
				case TypeCode.DateTime:
					throw new NotSupportedException("DateTime types cannot be converted to GTypes.");
				case TypeCode.DBNull:
					throw new NotSupportedException("DBNull cannot be converted to GTypes.");
				case TypeCode.Decimal: //Int128
					throw new NotSupportedException("Decimal types cannot be converted to GTypes.");
				case TypeCode.Double:
					gtype = GLib.GType.Double;
					break;
				case TypeCode.Empty:
					gtype = GLib.GType.Pointer;
					break;
				case TypeCode.Int16:
					gtype = GLib.GType.Int;
					break;
				case TypeCode.Int32:
					gtype = GLib.GType.Int;
					break;
				case TypeCode.Int64:
					gtype = GLib.GType.Int64;
					break;
				case TypeCode.Object:
					gtype = GLib.GType.Object;
					break;
				case TypeCode.SByte:
					gtype = GLib.GType.Char;
					break;
				case TypeCode.Single:
					gtype = GLib.GType.Float;
					break;
				case TypeCode.String:
					gtype = GLib.GType.String;
					break;
				case TypeCode.UInt16:
					gtype = GLib.GType.UInt;
					break;
				case TypeCode.UInt32:
					gtype = GLib.GType.UInt;
					break;
				case TypeCode.UInt64:
					gtype = GLib.GType.UInt64;
					break;
			}

			g_value_init(ref value, gtype.Val);
			value.Val = o;
			return value;
		}
	}

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
