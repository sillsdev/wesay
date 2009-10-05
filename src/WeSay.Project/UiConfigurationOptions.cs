using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WeSay.Project
{
	[XmlRoot("uiOptions")]
	public class UiConfigurationOptions
	{
		[XmlElement("language")]
		public string Language { get; set; }

		[XmlElement("labelFontName")]
		public string LabelFontName { get; set; }

		[XmlElement("labelFontSizeInPoints")]
		public float LabelFontSizeInPoints{get; set;}

		public UiConfigurationOptions()
		{
			Language = "wesay-en";
			LabelFontName = SystemFonts.CaptionFont.FontFamily.Name;
			LabelFontSizeInPoints = SystemFonts.CaptionFont.SizeInPoints;
		}

		/// <returns>null if not found in the dom</returns>
		public static UiConfigurationOptions CreateFromDom(XmlDocument dom)
		{
			var node = dom.SelectSingleNode("//uiOptions");
			if (node == null)
				return null;
			using (var reader = new StringReader(node.OuterXml))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(UiConfigurationOptions));
				var uiOptions = (UiConfigurationOptions)serializer.Deserialize(reader);
				return uiOptions;
			}
		}

		public void Save(XmlWriter writer)
		{
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add("", "");//don't add the silly namespace on the element
			XmlSerializer serializer = new XmlSerializer(typeof(UiConfigurationOptions));
			serializer.Serialize(writer, this, ns);
		}

		public Font GetLabelFont()
		{
			return new Font(LabelFontName, LabelFontSizeInPoints);
		}

		public void SetLabelFont(Font font)
		{
			LabelFontName = font.FontFamily.Name;
			LabelFontSizeInPoints = font.SizeInPoints;
		}
	}
}
