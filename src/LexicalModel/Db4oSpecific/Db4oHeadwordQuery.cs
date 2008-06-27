using System;
using System.Collections.Generic;
using System.Globalization;
using Db4objects.Db4o;
using Palaso.Base32;
using WeSay.Data;
using WeSay.Foundation;

namespace WeSay.LexicalModel.Db4oSpecific
{
	internal class Db4oHeadwordQuery: IQuery<LexEntry>
	{
		private readonly Db4oDataSource _db4oData; // for data
		private readonly WritingSystem _writingSystem;
		private readonly LexEntryRepository _repository;

		public Db4oHeadwordQuery(LexEntryRepository repository,
								 Db4oDataSource ds,
								 WritingSystem writingSystem)
		{
			_db4oData = ds;
			_repository = repository;
			_writingSystem = writingSystem;
		}

		public IEnumerable<string[]> GetDisplayStrings(LexEntry item)
		{
			List<string[]> keys = new List<string[]>();
			string form = item.GetHeadWordForm(_writingSystem.Id);
			byte[] keydata = _writingSystem.GetSortKey(form).KeyData;

			//turn that byte into something which, when sorted with ordinal sort order, will
			//give the correct ordering
			string key = Base32Convert.ToBase32HexString(keydata, Base32FormattingOptions.None);

			//this will change to incorporate homograph number, when the user can edit that (or we
			//import it)
			string universalSortableDateTimePattern = DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern;
			key += "_" + item.CreationTime.ToString(universalSortableDateTimePattern);

			string[] columns = new string[1];
			columns[0] = key;
			keys.Add(columns);
			return keys;
		}

		public ResultSet<LexEntry> RetrieveItems()
		{
			List<RecordToken<LexEntry>> result = new List<RecordToken<LexEntry>>();

			IObjectSet set = _db4oData.Data.Get(typeof (LexEntry));

			//enhance: This will be slow and take a lot of ram, as it will bring each entry
			//into ram.  It is theoretically possible to just query on the lexemeform
			//and citation form for each entry, and compute the headword.
			foreach (LexEntry entry in set)
			{
				int i = 0;
				foreach (string[] key in GetDisplayStrings(entry))
				{
					result.Add(
							new RecordToken<LexEntry>(_repository,
													  key,
													  new Db4oRepositoryId(
															  _db4oData.Data.Ext().GetID(entry))));
					++i;
				}
			}
			result.Sort(new RecordTokenComparer<LexEntry>(StringComparer.Ordinal));
			return new ResultSet<LexEntry>(_repository, result);
		}
	}
}