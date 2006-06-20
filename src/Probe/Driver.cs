using System;

namespace Probe
{
	/// <summary>
	/// Summary description for Driver.
	/// </summary>
	class Driver
	{
		#region Glade Widgets
#pragma warning disable 649

	[Glade.Widget] Gtk.Window window1;

#pragma warning restore 649
	#endregion


	/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			new Driver(args);
		}

		public Driver(string[] args)
		{
			Gtk.Application.Init();

		   //for resource (couldn't get it to work) Glade.XML gxml = new Glade.XML (null, "probe.glade", "window1", null);
			Glade.XML gxml = new Glade.XML ("probe.glade", "window1", null);
			gxml.Autoconnect (this);
			Gtk.Application.Run();
		}

		// Connect the Signals defined in Glade
		public void on_window1_delete_event (object o, Gtk.DeleteEventArgs args)
		{
			Gtk.Application.Quit();
			args.RetVal = true;
		}

		#region Button Click Event handlers

		protected void on_toolbutton1_clicked(object o, EventArgs args)
		{
			return;
		}

		protected void on_toolbutton2_clicked(object o, EventArgs args)
		{
			Gtk.FileSelection fDlg = new Gtk.FileSelection("Choose a File");
			fDlg.Modal = true;

			int nRc = fDlg.Run();
			fDlg.Hide();

			if(nRc == (int)Gtk.ResponseType.Ok)
			{
			}
			return;
		}

		protected void on_toolbutton3_clicked(object o, EventArgs args)
		{
			Gtk.Application.Quit();
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
			Gtk.FileSelection fDlg = new Gtk.FileSelection("Choose a File");
			fDlg.Modal = true;

			int nRc = fDlg.Run();
			fDlg.Hide();

			if(nRc == (int)Gtk.ResponseType.Ok)
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
			Gtk.Application.Quit();
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
				Gtk.DialogFlags.DestroyWithParent,
				Gtk.MessageType.Info,
				Gtk.ButtonsType.Ok,
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