using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.TreeViewIList
{
	public class TreeViewIListSelection
	{
		private Gtk.TreeSelection selection;

		public event EventHandler Changed;

		public TreeViewIListSelection(Gtk.TreeSelection selection)
		{
			this.selection = selection;
			selection.Changed += new EventHandler(OnSelectionChanged);
		}

		private void OnSelectionChanged(object sender, EventArgs e)
		{

			EventHandler changed = Changed;
			if (changed != null)
			{
				changed(this, e);
			}
		}

		public void Select(int index)
		{
			Gtk.TreePath path = TreeView.Model.GetPath(index);
			selection.SelectPath(path);
			TreeView.ScrollToCell(path, TreeView.Columns[0], false, 0, 0);
		}

		public Gtk.SelectionMode Mode
		{
			get
			{
				return Gtk.SelectionMode.Single;
			}
		}

		public TreeViewAdaptorIList TreeView
		{
			get
			{
				return selection.TreeView as TreeViewAdaptorIList;
			}
		}
		public int Selected
		{
			get
			{
				Gtk.TreePath[] paths = selection.GetSelectedRows();
				if (paths.Length > 0)
				{
					return TreeView.Model.GetIndex(paths[0].Handle);
				}
				return 0;
			}
			set
			{
				Select(value);
			}
		}
	}
}
