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
public class KeyValueTest
{
	private DataProvider provider;

	[SetUp]
	public void SetUp()
	{
		provider=new DataProvider();
	}

	[Test]
	public void KeysTest()
	{
		TstDictionary tst = new TstDictionary();
		foreach(String s in provider.Entries)
		{
			tst.Add((String)s, provider.EntryTable[s]);
		}
		ICollection keys = tst.Keys;
		Assert.AreEqual(provider.Entries.Count,keys.Count);

		foreach(string key in keys)
		{
			Assert.IsTrue(tst.ContainsKey(key));
			Assert.IsTrue(provider.Entries.Contains(key));
		}
	}

	[Test]
	public void ValuesTest()
	{
		TstDictionary tst = new TstDictionary();
		foreach(String s in provider.Entries)
		{
			tst.Add((String)s, provider.EntryTable[s]);
		}
		ICollection values = tst.Values;
		Assert.AreEqual(provider.Entries.Count,values.Count);

		foreach(Object value in values)
		{
			Assert.IsTrue(tst.ContainsValue(value));
			Assert.IsTrue(provider.EntryTable.ContainsValue(value));
		}
	}

}
}
