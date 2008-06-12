using System;
using Palaso.Progress;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;

namespace WeSay.Project.Tests
{
	/// <summary>
	/// This creates (and disposes of) a temporary, fully-functional,
	/// non-ui environment for tests that hit against a real db4o system
	///
	/// TODO: disposing this seems to be the cause of this error:
	///     System.AppDomainUnloadedException: Attempted to access an unloaded AppDomain.
	///
	///  This is extremely slow to use. TODO: provide a way to generate the folder once and
	/// just reuse copies of it for each test.

	/// </summary>
	public class Db4oProjectSetupForTesting:IDisposable
	{
		private bool _disposed = false;
		private ProjectDirectorySetupForTesting _projectDirectory;
		public WeSayWordsProject _project;
		public LexEntryRepository _lexEntryRepository;

		public Db4oProjectSetupForTesting(string xmlOfEntries)
	   {
			_projectDirectory = new ProjectDirectorySetupForTesting(xmlOfEntries);

			_project = new WeSayWordsProject();
			_project.LoadFromLiftLexiconPath(_projectDirectory.PathToLiftFile);
			CacheBuilder cacheBuilder = new CacheBuilder(_projectDirectory.PathToLiftFile);
			cacheBuilder.DoWork(new NullProgressState());

			_lexEntryRepository = new LexEntryRepository(_project.PathToDb4oLexicalModelDB);// InMemoryRecordListManager();
	   }

	   #region IDisposable Members


		~Db4oProjectSetupForTesting()
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
					_lexEntryRepository.Dispose();
					_project.Dispose();
					_projectDirectory.Dispose();

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
