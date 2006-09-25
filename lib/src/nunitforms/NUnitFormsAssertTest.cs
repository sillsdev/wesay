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

using NUnit.Framework;

namespace NUnit.Extensions.Forms
{
	/// <summary>
	/// One of three base classes for your NUnitForms tests.  This one can be
	/// used by people who want "built-in" Assert functionality and prefer
	/// the newer style "Assert" syntax.
	/// </summary>
	[TestFixture]
	public class NUnitFormsAssertTest : NUnitFormTest
	{
		public void AreEqual(double expected, double actual, double delta)
		{
			Assert.AreEqual(expected, actual, delta);
		}

		public void AreEqual(float expected, float actual, float delta)
		{
			Assert.AreEqual(expected, actual, delta);
		}

		public void AreEqual(object expected, object actual)
		{
			Assert.AreEqual(expected, actual);
		}

		public void AreEqual(int expected, int actual)
		{
			Assert.AreEqual(expected, actual);
		}

		public void AreEqual(decimal expected, decimal actual)
		{
			Assert.AreEqual(expected, actual);
		}

		public void AreEqual(double expected, double actual, double delta, string message)
		{
			Assert.AreEqual(expected, actual, delta, message);
		}

		public void AreEqual(float expected, float actual, float delta, string message)
		{
			Assert.AreEqual(expected, actual, delta, message);
		}

		public void AreEqual(object expected, object actual, string message)
		{
			Assert.AreEqual(expected, actual, message);
		}

		public void AreEqual(int expected, int actual, string message)
		{
			Assert.AreEqual(expected, actual, message);
		}

		public void AreEqual(decimal expected, decimal actual, string message)
		{
			Assert.AreEqual(expected, actual, message);
		}

		public void AreSame(object expected, object actual)
		{
			Assert.AreSame(expected, actual);
		}

		public void AreSame(object expected, object actual, string message)
		{
			Assert.AreSame(expected, actual, message);
		}

		public void Fail()
		{
			Assert.Fail();
		}

		public void Fail(string message)
		{
			Assert.Fail(message);
		}

		public void IsTrue(bool condition)
		{
			Assert.IsTrue(condition);
		}

		public void IsTrue(bool condition, string message)
		{
			Assert.IsTrue(condition, message);
		}

		public void IsFalse(bool condition)
		{
			Assert.IsFalse(condition);
		}

		public void IsFalse(bool condition, string message)
		{
			Assert.IsFalse(condition, message);
		}

		public void IsNull(object anObject)
		{
			Assert.IsNull(anObject);
		}

		public void IsNull(object anObject, string message)
		{
			Assert.IsNull(anObject, message);
		}

		public void IsNotNull(object anObject)
		{
			Assert.IsNotNull(anObject);
		}

		public void IsNotNull(object anObject, string message)
		{
			Assert.IsNotNull(anObject, message);
		}
	}
}