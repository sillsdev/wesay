using System.Collections.Generic;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;

namespace WeSay.Data.Tests
{
	public class SimpleIntSortHelper: ISortHelper<int, SimpleIntTestClass>
		{
		private IRecordListManager _recordListManager;

		public SimpleIntSortHelper(IRecordListManager recordListManager)
		{
			_recordListManager = recordListManager;
		}


		#region ISortHelper<Key,Value,long> Members

			public IComparer<int> KeyComparer
			{
				get
				{
					return Comparer<int>.Default;
				}
			}

			public List<KeyValuePair<int, long>> GetKeyIdPairs()
			{
				List<KeyValuePair<int, long>> keyIdPairs = new List<KeyValuePair<int, long>>();

				if (_recordListManager is Db4oRecordListManager)
				{
					Db4oDataSource db4oData = ((Db4oRecordListManager)_recordListManager).DataSource;
					IExtObjectContainer database = db4oData.Data.Ext();

					IQuery query = database.Query();
					query.Constrain(typeof(SimpleIntTestClass));
					IObjectSet resultList = query.Execute();


					foreach (SimpleIntTestClass simpleIntTestClass in resultList)
					{
						keyIdPairs.Add(new KeyValuePair<int, long>(simpleIntTestClass.I, database.GetID(simpleIntTestClass)));
					}

				}
				else
				{
					foreach (SimpleIntTestClass testClass in _recordListManager.GetListOfType<SimpleIntTestClass>())
					{
						keyIdPairs.Add(new KeyValuePair<int, long>(testClass.I, 0L));
					}
				}
				return keyIdPairs;
			}

			public IEnumerable<int> GetKeys(SimpleIntTestClass item)
			{
				List<int> result = new List<int>();
				result.Add(item.I);
				return result;
			}

			public string Name
			{
				get
				{
					return "SimpleIntSortHelper";
				}
			}

			#endregion
		}
}
