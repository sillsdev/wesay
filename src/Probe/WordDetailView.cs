using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using Glade;
using WeSay.Core;
using Spring.Objects.Factory;

namespace WeSay.UI
{
  class WordDetailView : ViewHandler, IInitializingObject
  {
	LexicalEntry _currentLexicalEntry;

#pragma warning disable 649
	[Widget]
	protected Gtk.Table _wordDetailTable;
	[Widget]
	protected Gtk.Entry _word;
	[Widget]
	protected Gtk.Entry _gloss;
	[Widget]
	protected Gtk.Entry _example;
	[Widget]
	protected Gtk.ToolButton _buttonAdd;
	[Widget]
	protected Gtk.ToolButton _buttonDelete;
	[Widget]
	protected Gtk.ToolButton _buttonForward;
	[Widget]
	protected Gtk.ToolButton _buttonBackward;
	[Widget]
	protected Gtk.ToolButton _buttonFirst;
	[Widget]
	protected Gtk.ToolButton _buttonLast;
	[Widget]
	public Gtk.VBox _wordDetailVBox;
#pragma warning restore 649

	public WordDetailView() {
	  Glade.XML gxml = new Glade.XML("probe.glade", "_wordDetailHolder", null);
	  gxml.Autoconnect(this);
	  _word.ModifyFont(Pango.FontDescription.FromString("default 30"));
	  _gloss.ModifyFont(Pango.FontDescription.FromString("default 20"));
	  _example.ModifyFont(Pango.FontDescription.FromString("default 20"));
	}

	//called by Spring factory. Part of IInitializingObject
	public void AfterPropertiesSet() {
	  _wordDetailVBox.Reparent(s_tabcontrol);
	  s_tabcontrol.SetTabLabelText(_wordDetailVBox, "Detail");
	  Update();
	  WireEvents();
	}

	public Gtk.Notebook ParentTabControl {
	  set {
		_wordDetailVBox.Reparent(value);
		value.SetTabLabelText(_wordDetailVBox, TabLabel);
	  }
	}
	public string TabLabel {
	  get {
		return "details details";
	  }
	}

	private void WireEvents() {
	  _wordDetailVBox.Parent.FocusInEvent += new FocusInEventHandler(OnFocused);
	  _buttonAdd.Clicked += new EventHandler(add_Clicked);
	  _buttonDelete.Clicked += new EventHandler(delete_Clicked);

	  _buttonFirst.Clicked += new EventHandler(first_Clicked);
	  _buttonBackward.Clicked += new EventHandler(back_Clicked);
	  _buttonForward.Clicked += new EventHandler(next_Clicked);
	  _buttonLast.Clicked += new EventHandler(last_Clicked);

	  _word.FocusOutEvent += new FocusOutEventHandler(OnWord_FocusOutEvent);
	  _word.EditingDone += new EventHandler(OnWord_EditingDone);
	  _gloss.EditingDone += new EventHandler(OnGloss_EditingDone);
	  _gloss.FocusOutEvent += new FocusOutEventHandler(OnGloss_FocusOutEvent);
	  _example.EditingDone += new EventHandler(_example_EditingDone);
	  _example.FocusOutEvent += new FocusOutEventHandler(_example_FocusOutEvent);
	}

	public void OnFocused(object o, FocusInEventArgs args) {
	  Update();
	  args.RetVal = false;
	}

	void _example_FocusOutEvent(object o, FocusOutEventArgs args) {
	  _example.FinishEditing();
	}

	void _example_EditingDone(object sender, EventArgs e) {
	  _currentLexicalEntry.Example = _example.Text;
	  _model.OnChanged(_currentLexicalEntry);
	}

	void OnWord_FocusOutEvent(object o, FocusOutEventArgs args) {
	  _word.FinishEditing();
	}

	void OnGloss_FocusOutEvent(object o, FocusOutEventArgs args) {
	  _gloss.FinishEditing();
	}

	void OnWord_EditingDone(object sender, EventArgs e) {
	  _currentLexicalEntry.LexicalForm = _word.Text;
	  _model.OnChanged(_currentLexicalEntry);
	}

	void OnGloss_EditingDone(object sender, EventArgs e) {
	  _currentLexicalEntry.Gloss = _gloss.Text;
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

	void last_Clicked(object sender, EventArgs e) {
	  SaveCurrentEditting();
	  _currentLexicalEntry = _model[_model.Count - 1];
	  Update();
	}

	void back_Clicked(object sender, EventArgs e) {
	  SaveCurrentEditting();
	  int index = _model.IndexOf(_currentLexicalEntry);
	  if(index > 0){
		_currentLexicalEntry = _model[--index];
		Update();
	  }
	}

	void first_Clicked(object sender, EventArgs e) {
	  SaveCurrentEditting();
	  _currentLexicalEntry = _model[0];
	  Update();
	}

	void next_Clicked(object sender, EventArgs e) {
	  SaveCurrentEditting();
	  int index = _model.IndexOf(_currentLexicalEntry);
	  if (++index < _model.Count) {
		_currentLexicalEntry = _model[index];
		Update();
	  }
	}

	private void SaveCurrentEditting() {
	  Gtk.Entry e = _wordDetailTable.FocusChild as Gtk.Entry;
	  if (e != null) {
		e.FinishEditing();
	  }
	}

	private void Update() {
	  if (_model.Count == 0) {
		_currentLexicalEntry = new LexicalEntry();
		_model.Add(_currentLexicalEntry);
	  }
	  if (!_model.Contains(_currentLexicalEntry)) {
		_currentLexicalEntry = _model[0];
	  }
	  _word.Text = _currentLexicalEntry.LexicalForm;
	  _gloss.Text = _currentLexicalEntry.Gloss;
	  _example.Text = _currentLexicalEntry.Example;
	}
  }
}
