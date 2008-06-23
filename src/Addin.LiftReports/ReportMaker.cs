using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Addin.LiftReports.Properties;
using Mono.Addins;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;
using WeSay.Foundation;

namespace Addin.LiftReports
{
	[Extension]
	public class ReportMaker : IWeSayAddin
	{
		protected bool _launchAfterTransform = true;
		private string _pathToOutput;


		public Image ButtonImage
		{
			get
			{
				return Resources.image;
			}
		}

		public bool Available
		{
			get
			{
				return true;
			}
		}

		public string LocalizedName
		{
			get
			{
				return StringCatalog.Get("~View Report");
			}
		}

		public string Description
		{
			get
			{
				return StringCatalog.Get("~Shows some information about the lexicon.");
			}
		}

		//for unit tests
		public string PathToOutput
		{
			get
			{
				return _pathToOutput;
			}
		}

		//for unit tests
		public bool LaunchAfterTransform
		{
			set
			{
				_launchAfterTransform = value;
			}
		}

		#region IWeSayAddin Members

		public object SettingsToPersist
		{
			get
			{
				return null;
			}
			set
			{

			}
		}

		public string ID
		{
			get
			{
				return "ReportMaker";
			}
		}

		#endregion

		#region IThingOnDashboard Members

		public WeSay.Foundation.Dashboard.DashboardGroup Group
		{
			get { return WeSay.Foundation.Dashboard.DashboardGroup.Share; }
		}

		public string LocalizedLabel
		{
			get { return this.LocalizedName; }
		}

		public WeSay.Foundation.Dashboard.ButtonStyle DashboardButtonStyle
		{
			get { return WeSay.Foundation.Dashboard.ButtonStyle.IconVariableWidth; }
		}

		public Image DashboardButtonImage
		{
			get { return null; }
		}

		#endregion

		public void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			HtmlReport r = new HtmlReport();
			_pathToOutput = r.GenerateReport(projectInfo.PathToLIFT);
			if (_launchAfterTransform)
			{
				Process.Start(_pathToOutput);
			}
		}
	}
}
