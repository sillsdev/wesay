using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Palaso.Text;
using WeSay.Foundation;
using WeSay.Foundation.Options;

namespace WeSay.LexicalModel.Foundation.Options
{
	public class LiftRangeReader : IOptionListReader
	{
		private readonly string _rangeId;

		public LiftRangeReader(string rangeId)
		{
			_rangeId = rangeId;
		}

		public OptionsList LoadFromFile(string path)
		{
			var dom = new XmlDocument();
			dom.Load(path);
			var range = new OptionsList();
			var nodes = dom.SelectNodes("//range[@id='" + _rangeId + "']/range-element");
			if(nodes==null)
				return range;

			foreach (XmlNode optionNode in nodes)
			{
				var option = new Option();
				option.Key = Palaso.XmlHelpers.GetAttributeValue(optionNode, "id");
				option.Name = ReadMultiText(optionNode.SelectSingleNode("label"));
				option.Abbreviation = ReadMultiText(optionNode.SelectSingleNode("abbrev"));
				option.Description = ReadMultiText(optionNode.SelectSingleNode("description"));

				//TODO: add other, custom attributes (FLEx uses guid and parentguid)

				range.Options.Add(option);
			}


			return range;
	   }

		public MultiText ReadMultiText(XmlNode node)
		{

			var text = new MultiText();
			if(node==null)
				return text;

			var nodes = node.SelectNodes("form");
			if(nodes==null)
				return text;
			foreach (XmlNode form in nodes)
			{
				string s = form.InnerText.Trim().Replace('\n', ' ').Replace("  ", " ");
				var item = form.Attributes.GetNamedItem("lang");
				if (item != null)
				{
					string ws = item.Value;
					text.SetAlternative(ws, s);
				}
			}
			return text;
		}
   }
}