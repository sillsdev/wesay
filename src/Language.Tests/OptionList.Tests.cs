using System;
using System.Collections.Generic;
using System.IO;
using Exortech.NetReflector;
using NUnit.Framework;
using WeSay.Foundation;

namespace WeSay.Language.Tests
{
	[TestFixture]
	public class OptionListTests
	{
		[SetUp]
		public void Setup()
		{

		}

		[TearDown]
		public void TearDown()
		{

		}

		[Test]
		public void DeSerialize()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof(OptionsList));
			t.Add(typeof(Option));
			t.Add(typeof(MultiText));
			t.Add(typeof(LanguageForm));

			NetReflectorReader r = new NetReflectorReader(t);

			OptionsList list = (OptionsList) r.Read(
				@"<optionsList>
					<options>
						<option>
							<name>
								<key>verb</key>
								<form ws='en'>verb</form>
							</name>
						</option>
					</options>
				</optionsList>");

			Assert.AreEqual("verb", list.Options[0].Name.GetAlternative("en"));

		}

	}

}