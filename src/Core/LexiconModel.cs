using System;
using System.Collections.Generic;
using System.Text;
using com.db4o;
using Gtk;

namespace WeSay.Core
{

  public class LexiconModel : IDisposable
  {
	ObjectContainer _db;
	LexicalEntry _currentLexicalEntry;
	bool _filtered;
	IList<LexicalEntry> _lexicalEntries;

	public LexiconModel(string filePath) {
	  _filtered = false;
	  _db = Db4o.OpenFile(filePath);
	  UpdateLexicalEntries();
	  UpdateCurrentLexicalEntry();
	}

	public void Dispose() {
	  _db.Close();
	}

	public IList<LexicalEntry> LexicalEntries {
	  get {
		return _lexicalEntries;
	  }
	}

	public LexicalEntry CurrentLexicalEntry {
	  get {
		return _currentLexicalEntry;
	  }
	  set {
		if (LexicalEntries.Contains(value)) {
		  _currentLexicalEntry = value;
		}
		else {
		  throw new ApplicationException("That LexicalEntry does not exist in collection");
		}
	  }
	}

	public bool Filtered {
	  get {
		return _filtered;
	  }
	  set {
		_filtered = value;
		UpdateLexicalEntries();
		UpdateCurrentLexicalEntry();
	  }
	}

	private void UpdateCurrentLexicalEntry() {
	  if (!LexicalEntries.Contains(_currentLexicalEntry)) {
		_currentLexicalEntry = LexicalEntries[0];
	  }
	}

	private void UpdateLexicalEntries() {
		if (_filtered) {
		  _lexicalEntries = _db.Query<LexicalEntry>(delegate(LexicalEntry lexicalEntry)
		  {
			return lexicalEntry.LexicalForm.Length <= 2;
		  });
		}
		else {
		  _lexicalEntries = _db.Query<LexicalEntry>(typeof(LexicalEntry));
		}
	}

	public void GotoNextRecord()
	{
		int i =1 + _lexicalEntries.IndexOf(CurrentLexicalEntry);
		if (i < _lexicalEntries.Count)
			CurrentLexicalEntry = _lexicalEntries[i];
	}
	public void GotoPreviousRecord()
	{
		int i = _lexicalEntries.IndexOf(CurrentLexicalEntry)-1;
		if (i >=0)
			CurrentLexicalEntry = _lexicalEntries[i];
	}

	public void GotoFirstRecord()
	{
		 CurrentLexicalEntry = _lexicalEntries[0];
	}
	public void GotoLastRecord()
	{
		CurrentLexicalEntry = _lexicalEntries[_lexicalEntries.Count - 1];
	}

	  public void Changed(LexicalEntry  entry)
	{
		_db.Set(entry);
	}
}

}
