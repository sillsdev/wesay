using System.Collections.Generic;
using System.Xml;
using WeSay.Language;

namespace WeSay.LexicalModel
{
   public  class LiftImporterFlexVer1Pt1 : LiftImporter
   {
	   /// <summary>
	   ///
	   /// </summary>
	   /// <param name="entries">An existing list to fill</param>
	   public LiftImporterFlexVer1Pt1(IList<LexEntry> entries)
		   : base(entries)
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
			   text.SetAlternative(GetStringAttribute(form, "ws"), form.InnerText);
		   }
	   }
   }
}
