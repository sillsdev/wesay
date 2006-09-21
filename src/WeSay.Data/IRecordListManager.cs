using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Data
{
	public interface IRecordListManager<T> where T : class, new()
	{
		IRecordList<T> Get();
		IRecordList<T> Get(IFilter<T> filter);
	}
}
