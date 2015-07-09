using System.Xml.XPath;
using SIL.IO;

namespace WeSay.Project.ConfigMigration.WeSayConfig
{
	public class MigrateConfig8To9
	{
		public void Migrate(XPathDocument inputDoc, string outputPath)
		{
			ConfigurationMigrator.MigrateUsingXSLT(inputDoc, "MigrateConfig8To9.xsl", outputPath);
			FileUtils.GrepFile(outputPath, "PartOfSpeech", "Parts of Speech");
		}


	}
}
