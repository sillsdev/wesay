using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using PicoContainer;
using PicoContainer.Defaults;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Tests;
using WeSay.UI;

namespace WeSay.App
{
	public class SampleTaskBuilder : ITaskBuilder, IDisposable
	{
		private bool _disposed = false;
		private IMutablePicoContainer _picoContext;

	   public SampleTaskBuilder(BasilProject project)
		{
			_picoContext =  CreateContainer();
			_picoContext.RegisterComponentInstance(project);

			//Db4oDataSource ds = new Db4oDataSource(project.PathToLexicalModelDB);
			//IComponentAdapter dsAdaptor= _picoContext.RegisterComponentInstance(ds);

			///* Because the data source is never actually touched by the normal pico container code,
			// * it never gets  added to this ordered list.  The ordered list is used for the lifecycle
			// * functions, such as dispose.  Without adding it explicitly, this will end up
			// * getting disposed of first, whereas we need to it to be disposed of last.
			// * Adding it explicity to the ordered list gives proper disposal order.
			// */
			//_picoContext.AddOrderedComponentAdapter(dsAdaptor);

			//Db4oBindingList<LexEntry> entries = new Db4oBindingList<LexEntry>(ds);

			IBindingList entries = new PretendRecordList();

			_picoContext.RegisterComponentInstance(entries);
		}

		public IList<ITask> Tasks
		{
			get
			{
				List<ITask> tools = new List<ITask>();
				tools.Add(CreateTool( "WeSay.CommonTools.DashboardControl"));
				tools.Add(CreateTool("WeSay.LexicalTools.EntryDetailControl"));
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
		   ITask i = (ITask)_picoContext.GetComponentInstance(fullToolName);
		   System.Diagnostics.Debug.Assert(i != null);
		   return i;
		}


		private static IMutablePicoContainer CreateContainer()
		{
			IMutablePicoContainer pico = new DefaultPicoContainer();

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
