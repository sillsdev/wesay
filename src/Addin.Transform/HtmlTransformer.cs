using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Xsl;
using Mono.Addins;
using WeSay.AddinLib;
using WeSay.Foundation;

namespace Addin.Transform
{
	[Extension]
	public class HtmlTransformer : LiftTransformer
	{
		public override string Name
		{
			get
			{
				return StringCatalog.Get("~Html Exporter");
			}
		}

		public override  string ShortDescription
		{
			get
			{
				return StringCatalog.Get("~Creates an Html version of the dictionary ready for printing.");
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
			XsltArgumentList arguments = new XsltArgumentList();
			//<xsl:param name="optionslist-writing-system" select="'en'"/>

			//<xsl:param name="headword-writing-system" select="/lift/entry/lexical-unit/form/@lang"/>

			//<xsl:param name="include-notes" select="false()"/>
			//<xsl:param name="group-by-grammatical-info" select="true()"/>
			arguments.AddParam("writing-system-info-file", string.Empty, projectInfo.LocateFile("writingSystemPrefs.xml"));
			arguments.AddParam("grammatical-info-optionslist-file", string.Empty, projectInfo.LocateFile("PartsOfSpeech.xml"));
			arguments.AddParam("output-intented-for-winword", string.Empty, intendedForWinWord.ToString()+"()");

			return TransformLift(projectInfo, "lift2html.xsl", ".htm",arguments);
		}
	}
}
