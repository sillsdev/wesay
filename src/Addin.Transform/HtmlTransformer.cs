using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Mono.Addins;
using WeSay.AddinLib;

namespace Addin.Transform
{
	[Extension]
	public class HtmlTransformer : LiftTransformer
	{
		public override string Name
		{
			get
			{
				return "Html Exporter";
			}
		}

		public override  string ShortDescription
		{
			get
			{
				return "Creates an Html version of the dictionary ready for printing.";
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
			string output = TransformLift(projectInfo, "lift2html.xsl", ".htm");

			Process.Start(output);
		}
	}
}
