using System;
using System.Collections.Generic;
using System.Xml;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;
using WeSay.Foundation;
using WeSay.Language;


namespace WeSay.Project
{
	[ReflectorType("field")]
	public class Field
	{
		private string _fieldName;
		private List<string> _writingSystemIds;
		private string _displayName=string.Empty;
		private string _description=string.Empty;
		private string _className=string.Empty;
		private string _dataTypeName;
		public enum MultiplicityType { ZeroOr1 = 0 }
		private MultiplicityType _multiplicity = MultiplicityType.ZeroOr1;

		private CommonEnumerations.VisibilitySetting _visibility=CommonEnumerations.VisibilitySetting.Visible ;
		private string _optionsListFile;

		/// <summary>
		/// These are just for getting the strings right, using ToString(). In order
		/// To allow extensions later, we aren't using these as a closed list of possible
		/// values.
		/// </summary>
		public enum FieldNames {EntryLexicalForm, SenseGloss, ExampleSentence, ExampleTranslation};

		public Field()
		{
			Initialize("unknown", "MultiText", MultiplicityType.ZeroOr1, new List<string>());
		}


		public Field(string fieldName, string className, IEnumerable<string> writingSystemIds)
			:this(fieldName, className, writingSystemIds,MultiplicityType.ZeroOr1,"MultiText")
		{
		}

		public Field(string fieldName, string className, IEnumerable<string> writingSystemIds, MultiplicityType multiplicity, string dataTypeName)
		{
			if (writingSystemIds == null)
			{
				throw new ArgumentNullException();
			}
			ClassName = className;
			Initialize(fieldName, dataTypeName, multiplicity, writingSystemIds);

		}

		public Field(Field field)
		{
			FieldName = field.FieldName;
			ClassName = field.ClassName;
			_writingSystemIds = new List<string>();
			foreach (string id in field.WritingSystemIds)
			{
				WritingSystemIds.Add(id);
			}
			Description =  field.Description;
			DisplayName = field.DisplayName;
			Multiplicity= field.Multiplicity;
			Visibility =  field.Visibility;
			DataTypeName = field.DataTypeName;
			OptionsListFile = field.OptionsListFile;
		}


		private void Initialize(string fieldName, string dataTypeName, MultiplicityType multiplicity, IEnumerable<string> writingSystemIds)
		{
			FieldName = fieldName;
			WritingSystemIds = new List<string>(writingSystemIds);
			this._multiplicity = multiplicity;
			DataTypeName = dataTypeName;
		}

		public bool IsCustom
		{
			get
			{
				if (FieldName == FieldNames.EntryLexicalForm.ToString())
				{
					return false;
				}

				if (FieldName == FieldNames.ExampleSentence.ToString())
				{
					return false;
				}

				if (FieldName == FieldNames.ExampleTranslation.ToString())
				{
					return false;
				}

				if (FieldName == FieldNames.SenseGloss.ToString())
				{
					return false;
				}

				return true;
			}
		}

		[ReflectorCollection("className", Required = true)]
		public string ClassName
		{
			get
			{
				if(_className.Length == 0)
				{
					throw new InvalidOperationException("className has not been initialized correctly");
				}
				return _className;
			}
			set
			{
				switch(value)
				{
					case null:
						throw new ArgumentNullException();
					case "LexEntry":
					case "LexSense":
					case "LexExampleSentence":
						_className = value;
						break;
					default:
						throw new ArgumentOutOfRangeException("className must be LexEntry or LexSense or LexExampleSentence");

				}
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
			return string.Format(_displayName);
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
		public CommonEnumerations.VisibilitySetting Visibility
		{
			get { return _visibility; }
			set { _visibility = value; }
		}

		public void ChangeWritingSystemId(string oldId, string newId)
		{
			int i = _writingSystemIds.FindIndex(delegate(string id) { return id == oldId; });
			if(i>-1)
			{
				_writingSystemIds[i] = newId;
			}
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

		[ReflectorProperty("dataType", Required = true)]
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

		public bool DoShow
		{
			get
			{
				return _visibility == CommonEnumerations.VisibilitySetting.Visible ||
					_visibility == CommonEnumerations.VisibilitySetting.ReadOnly;
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

		public static void ModifyMasterFromUser(Field master, Field user)
		{
			// this worked so long as the master had all possible valid writing systems in each field
			//          master.WritingSystemIds = GetIntersectionOfWsIdLists(master, user);
			master.WritingSystemIds = user.WritingSystemIds;
			master.Visibility = user.Visibility;
		}

//        private static List<string> GetIntersectionOfWsIdLists(Field master, Field user)
//        {
//            List<string> l = new List<string>();
//            foreach (string id in master.WritingSystemIds)
//            {
//                if (user.WritingSystemIds.Contains(id))
//                {
//                    l.Add(id);
//                }
//            }
//            return l;
//        }

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
