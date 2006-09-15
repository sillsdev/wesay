#if DEBUG
#define THROW_ON_OPTIMIZATION_FAILURE
#endif

using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Data
{
	public class Db4oDataSource : IDisposable
	{
		com.db4o.ObjectContainer _db;
		private bool _disposed = false;

		public Db4oDataSource(string filePath)
		{
			_db = com.db4o.Db4o.OpenFile(filePath);
			if (_db == null)
			{
				throw new ApplicationException("Problem opening " + filePath);
			}
#if THROW_ON_OPTIMIZATION_FAILURE
			((com.db4o.YapStream)_db).GetNativeQueryHandler().QueryOptimizationFailure += new com.db4o.inside.query.QueryOptimizationFailureHandler(OnQueryOptimizationFailure);
#endif
		}

#if THROW_ON_OPTIMIZATION_FAILURE
		void OnQueryOptimizationFailure(object sender, com.db4o.inside.query.QueryOptimizationFailureEventArgs args)
		{
			throw new ApplicationException("Query not Optimized", args.Reason);
			//System.Diagnostics.Debug.WriteLine("Query not Optimized:");
			//System.Diagnostics.Debug.WriteLine(args.Reason);
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
			if (!this._disposed)
			{
				_db.Close();
				_db.Dispose();
				_db = null;
				GC.SuppressFinalize(this);
			}
			_disposed = true;
		}

		#endregion
	}
}
