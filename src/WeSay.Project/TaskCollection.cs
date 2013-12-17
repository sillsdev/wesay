using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Autofac;

namespace WeSay.Project
{
	public class TaskCollection : List<ITaskConfiguration>
	{
		public TaskCollection(IContext context, ConfigFileReader configFileReader)
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
						String.Format("There was a problem reading {0}.  The error was: {1}",
									  WeSayWordsProject.PathToDefaultConfig,
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
