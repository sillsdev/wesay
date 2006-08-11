using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Gtk;
using WeSay.UI;
using PicoContainer;
using System.Collections.Specialized;

namespace WeSay.App
{
	public class SampleTaskBuilder : WeSay.UI.ITaskBuilder
	{
		private IMutablePicoContainer _picoContext;

		public SampleTaskBuilder(IMutablePicoContainer picoContext)
		{
			_picoContext = picoContext;
		}

		public IList<ITask> Tasks
		{
			get
			{
				List<ITask> tools = new List<ITask>();
				tools.Add(CreateTool( "WeSay.CommonTools.Dashboard"));
				tools.Add(CreateTool( "WeSay.LexicalTools.EntryViewTool"));
				return tools;
			}
		}

		private ITask CreateTool(string fullToolName)
		{
		   return (ITask)_picoContext.GetComponentInstance(fullToolName);
		}

	}
}
