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

	public class AllItems<T> : IFilter<T>
	{
		public Predicate<T> FilteringPredicate
		{
			get { return ReturnTrue; }
		}

		static private bool ReturnTrue(T t)
		{
			return true;
		}

		public string Key
		{
			get { return "AllItems"; }
		}
	}

}
