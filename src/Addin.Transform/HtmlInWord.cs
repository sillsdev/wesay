using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Mono.Addins;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;
using WeSay.Foundation;

namespace Addin.Transform
{
	[Extension]
	public class HtmlInWord : HtmlTransformer
	{
		public override string LocalizedName
		{
			get
			{
				return StringCatalog.Get("~Export to Word");
			}
		}

		public override string ID
		{
			get { return "ExportToWord"; }
		}

		public override string ShortDescription
		{
			get
			{
				return StringCatalog.Get("~Creates a very basic dictionary publication in Microsoft Word.");
			}
		}


		public override Image ButtonImage
		{
			get
			{
				return Resources.wordExport;
			}
		}

		public override void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			string output = CreateFileToOpen(projectInfo, false, false);
			if(string.IsNullOrEmpty(output))
			{
				return; // get this when the user cancels
			}
			if (_launchAfterTransform)
			{
				try
				{
					ProcessStartInfo si = new ProcessStartInfo();
					si.FileName = "WinWord.exe";
					//this flag f makes word create a new, untitled file
					//enhance: so, it would be nice to create this in temp (and deleting it later?)
					si.Arguments = "/f \"" + output+"\"";
					Process.Start(si);
				}
				catch (Exception e)
				{
					Palaso.Reporting.ErrorReport.ReportNonFatalMessage(e.Message);
				}
			}
		}
	}
}
