using System;
namespace WeSay.Data
{
	public interface IFilterable<T>
	{
		void ApplyFilter(Predicate<T> filter);
		bool IsFiltered
		{
			get;
		}
		void RemoveFilter();
	}
}
