using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;
//using Exortech.NetReflector;
using NUnit.Framework;
using System.ComponentModel;
using Palaso.Text;
using System.Collections;

namespace WeSay.Foundation.Tests
{
	public class MultiTextBaseTests
	{

		[Test]
		public void NullConditions()
		{
			MultiText text = new MultiText();
			Assert.AreSame(string.Empty, text["foo"], "never before heard of alternative should give back an empty string");
			Assert.AreSame(string.Empty, text["foo"], "second time");
			Assert.AreSame(string.Empty, text.GetBestAlternative("fox"));
			text.SetAlternative("zox", "");
			Assert.AreSame(string.Empty, text["zox"]);
			text.SetAlternative("zox", null);
			Assert.AreSame(string.Empty, text["zox"], "should still be empty string after setting to null");
			text.SetAlternative("zox", "something");
			text.SetAlternative("zox", null);
			Assert.AreSame(string.Empty, text["zox"], "should still be empty string after setting something and then back to null");
		}
		[Test]
		public void BasicStuff()
		{
			MultiText text = new MultiText();
			text["foo"] = "alpha";
			Assert.AreSame("alpha", text["foo"]);
			text["foo"] = "beta";
			Assert.AreSame("beta", text["foo"]);
			text["foo"] = "gamma";
			Assert.AreSame("gamma", text["foo"]);
			text["bee"] = "beeeee";
			Assert.AreSame("gamma", text["foo"], "setting a different alternative should not affect this one");
			text["foo"] = null;
			Assert.AreSame(string.Empty, text["foo"]);
		}

		//        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		//        public void GetIndexerThrowsWhenAltIsMissing()
		//        {
		//            MultiText text = new MultiText();
		//            text["foo"] = "alpha";
		//            string s = text["gee"];
		//        }
		//
		//        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		//        public void GetExactThrowsWhenAltIsMissing()
		//        {
		//            MultiText text = new MultiText();
		//            text["foo"] = "alpha";
		//            string s = text.GetExactAlternative("gee");
		//        }

		//        [Test]
		//        public void ImplementsIEnumerable()
		//        {
		//            MultiText text = new MultiText();
		//            IEnumerable ienumerable = text;
		//            Assert.IsNotNull(ienumerable);
		//        }

		[Test]
		public void Count()
		{
			MultiText text = new MultiText();
			Assert.AreEqual(0, text.Count);
			text["a"] = "alpha";
			text["b"] = "beta";
			text["g"] = "gamma";
			Assert.AreEqual(3, text.Count);
		}

		[Test]
		public void IterateWithForEach()
		{
			MultiText text = new MultiText();
			text["a"] = "alpha";
			text["b"] = "beta";
			text["g"] = "gamma";
			int i = 0;
			foreach (LanguageForm l in text)
			{
				switch (i)
				{
					case 0:
						Assert.AreEqual("a", l.WritingSystemId);
						Assert.AreEqual("alpha", l.Form);
						break;
					case 1:
						Assert.AreEqual("b", l.WritingSystemId);
						Assert.AreEqual("beta", l.Form);
						break;
					case 2:
						Assert.AreEqual("g", l.WritingSystemId);
						Assert.AreEqual("gamma", l.Form);
						break;
				}
				i++;
			}
		}
		[Test]
		public void GetEnumerator()
		{
			MultiText text = new MultiText();
			IEnumerator ienumerator = text.GetEnumerator();
			Assert.IsNotNull(ienumerator);
		}



		[Test]
		public void MergeWithEmpty()
		{
			MultiText old = new MultiText();
			MultiText newGuy = new MultiText();
			old.MergeIn(newGuy);
			Assert.AreEqual(0, old.Count);

			old = new MultiText();
			old["a"] = "alpha";
			old.MergeIn(newGuy);
			Assert.AreEqual(1, old.Count);
		}

		[Test]
		public void MergeWithOverlap()
		{
			MultiText old = new MultiText();
			old["a"] = "alpha";
			old["b"] = "beta";
			MultiText newGuy = new MultiText();
			newGuy["b"] = "newbeta";
			newGuy["c"] = "charlie";
			old.MergeIn(newGuy);
			Assert.AreEqual(3, old.Count);
			Assert.AreEqual("newbeta", old["b"]);
		}

		[Test]
		public void UsesNextAlternativeWhenMissing()
		{
			MultiText multiText = new MultiText();
			multiText["wsWithNullElement"] = null;
			multiText["wsWithEmptyElement"] = "";
			multiText["wsWithContent"] = "hello";
			Assert.AreEqual(String.Empty, multiText.GetExactAlternative("missingWs"));
			Assert.AreEqual(String.Empty, multiText.GetExactAlternative("wsWithEmptyElement"));
			Assert.AreEqual("hello", multiText.GetBestAlternative("missingWs"));
			Assert.AreEqual("hello", multiText.GetBestAlternative("wsWithEmptyElement"));
			Assert.AreEqual("hello*", multiText.GetBestAlternative("wsWithEmptyElement", "*"));
			Assert.AreEqual("hello", multiText.GetBestAlternative("wsWithNullElement"));
			Assert.AreEqual("hello*", multiText.GetBestAlternative("wsWithNullElement", "*"));
			Assert.AreEqual("hello", multiText.GetExactAlternative("wsWithContent"));
			Assert.AreEqual("hello", multiText.GetBestAlternative("wsWithContent"));
			Assert.AreEqual("hello", multiText.GetBestAlternative("wsWithContent", "*"));
		}


		[Test]
		public void SerializeWithXmlSerializer()
		{
			MultiText text = new MultiText();
			text["foo"] = "alpha";
			text["boo"] = "beta";
			string answer =
				@"<?xml version='1.0' encoding='utf-16'?>
<TestMultiTextHolder xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
  <name>
	<form lang='foo'>alpha</form>
	<form lang='boo'>beta</form>
  </name>
</TestMultiTextHolder>";
			CheckSerializeWithXmlSerializer(text, answer);
		}

		[Test]
		public void SerializeEmptyWithXmlSerializer()
		{
			MultiText text = new MultiText();
			string answer =
				@"<?xml version='1.0' encoding='utf-16'?>
<TestMultiTextHolder xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
  <name />
</TestMultiTextHolder>";
			CheckSerializeWithXmlSerializer(text, answer);
		}


		public void CheckSerializeWithXmlSerializer(MultiText multitext, string answer)
		{

			XmlSerializer ser = new XmlSerializer(typeof(TestMultiTextHolder));

			StringWriter writer = new System.IO.StringWriter();
			TestMultiTextHolder holder = new TestMultiTextHolder();
			holder.Name = multitext;
			ser.Serialize(writer, holder);

			string mtxml = writer.GetStringBuilder().ToString();
			mtxml = mtxml.Replace('"', '\'');
			Debug.WriteLine(mtxml);
			Assert.AreEqual(answer, mtxml);
		}

		//        [Test]
		//        public void DeSerializeWithNetReflector()
		//        {
		//            MultiText text = new MultiText();
		//            text["foo"] = "alpha";
		//
		//            NetReflectorTypeTable t = new NetReflectorTypeTable();
		//            t.Add(typeof(MultiText));
		//            t.Add(typeof(TestMultiTextHolder));
		//
		//
		//            string answer =
		//                @"<testMultiTextHolder>
		//                    <name>
		//				        <form starred='false' ws='en'>verb</form>
		//				        <form starred='false' ws='fr'>verbe</form>
		//				        <form starred='false' ws='es'>verbo</form>
		//			        </name>
		//                </testMultiTextHolder>";
		//            NetReflectorReader r = new NetReflectorReader(t);
		//            TestMultiTextHolder h = (TestMultiTextHolder)r.Read(answer);
		//            Assert.AreEqual(3, h._name.Count);
		//            Assert.AreEqual("verbo",h._name["es"]);
		//        }

		[Test]
		public void DeSerializesWithOldWsAttributes()
		{
			MultiText t = DeserializeWithXmlSerialization(
				@"<TestMultiTextHolder>
					 <name>
						<form ws='en'>verb</form>
						<form ws='fr'>verbe</form>
						<form ws='es'>verbo</form>
					</name>
					</TestMultiTextHolder>
				");
			Assert.AreEqual(3, t.Forms.Length);
			Assert.AreEqual("verbo", t["es"]);
		}

		[Test]
		public void DeSerializesWithNewWsAttributes()
		{
			MultiText t = DeserializeWithXmlSerialization(
				@"<TestMultiTextHolder>
					 <name>
						<form lang='en'>verb</form>
						<form lang='fr'>verbe</form>
						<form lang='es'>verbo</form>
					</name>
					</TestMultiTextHolder>
				");
			Assert.AreEqual(3, t.Forms.Length);
			Assert.AreEqual("verbo", t["es"]);
		}

		[Test]
		public void DeSerializesWhenEmpty()
		{
			MultiText t = DeserializeWithXmlSerialization(
				@"  <TestMultiTextHolder>
						<name/>
					</TestMultiTextHolder>");
			Assert.AreEqual(0, t.Forms.Length);
		}


		private MultiText DeserializeWithXmlSerialization(string answer)
		{
			StringReader r = new StringReader(answer);
			System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof(TestMultiTextHolder));
			TestMultiTextHolder holder = serializer.Deserialize(r) as TestMultiTextHolder;
			return holder.Name;
		}

		//  [ReflectorType("testMultiTextHolder")]
		public class TestMultiTextHolder
		{
			[XmlIgnore]
			public MultiText _name;

			[XmlElement("name")]
			//    [ReflectorProperty("name", typeof(MultiTextSerializorFactory), Required = true)]
			public MultiText Name
			{
				get { return _name; }
				set { _name = value; }
			}
		}

		[Test]
		public void Equals_DifferentNumberOfForms_False()
		{
			MultiText x = new MultiText();
			x["ws"] = "test";
			x["ws2"] = "test";
			MultiText y = new MultiText();
			y["ws"] = "test";
			Assert.IsFalse(x.Equals(y));
			Assert.IsFalse(y.Equals(x));
		}

		[Test]
		public void Equals_SameContent_True()
		{
			MultiText x = new MultiText();
			x["ws"] = "test";
			MultiText y = new MultiText();
			y.MergeIn(x);
			Assert.IsTrue(x.Equals(y));
			Assert.IsTrue(y.Equals(x));
		}

		[Test]
		public void Equals_Identity_True()
		{
			MultiText x = new MultiText();
			Assert.IsTrue(x.Equals(x));
		}

		[Test]
		public void Equals_DifferentValues_False()
		{
			MultiText x = new MultiText();
			x["ws"] = "test";
			MultiText y = new MultiText();
			y["ws"] = "test1";
			Assert.IsFalse(x.Equals(y));
			Assert.IsFalse(y.Equals(x));
		}

		[Test]
		public void Equals_DifferentWritingSystems_False()
		{
			MultiText x = new MultiText();
			x["ws"] = "test";
			MultiText y = new MultiText();
			y["ws1"] = "test";
			Assert.IsFalse(x.Equals(y));
			Assert.IsFalse(y.Equals(x));
		}

		[Test]
		public void ContainsEqualForm_SameContent_True()
		{
			MultiText x = new MultiText();
			x["ws1"] = "testing";
			x["ws"] = "test";
			x["ws2"] = "testing";
			LanguageForm form = new LanguageForm();
			form.WritingSystemId = "ws";
			form.Form = "test";
			Assert.IsTrue(x.ContainsEqualForm(form));
		}

		[Test]
		public void ContainsEqualForm_DifferentWritingSystem_False()
		{
			MultiText x = new MultiText();
			x["ws"] = "test";
			LanguageForm form = new LanguageForm();
			form.WritingSystemId = "wss";
			form.Form = "test";
			Assert.IsFalse(x.ContainsEqualForm(form));
		}

		[Test]
		public void ContainsEqualForm_DifferentValue_False()
		{
			MultiText x = new MultiText();
			x["ws"] = "test";
			LanguageForm form = new LanguageForm();
			form.WritingSystemId = "ws";
			form.Form = "tests";
			Assert.IsFalse(x.ContainsEqualForm(form));
		}

		[Test]
		public void HasFormWithSameContent_Identity_True()
		{
			MultiText x = new MultiText();
			x["ws1"] = "testing";
			x["ws"] = "test";
			x["ws2"] = "testing";
			Assert.IsTrue(x.HasFormWithSameContent(x));

		}
		[Test]
		public void HasFormWithSameContent_SameContent_True()
		{
			MultiText x = new MultiText();
			x["ws1"] = "testing";
			x["ws"] = "test";
			x["ws2"] = "testing";
			MultiText y = new MultiText();
			x["ws1"] = "testin";
			y["ws"] = "test";
			Assert.IsTrue(x.HasFormWithSameContent(y));
			Assert.IsTrue(y.HasFormWithSameContent(x));
		}

		[Test]
		public void HasFormWithSameContent_DifferentWritingSystem_False()
		{
			MultiText x = new MultiText();
			x["ws"] = "test";
			MultiText y = new MultiText();
			y["wss"] = "test";
			Assert.IsFalse(x.HasFormWithSameContent(y));
			Assert.IsFalse(y.HasFormWithSameContent(x));
		}

		[Test]
		public void HasFormWithSameContent_DifferentValue_False()
		{
			MultiText x = new MultiText();
			x["ws"] = "test";
			MultiText y = new MultiText();
			y["ws"] = "tests";
			Assert.IsFalse(x.HasFormWithSameContent(y));
			Assert.IsFalse(y.HasFormWithSameContent(x));
		}


		[Test]
		public void HasFormWithSameContent_BothEmpty_True()
		{
			MultiText x = new MultiText();
			MultiText y = new MultiText();
			Assert.IsTrue(x.HasFormWithSameContent(y));
			Assert.IsTrue(y.HasFormWithSameContent(x));
		}



		[Test]
		public void SetAnnotation()
		{
			MultiText multiText = new MultiText();
			multiText.SetAnnotationOfAlternativeIsStarred("zz", true);
			Assert.AreEqual(String.Empty, multiText.GetExactAlternative("zz"));
			Assert.IsTrue(multiText.GetAnnotationOfAlternativeIsStarred("zz"));
			multiText.SetAnnotationOfAlternativeIsStarred("zz", false);
			Assert.IsFalse(multiText.GetAnnotationOfAlternativeIsStarred("zz"));
		}

		[Test]
		public void ClearingAnnotationOfEmptyAlternativeRemovesTheAlternative()
		{
			MultiText multiText = new MultiText();
			multiText.SetAnnotationOfAlternativeIsStarred("zz", true);
			multiText.SetAnnotationOfAlternativeIsStarred("zz", false);
			Assert.IsFalse(multiText.ContainsAlternative("zz"));
		}

		[Test]
		public void ClearingAnnotationOfNonEmptyAlternative()
		{
			MultiText multiText = new MultiText();
			multiText.SetAnnotationOfAlternativeIsStarred("zz", true);
			multiText["zz"] = "hello";
			multiText.SetAnnotationOfAlternativeIsStarred("zz", false);
			Assert.IsTrue(multiText.ContainsAlternative("zz"));
		}

		[Test]
		public void EmptyingTextOfFlaggedAlternativeDoesNotDeleteIfFlagged()
		{
			// REVIEW: not clear really what behavior we want here, since user deletes via clearing text
			MultiText multiText = new MultiText();
			multiText["zz"] = "hello";
			multiText.SetAnnotationOfAlternativeIsStarred("zz", true);
			multiText["zz"] = "";
			Assert.IsTrue(multiText.ContainsAlternative("zz"));
		}

		[Test]
		public void AnnotationOfMisssingAlternative()
		{
			MultiText multiText = new MultiText();
			Assert.IsFalse(multiText.GetAnnotationOfAlternativeIsStarred("zz"));
			Assert.IsFalse(multiText.ContainsAlternative("zz"), "should not cause the creation of the alt");
		}


		[Test]
		public void ContainsEqualForm_DifferentStarred_False()
		{
			MultiText x = new MultiText();
			x["ws"] = "test";
			LanguageForm form = new LanguageForm();
			form.WritingSystemId = "ws";
			form.Form = "test";
			form.IsStarred = true;
			Assert.IsFalse(x.ContainsEqualForm(form));
		}


		[Test]
		public void HasFormWithSameContent_DifferentStarred_False()
		{
			MultiText x = new MultiText();
			x["ws"] = "test";
			MultiText y = new MultiText();
			y["ws"] = "test";
			y.SetAnnotationOfAlternativeIsStarred("ws", true);
			Assert.IsFalse(x.HasFormWithSameContent(y));
			Assert.IsFalse(y.HasFormWithSameContent(x));
		}


		[Test]
		public void HasFormWithSameContent_OneEmpty_False()
		{
			MultiText x = new MultiText();
			MultiText y = new MultiText();
			y["ws"] = "test";
			y.SetAnnotationOfAlternativeIsStarred("ws", true);
			Assert.IsFalse(x.HasFormWithSameContent(y));
			Assert.IsFalse(y.HasFormWithSameContent(x));
		}

	}
}
