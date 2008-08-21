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

			protected override void GetResultsCore(List<Dictionary<string, object>> result, object o)
			{
				bool matches = _predicate((T) o);
				Dictionary<string, object> dict = new Dictionary<string, object>();
				dict["Matches"] = matches;
				result.Add(dict);
				GetNestedQueryResults(result, o);
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
				object value = this._property.Invoke(o, null);
				if (value != null)
				{
					List<Dictionary<string, object>> oneResultPerRow =
							new List<Dictionary<string, object>>();
					foreach (Object item in (IEnumerable) value)
					{
						List<Dictionary<string, object>> subresults =
								new List<Dictionary<string, object>>();
						base.GetResultsCore(subresults, item);
						oneResultPerRow.AddRange(subresults);
					}
					Permuter.Permute(results, oneResultPerRow);
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

		public delegate bool Functor(object o);
		private sealed class FieldInfo
		{
			private readonly MethodInfo _methodInfo;
			private readonly bool _isEnumerable;
			private Functor _whereCondition;

			public MethodInfo Method
			{
				get { return _methodInfo; }
			}

			public bool IsEnumerable
			{
				get { return _isEnumerable; }
			}

			public Functor WhereCondition
			{
				get { return _whereCondition; }
				set { _whereCondition = value; }
			}

			public FieldInfo(MethodInfo methodInfo, bool isEnumerable)
			{
				_methodInfo = methodInfo;
				_isEnumerable = isEnumerable;
				_whereCondition = null;
			}
		}

		private List<Query> _nestedQueries;
		private Dictionary<string, FieldInfo> _fieldProperties;
		private readonly Type _t;
		private List<string> _labelRegistry;

		protected virtual void GetResultsCore(List<Dictionary<string, object>> result, object o)
		{
			PermuteResultsAtThisLevel(result, o);
			GetNestedQueryResults(result, o);
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

		private void PermuteResultsAtThisLevel(List<Dictionary<string, object>> results, object o)
		{
			Debug.Assert(o != null);
			// see if we have any results that we should return as a result
			if (_fieldProperties != null)
			{
				foreach (KeyValuePair<string, FieldInfo> pair in _fieldProperties)
				{
					MethodInfo getProperty = pair.Value.Method;
					object propertyValue = getProperty.Invoke(o, null);
					Functor whereCondition = pair.Value.WhereCondition;
					bool evaluateProperty = true;
					if (whereCondition != null)
					{
						evaluateProperty = whereCondition.Invoke(propertyValue);
					}
					if (evaluateProperty)
					{
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
				if (results.Count == 0 && _atLeastOne)
				{
					foreach (KeyValuePair<string, FieldInfo> pair in _fieldProperties)
					{
						Permuter.Permute(results, pair.Key, "");
					}
					if (_fieldProperties.Count == 0)
					{
						Permuter.Permute(results, "", "");
					}
				}
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
		private bool _atLeastOne;

		internal Query(Query root, Type t): this(t)
		{
			if (root == null)
			{
				throw new ArgumentNullException("root");
			}
			_root = root;
			_atLeastOne = false;
		}

		public Query(Type t)
		{
			if (t == null)
			{
				throw new ArgumentNullException("t");
			}
			_root = null;
			_atLeastOne = false;
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

			FieldProperties.Add(label, new FieldInfo(methodInfo, isEnumerable));
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
			if (isEnumerable)
			{
				throw new ArgumentOutOfRangeException("fieldName",
													  fieldName,
													  "Property does not implement IEnumerable<T>; use Show instead");
			}
			RegisterLabel(label);

			FieldProperties.Add(label, new FieldInfo(methodInfo, isEnumerable));
			return this;
		}

		private Dictionary<string, FieldInfo> FieldProperties
		{
			get
			{
				if (_fieldProperties == null)
				{
					_fieldProperties = new Dictionary<string, FieldInfo>();
				}
				return _fieldProperties;
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
			if (_fieldProperties != null)
			{
				foreach (KeyValuePair<string, FieldInfo> pair in _fieldProperties)
				{
					if (pair.Value.WhereCondition != null)
					{
						sb.Append(" Where <condition> ");
					}
					if (_atLeastOne)
					{
						sb.Append(" AtLeastOne ");
					}
					MethodInfo result = pair.Value.Method;
					sb.Append(" Show ");
					sb.Append(result.Name);
				}
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

		public Query AtLeastOne()
		{
			_atLeastOne = true;
			return this;
		}

		public Query Where(string label, Functor whereCondition)
		{
			_fieldProperties[label].WhereCondition = whereCondition;
			return this;
		}
	}
}