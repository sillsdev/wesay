using System;
using System.Collections.Generic;
using System.Text;
using Gtk;

namespace WeSay.App
{
	public class DictionaryTool : WeSay.App.ITool
	{
		private HBox _container;

		public DictionaryTool()
		{
		}

		public void Activate()
		{
			_container.PackStart(new Gtk.Label("dict"));
			_container.ShowAll();
		}

		public void Deactivate()
		{
			_container.Remove(_container.Children[0]);
		}

		public string Label
		{
			get {return "Dictionary";}
		}

		public HBox Container
		{
			get { return _container; }
			set { _container = value; }
		}
	}
}
