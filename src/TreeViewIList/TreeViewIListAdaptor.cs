using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace WeSay.TreeViewIList
{

	public class TreeViewAdaptorIList : Gtk.TreeView
	{
		private TreeModelIListAdaptor _model;
		private TreeModelIListConfiguration _modelConfiguration = new TreeModelIListConfiguration();

		public TreeViewAdaptorIList(IList store)
			: base(IntPtr.Zero)
		{
			string[] names = { "model" };
			GLib.Value[] vals =  { new GLib.Value(store) };
			CreateNativeObject(names, vals);
			vals[0].Dispose();

			this._modelConfiguration.DataSource = store;
			Model = new TreeModelIListAdaptor(_modelConfiguration);
		}

		public TreeViewAdaptorIList()
			: base()
		{
			Model = new TreeModelIListAdaptor(_modelConfiguration);
		}

		[DllImport("libgtk-win32-2.0-0.dll")]
		static extern void gtk_tree_view_set_model(IntPtr raw, IntPtr model);

		private void UpdateModel()
		{
			gtk_tree_view_set_model(Handle, _model == null ? IntPtr.Zero : _model.Handle);
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