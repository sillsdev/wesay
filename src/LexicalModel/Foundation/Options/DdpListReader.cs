using System.Xml;
using System.Xml.Serialization;
using SIL.Lift;
using SIL.Lift.Options;
using WeSay.LexicalModel.Foundation.Options;

namespace WeSay.LexicalModel.Foundation.Options
{
	public class DdpListReader : GenericOptionListReader
	{
		protected override XmlSerializer GetSerializer()
		{
			//Make it use our custom deserializer for SearchKeys

			XmlAttributes attrs = new XmlAttributes();

			/* Create an XmlElementAttribute to override the
			field that returns Instrument objects. The overridden field
			returns Brass objects instead. */
			XmlElementAttribute attr = new XmlElementAttribute();
			attr.ElementName = "searchKeys";
			attr.Type = typeof(ExampleFormsMultiText);

			attrs.XmlElements.Add(attr);
			XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();

			/* Add the type of the class that contains the overridden
			member and the XmlAttributes to override it with to the
			XmlAttributeOverrides object. */
			attrOverrides.Add(typeof(Option), "SearchKeys", attrs);

			return new XmlSerializer(typeof(OptionsList), attrOverrides);
		}



	}

	/// <summary>
	/// this class existing only to run some custom deserialization code,
	/// which concatenates all the <forms/>
	/// There may be a better way.
	/// </summary>
	public class ExampleFormsMultiText : MultiText
	{
		///<summary>
		/// from IXmlSerializable.
		///</summary>
		public override void ReadXml(XmlReader reader)
		{
			XmlDocument d = new XmlDocument();
			d.LoadXml(reader.ReadOuterXml());
			foreach (XmlNode form in d.SelectNodes("*/form"))
			{
				string s = form.InnerText.Trim().Replace('\n', ' ').Replace("  ", " ");
				string ws;
				if (form.Attributes.GetNamedItem("ws") != null) //old style, but out there
				{
					ws = form.Attributes.GetNamedItem("ws").Value;
				}
				else
				{
					ws = form.Attributes.GetNamedItem("lang").Value;
				}
				AppendForm(ws, s);
			}
		}

		/// <summary>
		/// we have a weird format coming in, where examples separated by commans and
		/// are spread between multiple "<form/>s"
		/// </summary>
		/// <param name="ws"></param>
		/// <param name="s"></param>
		private void AppendForm(string ws, string s)
		{
			s = s.TrimEnd(new char[] { ',', ' ' });//fieldworks has extra commas
			string existing = GetExactAlternative(ws);
			if(existing != string.Empty)
			{
				existing += ", ";
			}
			SetAlternative(ws, existing + s);
		}
	}
}