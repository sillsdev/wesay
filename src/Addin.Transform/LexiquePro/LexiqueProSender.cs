using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using Mono.Addins;
using Palaso.DictionaryServices.Lift;
using Palaso.i18n;
using WeSay.AddinLib;
using WeSay.LexicalModel;
using WeSay.Project;

namespace Addin.Transform.LexiquePro
{
	[Extension]
	public class LexiqueProSender : LiftTransformer, IWeSayAddinHasMoreInfo
	{
		public override string LocalizedName
		{
			get { return StringCatalog.Get("~Open in Lexique Pro"); }
		}

		public override bool Available
		{
			get { return Environment.OSVersion.Platform != PlatformID.Unix &&
						 !string.IsNullOrEmpty(GetPathToLexiquePro()); }
		}

		public override string LocalizedLabel
		{
			get { return LocalizedName; }
		}

		public override string LocalizedLongLabel
		{
			get { return LocalizedName; }
		}

		public override string Description
		{
			get
			{
				return
					StringCatalog.Get(
						"~Open dictionary in Lexique Pro for printing or web-page creation. Requires version LP ver 3.0 or higher.");
			}
		}

		public override Image ButtonImage
		{
			get { return Resources.lexiquePro; }
		}

		public override Image DashboardButtonImage
		{
			get { return Resources.greenLexiquePro; }
		}

		public override string ID
		{
			get { return "OpenWithLexiquePro"; }
		}

		public override void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			string pliftPath = CreatePLift(projectInfo, true, true);
			if (string.IsNullOrEmpty(pliftPath))
			{
				return; // get this when the user cancels
			}

			try
			{
				var startInfo= new ProcessStartInfo(GetPathToLexiquePro(), "/f " + "\"" + pliftPath + "\"");
				startInfo.WorkingDirectory = Path.GetDirectoryName(GetPathToLexiquePro());
				Process.Start(startInfo);
			}
			catch (Exception error)
			{
				Palaso.Reporting.ErrorReport.NotifyUserOfProblem("Sorry, could not open dictionary in Lexique Pro.\r\n" + error.Message);
			}
		}

		public string GetPathToLexiquePro()
		{
			try
			{
				var key = Registry.ClassesRoot.OpenSubKey(@"Applications\LexiquePro.exe\shell\open\command");
				if (key == null)
				{
					return null;
				}
				var cmd = key.GetValue("") as string;
				if (cmd != null)
				{
					cmd = cmd.Replace("/f", "");
					cmd = cmd.Replace("\"%1\"", "");
					cmd = cmd.Replace("\"", "");
				}

				return cmd;
			}
			catch (Exception error)
			{
				Palaso.Reporting.ErrorReport.NotifyUserOfProblem("WeSay encountered a problem while looking for Lexique Pro.\r\n" + error.Message);
			}
			return null;
		}

		protected string CreatePLift(ProjectInfo projectInfo,
									 bool includeXmlDirective,
									 bool linkToUserCss)
		{
			LexEntryRepository lexEntryRepository =
				projectInfo.ServiceProvider.GetService(typeof (LexEntryRepository)) as LexEntryRepository;
			{
				//In Oct 2008, LP didn't understand "plift" yet.
				var pliftPath = Path.Combine(projectInfo.PathToExportDirectory, projectInfo.Name + "-plift.lift");
				using (var dlg = new LameProgressDialog("Exporting to PLift..."))
				{
					dlg.Show();
					var maker = new PLiftMaker();
					maker.MakePLiftTempFile(pliftPath, lexEntryRepository,
														projectInfo.ServiceProvider.GetService(typeof (ViewTemplate)) as
														ViewTemplate,
														LiftWriter.ByteOrderStyle.BOM);
				}
				return pliftPath;
			}
		}

		public void ShowMoreInfoDialog(Form parentForm)
		{
			var dlg = new AboutLexiquePro();
			dlg.ShowDialog();
		}


	}
}