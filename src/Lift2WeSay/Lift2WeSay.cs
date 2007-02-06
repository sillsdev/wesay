using System;
using System.Collections;
using System.IO;
using System.Xml;
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
		string _status;

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
		public override string Status
		{
			get
			{
				return _status;
			}
			set
			{
				Console.WriteLine(value);
				_status = value;
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
		static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				PrintUsage();
				return;
			}
			//string projectPath = args[0];
			string sourcePath = args[0];
			string destPath = args[1];

			Console.WriteLine("Lift2WeSay is converting");
			Console.WriteLine("Lift: " + sourcePath);
			Console.WriteLine("to WeSay: " + destPath);

			if (File.Exists(destPath))
			{
				File.Delete(destPath);
			}

		   // DoImportUsingRecordListManager(destPath, sourcePath);
			DoImportUsingRawDb4o(destPath, sourcePath);
			Console.WriteLine("Done.");
		}

		private static void DoImportUsingRecordListManager(string destPath, string sourcePath)
		{
			using (IRecordListManager recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), destPath))
		{
			Db4oRecordListManager listManager = recordListManager as Db4oRecordListManager;
			IRecordList<LexEntry> entries =  listManager.GetListOfType<LexEntry>();

			Db4oLexModelHelper.Initialize(listManager.DataSource.Data);

//           using (Db4oDataSource ds = new Db4oDataSource(destPath))
//            {
//                using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
//                {
					if (Db4oLexModelHelper.Singleton == null)
					{
					  //  Db4oLexModelHelper.Initialize(ds.Data);
					}

				XmlDocument doc = new XmlDocument();
					doc.Load(sourcePath);
					LiftImporter importer = LiftImporter.CreateCorrectImporter(doc);

					WeSayWordsProject project = new WeSayWordsProject();
					WeSay.Project.WeSayWordsProject.Project.LoadFromProjectDirectoryPath(Directory.GetParent(Environment.CurrentDirectory).FullName);
					foreach (string name in WeSay.Project.WeSayWordsProject.Project.OptionFieldNames)
					{
						importer.ExpectedOptionTraits.Add(name);
					}
					foreach (string name in WeSay.Project.WeSayWordsProject.Project.OptionCollectionFieldNames)
					{
						importer.ExpectedOptionCollectionTraits.Add(name);
					}
					importer.Progress = new ConsoleProgress();
					importer.ReadFile(doc, entries);
				}
			//}
		}

		private static void DoImportUsingRawDb4o(string destPath, string sourcePath)
		{
				DateTime time = new DateTime();
			Lift2WeSay x = new Lift2WeSay();
			using (Db4oDataSource ds = new Db4oDataSource(destPath))
			{
				using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
				{
					if (Db4oLexModelHelper.Singleton == null)
					{
						Db4oLexModelHelper.Initialize(ds.Data);
					}

					XmlDocument doc = new XmlDocument();
					doc.Load(sourcePath);
					LiftImporter importer = LiftImporter.CreateCorrectImporter(doc);

					WeSayWordsProject project = new WeSayWordsProject();
					WeSay.Project.WeSayWordsProject.Project.LoadFromProjectDirectoryPath(Directory.GetParent(Environment.CurrentDirectory).FullName);
					foreach (string name in WeSay.Project.WeSayWordsProject.Project.OptionFieldNames)
					{
						importer.ExpectedOptionTraits.Add(name);
					}
					foreach (string name in WeSay.Project.WeSayWordsProject.Project.OptionCollectionFieldNames)
					{
						importer.ExpectedOptionCollectionTraits.Add(name);
					}
					importer.Progress = new ConsoleProgress();
					importer.ReadFile(doc, entries);
				}
			}
		}

		private static void PrintUsage()
		{
			Console.WriteLine("Usage: (must run from 'wesay' subdirectory of a basil project)");
			Console.WriteLine("Lift2WeSay inputfile outputfile");
//            Console.WriteLine("Lift2WeSay projectDirectory inputfile outputfile");
		}
	}
}
