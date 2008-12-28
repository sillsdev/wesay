using System.Diagnostics;
using System.IO;
using System.Reflection;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.Project;

namespace Addin.Transform
{
	public class PLiftMaker
	{
		//private string MakePLiftTempFile(IEnumerable<LexEntry> entries, ViewTemplate template, IFindEntries finder)
		//{
		//    string path = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
		//    LiftExporter exporter = new LiftExporter(path);
		//    exporter.SetUpForPresentationLiftExport(template, finder);
		//    foreach (LexEntry entry in entries)
		//    {
		//        exporter.Add(entry);
		//    }
		//    exporter.End();
		//    return path;
		//}

		public void  MakePLiftTempFile(string outputPath, LexEntryRepository lexEntryRepository, ViewTemplate template)
		{
			PLiftExporter exporter = new PLiftExporter(outputPath, lexEntryRepository, template);
			ResultSet<LexEntry> recordTokens =
					lexEntryRepository.GetAllEntriesSortedByHeadword(template.HeadwordWritingSystem);
			foreach (RecordToken<LexEntry> token in recordTokens)
			{
				int homographNumber = 0;
				if ((bool) token["HasHomograph"])
				{
					homographNumber = (int) token["HomographNumber"];
				}
				exporter.Add(token.RealObject, homographNumber);
			}

			exporter.End();
		}

		//
		//        public void MakeXHtmlFile(string outputPath, LexEntryRepository lexEntryRepository, WeSayWordsProject project)
		//        {
		//            IHomographCalculator homographCalculator = new HomographCalculator(lexEntryRepository, project.DefaultPrintingTemplate.HeadwordWritingSystem);
		//
		//            IEnumerable<LexEntry> entries = Lexicon.GetAllEntriesSortedByHeadword(project.DefaultPrintingTemplate.HeadwordWritingSystem);
		//            Db4oLexEntryFinder finder = new Db4oLexEntryFinder(lexEntryRepository.DataSource);
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
			Stream stream =
					Assembly.GetExecutingAssembly().GetManifestResourceStream("Addin.Publish." +
																			  xsltName);
			Debug.Assert(stream != null);
			return stream;
		}
	}
}