using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Palaso.Annotations;
using WeSay.Foundation.Options;

namespace WeSay.Project
{
	/// <summary>
	/// Uses chorus to regularly backup to a second drive (SD, SSD, etc.)
	/// </summary>
	[XmlRoot("backupPlan")]
	public class ChorusBackupMaker
	{
	  public const string ElementName = "backupPlan";

	  [XmlElement("pathToParentOfRepositories")]
		public string PathToParentOfRepositories;

		public static ChorusBackupMaker LoadFromReader(XmlReader reader)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ChorusBackupMaker));
			return (ChorusBackupMaker)serializer.Deserialize(reader);
		}

		public void Save(XmlWriter writer)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ChorusBackupMaker));
			serializer.Serialize(writer, this);
		}

	}
}
