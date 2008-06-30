using System;
using System.Collections.Generic;

namespace WeSay.Data
{
	public sealed class RecordTokenComparer<T>: IComparer<RecordToken<T>> where T:class, new()
	{
		private readonly IEnumerable<SortDefinition> _sortDefinitions;
		public RecordTokenComparer(IEnumerable<SortDefinition> sortDefinitions)
		{
			if (sortDefinitions == null)
			{
				throw new ArgumentNullException("sortDefinitions");
			}
			if (sortDefinitions.GetEnumerator().MoveNext() == false)
			{
				throw new ArgumentException("sortDefinitions cannot be an empty array");
			}
			_sortDefinitions = sortDefinitions;
		}

		#region IComparer<RecordToken> Members

		public int Compare(RecordToken<T> x, RecordToken<T> y)
		{
			int result = 0;
			foreach (SortDefinition definition in _sortDefinitions)
			{
				object yFieldValue;
				if (y == null)
				{
					yFieldValue = null;
				}
				else if(definition.Field == "RepositoryId")
				{
					yFieldValue = y.Id;
				}
				else
				{
					yFieldValue = y.Results[definition.Field];
				}

				object xFieldValue;
				if (x == null)
				{
					xFieldValue = null;
				}
				else if (definition.Field == "RepositoryId")
				{
					xFieldValue = x.Id;
				}
				else
				{
					xFieldValue = x.Results[definition.Field];
				}
				result = definition.Comparer.Compare(xFieldValue, yFieldValue);
				if(result != 0)
				{
					return result;
				}
			}
			return result;
		}

		#endregion
	} ;
}