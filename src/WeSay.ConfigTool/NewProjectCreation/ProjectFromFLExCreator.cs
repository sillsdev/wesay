using System;
using System.IO;
using System.Xml;
using Palaso.DictionaryServices.Model;
using Palaso.Reporting;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;

namespace WeSay.ConfigTool.NewProjectCreation
{
	public class ProjectFromFLExCreator
	{
		public static bool Create(string pathToNewDirectory, string pathToSourceLift)
		{
			if (!ReportIfLocked(pathToSourceLift))
				return false;

			if (!CreateProjectDirectory(pathToNewDirectory))
				return false;

			CopyOverLiftFile(pathToSourceLift, pathToNewDirectory);

			CopyOverRangeFileIfExists(pathToSourceLift, pathToNewDirectory);

			RemoveUnneededWritingSystems(pathToNewDirectory);
			CopyOverLdmlFiles(pathToSourceLift, BasilProject.GetPathToLdmlWritingSystemsFolder(pathToNewDirectory));

			using (var project = new WeSayWordsProject())
			{
				project.LoadFromProjectDirectoryPath(pathToNewDirectory);
				project.Save();
			}
			return true;
		}

		private static void RemoveUnneededWritingSystems(string pathToNewDirectory)
		{
			foreach (string path in Directory.GetFiles(BasilProject.GetPathToLdmlWritingSystemsFolder(pathToNewDirectory)))
			{
				File.Delete(path);
			}
		}

		private static void CopyOverLdmlFiles(string pathToSourceLift, string pathToNewDirectory)
		{
			foreach (string pathToLdml in Directory.GetFiles(Path.GetDirectoryName(pathToSourceLift), "*.ldml"))
			{
				string fileName = Path.GetFileName(pathToLdml);
				File.Copy(pathToLdml, Path.Combine(pathToNewDirectory, fileName), true);
			}
		}

		private static void CopyOverLiftFile(string pathToSourceLift, string pathToNewDirectory)
		{
			var projectName = Path.GetFileNameWithoutExtension(pathToNewDirectory);
			var pathToTargetLift = Path.Combine(pathToNewDirectory, projectName+".lift");
			File.Copy(pathToSourceLift, pathToTargetLift, true);
		}

		private static void CopyOverRangeFileIfExists(string pathToSourceLift, string pathToNewDirectory)
		{
			var projectName = Path.GetFileNameWithoutExtension(pathToNewDirectory);
			var  pathToSourceRange= pathToSourceLift+ "-ranges";
			if(File.Exists(pathToSourceRange))
			{
				var pathToTargetRanges = Path.Combine(pathToNewDirectory, projectName + ".lift-ranges");
				File.Copy(pathToSourceRange, pathToTargetRanges, true);
			}
		}

		private static bool CreateProjectDirectory(string directory)
		{
			try
			{
				WeSayWordsProject.CreateEmptyProjectFiles(directory);
			}
			catch (Exception e)
			{
				ErrorReport.NotifyUserOfProblem(
					"WeSay was not able to create a project there. \r\n" + e.Message);
				return false;
			}
			return true;
		}


		private static bool ReportIfLocked(string lift)
		{
			try
			{
				using (var lockTest = File.OpenRead(lift))
				{
				}
			}
			catch (Exception)
			{
				ErrorReport.NotifyUserOfProblem("Could not open the LIFT file.  Is some other program reading or writing it?");
				return false;
			}
			return true;
		}

		public static void LoadWritingSystemsFromExistingLiftTA(string path, ViewTemplate viewTemplate, WritingSystemCollection writingSystems)
		{
			var doc = new XmlDocument();
			doc.Load(path);
			foreach (XmlNode node in doc.SelectNodes("//@lang"))
			{
				if (!writingSystems.ContainsKey(node.Value))
				{
					writingSystems.AddSimple(node.Value);
				}
			}


			// replace all "v" fields with the first lexical-unit writing system
			//and all "en" with the first translation one...

			var vernacular = GetTopWritingSystem(doc, "//lexical-unit/form/@lang");
			if (vernacular != string.Empty)
			{
				viewTemplate.ChangeWritingSystemId(WritingSystem.IdForUnknownVernacular, vernacular);
				writingSystems.Remove(WritingSystem.IdForUnknownVernacular);
			}
			var analysis = GetTopWritingSystem(doc, "//sense/gloss/@lang");
			if (analysis == string.Empty)
			{
				analysis = GetTopWritingSystem(doc, "//sense/definition/@lang");
				//nb: we don't want to remove english, even if they don't use it
			}
			if (analysis != string.Empty)
			{
				viewTemplate.ChangeWritingSystemId(WritingSystem.IdForUnknownAnalysis, analysis);
			}

			AddWritingSystemsForField(doc, viewTemplate, "//lexical-unit/form/@lang", LexEntry.WellKnownProperties.LexicalUnit);
			AddWritingSystemsForField(doc, viewTemplate, "//sense/gloss/@lang", LexSense.WellKnownProperties.Gloss);

			AddWritingSystemsForField(doc, viewTemplate, "//sense/definition/form/@lang", LexSense.WellKnownProperties.Definition);

			AddAllGlossWritingSystemsToDefinition(viewTemplate);

			AddWritingSystemsForField(doc, viewTemplate, "//example/form/@lang", LexExampleSentence.WellKnownProperties.ExampleSentence);
			AddWritingSystemsForField(doc, viewTemplate, "//translation/form/@lang", LexExampleSentence.WellKnownProperties.Translation);

			//------------ hack
			var gloss = viewTemplate.GetField(LexSense.WellKnownProperties.Gloss);
			var def = viewTemplate.GetField(LexSense.WellKnownProperties.Definition);

			foreach (var id in def.WritingSystemIds)
			{
				if(!gloss.WritingSystemIds.Contains(id))
					gloss.WritingSystemIds.Add(id);
			}
			foreach (var id in gloss.WritingSystemIds)
			{
				if (!def.WritingSystemIds.Contains(id))
					def.WritingSystemIds.Add(id);
			}
		}

		private static string GetTopWritingSystem(XmlDocument doc, string xpath)
		{
			var nodes = doc.SelectNodes(xpath);
			if (nodes != null && nodes.Count > 0)
			{
				return nodes[0].Value;
			}
			return string.Empty;
		}

		/// <summary>
		/// This is done because even if they don't use definitions, their glosses are going to be moved over the definition field.
		/// </summary>
		/// <param name="viewTemplate"></param>
		private static void AddAllGlossWritingSystemsToDefinition(ViewTemplate viewTemplate)
		{
			var defField = viewTemplate.GetField(LexSense.WellKnownProperties.Definition).WritingSystemIds;
			foreach (var id in viewTemplate.GetField(LexSense.WellKnownProperties.Gloss).WritingSystemIds)
			{
				if(!defField.Contains(id))
				{
					defField.Add(id);
				}
			}
		}

		private static void AddWritingSystemsForField(XmlDocument doc, ViewTemplate viewTemplate, string xpath, string fieldName)
		{
			var f = viewTemplate.GetField(fieldName);

			//now add in what we find
			XmlNodeList nodes = doc.SelectNodes(xpath);
			if(nodes!=null && nodes.Count > 0)
			{
				//ok, so there is at least one match. Take out all of the default writing system from this
				//field before adding in the ones that were being used in FLEx.
				f.WritingSystemIds.Clear();
			}

			foreach (XmlNode node in nodes)
			{
				if (!f.WritingSystemIds.Contains(node.Value))
				{
					f.WritingSystemIds.Add(node.Value);
				}
			}
		}

		/*       public static IEnumerable<string> GetUniqueRegexMatches(string inputPath, string pattern)
		{
//            var ids = GetUniqueRegexMatches(path, "lang:b*=:b*((\"{.*}\")|('{.*}'))");

   var unique = new List<string>();//review probably slow for this purpose
			List<string> matches= new List<string>();
			Regex regex = new Regex(pattern, RegexOptions.Compiled);
			using (StreamReader reader = File.OpenText(inputPath))
			{
				while (!reader.EndOfStream)
				{
					foreach (Match match in regex.Matches(reader.ReadLine()))
					{
						if (match.Captures.Count > 0)
						{
							var id = match.Captures[0].ToString();
							if (!unique.Contains(id))
							{
								unique.Add(id);
							}
						}
					}
				}
				reader.Close();
			}
			return unique;
		}*/
	}
}