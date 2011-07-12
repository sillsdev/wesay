using System.Collections.Generic;
using Palaso.WritingSystems;
using Palaso.WritingSystems.Migration;

namespace WeSay.Project
{
	public class OrphanFinder
	{
		public delegate void IdReplacementStrategy(string newId, string oldId);
		public delegate IEnumerable<string> IdFindingStrategy();

		public static void FindOrphans(
			IdFindingStrategy idFindingStrategy,
			IdReplacementStrategy replacementStrategy,
			IWritingSystemRepository writingSystemRepository
			)
		{
			foreach (var wsId in idFindingStrategy())
			{
				// Check if it's in the repo
				if (writingSystemRepository.Contains(wsId))
				{
					continue;
				}
				// It's an orphan
				// Clean it
				var rfcTagCleaner = new Rfc5646TagCleaner(wsId);
				rfcTagCleaner.Clean();
				var conformantWritingSystem = WritingSystemDefinition.Parse(rfcTagCleaner.GetCompleteTag());
				// If it changed, then change
				if (conformantWritingSystem.RFC5646 != wsId)
				{
					conformantWritingSystem = WritingSystemDefinition.CreateCopyWithUniqueId(conformantWritingSystem, idFindingStrategy());
					replacementStrategy(wsId, conformantWritingSystem.RFC5646);
				}
				// Check if it's in the repo
				if (writingSystemRepository.Contains(conformantWritingSystem.RFC5646))
				{
					continue;
				}
				// It's not in the repo so set it
				writingSystemRepository.Set(conformantWritingSystem);
			}
			writingSystemRepository.Save();
		}
	}
}
