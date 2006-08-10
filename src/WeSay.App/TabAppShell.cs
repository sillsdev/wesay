using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using WeSay.UI;

namespace WeSay.App
{
	public class TabAppShell : WeSay.UI.IAppShell
	{
		private Gtk.Window window;
		private System.Collections.Hashtable _tabsToTools;
		private ITask _currentTool;
		private BasilProject _project;

		public TabAppShell(BasilProject project, ITaskBuilder taskBuilder)
		{
			_project = project;
			_tabsToTools = new System.Collections.Hashtable();

			window = new Gtk.Window ("WeSay");
			window.SetDefaultSize (600, 400);
			window.DeleteEvent += new DeleteEventHandler (WindowDelete);

			HBox hbox = new HBox (false, 0);
			window.Add (hbox);

			Notebook notebook = new Notebook ();
			notebook.SwitchPage += new SwitchPageHandler(OnNotebookSwitchPage);
			hbox.PackStart(notebook, true, true, 0);
			foreach (ITask t in taskBuilder.Tasks)
			{
				VBox container = new VBox();
				t.Container = container;
				int i = notebook.AppendPage(container, new Label(t.Label));
				_tabsToTools.Add(i, t);
			}

			window.ShowAll ();
		}

		void OnNotebookSwitchPage(object o, SwitchPageArgs args)
		{
			ITask t = (ITask)_tabsToTools[(int)args.PageNum];
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
