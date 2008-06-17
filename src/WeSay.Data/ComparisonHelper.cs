using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System
{
	/// <summary>
	/// Helper functionality for comparing items (finding, filtering, sorting, ...).
	///
	/// *****************************************
	/// Author:  Marek Ištvánek (Marek Istvanek)
	///          Slušovice (Slusovice)
	///          Morava (Moravia)
	///          Èeská republika (Czech Republic)
	///          Marek.Istvanek@atlas.cz
	/// _________________________________________
	/// Version: 2006.06.22
	/// * Static methods from <see cref="Db4oList{}"/>, <see cref="Db4oBindingList{}"/> moved here.
	/// *****************************************
	///
	/// </summary>
	/// <typeparam name="T">Type of item.</typeparam>
	public static class ComparisonHelper<T>
	{
		/// <summary>
		/// Default predicate always returning true.
		/// </summary>
		public static Predicate<T> DefaultPredicate
		{
			get { return delegate(T item) { return true; }; }
		}

		/// <summary>
		/// Default property predicate which gets property value from <see cref="PropertyDescriptor"/> and compares it to key value using <see cref="EqualityComparer{T}.Default.Equals"/>.
		/// </summary>
		/// <remarks>If item is null, the predicate returns false.</remarks>
		public static PropertyPredicate<T> DefaultPropertyPredicate
		{
			get
			{
				return delegate(T item, PropertyDescriptor property, object key)
					   {
						   if (item == null)
						   {
							   return false;
						   }
						   else
						   {
							   return
									   EqualityComparer<object>.Default.Equals(
											   property.GetValue(item), key);
						   }
					   };
			}
		}

		/// <summary>
		/// Default comparison using <see cref="Comparer{}.Default"/>.
		/// </summary>
		public static Comparison<T> DefaultComparison
		{
			get { return GetComparison(null); }
		}

		/// <summary>
		/// Default property comparison which uses <see cref="PropertyDescriptor.GetValue"/> to get values of compared items` properties and compares the values using <see cref="Comparer.Default.Compare"/>.
		/// </summary>
		public static PropertyComparison<T> DefaultPropertyComparison
		{
			get
			{
				return delegate(T item1, T item2, PropertyDescriptor property)
					   {
						   object value1 = property.GetValue(item1);
						   object value2 = property.GetValue(item2);
						   return Comparer.Default.Compare(value1, value2);
					   };
			}
		}

		/// <summary>
		/// Default equality comparison using <see cref="EqualityComparer{}.Default"/> comparer.
		/// </summary>
		public static EqualityComparison<T> DefaultEqualityComparison
		{
			get { return GetEqualityComparison(EqualityComparer<T>.Default); }
		}

		/// <summary>
		/// Inverses <paramref name="predicate"/> to return inverse return value.
		/// </summary>
		/// <param name="predicate">Predicate.</param>
		/// <returns>Inverse predicate.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="predicate"/> is null.</exception>
		public static Predicate<T> GetInversePredicate(Predicate<T> predicate)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return delegate(T item) { return !predicate(item); };
		}

		/// <summary>
		/// Converts <paramref name="comparer"/> to comparison type.
		/// </summary>
		/// <param name="comparer">Comparer. If null, <see cref="Comparer{}.Default"/> is used.</param>
		/// <returns>Comparison.</returns>
		public static Comparison<T> GetComparison(Comparer<T> comparer)
		{
			if (comparer == null)
			{
				comparer = Comparer<T>.Default;
			}
			return delegate(T x, T y) { return comparer.Compare(x, y); };
		}

		/// <summary>
		/// Converts <paramref name="comparison"/> to comparison with opposite direction order.
		/// </summary>
		/// <param name="comparison">Ascending comparison.</param>
		/// <param name="direction">Sort direction.</param>
		/// <returns>
		/// If <paramref name="direction"/> is <see cref="ListSortDirection.Descending"/>, <see cref="GetDescendingComparison"/> is used, otherwise <paramref name="comparison"/> is returned directly.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="comparison"/> is null.</exception>
		public static Comparison<T> GetComparison(Comparison<T> comparison,
												  ListSortDirection direction)
		{
			if (comparison == null)
			{
				throw new ArgumentNullException("comparison");
			}
			if (direction == ListSortDirection.Descending)
			{
				comparison = GetDescendingComparison(comparison);
			}
			return comparison;
		}

		/// <summary>
		/// Converts <paramref name="comparison"/> to comparison with opposite direction order.
		/// </summary>
		/// <param name="comparison">Ascending comparison.</param>
		/// <returns>Descending comparison.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="comparison"/> is null.</exception>
		public static Comparison<T> GetDescendingComparison(Comparison<T> comparison)
		{
			if (comparison == null)
			{
				throw new ArgumentNullException("comparison");
			}
			return delegate(T x, T y) { return -comparison(x, y); };
		}

		/// <summary>
		/// Gets property comparison with opposite sort direction than <paramref name="comparison"/>.
		/// </summary>
		/// <param name="comparison">Ascending comparison.</param>
		/// <returns>Descending comparison.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="comparison"/> is null.</exception>
		public static PropertyComparison<T> GetDescendingPropertyComparison(
				PropertyComparison<T> comparison)
		{
			if (comparison == null)
			{
				throw new ArgumentNullException("comparison");
			}
			return
					delegate(T x, T y, PropertyDescriptor property) { return -comparison(x, y, property); };
		}

		/// <summary>
		/// Converts <paramref name="comparison"/> to comparison with opposite direction order.
		/// </summary>
		/// <param name="comparison">Ascending comparison.</param>
		/// <param name="direction">Sort direction.</param>
		/// <returns>
		/// If <paramref name="direction"/> is <see cref="ListSortDirection.Descending"/>, <see cref="GetDescendingPropertyComparison"/> is used, otherwise <paramref name="comparison"/> is returned directly.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="comparison"/> is null.</exception>
		public static PropertyComparison<T> GetPropertyComparison(PropertyComparison<T> comparison,
																  ListSortDirection direction)
		{
			if (comparison == null)
			{
				throw new ArgumentNullException("comparison");
			}
			if (direction == ListSortDirection.Descending)
			{
				comparison = GetDescendingPropertyComparison(comparison);
			}
			return comparison;
		}

		/// <summary>
		/// Gets equality comparison from comparison, which returns true, if comparison returns 0.
		/// </summary>
		/// <param name="comparison">Comparison.</param>
		/// <returns>Equaler.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="comparison"/> is null.</exception>
		public static EqualityComparison<T> GetEqualityComparison(Comparison<T> comparison)
		{
			if (comparison == null)
			{
				throw new ArgumentNullException("comparison");
			}
			return delegate(T x, T y) { return comparison(x, y) == 0; };
		}

		/// <summary>
		/// Gets equality comparison from comparer.
		/// </summary>
		/// <param name="comparer">Comparer.</param>
		/// <returns>Equaler.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception>
		public static EqualityComparison<T> GetEqualityComparison(EqualityComparer<T> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}
			return delegate(T x, T y) { return comparer.Equals(x, y); };
		}
	}

	/// <summary>
	/// Used for finding an item by its property value.
	/// </summary>
	/// <typeparam name="T">Type of item.</typeparam>
	/// <param name="item">Item.</param>
	/// <param name="property">Item property to find.</param>
	/// <param name="key">Item property value to find.</param>
	/// <returns>true, if item has the specified property value.</returns>
	public delegate bool PropertyPredicate<T>(T item, PropertyDescriptor property, object key);

	/// <summary>
	/// Used for sorting items by their property.
	/// </summary>
	/// <typeparam name="T">Type of item.</typeparam>
	/// <param name="item1">First item.</param>
	/// <param name="item2">Second item.</param>
	/// <param name="property">Item property to compare.</param>
	/// <returns>
	/// 0, if <paramref name="item1"/> and <paramref name="item2"/> are the same.
	/// -1, if <paramref name="item1"/> is smaller than <paramref name="item2"/>.
	/// 1, if <paramref name="item1"/> is bigger than <paramref name="item2"/>.
	/// </returns>
	public delegate int PropertyComparison<T>(T item1, T item2, PropertyDescriptor property);

	/// <summary>
	/// Compares items whether they are equal or not.
	/// </summary>
	/// <typeparam name="T">Type of item.</typeparam>
	/// <param name="x">First item.</param>
	/// <param name="y">Second item.</param>
	/// <returns>true, if items are equal, otherwise false.</returns>
	public delegate bool EqualityComparison<T>(T x, T y);
}