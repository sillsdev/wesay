using System;
namespace WeSay.Data
{
	interface IFilterable<T>
	 where T : new()
	{
		void ApplyFilter(Predicate<T> filter);
		bool IsFiltered
		{
			get;
		}
		void RemoveFilter();
	}
}
