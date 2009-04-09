using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Chorus.sync;
using Chorus.Utilities;
using Palaso.UI.WindowsForms.i8n;
using WeSay.LexicalModel;

namespace WeSay.Project
{
	/// <summary>
	/// Uses chorus to regularly backup to a second drive (SD, SSD, etc.)
	/// </summary>
	[XmlRoot("backupPlan")]
	public class ChorusBackupMaker
	{
		internal CheckinDescriptionBuilder CheckinDescriptionBuilder { get; set; }
		public const string ElementName = "backupPlan";

	  public ChorusBackupMaker(CheckinDescriptionBuilder checkinDescriptionBuilder)
	  {
		  CheckinDescriptionBuilder = checkinDescriptionBuilder;
	  }


		/// <summary>
		/// for deserializer
		/// </summary>
	  internal ChorusBackupMaker()
	  {
	  }

		[XmlElement("pathToParentOfRepositories")]
		public string PathToParentOfRepositories;

		private DateTime _timeOfLastBackupAttempt;
		private LexEntryRepository _lexEntryRepository;

		public DateTime TimeOfLastBackupAttempt
		{
			get { return _timeOfLastBackupAttempt; }
		}

		public LexEntryRepository Repository
		{
			set { _lexEntryRepository = value; }
		}

		public static ChorusBackupMaker LoadFromReader(XmlReader reader, CheckinDescriptionBuilder checkinDescriptionBuilder)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ChorusBackupMaker));
			var x = (ChorusBackupMaker)serializer.Deserialize(reader);
			x.CheckinDescriptionBuilder = checkinDescriptionBuilder;
			return x;
		}

		public void Save(XmlWriter writer)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ChorusBackupMaker));
			serializer.Serialize(writer, this);
		}

		public void BackupNow(string pathToProjectDirectory, string localizationLanguageId)
		{
			if(pathToProjectDirectory.ToLower().IndexOf(@"sampleprojects\pretend")>=0)
			{
				return; //no way... if you want a unit test that includes CHorus, do it without
						//that no deprecated monstrosity.
			}
#if DEBUG
			Debug.Assert(pathToProjectDirectory.ToLower().IndexOf("wesaydev") < 0, "Whoops, something is trying to do a checkin of the wesay code!");
#endif
			_timeOfLastBackupAttempt = DateTime.Now;

			//nb: we're not really using the message yet, at least, not showing it to the user
			if(!string.IsNullOrEmpty(RepositoryManager.GetEnvironmentReadinessMessage(localizationLanguageId)))
			{
				Palaso.Reporting.Logger.WriteEvent("Backup not possible: {0}", RepositoryManager.GetEnvironmentReadinessMessage("en"));
			}

			LiftRepository.RightToAccessLiftExternally rightToAccessLiftExternally = null;
			if (_lexEntryRepository != null)
			{
				rightToAccessLiftExternally = _lexEntryRepository.GetRightToAccessLiftExternally();
			}

			try
			{
				ProjectFolderConfiguration projectFolder = new ProjectFolderConfiguration(pathToProjectDirectory);
				projectFolder.ExcludePatterns.Add("**/cache");
				projectFolder.ExcludePatterns.Add("**/Cache");
				projectFolder.ExcludePatterns.Add("*.old");
				projectFolder.ExcludePatterns.Add("*.tmp");
				projectFolder.ExcludePatterns.Add("*.bak");
				projectFolder.IncludePatterns.Add("audio/*.*");
				projectFolder.IncludePatterns.Add("pictures/*.*");
				projectFolder.IncludePatterns.Add("export/*.css"); //stylesheets
				projectFolder.IncludePatterns.Add("export/*.lpconfig");//lexique pro
				projectFolder.IncludePatterns.Add("*.*");
				projectFolder.IncludePatterns.Add(".hgIgnore");
			   // projectFolder.IncludePatterns.Add(project.ProjectDirectoryPath);

				Chorus.sync.SyncOptions options = new SyncOptions();
				options.DoMergeWithOthers = false;
				options.DoPullFromOthers = false;
				options.DoPushToLocalSources = true;
				options.RepositorySourcesToTry.Clear();
				if (!string.IsNullOrEmpty(PathToParentOfRepositories))
				{
					if (!Directory.Exists(PathToParentOfRepositories))
					{
						Palaso.Reporting.NonFatalErrorDialog.Show(string.Format("Could not Access the backup path, {0}", PathToParentOfRepositories));
					}
					else
					{
						RepositorySource backupSource = RepositorySource.Create(PathToParentOfRepositories, "backup",
																				false);
						options.RepositorySourcesToTry.Add(backupSource);
					}
				}
				options.CheckinDescription = CheckinDescriptionBuilder.GetDescription();

				RepositoryManager manager = RepositoryManager.FromRootOrChildFolder(projectFolder);


				if (!RepositoryManager.CheckEnvironmentAndShowMessageIfAppropriate("en"))//todo localization
				{
					Palaso.Reporting.Logger.WriteEvent("Backup not possible: {0}", RepositoryManager.GetEnvironmentReadinessMessage("en"));
					return;
				}


				//TODO: figure out how/what/when to show progress. THis is basically just throwing it away
				IProgress progress = new Chorus.Utilities.StringBuilderProgress();
				manager.SyncNow(options, progress);

				CheckinDescriptionBuilder.Clear();
			}
			catch (Exception error)
			{
				Palaso.Reporting.Logger.WriteEvent("Error during Backup: {0}", error.Message);
				//TODO we need some passive way indicating the health of the backup system
			}
			finally
			{
				if(rightToAccessLiftExternally !=null)
				{
					rightToAccessLiftExternally.Dispose();
				}
			}
		}
	}

	//todo: move to chorus
}
