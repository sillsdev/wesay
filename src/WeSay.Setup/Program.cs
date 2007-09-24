using System;
using System.Windows.Forms;
using Palaso.Reporting;
using WeSay.Setup.Properties;

namespace WeSay.Setup
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
			}

			UsageReporter.AppNameToUseInDialogs = "WeSay";
			UsageReporter.AppNameToUseInReporting = "WeSayConfig";
			UsageReporter.RecordLaunch();
			UsageReporter.DoTrivialUsageReport("usage@wesay.org",
											   "Thank you for helping us test WeSay!",
											   new int[] {1, 5, 20, 40, 60, 80, 100});

			Application.Run(new AdminWindow(args));

			Logger.WriteEvent("App Exiting Normally.");
			Logger.ShutDown();
		}

		private static void SetupErrorHandling()
		{
			ErrorReport.EmailAddress = "issues@wesay.org";
			ErrorReport.AddStandardProperties();
			ExceptionHandler.Init();
		}
	}
}