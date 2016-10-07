using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Builder;
using WeSay.UI;

namespace WeSay.Project
{
	public class TaskTypeCatalog
	{
		public Dictionary<string, Type> TaskNameToTaskType { get; set; }
		public Dictionary<string, Type> TaskNameToConfigType { get; set; }

		public TaskTypeCatalog()
		{
			TaskNameToTaskType = new Dictionary<string, Type>();
			TaskNameToConfigType = new Dictionary<string, Type>();
		}
		public void RegisterAllTypes(ContainerBuilder builder)
		{
			RegisterTask(builder, "Dashboard",
						 "WeSay.LexicalTools.Dashboard.Dash",
						 "WeSay.LexicalTools.Dashboard.DashboardConfiguration",
						 "LexicalTools");
			RegisterTask(builder, "Dictionary",
						 "WeSay.LexicalTools.DictionaryBrowseAndEdit.DictionaryTask",
						 "WeSay.LexicalTools.DictionaryBrowseAndEdit.DictionaryBrowseAndEditConfiguration",
						 "LexicalTools");

			RegisterControlAndFactory(builder,
						"WeSay.LexicalTools.DictionaryBrowseAndEdit.DictionaryControl",
						 "LexicalTools");

			RegisterControlAndFactory(builder,
						"WeSay.LexicalTools.EntryHeaderView",
						 "LexicalTools");

			RegisterControlAndFactory(builder,
						"WeSay.LexicalTools.EntryViewControl",
						 "LexicalTools");

			//builder.Register<ITaskForExternalNavigateToEntry>(c => (ITaskForExternalNavigateToEntry) c.Resolve("Dictionary"));

			RegisterTask(builder, "GatherWordsBySemanticDomains",
						 "WeSay.LexicalTools.GatherBySemanticDomains.GatherBySemanticDomainTask",
						 "WeSay.LexicalTools.GatherBySemanticDomains.GatherBySemanticDomainConfig",
						 "LexicalTools");
			RegisterTask(builder, "AddMissingInfo",
						 "WeSay.LexicalTools.AddMissingInfo.MissingInfoTask",
						 "WeSay.LexicalTools.AddMissingInfo.MissingInfoConfiguration",
						 "LexicalTools");
			RegisterTask(builder, "GatherWordList",
						 "WeSay.LexicalTools.GatherByWordList.GatherWordListTask",
						 "WeSay.LexicalTools.GatherByWordList.GatherWordListConfig",
						 "LexicalTools");
			RegisterTask(builder, "AdvancedHistory",
						 "WeSay.LexicalTools.Review.AdvancedHistory.AdvancedHistoryTask",
						 "WeSay.LexicalTools.Review.AdvancedHistory.AdvancedHistoryConfig",
						 "LexicalTools");

			RegisterTask(builder, "NotesBrowser",
				 "WeSay.LexicalTools.Review.NotesBrowser.NotesBrowserTask",
				 "WeSay.LexicalTools.Review.NotesBrowser.NotesBrowserConfig",
				 "LexicalTools");

			Type type = GetType("WeSay.LexicalTools.DictionaryBrowseAndEdit.ConfirmDelete", "LexicalTools");
			Type typeInterface = GetType("WeSay.LexicalTools.IConfirmDelete", "LexicalTools");
			builder.RegisterType(type).As(typeInterface).InstancePerDependency();

//            Type type = GetType( "WeSay.LexicalTools.Review.AdvancedHistory.AdvancedHistoryControl", assembly);
//            TaskNameToTaskType.Add(name, type);
//            builder.Register(type).Named(name).FactoryScoped();

		}
//
//        private void RegisterDictionaryTask(ContainerBuilder builder, string name, string classPath, string configClassPath, string assembly)
//        {
//            // _taskNameToClassPath.Add(name, classPath);
//            Type type = GetType(classPath, assembly);
//            TaskNameToTaskType.Add(name, type);
//
//            //review: there's probably a cleaner way to do this, is it even necessary?
//
//            var interfaces = new Type[] {typeof(ITask), typeof(ITaskForExternalNavigateToEntry) };
//
//            builder.Register(type).Named(name).As(interfaces).FactoryScoped();
//
//            //register the class that holds the configuration for this task
//            RegisterConfigurationClass(assembly, name, builder, configClassPath);
//        }

		/// <summary>
		/// this is a hack fo now (root problem is that this assumbly can't point directly at
		/// lexicaltools, because of circular dependencies).  Many ways to fix that...
		/// </summary>
		private void RegisterControlAndFactory(ContainerBuilder builder,
					string classPath, string assembly)
		{
			Type type = GetType(classPath, assembly);
			builder.RegisterType(type).InstancePerDependency();
			Type ftype = type.GetNestedType("Factory");
			builder.RegisterGeneratedFactory(ftype).InstancePerDependency();//review
		}

		private void RegisterTask(ContainerBuilder builder, string name, string classPath, string configClassPath, string assembly)
		{
			// _taskNameToClassPath.Add(name, classPath);
			Type type = GetType(classPath, assembly);
			TaskNameToTaskType.Add(name, type);
			builder.RegisterType(type).Named<ITask>(name).As<ITask>().InstancePerDependency();

			//register the class that holds the configuration for this task
			RegisterConfigurationClass(assembly, name, builder, configClassPath);
		}

		private void RegisterConfigurationClass(string assembly, string name, ContainerBuilder builder, string configClassPath)
		{
			Type type;
			if (!string.IsNullOrEmpty(configClassPath))
			{
				type = GetType(configClassPath, assembly);
				TaskNameToConfigType.Add(name, type);
				builder.RegisterType(type).Named<ITaskConfiguration>(name + "Config").As<ITaskConfiguration>().InstancePerDependency();
			}
		}

		public static Type GetType(string className, string assembly)
		{
			return Type.GetType(className + "," + assembly, true);
		}
	}
}