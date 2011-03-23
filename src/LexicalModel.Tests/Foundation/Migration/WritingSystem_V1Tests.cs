using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Exortech.NetReflector;
using NUnit.Framework;
using WeSay.LexicalModel.Foundation;

namespace WeSay.LexicalModel.Tests.Foundation.Migration
{
	[TestFixture]
	public class WritingSystem_V1Tests
	{
		[Test]
		public void NoSetupDefaultFont()
		{
			WritingSystem_V1 ws = new WritingSystem_V1("xx", new Font("Times", 33));
			Assert.AreEqual(33, ws.Font.Size);
		}

		[Test]
		public void Construct_DefaultFont()
		{
			WritingSystem_V1 ws = new WritingSystem_V1();
			Assert.IsNotNull(ws.Font);
		}

		[Test]
		public void DeserializeOne()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof(WritingSystem_V1));
			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystem_V1 ws =
				(WritingSystem_V1)
				r.Read(
@"<WritingSystem>
	<Abbreviation>xx</Abbreviation>
	<CustomSortRules>B c d R</CustomSortRules>
	<FontName>Tahoma</FontName>
	<FontSize>99</FontSize>
	<Id>one</Id>
	<IsAudio>False</IsAudio>
	<IsUnicode>True</IsUnicode>
	<WindowsKeyman>IPA Unicode 5.1(ver 1.2 US) MSK</WindowsKeyman>
	<RightToLeft>False</RightToLeft>
	<SortUsing>one</SortUsing>
	<SpellCheckingId>xx</SpellCheckingId>
</WritingSystem>");
			// since Linux may not have Tahoma, we
			// need to test against the font mapping
			Font font = new Font("Tahoma", 99);
			Assert.IsNotNull(ws);
			Assert.AreEqual("xx", ws.Abbreviation);
			Assert.AreEqual("B c d R", ws.CustomSortRules);
			Assert.AreEqual(font.Name, ws.FontName);
			Assert.AreEqual(font.Size, ws.FontSize);
			Assert.AreEqual("one", ws.ISO);
			Assert.AreEqual(false, ws.IsAudio);
			Assert.AreEqual(true, ws.IsUnicode);
			Assert.AreEqual("IPA Unicode 5.1(ver 1.2 US) MSK", ws.KeyboardName);
			Assert.AreEqual(false, ws.RightToLeft);
			Assert.AreEqual("one", ws.SortUsing);
			Assert.AreEqual("xx", ws.SpellCheckingId);
		}

		[Test]
		public void CustomSortRules_SerializeAndDeserialize()
		{
			WritingSystem_V1 ws = new WritingSystem_V1("one", new Font("Arial", 99));
			ws.SortUsing = CustomSortRulesType.CustomICU.ToString();

			string rules = "&n < ng <<< Ng <<< NG";
			ws.CustomSortRules = rules;

			string s = NetReflector.Write(ws);

			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystem_V1));
			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystem_V1 wsRead = (WritingSystem_V1) r.Read(s);
			Assert.IsNotNull(wsRead);
			Assert.AreEqual(rules, ws.CustomSortRules);
		}

		[Test]
		public void DeserializeOne_CustomSortRules_Before_SortUsing()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystem_V1));
			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystem_V1 ws =
				(WritingSystem_V1)
				r.Read(
					"<WritingSystem><CustomSortRules>test</CustomSortRules><SortUsing>CustomSimple</SortUsing><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id></WritingSystem>");
			Assert.IsNotNull(ws);
			Assert.AreEqual("test", ws.CustomSortRules);
			Assert.AreEqual(CustomSortRulesType.CustomSimple.ToString(), ws.SortUsing);
		}

		[Test]
		public void DeserializeOne_SortUsing_Before_CustomSortRules()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystem_V1));
			NetReflectorReader r = new NetReflectorReader(t);
			WritingSystem_V1 ws =
				(WritingSystem_V1)
				r.Read(
					"<WritingSystem><SortUsing>CustomSimple</SortUsing><CustomSortRules>test</CustomSortRules><FontName>Tahoma</FontName><FontSize>99</FontSize><Id>one</Id></WritingSystem>");
			Assert.IsNotNull(ws);
			Assert.AreEqual("test", ws.CustomSortRules);
			Assert.AreEqual(CustomSortRulesType.CustomSimple.ToString(), ws.SortUsing);
		}
	}
}
