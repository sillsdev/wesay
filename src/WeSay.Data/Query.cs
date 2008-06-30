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
		sealed private class ForEachQuery : Query
		{
			private readonly MethodInfo _property;
			public ForEachQuery(Query root, MethodInfo p, Type iEnumerableReturnType)
				: base(root, iEnumerableReturnType)
			{
				_property = p;
			}

			protected override void GetResultsCore(List<Dictionary<string, object>> results, object o)
			{
				object value = this._property.Invoke(o,null);
				if (value != null)
				{
					List<Dictionary<string, object>> oneResultPerRow = new List<Dictionary<string, object>>();
					foreach (Object item in (IEnumerable)value)
					{
						List<Dictionary<string, object>> subresults = new List<Dictionary<string, object>>();
						base.GetResultsCore(subresults, item);
						oneResultPerRow.AddRange(subresults);
					}
					Permuter.Permute(results, oneResultPerRow);
				}
			}
		}

		sealed private class InQuery:Query
		{
			private readonly MethodInfo _property;
			public InQuery(Query root, MethodInfo p):base(root, p.ReturnType)
			{
				_property = p;
			}

			protected override void GetResultsCore(List<Dictionary<string, object>> results, object o)
			{
				object value = this._property.Invoke(o, null);
				if (value != null)
				{
					base.GetResultsCore(results, value);
				}
			}

			protected override MethodInfo GetMethodInfo(string name)
			{
				PropertyInfo property = this._property.ReturnType.GetProperty(name);
				if(property == null)
				{
					throw new ArgumentOutOfRangeException("name", name, "There is no property with the given name");
				}
				return property.GetGetMethod();
			}


			protected override string TypeName
			{
				get
				{
					return this._property.Name;
				}
			}
		}
		private List<Query> _nestedQueries;
		private List<KeyValuePair<string, MethodInfo>> _resultProperties;
		private readonly Type _t;
		private List<string> _labelRegistry;

		protected virtual void GetResultsCore(List<Dictionary<string, object>> result, object o)
		{
			PermuteResultsAtThisLevel(result, o);
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
			if (this._resultProperties != null)
			{
				foreach (KeyValuePair<string, MethodInfo> pair in _resultProperties)
				{
					MethodInfo getProperty = pair.Value;
					object result = getProperty.Invoke(o, null);
					IEnumerable enumerableResult = result as IEnumerable;
					if(enumerableResult == null || result is string)
					{
						Permuter.Permute(results, pair.Key, result);
					}
					else
					{
						Permuter.Permute(results, pair.Key, enumerableResult);
					}
				}
			}
		}

		public IEnumerable<Dictionary<string, object>> GetResults(object o)
		{
			if(o == null)
			{
				throw new ArgumentNullException();
			}
			if(_t == null)
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

		private Query GetRoot() {
			Query root = this;
			if (this._root != null)
			{
				root = this._root;
			}
			return root;
		}

		private readonly Query _root;
		internal Query(Query root, Type t):this(t)
		{
			if (root == null)
			{
				throw new ArgumentNullException("root");
			}
			_root = root;
		}

		public Query(Type t)
		{
			if (t == null)
			{
				throw new ArgumentNullException("t");
			}
			_root = null;
			_t = t;
			_labelRegistry = null;
		}

		protected virtual string TypeName
		{
			get { return _t.Name; }
		}

		public Query In(string fieldName)
		{
			InQuery q = new InQuery(this._root??this, GetMethodInfo(fieldName));
			NestedQueries.Add(q);
			return q;
		}

		public Query ForEach(string fieldName)
		{
			MethodInfo mi = GetMethodInfo(fieldName);
			Type iEnumerableReturnType = GetIEnumerableReturnType(mi);
			if (iEnumerableReturnType == null)
			{
				throw new ArgumentOutOfRangeException("fieldName", fieldName, "Does not implement IEnumerable<T>");
			}

			ForEachQuery q = new ForEachQuery(this._root ?? this, mi, iEnumerableReturnType);

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
				throw new ArgumentOutOfRangeException("label", label, "Label has already been used in Query");
			}
			root._labelRegistry.Add(label);
		}

		public Query Show(string fieldName)
		{
			return Show(fieldName, fieldName);
		}

		public Query Show(string fieldName, string label)
		{
			MethodInfo mi = GetMethodInfo(fieldName);
			if (GetIEnumerableReturnType(mi) != null)
			{
				throw new ArgumentOutOfRangeException("fieldName", fieldName, "Property implements IEnumerable<T>; use ShowEach instead");
			}
			RegisterLabel(label);

			ResultProperties.Add(new KeyValuePair<string, MethodInfo>(label, mi));
			return this;
		}

		public Query ShowEach(string fieldName)
		{
			return ShowEach(fieldName, fieldName);
		}

		public Query ShowEach(string fieldName, string label)
		{
			MethodInfo mi = GetMethodInfo(fieldName);
			if (GetIEnumerableReturnType(mi) == null)
			{
				throw new ArgumentOutOfRangeException("fieldName", fieldName, "Property does not implement IEnumerable<T>; use Show instead");
			}
			RegisterLabel(label);

			ResultProperties.Add(new KeyValuePair<string, MethodInfo>(label, mi));
			return this;
		}

		private List<KeyValuePair<string, MethodInfo>> ResultProperties
		{
			get
			{
				if (_resultProperties == null)
				{
					_resultProperties = new List<KeyValuePair<string, MethodInfo>>();
				}
				return _resultProperties;
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
			if(property == null)
			{
				throw new ArgumentOutOfRangeException("name", name, "There is no property with the given name");
			}
			MethodInfo mi = property.GetGetMethod();
			if(mi == null)
			{
				throw new ArgumentOutOfRangeException("name", name, "The given property does not have a public getter.");
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
			if (_resultProperties != null)
			{
				foreach (KeyValuePair<string, MethodInfo> property in _resultProperties)
				{
					MethodInfo result = property.Value;
					sb.Append(" Show ");
					sb.Append(result.Name);
				}
			}

			sb.Append("] ");
			return sb.ToString();
		}

		private static Type GetIEnumerableReturnType(MethodInfo mi)
		{
			Debug.Assert(mi != null);
			// if our return type is string we want
			// to act on the string not the IEnumerable<char>
			Type returnType = mi.ReturnType;
			if(returnType == typeof(string))
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
					if (interfaceType == typeof(IEnumerable<>).MakeGenericType(arguments))
					{
						type = arguments[0];
					}
				}
			}
			return type;
		}
	}
}
