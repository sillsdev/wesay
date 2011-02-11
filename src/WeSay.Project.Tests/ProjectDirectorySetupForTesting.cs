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
		private string _projectDirectoryName;

		public ProjectDirectorySetupForTesting(string xmlOfEntries)
				: this(xmlOfEntries, Validator.LiftVersion) {}

		public ProjectDirectorySetupForTesting(string xmlOfEntries, string liftVersion)
		{
			_projectDirectoryName = Path.GetRandomFileName();
			_experimentDir = MakeDir(Path.GetTempPath(), ProjectDirectoryName);
			WeSayWordsProject.CreateEmptyProjectFiles(_experimentDir, ProjectName);

			//overwrite the blank lift file
			string liftContents =
					string.Format(
							"<?xml version='1.0' encoding='utf-8'?><lift version='{0}'>{1}</lift>",
							liftVersion,
							xmlOfEntries);
			File.WriteAllText(PathToLiftFile, liftContents);

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

		public string PathToUserConfigFile
		{
			get
			{
				var files = Directory.GetFiles(PathToDirectory, "*.WeSayUserConfig");
				if (files.Length > 1)
				{
					throw new IndexOutOfRangeException(String.Format(
						"Too many WeSayUserConfig files ({0}) in test folder.",
						files.Length
					));
				}
				if (files.Length == 1)
				{
					return files[0];
				}
				return Path.Combine(PathToDirectory, Environment.UserName + ".WeSayUserConfig");
			}
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
