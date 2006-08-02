using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using Glade;
using WeSay.Core;
using Spring.Objects.Factory;

namespace WeSay.UI
{
	class WordGridHandler : ViewHandler, IInitializingObject
  {
	LexiconTreeView _entryList;
	TreeModelAdapter _modelAdapter;

#pragma warning disable 649
	[Widget]
	protected Gtk.VBox _root;
	[Widget]
	protected Gtk.ScrolledWindow _entryScroller;
#pragma warning restore 649


	public WordGridHandler()//Container container, LexiconModel dataService)
	{

	  Glade.XML gxml = new Glade.XML("probe.glade", "_wordGridHolder", null);
	  gxml.Autoconnect(this);

	}

	//called by Spring factory. Part of IInitializingObject
	public void AfterPropertiesSet()
	{
	  _modelAdapter = new TreeModelAdapter(_model);
	  _entryList = new LexiconTreeView(_modelAdapter);

	  _entryScroller.Add(_entryList);

	  _root.Reparent(s_tabcontrol);
	  s_tabcontrol.SetTabLabelText(_root, "Words");


	  _entryList.FixedHeightMode = true;
	  _entryList.ShowAll();

	  AddColumn(_entryList, "Word", 0, 25, 120);
	  AddColumn(_entryList, "Gloss", 1, 12, 200);

	  _entryList.FocusInEvent += new FocusInEventHandler(OnFocused);

	}

	private void AddColumn(LexiconTreeView entryList, string title, int column, int fontSize, int minWidth) {
	  Gtk.CellRendererText renderer = new Gtk.CellRendererText();
	  renderer.SizePoints = fontSize;
	  Gtk.TreeViewColumn treeViewColumn = new Gtk.TreeViewColumn(title, renderer, "text", column);
	  treeViewColumn.Sizing = TreeViewColumnSizing.Fixed;
	  treeViewColumn.Visible = true;
	  treeViewColumn.Resizable = true;
	  treeViewColumn.MinWidth = minWidth;
	  entryList.AppendColumn(treeViewColumn);
	}

	public void OnFocused(object o, FocusInEventArgs args) {
	  _modelAdapter.Refresh();
	  args.RetVal = false;
	}
  }
}
