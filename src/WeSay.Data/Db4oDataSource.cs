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
		}

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
