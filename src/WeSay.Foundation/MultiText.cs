using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
//using Exortech.NetReflector;
//using Exortech.NetReflector.Util;
using Palaso.Text;
using WeSay.Foundation;

namespace WeSay.Foundation
{
	/// <summary>
	/// MultiText holds an array of LanguageForms, indexed by writing system ID.
	/// </summary>
	//NO: we haven't been able to do a reasonalbly compact xml representation except with custom deserializer
	//[ReflectorType("multiText")]
	[XmlInclude(typeof (LanguageForm))]
	public class MultiText : MultiTextBase, IParentable, IReportEmptiness, System.Xml.Serialization.IXmlSerializable
	{
		private WeSayDataObject _parent=null;

		public MultiText(WeSayDataObject parent) : base()
		{
			_parent = parent;
		}


		public MultiText()
			: base()
		{

		}



		/// <summary>
		/// We have this pesky "backreference" solely to enable fast
		/// searching in our current version of db4o (5.5), which
		/// can find strings fast, but can't be queried for the owner
		/// quickly, if there is an intervening collection.  Since
		/// each string in WeSay is part of a collection of writing
		/// system alternatives, that means we can't quickly get
		/// an answer, for example, to the question Get all
		/// the Entries that contain a senes which matches the gloss "cat".
		///
		/// Using this field, we can do a query asking for all
		/// the LanguageForms matching "cat".
		/// This can all be done in a single, fast query.
		///  In code, we can then follow the
		/// LanguageForm._parent up to the multitext, then this _parent
		/// up to it's owner, etc., on up the hierarchy to get the Entries.
		///
		/// Subclasses should provide a property which set the proper class.
		///
		/// 23 Jan 07, note: starting to switch to using these for notifying parent of changes, too.
		/// </summary>
		[XmlIgnore]
		public WeSayDataObject Parent
		{
			protected get { return _parent; }
			set { _parent = value; }
		}


		/// <summary>
		/// Subclasses should provide a "Parent" property which set the proper class.
		/// </summary>
		public WeSayDataObject ParentAsObject
		{
			get { return Parent; }
		}



		#region IEnumerable Members

		#endregion

		#region IEquatable<MultiText> Members

		#endregion

		///<summary>
		/// required by IXmlSerializable
		///</summary>
		public XmlSchema GetSchema()
		{
			return null;
		}

		///<summary>
		/// required by IXmlSerializable.
		/// This is wierd and sad, but this is tuned to the format we want in OptionLists.
		///</summary>
		public void ReadXml(XmlReader reader)
		{
			//enhance: this is a maximally inefficient way to read it, but ok if we're just using it for option lists
			XmlDocument d = new XmlDocument();
			d.LoadXml(reader.ReadOuterXml());
			foreach (XmlNode form in d.SelectNodes("*/form"))
			{
				string s = form.InnerText.Trim().Replace('\n', ' ').Replace("  ", " ");
				if(form.Attributes.GetNamedItem("ws")!=null) //old style, but out there
				{
					this.SetAlternative(form.Attributes["ws"].Value, s);
				}
				else
				{
					this.SetAlternative(form.Attributes["lang"].Value, s);
				}
			}
//reader.ReadEndElement();
		}

		///<summary>
		/// required by IXmlSerializable.
		/// This is wierd and sad, but this is tuned to the format we want in OptionLists.
		///</summary>
	   public void WriteXml(XmlWriter writer)
		{
			foreach (LanguageForm form in Forms)
			{
				writer.WriteStartElement("form");
				writer.WriteAttributeString("lang", form.WritingSystemId);
				//notice, no <text> wrapper
				writer.WriteString(form.Form);
				writer.WriteEndElement();
			}
		}

		#region IReportEmptiness Members

		public bool ShouldHoldUpDeletionOfParentObject
		{
			get { return Empty; }
		}

		public bool ShouldCountAsFilledForPurposesOfConditionalDisplay
		{
			get { return !Empty; }
		}

		public bool ShouldBeRemovedFromParentDueToEmptiness
		{
			get { return Empty; }
		}

		public void RemoveEmptyStuff()
		{
			List<LanguageForm> condemened = new List<LanguageForm>();
			foreach (LanguageForm f in Forms)
			{
				if (string.IsNullOrEmpty(f.Form))
				{
					condemened.Add(f);
				}
			}
			foreach (LanguageForm f in condemened)
			{
				this.RemoveLanguageForm(f);
			}
		}

		#endregion

		public static new MultiText Create(Dictionary<string, string> forms)
		{
			MultiText m = new MultiText();
			CopyForms(forms, m);
			return m;
		}
	}
	/*
	public class MultiTextSerializorFactory : ISerialiserFactory
	{
		public IXmlMemberSerialiser Create(ReflectorMember member, ReflectorPropertyAttribute attribute)
		{
			return new MultiTextSerialiser(member, attribute);
		}
	}

	internal class MultiTextSerialiser : XmlMemberSerialiser
	{
		public MultiTextSerialiser(ReflectorMember member, ReflectorPropertyAttribute attribute)
			: base(member, attribute) {}

		public override object Read(XmlNode node, NetReflectorTypeTable table)
		{
			MultiText text = new MultiText();
			if (node != null)
			{
				foreach (XmlNode form in node.SelectNodes("form"))
				{
					text[form.Attributes["ws"].Value] = form.InnerText.Trim().Replace('\n', ' ').Replace("  ", " ");
				}
			}
			return text;
		}
	}*/
}