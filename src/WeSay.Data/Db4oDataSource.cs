#if DEBUG
#define THROW_ON_OPTIMIZATION_FAILURE
#endif

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace WeSay.Data
{
  [Serializable]
  public class Db4oException : System.Data.Common.DbException
  {
	public Db4oException() : base() {}
	public Db4oException(string message):base(message) {}
	protected Db4oException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	public Db4oException(string message, Exception innerException) : base(message, innerException) {}
	public Db4oException(string message, int errorCode) : base(message, errorCode) {}
  }
	public class Db4oDataSource : IDisposable
	{
		com.db4o.ObjectContainer _db;
		private bool _disposed;

		public Db4oDataSource(string filePath)
		{
			_db = com.db4o.Db4o.OpenFile(filePath);
			if (_db == null)
			{
				throw new System.IO.IOException("Problem opening " + filePath);
			}
#if THROW_ON_OPTIMIZATION_FAILURE
			((com.db4o.YapStream)_db).GetNativeQueryHandler().QueryOptimizationFailure += new com.db4o.inside.query.QueryOptimizationFailureHandler(OnQueryOptimizationFailure);
#endif
		}

#if THROW_ON_OPTIMIZATION_FAILURE
		void OnQueryOptimizationFailure(object sender, com.db4o.inside.query.QueryOptimizationFailureEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine("Query not Optimized:");
			System.Diagnostics.Debug.WriteLine(args.Reason);
			throw new Db4oException("Query not Optimized", args.Reason);
		}
#endif

		public object Data
		{
			get
			{
				if (this._disposed)
				{
					throw new ObjectDisposedException("Db4oDataSource");
				}
				return _db;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
		  Dispose(true);
		}
	  protected virtual void Dispose(bool disposing)
	  {
		if (!this._disposed)
		{
		  if (disposing)
		  {
			_db.Close();
			_db.Dispose();
			_db = null;
			GC.SuppressFinalize(this);
		  }
		}
		_disposed = true;
	  }
		#endregion

	}
}
