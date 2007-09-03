using System.IO;
using Addin.LiftReports.Properties;
using NVelocity;
using NVelocity.App;

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
					string templatePath = Path.Combine(Path.GetTempPath(), "tempReportTemplate.vm");
					File.WriteAllText(templatePath, Resources.reportTemplate);

					//hack to get around weird Velocity problem; if I give it the
					//full path into temp, it chokes saying it can't handle the path format
					string oldCurrentDir = Directory.GetCurrentDirectory();
					Directory.SetCurrentDirectory(Path.GetTempPath());
					Template template = Velocity.GetTemplate(Path.GetFileName(templatePath));
					Directory.SetCurrentDirectory(oldCurrentDir);
					template.Merge(context, stream);
					File.Delete(templatePath);
				}
				return path;
			}
			catch (System.Exception e)
			{
				Palaso.Reporting.ErrorReport.ReportNonFatalMessage("Problem creating report : " + e);
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
