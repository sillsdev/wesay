using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Addin.LiftReports.Properties;
using LiftReports;

using Mono.Addins;
using WeSay.AddinLib;

namespace Addin.LiftReports
{
	[Extension]
	public class ReportMaker : IWeSayAddin
	{
		 public Image ButtonImage
		{
			get
			{
				return Resources.image;
			}
		}

		public bool Available
		{
			get
			{
				return true;
			}
		}

		public string Name
		{
			get
			{
				return "Report Maker";
			}
		}

		public string ShortDescription
		{
			get
			{
				return "Shows some information about the lexicon.";
			}
		}

/*        public void Launch(string pathToTopLevelDirectory, string pathToLIFT)
		{
			string path = Path.Combine(Path.GetTempPath(), "LexiconStats.htm");
			using (StreamWriter stream = File.CreateText(path))
			{
				stream.Write("<html><body>");
				stream.WriteLine("<p>{0}</p>", pathToTopLevelDirectory);
				stream.WriteLine("<p>{0}</p>", pathToLIFT);
				stream.Write("</body></html>");
				stream.Close();
			}
			System.Diagnostics.Process.Start(path);

		}
*/
		public void Launch(string pathToTopLevelDirectory, string pathToLIFT)
		{
			Form f = new Form();
			f.Size = new Size(400, 400);
			f.SuspendLayout();
			Panel panel = new Panel();
			panel.BackColor = Color.White;
			panel.Dock = DockStyle.Fill;
			f.Controls.Add(panel);
			Report r = new Report();
			r.PathToLift = pathToLIFT;
			r.Size = new Size(400, 1000);
			panel.Controls.Add(r) ;
			f.ResumeLayout();
			panel.VerticalScroll.Value = 0;
			panel.AutoScroll = true;
			f.ShowDialog();

		}
	}
}
