using System;
using System.Collections.Generic;
using System.Text;
using com.db4o;

namespace WeSay.Data
{
	public class Db4oDataSource : IDisposable
	{
		ObjectContainer _db;

		public Db4oDataSource(string filePath)
		{
			_db = Db4o.OpenFile(filePath);
			if (_db == null)
			{
				throw new ApplicationException("Problem opening " + filePath);
			}
		}

		public object Data
		{
			get
			{
				return _db;
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			_db.Close();
		}

		#endregion
	}
}
