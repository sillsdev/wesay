using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using WeSay.UI;
using Gtk;

namespace WeSay.CommonTools
{

	public class Dashboard : ITask
	{
		private VBox _container;
		private IBindingList _records;

		public Dashboard(IBindingList records)
		{
			_records = records;
		}

		public void Activate()
		{
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

		public VBox Container
		{
			get { return _container; }
			set { _container = value; }
		}
	}
}
