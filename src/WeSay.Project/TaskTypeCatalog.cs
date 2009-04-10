using System;
using System.Collections.Generic;

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

			RegisterTask(builder, "Dashboard", "WeSay.LexicalTools.Dashboard.Dash", "WeSay.LexicalTools.Dashboard.DashboardConfiguration", "LexicalTools");
			RegisterTask(builder, "Dictionary", "WeSay.LexicalTools.DictionaryBrowseAndEdit.DictionaryTask",
						 "WeSay.LexicalTools.DictionaryBrowseAndEdit.DictionaryBrowseAndEditConfiguration",
						 "LexicalTools");
			RegisterTask(builder, "GatherWordsBySemanticDomains", "WeSay.LexicalTools.GatherBySemanticDomainTask",
						 "WeSay.LexicalTools.GatherBySemanticDomainConfig", "LexicalTools");

			RegisterTask(builder, "AddMissingInfo", "WeSay.LexicalTools.AddMissingInfo.MissingInfoTask",
						 "WeSay.LexicalTools.AddMissingInfo.MissingInfoConfiguration", "LexicalTools");

			RegisterTask(builder, "GatherWordList", "WeSay.LexicalTools.GatherWordListTask",
						 "WeSay.LexicalTools.GatherByWordList.GatherWordListConfig", "LexicalTools");

		}

		private void RegisterTask(Autofac.Builder.ContainerBuilder builder, string name, string classPath, string configClassPath, string assembly)
		{
			// _taskNameToClassPath.Add(name, classPath);
			Type type = GetType(classPath, assembly);
			TaskNameToTaskType.Add(name, type);
			builder.Register(type).Named(name).FactoryScoped();

			//register the class that holds the configuration for this task
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