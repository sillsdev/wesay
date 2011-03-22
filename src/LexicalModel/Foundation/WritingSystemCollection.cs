using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palaso.WritingSystems;

namespace WeSay.LexicalModel.Foundation
{
	public class WritingSystemCollection : LdmlInFolderWritingSystemStore, IEnumerable<WritingSystem>
	{
		public string TestWritingSystemVernId;
		public string TestWritingSystemAnalId;
		public WritingSystem UnknownAnalysisWritingSystem { get; private set; }
		public WritingSystem UnknownVernacularWritingSystem { get; private set; }

		public bool Contains(string id)
		{
			throw new NotImplementedException();
		}

		// TODO Use .Get everywhere
		public WritingSystem this[string id]
		{
			get { throw new NotImplementedException(); }
		}

		public IEnumerable<WritingSystem> Values { get; private set; }


		public IEnumerable<string> Keys
		{
			get { throw new NotImplementedException(); }
		}

		// TODO Needs to change to Save() path is in ctor
		public void Write(string getPathToLdmlWritingSystemsFolder)
		{
			throw new NotImplementedException();
		}

		public void Load(string getPathToLdmlWritingSystemsFolder)
		{
			throw new NotImplementedException();
		}

		public IList<string> TrimToActualTextWritingSystemIds(IList<string> writingSystemIds)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<WritingSystem> GetActualTextWritingSystems()
		{
			throw new NotImplementedException();
		}

		public void Add(string testWritingSystemVernId, WritingSystem p1)
		{
			throw new NotImplementedException();
		}

		// TODO this is suspect, migrator?
		public void IdOfWritingSystemChanged(WritingSystem ws, string oldId)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<WritingSystem> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		// TODO Change to Get everywhere
		public bool TryGetValue(string writingSystemId, out WritingSystem writingSystem)
		{
			throw new NotImplementedException();
		}

		// TODO Change everywhere to use WS with one arg ctor
		public WritingSystem AddSimple(string id)
		{
			throw new NotImplementedException();
		}

		// TODO move to migrator
		public void LoadFromLegacyWeSayFile(string getPathToWritingSystemPrefs)
		{
			throw new NotImplementedException();
		}

		public new WritingSystem Get(string key)
		{
			throw new NotImplementedException();
		}
	}
}
