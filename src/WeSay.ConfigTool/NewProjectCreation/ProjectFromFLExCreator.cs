using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Palaso.Reporting;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;
using System.Linq;

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

			using (var project = new WeSayWordsProject())
			{
				project.LoadFromProjectDirectoryPath(pathToNewDirectory);

				LoadWritingSystemsFromExistingLift(pathToSourceLift, project.DefaultViewTemplate, project.WritingSystems);
				project.Save();
			}
			return true;
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
				Palaso.Reporting.ErrorReport.NotifyUserOfProblem("Could not open the LIFT file.  Is some other program reading or writing it?");
				return false;
			}
			return true;
		}

		public static void LoadWritingSystemsFromExistingLift(string path, ViewTemplate viewTemplate, WritingSystemCollection writingSystems)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(path);
			foreach (XmlNode node in doc.SelectNodes("//@lang"))
			{
				if (!writingSystems.ContainsKey(node.Value))
				{
					writingSystems.AddSimple(node.Value);
				}
			}

			//enhance... we could go through replacing all "v" fields with the first lexical-unit writing system
			//and all "en" with the first translation one...

			writingSystems.Remove(WritingSystem.IdForUnknownVernacular);

			AddWritingSystemsForField(doc, viewTemplate, "//lexical-unit/form/@lang", LexEntry.WellKnownProperties.LexicalUnit);
			AddWritingSystemsForField(doc, viewTemplate, "//sense/gloss/form/@lang", LexSense.WellKnownProperties.Gloss);
			AddWritingSystemsForField(doc, viewTemplate, "//sense/definition/form/@lang", LexSense.WellKnownProperties.Definition);
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

		private static void AddWritingSystemsForField(XmlDocument doc, ViewTemplate viewTemplate, string xpath, string fieldName)
		{
			var f = viewTemplate.GetField(fieldName);
		   //first take out all the existing ones
			f.WritingSystemIds.Clear();

			//now add in what we find
			foreach (XmlNode node in doc.SelectNodes(xpath))
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