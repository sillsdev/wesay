using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Mono.Addins;
using Palaso.I8N;
using Palaso.Progress;
using WeSay.AddinLib;
using WeSay.LexicalModel;

namespace Addin.Transform
{
	[Extension]
	public class SfmTransformer: LiftTransformer, IWeSayAddinHasSettings
	{
		private SfmTransformSettings _settings;

		public SfmTransformer()
		{
			_settings = new SfmTransformSettings();
		}

		public override string LocalizedName
		{
			get { return StringCatalog.Get("~Export To SFM"); }
		}

		public override string LocalizedLabel
		{
			get { return LocalizedName; }
		}

		public override string LocalizedLongLabel
		{
			get { return LocalizedName; }
		}

		public override string Description
		{
			get { return StringCatalog.Get("~Saves the lexicon in a form of standard format."); }
		}

		public override Image ButtonImage
		{
			get { return Resources.SfmTransformerButtonImage; }
		}

		/// <summary>
		/// this runs in a worker thread
		/// </summary>
		private static void OnDoGrepWork(object sender, DoWorkEventArgs args)
		{
			var progressState = (ProgressState) args.Argument;
			var workerArguments = (TransformWorkerArguments) (progressState.Arguments);

			progressState.StatusLabel = "Converting to MDF...";
			progressState.NumberOfStepsCompleted++;
			//System.Threading.Thread.Sleep(100);//don't event see that message otherwise
			GrepFile(workerArguments.outputFilePath, args);
		}

		private static void GrepFile(string inputPath, DoWorkEventArgs args)
		{
			var progressState = (ProgressState) args.Argument;
			var workerArguments = (TransformWorkerArguments) (progressState.Arguments);
			var sfmSettings = (SfmTransformSettings) workerArguments.postTransformArgument;

			string tempPath = inputPath + ".tmp";
			IEnumerable<SfmTransformSettings.ChangePair> pairs = sfmSettings.ChangePairs;
			using (StreamReader reader = File.OpenText(inputPath))
			{
				using (var writer = new StreamWriter(tempPath))
				{
					while (!reader.EndOfStream)
					{
						string line = reader.ReadLine();
						if (progressState.Cancel)
						{
							return;
						}
						if (line.StartsWith("\\dt "))
						{
							line = ConvertDateLineToToolboxFormat(line);
						}
						//we don't have a way of knowing      progressState.NumberOfStepsCompleted = ;
						foreach (SfmTransformSettings.ChangePair pair in pairs)
						{
							//this is super slow
							line = pair.regex.Replace(line, pair.to);
						}
						writer.WriteLine(line);
					}
					writer.Close();
				}
				reader.Close();
			}

			//System.Threading.Thread.Sleep(500);//don't event see that message otherwise
			progressState.StatusLabel = "Cleaning up...";
			progressState.NumberOfStepsCompleted++;
			File.Delete(inputPath);
			File.Move(tempPath, inputPath); //, backupPath);
			progressState.NumberOfStepsCompleted = progressState.TotalNumberOfSteps;
			Thread.Sleep(500); //don't event see that message otherwise
		}

		private static string ConvertDateLineToToolboxFormat(string line)
		{
			string[] parts = line.Split(' ');
			if (parts.Length > 1)
			{
				DateTime dt = DateTime.Parse(parts[1]);
				line = parts[0] + " " + dt.ToString(@"d\/MMM\/yyyy");
			}
			return line;
		}

		public override void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			_settings.FillEmptySettingsWithGuesses(projectInfo);
			SetupPostTransformMethod(OnDoGrepWork, _settings, 10 /*has some cushion*/);
			LexEntryRepository repo = projectInfo.ServiceProvider.GetService(typeof (LexEntryRepository)) as LexEntryRepository;
			string output = TransformLiftToText(projectInfo, "lift2sfm.xsl", "-sfm.txt");
			if (string.IsNullOrEmpty(output))
			{
				return; // get this when the user cancels
			}
			//GrepFile(output, _settings);

			if (_launchAfterTransform)
			{
				Process.Start(output);
			}
		}

		#region IWeSayAddinHasSettings Members

		public bool DoShowSettingsDialog(Form parentForm, ProjectInfo projectInfo)
		{
			var dlg = new SFMChangesDialog(_settings, projectInfo);
			return dlg.ShowDialog(parentForm) == DialogResult.OK;
		}

		public object Settings
		{
			get { return _settings; }
			set { _settings = (SfmTransformSettings) value; }
		}

		public override string ID
		{
			get { return "Export To SFM"; }
			//CAN'T CHANGE THIS WITHOUT PROVIDING A MIGRATION FOR FOLKS!
		}

		#endregion
	}
}