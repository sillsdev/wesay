using System;

namespace WeSay.Core
{
  public class LexiconTreeSelection
  {
	private Gtk.TreeSelection selection;

	public event EventHandler Changed;

	public LexiconTreeSelection(Gtk.TreeSelection selection) {
	  this.selection = selection;
	  selection.Changed += new EventHandler(OnChanged);
	}

	private void OnChanged(object o, EventArgs args) {
	  if (Changed != null) {
		Changed(this, args);
	  }
	}

		public void Select (int index)
		{
			selection.SelectPath (View.LexiconModelStore.GetPath(index));
		}

		public void SelectPath (Gtk.TreePath path)
		{
			selection.SelectPath (path);
		}

		public Gtk.SelectionMode Mode {
			get {
				return Gtk.SelectionMode.Single;
			}
		}

		public LexiconTreeView View {
			get {
				return selection.TreeView as LexiconTreeView;
			}
		}

		public int Selected {
			get {
		Gtk.TreePath[] paths = selection.GetSelectedRows();
		if (paths.Length > 0) {
		  return this.View.LexiconModelStore.GetIndex(paths[0].Handle);
		}
		return 0;
			}
			set {
				Select (value);
			}
		}
	}
}
