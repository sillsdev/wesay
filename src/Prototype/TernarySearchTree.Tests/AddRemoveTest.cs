// This source has been altered from the original which bears the following notice:
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
using NUnit.Framework;

namespace TernarySearchTree.Tests
{


[TestFixture]
public class AddRemoveTest
{
	private DataProvider provider;

	[SetUp]
	public void SetUp()
	{
		provider=new DataProvider();
	}

	[Test]
	public void AddKeys()
	{
		TstDictionary tst = new TstDictionary();
		foreach(String s in provider.Entries)
		{
			long version = tst.Version;
			tst.Add((String)s, provider.EntryTable[s]);
			Assert.IsFalse(version==tst.Version);
		}

		foreach(DictionaryEntry de in provider.EntryTable)
		{
			Assert.IsTrue(tst.Contains((string)de.Key),
				String.Format("{0} was not added properly",(string)de.Key)
				);
			Assert.IsTrue(tst.ContainsKey((string)de.Key),
				String.Format("{0} was not added properly",(string)de.Key)
				);
			Assert.IsTrue(tst.ContainsValue(de.Value),
				String.Format("{0} was not added properly",(string)de.Key)
				);
		}
	}

	[Test]
	public void AddKeysHashOrder()
	{
		TstDictionary tst = new TstDictionary();
		foreach(DictionaryEntry de in provider.EntryTable)
		{
			tst.Add((String)de.Key, de.Value);
		}

		foreach(DictionaryEntry de in provider.EntryTable)
		{
			Assert.IsTrue(tst.Contains((string)de.Key),
				String.Format("{0} was not added properly",(string)de.Key)
				);
		}
	}

	[Test]
	[ExpectedException(typeof(ArgumentNullException))]
	public void AddNullKey()
	{
		TstDictionary tst = new TstDictionary();
		tst.Add(null,null);
	}

	[Test]
	[ExpectedException(typeof(ArgumentException))]
	public void AddEmptyKey()
	{
		TstDictionary tst = new TstDictionary();
		tst.Add("",null);
	}


	[Test]
	[ExpectedException(typeof(ArgumentException))]
	public void AddDuplicateKey()
	{
		TstDictionary tst = new TstDictionary();
		tst.Add("key",null);
		tst.Add("key",null);
	}

	[Test]
	public void RemoveKeys()
	{
		TstDictionary tst = new TstDictionary();
		foreach(String s in provider.Entries)
		{
			tst.Add((String)s, provider.EntryTable[s]);
		}

		foreach(String s in provider.Entries)
		{
			long version = tst.Version;
			tst.Remove(s);
			Assert.IsFalse(version==tst.Version);
			Assert.IsFalse(tst.Contains(s),
						  String.Format("{0} was not removed properly",s)
						  );
		}
	}

	[Test]
	public void RemoveKeysHashOrder()
	{
		TstDictionary tst = new TstDictionary();
		foreach(DictionaryEntry de in provider.EntryTable)
		{
			tst.Add((String)de.Key, de.Value);
		}

		foreach(string s in provider.Entries)
		{
			tst.Remove(s);
			Assert.IsFalse(tst.Contains(s),
						  String.Format("{0} was not removed properly",s)
						  );
		}
	}

	[Test]
	[ExpectedException(typeof(ArgumentNullException))]
	public void RemoveNullKey()
	{
		TstDictionary tst = new TstDictionary();
		foreach(String s in provider.Entries)
		{
			tst.Add((String)s, provider.EntryTable[s]);
		}
		tst.Remove(null);
	}

	[Test]
	[ExpectedException(typeof(ArgumentException))]
	public void RemoveEmptyKey()
	{
		TstDictionary tst = new TstDictionary();
		foreach(String s in provider.Entries)
		{
			tst.Add((String)s, provider.EntryTable[s]);
		}
		tst.Add("",null);
	}


	[Test]
	public void Clear()
	{
		TstDictionary tst = new TstDictionary();
		foreach(String s in provider.Entries)
		{
			tst.Add((String)s, provider.EntryTable[s]);
		}
		long version = tst.Version;
		tst.Clear();
		Assert.IsFalse(version==tst.Version);
		Assert.AreEqual(0, tst.Count);
	}

}
}
