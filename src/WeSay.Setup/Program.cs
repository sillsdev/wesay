using System;
using System.Diagnostics;
using System.Windows.Forms;


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
			Reporting.Logger.Init();

			//bring in settings from any previous version
			if (Properties.Settings.Default.NeedUpgrade)
			{
				Properties.Settings.Default.Upgrade();
				Properties.Settings.Default.NeedUpgrade = false;
			}

			Reporting.UsageEmailDialog.IncrementLaunchCount();
			Reporting.UsageEmailDialog.DoTrivialUsageReport("issues@wesay.org", "Thank you for helping us test WeSay!", new int[] { 1, 5, 20, 40, 60, 80, 100 });

			Application.Run(new AdminWindow(args));

			Reporting.Logger.WriteEvent("App Exiting Normally.");
			Reporting.Logger.ShutDown();
		}

		private static void SetupErrorHandling()
		{
			 Reporting.ErrorReporter.EmailAddress = "issues@wesay.org";
			 Reporting.ErrorReporter.AddStandardProperties();
			 Reporting.ExceptionHandler.Init();
		}
	}
}