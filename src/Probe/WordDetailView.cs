using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using Glade;
using WeSay.Core;

namespace WeSay.UI
{
  class WordDetailView
  {
	protected LexiconModel _lexiconModel;
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

	public WordDetailView(Container container, LexiconModel lexiconModel) {
	  _lexiconModel = lexiconModel;

	  Glade.XML gxml = new Glade.XML("probe.glade", "_wordDetailHolder", null);
	  gxml.Autoconnect(this);
	  _wordDetailVBox.Reparent(container);
			_word.ModifyFont(Pango.FontDescription.FromString("default 30"));
			_gloss.ModifyFont(Pango.FontDescription.FromString("default 20"));
			_example.ModifyFont(Pango.FontDescription.FromString("default 20"));

	  WireEvents();

	  Update();
	}

	private void WireEvents() {
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
	  _example.EditingDone+=new EventHandler(_example_EditingDone);
	  _example.FocusOutEvent+=new FocusOutEventHandler(_example_FocusOutEvent);
	}

	  void _example_FocusOutEvent(object o, FocusOutEventArgs args)
	  {
		  _example.FinishEditing();
	  }

	  void _example_EditingDone(object sender, EventArgs e)
	  {
		  _lexiconModel.CurrentLexicalEntry.Example  = _example.Text;
		  _lexiconModel.OnChanged(_lexiconModel.CurrentLexicalEntry);
	  }

	void OnWord_FocusOutEvent(object o, FocusOutEventArgs args) {
	  _word.FinishEditing();
	}
	void OnGloss_FocusOutEvent(object o, FocusOutEventArgs args) {
	  _gloss.FinishEditing();
	}
	void OnWord_EditingDone(object sender, EventArgs e) {
	  _lexiconModel.CurrentLexicalEntry.LexicalForm = _word.Text;
	  _lexiconModel.OnChanged(_lexiconModel.CurrentLexicalEntry);
	}
	void OnGloss_EditingDone(object sender, EventArgs e) {
	  _lexiconModel.CurrentLexicalEntry.Gloss = _gloss.Text;
	  _lexiconModel.OnChanged(_lexiconModel.CurrentLexicalEntry);
	}

	void add_Clicked(object sender, EventArgs e) {
	  _lexiconModel.Add(new LexicalEntry());
	  Update();
	}

	void delete_Clicked(object sender, EventArgs e) {
	  _lexiconModel.Remove(_lexiconModel.CurrentLexicalEntry);
	  Update();
	}


	void last_Clicked(object sender, EventArgs e) {
	  SaveCurrentEditting();
	  _lexiconModel.GotoLastRecord();
	  Update();
	}

	void back_Clicked(object sender, EventArgs e) {
	  SaveCurrentEditting();
	  _lexiconModel.GotoPreviousRecord();
	  Update();
	}

	void first_Clicked(object sender, EventArgs e) {
	  SaveCurrentEditting();
	  _lexiconModel.GotoFirstRecord();
	  Update();
	}

	void next_Clicked(object sender, EventArgs e) {
	  SaveCurrentEditting();
	  _lexiconModel.GotoNextRecord();
	  Update();
	}

	private void SaveCurrentEditting() {
	  Gtk.Entry e = _wordDetailTable.FocusChild as Gtk.Entry;
	  ;
	  if (e != null)
		e.FinishEditing();
	}

	private void Update() {

	  _word.Text = _lexiconModel.CurrentLexicalEntry.LexicalForm;
	  _gloss.Text = _lexiconModel.CurrentLexicalEntry.Gloss;
	  _example .Text = _lexiconModel.CurrentLexicalEntry.Example ;
	}
  }
}
