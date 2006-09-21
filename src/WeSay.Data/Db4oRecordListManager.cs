using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Data
{
	public class Db4oRecordListManager<T> : IRecordListManager<T> where T : class, new()
	{
		#region IRecordListManager<T> Members

		public IRecordList<T> Get()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IRecordList<T> Get(IFilter<T> filter)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
