using System;
using System.IO;
using System.Threading;
using WeSay;
using WeSay.App;
using WeSay.Data;
using WeSay.Foundation.Progress;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace Lift2WeSay
{
	class ConsoleProgress : ProgressState
	{
		int _numberOfSteps;
		int _numberOfStepsCompleted;
		string _statusLabel;

		public ConsoleProgress() : base(null)
		{

		}
		public override int NumberOfSteps
		{
			get
			{
				return _numberOfSteps;
			}
			set
			{
				_numberOfSteps = value;
			}
		}
		public override string StatusLabel
		{
			get
			{
				return _statusLabel;
			}
			set
			{
				Console.WriteLine(value);
				_statusLabel = value;
			}
		}
		public override int NumberOfStepsCompleted
		{
			get
			{
				return _numberOfStepsCompleted;
			}
			set
			{
				Console.Write('.');
				_numberOfStepsCompleted = value;
			}
		}
	}
	class Lift2WeSay
	{
		[STAThread]
		static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				PrintUsage();
				return;
			}
			string sourcePath = args[0];
			string destPath = args[1];

			if (Path.GetFileName(destPath) != string.Empty)
			{
				Console.WriteLine(string.Format("You can only specify a directory for the output, not the name of the output. (eg. {0} instead of {1})", Path.GetDirectoryName(destPath), destPath));
				return;
			}
			string projectPath = Path.GetDirectoryName(destPath);
			if (projectPath.Length == 0)
			{
				projectPath = Environment.CurrentDirectory;
			}
			projectPath = Path.Combine(projectPath, "..");
			projectPath = Path.GetFullPath(projectPath);

			Console.WriteLine("Lift2WeSay is converting");
			Console.WriteLine("Lift: " + sourcePath);
			Console.WriteLine("to WeSay: " + destPath);
			Console.WriteLine("in project: " + projectPath);

			if (!WeSayWordsProject.IsValidProjectDirectory(projectPath))
			{
				Console.Error.WriteLine("destination must be in 'wesay' subdirectory of a basil project");
				return;
			}

			WeSayWordsProject project = new WeSayWordsProject();
			try
			{
				project.LoadFromProjectDirectoryPath(projectPath);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("WeSay was not able to open that project. \r\n" + e.Message);
				return;
			}


			ConsoleProgress progress = new ConsoleProgress();
			progress.Log += new EventHandler<ProgressState.LogEvent>(progress_Log);

			ImportLIFTCommand command = new ImportLIFTCommand(project.PathToDb4oLexicalModelDB, sourcePath);
			command.BeginInvoke(progress);

			while (true)
			{
				switch (progress.Status)
				{
					case ProgressState.StatusValue.NotStarted:
						break;
					case ProgressState.StatusValue.Busy:
						break;
					case ProgressState.StatusValue.Finished:
						Console.WriteLine(string.Empty);
						Console.WriteLine("Done.");
						return;
					case ProgressState.StatusValue.StoppedWithError:
						Console.Error.WriteLine(string.Empty);
						Console.Error.WriteLine("Error. Unable to complete import.");
						return;
					default:
						break;
				}
				Thread.Sleep(10);
			}
		}

		static void progress_Log(object sender, ProgressState.LogEvent e)
	  {
		Console.Error.WriteLine(e.message);
	  }

		private static void PrintUsage()
		{
			Console.WriteLine("Usage: (outputfile must be in 'wesay' subdirectory of a basil project)");
			Console.WriteLine("Lift2WeSay inputLiftFilePath targetWeSayDirectory");
		}
	}
}
