using System.Collections.Generic;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;

namespace WeSay.Data.Tests
{
	public class SimpleIntSortHelper: ISortHelper<SimpleIntTestClass>
		{
		private LexEntryRepository _recordListManager;

		public SimpleIntSortHelper(LexEntryRepository recordListManager)
		{
			_recordListManager = recordListManager;
		}


		#region ISortHelper<Key,Value,long> Members

			public IComparer<string> KeyComparer
			{
				get
				{
					return Comparer<string>.Default;
				}
			}

			public List<RecordToken> GetRecordTokensForMatchingRecords()
			{
				List<RecordToken> keyIdPairs = new List<RecordToken>();

				if (_recordListManager is LexEntryRepository)
				{
					Db4oDataSource db4oData = ((LexEntryRepository)_recordListManager).DataSource;
					IExtObjectContainer database = db4oData.Data.Ext();

					IQuery query = database.Query();
					query.Constrain(typeof(SimpleIntTestClass));
					IObjectSet resultList = query.Execute();


					foreach (SimpleIntTestClass simpleIntTestClass in resultList)
					{
						keyIdPairs.Add(new RecordToken(simpleIntTestClass.I.ToString(),
							new Db4oRepositoryId(database.GetID(simpleIntTestClass))));
					}

				}
				else
				{
					foreach (SimpleIntTestClass testClass in _recordListManager.GetListOfType<SimpleIntTestClass>())
					{
						keyIdPairs.Add(new RecordToken(testClass.I.ToString(),
							new Db4oRepositoryId(0L)));
					}
				}
				return keyIdPairs;
			}

			public IEnumerable<string> GetDisplayStrings(SimpleIntTestClass item)
			{
				List<string> result = new List<string>();
				result.Add(item.I.ToString());
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
