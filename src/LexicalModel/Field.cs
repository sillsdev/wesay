using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;
using SIL.DictionaryServices.Model;
using SIL.Lift;
using SIL.WritingSystems;

namespace WeSay.LexicalModel
{
	[ReflectorType("field")]
	public class Field
	{
		private string _fieldName;
		private BindingList<string> _writingSystemIds;
		private string _displayName = string.Empty;
		private string _description = string.Empty;
		private string _className = string.Empty;
		private string _dataTypeName;
		private bool _enabled;
		private bool _isSpellCheckingEnabled;
		private bool _isMeaningField;
		//private string _configurationName;

		private bool _enabledNotSet = true;

		public enum MultiplicityType
		{
			ZeroOr1 = 0,
			ZeroOrMore = 1
		}

		private MultiplicityType _multiplicity = MultiplicityType.ZeroOr1;

		public enum BuiltInDataType
		{
			MultiText,
			Option,
			OptionCollection,
			Picture,
			RelationToOneEntry
		}

		private CommonEnumerations.VisibilitySetting _visibility =
				CommonEnumerations.VisibilitySetting.Visible;

		private string _optionsListFile;
		private bool _isMultiParagraph;

		/// <summary>
		/// These are just for getting the strings right, using ToString(). In order
		/// To allow extensions later, we aren't using these as a closed list of possible
		/// values.
		///
		///
		/// NB: use of these is deprecated.  Better to use the WellKnownProperties of the class
		/// </summary>
		public enum FieldNames
		{
			EntryLexicalForm,
			ExampleSentence,
			ExampleTranslation
		} ;

		public Field()
		{
			Initialize("unknown", "MultiText", MultiplicityType.ZeroOr1, new List<string>());
		}

		public Field(string fieldName, string className, IEnumerable<string> writingSystemIds)
				: this(fieldName, className, writingSystemIds, MultiplicityType.ZeroOr1, "MultiText"
						) {}

		public Field(string fieldName,
					 string parentClassName,
					 IEnumerable<string> writingSystemIds,
					 MultiplicityType multiplicity,
					 string dataTypeName)
		{
			if (writingSystemIds == null)
			{
				throw new ArgumentNullException();
			}
			ClassName = parentClassName;
			Enabled = true; //without this lots of tests would need updating
			Initialize(fieldName, dataTypeName, multiplicity, writingSystemIds);
		}

		public Field(Field field)
		{
			FieldName = field.FieldName;
			ClassName = field.ClassName;
			_writingSystemIds = new BindingList<string>();
			_writingSystemIds.ListChanged += OnWritingSystemIdsChanged;
			foreach (string id in field.WritingSystemIds)
			{
				WritingSystemIds.Add(id);
			}
			Description = field.Description;
			DisplayName = field.DisplayName;
			Multiplicity = field.Multiplicity;
			Visibility = field.Visibility;
			DataTypeName = field.DataTypeName;
			OptionsListFile = field.OptionsListFile;
			Enabled = field.Enabled;
		}

		/// <summary>
		/// clean up after exposing field name to UI for user editting
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string MakeFieldNameSafe(string text)
		{
			//parentheses mess up our greps, don't really belong in xml names
			char[] charsToRemove = new[]
			{
			   ' ', '(', ')', '*', ']', '[', '?', '{', '}', '\\', '<', '>',
			   '+', '&'
			};
			foreach (char c in charsToRemove)
			{
				text = text.Replace(c.ToString(), "");
			}
			return text.Trim();
		}

		private void Initialize(string fieldName,
								string dataTypeName,
								MultiplicityType multiplicity,
								IEnumerable<string> writingSystemIds)
		{
			FieldName = fieldName;
			WritingSystemIds = new List<string>(writingSystemIds);
			_multiplicity = multiplicity;
			DataTypeName = dataTypeName;

			Debug.Assert(FieldNames.EntryLexicalForm.ToString() ==
						 LexEntry.WellKnownProperties.LexicalUnit);
			Debug.Assert(FieldNames.ExampleSentence.ToString() ==
						 LexExampleSentence.WellKnownProperties.ExampleSentence);
			Debug.Assert(FieldNames.ExampleTranslation.ToString() ==
						 LexExampleSentence.WellKnownProperties.Translation);
		}

		[Description(
				"The name of the field, as it will appear in the LIFT file. This is not visible to the WeSay user."
				)]
		[ReflectorCollection("fieldName", Required = true)]
		public string FieldName
		{
			get
			{
				if (_fieldName == null)
				{
					throw new InvalidOperationException(
							"FieldName must be set before it can be used.");
				}
				return _fieldName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_fieldName = MakeFieldNameSafe(value);
				if (_fieldName == "Definition") //versions prior to oct-23-2007 had the case wrong
				{
					_fieldName = "definition";
				}
				if (_fieldName == "CitationForm") //versions prior to nov-8-2007 had the name wrong
				{
					_fieldName = "citation";
				}
			}
		}

		public string Key
		{
			get { return _className + "." + _fieldName; }
		}

		[Description("The label of the field as it will be displayed to the user.")]
		[ReflectorCollection("displayName", Required = false)]
		public string DisplayName
		{
			get
			{
				if (String.IsNullOrEmpty(_displayName))
				{
					return "*" + FieldName;
				}
				return _displayName;
			}
			set { _displayName = value; }
		}

		//        /// <summary>
		//        /// this is needed for fields like sense note vs entry not, where in the app, they should have the same label
		//        /// </summary>
		//        [Description("The label of the field as it will be displayed in the config tool.")]
		//        [ReflectorCollection("configurationName", Required = false)]
		//        public string ConfigurationName
		//        {
		//            get
		//            {
		//                if (String.IsNullOrEmpty(_configurationName))
		//                {
		//                    return DisplayName;
		//                }
		//                return _configurationName;
		//            }
		//            set
		//            {
		//                if(value == DisplayName)//don't make a distinction unless you need to
		//                {
		//                    _configurationName = String.Empty;
		//                }
		//                else
		//                {
		//                    _configurationName = value;
		//                }
		//            }
		//        }

		[TypeConverter(typeof (ParentClassConverter))]
		[Description("The parent of this field. E.g. Entry, Sense, Example.")]
		[ReflectorCollection("className", Required = true)]
		public string ClassName
		{
			get
			{
				if (_className.Length == 0)
				{
					throw new InvalidOperationException(
							"className has not been initialized correctly");
				}
				return _className;
			}
			set
			{
				switch (value)
				{
					case null:
						throw new ArgumentNullException();
					case "PalasoDataObject":
					case "LexEntry":
					case "LexSense":
					case "LexExampleSentence":
						_className = value;
						break;
					default:
						throw new ArgumentOutOfRangeException("value",
															  value,
															  "className must be PalasoDataObject, LexEntry, LexSense, or LexExampleSentence");
				}
			}
		}

		[Browsable(false)]
		public bool UserCanDeleteOrModify
		{
			get
			{
				if (IsBuiltInViaCode)
				{
					return false;
				}

				if (PalasoDataObject.WellKnownProperties.Contains(FieldName))
				{
					return false;
				}

				if (LexEntry.WellKnownProperties.Contains(FieldName))
				{
					return false;
				}

				if (LexSense.WellKnownProperties.ContainsAnyCaseVersionOf(FieldName))
				{
					return false;
				}

				if (LexExampleSentence.WellKnownProperties.Contains(FieldName))
				{
					return false;
				}
				if (FieldName == "SILCAWL")
					return false;

				return true;
			}
		}

		[Browsable(false)]
		public bool UserCanRelocate
		{
			get
			{
				if (_fieldName == FieldNames.EntryLexicalForm.ToString() ||
					IsMeaningField ||
					_fieldName == FieldNames.ExampleSentence.ToString())
				{
					return false;
				}

				return true;
			}
		}

		[Browsable(false)]
		public bool UserCanModifyWritingSystems
		{
			get
			{
				if (_fieldName == "SILCAWL" ||
					_fieldName =="Picture")
					// || Don't disable this:  it is actually used to change the language used when displaying domains:  _fieldName == LexSense.WellKnownProperties.SemanticDomainDdp4)
				{
					return false;
				}

				return true;
			}
		}


		public bool UserCanModifySpellCheckFeature
		{
			get
			{
				if (_fieldName == "SILCAWL" ||
					_fieldName == "Picture" ||
					_fieldName == LexSense.WellKnownProperties.SemanticDomainDdp4)
				{
					return false;
				}

				return true;
			}
		}


		[TypeConverter(typeof (DataTypeClassConverter))]
		[Description(
				"The type of the field. E.g. multilingual text, option, option collection, relation."
				)]
		[ReflectorProperty("dataType", Required = true)]
		public string DataTypeName
		{
			get { return _dataTypeName; }
			set { _dataTypeName = value; }
		}

		[Description(
				"For options and option collections, the name of the xml file containing the valid set of options."
				)]
		[ReflectorProperty("optionsListFile", Required = false)]
		public string OptionsListFile
		{
			get { // this is about trying to get the win version to stop outputing <optionsListfile>(return)</optionsListFile>(whereas mono doesn't)
				if(_optionsListFile==null)
					return null;
				return _optionsListFile.Trim();
			}
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

#if GlossMeaning
				if (FieldName == FieldNames.SenseGloss.ToString())
				{
					return true;
				}
#endif
				return false;
			}
		}

		public override string ToString()
		{
			return DisplayName;
		}

		[Browsable(false)]
		[ReflectorProperty("writingSystems", typeof (WsIdCollectionSerializerFactory))]
		public IList<string> WritingSystemIds
		{
			get { return _writingSystemIds; }
			set
			{
				int i = 0;
				foreach (string s in value)
				{
					i++;
					if (s == null)
					{
						throw new ArgumentNullException("value",
														"Input System argument" + i + "is null");
					}
				}
				if(_writingSystemIds != null)
				{
					_writingSystemIds.ListChanged -= OnWritingSystemIdsChanged;
				}
				_writingSystemIds = new BindingList<string>(value);
				_writingSystemIds.ListChanged += OnWritingSystemIdsChanged;
				FireWritingSystemsChangedEvent();
			}
		}

		private void FireWritingSystemsChangedEvent()
		{
			if(WritingSystemsChanged != null)
			{
				WritingSystemsChanged(this, new EventArgs());
			}
		}

		public event EventHandler WritingSystemsChanged;

		private void OnWritingSystemIdsChanged(object sender, ListChangedEventArgs e)
		{
			FireWritingSystemsChangedEvent();
		}

		/// <summary>
		/// omit audio writing systems
		/// </summary>
		public IEnumerable<string> GetTextOnlyWritingSystemIds(IWritingSystemRepository writingSystems)
		{
			return writingSystems.TextWritingSystems().Where(ws => _writingSystemIds.Contains(ws.Id)).Select(ws => ws.Id);
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
			set
			{
				_visibility = value;

				//for backward compatibility:
				//we now use Enabled=false rather than Invisible
				if (_visibility == CommonEnumerations.VisibilitySetting.Invisible)
				{
					_visibility = CommonEnumerations.VisibilitySetting.Visible;
					Enabled = false;
				}
			}
		}

		[ReflectorProperty("enabled", Required = false)]
		public bool Enabled
		{
			get
			{
				if (_enabledNotSet) //for backwards compatibility, before we added Enabled
				{
					Enabled = Visibility == CommonEnumerations.VisibilitySetting.Visible;
				}
				return _enabled;
			}
			set
			{
				_enabled = value;
				_enabledNotSet = false;
			}
		}

		public void ChangeWritingSystemId(string oldId, string newId)
		{
			int i = _writingSystemIds.IndexOf(oldId);
			if (i > -1)
			{
				_writingSystemIds[i] = newId;
			}
		}

		/// <summary>
		/// built-in fields have properties which aren't user editable
		/// </summary>
		//        [Browsable(false)]
		//        public bool CanOnlyEditDisplayName
		//        {
		//            get { return IsBuiltInViaCode; }
		//        }
		[Browsable(false)]
		public bool ShowOptionListStuff
		{
			get
			{
				return DataTypeName == BuiltInDataType.Option.ToString() ||
					   DataTypeName == BuiltInDataType.OptionCollection.ToString();
			}
		}

		[Browsable(false)]
		[ReflectorProperty("multiplicity", Required = false)]
		public MultiplicityType Multiplicity
		{
			get { return _multiplicity; }
			set { _multiplicity = value; }
		}

		public bool GetDoShow(IReportEmptiness data, bool showNormallyHiddenFields)
		{
			return _enabled &&
				   ((showNormallyHiddenFields ||
					 (data != null && data.ShouldCountAsFilledForPurposesOfConditionalDisplay)) ||
					(_visibility == CommonEnumerations.VisibilitySetting.Visible ||
					 _visibility == CommonEnumerations.VisibilitySetting.ReadOnly));
		}

		public static string NewFieldNamePrefix
		{
			get { return "newField"; }
		}

		[Description("This setting is changed in the Dictionary Browse and Edit configuration.")]
		[ReflectorProperty("meaningField", Required = false)]
		public bool IsMeaningField
		{
			get { return _isMeaningField; }
			set { _isMeaningField = value; }
		}

		public bool CanOmitFromMainViewTemplate
		{
			get
			{
				if (_fieldName == FieldNames.EntryLexicalForm.ToString())
				{
					return false;
				}

				if (IsMeaningField)
				{
					return false;
				}
				return true;
			}
		}

		[Description("Do you want words in this field to be spell checked?")]
		[ReflectorProperty("spellCheckingEnabled", Required = false)]
		public bool IsSpellCheckingEnabled
		{
			get { return _isSpellCheckingEnabled; }
			set { _isSpellCheckingEnabled = value; }
		}

		[ReflectorProperty("multiParagraph", Required = false)]
		public bool IsMultiParagraph
		{
			get { return _isMultiParagraph; }
			set { _isMultiParagraph = value;}
		}


		[Browsable(false)]
		public bool HasWritingSystem(string writingSystemId)
		{
			return _writingSystemIds.Any(id => id == writingSystemId);
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

		internal class WsIdCollectionSerializerFactory: ISerialiserFactory
		{
			public IXmlMemberSerialiser Create(ReflectorMember member,
											   ReflectorPropertyAttribute attribute)
			{
				return new WsIdCollectionSerializer(member, attribute);
			}
		}

		internal class WsIdCollectionSerializer: XmlMemberSerialiser
		{
			public WsIdCollectionSerializer(ReflectorMember member,
											ReflectorPropertyAttribute attribute)
					: base(member, attribute) {}

			public override void Write(XmlWriter writer, object target)
			{
				writer.WriteStartElement("writingSystems");
				foreach (string s in ((Field) target)._writingSystemIds)
				{
					writer.WriteElementString("id", s);
				}
				writer.WriteEndElement();
			}

			public override object Read(XmlNode node, NetReflectorTypeTable table)
			{
				Debug.Assert(node.Name == "writingSystems");
				Debug.Assert(node != null);
				var l = new List<string>();
				foreach (XmlNode n in node.SelectNodes("id"))
				{
					l.Add(n.InnerText);
				}
				return l;
			}
		}

		#endregion


	}

	internal class ParentClassConverter: WeSayStringConverter
	{
		public override string[] ValidStrings
		{
			get { return new[] {"LexEntry", "LexSense", "LexExampleSentence"}; }
		}
	}

	internal class DataTypeClassConverter: WeSayStringConverter
	{
		public override string[] ValidStrings
		{
			get { return new[] {"MultiText", "Option", "OptionCollection", "RelationToOneEntry"}; }
		}
	}

	internal abstract class WeSayStringConverter: StringConverter
	{
		public abstract string[] ValidStrings { get; }

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

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(ValidStrings);
		}
	}
}