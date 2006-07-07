using System;
using System.Collections.Generic;
using System.Text;
using com.db4o;
using Gtk;

namespace WeSay.Core
{

  public class LexiconModel : ICollection<LexicalEntry>, IDisposable
  {
	static public string s_FilePath; //hack until we learn more of Spring.Net

	ObjectContainer _db;
	bool _filtered;
	IList<LexicalEntry> _lexicalEntries;

	public LexiconModel() {
	  _filtered = false;
	  FilePath = s_FilePath;
	}

	public LexiconModel(string filePath) {
	  _filtered = false;
	  s_FilePath = filePath;
	  FilePath = s_FilePath;
	}

	public string FilePath {
	  set {
		_db = Db4o.OpenFile(value);
		if (_db == null)
		  throw new ApplicationException("Problem opening " + value);
		RefreshLexicalEntries();
	  }
	}

	public void Dispose() {
	  _db.Close();
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
		if (_filtered != value) {
		  _filtered = value;
		  RefreshLexicalEntries();
		}
	  }
	}
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
	  _db.Delete(item);
	  _db.Commit();
	  RefreshLexicalEntries();
	  return true;
	}

	public void Add(LexicalEntry item) {
	  _db.Set(item);
	  _db.Commit();
	  RefreshLexicalEntries();
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
