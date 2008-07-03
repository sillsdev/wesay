using System;
using System.Collections.Generic;
using System.IO;
using LiftIO.Parsing;
using LiftIO.Validation;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;

namespace WeSay.LexicalModel
{
	public class LiftRepository:IRepository<LexEntry>
	{
		private class GuidRepositoryId:RepositoryId
		{
			private Guid _id;

			public GuidRepositoryId(Guid id)
			{
				this._id = id;
			}

			public override int CompareTo(RepositoryId other)
			{
				return CompareTo(other as GuidRepositoryId);
			}

			public int CompareTo(GuidRepositoryId other)
			{
				if(other == null)
				{
					return 1;
				}
				return Comparer<Guid>.Default.Compare(this._id, other._id);
			}

			public override bool Equals(RepositoryId other)
			{
				return Equals(other as GuidRepositoryId);
			}

			public bool Equals(GuidRepositoryId other)
			{
				if(other == null)
				{
					return false;
				}
				return Equals(this._id, other._id);
			}

		}

		private readonly string _liftFilePath;
		private DateTime lastModified;
		private readonly Dictionary<GuidRepositoryId, LexEntry> _entries;

		public LiftRepository(string filePath)
		{
			this._liftFilePath = filePath;
			this.lastModified = File.GetLastWriteTimeUtc(this._liftFilePath);
			_entries = new Dictionary<GuidRepositoryId, LexEntry>();
			LoadAllLexEntries();
		}

		public DateTime LastModified
		{
			get { return this.lastModified; }
		}

		public LexEntry CreateItem()
		{
			//todo write out new LexEntry
			return new LexEntry();
		}

		public int CountAllItems()
		{
			return GetAllItems().Length;
		}

		public RepositoryId GetId(LexEntry item)
		{
			return new GuidRepositoryId(item.Guid);
		}

		public LexEntry GetItem(RepositoryId id)
		{
			LoadAllLexEntries();
			return _entries[(GuidRepositoryId)id];
		}

		public void DeleteItem(LexEntry item)
		{
			//todo write out new LexEntry
			throw new ArgumentOutOfRangeException("item");
		}

		public void DeleteItem(RepositoryId id)
		{
			DeleteItem(GetItem(id));
		}

		public RepositoryId[] GetAllItems()
		{
			LoadAllLexEntries();
			GuidRepositoryId[] result = new GuidRepositoryId[_entries.Count];
			_entries.Keys.CopyTo(result, 0);
			return result;
		}

		public void SaveItem(LexEntry item)
		{
			//todo write out new LexEntry
			throw new ArgumentOutOfRangeException("item");
		}

		public bool CanQuery
		{
			get { return false; }
		}

		public bool CanPersist
		{
			get { return true; }
		}

		public void SaveItems(IEnumerable<LexEntry> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}

			foreach (LexEntry item in items)
			{
				SaveItem(item);
			}
		}

		public ResultSet<LexEntry> GetItemsMatching(Query query)
		{
			throw new NotSupportedException("Querying is not supported");
		}

		#region IDisposable Members
#if DEBUG
		~LiftRepository()
		{
			if (!this._disposed)
			{
				throw new ApplicationException("Disposed not explicitly called on LiftRepository.");
			}
		}
#endif

		private bool _disposed = false;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					// dispose-only, i.e. non-finalizable logic

				}

				// shared (dispose and finalizable) cleanup logic
				this._disposed = true;
			}
		}

		protected void VerifyNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("LiftRepository");
			}
		}
		#endregion
		private void LoadAllLexEntries()
		{
			_entries.Clear();
			using (LiftMerger merger = new LiftMerger())
			{
				merger.EntryCreatedEvent += OnEntryCreated;
				LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence> parser =
						new LiftParser<WeSayDataObject, LexEntry, LexSense, LexExampleSentence>(
								merger);

				parser.SetTotalNumberSteps += parser_SetTotalNumberSteps;
				parser.SetStepsCompleted += parser_SetStepsCompleted;

				parser.ParsingWarning += parser_ParsingWarning;

				try
				{
					parser.ReadLiftFile(_liftFilePath);
				}
				catch (Exception)
				{
					//our parser failed.  Hopefully, because of bad lift. Validate it now  to
					//see if that's the problem.
					Validator.CheckLiftWithPossibleThrow(_liftFilePath);

					//if it got past that, ok, send along the error the parser encountered.
					throw;
				}
			}
		}

		private void OnEntryCreated(object sender, LiftMerger.EntryCreatedEventArgs e)
		{
			_entries.Add(new GuidRepositoryId(e.Entry.Guid),
							e.Entry);
		}
		private void parser_ParsingWarning(object sender,
										   LiftParser
												   <WeSayDataObject, LexEntry, LexSense,
												   LexExampleSentence>.ErrorArgs e)
		{
		}

		private void parser_SetStepsCompleted(object sender,
											  LiftParser
													  <WeSayDataObject, LexEntry, LexSense,
													  LexExampleSentence>.ProgressEventArgs e)
		{
		}

		private void parser_SetTotalNumberSteps(object sender,
												LiftParser
														<WeSayDataObject, LexEntry, LexSense,
														LexExampleSentence>.StepsArgs e)
		{
		}


	}
}