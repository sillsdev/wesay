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

		public SampleTaskBuilder()
		{
		 }

		public IList<ITask> Tasks
		{
			get
			{
				System.Collections.Specialized.StringCollection assemblies = new System.Collections.Specialized.StringCollection();
				assemblies.Add(@"CommonTools.dll");
				assemblies.Add(@"LexicalTools.dll");
				assemblies.Add(@"LexicalModel.Tests.dll");

				List<ITask> tools = new List<ITask>();
				tools.Add(CreateTool(assemblies, "WeSay.CommonTools.Dashboard"));
				tools.Add(CreateTool(assemblies, "WeSay.LexicalTools.EntryViewTool"));
				return tools;
			}
		}



		private ITask CreateTool(StringCollection assemblies, string fullToolName)
		{
		   IMutablePicoContainer pico = new PicoContainer.Defaults.DefaultPicoContainer();

		   Type toolType = null;
		   foreach (Type t in FindTypesToRegister(assemblies))
		   {
			   pico.RegisterComponentImplementation(t);
			   if (t.FullName == fullToolName)
				   toolType = t;
		   }

		   return (ITask)pico.GetComponentInstanceOfType(toolType);
		}


		protected IList FindTypesToRegister(IList assemblies)
		{
			IList registerTypes = new ArrayList();

			foreach(string assembly in assemblies)
			{
				Type[] types = Assembly.LoadFrom(assembly).GetTypes();


				foreach(Type type in types)
				{
					registerTypes.Add(type);
				}
			}

			return registerTypes;
		}

	}
}
