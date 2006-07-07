using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using Glade;
using WeSay.Core;

namespace WeSay.UI
{
  class BrowseDetailHandler : ViewHandler, Spring.Objects.Factory.IInitializingObject
  {
	LexiconTreeView _entryList;
	TreeModelAdapter _treeModelAdapter;
	LexicalEntry _currentLexicalEntry;

#pragma warning disable 649
	[Widget]
	protected Gtk.Table _wordDetailWithBrowse_Table;
	[Widget]
	protected Gtk.Entry _wordDetailWithBrowse_word;
	[Widget]
	protected Gtk.Entry _wordDetailWithBrowse_gloss;
	[Widget]
	protected Gtk.Entry _wordDetailWithBrowse_example;
	[Widget]
	protected Gtk.ToolButton _wordDetailWithBrowse_buttonAdd;
	[Widget]
	protected Gtk.ToolButton _wordDetailWithBrowse_buttonDelete;
	[Widget]
	public Gtk.VBox _wordDetailWithBrowse_VBox;
	[Widget]
	protected Gtk.ScrolledWindow _wordDetailWithBrowse_entryScroller;

#pragma warning restore 649

	public BrowseDetailHandler() {

	  Glade.XML gxml = new Glade.XML("probe.glade", "_wordDetailWithBrowseHolder", null);
	  gxml.Autoconnect(this);
	}


	public void AfterPropertiesSet() {

	  _treeModelAdapter = new TreeModelAdapter(_model);
	  _entryList = new LexiconTreeView(_treeModelAdapter);

	  _wordDetailWithBrowse_entryScroller.Add(_entryList);
	  _wordDetailWithBrowse_VBox.Reparent(s_tabcontrol);
	  s_tabcontrol.SetTabLabelText(_wordDetailWithBrowse_VBox, "New Detail");

	  _wordDetailWithBrowse_word.ModifyFont(Pango.FontDescription.FromString("default 20"));
	  _wordDetailWithBrowse_gloss.ModifyFont(Pango.FontDescription.FromString("default 15"));
	  _wordDetailWithBrowse_example.ModifyFont(Pango.FontDescription.FromString("default 20"));

	  _entryList.FixedHeightMode = true;
	  AddColumn(_entryList, "Entries", 0, 15, 0);
	  _entryList.ShowAll();
	  LexiconTreeSelection selection = _entryList.LexiconTreeSelection;
	  selection.Select(0);
	  selection.Changed += new EventHandler(selection_Changed);

	  WireEvents();
	  Update();
	}

	void selection_Changed(object sender, EventArgs e) {
	  LexiconTreeSelection selection = (LexiconTreeSelection) sender;
	  _currentLexicalEntry = _model[selection.Selected];
	  Update();
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
	  Update();
	  args.RetVal = false;
	}

	private void WireEvents() {
	  _wordDetailWithBrowse_VBox.Parent.FocusInEvent += new FocusInEventHandler(OnFocused);
	  _wordDetailWithBrowse_buttonAdd.Clicked += new EventHandler(add_Clicked);
	  _wordDetailWithBrowse_buttonDelete.Clicked += new EventHandler(delete_Clicked);

	  _wordDetailWithBrowse_word.FocusOutEvent += new FocusOutEventHandler(OnWord_FocusOutEvent);
	  _wordDetailWithBrowse_word.EditingDone += new EventHandler(OnWord_EditingDone);
	  _wordDetailWithBrowse_word.TextInserted += new TextInsertedHandler(_wordDetailWithBrowse_word_TextInserted);
	  _wordDetailWithBrowse_word.TextDeleted += new TextDeletedHandler(_wordDetailWithBrowse_word_TextDeleted);
	  _wordDetailWithBrowse_gloss.EditingDone += new EventHandler(OnGloss_EditingDone);
	  _wordDetailWithBrowse_gloss.FocusOutEvent += new FocusOutEventHandler(OnGloss_FocusOutEvent);
	  _wordDetailWithBrowse_example.EditingDone += new EventHandler(OnExample_EditingDone);
	  _wordDetailWithBrowse_example.FocusOutEvent += new FocusOutEventHandler(OnExample_FocusOutEvent);
	}

	void _wordDetailWithBrowse_word_TextDeleted(object o, TextDeletedArgs args) {
	  _currentLexicalEntry.LexicalForm = _wordDetailWithBrowse_word.Text;
	  _treeModelAdapter.Refresh(_currentLexicalEntry);
	}

	void _wordDetailWithBrowse_word_TextInserted(object o, TextInsertedArgs args) {
	  _currentLexicalEntry.LexicalForm = _wordDetailWithBrowse_word.Text;
	  _treeModelAdapter.Refresh(_currentLexicalEntry);
	}

	void OnExample_FocusOutEvent(object o, FocusOutEventArgs args) {
	  _wordDetailWithBrowse_example.FinishEditing();
	}

	void OnExample_EditingDone(object sender, EventArgs e) {
	  _currentLexicalEntry.Example = _wordDetailWithBrowse_example.Text;
	  _model.OnChanged(_currentLexicalEntry);
	}

	void OnWord_FocusOutEvent(object o, FocusOutEventArgs args) {
	  _wordDetailWithBrowse_word.FinishEditing();
	}

	void OnGloss_FocusOutEvent(object o, FocusOutEventArgs args) {
	  _wordDetailWithBrowse_gloss.FinishEditing();
	}

	void OnWord_EditingDone(object sender, EventArgs e) {
	  _currentLexicalEntry.LexicalForm = _wordDetailWithBrowse_word.Text;
	  _model.OnChanged(_currentLexicalEntry);
	  _treeModelAdapter.Refresh(_currentLexicalEntry);
	}

	void OnGloss_EditingDone(object sender, EventArgs e) {
	  _currentLexicalEntry.Gloss = _wordDetailWithBrowse_gloss.Text;
	  _model.OnChanged(_currentLexicalEntry);
	}

	void add_Clicked(object sender, EventArgs e) {
	  SaveCurrentEditting();
	  _currentLexicalEntry = new LexicalEntry();
	  _model.Add(_currentLexicalEntry);
	  Update();
	}

	void delete_Clicked(object sender, EventArgs e) {
	  SaveCurrentEditting();
	  int index = _model.IndexOf(_currentLexicalEntry);
	  _model.Remove(_currentLexicalEntry);
	  if (index > 0) {
		_currentLexicalEntry = _model[--index];
	  }
	  Update();
	}

	private void SaveCurrentEditting() {
	  Gtk.Entry e = _wordDetailWithBrowse_Table.FocusChild as Gtk.Entry;
	  if (e != null)
		e.FinishEditing();
	}

	private void Update() {
	  if (_model.Count == 0) {
		_currentLexicalEntry = new LexicalEntry();
		_model.Add(_currentLexicalEntry);
	  }
	  if (!_model.Contains(_currentLexicalEntry)) {
		_currentLexicalEntry = _model[0];
	  }
	  int index = _model.IndexOf(_currentLexicalEntry);
	  _treeModelAdapter.Refresh(index);
	  _entryList.LexiconTreeSelection.Select(index);
	  _entryList.ScrollToCell(_treeModelAdapter.GetPath(index),_entryList.Columns[0],false,0,0);

	  _wordDetailWithBrowse_word.Text = _currentLexicalEntry.LexicalForm;
	  _wordDetailWithBrowse_gloss.Text = _currentLexicalEntry.Gloss;
	  _wordDetailWithBrowse_example.Text = _currentLexicalEntry.Example;
	}
  }
}
