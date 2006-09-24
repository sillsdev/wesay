using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WeSay.TreeViewIList
{
  [CLSCompliant(false)]
  public class IListComboBoxAdaptor : Gtk.ComboBox
	{
		private TreeModelIListAdaptor _model;
		private TreeModelIListConfiguration _modelConfiguration = new TreeModelIListConfiguration();

		public IListComboBoxAdaptor(IList store)
			: base(IntPtr.Zero)
		{
			this._modelConfiguration.DataSource = store;
			_model = new TreeModelIListAdaptor(_modelConfiguration);

			string[] names = { "model" };
			GLib.Value[] vals =  { new GLib.Value(_model) };
			CreateNativeObject(names, vals);
			vals[0].Dispose();
		}

		public IListComboBoxAdaptor()
			: base()
		{
		}

	private static class NativeMethods
	{
	  [DllImport("libgtk-x11-2.0.so")]
	  public static extern void gtk_combo_box_set_model(IntPtr raw, IntPtr model);
	  [DllImport("libgtk-win32-2.0-0.dll")]
	  public static extern IntPtr gtk_combo_box_get_type();
	}


		private void UpdateModel()
		{
			NativeMethods.gtk_combo_box_set_model(Handle, IntPtr.Zero);
			NativeMethods.gtk_combo_box_set_model(Handle, _model == null ? IntPtr.Zero : _model.Handle);
		}


		public static new GLib.GType GType
		{
			get
			{
			  return new GLib.GType(NativeMethods.gtk_combo_box_get_type());
			}
		}

		public new TreeModelIListAdaptor Model
		{
			get
			{
				return _model;
			}
			set
			{
				_model = value;
				UpdateModel();
			}
		}

		public IList DataSource
		{
			get
			{
				return this._modelConfiguration.DataSource;
			}
			set
			{
				this._modelConfiguration.DataSource = value;
				UpdateModel();
			}
		}

		public IList<GLib.GType> ColumnTypes
		{
			get
			{
				return (_modelConfiguration.ColumnTypes);
			}
			set
			{
				this._modelConfiguration.ColumnTypes = value;
			}
		}

		public ValueStrategyProvider GetValueStrategy
		{
			set
			{
				this._modelConfiguration.GetValueStrategy = value;
			}
		}
	}
}
