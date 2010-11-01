using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using WeSay.LexicalModel.Foundation;

namespace WeSay.Project.ConfigMigration.WritingSystems
{
	class WritingSystemsFromLiftCreator
	{
		private string _pathToProjectFolder;

		public WritingSystemsFromLiftCreator(string pathToProjectFolder)
		{
			_pathToProjectFolder = pathToProjectFolder;
		}

		public void CreateNonExistantWritingSystemsFoundInLift(string pathToLiftFile)
		{
			if (File.Exists(pathToLiftFile))
			{
				string pathToLdmlWritingSystemsFolder = BasilProject.GetPathToLdmlWritingSystemsFolder(_pathToProjectFolder);
				WritingSystemCollection writingSystems = new WritingSystemCollection();
				writingSystems.Load(pathToLdmlWritingSystemsFolder);
				using (XmlReader reader = XmlReader.Create(pathToLiftFile))
				{
					while (reader.Read())
					{
						if (reader.MoveToAttribute("lang"))
						{
							if (!writingSystems.Keys.Contains(reader.Value))
							{
								writingSystems.AddSimple(reader.Value);
							}
						}
					}
				}
				writingSystems.Write(pathToLdmlWritingSystemsFolder);
			}
		}
	}
}
