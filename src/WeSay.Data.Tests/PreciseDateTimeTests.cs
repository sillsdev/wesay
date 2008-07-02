using System;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class PreciseDateTimeTests
	{
		[Test]
		public void Test()
		{
			DateTime previousDt = DateTime.MinValue;
			for (int i = 0; i != 1000000; ++i)
			{
				DateTime dt = PreciseDateTime.UtcNow;
				Assert.AreNotEqual(previousDt, dt, "times should be different but both were {0}, iteration {1}", dt, i);
				previousDt = dt;
			}

		}

	}

}