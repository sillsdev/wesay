using System;
using System.IO;
using LiftIO.Validation;

namespace WeSay.Project.Tests
{
	/// <summary>
	/// Creates a valid WeSay project directory in temp dir, and removes it when disposed.
	/// </summary>
	public class ProjectDirectorySetupForTesting: IDisposable
	{
		private bool _disposed;
		private readonly string _experimentDir;
		private readonly string _projectName = "test";
	  //  private readonly string _pathToTasksBase;
		private string _projectDirectoryName;

		public ProjectDirectorySetupForTesting(string xmlOfEntries)
				: this(xmlOfEntries, Validator.LiftVersion) {}

		public ProjectDirectorySetupForTesting(string xmlOfEntries, string liftVersion)
		{
			_projectDirectoryName = Path.GetRandomFileName();
			_experimentDir = MakeDir(Path.GetTempPath(), ProjectDirectoryName);
			WeSayWordsProject.CreateEmptyProjectFiles(_experimentDir, ProjectName);
//            using (WeSayWordsProject p = new WeSayWordsProject())
//            {
//                p.PathToLiftFile = Path.Combine(_experimentDir, ProjectName + ".lift");
//           //     p.CreateEmptyProjectFiles(_experimentDir);
//                Assert.IsTrue(File.Exists(p.PathToConfigFile));
//                _pathToTasksBase = Path.Combine(p.ProjectDirectoryPath, "temptasks.xml");
//                File.Copy(p.PathToConfigFile, _pathToTasksBase);
//                p.EditorsSaveNow += p_EditorsSaveNow;
//                p.Save();
//            }

			//overwrite the blank lift file
			string liftContents =
					string.Format(
							"<?xml version='1.0' encoding='utf-8'?><lift version='{0}'>{1}</lift>",
							liftVersion,
							xmlOfEntries);
			File.WriteAllText(PathToLiftFile, liftContents);

		}

		private void p_EditorsSaveNow(object sender, EventArgs e)
		{
			//ok, the hard part is that now we have a config with tasks, but no view template.
//            XmlDocument doc = new XmlDocument();
//            doc.Load(_pathToTasksBase);
//            XmlWriter writer = (XmlWriter) sender;
//            IList<ViewTemplate> viewTemplates = WeSayWordsProject.Project.ViewTemplates;
//            writer.WriteStartElement("components");
//            foreach (ViewTemplate template in viewTemplates)
//            {
//                template.Write(writer);
//            }
//            writer.WriteEndElement();
//
//            writer.WriteStartElement("tasks");
//            foreach (XmlNode taskNode in doc.SelectNodes("//task"))
//            {
//                taskNode.WriteTo(writer);
//            }
//            writer.WriteEndElement();
		}

		public string PathToDirectory
		{
			get { return _experimentDir; }
		}

		public string PathToLiftFile
		{
			get { return Path.Combine(_experimentDir, "test.lift"); }
		}

		public string PathToConfigFile
		{
			get { return Path.Combine(_experimentDir, "test.wesayConfig"); }
		}

		public string PathToWritingSystemFile
		{
			get { return Path.Combine(_experimentDir, "WritingSystemPrefs.xml"); }
		}

		public string PathToFactoryDefaultsPartsOfSpeech
		{
			get
			{
				string fileName = "WritingSystemPrefs.xml";
				string path = Path.Combine(BasilProject.ApplicationCommonDirectory, fileName);
				if (File.Exists(path))
				{
					return path;
				}

				path = Path.Combine(BasilProject.DirectoryOfTheApplicationExecutable, fileName);
				if (File.Exists(path))
				{
					return path;
				}
				return path;
			}
		}

		private static string MakeDir(string existingParent, string newChild)
		{
			string dir = Path.Combine(existingParent, newChild);
			Directory.CreateDirectory(dir);
			return dir;
		}

		#region IDisposable Members

		~ProjectDirectorySetupForTesting()
		{
			if (!_disposed)
			{
				throw new InvalidOperationException("Disposed not explicitly called on " +
													GetType().FullName + ".");
			}
		}

		public bool IsDisposed
		{
			get { return _disposed; }
		}

		public string ProjectName
		{
			get { return _projectName; }
		}

		public string ProjectDirectoryName
		{
			get { return _projectDirectoryName; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					Palaso.TestUtilities.TestUtilities.DeleteFolderThatMayBeInUse(_experimentDir);
				}

				// shared (dispose and finalizable) cleanup logic
				_disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

		#endregion

		public WeSayWordsProject CreateLoadedProject()
		{

			WeSayWordsProject p = new WeSayWordsProject();
			p.LoadFromLiftLexiconPath(PathToLiftFile);


			return p;
		}


	}
}
