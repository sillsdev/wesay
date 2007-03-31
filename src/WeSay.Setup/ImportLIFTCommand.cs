using System;
using System.IO;
using System.Xml;
using LiftIO;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Progress;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay
{
	public class ImportLIFTCommand : BasicCommand
	{
		protected string _destinationDatabasePath;
		protected string _sourceLIFTPath;
		//private LiftImporter _importer;
		protected WeSay.Foundation.Progress.ProgressState _progress;

		public ImportLIFTCommand(string destinationDatabasePath, string sourceLIFTPath)
		{
			_destinationDatabasePath = destinationDatabasePath;
			_sourceLIFTPath = sourceLIFTPath;
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
				using (Db4oDataSource ds = new WeSay.Data.Db4oDataSource(tempTarget))
				{
					if (Db4oLexModelHelper.Singleton == null)
					{
						Db4oLexModelHelper.Initialize(ds.Data);
					}

					XmlDocument doc = new XmlDocument();
					doc.Load(_sourceLIFTPath);
					using (LiftMerger merger = new LiftMerger(ds))
					{
						foreach (string name in WeSay.Project.WeSayWordsProject.Project.OptionFieldNames)
						{
							merger.ExpectedOptionTraits.Add(name);
							//_importer.ExpectedOptionTraits.Add(name);
						}
						foreach (string name in WeSay.Project.WeSayWordsProject.Project.OptionCollectionFieldNames)
						{
							merger.ExpectedOptionCollectionTraits.Add(name);
						}

						LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence> parser =
							new LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>(merger);

						parser.SetTotalNumberSteps +=
							new EventHandler
								<LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.StepsArgs>(
								parser_SetTotalNumberSteps);
						parser.SetStepsCompleted +=
							new EventHandler
								<LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.ProgressEventArgs>(
								parser_SetStepsCompleted);

						parser.ParsingError += new EventHandler<LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>.ErrorArgs>(parser_ParsingError);
						parser.ReadFile(doc);
					}
				}
				ClearTheIncrementalBackupDirectory();

				//if we got this far without an error, move it
				string backupPath = _destinationDatabasePath + ".bak";
				//not needed File.Delete(backupPath);
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