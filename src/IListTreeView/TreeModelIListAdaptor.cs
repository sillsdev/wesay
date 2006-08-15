using System;
using System.Runtime.InteropServices;

namespace WeSay.IListTreeView
{
	public class IListTreeModelAdaptor : GLib.Object
	{
		private IListTreeModelConfiguration _configuration;

		private TreeModelInterfaceDelegates _treeModelInterface;

		public IListTreeModelAdaptor(IListTreeModelConfiguration configuration)
			: base(IntPtr.Zero)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException();
			}
			CreateNativeObject(new string[0], new GLib.Value[0]);
			BuildTreeModelInterface();

			this._configuration = configuration;
		}

		protected IListTreeModelConfiguration Configuration
		{
			get
			{
				return _configuration;
			}
			private set
			{
				_configuration = value;
			}
		}

		private bool IsValidIndex(int index)
		{
			return 0 <= index && index < PastTheEndIndex;
		}

		private void VerifyValidIndex(int index)
		{
			if (!IsValidIndex(index))
			{
				throw new ArgumentOutOfRangeException("index", "index is not valid (it may be past the end)");
			}
		}

		private bool IsValidColumn(int column)
		{
			return 0 <= column && column < _configuration.ColumnTypes.Count;
		}

		private void VerifyValidColumn(int column)
		{
			if (!IsValidColumn(column))
			{
				throw new ArgumentOutOfRangeException("column", "column is not valid (it may be past the end)");
			}
		}

		private GLib.GType GetColumnType(int column)
		{
			return this._configuration.ColumnTypes[column];
		}

		private int GetIndex(IntPtr path)
		{
			Gtk.TreePath treepath = new Gtk.TreePath(path);
			int depth = treepath.Depth;
			if (depth <= 0)
			{
				throw new ArgumentOutOfRangeException("path", "Path must have depth > 0");
			}
			return treepath.Indices[0];
		}

		private Gtk.TreePath GetPath(int index)
		{
			this.VerifyValidIndex(index);
			Gtk.TreePath path = new Gtk.TreePath();
			path.AppendIndex(index);
			path.Owned = false;
			return path;
		}

		private int PastTheEndIndex
		{
			get
			{
				return this._configuration.DataSource.Count;
			}
		}

		#region TreeModel Members

		[GLib.CDeclCallback]
		private delegate int GetFlagsDelegate ();
		[GLib.CDeclCallback]
		private delegate int GetNColumnsDelegate ();
		[GLib.CDeclCallback]
		private delegate IntPtr GetColumnTypeDelegate (int column);
		[GLib.CDeclCallback]
		private delegate bool GetNodeDelegate (out int index, IntPtr path);
		[GLib.CDeclCallback]
		private delegate IntPtr GetPathDelegate(int index);
		[GLib.CDeclCallback]
		private delegate void GetValueDelegate(int index, int column, ref GLib.Value value);
		[GLib.CDeclCallback]
		private delegate bool NextDelegate(ref int index);
		[GLib.CDeclCallback]
		private delegate bool ChildrenDelegate (out int child, int parent);
		[GLib.CDeclCallback]
		private delegate bool HasChildDelegate(int index);
		[GLib.CDeclCallback]
		private delegate int NChildrenDelegate(int index);
		[GLib.CDeclCallback]
		private delegate bool NthChildDelegate (out int child, int parent, int n);
		[GLib.CDeclCallback]
		private delegate bool ParentDelegate (out int parent, int child);

		[StructLayout(LayoutKind.Sequential)]
		private struct TreeModelInterfaceDelegates
		{
			public GetFlagsDelegate get_flags;
			public GetNColumnsDelegate get_n_columns;
			public GetColumnTypeDelegate get_column_type;
			public GetNodeDelegate get_row;
			public GetPathDelegate get_path;
			public GetValueDelegate get_value;
			public NextDelegate next;
			public ChildrenDelegate children;
			public HasChildDelegate has_child;
			public NChildrenDelegate n_children;
			public NthChildDelegate nth_child;
			public ParentDelegate parent;
		}

		[DllImport("gtksharpglue-2")]
		private static extern void gtksharp_node_store_set_tree_model_callbacks(IntPtr raw, ref TreeModelInterfaceDelegates cbs);

		private void BuildTreeModelInterface()
		{
			this._treeModelInterface.get_flags = new GetFlagsDelegate(this.GetFlags_callback);
			this._treeModelInterface.get_n_columns = new GetNColumnsDelegate(this.GetNColumns_callback);
			this._treeModelInterface.get_column_type = new GetColumnTypeDelegate(this.GetColumnType_callback);
			this._treeModelInterface.get_row = new GetNodeDelegate(this.GetRow_callback);
			this._treeModelInterface.get_path = new GetPathDelegate(this.GetPath_callback);
			this._treeModelInterface.get_value = new GetValueDelegate(this.GetValue_callback);
			this._treeModelInterface.next = new NextDelegate(this.Next_callback);
			this._treeModelInterface.children = new ChildrenDelegate(this.Children_callback);
			this._treeModelInterface.has_child = new HasChildDelegate(this.HasChild_callback);
			this._treeModelInterface.n_children = new NChildrenDelegate(this.NChildren_callback);
			this._treeModelInterface.nth_child = new NthChildDelegate(this.NthChild_callback);
			this._treeModelInterface.parent = new ParentDelegate(this.Parent_callback);

			gtksharp_node_store_set_tree_model_callbacks(this.Handle, ref this._treeModelInterface);
		}

		/// <summary>
		/// Gets the flags which indicate what this implementation supports.
		/// </summary>
		/// <remarks>The flags supported should not change during the lifecycle of the tree model</remarks>
		protected int GetFlags_callback()
		{
			Gtk.TreeModelFlags result = Gtk.TreeModelFlags.ItersPersist;
			result |= Gtk.TreeModelFlags.ListOnly;
			return (int) result;
		}

		/// <summary>
		/// Gets the number of columns supported by this implementation
		/// </summary>
		protected int GetNColumns_callback()
		{
			return this._configuration.ColumnTypes.Count;
		}

		/// <summary>
		/// Gets the type of data stored in column number index;
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		protected IntPtr GetColumnType_callback (int column)
		{
			return GetColumnType(column).Val;
		}

		/// <summary>
		/// Gets an iterator object for the given path.
		/// </summary>
		/// <param name="iter"></param>
		/// <param name="path"></param>
		/// <returns>true if iter was set</returns>
		protected bool GetRow_callback(out int index, IntPtr path)
		{
			if (path == IntPtr.Zero)
			{
				throw new ArgumentNullException("path");
			}
			index = GetIndex(path);
			return (IsValidIndex(index));
		}

		/// <summary>
		/// Turns a TreeIter specified by iter into a TreePath
		/// </summary>
		/// <param name="iter"></param>
		/// <returns></returns>
		protected IntPtr GetPath_callback(int index)
		{
			return GetPath(index).Handle;
		}

		[DllImport("libgtk-win32-2.0-0.dll")]
		private static extern void g_value_init (ref GLib.Value val, IntPtr type);

		protected void GetValue_callback (int index, int column, ref GLib.Value value)
		{
			VerifyValidIndex(index);
			VerifyValidColumn(column);

			g_value_init(ref value, GetColumnType(column).Val);
			object instance = this._configuration.DataSource[index];
			value.Val = this._configuration.GetValueStrategy(instance, column);
		}

		/// <summary>
		/// Sets iter to point to the node following it at the current level. If ther is no next iter,
		/// returns false and iter is set to be invalid
		/// </summary>
		/// <param name="iter"></param>
		/// <returns>true if iter has been changed to the next node</returns>
		protected bool Next_callback(ref int index)
		{
			VerifyValidIndex(index);
			return IsValidIndex(++index);
		}

		/// <summary>
		/// Sets iter to point to the first child of parent. If parent has no children,
		/// returns false and iter is set to be invalid. parent will remain valid after this
		/// function has been called.
		/// </summary>
		/// <param name="child"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		protected bool Children_callback(out int child, int parent) {
		  if (parent == -1) {
			child = 0;
			return true;
		  }
		  VerifyValidIndex(parent);
		  child = PastTheEndIndex;
		  return false;
		}

		/// <summary>
		/// Tests whether a given row has a child node
		/// </summary>
		/// <param name="iter"></param>
		/// <returns>true if iter has children otherwise false</returns>
		protected bool HasChild_callback(int index)
		{
			VerifyValidIndex(index);
			return false;
		}

		/// <summary>
		/// Returns the number of children that iter has
		/// </summary>
		/// <param name="iter"></param>
		/// <returns></returns>
		protected int NChildren_callback(int index)
		{
			if (index == -1)
			{
				return PastTheEndIndex;
			}
			VerifyValidIndex(index);
			return 0;
		}

		/// <summary
		/// Sets iter to be the child of parent using the given index.
		/// The first index is 0. If n is too big, or parent has no children,
		/// iter is set to an invalid iterator and false is returned.
		/// </summary>
		/// <param name="child"></param>
		/// <param name="parent"></param>
		/// <param name="n"></param>
		/// <returns>true if iter is valid</returns>
		protected bool NthChild_callback(out int child, int parent, int n)
		{
			if (parent == -1)
			{
				child = n;
				return true;
			}
			VerifyValidIndex(parent);
			child = PastTheEndIndex;
			return false;
		}

		/// <summary>
		/// Set iter to be the parent of child. If child is at the toplevel
		/// and doesn't have a parent, then iter is set to an invalid iterator
		/// and false is returned. child will remain a valid node after this function has been called.
		/// </summary>
		/// <param name="iter"></param>
		/// <param name="child"></param>
		/// <returns>true if iter is set to parent of child</returns>
		protected bool Parent_callback(out int parent, int child)
		{
			VerifyValidIndex(child);
			parent = PastTheEndIndex;
			return false;
		}

		[System.Runtime.InteropServices.DllImport("gtksharpglue-2")]
		static extern void gtksharp_node_store_emit_row_inserted(IntPtr handle, IntPtr path, int node_idx);

		private void EmitRowInserted(int index)
		{
			gtksharp_node_store_emit_row_inserted(Handle, this.GetPath_callback(index), index);
		}

		[System.Runtime.InteropServices.DllImport("gtksharpglue-2")]
		static extern void gtksharp_node_store_emit_row_deleted(IntPtr handle, IntPtr path, int node_idx);

		private void EmitRowDeleted(int index)
		{
			gtksharp_node_store_emit_row_deleted(Handle, GetPath(index).Handle, index);
		}

		[System.Runtime.InteropServices.DllImport("gtksharpglue-2")]
		static extern void gtksharp_node_store_emit_row_changed(IntPtr handle, IntPtr path, int node_idx);

		private void EmitRowChanged(int index)
		{
			gtksharp_node_store_emit_row_changed(Handle, this.GetPath_callback(index), index);
		}

		[System.Runtime.InteropServices.DllImport("gtksharpglue-2")]
		static extern IntPtr gtksharp_node_store_get_type();

		private static new GLib.GType GType
		{
			get
			{
				return new GLib.GType(gtksharp_node_store_get_type());
			}
		}
		#endregion
	}

}
