using System;
using Gtk;
using Glade;
using WeSay.Core;
using Spring.Context;
using Spring.Context.Support;

namespace WeSay.UI
{
  /// <summary>
  /// Summary description for Driver.
  /// </summary>
  class Driver : IDisposable
  {
	#region Glade Widgets
#pragma warning disable 649
	[Widget]
	Gtk.Window window1;
	[Widget]
	Gtk.Notebook _tabControl;
#pragma warning restore 649
	#endregion

	protected LexiconModel _model;

	[STAThread]
	static void Main(string[] args) {
	  Application.Init();

	  Driver d = new Driver(args);
	  Application.Run();
	}

	public Driver(string[] args) {
	  string filePath;
	  if (args.Length == 0) {
		filePath = @"c:\WeSay\src\unittests\thai5000.yap";
	  }
	  else {
		filePath = args[0];
	  }

	  Glade.XML gxml = new Glade.XML("probe.glade", "window1", null);
	  gxml.Autoconnect(this);

	  LexiconModel.s_FilePath = filePath;//hack
	  WordActionsView.s_tabcontrol = _tabControl;//hack

	  //hold on to your seats... this is where the ui gets built.
	  // IApplicationContext ctx = new Spring.Context.Support.XmlApplicationContext("file://AppContext.xml"); // ContextRegistry.GetContext();
	  IApplicationContext ctx = new Spring.Context.Support.XmlApplicationContext("assembly://Probe/WeSay.Probe/AppContext.xml"); // ContextRegistry.GetContext();

	  _model = (LexiconModel) ctx.GetObject("TheModel");// new LexiconModel(filePath);
	}

	public void Dispose() {
	  _model.Dispose();
	}

	// Connect the Signals defined in Glade
	public void OnWindowDeleteEvent(object o, DeleteEventArgs args) {
	  Application.Quit();
	  args.RetVal = true;
	  Dispose();
	}
  }
}
