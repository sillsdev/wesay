using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palaso.WritingSystems;

namespace WeSay.LexicalModel.Foundation
{
	public class WritingSystemCollection : LdmlInFolderWritingSystemStore/*, IEnumerable<WritingSystem>*/
	{
		// TODO Make this private, and should still build
		private WritingSystemCollection()
		{
			throw new NotImplementedException();
		}

		public WritingSystemCollection(string getPathToLdmlWritingSystemsFolder)
		{
			throw new NotImplementedException();
		}

		// TODO refactor wesay so that these are no longer used.
		//public WritingSystem UnknownAnalysisWritingSystem { get; private set; }
		public WritingSystem UnknownVernacularWritingSystem { get; private set; }


		public bool Contains(string id)
		{
			throw new NotImplementedException();
		}

		public new WritingSystem Get(string id)
		{
			throw new NotImplementedException();
		}

		//public new IEnumerable<WritingSystem> WritingSystemDefinitions { get; private set; }


		// CP TODO
		public IList<string> TrimToActualTextWritingSystemIds(IList<string> writingSystemIds)
		{
			throw new NotImplementedException();
		}

		// CP TODO
		public IEnumerable<WritingSystem> GetActualTextWritingSystems()
		{
			throw new NotImplementedException();
		}

		// TODO Change to Set, also has sig change (no key).
		//public void Add(string testWritingSystemVernId, WritingSystem p1)
		//{
		//    throw new NotImplementedException();
		//}

		// TODO Move to Palaso
		public void OnWritingSystemIDChange(WritingSystem ws, string oldId)
		{
			throw new NotImplementedException();
		}
		/*
		public IEnumerator<WritingSystem> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		*/

		public new IEnumerable<WritingSystem> WritingSystemDefinitions { get; private set; }

	}
}
