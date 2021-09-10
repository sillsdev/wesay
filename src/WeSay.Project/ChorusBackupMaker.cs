using Chorus.FileTypeHandlers.lift;
using Chorus.sync;
using Chorus.UI.Sync;
using Chorus.VcsDrivers.Mercurial;
using SIL.Reporting;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
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


		/// <returns>null if not found in the dom</returns>
		public static ChorusBackupMaker CreateFromDom(XmlDocument dom, CheckinDescriptionBuilder checkinDescriptionBuilder)
		{
			var node = dom.SelectSingleNode("//backupPlan");
			if (node == null)
				return null;
			using (var reader = new StringReader(node.OuterXml))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(ChorusBackupMaker));
				var backupMaker = (ChorusBackupMaker)serializer.Deserialize(reader);
				backupMaker.CheckinDescriptionBuilder = checkinDescriptionBuilder;
				return backupMaker;
			}
		}

		public void Save(XmlWriter writer)
		{
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add("", "");//don't add the silly namespace on the element
			XmlSerializer serializer = new XmlSerializer(typeof(ChorusBackupMaker));
			serializer.Serialize(writer, this, ns);
		}

		public void BackupNow(string pathToProjectDirectory, string localizationLanguageId, string pathToLiftFile)
		{
			if (pathToProjectDirectory.ToLower().IndexOf(@"sampleprojects\pretend") >= 0)
			{
				return; //no way... if you want a unit test that includes CHorus, do it without
						//that now deprecated monstrosity.
			}
			_timeOfLastBackupAttempt = DateTime.Now;

			//nb: we're not really using the message yet, at least, not showing it to the user
			if (!string.IsNullOrEmpty(HgRepository.GetEnvironmentReadinessMessage(localizationLanguageId)))
			{
				SIL.Reporting.Logger.WriteEvent("Backup not possible: {0}", HgRepository.GetEnvironmentReadinessMessage("en"));
			}

			try
			{
				var configuration = new ProjectFolderConfiguration(pathToProjectDirectory);
				LiftFolder.AddLiftFileInfoToFolderConfiguration(configuration);

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
					dlg.Text = "WeSay Automatic Backup";
					dlg.SyncOptions.DoMergeWithOthers = false;
					dlg.SyncOptions.DoPullFromOthers = false;
					dlg.SyncOptions.DoSendToOthers = true;
					dlg.SyncOptions.RepositorySourcesToTry.Clear();
					dlg.SyncOptions.CheckinDescription = CheckinDescriptionBuilder.GetDescription();
					dlg.UseTargetsAsSpecifiedInSyncOptions = true;
					dlg.SetSynchronizerAdjunct(new LiftSynchronizerAdjunct(pathToLiftFile));

					//in addition to checking in, will we be doing a backup to another media (e.g. sd card)?
					if (!string.IsNullOrEmpty(PathToParentOfRepositories))
					{
						var projectName = Path.GetFileName(pathToProjectDirectory);
						var backupSource = Chorus.VcsDrivers.RepositoryAddress.Create("test-backup-media", Path.Combine(PathToParentOfRepositories, projectName),
																				false);
						dlg.SyncOptions.RepositorySourcesToTry.Add(backupSource);
					}

					dlg.ShowDialog();

					if (dlg.FinalStatus.WarningEncountered ||  //not finding the backup media only counts as a warning
						dlg.FinalStatus.ErrorEncountered)
					{
						ErrorReport.NotifyUserOfProblem(new ShowOncePerSessionBasedOnExactMessagePolicy(),
														"There was a problem during auto backup. Chorus said:\r\n\r\n" +
														dlg.FinalStatus.LastWarning + "\r\n" +
														dlg.FinalStatus.LastError);
					}
				}
				CheckinDescriptionBuilder.Clear();
			}
			catch (Exception error)
			{
				SIL.Reporting.Logger.WriteEvent("Error during Backup: {0}", error.Message);
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
