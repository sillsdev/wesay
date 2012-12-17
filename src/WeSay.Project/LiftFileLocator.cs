using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Palaso.Reporting;

namespace WeSay.Project
{
	public static class LiftFileLocator
	{
		public static string LocateAt(string liftPath)
		{
			liftPath = GetFullPath(liftPath);
			if (!File.Exists(liftPath))
			{
				ErrorReport.NotifyUserOfProblem(
						String.Format(
								"WeSay tried to find the lexicon at '{0}', but could not find it.\r\n\r\nTry opening the LIFT file by double clicking on it.",
								liftPath));
				return null;
			}
			if (!FileIsUsable(liftPath))
			{
				return null;
			}
			return liftPath;
		}
		public static string LocateInDirectoryQuietly(string projectDirectoryPath)
		{
			return LocateInDirectory(projectDirectoryPath, false);
		}

		public static string LocateInDirectory(string projectDirectoryPath)
		{
			return LocateInDirectory(projectDirectoryPath, true);
		}

		private static string LocateInDirectory(string projectDirectoryPath, bool canNotify)
		{
			string preferredLiftFile;
			var liftPaths = Directory.GetFiles(projectDirectoryPath, "*.lift");
			if (liftPaths.Length == 0)
			{
				if (canNotify)
				{
					ErrorReport.NotifyUserOfProblem("Could not find a LIFT file to us in " + projectDirectoryPath);
				}
				return null;
			}
			if (liftPaths.Length == 1)
			{
				preferredLiftFile = liftPaths[0];
			}
			else
			{
				string parentDirectoryName = projectDirectoryPath.Split(new[] { Path.DirectorySeparatorChar }).LastOrDefault();
				preferredLiftFile = liftPaths.FirstOrDefault(
					fileName => String.Compare(
						Path.GetFileNameWithoutExtension(fileName),
						parentDirectoryName,
						StringComparison.OrdinalIgnoreCase
					) == 0
				);
				if (String.IsNullOrEmpty(preferredLiftFile))
				{
					var configFiles = Directory.GetFiles(projectDirectoryPath, "*.WeSayConfig");
					if (configFiles.Count() == 1)
					{
						preferredLiftFile = liftPaths.SingleOrDefault(lp =>
																	  Path.GetFileNameWithoutExtension(lp)
																		  .Equals(
																			  Path.GetFileNameWithoutExtension(
																				  configFiles.First())));
					}
				}
				if (String.IsNullOrEmpty(preferredLiftFile))
				{
					if (canNotify)
					{
						ErrorReport.NotifyUserOfProblem(
							"Expected only one LIFT file in {0}, but there were {1}. Wesay couldn't decide which one to use. Remove all but one and try again.",
							projectDirectoryPath,
							liftPaths.Length
						);
					}
					return null;
				}
			}
			return preferredLiftFile;
		}



		private static string GetFullPath(string liftPath)
		{
			if (!liftPath.Contains(Path.DirectorySeparatorChar.ToString()))
			{
				Logger.WriteEvent("Converting filename only liftPath {0} to full path {1}", liftPath, Path.GetFullPath(liftPath));
				liftPath = Path.GetFullPath(liftPath);
			}
			return liftPath;
		}

		private static bool FileIsUsable(string liftPath)
		{
			try
			{
				using (FileStream fs = File.OpenWrite(liftPath))
				{
					fs.Close();
				}
			}
			catch (UnauthorizedAccessException)
			{
				ErrorReport.NotifyUserOfProblem(
					String.Format(
						"WeSay was unable to open the file at '{0}' for writing, because the system won't allow it. Check that 'ReadOnly' is cleared, otherwise investigate your user permissions to write to this file.",
						liftPath));
				return false;
			}
			catch (IOException)
			{
				ErrorReport.NotifyUserOfProblem(
					String.Format(
						"WeSay was unable to open the file at '{0}' for writing, probably because it is locked by some other process on your computer. Maybe you need to quit WeSay? If you can't figure out what has it locked, restart your computer.",
						liftPath));
				return false;
			}
			return true;
		}
	}
}
