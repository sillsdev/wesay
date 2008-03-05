using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Xsl;
using Mono.Addins;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;

namespace Addin.Transform
{
	[Extension]
	public class HtmlTransformer : LiftTransformer
	{
		public override string Name
		{
			get
			{
				return StringCatalog.Get("~Export to HTML");
			}
		}

		public override  string ShortDescription
		{
			get
			{
				return StringCatalog.Get("~Creates a simple Html version of the dictionary, ready for printing.");
			}
		}


		public override Image ButtonImage
		{
			get
			{
				return Resources.printButtonImage;
			}
		}


		public override void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			string output = CreateFileToOpen(projectInfo, false);
			if (string.IsNullOrEmpty(output))
			{
				return; // get this when the user cancels
			}
			if (_launchAfterTransform)
			{
				Process.Start(output);
			}
		}

		protected string CreateFileToOpen(ProjectInfo projectInfo, bool intendedForWinWord)
		{
			Lexicon.Init((Db4oRecordListManager) projectInfo.RecordListManager);
			PLiftMaker maker = new PLiftMaker();
			string pliftPath = Path.Combine(projectInfo.PathToExportDirectory, projectInfo.Name+ ".html");
			maker.MakeXHtmlFile(pliftPath, (Db4oRecordListManager) projectInfo.RecordListManager, (WeSayWordsProject) projectInfo.Project);

			projectInfo.PathToLIFT = pliftPath;

			XsltArgumentList arguments = new XsltArgumentList();
			 arguments.AddParam("writing-system-info-file", string.Empty, projectInfo.LocateFile("writingSystemPrefs.xml"));
			arguments.AddParam("grammatical-info-optionslist-file", string.Empty, projectInfo.LocateFile("PartsOfSpeech.xml"));
			arguments.AddParam("output-intented-for-winword", string.Empty, intendedForWinWord.ToString()+"()");

			return TransformLift(projectInfo, "lift2html.xsl", ".htm",arguments);
		}
	}
}
