using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace WeSay.Data
{
	public class Query
	{
		[Obsolete]
		public sealed class PredicateQuery<T>: Query
		{
			private readonly Predicate<T> _predicate;

			public PredicateQuery(Predicate<T> predicate): base(typeof (T))
			{
				_predicate = predicate;
			}

			protected override void GetResultsCore(List<Dictionary<string, object>> results, object o)
			{
				bool matches = _predicate((T) o);
				Dictionary<string, object> dict = new Dictionary<string, object>();
				dict["Matches"] = matches;
				results.Add(dict);
				GetNestedQueryResults(results, o);
			}
		}

		private sealed class ForEachQuery: Query
		{
			private readonly MethodInfo _property;

			public ForEachQuery(Query root, MethodInfo p, Type iEnumerableReturnType)
					: base(root, iEnumerableReturnType)
			{
				_property = p;
			}

			protected override void GetResultsCore(List<Dictionary<string, object>> results,
												   object o)
			{
				object value = _property.Invoke(o, null);
				if (value != null)
				{
					List<Dictionary<string, object>> oneResultPerRow =
							new List<Dictionary<string, object>>();
					foreach (Object item in (IEnumerable) value)
					{
						List<Dictionary<string, object>> subresults =
								new List<Dictionary<string, object>>();
						if (WhereFilterDoesNotApply(item))
						{
							PermuteResultsAtThisLevel(subresults, item);
							GetNestedQueryResults(subresults, item);
							oneResultPerRow.AddRange(subresults);
						}
					}
					if (oneResultPerRow.Count == 0 && _requireAtLeastOneResult)
					{
						PermuteEmptyResult(results);
					}
					else
					{
						Permuter.Permute(results, oneResultPerRow);
					}
				}
			}
		}

		private sealed class InQuery: Query
		{
			private readonly MethodInfo _property;

			public InQuery(Query root, MethodInfo p): base(root, p.ReturnType)
			{
				_property = p;
			}

			protected override void GetResultsCore(List<Dictionary<string, object>> results,
												   object o)
			{
				object value = this._property.Invoke(o, null);
				if (value != null)
				{
					base.GetResultsCore(results, value);
				}
			}

			protected override MethodInfo GetMethodInfo(string name)
			{
				Type returnType = this._property.ReturnType;
				PropertyInfo property = returnType.GetProperty(name);
				if (property == null)
				{
					throw new ArgumentOutOfRangeException("name",
														  name,
														  String.Format(
																  "There is no property in class {0} with the given name",
																  returnType.Name));
				}
				return property.GetGetMethod();
			}

			protected override string TypeName
			{
				get { return this._property.Name; }
			}
		}

		public delegate bool Functor(IDictionary<string, object> data);
		private sealed class FieldProperties
		{
			private readonly MethodInfo _methodInfo;
			private readonly bool _isEnumerable;

			public MethodInfo Method
			{
				get { return _methodInfo; }
			}

			public bool IsEnumerable
			{
				get { return _isEnumerable; }
			}

			public FieldProperties(MethodInfo methodInfo, bool isEnumerable)
			{
				_methodInfo = methodInfo;
				_isEnumerable = isEnumerable;
			}

		}

		private List<Query> _nestedQueries;

		private Dictionary<string, FieldProperties> _showFieldProperties;

		Dictionary<string, FieldProperties> _whereFieldProperties;
		private Functor _whereCondition;

		private readonly Type _t;
		private List<string> _labelRegistry;

		protected virtual void GetResultsCore(List<Dictionary<string, object>> results, object o)
		{
			bool haveResult = false;
			if (WhereFilterDoesNotApply(o))
			{
				haveResult = PermuteResultsAtThisLevel(results, o);
				GetNestedQueryResults(results, o);
			}
			if (!haveResult && _requireAtLeastOneResult)
			{
				PermuteEmptyResult(results);
			}
		}

		private bool WhereFilterDoesNotApply(object o)
		{
			bool evaluateResults = true;
			if (_whereCondition != null)
			{
				Dictionary<string, object> data = new Dictionary<string, object>();
				foreach (KeyValuePair<string, FieldProperties> pair in _whereFieldProperties)
				{
					MethodInfo getProperty = pair.Value.Method;
					object propertyValue = getProperty.Invoke(o, null);
					data.Add(pair.Key, propertyValue);
				}
				evaluateResults = _whereCondition.Invoke(data);
			}
			return evaluateResults;
		}

		protected void GetNestedQueryResults(List<Dictionary<string, object>> result, object o)
		{
			if (_nestedQueries != null)
			{
				foreach (Query query in _nestedQueries)
				{
					query.GetResultsCore(result, o);
				}
			}
		}

		private bool PermuteResultsAtThisLevel(List<Dictionary<string, object>> results, object o)
		{
			Debug.Assert(o != null);
			// see if we have any results that we should return as a result
			bool haveAtLeastOneResult = false;
			if (_showFieldProperties != null)
			{

					foreach (KeyValuePair<string, FieldProperties> pair in _showFieldProperties)
					{
						haveAtLeastOneResult = true;
						MethodInfo getProperty = pair.Value.Method;
						object propertyValue = getProperty.Invoke(o, null);
						if (pair.Value.IsEnumerable)
						{
							IEnumerable enumerableResult = propertyValue as IEnumerable;
							Permuter.Permute(results, pair.Key, enumerableResult);
						}
						else
						{
							Permuter.Permute(results, pair.Key, propertyValue);
						}
					}
			}
			return haveAtLeastOneResult;
		}

		private void PermuteEmptyResult(List<Dictionary<string, object>> results)
		{
			if (_showFieldProperties != null && _showFieldProperties.Count > 0)
			{
				foreach (KeyValuePair<string, FieldProperties> pair in _showFieldProperties)
				{
					Permuter.Permute(results, pair.Key, (object)null);
				}
			}
			else
			{
				Permuter.Permute(results, "", (object)null);
			}
		}

		public IEnumerable<Dictionary<string, object>> GetResults(object o)
		{
			if (o == null)
			{
				throw new ArgumentNullException();
			}
			if (_t == null)
			{
				throw new InvalidOperationException();
			}

			Query root = GetRoot();

			if (!root._t.IsInstanceOfType(o))
			{
				throw new ArgumentOutOfRangeException("o",
													  o,
													  "Argument is type " + o.GetType().Name +
													  " but should be of type " + root._t.Name);
			}

			List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();
			root.GetResultsCore(results, o);
			foreach (Dictionary<string, object> result in results)
			{
				yield return result;
			}
		}

		private Query GetRoot()
		{
			Query root = this;
			if (this._root != null)
			{
				root = this._root;
			}
			return root;
		}

		private readonly Query _root;
		private bool _requireAtLeastOneResult;

		internal Query(Query root, Type t): this(t)
		{
			if (root == null)
			{
				throw new ArgumentNullException("root");
			}
			_root = root;
			_requireAtLeastOneResult = false;
		}

		public Query(Type t)
		{
			if (t == null)
			{
				throw new ArgumentNullException("t");
			}
			_root = null;

			_showFieldProperties = null;

			_whereFieldProperties = null;
			_whereCondition = null;

			_requireAtLeastOneResult = false;
			_t = t;
			_labelRegistry = null;
		}

		protected virtual string TypeName
		{
			get { return _t.Name; }
		}

		public Query In(string fieldName)
		{
			InQuery q = new InQuery(this._root ?? this, GetMethodInfo(fieldName));
			NestedQueries.Add(q);
			return q;
		}

		public Query ForEach(string fieldName)
		{
			MethodInfo methodInfo = GetMethodInfo(fieldName);
			Type iEnumerableReturnType = GetIEnumerableReturnType(methodInfo);
			if (iEnumerableReturnType == null)
			{
				throw new ArgumentOutOfRangeException("fieldName",
													  fieldName,
													  "Does not implement IEnumerable<T>");
			}

			ForEachQuery q = new ForEachQuery(_root ?? this, methodInfo, iEnumerableReturnType);

			NestedQueries.Add(q);
			return q;
		}

		private void RegisterLabel(string label)
		{
			// Register this label globally with the root query
			Query root = GetRoot();
			if (root._labelRegistry == null)
			{
				root._labelRegistry = new List<string>();
			}
			if (root._labelRegistry.Contains(label))
			{
				throw new ArgumentOutOfRangeException("label",
													  label,
													  "Label has already been used in Query");
			}
			root._labelRegistry.Add(label);
		}

		public Query AtLeastOne()
		{
			_requireAtLeastOneResult = true;
			return this;
		}

		public Query Where(string fieldName, Functor whereCondition)
		{
			return Where(new string[] {fieldName}, whereCondition);
		}

		public Query Where(string[] fieldNames, Functor whereCondition)
		{
			if (_whereCondition != null)
			{
				throw new Exception("You may only use a single where condition per local query.");
			}
			_whereCondition = whereCondition;
			foreach (string fieldName in fieldNames)
			{
				MethodInfo methodInfo = GetMethodInfo(fieldName);
				bool isEnumerable = ReturnTypeIsEnumerable(methodInfo);
				WhereFieldProperties.Add(fieldName, new FieldProperties(methodInfo, isEnumerable));
			}
			return this;
		}

		public Query Show(string fieldName)
		{
			return Show(fieldName, fieldName);
		}

		public Query Show(string fieldName, string label)
		{
			MethodInfo methodInfo = GetMethodInfo(fieldName);
			bool isEnumerable = ReturnTypeIsEnumerable(methodInfo);
			if (isEnumerable)
			{
				throw new ArgumentOutOfRangeException("fieldName",
													  fieldName,
													  "Property implements IEnumerable<T>; use ShowEach instead");
			}
			RegisterLabel(label);
			ShowFieldProperties.Add(label, new FieldProperties(methodInfo, isEnumerable));
			return this;
		}

		public Query ShowEach(string fieldName)
		{
			return ShowEach(fieldName, fieldName);
		}

		public Query ShowEach(string fieldName, string label)
		{
			MethodInfo methodInfo = GetMethodInfo(fieldName);
			bool isEnumerable = ReturnTypeIsEnumerable(methodInfo);
			if (!isEnumerable)
			{
				throw new ArgumentOutOfRangeException("fieldName",
													  fieldName,
													  "Property does not implement IEnumerable<T>; use Show instead");
			}
			RegisterLabel(label);

			ShowFieldProperties.Add(label, new FieldProperties(methodInfo, isEnumerable));
			return this;
		}

		private Dictionary<string, FieldProperties> ShowFieldProperties
		{
			get
			{
				if (_showFieldProperties == null)
				{
					_showFieldProperties = new Dictionary<string, FieldProperties>();
				}
				return _showFieldProperties;
			}
		}

		private List<Query> NestedQueries
		{
			get
			{
				if (this._nestedQueries == null)
				{
					this._nestedQueries = new List<Query>();
				}
				return _nestedQueries;
			}
		}

		private Dictionary<string, FieldProperties> WhereFieldProperties
		{
			get
			{
				if (_whereFieldProperties == null)
				{
					_whereFieldProperties = new Dictionary<string, FieldProperties>();
				}
				return _whereFieldProperties;
			}
		}

		protected virtual MethodInfo GetMethodInfo(string name)
		{
			PropertyInfo property = this._t.GetProperty(name);
			if (property == null)
			{
				throw new ArgumentOutOfRangeException("name",
													  name,
													  "There is no property with the given name");
			}
			MethodInfo mi = property.GetGetMethod();
			if (mi == null)
			{
				throw new ArgumentOutOfRangeException("name",
													  name,
													  "The given property does not have a public getter.");
			}
			return mi;
		}

		// name suitable for an identifier
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (_root == null)
			{
				sb.Append("Query ");
			}
			sb.Append('[');
			sb.Append(TypeName);
			sb.Append(' ');
			if (_nestedQueries != null)
			{
				foreach (Query query in _nestedQueries)
				{
					sb.Append(query.ToString());
				}
			}
			if (_showFieldProperties != null)
			{
				foreach (KeyValuePair<string, FieldProperties> pair in _showFieldProperties)
				{
					if (_requireAtLeastOneResult)
					{
						sb.Append(" AtLeastOne ");
					}
					MethodInfo result = pair.Value.Method;
					sb.Append(" Show ");
					sb.Append(result.Name);
				}
			}
			if (_whereCondition != null)
			{
				sb.Append(" Where <condition> ");
			}

			sb.Append("] ");
			return sb.ToString();
		}

		private static bool ReturnTypeIsEnumerable(MethodInfo methodInfo)
		{
			return GetIEnumerableReturnType(methodInfo) != null;
		}

		private static Type GetIEnumerableReturnType(MethodInfo methodInfo)
		{
			Debug.Assert(methodInfo != null);
			// if our return type is string we want
			// to act on the string not the IEnumerable<char>
			Type returnType = methodInfo.ReturnType;
			if (returnType == typeof (string))
			{
				return null;
			}
			Type type = null;
			Type[] interfaces = returnType.GetInterfaces();
			foreach (Type interfaceType in interfaces)
			{
				if (interfaceType.IsGenericType)
				{
					Type[] arguments = interfaceType.GetGenericArguments();
					if (interfaceType == typeof (IEnumerable<>).MakeGenericType(arguments))
					{
						type = arguments[0];
					}
				}
			}
				return type;
		}

	}
}