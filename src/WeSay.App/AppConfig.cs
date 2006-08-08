using System;
using System.Collections;
using System.Collections.Generic;
using Gtk;

namespace WeSay.App
{
	public class AppConfig
	{

		public AppConfig(Notebook _notebook, Hashtable tabsToTools)
		{
			AddTool(new DummyTool(), _notebook,tabsToTools );
			AddTool(new DictionaryTool(), _notebook,tabsToTools );
	   }

		private void AddTool(ITool tool, Notebook _notebook, Hashtable tabsToTools)
		{
			HBox container = new HBox();
			tool.Container = container;

			int i = _notebook.AppendPage(container, new Label(tool.Label));
			tabsToTools.Add(i, tool);
		}
	}
}
