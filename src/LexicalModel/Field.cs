using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;
using WeSay.Foundation;
using WeSay.LexicalModel;

namespace WeSay.LexicalModel
{
	[ReflectorType("field")]
	public class Field
	{
		private string _fieldName;
		private List<string> _writingSystemIds;
		private string _displayName = string.Empty;
		private string _description = string.Empty;
		private string _className = string.Empty;
		private string _dataTypeName;
		private bool _enabled;
		private bool _isSpellCheckingEnabled;
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
					 string className,
					 IEnumerable<string> writingSystemIds,
					 MultiplicityType multiplicity,
					 string dataTypeName)
		{
			if (writingSystemIds == null)
			{
				throw new ArgumentNullException();
			}
			this.ClassName = className;
			this.Enabled = true; //without this lots of tests would need updating
			Initialize(fieldName, dataTypeName, multiplicity, writingSystemIds);
		}

		public Field(Field field)
		{
			this.FieldName = field.FieldName;
			this.ClassName = field.ClassName;
			this._writingSystemIds = new List<string>();
			foreach (string id in field.WritingSystemIds)
			{
				this.WritingSystemIds.Add(id);
			}
			this.Description = field.Description;
			this.DisplayName = field.DisplayName;
			this.Multiplicity = field.Multiplicity;
			this.Visibility = field.Visibility;
			this.DataTypeName = field.DataTypeName;
			this.OptionsListFile = field.OptionsListFile;
			this.Enabled = field.Enabled;
		}

		/// <summary>
		/// clean up after exposing field name to UI for user editting
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string MakeFieldNameSafe(string text)
		{
			//parentheses mess up our greps, don't really belong in xml names
			char[] charsToRemove =
					new char[]
							{' ', '(', ')', '*', ']', '[', '?', '{', '}', '\\', '<', '>', '+', '&'};
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
			this.FieldName = fieldName;
			this.WritingSystemIds = new List<string>(writingSystemIds);
			this._multiplicity = multiplicity;
			this.DataTypeName = dataTypeName;

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
				if (this._fieldName == null)
				{
					throw new InvalidOperationException(
							"FieldName must be set before it can be used.");
				}
				return this._fieldName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this._fieldName = MakeFieldNameSafe(value);
				if (this._fieldName == "Definition") //versions prior to oct-23-2007 had the case wrong
				{
					this._fieldName = "definition";
				}
				if (this._fieldName == "CitationForm") //versions prior to nov-8-2007 had the name wrong
				{
					this._fieldName = "citation";
				}
			}
		}

		public string Key
		{
			get { return this._className + "." + this._fieldName; }
		}

		[Description("The label of the field as it will be displayed to the user.")]
		[ReflectorCollection("displayName", Required=false)]
		public string DisplayName
		{
			get
			{
				if (String.IsNullOrEmpty(this._displayName))
				{
					return "*" + this.FieldName;
				}
				return this._displayName;
			}
			set { this._displayName = value; }
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
				if (this._className.Length == 0)
				{
					throw new InvalidOperationException(
							"className has not been initialized correctly");
				}
				return this._className;
			}
			set
			{
				switch (value)
				{
					case null:
						throw new ArgumentNullException();
					case "WeSayDataObject":
					case "LexEntry":
					case "LexSense":
					case "LexExampleSentence":
						this._className = value;
						break;
					default:
						throw new ArgumentOutOfRangeException("value",
															  value,
															  "className must be WeSayDataObject, LexEntry, LexSense, or LexExampleSentence");
				}
			}
		}

		[Browsable(false)]
		public bool UserCanDeleteOrModify
		{
			get
			{
				if (this.IsBuiltInViaCode)
				{
					return false;
				}

				if (WeSayDataObject.WellKnownProperties.Contains(this.FieldName))
				{
					return false;
				}

				if (LexEntry.WellKnownProperties.Contains(this.FieldName))
				{
					return false;
				}

				if (LexSense.WellKnownProperties.ContainsAnyCaseVersionOf(this.FieldName))
				{
					return false;
				}

				if (LexExampleSentence.WellKnownProperties.Contains(this.FieldName))
				{
					return false;
				}

				return true;
			}
		}

		[Browsable(false)]
		public bool UserCanRelocate
		{
			get
			{
				if (this._fieldName == FieldNames.EntryLexicalForm.ToString() ||
					this._fieldName == LexSense.WellKnownProperties.Definition ||
					this._fieldName == FieldNames.ExampleSentence.ToString())
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
			get { return this._dataTypeName; }
			set { this._dataTypeName = value; }
		}

		[Description(
				"For options and option collections, the name of the xml file containing the valid set of options."
				)]
		[ReflectorProperty("optionsListFile", Required = false)]
		public string OptionsListFile
		{
			get { return this._optionsListFile; }
			set { this._optionsListFile = value; }
		}

		[Browsable(false)]
		public bool IsBuiltInViaCode
		{
			get
			{
				if (this.FieldName == FieldNames.EntryLexicalForm.ToString())
				{
					return true;
				}

				if (this.FieldName == FieldNames.ExampleSentence.ToString())
				{
					return true;
				}

				if (this.FieldName == FieldNames.ExampleTranslation.ToString())
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
			return this.DisplayName;
		}

		[Browsable(false)]
		[ReflectorProperty("writingSystems", typeof (WsIdCollectionSerializerFactory))]
		public IList<string> WritingSystemIds
		{
			get { return this._writingSystemIds; }
			set
			{
				int i = 0;
				foreach (string s in value)
				{
					i++;
					if (s == null)
					{
						throw new ArgumentNullException("value",
														"Writing System argument" + i + "is null");
					}
				}
				this._writingSystemIds = new List<string>(value);
			}
		}

		[Browsable(false)]
		public string Description
		{
			get { return this._description; }
			set { this._description = value; }
		}

		[Browsable(false)]
		[ReflectorCollection("visibility", Required = false)]
		public CommonEnumerations.VisibilitySetting Visibility
		{
			get { return this._visibility; }
			set
			{
				this._visibility = value;

				//for backward compatibility:
				//we now use Enabled=false rather than Invisible
				if (this._visibility == CommonEnumerations.VisibilitySetting.Invisible)
				{
					this._visibility = CommonEnumerations.VisibilitySetting.Visible;
					this.Enabled = false;
				}
			}
		}

		[ReflectorProperty("enabled", Required = false)]
		public bool Enabled
		{
			get
			{
				if (this._enabledNotSet) //for backwards compatibility, before we added Enabled
				{
					this.Enabled = this.Visibility == CommonEnumerations.VisibilitySetting.Visible;
				}
				return this._enabled;
			}
			set
			{
				this._enabled = value;
				this._enabledNotSet = false;
			}
		}

		public void ChangeWritingSystemId(string oldId, string newId)
		{
			int i = this._writingSystemIds.FindIndex(delegate(string id) { return id == oldId; });
			if (i > -1)
			{
				this._writingSystemIds[i] = newId;
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
				return
						this.DataTypeName == BuiltInDataType.Option.ToString() ||
						this.DataTypeName == BuiltInDataType.OptionCollection.ToString();
			}
		}

		[Browsable(false)]
		[ReflectorProperty("multiplicity", Required = false)]
		public MultiplicityType Multiplicity
		{
			get { return this._multiplicity; }
			set { this._multiplicity = value; }
		}

		public bool GetDoShow(IReportEmptiness data, bool showNormallyHiddenFields)
		{
			return
					this._enabled &&
					((showNormallyHiddenFields ||
					  (data != null && data.ShouldCountAsFilledForPurposesOfConditionalDisplay)) ||
					 (this._visibility == CommonEnumerations.VisibilitySetting.Visible ||
					  this._visibility == CommonEnumerations.VisibilitySetting.ReadOnly));
		}

		public static string NewFieldNamePrefix
		{
			get { return "newField"; }
		}

		public bool CanOmitFromMainViewTemplate
		{
			get
			{
				if (this._fieldName == FieldNames.EntryLexicalForm.ToString())
				{
					return false;
				}

#if GlossMeaning
				if(_fieldName == FieldNames.SenseGloss.ToString())
					return false;
#else
				if (this._fieldName == LexSense.WellKnownProperties.Definition)
				{
					return false;
				}
#endif

				return true;
			}
		}

		[Description("Do you want words in this field to be spell checked?")]
		[ReflectorProperty("spellCheckingEnabled", Required = false)]
		public bool IsSpellCheckingEnabled
		{
			get { return this._isSpellCheckingEnabled; }
			set { this._isSpellCheckingEnabled = value; }
		}

		[Browsable(false)]
		public bool HasWritingSystem(string writingSystemId)
		{
			return this._writingSystemIds.Exists(delegate(string s) { return s == writingSystemId; });
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

	internal class ParentClassConverter: WeSayStringConverter
	{
		public override string[] ValidStrings
		{
			get { return new string[] {"LexEntry", "LexSense", "LexExampleSentence"}; }
		}
	}

	internal class DataTypeClassConverter: WeSayStringConverter
	{
		public override string[] ValidStrings
		{
			get { return new string[] {"MultiText", "Option", "OptionCollection", "RelationToOneEntry"}; }
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
			return new StandardValuesCollection(this.ValidStrings);
		}
	}
}