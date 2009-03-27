using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Mono.Addins;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;
using WeSay.Foundation;

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
			string htmlPath = CreateFileToOpen(projectInfo, true, false);
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
				var factoryLayout = projectInfo.LocateFile(Path.Combine("Templates", "defaultDictionary.css"));
				File.Copy(factoryLayout, autoLayout, true);

				var autoFonts = Path.Combine(projectInfo.PathToExportDirectory, "autoFonts.css");
				CreateAutoFontsStyleSheet(autoFonts, projectInfo.WritingSystems);

				var customLayout = Path.Combine(projectInfo.PathToExportDirectory, "customLayout.css");
				if (!File.Exists(customLayout))
				{
					File.WriteAllText(customLayout, "/* To tweak a layout setting, copy the template you want to change from the autoLayout.css into this file, and make your changes */");
				}

				var customFonts = Path.Combine(projectInfo.PathToExportDirectory, "customFonts.css");
				if (!File.Exists(customFonts))
				{
					File.WriteAllText(customFonts, "/* To tweak a font setting, copy the template you want to change from the autoFonts.css into this file, and make your changes.*/" );
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

				PrinceXmlWrapper.CreatePdf(htmlPath, stylesheetPaths, pdfPath);
				Process.Start(pdfPath);
			}
			catch (Exception e)
			{
				ErrorReport.ReportNonFatalMessage(e.Message);
			}
		}

		private void CreateAutoFontsStyleSheet(string path, WritingSystemCollection writingSystemCollection)
		{
			using (var f = File.CreateText(path))
			{
				foreach (var pair in writingSystemCollection)
				{
					f.WriteLine(":lang("+pair.Key+") {");
					f.WriteLine("font-family: '"+pair.Value.FontName+"';");
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