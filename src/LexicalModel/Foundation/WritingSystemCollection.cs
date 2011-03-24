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
		private WritingSystemCollection()
		{
			throw new NotImplementedException();
		}

		public WritingSystemCollection(string getPathToLdmlWritingSystemsFolder)
		{
			throw new NotImplementedException();
		}

		// successfully moved to Palaso
		public bool Contains(string id)
		{
			throw new NotImplementedException();
		}

		public new WritingSystem Get(string id)
		{
			throw new NotImplementedException();
		}

		// successfully moved to Palaso
		public IEnumerable<WritingSystem> TextWritingSystems
 { get; private set; }
		// successfully moved to Palaso
		public new IEnumerable<WritingSystem> AllWritingSystems { get; private set; }

		// TODO Move to Palaso
		public void OnWritingSystemIDChange(WritingSystem ws, string oldId)
		{
			throw new NotImplementedException();
		}

	}
}
