using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Chorus.sync;
using Chorus.UI.Sync;
using Chorus.Utilities;
using Chorus.VcsDrivers.Mercurial;
using Palaso.Reporting;
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
			if(!string.IsNullOrEmpty(HgRepository.GetEnvironmentReadinessMessage(localizationLanguageId)))
			{
				Palaso.Reporting.Logger.WriteEvent("Backup not possible: {0}", HgRepository.GetEnvironmentReadinessMessage("en"));
			}

			try
			{
				ProjectFolderConfiguration configuration = new WeSayChorusProjectConfiguration(pathToProjectDirectory);

				// projectFolder.IncludePatterns.Add(project.ProjectDirectoryPath);

//                  if (!string.IsNullOrEmpty(PathToParentOfRepositories))
//                {
//                    if (!Directory.Exists(PathToParentOfRepositories))
//                    {
//                        ErrorReport.NotifyUserOfProblem(new ShowOncePerSessionBasedOnExactMessagePolicy(), "There was a problem during auto backup: Could not Access the backup path, {0}", PathToParentOfRepositories);
//                        //no, we still want to check in... return;
//                    }
//                    else
//                    {
//                        var projectName = Path.GetFileName(pathToProjectDirectory);
//                        var backupSource = Chorus.VcsDrivers.RepositoryAddress.Create("backup", Path.Combine(PathToParentOfRepositories, projectName),
//                                                                                false);
//                        options.RepositorySourcesToTry.Add(backupSource);
//                    }
//                }

				using (var dlg = new SyncDialog(configuration,
					   SyncUIDialogBehaviors.StartImmediatelyAndCloseWhenFinished,
					   SyncUIFeatures.Minimal))
				{
					dlg.Text = "Wesay Automatic Backup";
					dlg.SyncOptions.DoMergeWithOthers = false;
					dlg.SyncOptions.DoPullFromOthers = false;
					dlg.SyncOptions.DoPushToLocalSources = true;
					dlg.SyncOptions.RepositorySourcesToTry.Clear();
					dlg.SyncOptions.CheckinDescription = CheckinDescriptionBuilder.GetDescription();

					//in addition to checking in, will we be doing a backup to another media (e.g. sd card)?
					if (!string.IsNullOrEmpty(PathToParentOfRepositories))
					{
							var projectName = Path.GetFileName(pathToProjectDirectory);
							var backupSource = Chorus.VcsDrivers.RepositoryAddress.Create("backupMedia", Path.Combine(PathToParentOfRepositories, projectName),
																					false);
							dlg.SyncOptions.RepositorySourcesToTry.Add(backupSource);
					}

					dlg.ShowDialog();

					if (dlg.FinalStatus.WarningEncountered ||  //not finding the backup media only counts as a warning
						dlg.FinalStatus.ErrorEncountered)
					{
						ErrorReport.NotifyUserOfProblem(new ShowOncePerSessionBasedOnExactMessagePolicy(),
														"There was a problem during auto backup:\r\n\r\n" +
														dlg.FinalStatus.WarningEncountered +"\r\n"+
														dlg.FinalStatus.ErrorEncountered);
					}
				}
				CheckinDescriptionBuilder.Clear();
			}
			catch (Exception error)
			{
				Palaso.Reporting.Logger.WriteEvent("Error during Backup: {0}", error.Message);
				//TODO we need some passive way indicating the health of the backup system
			}
		}


		public void ResetTimeOfLastBackup()
		{
			_timeOfLastBackupAttempt = DateTime.Now;
		}
	}

	//todo: move to chorus
}
