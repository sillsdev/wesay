using System;
using Gtk;
using Glade;
using WeSay.Core;

namespace WeSay.UI
{
	/// <summary>
	/// Summary description for Driver.
	/// </summary>
	class Driver : IDisposable
	{
		#region Glade Widgets
		#pragma warning disable 649
		[Widget] Gtk.Window window1;
		[Widget] Gtk.Notebook _tabControl;
		[Widget] Gtk.TreeView _entryList;
		 [Widget] Gtk.VBox _detailVBox;
	   #pragma warning restore 649
		#endregion

		protected DataService _model;
		protected WordGridHandler _wordGridHandler;
		protected WordDetailView _wordDetailView;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.Init();
			Driver d = new Driver(args);
			Application.Run();
		}

		public Driver(string[] args)
		{
			Glade.XML gxml = new Glade.XML("probe.glade", "window1", null);
			gxml.Autoconnect(this);
			_model = new DataService(@"c:\WeSay\src\unittests\thai5000.yap");

			_wordGridHandler = new WordGridHandler(_entryList,_model);
			_wordDetailView = new WordDetailView(_detailVBox, _model);
	 }

		public void Dispose()
		{
			_model.Dispose();
		}

		// Connect the Signals defined in Glade
		public void OnWindowDeleteEvent(object o, DeleteEventArgs args)
		{
			Application.Quit();
			args.RetVal = true;
			Dispose();
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

		protected void on_buttonAddClicked(object o, EventArgs args)
		{
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


		protected void on_quit1_activate(object o, EventArgs args)
		{
			Application.Quit();
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
