using System.Xml.XPath;
using SIL.IO;

namespace WeSay.Project.ConfigMigration.WeSayConfig
{
	public class MigrateConfig5To6
	{
		public void Migrate(XPathDocument inputDoc, string outputPath)
		{
			ConfigurationMigrator.MigrateUsingXSLT(inputDoc, "MigrateConfig5To6.xsl", outputPath);
			FileUtils.GrepFile(outputPath, "SemanticDomainDdp4", "semantic-domain-ddp4");
		}


	}
}
