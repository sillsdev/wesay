using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Mono.Addins;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i18n;
using WeSay.AddinLib;

namespace Addin.Transform
{
#if REMOVED // after several months, we can really remove this file
	[Extension]
	public class HtmlInWord: HtmlTransformer
	{
		public override string LocalizedName
		{
			get { return StringCatalog.Get("~Export to Word"); }
		}

		public override string ID
		{
			get { return "ExportToWord"; }
		}

		public override string Description
		{
			get
			{
				return
						StringCatalog.Get(
								"~Creates a very basic dictionary publication in Microsoft Word.");
			}
		}

		public override Image ButtonImage
		{
			get { return Resources.wordExport; }
		}

		public override void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			string output = CreateFileToOpen(projectInfo, false, false);
			if (string.IsNullOrEmpty(output))
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
					si.Arguments = "/f \"" + output + "\"";
					Process.Start(si);
				}
				catch (Exception e)
				{
					ErrorReport.NotifyUserOfProblem(e.Message);
				}
			}
		}
	}
#endif
}