using System;
using System.Windows.Forms;
using Palaso.Reporting;
using WeSay.ConfigTool.Properties;

namespace WeSay.ConfigTool
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			SetupErrorHandling();
			Logger.Init();

			//bring in settings from any previous version
			if (Settings.Default.NeedUpgrade)
			{
				Settings.Default.Upgrade();
				Settings.Default.NeedUpgrade = false;
				Settings.Default.Save();
			}

			UsageReporter.AppReportingSettings = Settings.Default.Reporting;
			UsageReporter.AppNameToUseInDialogs = "WeSay Configuration Tool";
			UsageReporter.AppNameToUseInReporting = "WeSayConfig";
			UsageReporter.RecordLaunch();
			Settings.Default.Save();
			UsageReporter.DoTrivialUsageReport("usage@wesay.org",
											   "Thank you for letting us know you are using WeSay.",
											   new int[] {1, 5, 20, 40, 60, 80, 100});

			Application.Run(new ConfigurationWindow(args));
		}

		private static void SetupErrorHandling()
		{
			ErrorReport.EmailAddress = "issues@wesay.org";
			ErrorReport.AddStandardProperties();
			ExceptionHandler.Init();
		}
	}
}