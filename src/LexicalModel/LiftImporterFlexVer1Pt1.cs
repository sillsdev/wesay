using System.Collections.Generic;
using System.Xml;
using WeSay.Foundation;
using WeSay.Language;

namespace WeSay.LexicalModel
{
   public  class LiftImporterFlexVer1Pt1 : LiftImporter
   {
	   /// <summary>
	   ///
	   /// </summary>
	   public LiftImporterFlexVer1Pt1()
		   : base()
	   {
	   }

	   /// <summary>
	   /// this takes a text, rather than returning one just because the
	   /// lexical model classes currently always create their MultiText fields during the constructor.
	   /// </summary>
	   public override void ReadMultiText(XmlNode node, MultiText text)
	   {
		   foreach (XmlNode form in node.SelectNodes("form"))
		   {
			   text.SetAlternative(GetManditoryAttributeValue(form,  "ws"), form.InnerText);
		   }
	   }

	   protected override void ReadGrammi(LexSense sense, XmlNode senseNode)
	   {
		   XmlNode grammi = senseNode.SelectSingleNode("grammi");
		   if (grammi == null)
		   {
			   return;
		   }
		   OptionRef o = sense.GetOrCreateProperty<OptionRef>(LexSense.WellKnownProperties.PartOfSpeech );
		   o.Value = GetManditoryAttributeValue(grammi, "type"); /////<----- this is the reason for the override.
													//in later versions of lift, this was changed to "value"
	   }
   }
}
