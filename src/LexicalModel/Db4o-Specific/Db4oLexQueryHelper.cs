using System;
using System.Collections.Generic;
using Db4objects.Db4o;
using Palaso.Text;
using WeSay.Data;
using WeSay.Foundation;

namespace WeSay.LexicalModel.Db4o_Specific
{
	public class Db4oLexQueryHelper
	{

		//TODO: this is not ws savvy yet

//        [CLSCompliant(false)]
//        static public List<AncestorType> FindObjectsFromLanguageForm<AncestorType, MultiTextType>(IRecordListManager recordManager, string match)
//            where AncestorType : class
//            where MultiTextType : MultiText
//        {
//            if (recordManager is Db4oRecordListManager)
//            {
//                return FindObjectsFromLanguageForm<AncestorType, MultiTextType>(recordManager, match);
//            }
//            else
//            {
//                //                foreach (MultiTextType m in recordManager.GetListOfType<MultiTextType>() )
//                //                {
//                //
//                //                }
//                throw new NotImplementedException("this search is not available for non-db4o sources");
//            }
//        }

		public static List<AncestorType> FindObjectsFromLanguageForm<AncestorType, MultiTextType>(IRecordListManager recordManager, string match)
			where AncestorType : class, new()
			where MultiTextType : MultiText
		{
			Db4oDataSource db = ((Db4oRecordListManager)recordManager).DataSource;

			List<AncestorType> disconnectedGuys = FindDisconnectedObjectsFromLanguageForm<AncestorType, MultiTextType>(db, match);

			//ok, so those objects are going to be the same .net objects that our recordmanager handles.
			//So now we look up each on in the record manager and grab the equivalent there.
			// This is REALLY important to understand.  If you get an existing guy by querying db4o
			// directly, then make a change, that change WILL NOT be visible elsewhere in the program
			// and WILL NOT be saved. Repent!
			List<AncestorType> connectedGuys = new List<AncestorType>();
			foreach (AncestorType guy in disconnectedGuys)
			{
#if !DEBUG
				//try-catch is temporary fix for ws-655
				try
				{
#endif
					connectedGuys.Add(GetManagedObjectFromRawDb4oObject<AncestorType>(recordManager, guy));
#if !DEBUG
				}
				catch(Exception err)
				{
					Palaso.Reporting.Logger.WriteEvent("ERROR (not shown to user) at FindObjectsFromLanguageForm (see ws-655):" + err.Message);
				}
#endif
			}
			return connectedGuys;
		}

		internal static List<AncestorType> FindDisconnectedObjectsFromLanguageForm<AncestorType, MultiTextType>(Db4oDataSource db, string match)
			where AncestorType : class, new()
			where MultiTextType : MultiText
		{
			Db4objects.Db4o.Query.IQuery q = db.Data.Query();
			q.Constrain(typeof(LanguageForm));
			q.Descend("_form").Constrain(match);
			q.Descend("_parent").Constrain(typeof(MultiTextType));
			IObjectSet matches = q.Execute();
			return FindAncestorsOfLanguageForms<AncestorType, MultiTextType>(matches);
		}

		public static T FindObjectFromGuid<T>(Db4oDataSource db, Guid guid)
			where T : class
		{
			Db4objects.Db4o.Query.IQuery q = db.Data.Query();
			q.Constrain(typeof(T));
			q.Descend("_guid").Constrain(guid);
			IObjectSet matches = q.Execute();
			if (matches.Count == 0)
			{
				return null;
			}
			if (matches.Count > 1)
			{
				throw new ApplicationException(String.Format("There were {0} objects found with the guid {1}", matches.Count, guid));
			}
			System.Diagnostics.Debug.Assert(matches[0].GetType() == typeof(T));
			return (T)matches[0];
		}

		public static LexEntry FindFirstEntryMatchingId(Db4oDataSource db, string id)
		{
			Db4objects.Db4o.Query.IQuery q = db.Data.Query();
			q.Constrain(typeof(LexEntry));
			q.Descend("_id").Constrain(id);
			IObjectSet matches = q.Execute();
			if (matches.Count == 0)
			{
				return null;
			}
			if (matches.Count > 1)//review: not sure if we should throw or not
			{
				throw new ApplicationException(String.Format("There were {0} objects found with the id {1}", matches.Count, id));
			}
			System.Diagnostics.Debug.Assert(matches[0].GetType() == typeof(LexEntry));
			return (LexEntry)matches[0];
		}

		static private List<AncestorType> FindAncestorsOfLanguageForms<AncestorType, MultiTextType>(IObjectSet matches)
			where AncestorType : class
			where MultiTextType : MultiText
		{
			List<AncestorType> list = new List<AncestorType>(matches.Count);
			foreach (LanguageForm languageForm in matches)
			{
				MultiTextType multiText = (MultiTextType)languageForm.Parent;

				//walk up the tree until we find a parent of the desired class
				WeSayDataObject parent = multiText.ParentAsObject as WeSayDataObject;
				while ((parent != null) && !(parent is AncestorType))
				{
					parent = parent.Parent;
				}
				if (parent != null)
				{
					list.Add(parent as AncestorType);
				}
			}
			return list;
		}

		/// <summary>
		/// Try to add the sense to a matching entry. If none found, make a new entry with the sense
		/// </summary>
		static public void AddSenseToLexicon(IRecordListManager recordManager, MultiText lexemeForm, LexSense sense)
		{
			//review: the desired semantics of this find are unclear, if we have more than one ws
			IList<LexEntry> entriesWithSameForm = FindObjectsFromLanguageForm<LexEntry, LexicalFormMultiText>(recordManager, lexemeForm.GetFirstAlternative());
			if (entriesWithSameForm.Count == 0)
			{
				LexEntry entry = new LexEntry();
				entry.LexicalForm.MergeIn(lexemeForm);
				entry.Senses.Add(sense);
				recordManager.GetListOfType<LexEntry>().Add(entry);
			}
			else
			{
				LexEntry entry = entriesWithSameForm[0];// GetConnectedEntry(recordManager, entriesWithSameForm[0]);

				foreach (LexSense s in entry.Senses)
				{
					if (sense.Gloss.Forms.Length > 0)
					{
						LanguageForm glossWeAreAdding = sense.Gloss.Forms[0];
						string glossInThisWritingSystem = s.Gloss.GetExactAlternative(glossWeAreAdding.WritingSystemId);
						if (glossInThisWritingSystem == glossWeAreAdding.Form)
						{
							return; //don't add it again
						}
					}
				}
				entry.Senses.Add(sense);

			}
		}

		/// <summary>
		/// This is REALLY important to understand.  If you get an existing guy by querying db4o
		/// directly, then make a change, that change WILL NOT be visible elsewhere in the program
		/// and WILL NOT be saved!
		/// </summary>
		private static LexEntry GetConnectedEntry(IRecordListManager recordManager, LexEntry entryFromRawQuery)
		{
//            IRecordList<LexEntry> list = recordManager.GetListOfType<LexEntry>();
//            int index = list.GetIndexFromId(((Db4oRecordListManager)recordManager).DataSource.Data.Ext().GetID(entryFromRawQuery));
//            return list[index];
			return GetManagedObjectFromRawDb4oObject<LexEntry>(recordManager, entryFromRawQuery);
		}
		private static T GetManagedObjectFromRawDb4oObject<T>(IRecordListManager recordManager, T objectFromRawDb4oQuery) where T : class, new()
		{
			IRecordList<T> list = recordManager.GetListOfType<T>();
			int index = list.GetIndexFromId(((Db4oRecordListManager)recordManager).DataSource.Data.Ext().GetID(objectFromRawDb4oQuery));
			return list[index];
		}


	}

	public class Db4oLexEntryFinder: IFindEntries
	{
		private Db4oDataSource _dataSource;

		public Db4oLexEntryFinder(Db4oDataSource dataSource)
		{
			_dataSource = dataSource;
		}

		public LexEntry FindFirstEntryMatchingId(string id)
		{
			return Db4oLexQueryHelper.FindFirstEntryMatchingId(_dataSource, id);
		}
	}

	public class InMemoryLexEntryFinder : IFindEntries
	{
		private readonly InMemoryRecordList<LexEntry> _recordList;

		public InMemoryLexEntryFinder(InMemoryRecordList<LexEntry> recordList)
		{
			_recordList = recordList;
		}

		public LexEntry FindFirstEntryMatchingId(string id)
		{
			foreach (LexEntry entry in _recordList)
			{
				if(entry.Id == id)
					return entry;
			}
			return null;
		}
	}
}
