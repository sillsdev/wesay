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

		/// <summary>
		/// hack for netreflector use only
		/// </summary>
		[ReflectorProperty("writingSystems", Required = false)]
		public List<WritingSystemId> WrappedWritingSystemIds
		{
			get
			{
				List<WritingSystemId> l = new List<WritingSystemId>();
				foreach (string s in _writingSystemIds)
				{
					l.Add(new WritingSystemId(s));
				}
				return l;
			}
			set
			{
				_writingSystemIds.Clear();
				foreach (WritingSystemId id in value)
				{
					_writingSystemIds.Add(id._id);
				}
			}
		}

		[ReflectorType("writingSystem")]
		public class WritingSystemId
		{
		 [ReflectorProperty("id", typeof(WsIdFactory))]
		  public  string _id;

			public WritingSystemId()
			{

			}

		   public WritingSystemId(string s)
		   {
			   _id = s;
		   }


			internal class WsIdFactory : ISerialiserFactory
			{
				public IXmlMemberSerialiser Create(ReflectorMember member, ReflectorPropertyAttribute attribute)
				{
					return new WsIdSerializer(member, attribute);
				}
			}

			internal class WsIdSerializer : XmlMemberSerialiser
			{
				public WsIdSerializer(ReflectorMember member, ReflectorPropertyAttribute attribute)
					: base(member, attribute)
				{ }

				public override void Write(XmlWriter writer, object target)
				{
					object value =target;
					writer.WriteStartElement("id");
					WriteValue(writer, value.ToString());
					writer.WriteEndElement();
				}

				public override object Read(XmlNode node, NetReflectorTypeTable table)
				{
					System.Diagnostics.Debug.Assert(node.Name == "id");
					System.Diagnostics.Debug.Assert(node != null);
					return node.InnerText;
				}
			}
		}

//        public Field(string FieldName, params string[] writingSystems)
//        {

//            if(writingSystems == null)
//            {
//                throw new ArgumentNullException("writingSystems");
//            }
//            int i = 0;
//            foreach (string s in writingSystems)
//            {
//                i++;
//                if(s==null)
//                {
//                    throw new ArgumentNullException("writingSystem",
//                                                    "Writing System argument" + i.ToString() + "is null");
//                }
//            }
//            _fieldName = FieldName;
//            _writingSystems = writingSystems;
//        }

		public bool HasWritingSystem(string writingSystemId)
		{
			return _writingSystemIds.Exists(
										delegate(string s)
										{
											return s == writingSystemId;
										});
		}
	}
}
