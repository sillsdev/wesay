using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Palaso.IO;
using Palaso.Reporting;
using WeSay.ConfigTool.Properties;
#if __MonoCS__
using NDesk.DBus;
#endif

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
			SetUpReporting();

			Settings.Default.Save();
			try
			{
				// initialize Palaso keyboarding
				Palaso.UI.WindowsForms.Keyboarding.KeyboardController.Initialize();
				Application.Run(new ConfigurationWindow(args));
			}
			finally
			{
#if __MonoCS__
				// Chorus backup results in NDesk spinning up a thread that
				// continues until NDesk Bus is closed.  Failure to close the
				// thread results in a hang when closing.
				Bus.System.Close();
#endif
				Palaso.UI.WindowsForms.Keyboarding.KeyboardController.Shutdown();
			}
		}

		private static void SetUpReporting()
		{
			if (Settings.Default.Reporting == null)
			{
				Settings.Default.Reporting = new ReportingSettings();
				Settings.Default.Save();
			}
			UsageReporter.Init(Settings.Default.Reporting, "wesay.palaso.org", "UA-22170471-6");
			UsageReporter.AppNameToUseInDialogs = "WeSay Configuration Tool";
			UsageReporter.AppNameToUseInReporting = "WeSayConfig";
		}

		private static void SetupErrorHandling()
		{
			ErrorReport.EmailAddress = "issues@wesay.org";
			ErrorReport.AddStandardProperties();
			ExceptionHandler.Init();
		}

		public static void ShowHelpTopic(string topicLink)
		{
			string helpFilePath = FileLocator.GetFileDistributedWithApplication("WeSay_Helps.chm");
			if (File.Exists(helpFilePath))
			{
				//var uri = new Uri(helpFilePath);
				Help.ShowHelp(new Label(), helpFilePath, topicLink);
			}
			else
			{
				Process.Start("http://wesay.palaso.org/help/");
			}
			UsageReporter.SendNavigationNotice("Help: " + topicLink);
		}

	}
}