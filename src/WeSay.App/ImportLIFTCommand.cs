using System;
using System.IO;
using System.Xml;
using LiftIO;
using WeSay.App;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Progress;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay
{
	public class ImportLIFTCommand : BasicCommand
	{
		protected string _destinationDatabasePath;
		protected string _sourceLIFTPath;
		//private LiftImporter _importer;
		protected WeSay.Foundation.Progress.ProgressState _progress;
		private WeSay.Data.Db4oRecordList<LexEntry> _prewiredEntries=null;

		public ImportLIFTCommand(string destinationDatabasePath, string sourceLIFTPath)
		{
			_destinationDatabasePath = destinationDatabasePath;
			_sourceLIFTPath = sourceLIFTPath;
		}

		/// <summary>
		/// caller can set this if you want some actions to happen as each entry is
		/// imported, such as building up indices.  The given entries list will be
		/// used, and you can act on the events it fires.
		/// </summary>
		public Db4oRecordList<LexEntry> EntriesAlreadyWiredUp
		{
			set
			{
				_prewiredEntries = value;
			}
		}

		public Db4oRecordList<LexEntry> GetEntries(Db4oDataSource ds)
		{

			if (_prewiredEntries == null)
			{
				return new Db4oRecordList<LexEntry>(ds);
			}

			return _prewiredEntries;
		}

		protected override void DoWork2(ProgressState progress)
		{
			_progress = progress;
			_progress.Status = ProgressState.StatusValue.Busy;
			_progress.StatusLabel = "Validating...";
			string errors = Validator.GetAnyValidationErrors(_sourceLIFTPath);
			if (errors != null && errors != string.Empty)
			{
				progress.StatusLabel = "Problem with file format...";
				_progress.Status = ProgressState.StatusValue.StoppedWithError;
				_progress.WriteToLog(errors);
				return;
			}
			try
			{
				progress.StatusLabel = "Importing...";
				string tempTarget = Path.GetTempFileName();

				using (IRecordListManager recordListManager =
					new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), tempTarget))
				{
					Db4oDataSource ds = ((Db4oRecordListManager) recordListManager).DataSource;
					Db4oLexModelHelper.Initialize(ds.Data);
					//  Db4oRecordListManager ds = recordListManager as Db4oRecordListManager;

					//MONO bug as of 1.1.18 cannot bitwise or FileShare on FileStream constructor
					//                    using (FileStream config = new FileStream(project.PathToProjectTaskInventory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
					SetupTasksToBuildCaches(recordListManager);

					EntriesAlreadyWiredUp = (Db4oRecordList<LexEntry>) recordListManager.GetListOfType<LexEntry>();

					if (Db4oLexModelHelper.Singleton == null)
					{
						Db4oLexModelHelper.Initialize(ds.Data);
					}

					XmlDocument doc = new XmlDocument();
					doc.Load(_sourceLIFTPath);

					DoParsing(doc, ds);
				}
				ClearTheIncrementalBackupDirectory();

				//if we got this far without an error, move it
				string backupPath = _destinationDatabasePath + ".bak";
				//not needed File.Delete(backupPath);
				string cacheFolderName = _destinationDatabasePath + " Cache";
				if (Directory.Exists(cacheFolderName))
				{
					Directory.Delete(cacheFolderName, true);
					Directory.Move(tempTarget + " Cache", cacheFolderName);
			   }

				if (File.Exists(_destinationDatabasePath))
				{
					File.Replace(tempTarget, _destinationDatabasePath, backupPath);
				}
				else
				{
					File.Move(tempTarget, _destinationDatabasePath);
				}
				_progress.Status = ProgressState.StatusValue.Finished;
			}
			catch (Exception e)
			{
				_progress.WriteToLog(e.Message);
				_progress.Status = ProgressState.StatusValue.StoppedWithError;
			}
		}

		private void DoParsing(XmlDocument doc, Db4oDataSource ds)
		{
			Db4oRecordList<LexEntry> entriesList = GetEntries(ds);
			try
			{
				entriesList.WriteCacheSize = 0; //don't write after every record
				using (LiftMerger merger = new LiftMerger(ds, entriesList))
				{
					foreach (string name in WeSay.Project.WeSayWordsProject.Project.OptionFieldNames)
					{
						merger.ExpectedOptionTraits.Add(name);
						//_importer.ExpectedOptionTraits.Add(name);
					}
					foreach (
						string name in WeSay.Project.WeSayWordsProject.Project.OptionCollectionFieldNames)
					{
						merger.ExpectedOptionCollectionTraits.Add(name);
					}

					RunParser(doc, merger);
				}

				UpdateDashboardStats();
			}
			finally
			{
				if (entriesList != _prewiredEntries) //did we create it?
				{
					entriesList.Dispose();
				}
			}
		}

		private void UpdateDashboardStats()
		{
			foreach (ITask task in WeSayWordsProject.Project.Tasks)
			{
				if (task is IFinishCacheSetup)
				{
					((IFinishCacheSetup) task).FinishCacheSetup();
				}
			}
		}

		private static void SetupTasksToBuildCaches(IRecordListManager recordListManager)
		{
			recordListManager.DelayWritingCachesUntilDispose = true;
			ConfigFileTaskBuilder taskBuilder;
			using (
				FileStream configFile =
					new FileStream(WeSay.Project.WeSayWordsProject.Project.PathToProjectTaskInventory,
								   FileMode.Open, FileAccess.Read,
								   FileShare.ReadWrite))
			{
				taskBuilder = new ConfigFileTaskBuilder(configFile, WeSayWordsProject.Project,
														new EmptyCurrentWorkTask(), recordListManager);
			}

			WeSayWordsProject.Project.Tasks = taskBuilder.Tasks;

			//give the tasks a chance to register with the recordlistmanager
			foreach (ITask task in taskBuilder.Tasks)
			{
				task.Activate();
				task.Deactivate();
			}
		}

		private void RunParser(XmlDocument doc, LiftMerger merger)
		{
			LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence> parser =
				new LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>(merger);

			parser.SetTotalNumberSteps +=
				new EventHandler
					<LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.StepsArgs>(
					parser_SetTotalNumberSteps);
			parser.SetStepsCompleted +=
				new EventHandler
					<
						LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.
							ProgressEventArgs>(
					parser_SetStepsCompleted);

			parser.ParsingError +=
				new EventHandler
					<LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.ErrorArgs>(
					parser_ParsingError);
			parser.ReadFile(doc);
		}

		void parser_ParsingError(object sender, LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.ErrorArgs e)
		{
			_progress.WriteToLog(e.Exception.Message);
		}



		void parser_SetStepsCompleted(object sender, LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.ProgressEventArgs e)
		{
			_progress.NumberOfStepsCompleted = e.Progress;
		}

		void parser_SetTotalNumberSteps(object sender, LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.StepsArgs e)
		{
			_progress.NumberOfSteps = e.Steps;
		}

		/*public for unit-tests */
		public static void ClearTheIncrementalBackupDirectory()
		{
			if (!Directory.Exists(WeSay.Project.WeSayWordsProject.Project.PathToLiftBackupDir))
			{
				return;
			}
			string[] p = Directory.GetFiles(WeSay.Project.WeSayWordsProject.Project.PathToLiftBackupDir, "*.*");
			if (p.Length > 0)
			{
				string newPath = WeSay.Project.WeSayWordsProject.Project.PathToLiftBackupDir + ".old";

				int i = 0;
				while (Directory.Exists(newPath + i))
				{
					i++;
				}
				newPath += i;
				Directory.Move(WeSay.Project.WeSayWordsProject.Project.PathToLiftBackupDir,
							   newPath);
			}
		}

		protected override void DoWork(InitializeProgressCallback initializeCallback, ProgressCallback progressCallback,
									   StatusCallback primaryStatusTextCallback,
									   StatusCallback secondaryStatusTextCallback)
		{
			throw new NotImplementedException();
		}


	}

}