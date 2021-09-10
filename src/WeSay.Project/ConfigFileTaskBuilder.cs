using Autofac;
using Autofac.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WeSay.Project
{
	/// <summary>
	/// Given a list of task configurations, creates tasks from it.
	/// </summary>
	public class ConfigFileTaskBuilder
	{
		public static IList<ITask> CreateTasks(IComponentContext context, IEnumerable taskConfigurations)
		{
			var tasks = new List<ITask>();
			foreach (ITaskConfiguration config in taskConfigurations)
			{
				if (config.IsVisible && config.IsAvailable)
				{
					tasks.Add(CreateTask(context, config));
				}
			}
			return tasks;
		}



		private static ITask CreateTask(IComponentContext context, ITaskConfiguration config)
		{
			try
			{
				//make the task itself, handing it this configuration object.
				//its other constructor arguments come "automatically" out of the context
				return context.ResolveNamed<ITask>(config.TaskName, new Parameter[] { new NamedParameter("config", config) });
			}
			catch (Exception e)
			{
				string message = e.Message;
				while (e.InnerException != null) //the user will see this, so lets dive down to the actual cause
				{
					e = e.InnerException;
					message = e.Message;
				}
				return new FailedLoadTask(config.TaskName, "", message);
			}
		}
	}
}
