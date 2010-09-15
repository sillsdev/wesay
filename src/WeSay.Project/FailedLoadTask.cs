using System;
using System.Drawing;
using System.Windows.Forms;
using Palaso.I8N;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;

namespace WeSay.Project
{
	public class FailedLoadTask: ITask
	{
		private readonly string _label;
		private readonly string _longLabel;
		private readonly string _description;

		public FailedLoadTask(string label, string longLabel, string description)
		{
			_label = label;
			_longLabel = longLabel;
			_description = description;
		}

		public void Activate() {}

		public void Deactivate() {}

		#region ITask Members

		public void GoToUrl(string url)
		{
			throw new NotImplementedException();
		}

		#endregion

		public bool IsActive
		{
			get { return false; }
		}

		public string Label
		{
			get { return StringCatalog.GetFormatted("~Failed To Load: {0}", "", _label); }
		}

		public bool Available
		{
			get { return true; }
		}

		public string Description
		{
			get { return StringCatalog.GetFormatted("~Error: {0}", "", _description); }
		}

		public Control Control
		{
			get
			{
				TextBox t = new TextBox();
				t.Multiline = true;
				t.Dock = DockStyle.Fill;
				t.Text =
						String.Format(
								"Could not load the task '{0}'. Possibly, the setup in the admin program can be used to fix this.  The error was: [{1}]",
								_label,
								_description);
				return t;
			}
		}

		public bool IsPinned
		{
			get { return false; }
		}

		private const int CountNotApplicable = -1;

		public int GetRemainingCount()
		{
			return CountNotApplicable;
		}

		public int ExactCount
		{
			get { return CountNotApplicable; }
		}

		/// <summary>
		/// Gives a sense of the overall size of the task versus what's left to do
		/// </summary>
		public int GetReferenceCount()
		{
			return 0;
		}

		public bool AreCountsRelevant()
		{
			return false;
		}

		public string GetRemainingCountText()
		{
			return string.Empty;
		}

		public string GetReferenceCountText()
		{
			return string.Empty;
		}

		#region IThingOnDashboard Members

		public DashboardGroup Group
		{
			get { return DashboardGroup.Describe; }
		}

		public string LocalizedLabel
		{
			get { return StringCatalog.Get(Label); }
		}

		public string LocalizedLongLabel
		{
			get { return StringCatalog.Get(_longLabel); }
		}

		public ButtonStyle DashboardButtonStyle
		{
			get { return ButtonStyle.FixedAmount; }
		}

		public Image DashboardButtonImage
		{
			get { return null; }
		}

		#endregion
	}
}