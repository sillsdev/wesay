using System.IO;
using Palaso.Migration;

namespace WeSay.Project.ConfigMigration.WritingSystem
{
	public class WritingSystemPrefsVersionGetter:IFileVersion
	{
		public int GetFileVersion(string source)
		{
			if (File.Exists(source))
			{
				return 0;
			}
			return 1;   //Effectively means that the file has been migrated away.
		}

		public int StrategyGoodToVersion
		{
			get { return 1; }
		}
	}
}
