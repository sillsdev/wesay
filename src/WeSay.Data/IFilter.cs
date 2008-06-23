using System;

namespace WeSay.Data
{
	public interface IFilter<T>
	{
		Predicate<T> FilteringPredicate { get; }

		string Key { get; }
	}

	public class AllItems<T>: IFilter<T>
	{
		public Predicate<T> FilteringPredicate
		{
			get { return ReturnTrue; }
		}

		private static bool ReturnTrue(T t)
		{
			return true;
		}

		/// <summary>
		/// Filters are kept in a list; this is the string by which a filter is accessed.
		/// </summary>
		public string Key
		{
			get { return "AllItems"; }
		}
	}
}