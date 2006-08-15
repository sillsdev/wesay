using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace WeSay.TreeViewIList
{

	public class ComboBoxEntryIListAdaptor : Gtk.ComboBoxEntry
	{
		private TreeModelIListAdaptor _model;
		private TreeModelIListConfiguration _modelConfiguration = new TreeModelIListConfiguration();

		public ComboBoxEntryIListAdaptor(IList store)
			: base(IntPtr.Zero)
		{
			string[] names = { "model" };
			GLib.Value[] vals =  { new GLib.Value(store) };
			CreateNativeObject(names, vals);
			vals[0].Dispose();

			this._modelConfiguration.DataSource = store;
			Model = new TreeModelIListAdaptor(_modelConfiguration);
		}

		public ComboBoxEntryIListAdaptor()
			: base()
		{
			Model = new TreeModelIListAdaptor(_modelConfiguration);
		}


		[DllImport("libgtk-x11-2.0.so")]
		static extern void gtk_combo_box_set_model(IntPtr raw, IntPtr model);

		private void UpdateModel()
		{
			gtk_combo_box_set_model(Handle, IntPtr.Zero);
			gtk_combo_box_set_model(Handle, _model == null ? IntPtr.Zero : _model.Handle);
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

		public IList<GLib.GType> Column_Types
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

		public GetValueStrategyDelegate GetValueStrategy
		{
			set
			{
				this._modelConfiguration.GetValueStrategy = value;
			}
		}
	}
}