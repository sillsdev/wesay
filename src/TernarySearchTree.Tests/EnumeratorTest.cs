// This source has been altered from the original which bears the following notice:
//
// Ternary Search Tree Implementation for C#
//
// Copyright (c) 2004 Jonathan de Halleux
//
// This software is provided 'as-is', without any express or implied warranty.
//
// In no event will the authors be held liable for any damages arising from
// the use of this software.
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
//
//		1. The origin of this software must not be misrepresented;
//		you must not claim that you wrote the original software.
//		If you use this software in a product, an acknowledgment in the product
//		documentation would be appreciated but is not required.
//
//		2. Altered source versions must be plainly marked as such, and must
//		not be misrepresented as being the original software.
//
//		3. This notice may not be removed or altered from any source
//		distribution.
//
//	Ternary Search Tree Implementation for C# Library HomePage:
//		http://www.dotnetwiki.org
//	Author: Jonathan de Halleux
//  Algorithm found in J. L. Bentley and R. Sedgewick,
//      Fast algorithms for sorting and searching strings,
//      in Proceedings of the Eighth Annual ACM-SIAM Symposium on Discrete Algorithms,
//      New Orleans Louisiana, January 5-7,
//      1997

// created on 9/01/2004 at 13:11
using System;
using System.Collections;
using System.Collections.Specialized;
using NUnit.Framework;

namespace TernarySearchTree.Tests
{


[TestFixture]
public class EnumeratorTest
{
	private DataProvider provider;

	[SetUp]
	public void SetUp()
	{
		provider=new DataProvider();
	}

	[Test]
	public void EnumerateTest()
	{
		TstDictionary tst = new TstDictionary();
		foreach(String s in provider.Entries)
		{
			tst.Add((String)s, null);
		}

		StringCollection keys = new StringCollection();
		foreach(DictionaryEntry de in tst)
		{
			Assert.IsFalse(keys.Contains((string)de.Key));
			keys.Add((String)de.Key);
		}

		Assert.AreEqual(provider.Entries.Count,keys.Count);
		foreach(string key in provider.Entries)
			keys.Remove(key);
		Assert.AreEqual(0,keys.Count);
	}

	[Test]
	public void EnumerateTestHashOrder()
	{
		TstDictionary tst = new TstDictionary();
		foreach(DictionaryEntry de in provider.EntryTable)
		{
			tst.Add((String)de.Key, null);
		}

		StringCollection keys = new StringCollection();
		foreach(DictionaryEntry de in tst)
		{
			Assert.IsFalse(keys.Contains((string)de.Key));
			keys.Add((String)de.Key);
		}

		Assert.AreEqual(provider.Entries.Count,keys.Count);
		foreach(string key in provider.Entries)
			keys.Remove(key);
		Assert.AreEqual(0,keys.Count);
	}

	[Test]
	[ExpectedException(typeof(InvalidOperationException))]
	public void ResetCurrent()
	{
		TstDictionary tst = new TstDictionary();
		foreach(DictionaryEntry de in provider.EntryTable)
		{
			tst.Add((String)de.Key, null);
		}

		IDictionaryEnumerator en = tst.GetEnumerator();
		Object o = en.Current;
	}

	[Test]
	[ExpectedException(typeof(InvalidOperationException))]
	public void ResetEntry()
	{
		TstDictionary tst = new TstDictionary();
		foreach(DictionaryEntry de in provider.EntryTable)
		{
			tst.Add((String)de.Key, null);
		}

		IDictionaryEnumerator en = tst.GetEnumerator();
		Object o = en.Entry;
	}


	[Test]
	[ExpectedException(typeof(InvalidOperationException))]
	public void ResetKey()
	{
		TstDictionary tst = new TstDictionary();
		foreach(DictionaryEntry de in provider.EntryTable)
		{
			tst.Add((String)de.Key, null);
		}

		IDictionaryEnumerator en = tst.GetEnumerator();
		Object o = en.Key;
	}


	[Test]
	[ExpectedException(typeof(InvalidOperationException))]
	public void ResetValue()
	{
		TstDictionary tst = new TstDictionary();
		foreach(DictionaryEntry de in provider.EntryTable)
		{
			tst.Add((String)de.Key, null);
		}

		IDictionaryEnumerator en = tst.GetEnumerator();
		Object o = en.Value;
	}

	[Test]
	[ExpectedException(typeof(InvalidOperationException))]
	public void ResetCurrentEmpty()
	{
		TstDictionary tst = new TstDictionary();
		IDictionaryEnumerator en = tst.GetEnumerator();
		Object o = en.Current;
	}

	[Test]
	[ExpectedException(typeof(InvalidOperationException))]
	public void ResetEntryEmpty()
	{
		TstDictionary tst = new TstDictionary();
		IDictionaryEnumerator en = tst.GetEnumerator();
		Object o = en.Entry;
	}

	[Test]
	[ExpectedException(typeof(InvalidOperationException))]
	public void ResetKeyEmpty()
	{
		TstDictionary tst = new TstDictionary();
		IDictionaryEnumerator en = tst.GetEnumerator();
		Object o = en.Key;
	}

	[Test]
	[ExpectedException(typeof(InvalidOperationException))]
	public void ResetValueEmpty()
	{
		TstDictionary tst = new TstDictionary();
		IDictionaryEnumerator en = tst.GetEnumerator();
		Object o = en.Value;
	}

	[Test]
	[ExpectedException(typeof(InvalidOperationException))]
	public void ResetCalledWhileDictionaryModified()
	{
		TstDictionary tst = new TstDictionary();
		foreach(DictionaryEntry de in provider.EntryTable)
		{
			tst.Add((String)de.Key, null);
		}
		IDictionaryEnumerator en = tst.GetEnumerator();
		tst.Add("test",null);
		en.Reset();
	}

	[Test]
	[ExpectedException(typeof(InvalidOperationException))]
	public void MoveNextCalledWhileDictionaryModified()
	{
		TstDictionary tst = new TstDictionary();
		foreach(DictionaryEntry de in provider.EntryTable)
		{
			tst.Add((String)de.Key, null);
		}
		IDictionaryEnumerator en = tst.GetEnumerator();
		tst.Add("test",null);
		en.MoveNext();
	}

	[Test]
	[ExpectedException(typeof(InvalidOperationException))]
	public void CurrentCalledWhileDictionaryModified()
	{
		TstDictionary tst = new TstDictionary();
		foreach(DictionaryEntry de in provider.EntryTable)
		{
			tst.Add((String)de.Key, null);
		}
		IDictionaryEnumerator en = tst.GetEnumerator();
		en.MoveNext();
		tst.Add("test",null);
		Object o = en.Current;
	}

	[Test]
	[ExpectedException(typeof(InvalidOperationException))]
	public void EntryCalledWhileDictionaryModified()
	{
		TstDictionary tst = new TstDictionary();
		foreach(DictionaryEntry de in provider.EntryTable)
		{
			tst.Add((String)de.Key, null);
		}
		IDictionaryEnumerator en = tst.GetEnumerator();
		en.MoveNext();
		tst.Add("test",null);
		Object o = en.Entry;
	}
	[Test]
	[ExpectedException(typeof(InvalidOperationException))]
	public void KeyCalledWhileDictionaryModified()
	{
		TstDictionary tst = new TstDictionary();
		foreach(DictionaryEntry de in provider.EntryTable)
		{
			tst.Add((String)de.Key, null);
		}
		IDictionaryEnumerator en = tst.GetEnumerator();
		en.MoveNext();
		tst.Add("test",null);
		Object o = en.Key;
	}
	[Test]
	[ExpectedException(typeof(InvalidOperationException))]
	public void ValueCalledWhileDictionaryModified()
	{
		TstDictionary tst = new TstDictionary();
		foreach(DictionaryEntry de in provider.EntryTable)
		{
			tst.Add((String)de.Key, null);
		}
		IDictionaryEnumerator en = tst.GetEnumerator();
		en.MoveNext();
		tst.Add("test",null);
		Object o = en.Value;
	}


}
}
