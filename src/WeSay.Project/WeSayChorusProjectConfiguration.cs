using Chorus.sync;

namespace WeSay.Project
{
	class WeSayChorusProjectConfiguration : ProjectFolderConfiguration
	{
		public WeSayChorusProjectConfiguration(string pathToProjectDirectory)
			:base(pathToProjectDirectory)
		{
			//exclude has precedence, but these are redundant as long as we're using the policy
			//that we explicitly include all the files we understand.  At least someday, when these
			//effect what happens in a more persisten way (e.g. be stored in the hgrc), these would protect
			//us a bit from other apps that might try to do a *.* include

			ExcludePatterns.Add("**/cache");
			ExcludePatterns.Add("**/Cache");
			ExcludePatterns.Add("autoFonts.css");
			ExcludePatterns.Add("autoLayout.css");
			ExcludePatterns.Add("factoryDicitonary.css");
			ExcludePatterns.Add("*.old");
			ExcludePatterns.Add("*.WeSayUserMemory");
			ExcludePatterns.Add("*.tmp");
			ExcludePatterns.Add("*.bak");

			IncludePatterns.Add("audio/*.*");
			IncludePatterns.Add("pictures/*.*");
			IncludePatterns.Add("**.css"); //stylesheets
			IncludePatterns.Add("export/*.lpconfig");//lexique pro
			IncludePatterns.Add("**.lift");
			IncludePatterns.Add("**.WeSayConfig");
			IncludePatterns.Add("**.WeSayUserConfig");
			IncludePatterns.Add("**.xml");
			IncludePatterns.Add(".hgIgnore");

		}
	}
}