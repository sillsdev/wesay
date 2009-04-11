using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Palaso.Reporting;

namespace Addin.Backup
{
	public class BackupMaker
	{
		/// <summary>
		/// Caller must catch exceptions this throws for ordinary conditions (like if a destination is full)
		/// </summary>
		/// <param name="sourceProjectPath"></param>
		/// <param name="destinationZipPath"></param>
		/// <param name="paths"></param>
		public static void BackupToExternal(string sourceProjectPath,
											string destinationZipPath,
											string[] paths)
		{
			if (!Directory.Exists(Path.GetPathRoot(sourceProjectPath)))
			{
				throw new ApplicationException("Directory of the project doesn't exist.");
			}

			if (!Directory.Exists(Path.GetPathRoot(destinationZipPath)))
			{
				throw new ApplicationException("Directory of the destination doesn't exist.");
			}

			int numFilesToBeWritten = paths.Length;

			Logger.WriteEvent("Start Backup to {0}", destinationZipPath);

			//the tricky part here is to get all the paths to be relative, starting with the
			//project root, rather than specifying them all the way up to the hard drive

			Directory.SetCurrentDirectory(Directory.GetParent(sourceProjectPath).FullName);
			ZipFile zipFile = ZipFile.Create(destinationZipPath);
			zipFile.BeginUpdate();

			string pathPriorToRootOfProject = Directory.GetParent(sourceProjectPath).FullName;
			foreach (string s in paths)
			{
				zipFile.Add(s.Replace(pathPriorToRootOfProject + Path.DirectorySeparatorChar, ""));
			}
			zipFile.CommitUpdate();
			//A particular case where this exception will be thrown is when we try to zip a file
			//that we cannot get readaccess to. This is the case for locking in WeSay 0.4 and was
			//the cause for WS-1205
			if(zipFile.Count != numFilesToBeWritten)
			{
				throw new ZipException("The number of files written to the archive is not equal to the number of files that were to be backed up! This may be because one of the files is being used by another program.");
			}
			zipFile.Close();

			Logger.WriteEvent("Backup Done");
		}
	}
}