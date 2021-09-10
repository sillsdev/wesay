using NUnit.Framework;
using System;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class ConstructorThrowTests
	{
		/* These tests are a little experiment to see how throwing in the constructor
		 * affects the use of using.
		 *
		 * So, and IDisposable throwing in the constructor does not fit well with
		 * the use of using.
		 */
		class DisposeNotice
		{
			public bool DisposeCalled { get; set; }
		}

		class TestClassThrowInConstructor : IDisposable
		{
			private DisposeNotice _dn;
			public TestClassThrowInConstructor(DisposeNotice dn)
			{
				_dn = dn;
				throw new ApplicationException("TestClassThrowInConstructor");
			}

			public void Dispose()
			{
				_dn.DisposeCalled = true;
			}
		}

		class TestClassThrowInMethod : IDisposable
		{
			private DisposeNotice _dn;

			public TestClassThrowInMethod(DisposeNotice dn)
			{
				_dn = dn;
			}

			public void ThrowMe()
			{
				throw new ApplicationException("TestClassThrowInMethod");
			}

			public void Dispose()
			{
				_dn.DisposeCalled = true;
			}
		}

		[Test]
		public void Constructor_UsingWithThrowInConstructor_DisposeNotCalled()
		{
			var dn = new DisposeNotice();
			try
			{
				// Expecting that Dispose is not called
				using (var testClass = new TestClassThrowInConstructor(dn))
				{
				}
			}
			catch (ApplicationException)
			{
			}
			finally
			{
				Assert.IsFalse(dn.DisposeCalled);
			}
		}

		[Test]
		public void Constructor_UsingWithThrowInMethod_DisposeIsCalled()
		{
			var dn = new DisposeNotice();
			try
			{
				// Expecting that Dispose is called
				using (var testClass = new TestClassThrowInMethod(dn))
				{
					testClass.ThrowMe();
				}
			}
			catch (ApplicationException)
			{
			}
			finally
			{
				Assert.IsTrue(dn.DisposeCalled);
			}
		}

	}
}