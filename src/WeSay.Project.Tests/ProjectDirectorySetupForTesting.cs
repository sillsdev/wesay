using SIL.Lift.Validation;
using SIL.TestUtilities;
using SIL.Windows.Forms.Progress;
using SIL.WritingSystems;
using System;
using System.IO;

namespace WeSay.Project.Tests
{
	/// <summary>
	/// Creates a valid WeSay project directory in temp dir, and removes it when disposed.
	/// </summary>
	public class ProjectDirectorySetupForTesting : IDisposable
	{
		private bool _disposed;
		private const string ProjectName = "Test"; // Note the capital making a case difference from the folder name.
		private const string ProjectFolder = "test";

		private readonly TemporaryFolder _testFolder;
		private readonly TemporaryFolder _projectRootFolder;

		public ProjectDirectorySetupForTesting(string xmlOfEntries)
				: this(xmlOfEntries, Validator.LiftVersion) { }

		public ProjectDirectorySetupForTesting(string xmlOfEntries, string liftVersion)
		{
			_testFolder = new TemporaryFolder("WeSayProjectTest" + Path.GetRandomFileName());
			_projectRootFolder = new TemporaryFolder(_testFolder, ProjectFolder);
			string unlistedLanguage = WellKnownSubtags.UnlistedLanguage;
			WeSayWordsProject.CreateEmptyProjectFiles(_projectRootFolder.Path, ProjectName, unlistedLanguage);

			//overwrite the blank lift file
			string liftContents =
					string.Format(
							"<?xml version='1.0' encoding='utf-8'?><lift version='{0}'>{1}</lift>",
							liftVersion,
							xmlOfEntries);
			File.WriteAllText(PathToLiftFile, liftContents);

			// liftSynchronizerAdjunct requires some lift-ranges file
			File.WriteAllText(Path.ChangeExtension(PathToLiftFile, "lift-ranges"), @"<?xml version='1.0' encoding='utf-8'?>
<lift-ranges/>");
		}


		public string PathToDirectory
		{
			get { return _projectRootFolder.Path; }
		}

		public string PathToLiftFile
		{
			get { return Path.Combine(_projectRootFolder.Path, "Test.lift"); }
		}

		public string PathToConfigFile
		{
			get { return Path.Combine(_projectRootFolder.Path, "Test.WeSayConfig"); }
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

		public string ProjectDirectoryName
		{
			get { return _projectRootFolder.Path; }
		}

		public void Dispose()
		{
			_testFolder.Dispose();
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
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

		public WeSayWordsProject CreateLoadedProject(IProgressNotificationProvider progressProvider = null)
		{

			var p = new WeSayWordsProject();
			p.LoadFromLiftLexiconPath(PathToLiftFile, progressProvider);

			return p;
		}


	}
}
