using System;
using Gtk;
using Glade;
using WeSay.Core;

namespace Probe
{
	/// <summary>
	/// Summary description for Driver.
	/// </summary>
	class Driver
	{
		#region Glade Widgets
#pragma warning disable 649
		[Widget] Gtk.Window window1;
		[Widget] Gtk.Notebook _tabControl;
		[Widget] Gtk.TreeView _entryList;
#pragma warning restore 649
		#endregion

		DataService _data;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
	  Application.Init();
			new Driver(args);
	  Application.Run();
		}

		public Driver(string[] args)
		{
			Glade.XML gxml = new Glade.XML ("probe.glade", "window1", null);
			gxml.Autoconnect (this);

		   AddColumn("Word", new TreeCellDataFunc(OnRenderLexemeForm));
		   AddColumn("Gloss", new TreeCellDataFunc(OnRenderGloss));

		   TreeStore store = new TreeStore(typeof(int));
		   _data = new DataService(@"c:\WeSay\src\unittests\thai5000.yap");

		   int count = _data.LexicalEntries.Count;
		   for(int i=0; i< count; i++)
		   {
			   store.AppendValues(i);
		   }

		   _entryList.Model = store;

			Application.Run();
		}

		private void AddColumn(string title, TreeCellDataFunc handler)
		{
			Gtk.CellRendererText renderer = new Gtk.CellRendererText();
			Gtk.TreeViewColumn column = new Gtk.TreeViewColumn(title, renderer, new object[] { });
			column.SetCellDataFunc(renderer, handler);
			 _entryList.AppendColumn(column);
		}

		public void OnRenderLexemeForm(TreeViewColumn tree_column, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			LexicalEntry entry = GetEnterFromIterator(tree_model, ref iter);
			(cell as Gtk.CellRendererText).Text = entry.LexicalForm;
		}

		public void OnRenderGloss(TreeViewColumn tree_column, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			LexicalEntry entry = GetEnterFromIterator(tree_model, ref iter);
			(cell as Gtk.CellRendererText).Text = entry.Gloss;
		}

		private LexicalEntry GetEnterFromIterator(TreeModel tree_model, ref TreeIter iter)
		{
			int i = ((int)tree_model.GetValue(iter, 0));
			LexicalEntry entry = (LexicalEntry)_data.LexicalEntries[i];
			return entry;
		}


		// Connect the Signals defined in Glade
		public void OnWindowDeleteEvent(object o, DeleteEventArgs args) {
			Application.Quit();
			args.RetVal = true;
		}

		protected void TreeModelIfaceDelegates(object o, EventArgs args)
		{
			return;
		}

		protected void OnTabControl_switch_page(object o, SwitchPageArgs args)
		{

		}
		protected void OnEntryList_show(object o, EventArgs args)
		{

		}
		#region Button Click Event handlers

		protected void on_toolbutton1_clicked(object o, EventArgs args)
		{
			return;
		}

		protected void on_toolbutton2_clicked(object o, EventArgs args)
		{
			FileSelection fDlg = new FileSelection("Choose a File");
			fDlg.Modal = true;

			int nRc = fDlg.Run();
			fDlg.Hide();

			if(nRc == (int)ResponseType.Ok)
			{
			}
			return;
		}

		protected void on_toolbutton3_clicked(object o, EventArgs args)
		{
			Application.Quit();
			return;
		}

		#endregion

		#region Menu item handlers

		protected void on_new1_activate(object o, EventArgs args)
		{
			return;
		}

		protected void on_open1_activate(object o, EventArgs args)
		{
			FileSelection fDlg = new FileSelection("Choose a File");
			fDlg.Modal = true;

			int nRc = fDlg.Run();
			fDlg.Hide();

			if(nRc == (int)ResponseType.Ok)
			{
			}
			return;
		}

		protected void on_save1_activate(object o, EventArgs args)
		{
			return;
		}

		protected void on_save_as1_activate(object o, EventArgs args)
		{
			return;
		}

		protected void on_quit1_activate(object o, EventArgs args)
		{
			Application.Quit();
			return;
		}

		protected void on_cut1_activate(object o, EventArgs args)
		{
			return;
		}

		protected void on_copy1_activate(object o, EventArgs args)
		{
			return;
		}

		protected void on_delete1_activate(object o, EventArgs args)
		{
			return;
		}

		protected void on_paste1_activate(object o, EventArgs args)
		{
			return;
		}

		protected void on_about1_activate(object o, EventArgs args)
		{
			System.Text.StringBuilder AuthorStringBuild = new System.Text.StringBuilder ();

			AuthorStringBuild.Append ("gladesharp1 version 1.0\n\n");
			AuthorStringBuild.Append ("Sample Glade Application.\n");
			AuthorStringBuild.Append ("Copyright (c) 2004\n\n");

			Gtk.MessageDialog md = new Gtk.MessageDialog (
				this.window1,
				DialogFlags.DestroyWithParent,
				MessageType.Info,
				ButtonsType.Ok,
				AuthorStringBuild.ToString ()
				);

			int result = md.Run ();
			md.Hide();

			return;
		}

		#endregion

		// Common functions use by buttons and menu items
	}
}
