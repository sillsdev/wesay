using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WeSay.IListTreeView
{
	public class IListComboBoxAdaptor<T> : Gtk.ComboBox
	{
		private IListTreeModelAdaptor<T> _model;
		private IListTreeModelConfiguration<T> _modelConfiguration;

		public IListComboBoxAdaptor(IList<T> store)
			: base(IntPtr.Zero)
		{
			string[] names = { "model" };
			GLib.Value[] vals =  { new GLib.Value(store) };
			CreateNativeObject(names, vals);
			vals[0].Dispose();

			this._modelConfiguration.DataSource = store;
			Model = new IListTreeModelAdaptor<T>(_modelConfiguration);
		}

		public IListComboBoxAdaptor()
			: base()
		{
			Model = new IListTreeModelAdaptor<T>(_modelConfiguration);
		}


		[DllImport("libgtk-x11-2.0.so")]
		static extern void gtk_combo_box_set_model(IntPtr raw, IntPtr model);

		private void UpdateModel()
		{
			gtk_combo_box_set_model(Handle, IntPtr.Zero);
			gtk_combo_box_set_model(Handle, _model == null ? IntPtr.Zero : _model.Handle);
		}
		public new IListTreeModelAdaptor<T> Model
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

		public IList<T> DataSource
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

		public List<GLib.GType> Column_Types
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

		public GetValueStrategy<T> GetValueStrategy
		{
			set
			{
				this._modelConfiguration.GetValueStrategy = value;
			}
		}
	}
}
