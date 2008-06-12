using System;
using System.Collections.Generic;
using Db4objects.Db4o;
using Palaso.Text;
using WeSay.Data;
using WeSay.Foundation;

namespace WeSay.LexicalModel.Db4o_Specific
{
	public class Db4oLexEntryFinder: IFindEntries
	{
		private LexEntryRepository _lexEntryRepository;

		public Db4oLexEntryFinder(LexEntryRepository lexEntryRepository)
		{
			_lexEntryRepository = lexEntryRepository;
		}

		public LexEntry FindFirstEntryMatchingId(string id)
		{
			return _lexEntryRepository.GetLexEntryWithMatchingId(id);
		}
	}

	public class InMemoryLexEntryFinder : IFindEntries
	{
		private readonly IRecordList<LexEntry> _recordList;

		public InMemoryLexEntryFinder(IRecordList<LexEntry> recordList)
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
