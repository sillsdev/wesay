using System;
using System.Collections.Generic;
using Gtk;

namespace WeSay.App
{
	class WeSayApp
	{
		private Gtk.Window window;
		private System.Collections.Hashtable _tabsToTools;
		private ITool _currentTool;


		[STAThread]
		static void Main()
		{
			Application.Init();
			new WeSayApp();
			Application.Run();
		}


		public WeSayApp()
		{
			_tabsToTools = new System.Collections.Hashtable();

			window = new Gtk.Window ("WeSay");
			window.SetDefaultSize (600, 400);
			window.DeleteEvent += new DeleteEventHandler (WindowDelete);

			HBox hbox = new HBox (false, 0);
			window.Add (hbox);

			Notebook notebook = new Notebook ();
			notebook.SwitchPage += new SwitchPageHandler(OnNotebookSwitchPage);
			hbox.PackStart(notebook, true, true, 0);
			new AppConfig(notebook,  _tabsToTools);
			window.ShowAll ();
		}

		void OnNotebookSwitchPage(object o, SwitchPageArgs args)
		{
			ITool t = (ITool)_tabsToTools[(int)args.PageNum];
		   if (_currentTool == t)
			   return; //debounce

		   if(_currentTool!=null)
			 _currentTool.Deactivate();
			if(t!=null)
				t.Activate();
			_currentTool = t;
		}

		private void WindowDelete(object o, DeleteEventArgs args)
		{
			Application.Quit();
			args.RetVal = true;
		}

	}
}