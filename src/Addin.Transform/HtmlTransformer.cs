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
		public override string LocalizedName
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

		public override Image DashboardButtonImage
		{
			get { return Resources.greenPrinter; }
		}

		public override string ID
		{
			get { return "ExportToHtml"; }
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
			//the problem we're addressing here is that when this is launched from the wesay configuration
			//that won't (and doesn't want to) have locked up the db4o db by making a record list manager,
			//which it normally has no need for.
			//So if we're in that situation, we temporarily try to make one and then release it,
			//so it isn't locked when the user says "open wesay"

			Db4oRecordListManager manager = null;
			if (Lexicon.RecordListManager == null)
			{
				manager = (Db4oRecordListManager) ((WeSayWordsProject) projectInfo.Project).MakeRecordListManager();
			}
		   try
		   {

			   PLiftMaker maker = new PLiftMaker();
			   string pliftPath =
				   maker.MakePLiftTempFile(Lexicon.RecordListManager,
										   (WeSayWordsProject) projectInfo.Project);

			   projectInfo.PathToLIFT = pliftPath;

			   XsltArgumentList arguments = new XsltArgumentList();
			   arguments.AddParam("writing-system-info-file", string.Empty,
								  projectInfo.LocateFile("writingSystemPrefs.xml"));
			   arguments.AddParam("grammatical-info-optionslist-file", string.Empty,
								  projectInfo.LocateFile("PartsOfSpeech.xml"));
			   arguments.AddParam("output-intented-for-winword", string.Empty, intendedForWinWord.ToString() + "()");

			   return TransformLift(projectInfo, "plift2html.xsl", ".htm", arguments, true);
		   }
			finally
		   {
			   if (manager != null)
			   {
				   Lexicon.DeInitialize(true);
			   }
		   }
		}
	}
}
