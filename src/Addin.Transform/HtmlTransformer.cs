using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Xsl;
using Mono.Addins;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;

namespace Addin.Transform
{
	[Extension]
	public class HtmlTransformer: LiftTransformer
	{
		public override string LocalizedName
		{
			get { return StringCatalog.Get("~Export to HTML"); }
		}

		public override string LocalizedLabel
		{
			get { return LocalizedName; }
		}

		public override string LocalizedLongLabel
		{
			get { return LocalizedName; }
		}

		public override string Description
		{
			get
			{
				return
						StringCatalog.Get(
								"~Creates a simple Html version of the dictionary.  Not a very good way to go.");
			}
		}

		public override Image ButtonImage
		{
			get { return Resources.printButtonImage; }
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
			string output = CreateFileToOpen(projectInfo, true, true);
			if (string.IsNullOrEmpty(output))
			{
				return; // get this when the user cancels
			}
			if (_launchAfterTransform)
			{
				Process.Start(output);
			}
		}

		protected string CreateFileToOpen(ProjectInfo projectInfo,
										  bool includeXmlDirective,
										  bool linkToUserCss)
		{
			//the problem we're addressing here is that when this is launched from the wesay configuration
			//that won't (and doesn't want to) have locked up the db4o db by making a record list manager,
			//which it normally has no need for.
			//So if we're in that situation, we temporarily try to make one and then release it,
			//so it isn't locked when the user says "open wesay"

			LexEntryRepository lexEntryRepository = projectInfo.ServiceProvider.GetService(typeof(LexEntryRepository)) as LexEntryRepository;
		  //  using(lexEntryRepository.GetRightToAccessLiftExternally())
			{
				string pliftPath;
				using (LameProgressDialog dlg = new LameProgressDialog("Exporting to PLift..."))
				{
					dlg.Show();
					PLiftMaker maker = new PLiftMaker();
					pliftPath = maker.MakePLiftTempFile(lexEntryRepository, projectInfo.ServiceProvider.GetService(typeof(ViewTemplate)) as ViewTemplate);
				}

				projectInfo.PathToLIFT = pliftPath;

				XsltArgumentList arguments = new XsltArgumentList();
				arguments.AddParam("writing-system-info-file",
								   string.Empty,
								   projectInfo.LocateFile("WritingSystemPrefs.xml"));
				arguments.AddParam("grammatical-info-optionslist-file",
								   string.Empty,
								   projectInfo.LocateFile("PartsOfSpeech.xml"));
				arguments.AddParam("link-to-usercss", string.Empty, linkToUserCss + "()");

				return TransformLift(projectInfo,
									 "plift2html.xsl",
									 ".htm",
									 arguments,
									 //word doesn't notice that is is html if the <xml> directive is in there
									 includeXmlDirective);
			}
		}
	}
}