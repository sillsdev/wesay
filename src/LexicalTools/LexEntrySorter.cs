using System;
using System.Collections.Generic;
using System.Globalization;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	class LexicalFormSorter
	{
		ViewTemplate _viewTemplate;
		public LexicalFormSorter (ViewTemplate viewTemplate)
		{
			_viewTemplate = viewTemplate;
		}

		Db4oRecordListManager _recordManager;

		private List<KeyValuePair<string, long>> Initialize(Db4oRecordListManager recordManager)
		{
			Db4oDataSource db4oData = _recordManager.DataSource;
			IExtObjectContainer database = db4oData.Data.Ext();

			List<Type> OriginalList = Db4oLexModelHelper.Singleton.DoNotActivateTypes;
			Db4oLexModelHelper.Singleton.DoNotActivateTypes = new List<Type>();
			Db4oLexModelHelper.Singleton.DoNotActivateTypes.Add(typeof(LexEntry));

			IQuery query = database.Query();
			query.Constrain(typeof(LexicalFormMultiText));
			IObjectSet lexicalForms = query.Execute();

			List<KeyValuePair<string, long>> result = new List<KeyValuePair<string, long>>();

			foreach (LexicalFormMultiText lexicalForm in lexicalForms)
			{
				query = database.Query();
				query.Constrain(typeof(LexEntry));
				query.Descend("_lexicalForm").Constrain(lexicalForm).Identity();
				long[] ids = query.Execute().Ext().GetIDs();

				//// If LexEntry does not cascade delete its lexicalForm then we could have a case where we
				//// don't have a entry associated with this lexicalForm.
				if (ids.Length == 0)
				{
					continue;
				}

				result.Add(new KeyValuePair<string, long>(lexicalForm.ToString(), ids[0]));
			}

			Db4oLexModelHelper.Singleton.DoNotActivateTypes = OriginalList;

			return result;
		}

		public CachedSortedDb4oList<string, LexEntry>.Initializer Initializer
		{
			get {
				return Initialize;
			}
		}

		public IComparer<string> KeyComparer()
			get
		{
			StringComparer comparer;
			try
			{
				string WritingSystemId = _viewTemplate.GetField("EntryLexicalForm").WritingSystemIds[0];
				comparer = StringComparer.Create(CultureInfo.GetCultureInfo(WritingSystemId), false);
			}
			catch
			{
				comparer = StringComparer.InvariantCulture;
			}
	return comparer;
		}


		public string KeyProvider(LexEntry entry)
		{
			return entry.LexicalForm.GetFirstAlternative();
		}

	}
}

	}
}
