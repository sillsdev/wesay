using System;

namespace WeSay.LexicalModel
{
	public interface IFieldQuery<T>
	{
		Predicate<T> FilteringPredicate { get; }

		string Key { get; }

		Field Field { get; }
	}
}