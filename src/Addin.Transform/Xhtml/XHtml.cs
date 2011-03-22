using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Mono.Addins;
using Palaso.i18n;
using Palaso.Reporting;
using WeSay.AddinLib;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;

namespace Addin.Transform.Xhtml
{
	[Extension]
	public class Xhtml : HtmlTransformer//, IWeSayAddinHasMoreInfo
	{
		public override string LocalizedName
		{
			get { return StringCatalog.Get("~Export to XHTML"); }
		}

		public override string ID
		{
			get { return "XHTML"; }
		}

		public override string Description
		{
			get { return "Create a simple HTML file."; }
		}

		public override Image ButtonImage
		{
			get { return Resources.xhtml; }
		}

		public override Image DashboardButtonImage
		{
			get { return Resources.xhtml; }
		}

		public override bool Available
		{
			get
			{
				return true;
			}
		}

		public override void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			// create stylesheets in export directory
			var autoLayout = Path.Combine(projectInfo.PathToExportDirectory, "autoLayout.css");
			var factoryLayout = projectInfo.LocateFile(Path.Combine("templates", "defaultDictionary.css"));
			File.Copy(factoryLayout, autoLayout, true);

			string autoFonts = Path.Combine(projectInfo.PathToExportDirectory, "autoFonts.css");
			CreateAutoFontsStyleSheet(autoFonts, (PublicationFontStyleProvider)projectInfo.ServiceProvider.GetService(typeof(PublicationFontStyleProvider)), projectInfo.WritingSystems);

			// export to html
			string htmlPath = CreateFileToOpen(projectInfo, true, true);
			if (string.IsNullOrEmpty(htmlPath))
			{
				return; // get this when the user cancels
			}
			try
			{
				Process.Start(htmlPath);
			}
			catch (Exception e)
			{
				ErrorReport.NotifyUserOfProblem(e.Message);
			}
		}

		private void CreateAutoFontsStyleSheet(string path, PublicationFontStyleProvider styleProvider, WritingSystemCollection writingSystemCollection)
		{

			using (var f = File.CreateText(path))
			{
				foreach (var writingSystem in writingSystemCollection)
				{
					f.WriteLine(":lang("+writingSystem.Id+") {");
					f.WriteLine(styleProvider.GetAutoFontsCascadingStyleSheetLinesForWritingSystem(writingSystem));
					f.WriteLine("}");
					f.WriteLine();
				}
			}
		}
/*
		public void ShowMoreInfoDialog(Form parentForm)
		{
			//var dlg = new AboutMakePdfDictionary();
			//dlg.ShowDialog();
		}
		*/
	}
}