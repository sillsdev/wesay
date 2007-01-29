using System;
using System.Collections.Generic;
using System.Xml;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;
using WeSay.Language;


namespace WeSay.Project
{
	[ReflectorType("field")]
	public class Field
	{
		private string _fieldName;
		private List<string> _writingSystemIds;
		private string _displayName="";
		private string _description="";
		private string _className="";
		private string _dataTypeName;
		public enum MultiplicityType { ZeroOr1 = 0 }
		private MultiplicityType _multiplicity = MultiplicityType.ZeroOr1;

		public enum VisibilitySetting {Invisible, Visible};
		private VisibilitySetting _visibility=VisibilitySetting.Visible ;
		private string _optionsListFile;

		/// <summary>
		/// These are just for getting the strings right, using ToString(). In order
		/// To allow extensions later, we aren't using these as a closed list of possible
		/// values.
		/// </summary>
		public enum FieldNames {EntryLexicalForm, SenseGloss, ExampleSentence, ExampleTranslation};

		public Field()
			:this("unknown",new List<string>(),MultiplicityType.ZeroOr1,"MultiText")
		{
		}


		public Field(string fieldName, IEnumerable<string> writingSystemIds)
			:this(fieldName,writingSystemIds,MultiplicityType.ZeroOr1,"MultiText")
		{
		}

		public Field(string fieldName, IEnumerable<string> writingSystemIds, MultiplicityType multiplicity, string dataTypeName)
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
			_multiplicity = multiplicity;
			DataTypeName = dataTypeName;
		}


		public bool IsCustom
		{
			get
			{
				return this.ClassName != ""; //TEMP
			}
		}

		[ReflectorCollection("className", Required = false)]
		public string ClassName
		{
			get
			{
				return _className;
			}
			set
			{
				_className = value;
			}
		}


		[ReflectorCollection("fieldName", Required = true)]
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

		public override string ToString()
		{
			return _fieldName;
		}

		[ReflectorProperty("writingSystems", typeof(WsIdCollectionSerializerFactory))]
		public IList<string> WritingSystemIds
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
				_writingSystemIds = new List<string>(value);
			}
		}

		[ReflectorCollection("displayName", Required=false)]
		public string DisplayName
		{
			get
			{
				if(_displayName=="")
				{
					return "*"+FieldName;
				}
				return _displayName;
			}
			set { _displayName = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		[ReflectorCollection("visibility", Required=false)]
		public VisibilitySetting Visibility
		{
			get { return _visibility; }
			set { _visibility = value; }
		}

		public IList<WritingSystem> WritingSystems
		{
			get
			{
				List<WritingSystem> l = new List<WritingSystem>();
				foreach (string id in _writingSystemIds)
				{
					l.Add(BasilProject.Project.WritingSystems[id]);
				}
				return l;
			}
		}

		[ReflectorProperty("multiplicity", Required = false)]
		public MultiplicityType Multiplicity
		{
			get { return _multiplicity; }
			set { _multiplicity = value; }
		}

		[ReflectorProperty("dataType", Required = false)]
		public string DataTypeName
		{
			get { return _dataTypeName; }
			set { _dataTypeName = value; }
		}

		[ReflectorProperty("optionsListFile", Required = false)]
		public string OptionsListFile
		{
			get { return _optionsListFile; }
			set { _optionsListFile = value; }
		}


		public bool HasWritingSystem(string writingSystemId)
		{
			return _writingSystemIds.Exists(
										delegate(string s)
										{
											return s == writingSystemId;
										});
		}

		public static void ModifyMasterFromUser(Field master, Field user)
		{
			master.WritingSystemIds = GetIntersectionOfWsIdLists(master, user);
			master.Visibility = user.Visibility;
		}

		private static List<string> GetIntersectionOfWsIdLists(Field master, Field user)
		{
			List<string> l = new List<string>();
			foreach (string id in master.WritingSystemIds)
			{
				if (user.WritingSystemIds.Contains(id))
				{
					l.Add(id);
				}
			}
			return l;
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
