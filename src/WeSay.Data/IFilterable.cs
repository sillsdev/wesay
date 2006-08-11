using System;
namespace WeSay.Data
{
	interface IFilterable<T>
	{
		void ApplyFilter(Predicate<T> filter);
		bool IsFiltered
		{
			get;
		}
		void RemoveFilter();
	}
}
