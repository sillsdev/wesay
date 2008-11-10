using NUnit.Framework;

namespace WeSay.AddinLib.Tests
{
	[TestFixture]
	[Ignore("TestFixture needs review!")]
	public class SettingsTests
	{
		[SetUp]
		public void Setup() {}

		[TearDown]
		public void TearDown() {}

		//        [Test]
		//        public void Test()
		//        {
		//                Addin.Transform.LiftTransformer t = new Addin.Transform.LiftTransformer();
		//            object settings = t.SettingsToPersist;
		//            XmlSerializer x = new XmlSerializer( settings.GetType());
		//            using(XmlWriter w = XmlWriter.Create("test.txt"))
		//            {
		//                x.Serialize(w, settings);
		//            }
		//
		//            using (XmlReader r = XmlReader.Create("test.txt"))
		//            {
		//                Addin.Transform.LiftTransformer t2 = new Addin.Transform.LiftTransformer();
		//                t2.SettingsToPersist = x.Deserialize(r);
		//
		//            }
		//        }
	}
}