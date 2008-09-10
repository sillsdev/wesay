using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Mono.Addins;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;

namespace Addin.Transform
{
	[Extension]
	public class PdfMaker: HtmlTransformer
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
			get { return "EXPERIMENTAL: " + "Create a publication and open it in a PDF reader."; }
		}

		public override Image ButtonImage
		{
			get { return Resources.pdfDictionary; }
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

				string layoutCssPath =
						projectInfo.LocateFile(Path.Combine("Templates", "basicDictionary.css"));
				PrinceXmlWrapper.CreatePdf(htmlPath, new string[] {layoutCssPath}, pdfPath);
				Process.Start(pdfPath);
			}
			catch (Exception e)
			{
				ErrorReport.ReportNonFatalMessage(e.Message);
			}
		}
	}
}