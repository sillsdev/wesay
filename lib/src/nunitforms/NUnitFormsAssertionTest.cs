using NUnit.Framework;

#region Copyright (c) 2003-2005, Luke T. Maxon

/********************************************************************************************************************
'
' Copyright (c) 2003-2005, Luke T. Maxon
' All rights reserved.
'
' Redistribution and use in source and binary forms, with or without modification, are permitted provided
' that the following conditions are met:
'
' * Redistributions of source code must retain the above copyright notice, this list of conditions and the
' 	following disclaimer.
'
' * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
' 	the following disclaimer in the documentation and/or other materials provided with the distribution.
'
' * Neither the name of the author nor the names of its contributors may be used to endorse or
' 	promote products derived from this software without specific prior written permission.
'
' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
' WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
' PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
' ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
' LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
' INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
' OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
' IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
'
'*******************************************************************************************************************/

#endregion

namespace NUnit.Extensions.Forms
{
	/// <summary>
	/// One of three base classes for your NUnitForms tests.  This one can be
	/// used by people who want "built-in" Assertion functionality and prefer
	/// the older style "Assertion" syntax.
	/// </summary>
	[TestFixture]
	public class NUnitFormsAssertionTest : NUnitFormTest
	{
		public void Assert(bool condition, string message)
		{
			NUnit.Framework.Assert.IsTrue(condition, message);
		}

		public void Assert(bool condition)
		{
			NUnit.Framework.Assert.IsTrue(condition);
		}

		public void AssertEquals(double expected, double actual, double delta)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, delta);
		}

		public void AssertEquals(float expected, float actual, float delta)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, delta);
		}

		public void AssertEquals(object expected, object actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual);
		}

		public void AssertEquals(int expected, int actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual);
		}

		public void AssertEquals(string message, double expected, double actual, double delta)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, delta, message);
		}

		public void AssertEquals(string message, float expected, float actual, float delta)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, delta, message);
		}

		public void AssertEquals(string message, object expected, object actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, message);
		}

		public void AssertEquals(string message, int expected, int actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, message);
		}

		public void AssertNotNull(object anObject)
		{
			NUnit.Framework.Assert.IsNotNull(anObject);
		}

		public void AssertNotNull(string message, object anObject)
		{
			NUnit.Framework.Assert.IsNotNull(anObject, message);
		}

		public void AssertSame(object expected, object actual)
		{
			NUnit.Framework.Assert.AreSame(expected, actual);
		}

		public void AssertSame(string message, object expected, object actual)
		{
			NUnit.Framework.Assert.AreSame(expected, actual, message);
		}

		public void Fail()
		{
			NUnit.Framework.Assert.Fail();
		}

		public void Fail(string message)
		{
			NUnit.Framework.Assert.Fail(message);
		}
	}
}