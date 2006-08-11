using System;
using System.Collections;
using Gtk;
using PicoContainer;
using System.Reflection;

namespace WeSay.App
{
	class WeSayApp
	{

		[STAThread]
		static void Main()
		{
			Application.Init();
			BasilProject project = new BasilProject(@"..\..\SampleProjects\Thai");
			IMutablePicoContainer container = CreateContainer();

			container.RegisterComponentInstance(project);
		   // container.RegisterComponentInstance("path", project.MainDBPath);
		  //  WeSay.DataAdaptor.Db4oDataSource ds = new WeSay.DataAdaptor.Db4oDataSource(project.MainDBPath);
		  //  container.RegisterComponentInstance("ds", ds);
		   // WeSay.DataAdaptor.Db4oDataAdaptor<WeSay.LexicalModel.LexEntry>.Configuration x = new WeSay.DataAdaptor.Db4oDataAdaptor<WeSay.LexicalModel.LexEntry>.Configuration(ds);

			//container.RegisterComponentInstance(x);


			//ds = container.GetComponentInstanceOfType(typeof(WeSay.DataAdaptor.Db4oDataSource)) as WeSay.DataAdaptor.Db4oDataSource;

			WeSay.UI.IAppShell shell = new TabAppShell(project, new SampleTaskBuilder(container));
			Application.Run();
		}

		private static IMutablePicoContainer CreateContainer()
		{
			IMutablePicoContainer pico = new PicoContainer.Defaults.DefaultPicoContainer();


			System.Collections.Specialized.StringCollection assemblies = new System.Collections.Specialized.StringCollection();
			assemblies.Add(@"CommonTools.dll");
			assemblies.Add(@"LexicalTools.dll");
			assemblies.Add(@"LexicalModel.Tests.dll");
		   // assemblies.Add(@"DataAdaptor.dll");

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


		protected static IList FindTypesToRegister(IList assemblies)
		{
			IList registerTypes = new ArrayList();
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