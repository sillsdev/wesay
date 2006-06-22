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
		protected Gtk.Table _wordDetailTable;
		protected Gtk.Entry _word;
		protected Gtk.Entry _gloss;
		protected Gtk.ToolButton _buttonForward;
		 protected Gtk.ToolButton _buttonBackward;
		  protected Gtk.ToolButton _buttonFirst;
		  protected Gtk.ToolButton _buttonLast;
   protected Gtk.VBox _detailVBox;

		public WordDetailView(Gtk.VBox detailVBox, DataService dataService)
		{
			_detailVBox = detailVBox;

			Gtk.ToolButton  b = (Gtk.ToolButton )((Gtk.Toolbar)_detailVBox.Children[0]).Children[3];
			b.Clicked += new EventHandler(first_Clicked);
			b = (Gtk.ToolButton)((Gtk.Toolbar)_detailVBox.Children[0]).Children[4];
			b.Clicked += new EventHandler(back_Clicked);
			b = (Gtk.ToolButton)((Gtk.Toolbar)_detailVBox.Children[0]).Children[5];
			b.Clicked += new EventHandler(next_Clicked);
			b = (Gtk.ToolButton)((Gtk.Toolbar)_detailVBox.Children[0]).Children[6];
			b.Clicked += new EventHandler(last_Clicked);

			_wordDetailTable = (Gtk.Table) detailVBox.Children[1];
		   _dataService = dataService;
		   _word = (Gtk.Entry) _wordDetailTable.Children[3];
		   _word.FocusOutEvent += new FocusOutEventHandler(OnWord_FocusOutEvent);
		   _word.EditingDone += new EventHandler(OnWord_EditingDone);
		   _gloss = (Gtk.Entry)_wordDetailTable.Children[2];
			_gloss.EditingDone+=new EventHandler(OnGloss_EditingDone);
		   _gloss.FocusOutEvent += new FocusOutEventHandler(OnGloss_FocusOutEvent);

			Update();
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
