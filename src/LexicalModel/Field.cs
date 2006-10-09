using System;
using System.Collections.Generic;
using System.Xml;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;

namespace WeSay.LexicalModel
{
	[ReflectorType("field")]
	public class Field
	{
		private string _fieldName;
		private List<string> _writingSystemIds;

		public Field(string fieldName, string[] writingSystemIds)
		{
			FieldName = fieldName;
			if (writingSystemIds == null)
			{
				throw new ArgumentNullException();
			}
			else
			{
				WritingSystemIds = new List<string>(writingSystemIds);
			}
		}

		public Field()
		{
			_writingSystemIds = new List<string>();
		}

		[ReflectorCollection("name", Required = true)]
		public string FieldName
		{
			get
			{
				if (_fieldName == null)
				{
					throw new ArgumentNullException("FieldName");
				}
				return _fieldName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("FieldName");
				}
				_fieldName = value;
			}
		}


		[ReflectorProperty("writingSystems", typeof(WsIdCollectionSerializerFactory))]
		public List<string> WritingSystemIds
		{
			get
			{
				return _writingSystemIds;
			}
			set
			{
				int i = 0;
				foreach (string s in value)
				{
					i++;
					if (s == null)
					{
						throw new ArgumentNullException("writingSystem",
														"Writing System argument" + i.ToString() + "is null");
					}
				}
				_writingSystemIds = value;
			}
		}


		public bool HasWritingSystem(string writingSystemId)
		{
			return _writingSystemIds.Exists(
										delegate(string s)
										{
											return s == writingSystemId;
										});
		}

		#region persistence
		internal class WsIdCollectionSerializerFactory : ISerialiserFactory
		{
			public IXmlMemberSerialiser Create(ReflectorMember member, ReflectorPropertyAttribute attribute)
			{
				return new WsIdCollectionSerializer(member, attribute);
			}
		}

		internal class WsIdCollectionSerializer : XmlMemberSerialiser
		{
			public WsIdCollectionSerializer(ReflectorMember member, ReflectorPropertyAttribute attribute)
				: base(member, attribute)
			{ }

			public override void Write(XmlWriter writer, object target)
			{
				object value = target;
				writer.WriteStartElement("writingSystems");
				foreach (string s in ((Field)target)._writingSystemIds)
				{
					writer.WriteElementString("id", s);
				}
				writer.WriteEndElement();
			}

			public override object Read(XmlNode node, NetReflectorTypeTable table)
			{
				System.Diagnostics.Debug.Assert(node.Name == "writingSystems");
				System.Diagnostics.Debug.Assert(node != null);
				List<string> l = new List<string>();
				foreach (XmlNode n in node.SelectNodes("id"))
				{
					l.Add(n.InnerText);
				}
				return l;
			}
		}

		#endregion

	}
}
