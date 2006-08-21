using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Gtk;
using WeSay.UI;
using PicoContainer;
using System.Collections.Specialized;
using WeSay.LexicalModel;

namespace WeSay.App
{
	public class SampleTaskBuilder : WeSay.UI.ITaskBuilder, IDisposable
	{
		private bool _disposed = false;
		private IMutablePicoContainer _picoContext;

	   public SampleTaskBuilder(BasilProject project)
		{
			_picoContext =  CreateContainer();
			_picoContext.RegisterComponentInstance(project);

			WeSay.Data.Db4oDataSource ds = new WeSay.Data.Db4oDataSource(project.PathToLexicalModelDB);
			IComponentAdapter dsAdaptor= _picoContext.RegisterComponentInstance(ds);

			/* Because the data source is never actually touched by the normal pico container code,
			 * it never gets  added to this ordered list.  The ordered list is used for the lifecycle
			 * functions, such as dispose.  Without adding it explicitly, this will end up
			 * getting disposed of first, whereas we need to it to be disposed of last.
			 * Adding it explicity to the ordered list gives proper disposal order.
			 */
			_picoContext.AddOrderedComponentAdapter(dsAdaptor);

			WeSay.Data.Db4oBindingList<LexEntry> entries = new WeSay.Data.Db4oBindingList<LexEntry>(ds);
			_picoContext.RegisterComponentInstance(entries);
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


		//TODO(JH): having a builder than needs to be kept around so it can be disposed of is all wrong.
		//either I want to change it to something like TaskList rather than ITaskBuilder, or
		//it needs to create some disposable object other than a IList<>.
		//The reason we need to be able to dispose of it is because we need some way to
		//dispose of things that it might create, such as a data source.

		public void Dispose()
		{
			if (!this._disposed)
			{
				_picoContext.Dispose();
				_picoContext = null;
				GC.SuppressFinalize(this);
			}
			_disposed = true;
		}

		private ITask CreateTool(string fullToolName)
		{
		   return (ITask)_picoContext.GetComponentInstance(fullToolName);
		}


		private static IMutablePicoContainer CreateContainer()
		{
			IMutablePicoContainer pico = new PicoContainer.Defaults.DefaultPicoContainer();

			List<String> assemblies = new List<string>();
			assemblies.Add(@"CommonTools.dll");
			assemblies.Add(@"LexicalTools.dll");
			// assemblies.Add(@"LexicalModel.Tests.dll");
			//     assemblies.Add(@"WeSayData.dll");

			foreach (Type t in FindTypesToRegister(assemblies))
			{
				if (t.IsAbstract || t.IsInterface)
				{
				}
				else
				{
					pico.RegisterComponentImplementation(t.ToString(), t);
				}
			}

			return pico;
		}


		protected static IList<Type> FindTypesToRegister(IList<string> assemblies)
		{
			IList<Type> registerTypes = new List<Type>();
			foreach (string assembly in assemblies)
			{
				Type[] types = Assembly.LoadFrom(assembly).GetTypes();
				foreach (Type type in types)
				{
					registerTypes.Add(type);
				}
			}
			return registerTypes;
		}
	}
}
