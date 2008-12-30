using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.Xml.XPath;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;

namespace Addin.Transform
{
	public class FLExCompatibleXhtmlWriter
	{
		private XmlWriter _writer;
		private char _currentLetter;

		public FLExCompatibleXhtmlWriter()
		{
		}

		//        public void Write(TextWriter textWriter)
		//        {
		//            using (var writer = XmlWriter.Create(textWriter))
		//            {
		//                foreach (var entry in _repo.GetAllEntriesSortedByHeadword(_template.HeadwordWritingSystems[0]))
		//                {
		//                    writer.WriteStartElement("div");
		//                    writer.WriteAttributeString("class", "entry");
		//
		//                    writer.WriteEndElement();
		//                }
		//            }
		//      }

		public void Write(string pliftSource, TextWriter textWriter)
		{
			using (_writer = XmlWriter.Create(textWriter))
			{
				_writer.WriteStartElement("html");
				_writer.WriteStartElement("head");
				_writer.WriteEndElement();
				_writer.WriteStartElement("body");
				WriteClassAttr("dicBody");

				var doc = new XPathDocument(new StringReader(pliftSource));
				var navigator = doc.CreateNavigator();
				var entryIterator = navigator.Select("//entry");
				foreach (XPathNavigator entryNav in entryIterator)
				{
					XPathNavigator headwordFieldNode = entryNav.SelectSingleNode("field[@type='headword']");
					if(headwordFieldNode==null || string.IsNullOrEmpty(headwordFieldNode.Value))
						continue;
					AddLetterSectionIfNeeded(headwordFieldNode);
					StartDiv("entry");

					XPathNodeIterator nodes = entryNav.Select("*[not(sense)]");
					while (nodes.MoveNext())
					{
						switch (nodes.Current.Name)
						{
							case "field":
								DoField(nodes.Current);
								break;
						}
					}
					DoSenses(entryNav.Select("sense"));
					_writer.WriteEndElement();
				}

				if (_currentLetter != default(char))
				{
					EndDiv(); //the last letHead div}
					EndDiv(); //the last letData div
				}
				_writer.WriteEndElement();//body
				_writer.WriteEndElement(); //html
			}
		}

		private void DoSenses(XPathNodeIterator senses)
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

			while (senses.MoveNext())
			{
				StartSpan("sense");
				var nodes = senses.Current.Select("*");
				bool foundExample = false;
				while(nodes.MoveNext())
				{
					switch(nodes.Current.Name)
					{
						case "definition":
							DoDefinition(nodes.Current);
							break;
						case "field"://e.g. grammatical-info
							DoField(nodes.Current);
							break;
						case "example":
							if(!foundExample)
							{
								foundExample = true;
								StartSpan("examples");
							}
							DoExample(nodes.Current);
							break;
					}

				}
				if (foundExample)
				{
					EndSpan();//examples
				}
				EndSpan();
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

			StartSpan("definition_L2", "en");//todo: (en) we don't yet get this weird lang followed by more lang specs.

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

		private void AddLetterSectionIfNeeded(XPathNavigator headwordFieldNode)
		{
			char letter = Char.ToUpper(headwordFieldNode.Value[0]);

			if(letter != _currentLetter)
			{
				if(_currentLetter != default(char))
				{
					EndDiv();//finish off the previous letData
					EndDiv();//finish off the previous letHead
				}
				_currentLetter = letter;
				StartDiv("letHead");
				StartDiv("letter");
				_writer.WriteValue(letter + " " + char.ToLower(letter));
				EndDiv();
				StartDiv("letData");
			}
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

		private void DoField(XPathNavigator current)
		{
			/**/

			var type = current.GetAttribute("type", string.Empty);
			switch (type)
			{
				case "headword":
					DoHeadWord(current);
					break;
				case "grammatical-info":
					DoGrammaticalInfo(current);
					break;
			}
		}

		private void DoHeadWord(XPathNavigator current)
		{
			/*  <field type="headword">
				  <form lang="v" first="true"><text>cosmos</text></form>
				</field>
			 *
			 * TO
			 *
			 *  <span class="headword" lang="seh">a</span>
			 */

			WriteSpan("headword", GetAttribute(current, "lang"), current.Value);
		}

		private void StartSpan(string className)
		{
			_writer.WriteStartElement("span");
			_writer.WriteAttributeString("class", className);
		}

		private void WriteSpan(string className, string lang, string text)
		{
			_writer.WriteStartElement("span");
			_writer.WriteAttributeString("class",className);
			_writer.WriteAttributeString("lang", lang);
			_writer.WriteValue(text);
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
	}
}
