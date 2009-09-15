using System;
using System.Collections.Generic;
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
		public void RegisterAllTypes(Autofac.Builder.ContainerBuilder builder)
		{

			RegisterTask(builder, "Dashboard",
						 "WeSay.LexicalTools.Dashboard.Dash",
						 "WeSay.LexicalTools.Dashboard.DashboardConfiguration",
						 "LexicalTools");
			RegisterTask(builder, "Dictionary",
						 "WeSay.LexicalTools.DictionaryBrowseAndEdit.DictionaryTask",
						 "WeSay.LexicalTools.DictionaryBrowseAndEdit.DictionaryBrowseAndEditConfiguration",
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
						 "WeSay.LexicalTools.Review.AdvancedHistory.AdvancedHistoryTaskConfig",
						 "LexicalTools");

//            Type type = GetType( "WeSay.LexicalTools.Review.AdvancedHistory.AdvancedHistoryControl", assembly);
//            TaskNameToTaskType.Add(name, type);
//            builder.Register(type).Named(name).FactoryScoped();

		}
//
//        private void RegisterDictionaryTask(Autofac.Builder.ContainerBuilder builder, string name, string classPath, string configClassPath, string assembly)
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

		private void RegisterTask(Autofac.Builder.ContainerBuilder builder, string name, string classPath, string configClassPath, string assembly)
		{
			// _taskNameToClassPath.Add(name, classPath);
			Type type = GetType(classPath, assembly);
			TaskNameToTaskType.Add(name, type);
			builder.Register(type).Named(name).FactoryScoped();

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
				builder.Register(type).Named(name + "Config").FactoryScoped();
			}
		}

		public static Type GetType(string className, string assembly)
		{
			return Type.GetType(className + "," + assembly, true);
		}
	}
}