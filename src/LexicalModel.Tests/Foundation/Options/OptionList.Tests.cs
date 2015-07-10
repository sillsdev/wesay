using System.IO;
using NUnit.Framework;
using SIL.IO;
using SIL.Lift.Options;
using WeSay.LexicalModel.Foundation.Options;

namespace WeSay.LexicalModel.Tests.Foundation.Options
{
    [TestFixture]
    public class OptionListTests
    {
        [SetUp]
        public void Setup() {}

        [TearDown]
        public void TearDown() {}

        [Test]
        public void Ddp4LoadFromFile_GetsExamples()
        {
            using (var file = new TempFile(
                              @"<?xml version='1.0' encoding='utf-8'?><optionsList>
  <option>
         <key>1.1 Sky</key>
         <name>
            <form ws='en'>Sky</form>
            <form ws='fr'>Ciel</form>
         </name>
         <abbreviation>
            <form ws='en'>1.1</form>
         </abbreviation>
         <description>
            <form ws='en'>Use this domain for words related to the sky.</form>
         </description>
         <searchKeys>
            <form ws='en'>sky, firmament, canopy, vault</form>
            <form ws='fr'>ciel, firmament</form>
            <form ws='en'>air, atmosphere, airspace, stratosphere, ozone layer</form>
            <form ws='en'>heaven, space, outer space, ether, void, solar system</form>
            <form ws='en'>aerial, heavenly, atmospheric, stratospheric, celestial,</form>
            <form ws='en'>blue, clear, black (night), cloudy, cloud filled, brilliant, dark, night sky, foreboding, stormy, dark, star studded</form>
            <form ws='fr'>bleu, noir, clair, nuageux, plein de nuages, brillant, ciel de nuit, orageux, étoilé, menaçant</form>
            <form ws='en'>horizon, skyline</form>
            <form ws='en'>heavenly body, celestial body, luminary</form>
            <form ws='fr'>l'horizon</form>
            <form ws='en'>aurora borealis, northern lights</form>
            <form ws='en'>aloft, up in the sky, up in the air,</form>
         </searchKeys>
      </option>
                 </optionsList>"))
            {

                OptionsList list = new DdpListReader().LoadFromFile(file.Path);
                Assert.AreEqual("1.1 Sky", list.Options[0].Key);
                Assert.AreEqual("Sky", list.Options[0].Name.GetBestAlternative("en"));
                Assert.AreEqual("Ciel", list.Options[0].Name.GetBestAlternative("fr"));
                var keys = list.Options[0].GetSearchKeys("en");
                Assert.IsTrue(keys.Contains("sky"));
                Assert.IsTrue(keys.Contains("canopy"));
                Assert.IsTrue(keys.Contains("solar system"));
                Assert.IsTrue(keys.Contains("aloft"));

            }
        }

        [Test]//WS-1293  Ddp names not matching because of extra spaces
        public void LoadFromile_KeyWithTrailingSpace_KeyRemoved()
        {
            using (var file = new TempFile(
                @"<?xml version='1.0' encoding='utf-8'?>
                 <optionsList>
		                <option>
                            <key>verb  </key>
			                <name>
				                <form ws='en'>verb</form>
			                </name>
		                </option>
                 </optionsList>"))
            {

                OptionsList list = Load(file.Path);
                Assert.AreEqual("verb", list.Options[0].Key);
             }
        }

        [Test]
        public void LoadFromFile()
        {
            string path = Path.GetTempFileName();
            File.WriteAllText(path,
                              @"<?xml version='1.0' encoding='utf-8'?><optionsList>
		                <option>
                            <key>verb</key>
			                <name>
				                <form ws='en'>verb</form>
				                <form ws='xy'>xyverb</form>
			                </name>
		                </option>
		                <option>
			                <key>noun</key>
			                <name>
				                <form ws='en'>noun</form>
				                <form ws='fr'>nom</form>
				                <form ws='es'>nombre</form>
				                <form ws='th'>นาม</form>
			                </name>
		                </option>
                 </optionsList>");

            OptionsList list = Load(path); 
            File.Delete(path);
            Assert.AreEqual("verb", list.Options[0].Key);
            Assert.AreEqual("verb", list.Options[0].Name.GetBestAlternative("en"));
            Assert.AreEqual("xyverb", list.Options[0].Name.GetBestAlternative("xy"));
            Assert.AreEqual("noun", list.Options[1].Name.GetBestAlternative("en"));
        }

        [Test]
        public void DeserializeWithEmptyOption()
        {
            string path = Path.GetTempFileName();
            File.WriteAllText(path,
                              @"<?xml version='1.0' encoding='utf-8'?><optionsList>
		                 <option>
                                <key>abc</key>
                                <name />
                                <abbreviation />
                                <description />
                              </option>
                 </optionsList>");

            OptionsList list = Load(path);
            File.Delete(path);
            Assert.IsNotNull(list.Options[0].Name.Forms);
            Assert.AreEqual(string.Empty, list.Options[0].Name.GetBestAlternative("z"));
        }

        private OptionsList Load(string path)
        {
            return new GenericOptionListReader().LoadFromFile(path);
        }

        [Test]
        public void LoadFromOldFormatFile()
        {
            string path = Path.GetTempFileName();
            File.WriteAllText(path,
                              @"<?xml version='1.0' encoding='utf-8'?><optionsList>
		                <options>
<option>
                            <key>verb</key>
			                <name>
				                <form ws='en'>verb</form>
				                <form ws='xy'>xyverb</form>
			                </name>
		                </option>
		                <option>
			                <key>noun</key>
			                <name>
				                <form ws='en'>noun</form>
				                <form ws='fr'>nom</form>
				                <form ws='es'>nombre</form>
				                <form ws='th'>นาม</form>
			                </name>
		                </option>
</options>
                 </optionsList>");

            OptionsList list = Load(path); 
            File.Delete(path);
            Assert.AreEqual("verb", list.Options[0].Key);
            Assert.AreEqual("verb", list.Options[0].Name.GetBestAlternative("en"));
            Assert.AreEqual("xyverb", list.Options[0].Name.GetBestAlternative("xy"));
            Assert.AreEqual("noun", list.Options[1].Name.GetBestAlternative("en"));
        }

        //        [Test]
        //        public void DeSerialize()
        //        {
        //            NetReflectorTypeTable t = new NetReflectorTypeTable();
        //            t.Add(typeof(OptionsList));
        //            t.Add(typeof(Option));
        //            t.Add(typeof(MultiText));
        //            t.Add(typeof(LanguageForm));
        //
        //            NetReflectorReader r = new NetReflectorReader(t);
        //
        //            OptionsList list = (OptionsList) r.Read(
        //                @"<optionsList>
        //	                <options>
        //		                <option>
        //                            <key>verb</key>
        //			                <name>
        //				                <form ws='en'><text>verb</text></form>
        //			                </name>
        //		                </option>
        //	                </options>
        //                </optionsList>");
        //
        //            Assert.AreEqual("verb", list.Options[0].Name.GetBestAlternative("en"));
        //            
        //        }

        //        [Test]
        //        public void SaveToFile()
        //        {
        //            StringWriter writer = new System.IO.StringWriter();
        //            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
        //            XmlAttributes ignoreAttr = new XmlAttributes();
        //            ignoreAttr.XmlIgnore = true;
        //            overrides.Add(typeof(Annotatable), "IsStarred", ignoreAttr);
        //
        //
        //            System.Xml.Serialization.XmlSerializer serializer = new XmlSerializer(typeof (OptionsList), overrides);
        //            
        //            Option x = new Option();
        //            x.Abbreviation.SetAlternative("a", "aabrev");
        //            x.Abbreviation.SetAlternative("b", "babrev");
        //            x.Key = "akey";
        //            x.Name.SetAlternative("a", "aname");
        //            x.Name.SetAlternative("b", "bname");
        //
        //            Option y = new Option();
        //            y.Abbreviation.SetAlternative("a", "aabrev");
        //            y.Abbreviation.SetAlternative("b", "babrev");
        //            y.Key = "akey";
        //            y.Name.SetAlternative("a", "aname");
        //            y.Name.SetAlternative("b", "bname");
        //
        //            OptionsList list = new OptionsList();
        //            list.Options.Add(x);
        //            list.Options.Add(y);
        //
        //
        //            serializer.Serialize(writer, list);
        //            string xml = writer.GetStringBuilder().ToString();
        //            Debug.WriteLine(xml);
        //            Assert.AreEqual("", xml);
        //        }

        [Test]
        public void SaveToFile()
        {
            Option x = new Option();
            x.Abbreviation.SetAlternative("a", "aabrev");
            x.Abbreviation.SetAlternative("b", "babrev");
            x.Key = "akey";
            x.Name.SetAlternative("a", "aname");
            x.Name.SetAlternative("b", "bname");

            Option y = new Option();
            y.Abbreviation.SetAlternative("a", "aabrev");
            y.Abbreviation.SetAlternative("b", "babrev");
            y.Key = "akey";
            y.Name.SetAlternative("a", "aname");
            y.Name.SetAlternative("b", "bname");

            OptionsList list = new OptionsList();
            list.Options.Add(x);
            list.Options.Add(y);

            string path = Path.GetTempFileName();
            list.SaveToFile(path);

            // Debug.WriteLine(xml);
            Load(path);
            Assert.AreEqual("aname", list.Options[1].Name.GetBestAlternative("a"));
        }


    }
}