using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using WeSay.UI;

namespace WeSay.Admin
{
	public partial class TaskListControl : UserControl
	{

		public TaskListControl()
		{
			InitializeComponent();


		}

		private void TaskList_Load(object sender, System.EventArgs e)
		{
			if (this.DesignMode)
				return;

			LoadInventory();

			BasilProject.Project.HackedEditorsSaveNow += new EventHandler(Project_HackedEditorsSaveNow);
		}

		void Project_HackedEditorsSaveNow(object sender, EventArgs e)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;

			XmlWriter writer = XmlWriter.Create(WeSayWordsProject.Project.PathToProjectTaskInventory, settings);
			writer.WriteStartDocument();
			writer.WriteStartElement("tasks");
			foreach (TaskInfo t in _taskList.Items)
			{
				if (_taskList.GetItemChecked(_taskList.Items.IndexOf(t)))
				{
					writer.WriteRaw(t.Node.OuterXml);
				}
			}
			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Close();
		}

		private void LoadInventory()
		{
			try
			{
				XmlDocument inventoryDoc = new XmlDocument();
				inventoryDoc.Load(Path.Combine(BasilProject.Project.ApplicationCommonDirectory, "taskInventory.xml"));
				XmlDocument projectDoc = null;
				if (File.Exists(WeSayWordsProject.Project.PathToProjectTaskInventory))
				{
					try
					{
						projectDoc = new XmlDocument();
						projectDoc.Load(WeSayWordsProject.Project.PathToProjectTaskInventory);
					}
					catch (Exception e)
					{
						MessageBox.Show("There was a problem reading the task xml. " + e.Message);
						projectDoc = null;
					}
				}

				//if there are no tasks, might as well be no document, so clear it out
				if(projectDoc != null && (null == projectDoc.SelectSingleNode("tasks/task")))
				{
					projectDoc = null;
				}

				foreach (XmlNode node in inventoryDoc.SelectNodes("tasks/task"))
				{
					TaskInfo task = new TaskInfo(node);
					XmlNode foundMatchingTask = null;
					bool showCheckMark;

					if (projectDoc == null)
					{
						XmlAttribute isDefault = node.Attributes["default"];
						showCheckMark = (isDefault != null) && isDefault.Value == "true";
					}
					else
					{
						foundMatchingTask = projectDoc.SelectSingleNode("tasks/task[@id='" + task.Id + "']");
						showCheckMark = foundMatchingTask != null;
					}
					this._taskList.Items.Add(task, showCheckMark);
				}
			}
			catch (Exception error)
			{
				MessageBox.Show("There may have been a problem reading the master task inventory xml. " + error.Message);
			}
		}
	}

	public class TaskInfo
	{
		private XmlNode _node;
		public TaskInfo(XmlNode node)
		{
			_node  = node;
		}

		public string Id
		{
			get
			{
				Debug.Assert(this.Node.Attributes["id"] != null,"Tasks must have ids.");
				return GetOptionalAttributeString(this.Node, "id", "task");
			}
		}

		public XmlNode Node
		{
			get { return this._node; }
		}

		public override string ToString()
		{
				XmlNode label = this.Node.SelectSingleNode("label");
				if (label != null)
				{
					return label.InnerText;
				}
				else
				{
					return GetOptionalAttributeString(this.Node, "id", "task");
				}

		}

		private string GetOptionalAttributeString(XmlNode xmlNode, string name, string defaultValue)
		{
			XmlAttribute attr = xmlNode.Attributes[name];
			if (attr == null)
				return defaultValue;
			return attr.Value;
		}

	}
}
