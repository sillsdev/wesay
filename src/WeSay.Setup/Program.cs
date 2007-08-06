using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Palaso.Reporting;


namespace WeSay.Setup
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);


			SetupErrorHandling();
			Palaso.Reporting.Logger.Init();

			//bring in settings from any previous version
			if (Properties.Settings.Default.NeedUpgrade)
			{
				Properties.Settings.Default.Upgrade();
				Properties.Settings.Default.NeedUpgrade = false;
			}

			UsageReporter.AppNameToUseInDialogs = "WeSay";
			UsageReporter.AppNameToUseInReporting = "WeSayConfig";
			Palaso.Reporting.UsageReporter.RecordLaunch();
			Palaso.Reporting.UsageReporter.DoTrivialUsageReport("usage@wesay.org", "Thank you for helping us test WeSay!", new int[] { 1, 5, 20, 40, 60, 80, 100 });

			Application.Run(new AdminWindow(args));

			Palaso.Reporting.Logger.WriteEvent("App Exiting Normally.");
			Palaso.Reporting.Logger.ShutDown();
		}

		private static void SetupErrorHandling()
		{
			 Palaso.Reporting.ErrorReport.EmailAddress = "issues@wesay.org";
			 Palaso.Reporting.ErrorReport.AddStandardProperties();
			 Palaso.Reporting.ExceptionHandler.Init();
		}
	}
}