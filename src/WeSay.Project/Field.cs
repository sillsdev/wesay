using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;

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
		public enum MultiplicityType { ZeroOr1 = 0 , ZeroOrMore = 1}
		private MultiplicityType _multiplicity = MultiplicityType.ZeroOr1;

		public enum BuiltInDataType
		{
			MultiText, Option, OptionCollection
		}

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


		[Description("The name of the field, as it will appear in the LIFT file. This is not visible to the WeSay user.")]
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
				_fieldName = value.Replace(" ","").Trim();//helpful when exposed to UI for user editting
			}
		}
		[Description("The label of the field as it will be displayed to the user.")]
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

		[TypeConverter(typeof(ParentClassConverter))]
		[Description("The parent of this field. E.g. Entry, Sense, Example.")]
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

		[Browsable(false)]
		public bool UserCanDeleteOrModify
		{
			get
			{
				if(IsBuiltInViaCode)
					return false;

				if (LexEntry.WellKnownProperties.Contains(this.FieldName))
					return false;

				if (LexSense.WellKnownProperties.Contains(this.FieldName))
					return false;

				if (LexExampleSentence.WellKnownProperties.Contains(this.FieldName))
					return false;

				return true;
			}
		}

		[TypeConverter(typeof(DataTypeClassConverter))]
		[Description("The type of the field. E.g. multilingual text, option, option collection, relation.")]
		[ReflectorProperty("dataType", Required = true)]
		public string DataTypeName
		{
			get { return _dataTypeName; }
			set { _dataTypeName = value; }
		}

		[Description("For options and option collections, the name of the xml file containing the valid set of options.")]
		[ReflectorProperty("optionsListFile", Required = false)]
		public string OptionsListFile
		{
			get { return _optionsListFile; }
			set { _optionsListFile = value; }
		}

		[Browsable(false)]
		public bool IsBuiltInViaCode
		{
			get
			{
				if (FieldName == FieldNames.EntryLexicalForm.ToString())
				{
					return true;
				}

				if (FieldName == FieldNames.ExampleSentence.ToString())
				{
					return true;
				}

				if (FieldName == FieldNames.ExampleTranslation.ToString())
				{
					return true;
				}

				if (FieldName == FieldNames.SenseGloss.ToString())
				{
					return true;
				}

				return false;
			}
		}


		public override string ToString()
		{
			return DisplayName;
		}

		[Browsable(false)]
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


		[Browsable(false)]
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		[Browsable(false)]
		[ReflectorCollection("visibility", Required = false)]
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
		[Browsable(false)]
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

		[Browsable(false)]
		[ReflectorProperty("multiplicity", Required = false)]
		public MultiplicityType Multiplicity
		{
			get { return _multiplicity; }
			set { _multiplicity = value; }
		}



		[Browsable(false)]
		public bool DoShow
		{
			get
			{
				return _visibility == CommonEnumerations.VisibilitySetting.Visible ||
					_visibility == CommonEnumerations.VisibilitySetting.ReadOnly;
			}
		}


		[Browsable(false)]
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

	class ParentClassConverter : WeSayStringConverter
	{
		public override string[] ValidStrings
		{
			get
			{
				return new string[] { "LexEntry", "LexSense", "LexExampleSentence" };
			}
		}
	}

	class DataTypeClassConverter : WeSayStringConverter
	{
		public override string[] ValidStrings
		{
			get
			{
				return new string[] { "MultiText", "Option", "OptionCollection", "RelationToOneEntry" };
			}
		}
	}

	abstract class WeSayStringConverter : StringConverter
	{
		abstract public string[] ValidStrings
		{
			get;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			//true means show a combobox
			return true;
		}

		//        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		//        {
		//            if ((String)value == String.Empty)
		//            {
		//                return "default";
		//            }
		//            else
		//            {
		//                return value;
		//            }
		//        }
		//
		//        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		//        {
		//            if ((String)value == "default")
		//            {
		//                return String.Empty;
		//            }
		//            else
		//            {
		//                return value;
		//            }
		//        }

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			//true will limit to list. false will show the list,
			//but allow free-form entry
			return true;
		}

		public override System.ComponentModel.TypeConverter.StandardValuesCollection
			  GetStandardValues(ITypeDescriptorContext context)
		{

			return new StandardValuesCollection(ValidStrings);
		}
	}
}
