using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Palaso.Text;
using WeSay.Foundation;
using WeSay.Foundation.Options;

namespace WeSay.LexicalModel.Foundation.Options
{
	public class LiftRangeWriter
	{
		private readonly string _rangeId;

		public LiftRangeWriter(string rangeId)
		{
			_rangeId = rangeId;
		}

		/// <summary>
		/// save the list, either by creating the file, or modifying an existing one, leave any other existing lists untouched
		/// </summary>
		public void Save(string path, OptionsList optionsList)
		{
			XmlNode rootNode=null;
			var dom = new XmlDocument();
			if(File.Exists(path))
			{
				dom.Load(path);
				rootNode = dom.SelectSingleNode("lift-ranges");
				if(rootNode == null)
				{
					Palaso.Reporting.ErrorReport.NotifyUserOfProblem("The file {0} seem to be malformed.  It will be removed and re-written.", path);
					File.Delete(path);
					dom = new XmlDocument();
				}
			}

			if (rootNode == null)
			{
				 rootNode = dom.CreateNode(XmlNodeType.Element, "lift-ranges", string.Empty);
				dom.AppendChild(rootNode);
			}

			var rangeNode = rootNode.SelectSingleNode("//range[@id='" + _rangeId + "']");
			if (rangeNode == null)
			{
				rangeNode = dom.CreateNode(XmlNodeType.Element, "range", string.Empty);
				rootNode.AppendChild(rangeNode);
				Palaso.XmlHelpers.AddOrUpdateAttribute(rangeNode, "id", _rangeId);
			}
			else
			{
				rangeNode.InnerXml = string.Empty;//clear it out
			}


			foreach (var option in optionsList.Options)
			{
				var optionNode = dom.CreateNode(XmlNodeType.Element, "range-element", string.Empty);
				Palaso.XmlHelpers.AddOrUpdateAttribute(optionNode, "id", option.Key);
				//TODO: add other, custom attributes (FLEx uses guid and parentguid)

				optionNode.InnerXml = option.Abbreviation.GetXmlAsElementFormText("abbrev")
									+ option.Name.GetXmlAsElementFormText("label")
									+ option.Description.GetXmlAsElementFormText("description");

				rangeNode.AppendChild(optionNode);
			}

			dom.Save(path);
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