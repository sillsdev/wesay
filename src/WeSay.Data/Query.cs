using System;

namespace WeSay.Data
{
	   public enum Operation
		{
			Contains,
			DoesNotContain,
			Equals,
			NotEqual,
			GreaterThan,
			GreaterThanOrEqual,
			LessThan,
			LessThanOrEqual,
			Like,
			And,
			Or
		}

		public enum Order
		{
			Unsorted,
			Ascending,
			Descending
		}

	public class Criteria
	{
		private string _fieldName;
		private Operation _operation;
		private string _value;
		private Query _criteria;
	}
	public class Query
	{
		public Query(string className)
		{
		}

		public void AddCriterion(string fieldName, Operation operation, Query value)
		{

		}

		public void AddCriterion(string fieldName, Operation operation, string value)
		{

		}

		public void AddResult(string fieldName, Order sorted)
		{
		}
	}
}
