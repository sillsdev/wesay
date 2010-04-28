using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Xml;
using Enchant;
using Exortech.NetReflector;
using Palaso.I8N;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.Keyboarding;
using Palaso.WritingSystems;

namespace WeSay.LexicalModel.Foundation
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
	public class WritingSystem: IComparer<string>, IComparer
	{
		private WritingSystemDefinition _palasoWritingSystem = new WritingSystemDefinition();
		public static string IdForUnknownAnalysis = "en";
		public static string IdForUnknownVernacular = "v";
		private string _spellCheckingId;

		private bool _isAudio;
		private Font _font;
		private string _id;
		private string _keyboardName;
		private bool _rightToLeft;
		private bool _isUnicode = true;

		public WritingSystem(XmlNode node): this()
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
		}

		/// <summary>
		/// default for testing only
		/// </summary>
		public WritingSystem(string id, Font font): this()
		{
			_palasoWritingSystem.ISO = id;
			_font = font;
		}

		[ReflectorProperty("Id", Required = true)]
		public string Id
		{
			get { return _palasoWritingSystem.Id; }
			set
			{
				throw new InvalidOperationException("This method is obsolete");
				//Right now Id has a InvalidOperation setter as palaso treats the Id as a read-only
				//property consisting of iso-region-script-variant. The id property in WeSay's ws
				//is supposed to be equivalent to the iso code if I understand correctly.
			}
		}

		[ReflectorProperty("Abbreviation", Required = false)]
		public string Abbreviation
		{
			get
			{
				if (string.IsNullOrEmpty(_palasoWritingSystem.Abbreviation))
				{
					return _palasoWritingSystem.Id;
				}
				else
				{
					return _palasoWritingSystem.Abbreviation;
				}
			}
			set { _palasoWritingSystem.Abbreviation = value; }
		}

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
				if (_palasoWritingSystem.SortUsing == WritingSystemDefinition.SortRulesType.DefaultOrdering)
				{
					return Id;
				}
				if (_palasoWritingSystem.SortUsing == WritingSystemDefinition.SortRulesType.OtherLanguage)
				{
					return _palasoWritingSystem.SortRules;
				}
				return _palasoWritingSystem.SortUsing.ToString();
			}
			set
			{
				if (_palasoWritingSystem.SortUsing.ToString() != value)
				{
					WritingSystemDefinition.SortRulesType newSortRulesType = AdaptToSortRulesType(value);

					bool switchingFromNonCustomToCustomSortRules =
						(!UsesCustomSortRules) && IsCustomSortRuleType(newSortRulesType);
					bool switchingToDefaultSortOrder =
						(newSortRulesType == WritingSystemDefinition.SortRulesType.DefaultOrdering);
					bool switchingToNonDefaultSystemSort =
						(newSortRulesType == WritingSystemDefinition.SortRulesType.OtherLanguage);

					if (switchingFromNonCustomToCustomSortRules || switchingToDefaultSortOrder)
					{
						_palasoWritingSystem.SortRules = null;
					}
					else if (switchingToNonDefaultSystemSort)
					{
						_palasoWritingSystem.SortRules = value;
					}
					_palasoWritingSystem.SortUsing = newSortRulesType;
				}
			}
		}

		private WritingSystemDefinition.SortRulesType AdaptToSortRulesType(string sortUsingString)
		{
			WritingSystemDefinition.SortRulesType palasoSortRulesType;
			if(sortUsingString == CustomSortRulesType.CustomICU.ToString())
			{
				palasoSortRulesType = WritingSystemDefinition.SortRulesType.CustomICU;
			}
			else if(sortUsingString == CustomSortRulesType.CustomSimple.ToString())
			{
				palasoSortRulesType = WritingSystemDefinition.SortRulesType.CustomSimple;
			}
			else if(sortUsingString == null)
			{
				palasoSortRulesType = WritingSystemDefinition.SortRulesType.DefaultOrdering;
			}
			else
			{
				palasoSortRulesType = WritingSystemDefinition.SortRulesType.OtherLanguage;
			}
			return palasoSortRulesType;
		}

		[Browsable(false)]
		public bool UsesCustomSortRules
		{
			get
			{
				bool isUsingCustomSortRules = IsCustomSortRuleType(_palasoWritingSystem.SortUsing);
				return isUsingCustomSortRules;
			}
		}

		private bool IsCustomSortRuleType(WritingSystemDefinition.SortRulesType sortRuleType)
		{
			return (sortRuleType == WritingSystemDefinition.SortRulesType.CustomICU) ||
				   (sortRuleType == WritingSystemDefinition.SortRulesType.CustomSimple);
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
					_palasoWritingSystem.SortRules = null;
					return null;
				}
				return _palasoWritingSystem.SortRules;
			}
			set
			{
				// should only be set if UsesCustomSortRules == true but can't because of NetReflector
				if (_palasoWritingSystem.SortRules != value)
				{
					_palasoWritingSystem.SortRules = value;
				}
				// cannot do the following due to NetReflector wanting to set to null!
				// throw new InvalidOperationException("CustomSortRules can only be set when UsesCustomSortRules is true");
			}
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
		[ReflectorProperty("FontName", Required = true)]
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
					return _palasoWritingSystem.DefaultFontName;
				}
			}
			set
			{
				try
				{
					_font = new Font(value, FontSize);
				}
				catch (Exception error)
				{
					//see http://www.wesay.org/issues/browse/WS-14949
#if MONO
					const string hint = "";
#else
					const string hint = "You may be able to repair the font: drag it to your desktop, right-click on it, tell it to install. In the meantime, ";
#endif
					ErrorReport.NotifyUserOfProblem(new ShowOncePerSessionBasedOnExactMessagePolicy(),
													"There is a problem with the font {0} on this computer. {1} WeSay will have to use the System default font instead."+Environment.NewLine+"The error was: {2}",
													value, hint, error.Message);
					_font = new Font(SystemFonts.DefaultFont.FontFamily, FontSize);
				}
				_palasoWritingSystem.DefaultFontName = _font.Name;
			}
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
					return (int) _palasoWritingSystem.DefaultFontSize;
				}
			}
			set
			{
				_palasoWritingSystem.DefaultFontSize = value;
				_font = new Font(FontName, _palasoWritingSystem.DefaultFontSize);
			}
		}

		[ReflectorProperty("RightToLeft", Required = false)]
		public bool RightToLeft
		{
			get { return _rightToLeft; }
			set { _rightToLeft = value; }
		}

		[TypeConverter(typeof (SpellCheckerIdToDisplayStringConverter))]
		[ReflectorProperty("SpellCheckingId", Required = false)]
		public string SpellCheckingId
		{
			get
			{
				if (string.IsNullOrEmpty(_spellCheckingId))
				{
					return _id;
				}
				else
				{
					return _spellCheckingId;
				}
			}
			set { _spellCheckingId = value; }
		}

		[ReflectorProperty("IsAudio", Required = false)]
		public bool IsAudio
		{
			get { return _isAudio; }
			set { _isAudio = value; }
		}

		[ReflectorProperty("IsUnicode", Required = false)]
		public bool IsUnicode
		{
			get { return _isUnicode; }
			set { _isUnicode = value; }
		}

		public string Region
		{
			get { return _palasoWritingSystem.Region; }
			set { _palasoWritingSystem.Region = value; }
		}

		public string Script
		{
			get { return _palasoWritingSystem.Script; }
			set { _palasoWritingSystem.Script = value; }
		}

		public string Variant
		{
			get { return _palasoWritingSystem.Variant; }
			set { _palasoWritingSystem.Variant = value; }
		}

		public string ISO
		{
			get { return _palasoWritingSystem.ISO; }
			set { _palasoWritingSystem.ISO = value; }
		}

		#region IComparer<string> Members

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
			return _palasoWritingSystem.Collator.Compare(x, y);
			//return _sortComparer(x, y);
		}

		public int Compare(object x, object y)
		{
			return Compare((string) x, (string) y);
		}

		#endregion

		public override string ToString()
		{
			return Id;
		}

		public SortKey GetSortKey(string source)
		{
			return _palasoWritingSystem.Collator.GetSortKey(source);
		}

		// Same if behavior is same (not appearance)
		public override int GetHashCode()
		{
			int hashCode = HashCombine(Id.GetHashCode(), SortUsing.GetHashCode());
			if (UsesCustomSortRules)
			{
				hashCode = HashCombine(hashCode, CustomSortRules.GetHashCode());
			}
			return hashCode;
		}

		private static int HashCombine(int seed, int hash)
		{
			// following line lifted from boost/functional/hash.hpp which is
			//  Copyright Daniel James 2005-2007.
			//  Use, modification, and distribution are subject to the Boost Software License, Version 1.0.
			//  see license at http://www.boost.org/LICENSE_1_0.txt
			return (int) (seed ^ (hash + 0x9e3779b9 + (seed << 6) + (seed >> 2)));
		}

		#region Nested type: KeyboardListHelper

		public class KeyboardListHelper: StringConverter
		{
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				//true means show a combobox
				return true;
			}

			public override object ConvertTo(ITypeDescriptorContext context,
											 CultureInfo culture,
											 object value,
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

			public override object ConvertFrom(ITypeDescriptorContext context,
											   CultureInfo culture,
											   object value)
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

			public override StandardValuesCollection GetStandardValues(
				ITypeDescriptorContext context)
			{
				List<String> keyboards = new List<string>();
				keyboards.Add(String.Empty); // for 'default'

				foreach (KeyboardController.KeyboardDescriptor keyboard in
					KeyboardController.GetAvailableKeyboards(KeyboardController.Engines.All))
				{
					keyboards.Add(keyboard.Name);
				}
				return new StandardValuesCollection(keyboards);
			}
		}

		#endregion

		#region Nested type: SortComparer

		private delegate int SortComparer(string s1, string s2);

		#endregion

		#region Nested type: SortKeyGenerator

		private delegate SortKey SortKeyGenerator(string s);

		#endregion

		#region Nested type: SpellCheckerIdToDisplayStringConverter

		public class SpellCheckerIdToDisplayStringConverter: StringConverter
		{
			public delegate IList<string> GetInstalledSpellCheckingIdsDelegate();

			private GetInstalledSpellCheckingIdsDelegate _getInstalledSpellCheckingIdsStrategy =
				DefaultGetInstalledSpellCheckingIdsStrategy;

			public GetInstalledSpellCheckingIdsDelegate GetInstalledSpellCheckingIdsStrategy
			{
				get { return _getInstalledSpellCheckingIdsStrategy; }
				set
				{
					if (value == null)
					{
						_getInstalledSpellCheckingIdsStrategy =
							DefaultGetInstalledSpellCheckingIdsStrategy;
					}
					else
					{
						_getInstalledSpellCheckingIdsStrategy = value;
					}
				}
			}

			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				//true means show a combobox
				return true;
			}

			public override object ConvertTo(ITypeDescriptorContext context,
											 CultureInfo culture,
											 object value,
											 Type destinationType)
			{
				if (destinationType == null)
				{
					throw new ArgumentNullException("destinationType");
				}

				if (destinationType != typeof (string))
				{
					throw GetConvertToException(value, destinationType);
				}
				if( (String) value == "none") //added as Mono bugfix
				{
					return "none";
				}
				if ((String) value == String.Empty)
				{
					Console.WriteLine("Yoyo");
					return "none";
				}
				else
				{
					string valueAsString = value.ToString();

					string display;
					if (!GetInstalledSpellCheckingIdsStrategy().Contains(valueAsString))
					{
						string notInstalledTail = " (" + StringCatalog.Get("Not installed") + ")";
						display = valueAsString + notInstalledTail;
					}
					else
					{
						try
						{
							string id = valueAsString.Replace('_', '-');
							CultureInfo cultureInfo = CultureInfo.GetCultureInfoByIetfLanguageTag(id);
							display = valueAsString + " (" + cultureInfo.NativeName + ")";
						}
						catch
						{
							// don't care if this fails it was just to add a little more info to user.
							//mono doesn't support this now
							display = valueAsString;
						}
					}
					return display;
				}
			}

			public override object ConvertFrom(ITypeDescriptorContext context,
											   CultureInfo culture,
											   object value)
			{
				if ((String) value == "none")
				{
					return "none"; //String.Empty;
				}
				else
				{
					// display name is "en_US (English (United States))"
					// we just want the first part so need to strip off any initial whitespace and get the first
					// whitespace delimited token: en_US
					string displayNameWhitespaceStrippedFromBeginning =
						value.ToString().TrimStart(null);
					string[] whitespaceDelimitedTokens =
						displayNameWhitespaceStrippedFromBeginning.Split(null);
					string id = whitespaceDelimitedTokens[0];
					return id;
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
				return false;
			}

			public override StandardValuesCollection GetStandardValues(
				ITypeDescriptorContext context)
			{
				List<String> spellCheckerIds = new List<string>();

				spellCheckerIds.Add("none");//String.Empty); // for 'none' (changed for Mono bug)

				try
				{
					spellCheckerIds.AddRange(GetInstalledSpellCheckingIdsStrategy());
				}
				catch
				{
					// do nothing if enchant is not installed
				}
				spellCheckerIds.Sort();
				return new StandardValuesCollection(spellCheckerIds);
			}

			private static IList<string> DefaultGetInstalledSpellCheckingIdsStrategy()
			{
				List<string> installedSpellCheckingIds = new List<string>();
				using (Broker broker = new Broker())
				{
					foreach (DictionaryInfo dictionary in broker.Dictionaries)
					{
						string id = dictionary.Language;
						installedSpellCheckingIds.Add(id);
					}
				}
				return installedSpellCheckingIds;
			}
		}

		#endregion

	}
}