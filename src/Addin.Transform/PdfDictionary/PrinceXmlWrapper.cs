using System;
using System.Collections.Generic;
using System.IO;

namespace Addin.Transform.PdfDictionary
{
	internal class PrinceXmlWrapper
	{
		public static bool IsPrinceInstalled
		{
			get
			{
				Prince p = new Prince();
				if (p == null)
				{
					return false;
				}

				return File.Exists(GetPrincePath());
			}
		}

		public static bool CreatePdf(string htmlPath,
									 IEnumerable<string> styleSheetPaths,
									 string pdfPath)
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
				p = new Prince(); //maybe it would look in %path%?
			}
		   // no: this makes princexml think we're putting out ascii:  p.SetHTML(true);
			p.SetLog(Path.GetTempFileName());
			foreach (string styleSheetPath in styleSheetPaths)
			{
				p.AddStyleSheet(styleSheetPath);
			}
			return p.Convert(htmlPath, pdfPath);
		}

		private static string GetPrincePath()
		{
			string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

			string princePath = AppendPrincePath(programFilesPath);
			if (File.Exists(princePath))
			{
				return princePath;
			}
			// else clause is for 32-bit prince on 64 bit OS
			else
			{
				programFilesPath = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
				return AppendPrincePath(programFilesPath);
			}
		}

		private static string AppendPrincePath(string princePath)
		{
			princePath = Path.Combine(princePath, "prince");
			princePath = Path.Combine(princePath, "engine");
			princePath = Path.Combine(princePath, "bin");
			princePath = Path.Combine(princePath, "prince.exe");
			return princePath;
		}
	}
}