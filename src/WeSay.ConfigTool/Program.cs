using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Palaso.IO;
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
			SetUpReporting();

			Settings.Default.Save();

			using (new Palaso.PalasoSetup())
			{
				try
				{
					// initialize Palaso keyboarding
					Palaso.UI.WindowsForms.Keyboarding.KeyboardController.Initialize();
					Application.Run(new ConfigurationWindow(args));
				}
				finally
				{
					Palaso.UI.WindowsForms.Keyboarding.KeyboardController.Shutdown();
				}
			}
		}

		private static void SetUpReporting()
		{
			if (Settings.Default.Reporting == null)
			{
				Settings.Default.Reporting = new ReportingSettings();
				Settings.Default.Save();
			}
			// If this is a release build, then allow the environment variable to be set to true
			// so that testers are not generating user stats
			string developerSetting = System.Environment.GetEnvironmentVariable("WESAY_TRACK_AS_DEVELOPER");
			bool developerTracking = !string.IsNullOrEmpty(developerSetting) && (developerSetting.ToLower() == "yes" || developerSetting.ToLower() == "true" || developerSetting == "1");
			bool reportAsDeveloper =
#if DEBUG
 true
#else
 developerTracking
#endif
;
			UsageReporter.Init(Settings.Default.Reporting, "wesay.palaso.org", "UA-22170471-6", reportAsDeveloper);
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
			string helpFilePath = FileLocator.GetFileDistributedWithApplication(true, "WeSay_Helps.chm");
			if (String.IsNullOrEmpty(helpFilePath))
			{
				string commonDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
				helpFilePath = Path.Combine(commonDataFolder, Path.Combine("wesay", "WeSay_Helps.chm"));
			}
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
