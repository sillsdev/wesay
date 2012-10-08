using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Linq;
using Palaso.Xml;
using WeSay.Project;

namespace Addin.Transform.PdfDictionary
{
	public class FLExCompatibleXhtmlWriter
	{
		private XmlWriter _writer;
		/// <summary>
		/// normally 'a', or 'b', etc, but can also be "ng", "th", etc.
		/// </summary>
		private string _currentLetterGroup = string.Empty;
		private bool _linkToUserCss;
		private readonly ViewTemplate _viewTemplate;

		public FLExCompatibleXhtmlWriter():this(false, new ViewTemplate())
		{
		}

		public FLExCompatibleXhtmlWriter(bool linkToUserCss, ViewTemplate viewTemplate)
		{
			_linkToUserCss = linkToUserCss;
			_viewTemplate = viewTemplate;
			Grouper = new MultigraphParser(new string[]{} );//property needs to be set by the client to get anything interesting
		}

		public MultigraphParser Grouper { get; set; }

		public void Write(TextReader pliftReader, TextWriter textWriter)
		{
			using (_writer = XmlWriter.Create(textWriter, CanonicalXmlSettings.CreateXmlWriterSettings()))
			{
				//  _writer.WriteProcessingInstruction("xml-stylesheet", @"type='text/css' href='dictionary.css");
				_writer.WriteRaw("<!DOCTYPE HTML>");
				_writer.WriteStartElement("html");
				_writer.WriteStartElement("head");
				_writer.WriteStartElement("meta");
				_writer.WriteAttributeString("charset", "UTF-8");
//            	_writer.WriteAttributeString("http-equiv", "content-type");
//				_writer.WriteAttributeString("content","text/html; charset=utf-8");
				_writer.WriteEndElement();


				//  Note: WriteRaw will not write chorus compliant formatting.  Use WriteNode instead. CP 2011-01
				//jh: Cambell, exported stuff shouldn't be part of chorus send/receive.
				if (_linkToUserCss)
				{
					_writer.WriteRaw("<LINK rel='stylesheet' href='autoLayout.css' type='text/css' />");
					_writer.WriteRaw("<LINK rel='stylesheet' href='autoFonts.css' type='text/css' />");
					_writer.WriteRaw("<LINK rel='stylesheet' href='customLayout.css' type='text/css' />");
					_writer.WriteRaw("<LINK rel='stylesheet' href='customFonts.css' type='text/css' />");
				}
				_writer.WriteEndElement();
				_writer.WriteStartElement("body");
				WriteClassAttr("dicBody");

				var doc = new XPathDocument(pliftReader);
				var navigator = doc.CreateNavigator();
				var entryIterator = navigator.Select("//entry");
				foreach (XPathNavigator entryNav in entryIterator)
				{
					XPathNavigator headwordFieldNode = entryNav.SelectSingleNode("field[@type='headword']");
					if(headwordFieldNode==null || string.IsNullOrEmpty(headwordFieldNode.Value))
						continue;
					AddLetterSectionIfNeeded(headwordFieldNode.Value);
					StartDiv("entry");
					OutputNonSenseFieldsOfEnry(entryNav);
					DoSenses(entryNav.Select("sense"), headwordFieldNode);
					EndDiv();//entry
				}

				if (_currentLetterGroup != string.Empty)
				{
					EndDiv(); //the last letHead div}
					EndDiv(); //the last letData div
				}
				_writer.WriteEndElement();//body
				_writer.WriteEndElement(); //html
			}
		}

		private void OutputNonSenseFieldsOfEnry(XPathNavigator entryNav)
		{
			XPathNodeIterator nodes = entryNav.Select("*[not(sense)]");
			List<string> encouteredRelationTypes = new List<string>();
			while (nodes.MoveNext())
			{
				switch (nodes.Current.Name)
				{
					case "relation":
						//we need to group up all relations of a type
						string rtype = nodes.Current.GetAttribute("type", string.Empty);
						if (encouteredRelationTypes.Contains(rtype))
							continue;
						encouteredRelationTypes.Add(rtype);
						DoRelationsOfType(entryNav, rtype);
						break;
					case "field":
						DoField(nodes.Current);
						break;
				}
			}
		}

		private void DoRelationsOfType(XPathNavigator entryNav, string rtype)
		{
			StartSpan("crossrefs");
			WriteSpan("crossref-type", "en", rtype);
			StartSpan("crossref-targets");
			XPathNodeIterator relationsOfOneType = entryNav.Select("relation[@type='"+rtype+"']");
			while(relationsOfOneType.MoveNext())
			{
				DoRelation(relationsOfOneType.Current);
			}
			EndSpan();
			EndSpan();
		}

		private void DoRelation(XPathNavigator relation)
		{
			XPathNavigator target=  relation.SelectSingleNode("field[@type='headword-of-target']");
			if (target == null)
				return;

////span[@class='crossrefs']/span[@class='crossref-targets' and count(span[@class='xitem']) == 2]");
			//string rtype = relation.GetAttribute("type",string.Empty);
			StartSpan("xitem");
			WriteSpan("crossref", GetLang(target), target.Value);
			EndSpan();

		}

		private void OutputHomographNumberIfNeeded(XPathNavigator headwordFieldNav)
		{
			var homographNumber = headwordFieldNav.SelectSingleNode("parent::entry").GetAttribute("order", string.Empty);
			if(!string.IsNullOrEmpty(homographNumber))
			{
				WriteSpan("xhomographnumber", "en"/*todo*/, homographNumber);
			}
		}

		private void DoSenses(XPathNodeIterator senses, XPathNavigator headwordFieldNode)
		{
			/*   <sense id="ASSOC_c33c51d4-f405-4d34-99c3-5eb36881a0d1">
	  <field type="grammatical-info">
		<form lang="en"><text>associative</text></form>
	  </field>
	  <example>
		<form lang="seh"><text>mwana wa Fátima</text></form>
		<translation>
		  <form lang="en"><text>child of Fatima</text></form>
		  <form lang="pt"><text>criança de Fátima</text></form>
		</translation>
	  </example>
	  <definition>
		<form lang="en"><text>of</text></form>
		<form lang="pt"><text>de</text></form>
	  </definition>
	</sense>
  *
  * TO
  *
  *  <span class="senses">
		<span class="sense" id="hvo6982">
			<span class="grammatical-info">
				<span class="partofspeech" lang="pt">Assoc</span>
			</span>
			<span class="definition_L2" lang="pt">
				<span class="xitem" lang="pt">
						<span class="xlanguagetag" lang="en">Por </span>
					de
				</span>
				<span class="xitem" lang="en"><span class="xlanguagetag" lang="en">Eng
				</span>of</span>
			</span>
			<span class="examples">
				<span class="example" lang="seh"><span class="xlanguagetag" lang="en">Sen
					</span>mwana wa Fátima</span>
				<span class="translations">
					<span class="translation_L2" lang="pt">
						<span class="xitem" lang="pt"><span class="xlanguagetag" lang="en">Por
							</span>criança de Fátima</span>
						<span class="xitem" lang="en"><span class="xlanguagetag" lang="en">Eng
							</span>child of Fatima</span>
					</span>
				</span>
			</span>
		</span>
	</span>
  */

			StartSpan("senses");
			var hasMultipleSenses = senses.Count > 1;
			while (senses.MoveNext())
			{
				StartSpan("sense");
				if (hasMultipleSenses)
				{
					WriteSpan("xsensenumber", "en", senses.CurrentPosition.ToString());
				}

				var nodes = senses.Current.Select("*");
				bool handledExamples = false;
				while(nodes.MoveNext())
				{
					switch(nodes.Current.Name)
					{
						case "illustration":
							DoIllustration(nodes.Current, headwordFieldNode);
							break;
						case "definition":
							DoDefinition(nodes.Current);
							break;
						case "field"://e.g. grammatical-info
							DoField(nodes.Current);
							break;
						case "example":
							if (!handledExamples)
							{
								handledExamples = true;
								DoExamples(senses.Current);
							}
							break;
					}

				}
				EndSpan();
			}
			EndSpan();

		}

		private void DoIllustration(XPathNavigator pictureNode, XPathNavigator headwordFieldNode)
		{

				var href = pictureNode.GetAttribute("href", string.Empty);
			var caption = pictureNode.GetAttribute("label", string.Empty);
			StartSpan("pictureRight");
			try
			{

				_writer.WriteStartElement("img");
				//            _writer.WriteAttributeString("src", string.Format("..{0}pictures{0}{1}", Path.DirectorySeparatorChar, href));
				var picturePath = href;

				//there was a version of lift where the "pictures" was explict, some where it isn't.
				if (!picturePath.StartsWith("pictures"))
				{
					picturePath = Path.Combine("pictures", picturePath);
				}
				_writer.WriteAttributeString("src", string.Format("..{0}{1}", Path.DirectorySeparatorChar, picturePath));
				_writer.WriteEndElement();


				if (!string.IsNullOrEmpty(caption))
				{
					StartDiv("pictureCaption");
					WriteSpan("pictureLabel", "en" /*todo*/, caption);
					EndDiv();
				}

				else if (headwordFieldNode != null && !string.IsNullOrEmpty(headwordFieldNode.Value))
				{
					var form = headwordFieldNode.SelectSingleNode(".//text").Value;
					var lang = headwordFieldNode.SelectSingleNode("form").GetAttribute("lang", "");
					StartDiv("pictureCaption");
					WriteSpan("pictureLabel", lang, form);
					EndDiv();
				}

			}
			catch (Exception)
			{
#if DEBUG
				throw;
#endif
			}
			finally { EndSpan(); }
		}

		private void DoExamples(XPathNavigator senseNav)
		{
			StartSpan("examples");
			XPathNodeIterator example = senseNav.Select("example");
			Debug.Assert(example.Count>0);

			while (example.MoveNext())
			{
				DoExample(example.Current);
			}
			EndSpan();
		}

		private void DoExample(XPathNavigator example)
		{
			//this is currently a weird structure, but we don't control it.
			bool foundTranslation = false;
			foreach (XPathNavigator form in example.SelectChildren("form", string.Empty))
			{
				WriteSpan("example", GetLang(form), form.Value);
				foreach (XPathNavigator translation in example.SelectChildren("translation", string.Empty))
				{
					foreach (XPathNavigator transForm in translation.SelectChildren("form", string.Empty))
					{
						if(!foundTranslation)
						{
							foundTranslation = true;
							StartSpan("translations");
						}
						WriteSpan("translation", GetLang(transForm), transForm.Value);
					}
				}
			}
			if(foundTranslation)
			{
				EndSpan();
			}
		}


		private void DoGrammaticalInfo(XPathNavigator gram)
		{
			StartSpan("grammatical-info");
			WriteSpan("partofspeech", GetLang(gram), gram.Value);
			EndSpan();
		}

		private void DoDefinition(XPathNavigator defNode)
		{
			/*       <definition>
		<form lang="en"><text>Evidential, "truly"</text></form>
		<form lang="pt"><text>Evidential, "mesmo"</text></form>
	  </definition>
			 *
			 *   <span class="definition_L2" lang="pt">
				<span class="xitem" lang="pt">
						<span class="xlanguagetag" lang="en">Por </span>
					de
				</span>
				<span class="xitem" lang="en"><span class="xlanguagetag" lang="en">Eng
				</span>of</span>
			</span>
			 */

		  //  StartSpan("definition_L2", "en");//todo: (en) we don't yet understand this weird lang followed by more lang specs.
			StartSpan("definition_L2");//todo: (en) we don't yet understand this weird lang followed by more lang specs.

			foreach (XPathNavigator form in defNode.SelectChildren("form",string.Empty))
			{
				WriteSpan("xitem", GetLang(form), form.Value);
			}

			EndSpan();
		}

		private string GetLang(XPathNavigator form)
		{
			return form.GetAttribute("lang", string.Empty);
		}

		private void AddLetterSectionIfNeeded(string headword)
		{
			if(string.IsNullOrEmpty(headword))
				return;

			var group = Grouper.GetFirstMultigraph(headword);
			if(group != _currentLetterGroup)
			{
				if(_currentLetterGroup != string.Empty)
				{
					EndDiv();//finish off the previous letData
					EndDiv();//finish off the previous letHead
				}
				_currentLetterGroup = group;
				StartDiv("letHead");
				StartDiv("letter");
				if(group.ToLowerInvariant() == group.ToUpperInvariant())
				{
					_writer.WriteValue(group.ToString());
				}
				else
				{
					_writer.WriteValue(CapitalizeFirstOnly(group) + " " + group.ToLowerInvariant());
				}
				EndDiv();
				StartDiv("letData");
			}
		}

		private string CapitalizeFirstOnly(string multigraph)
		{
			string output = multigraph.Substring(0, 1);
			if(multigraph.Length>1)
				output += multigraph.Substring(1).ToLowerInvariant();
			return output;
		}

		private void EndSpan()
		{
			_writer.WriteEndElement();
		}
		private void EndDiv()
		{
			_writer.WriteEndElement();
		}

		private void StartDiv(string className)
		{
			_writer.WriteStartElement("div");
			WriteClassAttr(className);
		}

		private void WriteClassAttr(string className)
		{
			_writer.WriteAttributeString("class", className);
		}

		private void DoField(XPathNavigator entryNav)
		{
			/**/

			var type = entryNav.GetAttribute("type", string.Empty);
			switch (type)
			{
				case "headword":
					DoHeadWord(entryNav);

					break;
				case "grammatical-info":
					DoGrammaticalInfo(entryNav);
					break;

				default:
					DoCustomField(entryNav);
					break;
			}
		}

		private void DoCustomField(XPathNavigator fieldNav)
		{
			var type = fieldNav.GetAttribute("type", string.Empty);
			XPathNodeIterator forms = fieldNav.SelectChildren("form", string.Empty);

			if (!forms.MoveNext())
				return;

			var label = _viewTemplate.GetField(type).DisplayName; //enhance: give the user a way to choose the label, separately from the on-screen label
			StartSpan("customFieldLabel", "en" /*we don't really know the label language*/, label);
			EndSpan();

			do
			{
				WriteSpan(type, GetLang(forms.Current), forms.Current.Value);
			} while (forms.MoveNext());
		}

		private void DoHeadWord(XPathNavigator headwordFieldNav)
		{
			/*  <field type="headword">
				  <form lang="v" first="true"><text>cosmos</text></form>
				</field>
			 *
			 * TO
			 *
			 *  <span class="headword" lang="seh">a</span>
			 */

			XPathNodeIterator forms = headwordFieldNav.SelectChildren("form", string.Empty);

			if(!forms.MoveNext())
				return;

			StartSpan("headword", GetLang(forms.Current), forms.Current.Value);
			OutputHomographNumberIfNeeded(headwordFieldNav);
			EndSpan();

			while(forms.MoveNext())
			{
				//NB: had to make up this style name, wasn't in FLEx yet.
				WriteSpan("headword-secondary", GetLang(forms.Current), forms.Current.Value);
				//notice, we're not bothering with homograph #s on these
			}
		}

		private void StartSpan(string className)
		{
			_writer.WriteStartElement("span");
			_writer.WriteAttributeString("class", className);
		}
		private void StartSpan(string className, string lang, string text)
		{
			_writer.WriteStartElement("span");
			_writer.WriteAttributeString("class", className);
			_writer.WriteAttributeString("lang", lang);
			_writer.WriteValue(text);
		}

		private void WriteSpan(string className, string lang, string text)
		{
			StartSpan(className,lang,text);
			_writer.WriteEndElement();
		}
/*
		private void WriteSpn(string className, string lang, string text)
		{
			StartSpan(className, lang, text);
			_writer.WriteEndElement();
		}

		private void StartSpan(string className, string lang)
		{
			_writer.WriteStartElement("span");
			_writer.WriteAttributeString("class", className);
			_writer.WriteAttributeString("lang", lang);
		}
		private string GetAttribute(XPathNavigator current, string name)
		{
			return current.GetAttribute(name, string.Empty);
		}
*/
	}
}