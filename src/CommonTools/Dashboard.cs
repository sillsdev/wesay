using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using WeSay.UI;

namespace WeSay.CommonTools
{

	public class Dashboard : ITask
	{
  //      private VBox _container;
		private IBindingList _records;
		private WeSay.UI.IProject _project;
		private Control _control;

		public Dashboard(WeSay.UI.IProject project, IBindingList records)
		{
			_records = records;
			_project = project;
		}

		public void Activate()
		{
			//_container.PackStart(new Gtk.Label(_project.Name));
			//_container.PackStart(new Gtk.Label(_records.Count.ToString() + " Records"));

			//Gtk.HTML _html = new HTML();
			//_html.LoadFromString("<html>Hello</html>");
			//_container.PackStart(_html);
			//_container.ShowAll();
		}

		public void Deactivate()
		{
		//    _container.Children[0].Destroy();
		}

		public string Label
		{
			get { return "Start"; }
		}

		public Control Control
		{
			get { return _control; }
		}


	}
}
