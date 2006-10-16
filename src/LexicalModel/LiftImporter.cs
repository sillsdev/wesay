using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using WeSay.Language;

namespace WeSay.LexicalModel
{
   public  class LiftImporter
	{
	   private IList<LexEntry> _entries;
	   private int _nodesToImport;
	   public event EventHandler Progress;

	   /// <summary>
	   ///
	   /// </summary>
	   /// <param name="entries">An existing list to fill</param>
	   public LiftImporter(IList<LexEntry> entries)
	   {
		   this._entries = entries;
	   }

	   public int NodesToImport
	   {
		   get { return _nodesToImport; }
	   }

	   public IList<LexEntry> ReadFile(string path)
	   {
		   XmlDocument doc =new XmlDocument();
		   doc.Load(path);
		   XmlNodeList entryNodes = doc.SelectNodes("./lift/entry");
		   _nodesToImport = entryNodes.Count;
		   int count = 0;
		   const int kInterval = 50;
		   int nextProgressPoint = count + kInterval;
		   //tell anyonelistening that now is a good time to get our total expected cound
		   Progress.Invoke(this, new ProgressEventArgs(0));

		   foreach (XmlNode node in entryNodes)
		   {
			   this._entries.Add(this.ReadEntry(node));
			   count++;
			   if (count >= nextProgressPoint)
			   {
				   if (Progress != null)
				   {
					   ProgressEventArgs progressEventArgs = new ProgressEventArgs(count);
					   Progress.Invoke(this, progressEventArgs);
					   if (progressEventArgs.Cancel)
						   break;
				   }
				   nextProgressPoint = count + kInterval;
			   }
		   }
		   return this._entries;
	   }

	  public static void ReadMultiTextOrNull(XmlNode node, string query, MultiText text)
	   {
		   XmlNode element = node.SelectSingleNode(query);
		   if (element != null)
		   {
			   ReadMultiText(element,text);
		   }
	   }

	   /// <summary>
	   /// this takes a text, rather than returning one just because the
	   /// lexical model classes currently always create their MultiText fields during the constructor.
	   /// </summary>
		public static void ReadMultiText(XmlNode node, MultiText text)
	   {
		   foreach(XmlNode form in node.SelectNodes("form"))
		   {
			   text.SetAlternative(GetStringAttribute(form, "lang"), form.InnerText);
		   }
	   }

	   private static string GetStringAttribute(XmlNode form, string attr)
	   {
		   return form.Attributes[attr].Value;
	   }

	   public static LexExampleSentence ReadExample(XmlNode xmlNode)
	   {
		   LexExampleSentence example = new LexExampleSentence();
		   ReadMultiTextOrNull(xmlNode, "source", example.Sentence);
		   //NB: will only read in one translation
		   ReadMultiTextOrNull(xmlNode, "trans", example.Translation);
		   return example;
	   }

	   public LexSense ReadSense(XmlNode xmlNode)
	   {
		   LexSense sense = new LexSense();
		   ReadMultiTextOrNull(xmlNode, "gloss", sense.Gloss);
		   foreach (XmlNode n in xmlNode.SelectNodes("example"))
		   {
			   sense.ExampleSentences.Add(ReadExample(n));
		   }
		   return sense;
	   }

	   private static string GetOptionalAttributeString(XmlNode xmlNode, string name)
	   {
		   XmlAttribute attr= xmlNode.Attributes[name];
		   if (attr == null)
			   return null;
		   return attr.Value;
	   }

	   public LexEntry ReadEntry(XmlNode xmlNode)
	   {
		   LexEntry entry;
		   string guid = GetOptionalAttributeString(xmlNode, "id");
		   if (guid != null)
		   {
			   entry = new LexEntry(new Guid(guid));
		   }
		   else
		   {
			   entry = new LexEntry();
		   }
		   ReadMultiText(xmlNode, entry.LexicalForm);

		  foreach (XmlNode n in xmlNode.SelectNodes("sense"))
		   {
			   entry.Senses.Add(ReadSense(n));
		   }

		   return entry;
	   }
   }

   public class ProgressEventArgs : EventArgs
   {
	   private int _progress;
	   private bool _cancel=false;
	   public ProgressEventArgs(int progress)
	   {
		   _progress = progress;
	   }



	   public int Progress
	   {
		   get { return _progress; }
	   }

	   public bool Cancel
	   {
		   get { return _cancel; }
		   set { _cancel = value; }
	   }
   }
}
