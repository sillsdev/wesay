using System;

namespace WeSay.Project
{
	public interface IFieldQuery<T>
	{
		Predicate<T> FilteringPredicate { get; }

		string Key { get; }

		Field Field
		{
			get;
		}
	}
}