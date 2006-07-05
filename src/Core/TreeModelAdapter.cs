using System;

namespace WeSay.Core
{
  public class TreeModelAdapter : GLib.Object
  {
	TreeModelInterfaceDelegates _treeModelInterface;

	static GLib.GType[] _columnTypes = new GLib.GType[2] { GLib.GType.String, GLib.GType.String };
	LexiconModel _lexiconModel;
	int _originalCount;

	public TreeModelAdapter(LexiconModel lexiconModel): base(IntPtr.Zero) {
	  CreateNativeObject(new string[0], new GLib.Value[0]);
	  this.BuildTreeModelInterface();

	  _lexiconModel = lexiconModel;
	  _originalCount = _lexiconModel.Count;
	}

	public void Refresh() {
	  int refreshCount = Math.Min(_lexiconModel.Count, _originalCount);
	  int addCount = Math.Max(_lexiconModel.Count - _originalCount, 0);
	  int deleteCount = Math.Max(_originalCount - _lexiconModel.Count, 0);
	  //int i;
	  //for (i = 0; i < refreshCount; ++i) {
	  //  EmitRowChanged(i);
	  //}
	  //--i;
	  int i = refreshCount - 1;
	  for (int j = 0; j < addCount; ++j) {
		EmitRowInserted(i + j);
	  }
	  for (int j = deleteCount; j > 0; --j) {
		EmitRowDeleted(i + j);
	  }
	  _originalCount = _lexiconModel.Count;
	}

	bool IsValidIndex(int index) {
	  return 0 <= index && index < _lexiconModel.Count;
	}

	void VerifyValidIndex(int index) {
	  if (!IsValidIndex(index)) {
		throw new ArgumentOutOfRangeException("iter", "index is not valid (it may be past the end)");
	  }
	}

	void VerifyValidColumn(int column) {
	  if (column >= _columnTypes.Length) {
		throw new ArgumentOutOfRangeException("column", column, ("must be less than " + _columnTypes.Length.ToString()));
	  }
	  if (column < 0) {
		throw new ArgumentOutOfRangeException("column", column, "must be 0 or greater");
	  }
	}

	#region TreeModel Members

	[GLib.CDeclCallback]
	private delegate int GetFlagsDelegate();
	[GLib.CDeclCallback]
	private delegate int GetNColumnsDelegate();
	[GLib.CDeclCallback]
	delegate IntPtr GetColumnTypeDelegate(int column);
	[GLib.CDeclCallback]
	delegate bool GetRowDelegate(out int index, IntPtr path);
	[GLib.CDeclCallback]
	delegate IntPtr GetPathDelegate(int index);
	[GLib.CDeclCallback]
	delegate void GetValueDelegate(int index, int column, ref GLib.Value value);
	[GLib.CDeclCallback]
	delegate bool NextDelegate(ref int index);
	[GLib.CDeclCallback]
	delegate bool ChildrenDelegate(out int child, int parent);
	[GLib.CDeclCallback]
	delegate bool HasChildDelegate(int index);
	[GLib.CDeclCallback]
	delegate int NChildrenDelegate(int index);
	[GLib.CDeclCallback]
	delegate bool NthChildDelegate(out int child, int parent, int n);
	[GLib.CDeclCallback]
	delegate bool ParentDelegate(out int parent, int child);


	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
	struct TreeModelInterfaceDelegates
	{
	  public GetFlagsDelegate get_flags;
	  public GetNColumnsDelegate get_n_columns;
	  public GetColumnTypeDelegate get_column_type;
	  public GetRowDelegate get_row;
	  public GetPathDelegate get_path;
	  public GetValueDelegate get_value;
	  public NextDelegate next;
	  public ChildrenDelegate children;
	  public HasChildDelegate has_child;
	  public NChildrenDelegate n_children;
	  public NthChildDelegate nth_child;
	  public ParentDelegate parent;
	}

	[System.Runtime.InteropServices.DllImport("gtksharpglue-2")]
	static extern void gtksharp_node_store_set_tree_model_callbacks(IntPtr raw, ref TreeModelInterfaceDelegates cbs);

	private void BuildTreeModelInterface() {
	  this._treeModelInterface.get_flags = new GetFlagsDelegate(GetFlags_callback);
	  this._treeModelInterface.get_n_columns = new GetNColumnsDelegate(GetNColumns_callback);
	  this._treeModelInterface.get_column_type = new GetColumnTypeDelegate(GetColumnType_callback);
	  this._treeModelInterface.get_row = new GetRowDelegate(GetRow_callback);
	  this._treeModelInterface.get_path = new GetPathDelegate(GetPath_callback);
	  this._treeModelInterface.get_value = new GetValueDelegate(GetValue_callback);
	  this._treeModelInterface.next = new NextDelegate(Next_callback);
	  this._treeModelInterface.children = new ChildrenDelegate(Children_callback);
	  this._treeModelInterface.has_child = new HasChildDelegate(HasChild_callback);
	  this._treeModelInterface.n_children = new NChildrenDelegate(NChildren_callback);
	  this._treeModelInterface.nth_child = new NthChildDelegate(NthChild_callback);
	  this._treeModelInterface.parent = new ParentDelegate(Parent_callback);

	  gtksharp_node_store_set_tree_model_callbacks(Handle, ref this._treeModelInterface);
	}

	/// <summary>
	/// Gets the flags which indicate what this implementation supports.
	/// </summary>
	/// <remarks>The flags supported should not change during the lifecycle of the tree model</remarks>
	int GetFlags_callback () {
	  Gtk.TreeModelFlags result = Gtk.TreeModelFlags.ItersPersist;
	  result |= Gtk.TreeModelFlags.ListOnly;
	  return (int) result;
	}

	/// <summary>
	/// Gets the number of columns supported by this implementation
	/// </summary>
	int GetNColumns_callback() {
	  return _columnTypes.Length;
	}

	/// <summary>
	/// Gets the type of data stored in column number index;
	/// </summary>
	/// <param name="column"></param>
	/// <returns></returns>
	IntPtr GetColumnType_callback(int column) {
	  return _columnTypes[column].Val;
	}

	/// <summary>
	/// Gets an iterator object for the given path.
	/// </summary>
	/// <param name="iter"></param>
	/// <param name="path"></param>
	/// <returns>true if iter was set</returns>
	public bool GetRow_callback(out int index, IntPtr path) {
	  if (path == IntPtr.Zero){
		throw new ArgumentNullException("path");
	  }
	  Gtk.TreePath treepath = new Gtk.TreePath(path);
	  int depth = treepath.Depth;
	  if (depth <= 0) {
		throw new ArgumentOutOfRangeException("path", "Path must have depth > 0");
	  }
	  index = treepath.Indices[0];
	  return (IsValidIndex(index));
	}

	/// <summary>
	/// Turns a TreeIter specified by iter into a TreePath
	/// </summary>
	/// <param name="iter"></param>
	/// <returns></returns>
	public IntPtr GetPath_callback(int index) {
	  this.VerifyValidIndex(index);
	  return GetPath(index).Handle;
	}

	private Gtk.TreePath GetPath(int index) {
	  Gtk.TreePath path = new Gtk.TreePath();
	  path.AppendIndex(index);
	  path.Owned = false;
	  return path;
	}

	[System.Runtime.InteropServices.DllImport("libgobject-2.0-0.dll")]
	static extern void g_value_init(ref GLib.Value val, IntPtr type);

	/// <summary>
	/// Gets the value of row iter of column column and puts it in value.
	/// </summary>
	/// <param name="iter"></param>
	/// <param name="column"></param>
	/// <param name="value"></param>
	public void GetValue_callback(int index, int column, ref GLib.Value value) {
	  VerifyValidIndex(index);
	  VerifyValidColumn(column);
	  g_value_init(ref value, _columnTypes[column].Val);

	  LexicalEntry row = _lexiconModel[index];
	  switch (column) {
		case 0: {
		  value.Val = row.LexicalForm;
		  break;
		}
		case 1: {
		  value.Val = row.Gloss;
		  break;
		}
		default: {
		  break;
		}
	  }
	}

	/// <summary>
	/// Sets iter to point to the node following it at the current level. If ther is no next iter,
	/// returns false and iter is set to be invalid
	/// </summary>
	/// <param name="iter"></param>
	/// <returns>true if iter has been changed to the next node</returns>
	public bool Next_callback(ref int index) {
	  VerifyValidIndex(index);
	  return(IsValidIndex(++index));
	}

	/// <summary>
	/// Sets iter to point to the first child of parent. If parent has no children,
	/// returns false and iter is set to be invalid. parent will remain valid after this
	/// function has been called.
	/// </summary>
	/// <param name="iter"></param>
	/// <param name="parent"></param>
	/// <returns></returns>
	public bool Children_callback(out int child, int parent) {
	  if (parent == -1) {
		child = 0;
		return true;
	  }
	  VerifyValidIndex(parent);
	  child = _lexiconModel.Count;
	  return false;
	}

	/// <summary>
	/// Tests whether a given row has a child node
	/// </summary>
	/// <param name="iter"></param>
	/// <returns>true if iter has children otherwise false</returns>
	public bool HasChild_callback(int index) {
	  VerifyValidIndex(index);
	  return false;
	}

	/// <summary>
	/// Returns the number of children that iter has
	/// </summary>
	/// <param name="iter"></param>
	/// <returns></returns>
	public int NChildren_callback(int index) {
	  if (index == -1) {
		return _lexiconModel.Count;
	  }
	  VerifyValidIndex(index);
	  return 0;
	}

	/// <summary>
	/// Sets iter to be the child of parent using the given index.
	/// The first index is 0. If n is too big, or parent has no children,
	/// iter is set to an invalid iterator and false is returned.
	/// </summary>
	/// <param name="iter"></param>
	/// <param name="parent"></param>
	/// <param name="n"></param>
	/// <returns>true if iter is valid</returns>
	public bool NthChild_callback(out int child, int parent, int n) {
	  if (parent == -1) {
		child = n;
		return true;
	  }
	  VerifyValidIndex(parent);
	  child = _lexiconModel.Count;
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
	public bool Parent_callback(out int parent, int child) {
	  VerifyValidIndex(child);
	  parent = _lexiconModel.Count;
	  return false;
	}

	[System.Runtime.InteropServices.DllImport("gtksharpglue-2")]
	static extern void gtksharp_node_store_emit_row_inserted(IntPtr handle, IntPtr path, int node_idx);

	private void EmitRowInserted(int index) {
	  gtksharp_node_store_emit_row_inserted(Handle, this.GetPath_callback(index), index);
	}

	[System.Runtime.InteropServices.DllImport("gtksharpglue-2")]
	static extern void gtksharp_node_store_emit_row_deleted(IntPtr handle, IntPtr path, int node_idx);

	private void EmitRowDeleted(int index) {
	  gtksharp_node_store_emit_row_deleted(Handle, GetPath(index).Handle, index);
	}

	[System.Runtime.InteropServices.DllImport("gtksharpglue-2")]
	static extern void gtksharp_node_store_emit_row_changed(IntPtr handle, IntPtr path, int node_idx);

	private void EmitRowChanged(int index) {
	  gtksharp_node_store_emit_row_changed(Handle, this.GetPath_callback(index), index);
	}

	[System.Runtime.InteropServices.DllImport("gtksharpglue-2")]
	static extern IntPtr gtksharp_node_store_get_type();

	public static new GLib.GType GType {
	  get {
		return new GLib.GType(gtksharp_node_store_get_type());
	  }
	}

	#endregion
  }
}
