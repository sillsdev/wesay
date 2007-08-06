using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Addin.LiftReports.Properties;
using Commons.Collections;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;

namespace Addin.LiftReports
{
	public class HtmlReport
	{
		public HtmlReport()
		{

		}

		public string GenerateReport(string pathToLIFT)
		{
			Velocity.Init();
			NVelocity.VelocityContext context = new NVelocity.VelocityContext();
			context.Put("pathToLift", pathToLIFT);
			XPathChart chart = new XPathChart();
			context.Put("chart", chart);
			chart.PathToXmlDocument = pathToLIFT;

			try
			{
				string dir = Path.Combine(Path.GetTempPath(), "WeSayReport");
				if (Directory.Exists(dir))
				{
					Directory.Delete(dir, true);
				}
				Directory.CreateDirectory(dir);
				string path = Path.Combine(dir, "WeSayReport.htm");
			   // File.Copy("chart.png", Path.Combine(dir, "chart.png"));

				context.Put("foo", new Foo(dir));

				using (StreamWriter stream = File.CreateText(path))
				{
					/*I couldn't get nvelocity to read directly from the resource*/
					File.WriteAllText("tempReportTemplate.vm", Resources.reportTemplate );
					Template template = Velocity.GetTemplate("tempReportTemplate.vm");
					template.Merge(context, stream);
				}
				return path;
			}
			catch (System.Exception e)
			{
				Reporting.ErrorReport.ReportNonFatalMessage("Problem creating report : " + e);
			}
			return null;
		}
	}

	public class Foo
	{
		protected string _dir;
		public Foo(string dir)
		{
			_dir = dir;
		}

		public string GetChart(string b)
		{
			string s = Path.Combine(_dir, "chart.png");
			File.Copy("chart.png", s);
			return s;
		}

		public string Name
		{
			get
			{
				return "FOOOOO";
			}
		}
	}

}
