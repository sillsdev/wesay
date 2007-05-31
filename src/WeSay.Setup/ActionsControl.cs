using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Mono.Addins;
using WeSay.AddinLib;
using WeSay.Project;

namespace WeSay.Setup
{
	public partial class ActionsControl : UserControl
	{
		private bool _loaded=false;

		public ActionsControl()
		{
			InitializeComponent();
			this.Resize += new EventHandler(ActionsControl_Resize);
		}

		void ActionsControl_Resize(object sender, EventArgs e)
		{
			//this is part of dealing with .net not adjusting stuff well for different dpis

			this._addinsList.Width = this.Width - 20;
			this._addinsList.Height = (this.Height - _addinsList.Top) - 40;
			this.label1.Top = this.Bottom - (10+label1.Height);
		}

		private void OnVisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
			{
				if (!_loaded)
				{
					LoadAddins();
					_loaded = true;
				}
			   // UpdateStatesOfThings();
			}
		}

		private void LoadAddins()
		{
			_addinsList.Clear();
			if(!AddinManager.IsInitialized)
			{
				AddinManager.Initialize(Application.UserAppDataPath);
				AddinManager.Registry.Rebuild(null);
				AddinManager.Shutdown();
				AddinManager.Initialize(Application.UserAppDataPath);
				//these (at least AddinLoaded) does get called after initialize, when you
				//do a search for objects (e.g. GetExtensionObjects())
				AddinManager.AddinLoaded += new AddinEventHandler(AddinManager_AddinLoaded);
				AddinManager.AddinLoadError += new AddinErrorEventHandler(AddinManager_AddinLoadError);
				AddinManager.AddinUnloaded += new AddinEventHandler(AddinManager_AddinUnloaded);
				AddinManager.ExtensionChanged += new ExtensionEventHandler(AddinManager_ExtensionChanged);
			}

			foreach (IWeSayAddin addin in AddinManager.GetExtensionObjects(typeof(IWeSayAddin)))
			{
				AddAddin(addin);
			}
			AddAddin(new ComingSomedayAddin("Export To OpenOffice", ""));
			AddAddin(new ComingSomedayAddin("Export To Word", ""));
			AddAddin(new ComingSomedayAddin("Export To Lexique Pro", ""));
			AddAddin(
				new ComingSomedayAddin("Send project to developers", "Sends your project to WeSay for help/debugging."));
		}

		private void AddAddin(IWeSayAddin addin)
		{
			ActionItemControl control = new ActionItemControl(addin, true);
			control.DoShowInWeSay = AddinSet.Singleton.DoShowInWeSay(addin.ID);
			_addinsList.AddControlToBottom(control);
			control.Launch += new EventHandler(OnLaunchAction);
		}

		void OnLaunchAction(object sender, EventArgs e)
		{

			IWeSayAddin addin = sender as IWeSayAddin;

			WeSayWordsProject project = Project.WeSayWordsProject.Project;
			string[] filesBelongingToProject = WeSayWordsProject.GetFilesBelongingToProject(project.ProjectDirectoryPath);
			ProjectInfo projectInfo = new ProjectInfo(project.Name,
										 project.ProjectDirectoryPath,
										 project.PathToLiftFile,
										 filesBelongingToProject,
										 WeSay.AddinLib.AddinSet.Singleton.LocateFile);

			try
			{
				addin.Launch(this.ParentForm, projectInfo);
			}
			catch (Exception error)
			{
				Reporting.ErrorReporter.ReportNonFatalMessage(error.Message);
			}
		}

		void AddinManager_ExtensionChanged(object sender, ExtensionEventArgs args)
		{
			Reporting.Logger.WriteEvent("Addin 'extensionChanged': {0}",args.Path);
		}

		void AddinManager_AddinUnloaded(object sender, AddinEventArgs args)
		{
			Reporting.Logger.WriteEvent("Addin unloaded: {0}",args.AddinId);
		}

		void AddinManager_AddinLoadError(object sender, AddinErrorEventArgs args)
		{
			Reporting.Logger.WriteEvent("Addin load error: {0}", args.AddinId);
		}

		void AddinManager_AddinLoaded(object sender, AddinEventArgs args)
		{
			Reporting.Logger.WriteEvent("Addin loaded: {0}", args.AddinId);
		}

	}
}
