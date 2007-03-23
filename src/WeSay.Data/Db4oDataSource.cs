#if DEBUG
#define THROW_ON_OPTIMIZATION_FAILURE
#endif

using System;
using System.Runtime.Serialization;
using Db4objects.Db4o.Diagnostic;

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
		Db4objects.Db4o.IObjectContainer _db;
		private bool _disposed;

		public Db4oDataSource(string filePath)
		{
			Db4objects.Db4o.Config.IConfiguration db4oConfiguration = Db4objects.Db4o.Db4oFactory.Configure();
			db4oConfiguration.MarkTransient("NonSerialized"); // this attribute is build-in to .net

			_db = Db4objects.Db4o.Db4oFactory.OpenFile(filePath);
			if (_db == null)
			{
				throw new System.IO.IOException("Problem opening " + filePath);
			}
#if THROW_ON_OPTIMIZATION_FAILURE
			((Db4objects.Db4o.YapStream)_db).GetNativeQueryHandler().QueryOptimizationFailure += new Db4objects.Db4o.Inside.Query.QueryOptimizationFailureHandler(OnQueryOptimizationFailure);
#endif

			//remove diagnostic messages which slow down the tests
			db4oConfiguration.Diagnostic().RemoveAllListeners();
		}

#if THROW_ON_OPTIMIZATION_FAILURE
		void OnQueryOptimizationFailure(object sender, Db4objects.Db4o.Inside.Query.QueryOptimizationFailureEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine("Query not Optimized:");
			System.Diagnostics.Debug.WriteLine(args.Reason);
			throw new Db4oException("Query not Optimized", args.Reason);
		}
#endif
		[CLSCompliant(false)]
		public Db4objects.Db4o.IObjectContainer Data
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
