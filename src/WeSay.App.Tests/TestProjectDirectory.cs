using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using WeSay.Foundation.Tests;
using WeSay.Project;

namespace WeSay.App.Tests
{
	/// <summary>
	/// Creates a valid WeSay project directory in temp dir, and removes it when disposed.
	/// </summary>
	public class TestProjectDirectory : IDisposable
	{
		private bool _disposed = false;
		private readonly string _experimentDir;
		private string _projectName = "test";

		public TestProjectDirectory(string xmlOfEntries)
		{
			_experimentDir = MakeDir(Path.GetTempPath(), Path.GetRandomFileName());
			using (WeSayWordsProject p = new WeSayWordsProject())
			{
				p.PathToLiftFile = Path.Combine(_experimentDir, _projectName + ".lift");
				p.CreateEmptyProjectFiles(_experimentDir);
				p.Save();
				Assert.IsTrue(File.Exists(p.PathToConfigFile));
			}

			//overwrite the blank lift file
			string liftContents = string.Format("<?xml version='1.0' encoding='utf-8'?><lift version='{0}'>{1}</lift>", LiftIO.Validator.LiftVersion, xmlOfEntries);
			File.WriteAllText(PathToLiftFile, liftContents);
		}

		public string PathToLiftFile
		{
			get { return Path.Combine(_experimentDir, "test.lift"); }
		}
		private static string MakeDir(string existingParent, string newChild)
		{
			string dir = Path.Combine(existingParent, newChild);
			Directory.CreateDirectory(dir);
			return dir;
		}
		#region IDisposable Members


		~TestProjectDirectory()
		{
			if (!this._disposed)
			{
				throw new InvalidOperationException("Disposed not explicitly called on " + GetType().FullName + ".");
			}
		}

		public bool IsDisposed
		{
			get
			{
				return _disposed;
			}
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
					TestUtilities.DeleteFolderThatMayBeInUse(_experimentDir);
				}

				// shared (dispose and finalizable) cleanup logic
				this._disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

		#endregion
	}
}
