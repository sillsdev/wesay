using System;
using System.Collections.Generic;
using System.Text;
using com.db4o;
using Gtk;

namespace WeSay.Core
{

  public class LexiconModel : ICollection<LexicalEntry>, IDisposable
  {
	ObjectContainer _db;
	LexicalEntry _currentLexicalEntry;
	bool _filtered;
	IList<LexicalEntry> _lexicalEntries;

	public LexiconModel(string filePath) {
	  _filtered = false;
	  _db = Db4o.OpenFile(filePath);
	  RefreshLexicalEntries();
	  RefreshCurrentLexicalEntry();
	}

	public void Dispose() {
	  _db.Close();
	}

	private void RefreshCurrentLexicalEntry() {
	  if (!_lexicalEntries.Contains(_currentLexicalEntry)) {
		_currentLexicalEntry = _lexicalEntries[0];
	  }
	}

	private void RefreshLexicalEntries() {
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

	public bool Filtered {
	  get {
		return _filtered;
	  }
	  set {
		_filtered = value;
		RefreshLexicalEntries();
		RefreshCurrentLexicalEntry();
	  }
	}
	#region Iterator
	public LexicalEntry CurrentLexicalEntry {
	  get {
		return _currentLexicalEntry;
	  }
	  set {
		if (_lexicalEntries.Contains(value)) {
		  _currentLexicalEntry = value;
		}
		else {
		  throw new ApplicationException("That LexicalEntry does not exist in collection");
		}
	  }
	}

	public bool GotoNextRecord() {
	  int i = 1 + _lexicalEntries.IndexOf(CurrentLexicalEntry);
	  if (i < _lexicalEntries.Count) {
		CurrentLexicalEntry = _lexicalEntries[i];
		return true;
	  }
	  return false;
	}
	public bool GotoPreviousRecord() {
	  int i = _lexicalEntries.IndexOf(CurrentLexicalEntry) - 1;
	  if (i >= 0) {
		CurrentLexicalEntry = _lexicalEntries[i];
		return true;
	  }
	  return false;
	}

	public void GotoFirstRecord() {
	  CurrentLexicalEntry = _lexicalEntries[0];
	}
	public void GotoLastRecord() {
	  CurrentLexicalEntry = _lexicalEntries[_lexicalEntries.Count - 1];
	}
	#endregion

	#region IList<LexicalEntry> Members

	public int IndexOf(LexicalEntry item) {
	  return _lexicalEntries.IndexOf(item);
	}

	public LexicalEntry this[int index] {
	  get {
		return _lexicalEntries[index];
	  }
	}

	#endregion

	public void OnChanged(LexicalEntry entry) {
	  _db.Set(entry);
	  _db.Commit();
	}

	#region ICollection<LexicalEntry> Members

	public void Clear() {
	  foreach (LexicalEntry entry in _lexicalEntries) {
		_db.Delete(entry);
	  }
	  _db.Commit();
	}

	public bool Contains(LexicalEntry item) {
	  return _lexicalEntries.Contains(item);
	}

	public void CopyTo(LexicalEntry[] array, int arrayIndex) {
	  _lexicalEntries.CopyTo(array, arrayIndex);
	}

	public int Count {
	  get {
		return _lexicalEntries.Count;
	  }
	}

	public bool IsReadOnly {
	  get {
		return false;
	  }
	}

	public bool Remove(LexicalEntry item) {
	  if (!GotoNextRecord()) {
		GotoPreviousRecord();
	  }
	  _db.Delete(item);
	  _db.Commit();
	  RefreshLexicalEntries();
	  return true;
	}

	public void Add(LexicalEntry item) {
	  _db.Set(item);
	  _db.Commit();
	  RefreshLexicalEntries();
	  CurrentLexicalEntry = item;
	}

	#endregion

	#region IEnumerable<LexicalEntry> Members

	public IEnumerator<LexicalEntry> GetEnumerator() {
	  return _lexicalEntries.GetEnumerator();
	}

	#endregion

	#region IEnumerable Members

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
	  return _lexicalEntries.GetEnumerator();
	}

	#endregion
  }

}
