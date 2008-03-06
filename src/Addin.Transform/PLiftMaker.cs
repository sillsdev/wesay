

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Palaso.UI.WindowsForms.i8n;
using Palaso.UI.WindowsForms.Progress;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace Addin.Transform
{
	public class PLiftMaker
	{
		public string MakePLiftTempFile(IEnumerable<LexEntry> entries, ViewTemplate template, IHomographCalculator homographCalculator,  IFindEntries finder)
		{
			string path = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
			LiftExporter exporter = new LiftExporter(path);
			exporter.SetUpForPresentationLiftExport(template, homographCalculator, finder);
			foreach (LexEntry entry in entries)
			{
				exporter.Add(entry);
			}
			exporter.End();
			return path;
		}

		public string MakePLiftTempFile( Db4oRecordListManager recordListManager, WeSayWordsProject project)
		{
			IHomographCalculator homographCalculator = new HomographCalculator(recordListManager, project.DefaultPrintingTemplate.HeadwordWritingSytem);

			IEnumerable<LexEntry> entries = Lexicon.GetAllEntriesSortedByHeadword(project.DefaultPrintingTemplate.HeadwordWritingSytem);
			Db4oLexEntryFinder finder = new Db4oLexEntryFinder(recordListManager.DataSource);

			return MakePLiftTempFile(entries, project.DefaultPrintingTemplate, homographCalculator, finder);
		}
//
//        public void MakeXHtmlFile(string outputPath, Db4oRecordListManager recordListManager, WeSayWordsProject project)
//        {
//            IHomographCalculator homographCalculator = new HomographCalculator(recordListManager, project.DefaultPrintingTemplate.HeadwordWritingSytem);
//
//            IEnumerable<LexEntry> entries = Lexicon.GetAllEntriesSortedByHeadword(project.DefaultPrintingTemplate.HeadwordWritingSytem);
//            Db4oLexEntryFinder finder = new Db4oLexEntryFinder(recordListManager.DataSource);
//
//            string pliftPath = MakePLiftTempFile(entries, project.DefaultPrintingTemplate, homographCalculator, finder);
//            try
//            {
//                using (Stream xsltStream = GetXsltStream("plift2html.xsl"))
//                {
//                    TransformWithProgressDialog dlg = new TransformWithProgressDialog(pliftPath,
//                                                                                      outputPath,
//                                                                                      xsltStream,
//                                                                                      "lift/entry");
//                    dlg.TaskMessage =
//                        StringCatalog.Get("Preparing dictionary for printing...",
//                                          "This is shown when WeSay is creating the pdf document for printing.");
//                    dlg.AddArgument("writing-system-info-file", project.PathToWritingSystemPrefs);
//                    dlg.Transform();
//                }
//            }
//            finally
//            {
//                if(File.Exists(pliftPath))
//                {
//                    File.Delete(pliftPath);
//                }
//            }
//        }

		public static Stream GetXsltStream(string xsltName)
		{
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Addin.Publish." + xsltName);
			Debug.Assert(stream != null);
			return stream;
		}
	}
}