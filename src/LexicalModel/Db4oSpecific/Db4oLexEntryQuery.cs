using System;
using System.Collections.Generic;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Data;
using WeSay.Foundation;

namespace WeSay.LexicalModel.Db4oSpecific
{
	internal class Db4oLexEntryQuery: IQuery<LexEntry>
	{
		private readonly LexEntryRepository _lexEntryRepository; // for data
		private readonly WritingSystem _writingSystem;
		private readonly bool _isWritingSystemIdUsedByLexicalForm;

		public Db4oLexEntryQuery(LexEntryRepository lexEntryRepository,
								 WritingSystem writingSystem,
								 bool isWritingSystemIdUsedByLexicalForm)
		{
			if (lexEntryRepository == null)
			{
				throw new ArgumentNullException("lexEntryRepository");
			}
			if (writingSystem == null)
			{
				throw new ArgumentNullException("writingSystem");
			}

			_lexEntryRepository = lexEntryRepository;
			_writingSystem = writingSystem;
			_isWritingSystemIdUsedByLexicalForm = isWritingSystemIdUsedByLexicalForm;
		}

		public IEnumerable<string> GetDisplayStrings(LexEntry item)
		{
			//List<string> keys = new List<string>();
			//using a dictionary here just to prevent duplicate keys
			Dictionary<string, object> keys = new Dictionary<string, object>();

			if (_isWritingSystemIdUsedByLexicalForm)
			{
				string key = item.LexicalForm.GetBestAlternative(_writingSystem.Id, "*");
				if (key == "*")
				{
					key = "(" +
						  StringCatalog.Get("~Empty",
											"This is what shows for a word in a list when the user hasn't yet typed anything in for the word.  Like if you click the 'New Word' button repeatedly.") +
						  ")";
				}
				keys.Add(key, null);
			}
			else
			{
				bool hasSense = false;
				//nb: the logic here relies on gloss being a requirement of having a sense.
				//If definition were allowed instead, we'd want to go to the second clause instead of outputtig
				//empty labels.
				foreach (LexSense sense in item.Senses)
				{
					hasSense = true;

					foreach (string s in
							KeyToEntryIdInitializer.SplitGlossAtSemicolon(sense.Gloss,
																		  _writingSystem.Id))
					{
						if (s != "*" && !keys.ContainsKey(s))
						{
							keys.Add(s, null);
						}
					}
					foreach (string s in
							KeyToEntryIdInitializer.SplitGlossAtSemicolon(sense.Definition,
																		  _writingSystem.Id))
					{
						if (s != "*" && !keys.ContainsKey(s))
						{
							keys.Add(s, null);
						}
					}
				}
				if (!hasSense)
				{
					keys.Add(
							"(" +
							StringCatalog.Get("~No Gloss",
											  "This is what shows if the user is listing words by the glossing language, but the word doesn't have a gloss.") +
							")",
							null);
				}
			}
			return keys.Keys;
		}

		public ResultSet<LexEntry> RetrieveItems()
		{
			if (_lexEntryRepository != null)
			{
				if (_isWritingSystemIdUsedByLexicalForm)
				{
					return
							new ResultSet<LexEntry>(_lexEntryRepository,
													GetAllEntriesSortedByLexicalForm());
				}
				else
				{
					/* this broke when we moved gloss to just be a property (and definition is too)
			  * But we only call it in the unlikely case that this particular index has disapeared,
			  * so the fix can be really slow (this index is set up without respect to particular
			  * tasks being active, unlike some other indices).
			  *
			  * return KeyToEntryIdInitializer.GetGlossToEntryIdPairs(_db4oData,
			  *                                                            _writingSystem.Id);
			  */
					List<RecordToken<LexEntry>> tokens = new List<RecordToken<LexEntry>>();
					foreach (RepositoryId id in _lexEntryRepository.GetAllItems())
					{
						LexEntry entry = _lexEntryRepository.GetItem(id);
						int i = 0;
						foreach (string s in GetDisplayStrings(entry))
						{
							tokens.Add(
									new RecordToken<LexEntry>(_lexEntryRepository, this, i, s, id));
							++i;
						}
					}
					return new ResultSet<LexEntry>(_lexEntryRepository, tokens);
				}
			}
			throw new InvalidOperationException();
		}

		private List<RecordToken<LexEntry>> GetAllEntriesSortedByLexicalForm()
		{
			IExtObjectContainer database = _lexEntryRepository.Db4oDataSource.Data.Ext();

			List<Type> OriginalList = Db4oLexModelHelper.Singleton.DoNotActivateTypes;
			Db4oLexModelHelper.Singleton.DoNotActivateTypes = new List<Type>();
			Db4oLexModelHelper.Singleton.DoNotActivateTypes.Add(typeof (LexEntry));

			IQuery query = database.Query();
			query.Constrain(typeof (LexicalFormMultiText));
			IObjectSet lexicalForms = query.Execute();

			List<RecordToken<LexEntry>> result = new List<RecordToken<LexEntry>>();

			foreach (LexicalFormMultiText lexicalForm in lexicalForms)
			{
				query = database.Query();
				query.Constrain(typeof (LexEntry));
				query.Descend("_lexicalForm").Constrain(lexicalForm).Identity();
				long[] ids = query.Execute().Ext().GetIDs();

				//// If LexEntry does not cascade delete its lexicalForm then we could have a case where we
				//// don't have a entry associated with this lexicalForm.
				if (ids.Length == 0)
				{
					continue;
				}

				string displayString = lexicalForm.GetBestAlternative(_writingSystem.Id, "*");
				if (displayString == "*")
				{
					displayString = "(" +
									StringCatalog.Get("~Empty",
													  "This is what shows for a word in a list when the user hasn't yet typed anything in for the word.  Like if you click the 'New Word' button repeatedly.") +
									")";
				}
				Db4oRepositoryId id = new Db4oRepositoryId(ids[0]);
				int i =
						result.FindAll(
								delegate(RecordToken<LexEntry> match) { return match.Id == id; }).
								Count;
				result.Add(
						new RecordToken<LexEntry>(_lexEntryRepository, this, i, displayString, id));
			}

			Db4oLexModelHelper.Singleton.DoNotActivateTypes = OriginalList;

			result.Sort(new RecordTokenComparer<LexEntry>(_writingSystem));
			return result;
		}
	}
}