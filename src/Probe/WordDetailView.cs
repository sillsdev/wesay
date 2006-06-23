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
		protected DataService _dataService;
		[Widget]protected Gtk.Table _wordDetailTable;
		[Widget] protected Gtk.Entry _word;
		[Widget]  protected Gtk.Entry _gloss;
		[Widget]  protected Gtk.ToolButton _buttonForward;
		[Widget] protected Gtk.ToolButton _buttonBackward;
		[Widget] protected Gtk.ToolButton _buttonFirst;
		[Widget] protected Gtk.ToolButton _buttonLast;

		[Widget]  public Gtk.VBox _wordDetailVBox;

		public WordDetailView(Container container, DataService dataService)
		{
			_dataService = dataService;

			Glade.XML gxml = new Glade.XML("probe.glade", "_wordDetailHolder", null);
			gxml.Autoconnect(this);
			_wordDetailVBox.Reparent(container);

			WireEvents();

			Update();
		}

		private void WireEvents()
		{
			_buttonFirst.Clicked += new EventHandler(first_Clicked);
			_buttonBackward.Clicked += new EventHandler(back_Clicked);
			_buttonForward.Clicked += new EventHandler(next_Clicked);
			_buttonLast.Clicked += new EventHandler(last_Clicked);

			_word.FocusOutEvent += new FocusOutEventHandler(OnWord_FocusOutEvent);
			_word.EditingDone += new EventHandler(OnWord_EditingDone);
			_gloss.EditingDone += new EventHandler(OnGloss_EditingDone);
			_gloss.FocusOutEvent += new FocusOutEventHandler(OnGloss_FocusOutEvent);
		}

		void OnWord_FocusOutEvent(object o, FocusOutEventArgs args)
		{
			_word.FinishEditing();
		}
		void OnGloss_FocusOutEvent(object o, FocusOutEventArgs args)
		{
			_gloss.FinishEditing();
		}
		void OnWord_EditingDone(object sender, EventArgs e)
		{
			_dataService.CurrentLexicalEntry.LexicalForm = _word.Text;
			_dataService.Changed(_dataService.CurrentLexicalEntry);
		}
		void OnGloss_EditingDone(object sender, EventArgs e)
		{
			_dataService.CurrentLexicalEntry.Gloss = _gloss.Text;
			_dataService.Changed(_dataService.CurrentLexicalEntry);
	   }

		void last_Clicked(object sender, EventArgs e)
		{
			SaveCurrentEditting();
			_dataService.GotoLastRecord();
			Update();
		}

		void back_Clicked(object sender, EventArgs e)
		{
			SaveCurrentEditting();
			_dataService.GotoPreviousRecord();
			Update();
		}

		void first_Clicked(object sender, EventArgs e)
		{
			SaveCurrentEditting();
			_dataService.GotoFirstRecord();
			Update();
		}

		void next_Clicked(object sender, EventArgs e)
		{
			SaveCurrentEditting();
			_dataService.GotoNextRecord();
			Update();
		}

		private void SaveCurrentEditting()
		{
			Gtk.Entry e = _wordDetailTable.FocusChild as Gtk.Entry; ;
			if (e != null)
				e.FinishEditing();
		}

		private void Update()
		{

			_word.Text = _dataService.CurrentLexicalEntry.LexicalForm;
			_gloss.Text = _dataService.CurrentLexicalEntry.Gloss;
		}

		public void OnRenderLexemeForm(TreeViewColumn tree_column, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			(cell as Gtk.CellRendererText).Text = _dataService.GetLexemeForm(tree_model, iter);
		}

		public void OnRenderGloss(TreeViewColumn tree_column, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			(cell as Gtk.CellRendererText).Text = _dataService.GetGloss(tree_model, iter);
		}
	}
 }
