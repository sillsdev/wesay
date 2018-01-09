using System;
using System.Collections.Generic;
using System.IO;
using SIL.IO;

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
			bool retval = true;
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
			using (var log = new TempFile())
			{
				p.SetLog(log.Path);
			foreach (string styleSheetPath in styleSheetPaths)
			{
				p.AddStyleSheet(styleSheetPath);
			}
				if(!p.Convert(htmlPath, pdfPath))
				{
					string errorString = File.ReadAllText(log.Path);
					if (errorString.Contains("error: can't open output file: Permission denied"))
					{
						SIL.Reporting.ErrorReport.NotifyUserOfProblem("Sorry! We couldn't generate the PDF file, probably because the old one is still open in your PDF viewer. Close the PDF and then try again. (Permission denied writing to PDF file)");
						retval = false;
					}
					else
					{
						throw new ApplicationException(errorString);
					}
				}
			}
			return retval;
		}

		private static string GetPrincePath()
		{
#if __MonoCS__
			return "/usr/bin/prince";
#else
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
#endif
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