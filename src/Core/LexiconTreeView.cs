using System;

namespace WeSay.Core
{
	public class LexiconTreeView : Gtk.TreeView {

		TreeModelAdapter store;

		public LexiconTreeView (TreeModelAdapter store) : base (IntPtr.Zero)
		{
			string[] names = { "model" };
			GLib.Value[] vals =  { new GLib.Value (store) };
			CreateNativeObject (names, vals);
			vals [0].Dispose ();
			LexiconModelStore = store;
		}

		public LexiconTreeView () : base () {}

		[System.Runtime.InteropServices.DllImport("libgtk-win32-2.0-0.dll")]
		static extern void gtk_tree_view_set_model(IntPtr raw, IntPtr model);

		public TreeModelAdapter LexiconModelStore {
			get {
				return store;
			}
			set {
				store = value;
				gtk_tree_view_set_model (Handle, store == null ? IntPtr.Zero : store.Handle);
			}
		}

		public Gtk.TreeViewColumn AppendColumn (string title, Gtk.CellRenderer cell, Gtk.NodeCellDataFunc cell_data)
		{
			Gtk.TreeViewColumn col = new Gtk.TreeViewColumn ();
			col.Title = title;
			col.PackStart (cell, true);
			col.SetCellDataFunc (cell, cell_data);

			AppendColumn (col);
			return col;
		}
	}

}
