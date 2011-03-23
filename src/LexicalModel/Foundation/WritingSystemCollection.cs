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
		public WritingSystem UnknownAnalysisWritingSystem { get; private set; }
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


		// TODO Needs to change to Save() path is in ctor
		public void Write(string getPathToLdmlWritingSystemsFolder)
		{
			throw new NotImplementedException();
		}

		// TODO remove (it is private and called in the ctor)
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
