using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using WeSay.Project;

namespace WeSay.LexicalTools.Dashboard
{
	public class DashboardConfiguration: ITaskConfiguration
	{
		public override string ToString()
		{
			return "Dashboard";
		}

		public string TaskName
		{
			get { return "Dashboard"; }
		}

		public string Label
		{
			get { return "Dashboard"; }
		}

		public string LongLabel
		{
			get { return "Dashboard"; }
		}

		public string Description
		{
			get { return "This task is the first thing the user sees when WeSay comes up.  It gives the user some status of the project and reminds him what he was working on. Finally, this tab will allow him to switch to a different work task, if you have enabled more than one."; }
		}

		public string RemainingCountText
		{
			get { return String.Empty; }
		}

		public string ReferenceCountText
		{
			get { return String.Empty; }
		}

		public bool IsPinned
		{
			get { return true; }
		}

		public bool IsAvailable
		{
			get { return true; }
		}

		public virtual bool IsOptional
		{
			get { return false; }
		}

		public void Write(XmlWriter writer)
		{
			writer.WriteStartElement("task");
			writer.WriteAttributeString("taskName", TaskName);
			writer.WriteAttributeString("visible", IsVisible ? "true" : "false");
			writer.WriteEndElement();
		}

		public bool AreEquivalent(ITaskConfiguration taskConfiguration)
		{
			return taskConfiguration is DashboardConfiguration;
		}

		public bool IsVisible
		{
			set
			{
			}
			get { return true; }
		}
	}
}
