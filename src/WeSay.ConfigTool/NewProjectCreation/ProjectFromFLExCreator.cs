using System;
using System.IO;
using System.Text;
using System.Xml;
using Palaso.Code;
using Palaso.DictionaryServices.Model;
using Palaso.Reporting;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;

namespace WeSay.ConfigTool.NewProjectCreation
{
	public class ProjectFromFLExCreator
	{
		public static bool Create(string pathToNewDirectory, string pathToSourceLift)
		{
			try
			{
				Logger.WriteEvent(@"Starting Project creation from " + pathToSourceLift);

				if (!ReportIfLocked(pathToSourceLift))
					return false;

				if (!CreateProjectDirectory(pathToNewDirectory))
					return false;

				CopyOverLiftFile(pathToSourceLift, pathToNewDirectory);

				CopyOverRangeFileIfExists(pathToSourceLift, pathToNewDirectory);

				CopyOverLdmlFiles(pathToSourceLift, BasilProject.GetPathToLdmlWritingSystemsFolder(pathToNewDirectory));

				using (var project = new WeSayWordsProject())
				{
					project.LoadFromProjectDirectoryPath(pathToNewDirectory);
					SetWritingSystemsForFields(pathToSourceLift, project.DefaultViewTemplate, project.WritingSystems);
					project.Save();
				}
				Logger.WriteEvent(@"Finished Importing");
				return true;

			}
			catch(Exception e)
			{
				Palaso.Reporting.ErrorReport.NotifyUserOfProblem(e, "WeSay was unable to finish importing that LIFT file.  If you cannot fix the problem yourself, please zip and send the exported folder to issues (at) wesay (dot) org");
				try
				{
					Logger.WriteEvent(@"Removing would-be target directory");
					Directory.Delete(pathToNewDirectory, true);
				}
				catch (Exception)
				{
					//swallow
				}
				return false;
			}
		}

		private static void CopyOverLdmlFiles(string pathToSourceLift, string pathToNewDirectory)
		{
			foreach (string pathToLdml in Directory.GetFiles(Path.GetDirectoryName(pathToSourceLift), "*.ldml"))
			{
				string fileName = Path.GetFileName(pathToLdml);
				Logger.WriteMinorEvent(@"Copying LDML file " + fileName);
				File.Copy(pathToLdml, Path.Combine(pathToNewDirectory, fileName), true);
			}
		}

		private static void CopyOverLiftFile(string pathToSourceLift, string pathToNewDirectory)
		{
			var projectName = Path.GetFileNameWithoutExtension(pathToNewDirectory);
			var pathToTargetLift = Path.Combine(pathToNewDirectory, projectName+".lift");
			Logger.WriteMinorEvent(@"Copying Lift file " + pathToSourceLift);
			File.Copy(pathToSourceLift, pathToTargetLift, true);
		}

		private static void CopyOverRangeFileIfExists(string pathToSourceLift, string pathToNewDirectory)
		{
			var projectName = Path.GetFileNameWithoutExtension(pathToNewDirectory);
			var  pathToSourceRange= pathToSourceLift+ "-ranges";
			if(File.Exists(pathToSourceRange))
			{
				var pathToTargetRanges = Path.Combine(pathToNewDirectory, projectName + ".lift-ranges");
				Logger.WriteMinorEvent(@"Copying Range file " + pathToTargetRanges);
				File.Copy(pathToSourceRange, pathToTargetRanges, true);
			}
		}

		private static bool CreateProjectDirectory(string directory)
		{
			try
			{
				RequireThat.Directory(directory).DoesNotExist();
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

		public static void SetWritingSystemsForFields(string path, ViewTemplate viewTemplate, IWritingSystemRepository writingSystems)
		{
			var doc = new XmlDocument();
			doc.Load(path); //will throw if the file is ill-formed
			var missingWritingSystems = new StringBuilder();

			foreach (XmlNode node in doc.SelectNodes("//@lang"))
			{
				if (!writingSystems.Contains(node.Value))
				{
					writingSystems.Set(WritingSystemDefinition.Parse(node.Value));
					missingWritingSystems.AppendFormat("{0},", node.Value);
				}
			}

			if(missingWritingSystems.Length > 0)
			{
				var list = missingWritingSystems.ToString().Trim(new char[]{','});
					ErrorReport.NotifyUserOfProblem(
						"WeSay had a problem locating information on at least one writing system used in the LIFT export from FLEx.  One known cause of this is an old version of FLEx. In the folder containing the LIFT file, there should have been '___.ldml' files for the following writing systems: {0}.\r\nBecause these Writing System definitions were not found, WeSay will create blank writing systems for each of these, which you will need to set up with the right fonts, keyboards, etc.", list);
			}
			// replace all "v" fields with the first lexical-unit writing system
			//and all "en" with the first translation one...

			var vernacular = GetTopWritingSystem(doc, "//lexical-unit/form/@lang");
			if (vernacular != string.Empty)
			{
				viewTemplate.OnWritingSystemIDChange(WritingSystemInfo.IdForUnknownVernacular, vernacular);
				writingSystems.Remove(WritingSystemInfo.IdForUnknownVernacular);
			}
			var analysis = GetTopWritingSystem(doc, "//sense/gloss/@lang");
			if (analysis == string.Empty)
			{
				analysis = GetTopWritingSystem(doc, "//sense/definition/@lang");
				//nb: we don't want to remove english, even if they don't use it
			}
			if (analysis != string.Empty)
			{
				viewTemplate.OnWritingSystemIDChange(WritingSystemInfo.IdForUnknownAnalysis, analysis);
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