using System;
using System.Drawing;
using System.Windows.Forms;
using Chorus.UI.Sync;
using Chorus.VcsDrivers;
using Chorus.VcsDrivers.Mercurial;
using Mono.Addins;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;
using WeSay.Foundation;
using WeSay.Project;

namespace Addin.Backup
{
	[Extension]
	public class SynchronizeAction : IWeSayAddin, IWeSayAddinHasSettings
	{
		private SynchronizeSettings _settings;

		public SynchronizeAction()
		{
			_settings = new SynchronizeSettings();
		}

		#region IWeSayAddin Members

		public Image ButtonImage
		{
			get { return Resources.greenSynchronize; }
		}

		public bool Available
		{
			get { return string.IsNullOrEmpty(HgRepository.GetEnvironmentReadinessMessage("en")); }
		}

		public string LocalizedName
		{
			get
			{
				return StringCatalog.Get("~Send/Recieve",
										 "Label for action which uses Chorus to synchronize the dictionary with other users, devices, and servers. Here, 'synchronizing' means putting your changes in the repository, and getting changes that others have put in the repository. In English, we are going with Send/Receive because it is similar to email and this is the term Paratext uses.");
			}
		}

		public string Description
		{
			get
			{
				return
						StringCatalog.Get(
								"~Sends your changes to usb key, server, or other users, and receives the changes made by other members of your team.",
								"description of synchronize action");
			}
		}

		public string ID
		{
			get { return "SynchronizeAction"; }
			set { throw new NotImplementedException(); }
		}

		#region IWeSayAddinHasSettings Members

		public bool DoShowSettingsDialog(Form parentForm, ProjectInfo projectInfo)
		{
			return false;
		}

		public object Settings
		{
			get { return _settings; }
			set { _settings = value as SynchronizeSettings; }
		}

		#endregion

		public void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			var configuration = (Chorus.sync.ProjectFolderConfiguration)
								projectInfo.ServiceProvider.GetService(typeof (Chorus.sync.ProjectFolderConfiguration));
			using(var dlg = new SyncDialog(configuration,
					SyncUIDialogBehaviors.Lazy,
					SyncUIFeatures.Everything))
			{
				dlg.Text = "Wesay Send/Receive";
				dlg.SyncOptions.DoMergeWithOthers = true;
				dlg.SyncOptions.DoPullFromOthers = true;
				dlg.SyncOptions.DoPushToLocalSources = true;


			   // leave it with the default, for now... dlg.SyncOptions.RepositorySourcesToTry.Clear();
				//dlg.SyncOptions.CheckinDescription = CheckinDescriptionBuilder.GetDescription();

				dlg.ShowDialog(parentForm);

				if (dlg.SyncResult.DidGetChangesFromOthers)
				{
					Application.Restart();
				}
			}

		}

		#endregion

		#region IThingOnDashboard Members

		public DashboardGroup Group
		{
			get { return DashboardGroup.Share; }
		}

		public string LocalizedLabel
		{
			get { return LocalizedName; }
		}

		public string LocalizedLongLabel
		{
			get
			{
				return StringCatalog.Get("~Send/Recieve Changes to devices, servers, or other users on your network.",
										 "Long label for synchronize action.");
			}
		}

		public ButtonStyle DashboardButtonStyle
		{
			get { return ButtonStyle.IconVariableWidth; }
		}

		public Image DashboardButtonImage
		{
			get { return Resources.greenSynchronize; }
		}

		#endregion

	}

	[Serializable]
	public class SynchronizeSettings
	{
//        private string _pathToExecutable = @"$wesayApplicationDirectory\mercurial.bat";
//        private string _arguments = @"$projectPath";
//
//        public string PathToExecutable
//        {
//            get { return _pathToExecutable; }
//            set { _pathToExecutable = value; }
//        }
//
//        public string Arguments
//        {
//            get { return _arguments; }
//            set { _arguments = value; }
//        }
//
//        public string GetRuntimeProcessPath(ProjectInfo info)
//        {
//            return _pathToExecutable.Replace("$wesayApplicationDirectory",
//                                             info.PathToApplicationRootDirectory);
//        }
//
//        public string GetRuntimeArguments(ProjectInfo info)
//        {
//            return _arguments.Replace("$projectPath", info.PathToTopLevelDirectory);
//        }
	}
}