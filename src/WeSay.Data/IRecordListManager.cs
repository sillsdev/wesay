using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Data
{
	public interface IRecordListManager: IDisposable
	{
		IRecordList<T> Get<T>()  where T : class, new();
		IRecordList<T> Get<T>(IFilter<T> filter)  where T : class, new();
	}
}
