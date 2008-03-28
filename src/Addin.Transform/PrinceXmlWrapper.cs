using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Addin.Transform
{
	internal class PrinceXmlWrapper
	{
		public static bool IsPrinceInstalled
		{
			get
			{
				Prince p = new Prince();
				if (p == null)
					return false;

				return File.Exists(GetPrincePath());
			}
		}
		public static bool CreatePdf(string htmlPath, IEnumerable<string> styleSheetPaths, string pdfPath)
		{
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				return false;
			}
			string princePath = GetPrincePath();
			Prince p;
			if (File.Exists(princePath))
			{
				p = new Prince(princePath);
			}
			else
			{
				p = new Prince();  //maybe it would look in %path%?
			}
			p.SetHTML(true);
			foreach (string styleSheetPath in styleSheetPaths)
			{
				p.AddStyleSheet(styleSheetPath);
			}
			return p.Convert(htmlPath, pdfPath);
		}

		private static string GetPrincePath()
		{
			string princePath = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			princePath = Path.Combine(princePath, "prince");
			princePath = Path.Combine(princePath, "engine");
			princePath = Path.Combine(princePath, "bin");
			princePath = Path.Combine(princePath, "prince.exe");
			return princePath;
		}
	}
}
