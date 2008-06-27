using System;
using System.Collections;
using System.Collections.Generic;

namespace WeSay.Data
{
	public static class Permuter
	{
		// if we start with: {}      and add : a we get {a}
		// if we start with: {a}     and add : b we get {a,b}
		// if we start with: {a,b}   and add : c we get {a,b,c}
		// if we start with: {a},{b} and add : c we get {a,c}, {b,c}
		public static void Permute<T>(List<Dictionary<string, T>> results, string key, T newItem)
		{
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			if (results.Count == 0)
			{
				results.Add(new Dictionary<string, T>());
			}
			foreach (Dictionary<string, T> row in results)
			{
				row.Add(key, newItem);
			}
		}

		// if we start with: {}      and add : {a,b} we get {a},{b}
		// if we start with: {a}     and add : {b,c} we get {a,b}, {a,c}
		// if we start with: {a,b}   and add : {c,d} we get {a,b,c}, {a,b,d}
		// if we start with: {a},{b} and add : {c,d} we get {a,c}, {a,d}, {b,c}, {b,d}
		public static void Permute<T>(List<Dictionary<string, T>> results, string key, IEnumerable newItems)
		{
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			if (newItems == null)
			{
				throw new ArgumentNullException("newItems");
			}
			if(newItems.GetEnumerator().MoveNext() == false)
			{
				//we have no items to merge in
				return;
			}
			Dictionary<string, T>[] original = results.ToArray();
			results.Clear();

			if(original.Length == 0)
			{
				// we have to have at least one item if we are going to do the
				// foreach loop below
				original = new Dictionary<string, T>[] { new Dictionary<string, T>() };
			}
			foreach (Dictionary<string, T> row in original)
			{
				foreach (object newItem in newItems)
				{
					// duplicate the original results for each new item
					Dictionary<string, T> columns = new Dictionary<string, T>(row);
					columns.Add(key, (T)newItem);
					results.Add(columns);
				}
			}
		}


		// if we start with: {}      and add : {a},{b} we get {a},{b}
		// if we start with: {a}     and add : {b},{c} we get {a,b}, {a,c}
		// if we start with: {a,b}   and add : {c},{d} we get {a,b,c}, {a,b,d}
		// if we start with: {a},{b} and add : {c},{d} we get {a,c}, {a,d}, {b,c}, {b,d}
		// if we start with: {}      and add : {a,b},{c,d} we get {a,b},{c,d}
		// if we start with: {a}     and add : {b,c},{d,e} we get {a,b,c}, {a,d,e}
		// if we start with: {a,b}   and add : {c,d},{e,f} we get {a,b,c,d}, {a,b,e,f}
		// if we start with: {a},{b} and add : {c,d},{e,f} we get {a,c,d}, {a,e,f}, {b,c,d}, {b,e,f}
		public static void Permute<T>(List<Dictionary<string, T>> results, IEnumerable<Dictionary<string, T>> newItems)
		{
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			if (newItems == null)
			{
				throw new ArgumentNullException("newItems");
			}
			if (newItems.GetEnumerator().MoveNext() == false)
			{
				//we have no items to merge in
				return;
			}
			Dictionary<string, T>[] original = results.ToArray();
			results.Clear();

			if (original.Length == 0)
			{
				// we have to have at least one item if we are going to do the
				// foreach loop below
				original = new Dictionary<string, T>[] { new Dictionary<string, T>() };
			}
			foreach (Dictionary<string, T> row in original)
			{
				foreach (Dictionary<string, T> newItem in newItems)
				{
					// duplicate the original results for each new item
					Dictionary<string, T> columns = new Dictionary<string, T>(row);
					foreach (KeyValuePair<string, T> pair in newItem)
					{
						columns.Add(pair.Key, pair.Value);
					}
					results.Add(columns);
				}
			}
		}
	}
}
