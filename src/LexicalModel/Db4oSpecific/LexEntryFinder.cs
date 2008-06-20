using WeSay.Data;

namespace WeSay.LexicalModel.Db4oSpecific
{
	public class Db4oLexEntryFinder: IFindEntries
	{
		private readonly LexEntryRepository _lexEntryRepository;

		public Db4oLexEntryFinder(LexEntryRepository lexEntryRepository)
		{
			_lexEntryRepository = lexEntryRepository;
		}

		public LexEntry FindFirstEntryMatchingId(string id)
		{
			return _lexEntryRepository.GetLexEntryWithMatchingId(id);
		}
	}

	public class InMemoryLexEntryFinder: IFindEntries
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
				if (entry.Id == id)
				{
					return entry;
				}
			}
			return null;
		}
	}
}