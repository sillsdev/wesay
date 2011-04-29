using System.IO;
using System.Xml;
using Palaso.WritingSystems;

namespace WeSay.Project.ConfigMigration.WritingSystem
{
	class WritingSystemsFromLiftCreator
	{
		private readonly string _pathToProjectFolder;

		public WritingSystemsFromLiftCreator(string pathToProjectFolder)
		{
			_pathToProjectFolder = pathToProjectFolder;
		}

		public void CreateNonExistantWritingSystemsFoundInLift(string pathToLiftFile)
		{
			if (File.Exists(pathToLiftFile))
			{
				string pathToLdmlWritingSystemsFolder = BasilProject.GetPathToLdmlWritingSystemsFolder(_pathToProjectFolder);
				IWritingSystemRepository writingSystems = new LdmlInFolderWritingSystemRepository(pathToLdmlWritingSystemsFolder);
				using (var reader = XmlReader.Create(pathToLiftFile))
				{
					while (reader.Read())
					{
						if (reader.MoveToAttribute("lang"))
						{
							if (!writingSystems.Contains(reader.Value))
							{
								string id = reader.Value;
								writingSystems.Set(WritingSystemDefinition.Parse(id));
							}
						}
					}
				}
				writingSystems.Save();
			}
		}
	}
}
