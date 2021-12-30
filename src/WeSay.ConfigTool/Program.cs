using SIL.IO;
using SIL.Reporting;
using SIL.Windows.Forms.Reporting;
using Gecko;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
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
			Icu.Wrapper.Init();

			SetUpXulRunner();

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
				SIL.Windows.Forms.Keyboarding.KeyboardController.Initialize();
				SIL.WritingSystems.Sldr.Initialize();
				Application.Run(new ConfigurationWindow(args));
			}
			finally
			{
				SIL.Windows.Forms.Keyboarding.KeyboardController.Shutdown();
				SIL.WritingSystems.Sldr.Cleanup();
				Icu.Wrapper.Cleanup();
			}
		}

		public static void SetUpXulRunner()
		{
			try
			{
				if (WeSay.UI.Platform.IsLinux)
				{
					string initXulRunnerOption = Environment.GetEnvironmentVariable("WESAY_INIT_XULRUNNER") ?? String.Empty;
					// Initialize XULRunner - required to use the geckofx WebBrowser Control (GeckoWebBrowser).
					string xulRunnerLocation = XULRunnerLocator.GetXULRunnerLocation();
					if (String.IsNullOrEmpty(xulRunnerLocation))
						throw new ApplicationException("The XULRunner library is missing or has the wrong version");
					string librarySearchPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH") ?? String.Empty;
					if (!librarySearchPath.Contains(xulRunnerLocation))
						throw new ApplicationException("LD_LIBRARY_PATH must contain " + xulRunnerLocation);

					Xpcom.Initialize(xulRunnerLocation);
					GeckoPreferences.User["gfx.font_rendering.graphite.enabled"] = true;
				}
			}
			catch (ApplicationException e)
			{
				ErrorReport.NotifyUserOfProblem(e.Message);
			}
			catch (Exception e)
			{
				ErrorReport.NotifyUserOfProblem(e.Message);
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
			ExceptionHandler.Init(new WinFormsExceptionHandler());
		}

		public static void ShowHelpTopic(string topicLink)
		{
			string helpFilePath = FileLocationUtilities.GetFileDistributedWithApplication(true, "WeSay_Helps.chm");
			if (String.IsNullOrEmpty(helpFilePath))
			{
				string commonDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
				helpFilePath = Path.Combine(commonDataFolder, Path.Combine(Project.BasilProject.WeSaySharedDirectory, "WeSay_Helps.chm"));
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
