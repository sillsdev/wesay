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
		[Widget]  Gtk.HBox _dummyDetailViewHolder;
		[Widget]  Gtk.VBox _dummyWordGridViewHolder;
		#pragma warning restore 649
		#endregion

		protected DataService _model;

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

			WordGridHandler g = new WordGridHandler(_dummyWordGridViewHolder, _model);
			WordDetailView d = new WordDetailView(_dummyDetailViewHolder, _model);
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

		#region Button Click Event handlers


		#endregion

		#region Menu item handlers

		protected void on_quit1_activate(object o, EventArgs args)
		{
			Application.Quit();
			return;
		}


		#endregion
	}
}
