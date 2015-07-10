using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SIL.Migration;
using SIL.WritingSystems;
using SIL.WritingSystems.Migration;
using SIL.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;

namespace WeSay.Project.ConfigMigration.WritingSystem
{
	public class WritingSystemPrefsToLdmlMigrationStrategy : IMigrationStrategy
	{
		internal class SubTag
		{
			private readonly List<string> _subTagParts;

			public SubTag()
			{
				_subTagParts = new List<string>();
			}

			public SubTag(SubTag rhs)
			{
				_subTagParts = new List<string>(rhs._subTagParts);
			}

			public int Count
			{
				get { return _subTagParts.Count; }
			}

			public string CompleteTag
			{
				get
				{
					if (_subTagParts.Count == 0)
					{
						return String.Empty;
					}
					string subtagAsString = "";
					foreach (string part in _subTagParts)
					{
						if (!String.IsNullOrEmpty(subtagAsString))
						{
							subtagAsString = subtagAsString + "-";
						}
						subtagAsString = subtagAsString + part;
					}
					return subtagAsString;
				}
			}

			public IEnumerable<string> AllParts
			{
				get { return _subTagParts; }
			}

			public static List<string> ParseSubtagForParts(string subtagToParse)
			{
				var parts = new List<string>();
				parts.AddRange(subtagToParse.Split('-'));
				parts.RemoveAll(str => str == "");
				return parts;
			}

			public void AddToSubtag(string partsToAdd)
			{
				List<string> partsOfStringToAdd = ParseSubtagForParts(partsToAdd);
				foreach (string part in partsOfStringToAdd)
				{
					if (StringContainsNonAlphaNumericCharacters(part))
					{
						throw new ArgumentException(String.Format("Rfc5646 tags may only contain alphanumeric characters. '{0}' can not be added to the Rfc5646 tag.", part));
					}
					if (Contains(part))
					{
						throw new ArgumentException(String.Format("Subtags may not contain duplicates. The subtag '{0}' was already contained.", part));
					}
					_subTagParts.Add(part);
				}
			}

			private static bool StringContainsNonAlphaNumericCharacters(string stringToSearch)
			{
				return stringToSearch.Any(c => !Char.IsLetterOrDigit(c));
			}

			public void RemoveAllParts(string partsToRemove)
			{
				List<string> partsOfStringToRemove = ParseSubtagForParts(partsToRemove);

				foreach (string partToRemove in partsOfStringToRemove)
				{
					if (!Contains(partToRemove))
					{
						continue;
					}
					int indexOfPartToRemove = _subTagParts.FindIndex(partInSubtag => partInSubtag.Equals(partToRemove, StringComparison.OrdinalIgnoreCase));
					_subTagParts.RemoveAt(indexOfPartToRemove);
				}
			}

			public bool Contains(string partToFind)
			{
				return _subTagParts.Any(part => part.Equals(partToFind, StringComparison.OrdinalIgnoreCase));
			}

		}

		private readonly Action<int, IEnumerable<LdmlMigrationInfo>> _migrationHandler;
		private IAuditTrail _changeLog;

		public WritingSystemPrefsToLdmlMigrationStrategy(Action<int, IEnumerable<LdmlMigrationInfo>> migrationHandler, IAuditTrail changeLog)
		{
			_migrationHandler = migrationHandler;
			_changeLog = changeLog;
		}

		public void Migrate(string sourceFilePath, string destinationFilePath)
		{
			string sourceFileName = Path.GetFileName(sourceFilePath);

			var migrationInfo = new List<LdmlMigrationInfo>();
			if(!Directory.Exists(destinationFilePath))
			{
				Directory.CreateDirectory(destinationFilePath);
			}
			var wesayWsCollection = new WritingSystemCollection_V1();
			wesayWsCollection.LoadFromLegacyWeSayFile(sourceFilePath);

			foreach (var writingSystem in wesayWsCollection.Values)
			{
				var currentMigrationInfo = new SIL.WritingSystems.Migration.LdmlMigrationInfo(sourceFileName)
				{
					LanguageTagBeforeMigration = writingSystem.ISO
				};
				var wsDef = new WritingSystemDefinitionV0();
				if(writingSystem.IsAudio)
				{
					wsDef.Script = WellKnownSubTags.Audio.Script;
					wsDef.Variant = WellKnownSubTags.Audio.PrivateUseSubtag;
				}
				else
				{
					var subtag = new SubTag();
					subtag.AddToSubtag(writingSystem.ISO);
					subtag.RemoveAllParts("audio");
					writingSystem.ISO = subtag.CompleteTag;
				}

				wsDef.ISO639 = writingSystem.ISO;
				wsDef.DefaultFontName = writingSystem.FontName;
				wsDef.DefaultFontSize = writingSystem.FontSize;
				wsDef.Abbreviation = writingSystem.Abbreviation;
				wsDef.IsLegacyEncoded = !writingSystem.IsUnicode;
				wsDef.Keyboard = writingSystem.KeyboardName;
				wsDef.RightToLeftScript = writingSystem.RightToLeft;
				wsDef.SpellCheckingId = writingSystem.SpellCheckingId;
				wsDef.DateModified = DateTime.Now;
				if (writingSystem.SortUsing.Equals("CustomICU"))
				{
					wsDef.SortRules = writingSystem.CustomSortRules;
					wsDef.SortUsing = WritingSystemDefinitionV0.SortRulesType.CustomICU;
				}
				else if (writingSystem.SortUsing.Equals("CustomSimple"))
				{
					wsDef.SortRules = writingSystem.CustomSortRules;
					wsDef.SortUsing = WritingSystemDefinitionV0.SortRulesType.CustomSimple;
				}
				else if (!String.IsNullOrEmpty(writingSystem.SortUsing))
				{
					//when sorting like other language
					wsDef.SortRules = writingSystem.SortUsing;
					wsDef.SortUsing = WritingSystemDefinitionV0.SortRulesType.OtherLanguage;
				}

				//wsDef.VerboseDescription //not written out by ldmladaptor - flex?
				//wsDef.StoreID = ??? //what to do?
				//wsDef.NativeName //not written out by ldmladaptor - flex?

				string pathForNewLdmlFile = Path.Combine(destinationFilePath, wsDef.Rfc5646 + ".ldml");
				new LdmlAdaptorV0().Write(pathForNewLdmlFile, wsDef, null);

				currentMigrationInfo.LanguageTagAfterMigration = wsDef.Rfc5646;
				if (currentMigrationInfo.LanguageTagBeforeMigration != currentMigrationInfo.LanguageTagAfterMigration)
				{
					_changeLog.LogChange(currentMigrationInfo.LanguageTagBeforeMigration, currentMigrationInfo.LanguageTagAfterMigration);
				}
				migrationInfo.Add(currentMigrationInfo);
			}
			_migrationHandler(0, migrationInfo);
		}

		public void PreMigrate()
		{
			throw new NotImplementedException();
		}

		public void PostMigrate(string sourcePath, string destinationPath)
		{
			throw new NotImplementedException();
		}

		public int FromVersion
		{
			get { return 0; }
		}

		public int ToVersion
		{
			get { return 1; }
		}

		public class WellKnownSubTags
		{
			public class Unwritten
			{
				public const string Script = "Zxxx";
			}

			public class Audio
			{
				public const string PrivateUseSubtag = "x-audio";
				public const string Script = Unwritten.Script;
			}

			public class Ipa
			{
				public const string VariantSubtag = "fonipa";
				public const string PhonemicPrivateUseSubtag = "x-emic";
				public const string PhoneticPrivateUseSubtag = "x-etic";
			}
		}
	}
}
