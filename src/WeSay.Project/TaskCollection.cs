using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Autofac;
using System.Linq;

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
#if MONO
					if(task.TaskName != "NotesBrowser")
#endif
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
