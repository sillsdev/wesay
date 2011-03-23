using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Palaso.Migration;
using Palaso.WritingSystems;
using Palaso.WritingSystems.Migration;

namespace WeSay.LexicalModel.Foundation.WritingSystemMigration
{
	class WesayWsPrefsToPalasoWsLdmlMigrationStrategy:IMigrationStrategy
	{
		private ConsumerLevelRfcTagChanger _changer;

		public WesayWsPrefsToPalasoWsLdmlMigrationStrategy(ConsumerLevelRfcTagChanger rfcTagChanger)
		{
			_changer = rfcTagChanger;
		}

		public void Migrate(string sourceFilePath, string destinationFilePath)
		{
			if(!Directory.Exists(destinationFilePath))
			{
				Directory.CreateDirectory(destinationFilePath);
			}
			var _wesayWsCollection = new WritingSystemCollection_V1();
			_wesayWsCollection.LoadFromLegacyWeSayFile(sourceFilePath);
			foreach (var writingSystem in _wesayWsCollection.Values)
			{
				var wsDef = new PalasoWritingSystemDefinitionV0();
				wsDef.ISO639 = writingSystem.ISO;
				//wsDef.Script = writingSystem.Script;
				//wsDef.Region = writingSystem.Region;
				//wsDef.Variant = writingSystem.Variant;
				//wsDef.DefaultFontName = writingSystem.FontName;
				//wsDef.Abbreviation = writingSystem.Abbreviation;
				//wsDef.DefaultFontSize = writingSystem.FontSize;
				//wsDef.IsLegacyEncoded = !writingSystem.IsUnicode;
				//wsDef.Keyboard = writingSystem.KeyboardName;
				//wsDef.RightToLeftScript = writingSystem.RightToLeft;
				//wsDef.SortRules = writingSystem.CustomSortRules;
				//wsDef.SortUsing = GetEquivalentPalasoSortRulesType(writingSystem.SortUsing);
				//wsDef.SpellCheckingId = writingSystem.SpellCheckingId;
				//wsDef.DateModified = DateTime.Now;
				//wsDef.VerboseDescription //not written out by ldmladaptor - flex?
				//wsDef.StoreID = ??? //what to do?
				//wsDef.NativeName //not written out by ldmladaptor - flex?

				string pathForNewLdmlFile = Path.Combine(destinationFilePath, wsDef.Rfc5646 + ".ldml");
				new PalasoLdmlAdaptorV0().Write(pathForNewLdmlFile, wsDef, Stream.Null);
			}
		}

		private PalasoWritingSystemDefinitionV0.SortRulesType GetEquivalentPalasoSortRulesType(string sortRulesType)
		{
			//if(sortRulesType.Equals("CustomICU"))
			//{
			//    return PalasoWritingSystemDefinitionV0.SortRulesType.CustomICU;
			//}
			//if(sortRulesType.Equals("CustomSimple"))
			//{
			//    return PalasoWritingSystemDefinitionV0.SortRulesType.CustomSimple;
			//}
			//return PalasoWritingSystemDefinitionV0.SortRulesType.DefaultOrdering;
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
	}
}
