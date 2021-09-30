using SIL.Code;
using SIL.Reporting;
using SIL.LCModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using WeSay.Project;

namespace WeSay.ConfigTool.NewProjectCreation
{
	/// <summary>
	/// This class should be on its way to being un-needed, with the advent of "Send/Receive".  Its use was primarily pre-chorus,
	/// pre-lift bridge, when we wanted to copy lift-related files around.  In the new, better system, we are given a chorus
	/// repository with all the lift files, just lacking the wesay specific ones. Then, ProjectFromLiftFolderCreator is
	/// used to just add the files we need for WeSay.
	/// This class uses that, too, but it also has to create a folder & copy files around.
	/// </summary>
	public class ProjectFromRawFLExLiftFilesCreator
	{
		public static bool Create(string pathToNewDirectory, string pathToSourceLift)
		{
			try
			{
				Logger.WriteEvent(@"Starting Project creation from " + pathToSourceLift);

				if (!ReportIfLocked(pathToSourceLift))
					return false;
				
				//SIL.LCModel.DomainServices.DataMigration.PerformMigration(Path.GetDirectoryName(pathToSourceLift));
				RequireThat.Directory(pathToNewDirectory).DoesNotExist();

				Directory.CreateDirectory(pathToNewDirectory);

				CopyOverLiftFile(pathToSourceLift, pathToNewDirectory);

				CopyOverPictures(Path.GetDirectoryName(pathToSourceLift), BasilProject.GetPathToPictures(pathToNewDirectory));
				//what about audio?
				CopyOverAudio(Path.GetDirectoryName(pathToSourceLift), BasilProject.GetPathToPictures(pathToNewDirectory));

				CopyOverRangeFileIfExists(pathToSourceLift, pathToNewDirectory);

				CopyOverLdmlFiles(pathToSourceLift, BasilProject.GetPathToLdmlWritingSystemsFolder(pathToNewDirectory));

				File.Create(Path.Combine(pathToNewDirectory, ".newlycreatedfromFLEx")).Dispose();

				//The config file is created on project open when all of the orphaned writing systems have been identified.

				Logger.WriteEvent(@"Finished Importing");
				return true;

			}
			catch (Exception e)
			{
				SIL.Reporting.ErrorReport.NotifyUserOfProblem(e, "WeSay was unable to finish importing that LIFT file.  If you cannot fix the problem yourself, please zip and send the exported folder to issues (at) wesay (dot) org");
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

		public static HashSet<string> AllLangs(string str, bool ignoreCase = false)
		{
			string substr = "lang=\"";
			if (string.IsNullOrWhiteSpace(str) ||
				string.IsNullOrWhiteSpace(substr))
			{
				throw new ArgumentException("String or substring is not specified.");
			}

			var langs = new HashSet<string>();
			int index = 0;
			index = str.IndexOf(substr, index, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
			
			while (index != -1)
			{
				//				indexes.Add(index++);
				int start = index + 6;
				int end = str.IndexOf(">", start) -1;
				int length = end - start;
				var alang = str.Substring(start, length);
				langs.Add(alang);
				index++;
				index = str.IndexOf(substr, index, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

			}

			return langs;
		}

		private static void CopyOverLdmlFiles(string pathToSourceLift, string pathToNewDirectory)
		{
			if (!Directory.Exists(pathToNewDirectory))
			{
				Directory.CreateDirectory(pathToNewDirectory);
			}
			string pathToSourceWritingSystems = Path.Combine(Path.GetDirectoryName(pathToSourceLift), "WritingSystems");
			if (!Directory.Exists(pathToSourceWritingSystems))
			{
				pathToSourceWritingSystems = Path.GetDirectoryName(pathToSourceLift);
			}
			// make set of all writing systems used in .lift and lift-ranges(if present)
			//			var ldml_lift = Regex.Matches(File.ReadAllText(pathToSourceLift, Encoding.UTF8), @"<[^<>]*>").Cast<Match>().Select(p => p.Value).ToList();
			HashSet<string> uniqueLdmls = AllLangs(File.ReadAllText(pathToSourceLift, Encoding.UTF8));
			string liftRangePath = Path.ChangeExtension(pathToSourceLift, "lift-ranges");
			if (File.Exists(liftRangePath))
			{
				HashSet<string> ldmlRange = AllLangs(File.ReadAllText(liftRangePath, Encoding.UTF8));
				foreach (string alang in ldmlRange)
				{
					uniqueLdmls.Add(alang);
				}
			}
			//remove any references to "qaa-x-spec" here so that it doesn't matter which order lift and ldml files are copied in!
			uniqueLdmls.Remove("qaa-x-spec");
			// then only copy used writing systems!
			foreach (string alang in uniqueLdmls)
			{
				if (alang.Length > 0)
				{
					string fileName = alang + ".ldml";
					Logger.WriteMinorEvent(@"Copying LDML file " + fileName);
					File.Copy(Path.Combine(pathToSourceWritingSystems, fileName), Path.Combine(pathToNewDirectory, fileName), true);
				}
			}
		}

		private static void CopyOverLiftFile(string pathToSourceLift, string pathToNewDirectory)
		{
			var projectName = Path.GetFileNameWithoutExtension(pathToNewDirectory);
			var pathToTargetLift = Path.Combine(pathToNewDirectory, projectName + ".lift");
			//open source lift file as xml and strip out qaa-x-spec
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(pathToSourceLift);
			XmlNodeList xnList = xmlDoc.SelectNodes("/lift/header/fields/field/form[@lang='qaa-x-spec']");
			foreach (XmlNode xn in xnList)  //each Field node
			{
				if (xn != null)
				{
					xn.ParentNode.RemoveChild(xn);
				}
			}
			xmlDoc.Save(pathToTargetLift);
			//change href attribute of range nodes if they exist
			xmlDoc.Load(pathToSourceLift);
			xnList = xmlDoc.SelectNodes("/lift/header/ranges/range");
			foreach (XmlNode xn in xnList)  //each Field node
			{
				if (xn != null)
				{
					xn.Attributes["href"].Value = "file://" + pathToTargetLift + "-ranges";
				}
			}
			Logger.WriteMinorEvent(@"Copying Lift file " + pathToSourceLift);
			xmlDoc.Save(pathToTargetLift);
		}

		private static void CopyOverPictures(string pathToSourceLift, string pathToNewDirectory)
		{
			var pathToSourcePictures = Path.Combine(pathToSourceLift, "pictures");
			if (Directory.Exists(pathToSourcePictures))
			{
				if (!Directory.Exists(pathToNewDirectory))
				{
					Directory.CreateDirectory(pathToNewDirectory);
				}
				foreach (string pathToPicture in Directory.GetFiles(pathToSourcePictures))
				{
					string fileName = Path.GetFileName(pathToPicture);
					Logger.WriteMinorEvent(@"Copying picture " + fileName);
					File.Copy(pathToPicture, Path.Combine(pathToNewDirectory, fileName), true);
				}
			}
		}

		private static void CopyOverAudio(string pathToSourceLift, string pathToNewDirectory)
		{
			var pathToSourceAudio = Path.Combine(pathToSourceLift, "audio");
			if (Directory.Exists(pathToSourceAudio))
			{
				if (!Directory.Exists(pathToNewDirectory))
				{
					Directory.CreateDirectory(pathToNewDirectory);
				}
				foreach (string pathToAudio in Directory.GetFiles(pathToSourceAudio))
				{
					string fileName = Path.GetFileName(pathToAudio);
					Logger.WriteMinorEvent(@"Copying audio " + fileName);
					File.Copy(pathToAudio, Path.Combine(pathToNewDirectory, fileName), true);
				}
			}
		}

		private static void CopyOverRangeFileIfExists(string pathToSourceLift, string pathToNewDirectory)
		{
			var projectName = Path.GetFileNameWithoutExtension(pathToNewDirectory);
			var pathToSourceRange = pathToSourceLift + "-ranges";
			if (File.Exists(pathToSourceRange))
			{
				var pathToTargetRanges = Path.Combine(pathToNewDirectory, projectName + ".lift-ranges");
				Logger.WriteMinorEvent(@"Copying Range file " + pathToTargetRanges);
				File.Copy(pathToSourceRange, pathToTargetRanges, true);
			}
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