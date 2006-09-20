using System;
namespace WeSay.Data
{
	public interface IFilter<T>
	{
		Predicate<T> Inquire
		{
			get;
		}

		string Key
		{
			get;
		}
	}
}
