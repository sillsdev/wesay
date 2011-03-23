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
using Palaso.i18n;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.Keyboarding;
using Palaso.WritingSystems;
using Spart;
using WeSay.LexicalModel.Foundation.WritingSystemMigration;

namespace WeSay.LexicalModel.Foundation
{

	[ReflectorType("WritingSystem")]
	public class WritingSystem_V1 : IComparer<string>, IComparer
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
		private readonly Font _fallBackFont = new Font(FontFamily.GenericSansSerif, 12);
		private bool _isUnicode = true;
		private string _abbreviation;
		private WritingSystemDefinition _palasoWritingSystemDefinition;

		/// <summary>
		/// default constructor required for deserialization
		/// </summary>
		public WritingSystem_V1()
		{
			_palasoWritingSystemDefinition = new WritingSystemDefinition();
		}

		public WritingSystem_V1(string id, Font font)
		{
			ISO = id;
			Font = font;
			_palasoWritingSystemDefinition = new WritingSystemDefinition();
		}

		[ReflectorProperty("Id", Required = true)]
		public string ISO{ get; set;}

		public string Script { get; set; }

		public string Region { get; set; }

		public string Variant { get; set; }

		[ReflectorProperty("Abbreviation", Required = false)]
		public string Abbreviation
		{
			get
			{
				if (string.IsNullOrEmpty(_abbreviation))
				{
					return ISO;
				}
				return _abbreviation;
			}
			set { _abbreviation = value; }
		}

		[Browsable(false)]
		public Font Font
		{
			get
			{
				if ((FontName == null) || (FontSize == 0))
				{
					return _fallBackFont;
				}
				return new Font(FontName, FontSize);
			}
			set
			{
				if (value == null)
				{
					FontName = _fallBackFont.Name;
					FontSize = Convert.ToInt32(_fallBackFont.Size);
				}
				else
				{
					FontName = value.Name;
					FontSize = Convert.ToInt32(value.Size);
				}
			}
		}

		/// <summary>
		/// A system id for sorting or a CustomSortRulesType as a string
		/// </summary>
		[Browsable(false)]
		[ReflectorProperty("SortUsing", Required = false)]
		public string SortUsing{ get; set; }

		[Browsable(false)]
		public bool UsesCustomSortRules { get; set; }

		/// <summary>
		/// returns null if UsesCustomSortRules is false
		/// </summary>
		[Browsable(false)]
		[ReflectorProperty("CustomSortRules", Required = false)]
		public string CustomSortRules{ get; set; }

		[TypeConverter(typeof(KeyboardListHelper))]
		[Browsable(true)]
		[ReflectorProperty("WindowsKeyman", Required = false)]
		public string KeyboardName{ get; set; }

		[Browsable(false)]
		[ReflectorProperty("FontName", Required = true)]
		public string FontName{ get; set; }

		[Browsable(false)]
		[ReflectorProperty("FontSize", Required = true)]
		public int FontSize{ get; set; }

		[ReflectorProperty("RightToLeft", Required = false)]
		public bool RightToLeft{ get; set; }

		[TypeConverter(typeof(SpellCheckerIdToDisplayStringConverter))]
		[ReflectorProperty("SpellCheckingId", Required = false)]
		public string SpellCheckingId{ get; set; }

		[ReflectorProperty("IsAudio", Required = false)]
		public bool IsAudio{ get; set; }

		[ReflectorProperty("IsUnicode", Required = false)]
		public bool IsUnicode{ get; set; }

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
			return _palasoWritingSystemDefinition.Collator.Compare(x, y);
			//return _sortComparer(x, y);
		}

		public int Compare(object x, object y)
		{
			return Compare((string)x, (string)y);
		}

		#endregion

		public override string ToString()
		{
			return _palasoWritingSystemDefinition.Id;
		}

		public SortKey GetSortKey(string source)
		{
			return _palasoWritingSystemDefinition.Collator.GetSortKey(source);
		}

		// Same if behavior is same (not appearance)
		public override int GetHashCode()
		{
			int hashCode = HashCombine(_palasoWritingSystemDefinition.ISO.GetHashCode(), SortUsing.GetHashCode());
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
			return (int)(seed ^ (hash + 0x9e3779b9 + (seed << 6) + (seed >> 2)));
		}

		#region Nested type: KeyboardListHelper

		public class KeyboardListHelper : StringConverter
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
				if ((String)value == String.Empty)
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
				if ((String)value == "default")
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

		public class SpellCheckerIdToDisplayStringConverter : StringConverter
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

				if (destinationType != typeof(string))
				{
					throw GetConvertToException(value, destinationType);
				}
				if ((String)value == "none") //added as Mono bugfix
				{
					return "none";
				}
				if ((String)value == String.Empty)
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
				if ((String)value == "none")
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