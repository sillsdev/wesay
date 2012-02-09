using System;
using System.Collections.Generic;
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
using Palaso.WritingSystems;
using WeSay.AddinLib;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;

namespace Addin.Transform.PdfDictionary
{
	[Extension]
	public class PdfMaker : HtmlTransformer, IWeSayAddinHasMoreInfo
	{
		public override string LocalizedName
		{
			get { return StringCatalog.Get("~Make Pdf Dictionary"); }
		}

		public override string ID
		{
			get { return "PdfMaker"; }
		}

		public override string Description
		{
			get { return "Create a simple PDF, ready to print."; }
		}

		public override Image ButtonImage
		{
			get { return Resources.pdfDictionary; }
		}

		public override Image DashboardButtonImage
		{
			get { return Resources.greenPdfDictionary; }
		}

		public override bool Available
		{
			get
			{
				try
				{
					return PrinceXmlWrapper.IsPrinceInstalled;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public override void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			if (!Available)
				throw new ConfigurationException(
					"WeSay could not find PrinceXml.  Make sure you've installed it (get it from princexml.com).");

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

		private void OnDoWork(object sender, DoWorkEventArgs e)
		{
			ProgressState progressState = (ProgressState) e.Argument;

			ProjectInfo projectInfo = (ProjectInfo) progressState.Arguments;

			progressState.StatusLabel = "Converting dictionary to XHTML...";

			string htmlPath = CreateFileToOpen(projectInfo, false);

			if (string.IsNullOrEmpty(htmlPath))
			{
				return; // get this when the user cancels
			}
			try
			{
				string pdfPath = Path.Combine(projectInfo.PathToExportDirectory,
											  projectInfo.Name + ".pdf");

				var stylesheetPaths = new List<string>();

				var autoLayout = Path.Combine(projectInfo.PathToExportDirectory, "autoLayout.css");
				var factoryLayout = projectInfo.LocateFile(Path.Combine("templates", "defaultDictionary.css"));
				File.Copy(factoryLayout, autoLayout, true);

				var autoFonts = Path.Combine(projectInfo.PathToExportDirectory, "autoFonts.css");
				CreateAutoFontsStyleSheet(autoFonts,
										  (PublicationFontStyleProvider)
										  projectInfo.ServiceProvider.GetService(
											  typeof (PublicationFontStyleProvider)), projectInfo.WritingSystems);

				var customLayout = Path.Combine(projectInfo.PathToExportDirectory, "customLayout.css");
				if (!File.Exists(customLayout))
				{
					File.WriteAllText(customLayout,
									  "/* To tweak a layout setting, copy the template you want to change from the autoLayout.css into this file, and make your changes */");
				}

				var customFonts = Path.Combine(projectInfo.PathToExportDirectory, "customFonts.css");
				if (!File.Exists(customFonts))
				{
					File.WriteAllText(customFonts,
									  "/* To tweak a font setting, copy the template you want to change from the autoFonts.css into this file, and make your changes.*/");
				}

				//NB: experiments with princexml 6.0 showed that the last guy wins.
				///beware... it's actually not totally clear what precendence we even want between layout and font! There's an interplay
				/// between what is specified in the base css... if, for example, they specify a font family,
				/// well then we want to override that with custom fonts.  But if they just want, say, to bold
				/// something, that would be fine to override
				stylesheetPaths.Add(autoFonts);
				stylesheetPaths.Add(autoLayout);
				stylesheetPaths.Add(customFonts);
				stylesheetPaths.Add(customLayout);

				progressState.StatusLabel = "Converting XHTML to PDF...";

				PrinceXmlWrapper.CreatePdf(htmlPath, stylesheetPaths, pdfPath);

				if (_launchAfterTransform)
				{
					progressState.StatusLabel = "Opening PDF...";
					Process.Start(pdfPath);
				}
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem(error.Message);
			}
		}


		private void CreateAutoFontsStyleSheet(string path, PublicationFontStyleProvider styleProvider, IWritingSystemRepository writingSystemCollection)
		{

			using (var f = File.CreateText(path))
			{
				foreach (var writingSystem in writingSystemCollection.AllWritingSystems)
				{
					f.WriteLine(":lang("+writingSystem.Id+") {");
					f.WriteLine(styleProvider.GetAutoFontsCascadingStyleSheetLinesForWritingSystem(writingSystem));
					f.WriteLine("}");
					f.WriteLine();
				}
			}
		}

		public void ShowMoreInfoDialog(Form parentForm)
		{
			var dlg = new AboutMakePdfDictionary();
			dlg.ShowDialog();
		}
	}
}
