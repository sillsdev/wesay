using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Palaso.Migration;

namespace WeSay.Project.ConfigMigration
{
	internal class XslFromResourceMigrator : XslMigrationStrategy
	{
		public XslFromResourceMigrator(int fromVersion, int toVersion, string xslSource) : base(fromVersion, toVersion, xslSource)
		{
		}

		protected override TextReader OpenXslStream(string xslSource)
		{
			string resourceName = "ConfigMigration." + xslSource;
			var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
				typeof(WeSayWordsProject), resourceName
			);
			return new StreamReader(stream);
		}
	}
}
