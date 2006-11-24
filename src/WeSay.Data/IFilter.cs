using System;
namespace WeSay.Data
{
	public interface IFilter<T>
	{
		Predicate<T> FilteringPredicate
		{
			get;
		}

		string Key
		{
			get;
		}
	}
}
