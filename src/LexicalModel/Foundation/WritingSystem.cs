using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using Enchant;
using Palaso.i18n;
using Palaso.WritingSystems;

namespace WeSay.LexicalModel.Foundation
{
	public class WritingSystem : WritingSystemDefinition
	{
		// TODO Use of this ctor should be removed.  Maybe a helper?
		public WritingSystem(string testWritingSystemVernId, Font font)
		{
			throw new NotImplementedException();
		}

		//TODO This is fine to keep, but if a real WS is to be created, e.g. in tests then use the 1 arg ctor
		public WritingSystem()
		{
			throw new NotImplementedException();
		}

		//public WritingSystem(string id)
		//{
		//}

		//public string DefaultFontName { get; private set; }
		//public int DefaultFontSize { get; set; }
		public bool IsUnicodeEncoded { get; set; } // TODO Introduce IsUnicodeEncoded to palaso wsd.

		//public bool IsVoice { get; set; } // Rename to IsVoice
		//public bool RightToLeftScript { get; set; }
		//public string Keyboard { get; set; }

		// TODO Move to a helper

		//public void SetFont(Font value)
		//{
		//    throw new NotImplementedException();
		//}

		//public Font CreateFont()
		//{
		//    return new Font(DefaultFontName, DefaultFontSize);
		//}

		public object CustomSortRules { get; set; }

		//public string Id
		//{
		//    get { throw new NotImplementedException(); }
		//}

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

		// TODO this is only used in tests: should we care about conversion to/from string?
		public new string SortUsing { get; set; }

		// TODO Should be able to use .Collator on PWSD
		//public IComparable Compare(string èdit, string edít)
		//{
		//    throw new NotImplementedException();
		//}

	}
}