using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using Exortech.NetReflector;
using kmcomapi;
using Palaso.WritingSystems.Collation;
using Spart;

namespace WeSay.Language
{
	public enum CustomSortRulesType
	{
		/// <summary>
		/// Custom Simple (Shoebox/Toolbox) style rules
		/// </summary>
		[Description("Custom Simple (Shoebox style) rules")]
		CustomSimple,
		/// <summary>
		/// Custom ICU rules
		/// </summary>
		[Description("Custom ICU rules")]
		CustomICU
	}


	[ReflectorType("WritingSystem")]
	public class WritingSystem : IComparer<string>
	{
		public static string IdForUnknownVernacular = "v";
		public static string IdForUnknownAnalysis = "en";

		private Font _font;
		private string _id;
		private string _keyboardName;
		private bool _rightToLeft = false;
		private string _abbreviation;
		private string _sortUsing;
		private string _customSortRules;

//        public WritingSystem(string filePath)
//        {
//            _fontPrefsDoc = new XmlDocument();
//            _fontPrefsDoc.Load(filePath);
//            XmlNode node = _fontPrefsDoc.SelectSingleNode("writingSystemPrefs");
//            _id = node.Attributes["id"].Value;
//        }
		public WritingSystem(XmlNode node) : this()
		{
			_id = node.Attributes["id"].Value;
			XmlNode fontNode = node.SelectSingleNode("font");
			string name = fontNode.Attributes["name"].Value;
			float size = float.Parse(fontNode.Attributes["baseSize"].Value);

			_font = new Font(name, size);
		}

		/// <summary>
		/// default constructor required for deserialization
		/// </summary>
		public WritingSystem()
		{
			InitializeSorting();
		}

		/// <summary>
		/// default for testing only
		/// </summary>
		public WritingSystem(string id, Font font) : this()
		{
			_id = id;
			_font = font;
		}

		public override string ToString()
		{
			return Id;
		}

		[ReflectorProperty("Id", Required = true)]
		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[ReflectorProperty("Abbreviation", Required = false)]
		public string Abbreviation
		{
			get
			{
				if (string.IsNullOrEmpty(_abbreviation))
				{
					return _id;
				}
				else
				{
					return _abbreviation;
				}
			}
			set { _abbreviation = value; }
		}

		[ReflectorProperty("RightToLeft", Required = false)]
		public bool RightToLeft
		{
			get { return _rightToLeft; }
			set { _rightToLeft = value; }
		}

//        //we'll be getting rid of this property
//        [Browsable(true), System.ComponentModel.DisplayName("Vernacular")]
//        public string VernacularDefault
//        {
//            get { return _id; }
//            set
//            {
//                _id = value;
//            }
//        }

		[Browsable(false)]
		public Font Font
		{
			get
			{
				if (_font == null)
				{
					_font = new Font(FontFamily.GenericSansSerif, FontSize);
				}
				return _font;
			}
			set { _font = value; }
		}

		/// <summary>
		/// A system id for sorting or a CustomSortRulesType as a string
		/// </summary>
		[Browsable(false)]
		[ReflectorProperty("SortUsing", Required = false)]
		public string SortUsing
		{
			get
			{
				if (String.IsNullOrEmpty(_sortUsing))
				{
					return Id;
				}
				return _sortUsing;
			}
			set
			{
				if (_sortUsing != value)
				{
					_sortUsing = value;
					if (!UsesCustomSortRules)
					{
						_customSortRules = null;
					}
					InitializeSorting();
				}
			}
		}

		public bool UsesCustomSortRules
		{
		   get
		   {
			   if (SortUsing == null)
			   {
				   return false;
			   }
			   return Enum.IsDefined(typeof(CustomSortRulesType), SortUsing);
		   }
		}

		/// <summary>
		/// returns null if UsesCustomSortRules is false
		/// </summary>
		[Browsable(false)]
		[ReflectorProperty("CustomSortRules", Required = false)]
		public string CustomSortRules
		{
			get
			{
				if (!UsesCustomSortRules)
				{
					_customSortRules=null;
					return null;
				}
				return _customSortRules ?? string.Empty;
			}
			set
			{
				// should only be set if UsesCustomSortRules == true but can't because of NetReflector
				if (_customSortRules != value)
				{
					_customSortRules = value;
					InitializeSorting();
				}
				// cannot do the following due to NetReflector wanting to set to null!
				// throw new InvalidOperationException("CustomSortRules can only be set when UsesCustomSortRules is true");
			}
		}

		private void InitializeSortingFromWritingSystemId(string id)
		{
			CultureInfo cultureInfo = null;
			if (!String.IsNullOrEmpty(id))
			{
				cultureInfo = GetCultureInfoFromWritingSystemId(id);
			}
			if (cultureInfo == null)
			{
				cultureInfo = CultureInfo.InvariantCulture;
			}

			_sortComparer = cultureInfo.CompareInfo.Compare;
			_sortKeyGenerator = cultureInfo.CompareInfo.GetSortKey;
		}

		private delegate SortKey SortKeyGenerator(string s);

		private delegate int SortComparer(string s1, string s2);

		private SortComparer _sortComparer;
		private SortKeyGenerator _sortKeyGenerator;

		private void InitializeSorting()
		{
			if (UsesCustomSortRules)
			{
				InitializeSortingFromCustomRules(CustomSortRules);
			}
			else
			{
				InitializeSortingFromWritingSystemId(SortUsing);
			}
		}

		// This should always succeed. We try to use the rule collator associated
		// with the custom sort rules type, if that still doesn't work then we
		// fall back to using the system sort from the id or the invariant sort
		private void InitializeSortingFromCustomRules(string rules)
		{
			ICollator collator=null;
			try
			{
				if (SortUsing == CustomSortRulesType.CustomSimple.ToString())
				{
					try
					{
						collator = new SimpleRulesCollator(rules);
					}
					catch (ParserErrorException e)
					{
						Palaso.Reporting.Logger.WriteMinorEvent(e.Message);
						Palaso.Reporting.Logger.WriteMinorEvent(
								"Failed to parse simple sort rules, falling back to system sorting");
					}
				}

				else if (SortUsing == CustomSortRulesType.CustomICU.ToString())
				{
					try
					{
						collator = new IcuRulesCollator(rules);
					}
					catch (ApplicationException e)
					{
						Palaso.Reporting.Logger.WriteMinorEvent(e.Message);
						Palaso.Reporting.Logger.WriteMinorEvent(
								"Failed to parse ICU sort rules, falling back to system sorting");
					}
				}
				else
				{
					Debug.Fail("unknown CustomSortRulesType");
				}
			}
			catch (DllNotFoundException e)
			{
				Palaso.Reporting.Logger.WriteMinorEvent(e.Message);
				Palaso.Reporting.Logger.WriteMinorEvent("Falling back to system sorting");
			}

			if (collator == null)
			{
				InitializeSortingFromWritingSystemId(Id);
			}
			else
			{
				_sortComparer = collator.Compare;
				_sortKeyGenerator = collator.GetSortKey;
			}
		}

		private static CultureInfo GetCultureInfoFromWritingSystemId(string sortUsing)
		{
			CultureInfo ci = null;
			try
			{
				ci = CultureInfo.GetCultureInfo(sortUsing);
			}
			catch (ArgumentException e)
			{
				if (e is ArgumentNullException ||
					e is ArgumentOutOfRangeException)
				{
					throw;
				}
				if (Environment.OSVersion.Platform != PlatformID.Unix)
				{
					ci = TryGetCultureInfoByIetfLanguageTag(sortUsing); // not supported by mono yet
				}
			}
			return ci;
		}

		private static CultureInfo TryGetCultureInfoByIetfLanguageTag(string ietfLanguageTag)
		{
			CultureInfo ci = null;
			try
			{
				ci = CultureInfo.GetCultureInfoByIetfLanguageTag(ietfLanguageTag);
			}
			catch (ArgumentException ex)
			{
				if (ex is ArgumentNullException ||
					ex is ArgumentOutOfRangeException)
				{
					throw;
				}
			}
			return ci;
		}

		public SortKey GetSortKey(string source)
		{
			return _sortKeyGenerator(source);
		}

		///<summary>
		///Compares two strings and returns a value indicating whether one is less than, equal to, or greater than the other.
		///</summary>
		///
		///<returns>
		/// Less than zero when x is less than y.
		/// Zero when x equals y.
		/// Greater than zero when x is greater than y.
		///</returns>
		///
		///<param name="y">The second object to compare.</param>
		///<param name="x">The first object to compare.</param>
		public int Compare(string x, string y)
		{
			return _sortComparer(x, y);
		}

		[TypeConverter(typeof (KeyboardListHelper))]
		[Browsable(true)]
		[ReflectorProperty("WindowsKeyman", Required = false)]
		public string KeyboardName
		{
			get { return _keyboardName; }
			set { _keyboardName = value; }
		}

		[Browsable(false)]
		[ReflectorProperty("FontName", Required=true)]
		public string FontName
		{
			get
			{
				if (_font == null)
				{
					return "Arial";
				}
				else
				{
					return _font.Name;
				}
			}
			set { _font = new Font(value, FontSize); }
		}

		[Browsable(false)]
		[ReflectorProperty("FontSize", Required = true)]
		public int FontSize
		{
			get
			{
				if (_font == null)
				{
					return 12;
				}
				else
				{
					return (int) _font.Size;
				}
			}
			set { _font = new Font(FontName, value); }
		}

		public class KeyboardListHelper : StringConverter
		{
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				//true means show a combobox
				return true;
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
											 Type destinationType)
			{
				if ((String) value == String.Empty)
				{
					return "default";
				}
				else
				{
					return value;
				}
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if ((String) value == "default")
				{
					return String.Empty;
				}
				else
				{
					return value;
				}
			}

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

			public override StandardValuesCollection
					GetStandardValues(ITypeDescriptorContext context)
			{
				List<String> keyboards = new List<string>();
				keyboards.Add(String.Empty); // for 'default'

				if (Environment.OSVersion.Platform != PlatformID.Unix)
				{
					//if (!GetKeyboardsFromKeyman7(keyboards))
					{
						GetKeyboardsFromKeyman6(keyboards);
					}
				}

				foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
				{
					keyboards.Add(lang.LayoutName);
				}
				return new StandardValuesCollection(keyboards);
			}

			private static void GetKeyboardsFromKeyman6(ICollection<string> keyboards)
			{
				try
				{
					KeymanLink.KeymanLink keymanLink = new KeymanLink.KeymanLink();
					if (keymanLink.Initialize(false))
					{
						foreach (KeymanLink.KeymanLink.KeymanKeyboard keyboard in keymanLink.Keyboards)
						{
							keyboards.Add(keyboard.KbdName);
						}
					}
				}
				catch (Exception err)
				{
					Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
						"WeSay ran into an error when looking for Keyman and asking it for a list of keyboards.  You can still use Keyman by assigning a Windows System Keyboard to the Keyman keyboard. \r\n{0}",
						err.Message);
				}
			}

			private static bool GetKeyboardsFromKeyman7(ICollection<string> keyboards)
			{
				try
				{
					TavultesoftKeymanClass keyman = new TavultesoftKeymanClass();

						foreach (IKeymanKeyboard keyboard in keyman.Keyboards)
						{
							keyboards.Add(keyboard.KeyboardName);
						}
				}
				catch (Exception )
				{
					return false;
				}
				return true;
			}
		}
	}
}