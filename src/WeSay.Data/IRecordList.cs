using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace WeSay.Data
{
	public interface IRecordList<T>: IBindingList, IFilterable<T>, IList<T>, ICollection<T>, IEnumerable<T> where T : class, new()
	{
	}
}
