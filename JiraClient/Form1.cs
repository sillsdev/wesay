using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using JiraClient.org.wesay.www;

namespace JiraClient
{

	public partial class Form1 : Form
	{
		private JiraIssue _issue;

		public JiraIssue Issue
		{
			get { return _issue; }
			set { _issue = value; }
		}
		public Form1()
		{
			InitializeComponent();
			//propertyGrid1.s
		}



		private void Form1_Load(object sender, EventArgs e)
		{
			_issue = new JiraIssue();
		   this.propertyGrid1.SelectedObject = _issue;
			//propertyGrid1.PropertySort = PropertySort.Alphabetical;
		 }

		private void button1_Click(object sender, EventArgs e)
		{
			JiraSoapServiceService service = new JiraClient.org.wesay.www.JiraSoapServiceService();
			System.Diagnostics.Debug.WriteLine("Logging on...");
			string loginToken = service.login("x", "y");
			System.Diagnostics.Debug.WriteLine("Done.");

			_issue.summary = "Test from .net client";
			_issue = (JiraIssue) service.createIssue(loginToken, _issue as org.wesay.www.RemoteIssue);

			System.Diagnostics.Debug.WriteLine("Key: "+_issue.key);

		}
	}

	public class JiraIssue : JiraClient.org.wesay.www.RemoteIssue
	{
		public enum IssueTypes { bug = 1 };
		public JiraIssue()
			: base()
		{
			this.project = "WS";
			this.type = IssueTypes.bug;


		}

		[Category("Basic")]
		public new IssueTypes type
		{
			get { return (IssueTypes)Enum.Parse(typeof(IssueTypes), base.type); }
			set { base.type = Enum.GetName(typeof(IssueTypes), value); }
		}
	}
}