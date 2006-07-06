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

	public WordDetailView()
	{
	 // _model = lexiconModel;

	  Glade.XML gxml = new Glade.XML("probe.glade", "_wordDetailHolder", null);
	  gxml.Autoconnect(this);
			_word.ModifyFont(Pango.FontDescription.FromString("default 30"));
			_gloss.ModifyFont(Pango.FontDescription.FromString("default 20"));
			_example.ModifyFont(Pango.FontDescription.FromString("default 20"));



	}

	//called by Spring factory. Part of IInitializingObject
	public void AfterPropertiesSet()
	{
		_wordDetailVBox.Reparent(s_tabcontrol);
		s_tabcontrol.SetTabLabelText(_wordDetailVBox, "Detail");
		Update();
		WireEvents();
	}

	public Gtk.Notebook ParentTabControl
	{
		set
		{
			_wordDetailVBox.Reparent(value);
			value.SetTabLabelText(_wordDetailVBox, TabLabel);
		}
	}
	public string TabLabel
	{
		get
		{
			return "details details";
		}
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
		  _model.CurrentLexicalEntry.Example  = _example.Text;
		  _model.OnChanged(_model.CurrentLexicalEntry);
	  }

	void OnWord_FocusOutEvent(object o, FocusOutEventArgs args) {
	  _word.FinishEditing();
	}
	void OnGloss_FocusOutEvent(object o, FocusOutEventArgs args) {
	  _gloss.FinishEditing();
	}
	void OnWord_EditingDone(object sender, EventArgs e) {
	  _model.CurrentLexicalEntry.LexicalForm = _word.Text;
	  _model.OnChanged(_model.CurrentLexicalEntry);
	}
	void OnGloss_EditingDone(object sender, EventArgs e) {
	  _model.CurrentLexicalEntry.Gloss = _gloss.Text;
	  _model.OnChanged(_model.CurrentLexicalEntry);
	}

	void add_Clicked(object sender, EventArgs e) {
	  _model.Add(new LexicalEntry());
	  Update();
	}

	void delete_Clicked(object sender, EventArgs e) {
	  _model.Remove(_model.CurrentLexicalEntry);
	  Update();
	}


	void last_Clicked(object sender, EventArgs e) {
	  SaveCurrentEditting();
	  _model.GotoLastRecord();
	  Update();
	}

	void back_Clicked(object sender, EventArgs e) {
	  SaveCurrentEditting();
	  _model.GotoPreviousRecord();
	  Update();
	}

	void first_Clicked(object sender, EventArgs e) {
	  SaveCurrentEditting();
	  _model.GotoFirstRecord();
	  Update();
	}

	void next_Clicked(object sender, EventArgs e) {
	  SaveCurrentEditting();
	  _model.GotoNextRecord();
	  Update();
	}

	private void SaveCurrentEditting() {
	  Gtk.Entry e = _wordDetailTable.FocusChild as Gtk.Entry;
	  ;
	  if (e != null)
		e.FinishEditing();
	}

	private void Update() {

	  _word.Text = _model.CurrentLexicalEntry.LexicalForm;
	  _gloss.Text = _model.CurrentLexicalEntry.Gloss;
	  _example .Text = _model.CurrentLexicalEntry.Example ;
	}
  }
}
