using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Palaso.WritingSystems;

namespace WeSay.LexicalModel.Foundation
{
	public class WritingSystemCollection : LdmlInFolderWritingSystemRepository
	{
		// TODO Make this private, and should still build
		// not used in Palaso; not used in WeSay either
		private WritingSystemCollection()
		{
			throw new NotImplementedException();
		}

		// already present in LdmlInFolderWS
		public WritingSystemCollection(string getPathToLdmlWritingSystemsFolder)
		{
			throw new NotImplementedException();
		}

		// successfully moved to Palaso
		public bool Contains(string id)
		{
			throw new NotImplementedException();
		}
		// already in Palaso
		public new WritingSystemDefinition Get(string id)
		{
			throw new NotImplementedException();
		}

		// successfully moved to Palaso
		public IEnumerable<WritingSystemDefinition> TextWritingSystems
 { get; private set; }
		// successfully moved to Palaso
		public new IEnumerable<WritingSystemDefinition> AllWritingSystems { get; private set; }

		// successfully moved to Palaso
		public void OnWritingSystemIDChange(WritingSystemDefinition ws, string oldId)
		{
			throw new NotImplementedException();
		}

	}
}
