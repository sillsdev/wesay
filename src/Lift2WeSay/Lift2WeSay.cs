using System;
using System.IO;
using System.Xml;
using WeSay.Data;
using WeSay.Foundation.Progress;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

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
			string sourcePath = args[0];
			string destPath = args[1];

			Console.WriteLine("Lift2WeSay is converting");
			Console.WriteLine("Lift: " + sourcePath);
			Console.WriteLine("to WeSay: " + destPath);

			if (File.Exists(destPath))
			{
				File.Delete(destPath);
			}

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
					importer.Progress = new ConsoleProgress();
					importer.ReadFile(doc, entries);
				}
			}
			Console.WriteLine("Done.");
		}

		private static void PrintUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("Lift2WeSay inputfile outputfile");
		}
	}
}
