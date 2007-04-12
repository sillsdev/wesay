
using System.IO;
using System.Windows.Forms;
using WeSay.Project;

namespace WeSay.App
{
	/// <summary>
	/// this was just a prototype... needs thinking and work
	/// </summary>
	public class BackupService
	{
		public static void BackupToExternal(string destinationZipPath)
		{
			if (!Directory.Exists(Path.GetPathRoot(destinationZipPath)))
				return;

			Reporting.Logger.WriteEvent("Incremental Backup Start");

			LameProgressDialog dlg = new LameProgressDialog("Backing up to external...");
			dlg.Show();

			Application.DoEvents();
			try
			{
				ICSharpCode.SharpZipLib.Zip.FastZip f = new ICSharpCode.SharpZipLib.Zip.FastZip();
				f.CreateZip(destinationZipPath, WeSayWordsProject.Project.ProjectDirectoryPath, true, null);
			}
			finally
			{
				if (dlg != null)
				{
					dlg.Close();
					dlg.Dispose();
				}
			}

			Reporting.Logger.WriteEvent("Incremental Backup Done");

		}
	}
}
