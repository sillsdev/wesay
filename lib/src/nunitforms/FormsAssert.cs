namespace NUnit.Extensions.Forms
{
	public class FormsAssert
	{
		public static void AreEqual(object o, object o2, string error)
		{
			if(!o.Equals(o2))
			{
				throw new FormsTestAssertionException("should be equal " + o + " : " + o2 + " , " + error);
			}
		}

		public static void IsTrue(bool val)
		{
			if(!val)
			{
				throw new FormsTestAssertionException("was not true");
			}
		}
	}
}