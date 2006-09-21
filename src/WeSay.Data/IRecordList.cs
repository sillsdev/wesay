using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace WeSay.Data
{
	public interface IRecordList<T>: IBindingList, IFilterable<T>, IEquatable<IRecordList<T>> where T : class, new()
	{
	}
}
