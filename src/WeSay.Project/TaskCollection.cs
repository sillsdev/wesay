using Autofac;
using System;
using System.Collections.Generic;
using System.Xml;

namespace WeSay.Project
{
	public class TaskCollection : List<ITaskConfiguration>
	{
		public TaskCollection(IComponentContext context, ConfigFileReader configFileReader)
		{

			try
			{
				foreach (var task in configFileReader.GetTasksConfigurations(context))
				{
					Add(task);
				}

			}
			catch (Exception error)
			{
				throw new ApplicationException(
						String.Format("There was a problem processing the tasks of the Config File (or default config file).  The error was: {0}",
									  error.Message));
			}
		}

		public void Write(XmlWriter writer)
		{
			writer.WriteStartElement("tasks");
			foreach (ITaskConfiguration configuration in this)
			{
				configuration.Write(writer);
			}
			writer.WriteEndElement();
		}
	}
}
