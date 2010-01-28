using System.Xml.XPath;
using Palaso.IO;
using WeSay.Project;

namespace WeSay.Project.ConfigMigration
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
