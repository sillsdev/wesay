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
		   //for resource (couldn't get it to work) Glade.XML gxml = new Glade.XML (null, "probe.glade", "window1", null);
			Glade.XML gxml = new Glade.XML ("probe.glade", "window1", null);
			gxml.Autoconnect (this);
			this._tabControl.SwitchPage += new SwitchPageHandler(OnTabControl_switch_page);


			Gtk.CellRendererText lexicalFormCellRenderer = new Gtk.CellRendererText();
			 Gtk.TreeViewColumn lexicalFormColumn = new Gtk.TreeViewColumn("Word", lexicalFormCellRenderer, new object[] { });
			//lexicalFormColumn.PackStart(lexicalFormCell, true);
		  //  lexicalFormColumn.SetCellDataFunc(lexicalFormCell, new CellLayoutDataFunc(RX));
		  //  lexicalFormColumn.SetCellDataFunc(lexicalFormCell, new NodeCellDataFunc(NX));
			 lexicalFormColumn.SetCellDataFunc(lexicalFormCellRenderer, new TreeCellDataFunc(TX));


		   TreeStore store = new TreeStore(typeof(int));
		   _data = new DataService(@"c:\WeSay\src\unittests\thai5000.yap");
			//           foreach (LexicalEntry entry in data.LexicalEntries)
			int count = _data.LexicalEntries.Count;
			for(int i=0; i< count; i++)
		   {
			   store.AppendValues(i);
			}

		   _entryList.Model = store;
		   _entryList.AppendColumn(lexicalFormColumn);

		   _entryList.ShowAll();
		   window1.ShowAll();

			Application.Run();
		}

		public void TX(TreeViewColumn tree_column, CellRenderer cell, TreeModel tree_model, TreeIter iter)
		{
			//LexicalEntry entry = (LexicalEntry)tree_model.GetValue(iter, 0);
			//(cell as Gtk.CellRendererText).Text = entry.Gloss;
//            int i = ((Z)tree_model.GetValue(iter, 0)).x;
			int i = ((int)tree_model.GetValue(iter, 0));
			LexicalEntry entry = (LexicalEntry)_data.LexicalEntries[i];
			(cell as Gtk.CellRendererText).Text = entry.Gloss;
		}



		//private void RenderGloss(CellLayout cell_layout, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		//{
		//    LexicalEntry entry = (LexicalEntry)model.GetValue(iter, 0);
		//    (cell as Gtk.CellRendererText).Text = entry.Gloss;
		//}
		//void tabControl_SwitchPage(object o, SwitchPageArgs args)
		//{
		//    throw new Exception("The method or operation is not implemented.");
		//}

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
