using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using PicoContainer;
using PicoContainer.Defaults;
using WeSay.Data;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Tests;
using WeSay.UI;
using System.Collections;

namespace WeSay.App
{
	public class SampleTaskBuilder : ITaskBuilder, IDisposable
	{
		private bool _disposed = false;
		private IMutablePicoContainer _parentPicoContext;
		BasilProject _project;

		public SampleTaskBuilder(BasilProject project)
		{
			_parentPicoContext = CreateContainer();
			_parentPicoContext.RegisterComponentInstance(project);
			_project = project;

			if (project.PathToLexicalModelDB.IndexOf("PRETEND") > -1)
			{
			  IBindingList pEntries = new PretendRecordList();
			  _parentPicoContext.RegisterComponentInstance(pEntries);
			}
			else
			{
				Db4oDataSource ds = new Db4oDataSource(project.PathToLexicalModelDB);
				IComponentAdapter dsAdaptor = _parentPicoContext.RegisterComponentInstance(ds);

				///* Because the data source is never actually touched by the normal pico container code,
				// * it never gets  added to this ordered list.  The ordered list is used for the lifecycle
				// * functions, such as dispose.  Without adding it explicitly, this will end up
				// * getting disposed of first, whereas we need it to be disposed of last.
				// * Adding it explicity to the ordered list gives proper disposal order.
				// */
				_parentPicoContext.AddOrderedComponentAdapter(dsAdaptor);

				Db4oBindingList<LexEntry> entries = new Db4oBindingList<LexEntry>(ds);
				_parentPicoContext.RegisterComponentInstance("All Entries", entries);
		   }

		}

		public IList<ITask> Tasks
		{
			get
			{
				List<ITask> tools = new List<ITask>();
				tools.Add(CreateTool("WeSay.CommonTools.DashboardControl,CommonTools"));
				tools.Add(CreateTool("WeSay.LexicalTools.EntryDetailTask,LexicalTools"));
				tools.Add(CreateTool("WeSay.LexicalTools.LexFieldTask,LexicalTools",
					CreateLexFieldConfiguration("Add Meanings", "GhostGloss")));
				tools.Add(CreateTool("WeSay.CommonTools.PictureControl,CommonTools",
					CreatePictureConfiguration("Collect Words", "RealWord.gif")));
				tools.Add(CreateTool("WeSay.CommonTools.PictureControl,CommonTools",
					CreatePictureConfiguration("Semantic Domains", "SemDom.gif")));
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
				_parentPicoContext.Dispose();
				_parentPicoContext = null;
				GC.SuppressFinalize(this);
			}
			_disposed = true;
		}

		private IMutablePicoContainer NewChildContainer(IList instances)
		{
			IMutablePicoContainer child = new DefaultPicoContainer(_parentPicoContext);
			_parentPicoContext.AddChildContainer(child);
			if (instances != null)
			{
				foreach (object instance in instances)
				{
					child.RegisterComponentInstance(instance);
				}
			}
			return child;
		}


		private ITask CreateTool(IMutablePicoContainer picoContext, string fullToolClass)
		{
			RegisterType(picoContext, fullToolClass);

		   ITask i = (ITask)picoContext.GetComponentInstance(fullToolClass);
		   System.Diagnostics.Debug.Assert(i != null);
		   return i;
		}

		private static void RegisterType(IMutablePicoContainer picoContext, string fullToolClass)
		{
			picoContext.RegisterComponentImplementation(fullToolClass, Type.GetType(fullToolClass, true));
		}


		private ITask CreateTool(string fullToolClass, IList instances)
		{
			return CreateTool(NewChildContainer(instances), fullToolClass);
		}

		private ITask CreateTool(string fullToolClass)
		{
			return CreateTool(_parentPicoContext, fullToolClass);
		}

		private ITask CreateTool(string fullToolClass, string label)
		{
			IList instances = new List<string>();
			instances.Add(label);
			return CreateTool(fullToolClass, instances);
		}

		private IList CreatePictureConfiguration(string label, string pictureFilePath)
		{
			IList instances = new List<object>();
			instances.Add(label);
			instances.Add(new System.Drawing.Bitmap(pictureFilePath));

			return instances;
		}

		private IList CreateLexFieldConfiguration(string label, string fieldToShow)
		{
			IList instances = new List<object>();
			Predicate<LexEntry> entryFilter = delegate(LexEntry entry)
						{
							if (entry.Senses.Count == 0)
							{
								return true;
							}
							foreach (LexSense sense in entry.Senses) {
								if (sense.Gloss[_project.AnalysisWritingSystemDefault.Id] == string.Empty)
								{
									return true;
								}
							}
							return false;
						};

			Db4oDataSource ds = (Db4oDataSource)_parentPicoContext.GetComponentInstance(typeof(Db4oDataSource));
			Db4oBindingList<LexEntry> entries = new Db4oBindingList<LexEntry>(ds, entryFilter);
			instances.Add(entries);
			instances.Add(label);
			System.Predicate<string> fieldFilter = delegate(string s)
							{
								return s == fieldToShow;
							};
			instances.Add(fieldFilter);

			return instances;
		}

		private static IMutablePicoContainer CreateContainer()
		{
			IMutablePicoContainer pico = new DefaultPicoContainer();

			//List<String> assemblies = new List<string>();
			//assemblies.Add(@"CommonTools.dll");
			//assemblies.Add(@"LexicalTools.dll");
			//// assemblies.Add(@"LexicalModel.Tests.dll");
			////     assemblies.Add(@"WeSayData.dll");

			//foreach (Type t in FindTypesToRegister(assemblies))
			//{
			//    if (t.IsAbstract || t.IsInterface)
			//    {
			//    }
			//    else
			//    {
			//        pico.RegisterComponentImplementation(t.ToString(), t);
			//    }
			//}

			return pico;
		}


		//protected static IList<Type> FindTypesToRegister(IList<string> assemblies)
		//{
		//    IList<Type> registerTypes = new List<Type>();
		//    foreach (string assembly in assemblies)
		//    {
		//        Type[] types = Assembly.LoadFrom(assembly).GetTypes();
		//        foreach (Type type in types)
		//        {
		//            registerTypes.Add(type);
		//        }
		//    }
		//    return registerTypes;
		//}

	}
}
