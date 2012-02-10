using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Mono.Addins;
using Palaso.i18n;
using Palaso.Progress;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.Progress;
using WeSay.AddinLib;
using Addin.Transform.OpenOffice;
using WeSay.Foundation;

namespace Addin.Transform.PdfDictionary
{
	[Extension]
	public class LibreOfficePdf : IWeSayAddin, IWeSayAddinHasMoreInfo
	{
		private Boolean _launchAfterExport = true;
		private string _odtFile;

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
				try
				{
#if MONO
					Process loffice = new Process();
					loffice.StartInfo.Arguments = "-headless -h";//|head -1";//|cut -d\\  -f2";
					loffice.StartInfo.RedirectStandardOutput = true;
					loffice.StartInfo.UseShellExecute = false;
					loffice.StartInfo.FileName = "libreoffice";
					loffice.Start();
					string output = loffice.StandardOutput.ReadLine();
					loffice.WaitForExit();
					int ecode = loffice.ExitCode;
					if (ecode==0 && !String.IsNullOrEmpty(output))
					{
						string[] words = output.Split(' ');
						decimal loversion = Convert.ToDecimal(words[1]);
						decimal minver = new decimal( 3.4 );
						if (loversion.CompareTo(minver) >= 0)
							return true;
					}
#endif
					return false;
				}
				catch (Exception error)
				{
					return false;
				}
			}
		}

		public void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			if (!Available)
				throw new ConfigurationException(
					"WeSay could not find LibreOffice 3.4 or newer.  Make sure you have installed it.");

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
				//dlg.CancelRequested += new EventHandler(OnCancelRequested);
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
				loffice.StartInfo.Arguments = "--headless --convert-to pdf " + OdtFile;
				loffice.StartInfo.UseShellExecute = true;
				loffice.StartInfo.FileName = "libreoffice";
				loffice.Start();
				loffice.WaitForExit();

				if (_launchAfterExport)
				{
					progressState.StatusLabel = "Opening PDF...";
					Process runpdf = new Process();
					runpdf.StartInfo.FileName = pdfPath;
					runpdf.Start();
					//Process.Start(pdfPath);
				}
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem(error.Message);
			}
		}

		public void ShowMoreInfoDialog(Form parentForm)
		{
			var dlg = new Addin.Transform.PdfDictionary.AboutMakePdfDictionary();
			dlg.ShowDialog();
		}

		public bool LaunchAfterExport
		{
			set { _launchAfterExport = value; }
		}

	}
}
