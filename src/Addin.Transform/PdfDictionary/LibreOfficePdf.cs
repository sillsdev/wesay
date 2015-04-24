using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Mono.Addins;
using SIL.IO;
using SIL.i18n;
using SIL.Progress;
using SIL.Reporting;
using SIL.Windows.Forms.Progress;
using WeSay.AddinLib;
using Addin.Transform.OpenOffice;
using WeSay.Foundation;

namespace Addin.Transform.PdfDictionary
{
	[Extension]
	public class LibreOfficePdf : IWeSayAddin //, IWeSayAddinHasMoreInfo
	{
		private Boolean _launchAfterExport = true;
		private string _odtFile;
		int ProductMajor;
		enum availStatus
		{
			NotChecked,
			NotInstalled,
			Available
		}
		private availStatus _isAvailable = availStatus.NotChecked;

		public string LocalizedName
		{
			get { return StringCatalog.Get("~Make Pdf Dictionary"); }
		}

		public string LocalizedLabel
		{
			get { return LocalizedName; }
		}

		public string LocalizedLongLabel
		{
			get { return LocalizedName; }
		}

		public string OdtFile
		{
			get { return _odtFile; }
			set { _odtFile = value; }
		}

		public string ID
		{
			get { return "LibreOfficePdf"; }
		}

		public string Description
		{
			get { return "Create a PDF, ready to print."; }
		}

		public Image ButtonImage
		{
			get { return Resources.pdfDictionary; }
		}

		public Image DashboardButtonImage
		{
			get { return Resources.greenPdfDictionary; }
		}

		public ButtonStyle DashboardButtonStyle
		{
			get { return ButtonStyle.IconVariableWidth; }
		}

		public DashboardGroup Group
		{
			get { return DashboardGroup.Share; }
		}

		public bool Available
		{
			get
			{
				if (_isAvailable == availStatus.Available)
					return true;
				if (_isAvailable == availStatus.NotInstalled)
					return false;
				try
				{
					bool retval = false;
					ProductMajor = 0;
#if __MonoCS__
					// find which libreoffice
					// check versionrc file exists ../lib/libreoffice/program/versionrc
					// open versionrc file for reading
					// look at each line and split at '='

					// DG - Apr2012 libreoffice 3.5 changed the format so now checking ProductMajor
					// look for words[0] = ProductMajor
					// version number is words[1], check is >= 340

					Process loffice = new Process();
					loffice.StartInfo.Arguments = "libreoffice";
					loffice.StartInfo.RedirectStandardOutput = true;
					loffice.StartInfo.UseShellExecute = false;
					loffice.StartInfo.FileName = "which";
					loffice.Start();
					string output = loffice.StandardOutput.ReadLine();
					loffice.WaitForExit();
					int ecode = loffice.ExitCode;
					if (ecode==0 && !String.IsNullOrEmpty(output))
					{
						string binpath = Path.GetDirectoryName(output);
						string installedpath = Path.GetDirectoryName(binpath);
						string rcpath = Path.Combine (installedpath, "lib");
						rcpath = Path.Combine (rcpath, "libreoffice");
						rcpath = Path.Combine (rcpath, "program");
						rcpath = Path.Combine (rcpath, "versionrc");
						if (File.Exists(rcpath))
						{
							StreamReader rcfile = File.OpenText(rcpath);
							string rcline;
							rcline = rcfile.ReadLine();
							while (!String.IsNullOrEmpty(rcline))
							{
								string[] words = rcline.Split('=');
								if (words[0] == "ProductMajor")
								{
									ProductMajor = Convert.ToInt32(words[1]);
									int minver = 340;
									if (ProductMajor >= minver)
									{
										_isAvailable = availStatus.Available;
										retval = true;
									}
									break;
								}

								rcline = rcfile.ReadLine();
							}

						}
					}
#endif
					if (!retval)
					{
						_isAvailable = availStatus.NotInstalled;
					}

					return retval;
				}
				catch (Exception)
				{
					_isAvailable = availStatus.NotInstalled;
					return false;
				}
			}
		}

		private string UserConfigDir
		{
// Note that libreoffice 3.5.0 and newer use homedir/.config/libreoffice
// so will need to update this file to handle that DG: 02/2012
			get {
				if (!Available) return null;

				string ucdir;
				if (ProductMajor < 350)
				{
					string homedir = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					ucdir = Path.Combine(homedir, ".libreoffice");
				}
				else
				{
					string homedir = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
					ucdir = Path.Combine (homedir, "libreoffice");
				}
				return ucdir;
			}
		}

		private string UserCliConfigDir
		{
			get { return UserConfigDir + "-cli"; }
		}

		private bool CheckUserCliConfigDir()
		{
			if (!Available)
			{
				return false;
			}
			if (!Directory.Exists(UserCliConfigDir))
			{
				Directory.CreateDirectory(UserCliConfigDir);
			}
			if (!Directory.Exists(UserCliConfigDir))
			{
				return false;
			}
			return true;
		}

		public void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			if (!Available)
				throw new ConfigurationException(
					"WeSay could not find LibreOffice 3.4 or newer.  Make sure you have installed it.");
			if (!CheckUserCliConfigDir())
				throw new ConfigurationException(
					"WeSay could not configure LibreOffice 3.4 or newer.  Make sure you have installed it.");


			OpenOfficeAddin odtAddin = new OpenOfficeAddin();

			OdtFile = Path.Combine(projectInfo.PathToExportDirectory, projectInfo.Name + ".odt");
			odtAddin.LaunchAfterExport = false;
			odtAddin.Launch(parentForm, projectInfo);

			bool succeeded = ((File.Exists(OdtFile)) && (new FileInfo(OdtFile).Length > 0));

			if (succeeded)
			{
			using (ProgressDialog dlg = new ProgressDialog())
				{
				dlg.BarStyle = ProgressBarStyle.Marquee; //we have no idea how much progress we've made

				dlg.Overview = "Creating PDF Dictionary";
				BackgroundWorker worker = new BackgroundWorker();
				worker.DoWork += OnDoWork;
				dlg.BackgroundWorker = worker;
				dlg.CanCancel = true;
				dlg.ProgressState.Arguments = projectInfo;

				dlg.ShowDialog();
				if (dlg.ProgressStateResult != null &&
					dlg.ProgressStateResult.ExceptionThatWasEncountered != null)
					{
					ErrorReport.ReportNonFatalException(
						dlg.ProgressStateResult.ExceptionThatWasEncountered);
					}
				}
			}
		}

		public bool Deprecated
		{
			get { return false; }
		}

		private void OnDoWork(object sender, DoWorkEventArgs e)
		{
			ProgressState progressState = (ProgressState) e.Argument;

			ProjectInfo projectInfo = (ProjectInfo) progressState.Arguments;

			progressState.StatusLabel = "Converting document to PDF...";

			try
			{
				string pdfPath = Path.Combine(projectInfo.PathToExportDirectory,
											  projectInfo.Name + ".pdf");

				//string loexec = "libreoffice --headless --convert-to pdf -outdir " +
				//	projectInfo.PathToExportDirectory + " " + OdtFile;

				Process loffice = new Process();
				loffice.StartInfo.WorkingDirectory = projectInfo.PathToExportDirectory;
				loffice.StartInfo.Arguments = "--invisible -env:UserInstallation=file://" +
					UserCliConfigDir + " --convert-to pdf \"" + OdtFile + "\"";
				loffice.StartInfo.UseShellExecute = true;
				loffice.StartInfo.FileName = "libreoffice";
				loffice.Start();
				loffice.WaitForExit();
				int ecode = loffice.ExitCode;
				if (ecode != 0)
					throw new ApplicationException("LibreOffice did not convert the file to Pdf: " + ecode.ToString());

				if (_launchAfterExport)
				{
					progressState.StatusLabel = "Opening PDF...";
					Process runpdf = new Process();
					runpdf.StartInfo.FileName = pdfPath;
					runpdf.Start();
				}
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem(error.Message);
			}
		}

		/* public void ShowMoreInfoDialog(Form parentForm)
		{
			var dlg = new Addin.Transform.PdfDictionary.AboutMakePdfDictionary();
			dlg.ShowDialog();
		}*/

		public bool LaunchAfterExport
		{
			set { _launchAfterExport = value; }
		}

	}
}
