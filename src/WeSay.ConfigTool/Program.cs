using System;
using System.Windows.Forms;
using Palaso.Reporting;
using WeSay.ConfigTool.Properties;
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
			}

			UsageReporter.AppNameToUseInDialogs = "WeSay Configuration Tool";
			UsageReporter.AppNameToUseInReporting = "WeSayConfig";
			UsageReporter.RecordLaunch();
			UsageReporter.DoTrivialUsageReport("usage@wesay.org",
											   "Thank you for letting us know you are using WeSay.",
											   new int[] {1, 5, 20, 40, 60, 80, 100});

			Application.Run(new ConfigurationWindow(args));

#if !DEBUG
			try
			{
#endif
			Logger.WriteEvent("App Exiting Normally.");
				Logger.ShutDown();
#if !DEBUG
			}
			catch (Exception err)
			{
			 // we don't know what caused ws-596, but it isn't worth crashing over
			}
#endif
			}

		private static void SetupErrorHandling()
		{
			ErrorReport.EmailAddress = "issues@wesay.org";
			ErrorReport.AddStandardProperties();
			ExceptionHandler.Init();
		}
	}
}