using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Data
{
	public interface IRecordListManager: IDisposable
	{
		void Register<T>(IFilter<T> filter) where T : class, new();
		IRecordList<T> Get<T>()  where T : class, new();
		IRecordList<T> Get<T>(IFilter<T> filter)  where T : class, new();

		/// <summary>
		/// Call this, for example, when switching records in the gui. You don't need to know
		/// whether a commit is pending or not.
		/// </summary>
		void GoodTimeToCommit();
	}
}
