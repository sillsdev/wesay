using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mono.Addins;
using Palaso.Reporting;
using WeSay.AddinLib;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.ConfigTool
{
	public partial class ActionsControl: ConfigurationControlBase
	{
		public ActionsControl(ILogger logger)
			: base("setup and use plug-in actions", logger)
		{
			InitializeComponent();
		}

		//private void ActionsControl_Resize(object sender, EventArgs e)
		//{
		//    //this is part of dealing with .net not adjusting stuff well for different dpis

		//    _addinsList.Width = Width - 20;
		//    _addinsList.Height = (Height - _addinsList.Top) - 40;
		//}

		private void LoadAddins()
		{
			List<string> alreadyFound = new List<string>();

			_addinsList.SuspendLayout();
			_addinsList.Controls.Clear();
			_addinsList.RowStyles.Clear();
			if (!AddinManager.IsInitialized)
			{
				try
				{
					TryToInitializeMonoAddins();
				}
				catch
				{
					Palaso.Reporting.ErrorReport.NotifyUserOfProblem("Sorry, something went wrong collecting up the available addins.  Please go to this folder:\r\n"+ Application.UserAppDataPath +"\r\nand delete any folders which begin with \"addin-db\", then quit this program and try again.");
					return;
				}

			}

			foreach (IWeSayAddin addin in
					AddinManager.GetExtensionObjects(typeof (IWeSayAddin)))
			{
				//this alreadyFound business is a hack to prevent duplication in some
				// situation I haven't tracked down yet.
				if (!alreadyFound.Contains(addin.ID))
				{
					alreadyFound.Add(addin.ID);
					AddAddin(addin);
				}
			}
			//            AddAddin(new ComingSomedayAddin("Export To OpenOffice", ""));
			//            AddAddin(new ComingSomedayAddin("Export To Lexique Pro", ""));
			//            AddAddin(
			//                    new ComingSomedayAddin("Send project to developers",
			//                                           "Sends your project to WeSay for help/debugging."));
			_addinsList.ResumeLayout();
		}

		private void TryToInitializeMonoAddins()
		{
			AddinManager.Initialize(Application.UserAppDataPath);
			AddinManager.Registry.Rebuild(null);
			AddinManager.Shutdown();
			AddinManager.Initialize(Application.UserAppDataPath);
			//these (at least AddinLoaded) does get called after initialize, when you
			//do a search for objects (e.g. GetExtensionObjects())
			AddinManager.AddinLoaded += AddinManager_AddinLoaded;
			AddinManager.AddinLoadError += AddinManager_AddinLoadError;
			AddinManager.AddinUnloaded += AddinManager_AddinUnloaded;
			AddinManager.ExtensionChanged += AddinManager_ExtensionChanged;
		}

		private void AddAddin(IWeSayAddin addin)
		{
			ActionItemControl control = new ActionItemControl(addin,
															  true,
															  WeSayWordsProject.Project.
																	  GetProjectInfoForAddin());
			// GetProjectInfo());
			control.DoShowInWeSay = AddinSet.Singleton.DoShowInWeSay(addin.ID);
			control.TabIndex = _addinsList.RowCount;
			control.Launch += OnLaunchAction;

			_addinsList.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			_addinsList.Controls.Add(control);
		}

		private void OnLaunchAction(object sender, EventArgs e)
		{
			IWeSayAddin addin = (IWeSayAddin) sender;

			try
			{
				addin.Launch(ParentForm,

					//working on fixing ws-1026, found that creating this repository just locked the
					//file, and no existing actions appear to need it anyhow
			 WeSayWordsProject.Project.GetProjectInfoForAddin());

//                using (
//                        LexEntryRepository lexEntryRepository =
//                                new LexEntryRepository(WeSayWordsProject.Project.PathToRepository))
//                {
//                    addin.Launch(ParentForm,
//                                 WeSayWordsProject.Project.GetProjectInfoForAddin(lexEntryRepository));
//                }
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem(error.Message);
			}
		}

		private static void AddinManager_ExtensionChanged(object sender, ExtensionEventArgs args)
		{
			Logger.WriteEvent("Addin 'extensionChanged': {0}", args.Path);
		}

		private static void AddinManager_AddinUnloaded(object sender, AddinEventArgs args)
		{
			Logger.WriteEvent("Addin unloaded: {0}", args.AddinId);
		}

		private static void AddinManager_AddinLoadError(object sender, AddinErrorEventArgs args)
		{
			Logger.WriteEvent("Addin load error: {0}", args.AddinId);
		}

		private static void AddinManager_AddinLoaded(object sender, AddinEventArgs args)
		{
			Logger.WriteEvent("Addin loaded: {0}", args.AddinId);
		}

		private void ActionsControl_Load(object sender, EventArgs e)
		{
			LoadAddins();
		}
	}
}