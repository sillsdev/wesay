
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using Palaso.Data;
using Palaso.Lift;
using Palaso.DictionaryServices;
using Palaso.DictionaryServices.Model;
using Palaso.DictionaryServices.Lift;
using Mono.Addins;

using ICSharpCode.SharpZipLib.Zip;

using Palaso.i18n;
using Palaso.Reporting;
using Palaso.Progress;
using Palaso.UI.WindowsForms.Progress;

using WeSay.Project;
using WeSay.AddinLib;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Foundation;
using WeSay.Foundation;

namespace Addin.Transform.OpenOffice
{
	[Extension]
	public class OpenOfficeAddin : IWeSayAddin
	{
		private Boolean _launchAfterExport = true;

		public OpenOfficeAddin()
		{

		}

		public string LocalizedName
		{
			get { return StringCatalog.Get("Save in OpenOffice"); }
		}

		public string ID
		{
			get { return "OpenOffice"; }
		}

		public string Description
		{
			get { return "Save the dictionary in Open Document Text (OpenOffice) format."; }
		}

		public Image ButtonImage
		{
			get { return Resources.openOffice; }
		}

		public Image DashboardButtonImage
		{
			get { return Resources.openOffice; }
		}

		public DashboardGroup Group
		{
			get { return DashboardGroup.Share; }
		}

		public string LocalizedLabel
		{
			get { return StringCatalog.Get("Make OpenOffice Document"); }
		}

		public string LocalizedLongLabel
		{
			get {
				return  StringCatalog.Get(
						"Save the dictionary in Open Document Text (OpenOffice) format.");
			}
		}


		public ButtonStyle DashboardButtonStyle
		{
			get { return ButtonStyle.IconVariableWidth; }
		}

		public bool Available
		{
			get
			{
				return true;
			}
		}

		public void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			string odtPath = Path.Combine(projectInfo.PathToExportDirectory,
											  projectInfo.Name + ".odt");
			try
			{

				TransformWorkerArguments args = new TransformWorkerArguments();
				args.odtPath = odtPath;
				args.name = projectInfo.Name;
				args.exportDir = projectInfo.PathToExportDirectory;
				args.appRoot = projectInfo.PathToApplicationRootDirectory;
				args.viewTemplate =  projectInfo.ServiceProvider.GetService(typeof(ViewTemplate))
					as ViewTemplate;
				args.lexEntryRepository = projectInfo.ServiceProvider.GetService(typeof(LexEntryRepository))
					as LexEntryRepository;

				string templateDir = Path.Combine(projectInfo.PathToApplicationRootDirectory, "templates");
				args.odfTemplate = Path.Combine(templateDir, "odfTemplate");
				args.topLevelDir = projectInfo.PathToTopLevelDirectory;
				if (! Directory.Exists(args.odfTemplate))
				{
					Object [] msgArgs = {args.odfTemplate };
					ErrorReport.NotifyUserOfProblem("Directory {0} does not exist.", msgArgs);
				}
				if (DoTransformWithProgressDialog(args) && _launchAfterExport)
					Process.Start(odtPath);
			}
			catch (Exception e)
			{
				ErrorReport.NotifyUserOfProblem(e.Message);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns>false if not successful or cancelled</returns>
		private static bool DoTransformWithProgressDialog
			(TransformWorkerArguments arguments)
		{

			using (ProgressDialog dlg = new ProgressDialog())
			{
				dlg.Overview = "Please wait...";
				BackgroundWorker worker = new BackgroundWorker();
				worker.DoWork += OnDoTransformWork;
				dlg.BackgroundWorker = worker;
				dlg.CanCancel = true;
				//dlg.CancelRequested += new EventHandler(OnCancelRequested);
				dlg.ProgressState.Arguments = arguments;
				dlg.ShowDialog();
				worker.RunWorkerAsync();
				if (dlg.ProgressStateResult != null &&
					dlg.ProgressStateResult.ExceptionThatWasEncountered != null)
				{
					ErrorReport.ReportNonFatalException(
							dlg.ProgressStateResult.ExceptionThatWasEncountered);
					return false;
				}
				return !dlg.ProgressState.Cancel;
			}
		}

		internal class TransformWorkerArguments
		{
			public ViewTemplate viewTemplate;
			public LexEntryRepository lexEntryRepository;
			public string odtPath;
			public string name;
			public string exportDir;
			public string topLevelDir;
			public string appRoot;
			public string odfTemplate;
		}

		private static void SortLift(string outputPath, LexEntryRepository lexEntryRepository, ViewTemplate template)
		{
			using (var exporter = new LiftWriter(outputPath, LiftWriter.ByteOrderStyle.NoBOM))
			{
				WritingSystem firstWs = template.HeadwordWritingSystems[0];
				ResultSet<LexEntry> recordTokens =
					lexEntryRepository.GetAllEntriesSortedByHeadword(firstWs);
				int index = 0;
				foreach (RecordToken<LexEntry> token in recordTokens)
				{
					int homographNumber = 0;
					if ((bool) token["HasHomograph"])
					{
						homographNumber = (int) token["HomographNumber"];
					}
					LexEntry lexEntry = token.RealObject;
					EmbeddedXmlCollection sortedAnnotation = new EmbeddedXmlCollection();
					sortedAnnotation.Values.Add("<annotation name='sorted-index' value='" + (++index) +"'/>");

					lexEntry.Properties.Add(new KeyValuePair<string,object>("SortedIndex", sortedAnnotation));

					exporter.Add(lexEntry, homographNumber);
				}

				exporter.End();
			}
		}

		/// <summary>
		/// this runs in a worker thread
		/// </summary>
		private static void OnDoTransformWork(object sender, DoWorkEventArgs args)
		{
			ProgressState progressState = (ProgressState) args.Argument;
			if (progressState == null) return;
			TransformWorkerArguments arguments =
				(TransformWorkerArguments)progressState.Arguments;
			//ProjectInfo projectInfo = arguments.projectInfo;


			string liftPath = Path.Combine(arguments.exportDir,
											arguments.name + ".lift");
			progressState.StatusLabel = "Sorting Lift";
			SortLift(liftPath, arguments.lexEntryRepository, arguments.viewTemplate);
			//var maker = new PLiftMaker();
			//maker.MakePLiftTempFile(pliftPath, arguments.lexEntryRepository, arguments.viewTemplate);
			String odfTemplate =  arguments.odfTemplate;
			String contentPath = Path.Combine(arguments.exportDir, "content.xml");
			String stylesPath = Path.Combine(arguments.exportDir, "styles.xml");
			Stream stylesOutput = new FileStream(stylesPath, FileMode.Create);
			Stream contentOutput = new FileStream(contentPath, FileMode.Create);

			progressState.StatusLabel = "Preparing ODT content";
			XslCompiledTransform transform = null;
			XmlReaderSettings readerSettings = new XmlReaderSettings();
			try
			{
				transform = new XslCompiledTransform();

				System.IO.StringReader sr =
					   new System.IO.StringReader(Resources.lift2odfContent);

				XmlReader xsltReader = XmlReader.Create(sr, readerSettings);
				XsltSettings settings = new XsltSettings(true, true);
				transform.Load(xsltReader, settings, new XmlUrlResolver());
				xsltReader.Close();

				XsltArgumentList xsltArgs = new XsltArgumentList();
				xsltArgs.AddParam("title", "", arguments.name);
				// TODO what is the correct url base path for illustrations?
				// It seems one level up just gets out of the zip file, so use 2 levels here
				xsltArgs.AddParam("urlBase", "",  "../../pictures/");

				transform.Transform(liftPath, xsltArgs, contentOutput);
				contentOutput.Close();

				progressState.StatusLabel = "Preparing ODT styles";

				transform = new XslCompiledTransform();

				StringReader srStyles = new StringReader(Resources.ldml2odfStyles);
				XmlReader xsltReaderStyles = XmlReader.Create(srStyles, readerSettings);
				XsltSettings stylesSettings = new XsltSettings(true, true);
				transform.Load(xsltReaderStyles, stylesSettings, new XmlUrlResolver());
				xsltReaderStyles.Close();

				xsltArgs = new XsltArgumentList();
				xsltArgs.AddParam("primaryLangCode", "", GetHeadwordWritingSystemId(arguments.viewTemplate));

				string pathToTempFile = CreateSingleWritingsystemsFileForEasyXslProcessing();
				transform.Transform(pathToTempFile, xsltArgs, stylesOutput);
				File.Delete(pathToTempFile);

				stylesOutput.Close();
				if (File.Exists(arguments.odtPath))
					File.Delete(arguments.odtPath);
				progressState.StatusLabel = "Creating ODT file";
				ZipFile zipFile = ZipFile.Create(arguments.odtPath);
				zipFile.BeginUpdate();
				zipFile.Add(stylesPath, "styles.xml");
				zipFile.Add(contentPath, "content.xml");

				// add files from template
				if (Directory.Exists(odfTemplate))
					addDirectoryFilesToZip(zipFile, odfTemplate, "");
				else
				{
					progressState.WriteToLog("Directory not found: " + odfTemplate);
				}
				zipFile.CommitUpdate();
				zipFile.Close();
				progressState.StatusLabel = "Openning in OpenOffice";
			}
			catch(Exception e)
			{
				progressState.ExceptionThatWasEncountered = e;
				progressState.WriteToLog(e.Message);
			}
		}

		private static string CreateSingleWritingsystemsFileForEasyXslProcessing()
		{
			string pathToTempFile = Path.GetTempFileName();

			StreamWriter tempFileWriter = new StreamWriter(pathToTempFile);
			string xmlHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>";
			tempFileWriter.WriteLine(xmlHeader);
			tempFileWriter.WriteLine("<root>");

			string pathToLdmlWritingSystemsFolder =
				BasilProject.GetPathToLdmlWritingSystemsFolder(BasilProject.Project.ProjectDirectoryPath);

			foreach (string fileName in Directory.GetFiles(pathToLdmlWritingSystemsFolder, "*.ldml"))
			{
				StreamReader ldmlFileStream = new StreamReader(fileName);
				string line;
				while ((line = ldmlFileStream.ReadLine()) != null)
				{
					if(line.Contains("<?xml"))
					{
						continue;
					}
					tempFileWriter.WriteLine(line);
				}
				ldmlFileStream.Close();
			}
			tempFileWriter.WriteLine("</root>");
			tempFileWriter.Close();
			return pathToTempFile;
		}

		//private static string GetPathToPrimaryWritingSystemsLdmlFile(TransformWorkerArguments arguments)
		//{
		//    string pathToHeadWordWritingSystemLdmlFile = "";
		//    string pathToLdmlWritingSystemsFolder =
		//        BasilProject.GetPathToLdmlWritingSystemsFolder(BasilProject.Project.ProjectDirectoryPath);
		//    foreach (string fileName in Directory.GetFiles(pathToLdmlWritingSystemsFolder))
		//    {
		//        if(Path.GetFileNameWithoutExtension(fileName) == GetHeadwordWritingSystemId(arguments.viewTemplate))
		//        {
		//            pathToHeadWordWritingSystemLdmlFile = Path.GetFullPath(fileName);
		//            break;
		//        }
		//    }
		//    if(String.IsNullOrEmpty(pathToHeadWordWritingSystemLdmlFile)){
		//        string errorMessage = String.Format(
		//            "No Ldml writing system file could be found in folder {0}. The headword writing system is {1}",
		//            pathToLdmlWritingSystemsFolder, GetHeadwordWritingSystemId(arguments.viewTemplate));
		//        throw new ApplicationException(errorMessage);

		//    }
		//    return pathToHeadWordWritingSystemLdmlFile;
		//}

		//private static string CreateTempFileContainingListOfLdmlWritingSystemFiles()
		//{
		//    string pathToTempFile = Path.GetTempFileName();
		//    StreamWriter tempFileWriter = new StreamWriter(pathToTempFile, false, System.Text.Encoding.UTF8);
		//    XmlWriter xmlListOfLdmlWritingsystemFiles = XmlWriter.Create(tempFileWriter);
		//    xmlListOfLdmlWritingsystemFiles.WriteStartDocument();
		//    string pathToLdmlWritingSystemsFolder =
		//        BasilProject.GetPathToLdmlWritingSystemsFolder(BasilProject.Project.ProjectDirectoryPath);
		//    xmlListOfLdmlWritingsystemFiles.WriteStartElement("LdmlWsFiles");
		//    foreach (string fileName in Directory.GetFiles(pathToLdmlWritingSystemsFolder, "*.ldml"))
		//    {
		//        xmlListOfLdmlWritingsystemFiles.WriteStartElement("LdmlWsFile");
		//        xmlListOfLdmlWritingsystemFiles.WriteString(Path.GetFullPath(fileName));
		//        xmlListOfLdmlWritingsystemFiles.WriteEndElement();
		//    }
		//    xmlListOfLdmlWritingsystemFiles.WriteEndElement();
		//    xmlListOfLdmlWritingsystemFiles.WriteEndDocument();
		//    xmlListOfLdmlWritingsystemFiles.Close();
		//    tempFileWriter.Close();
		//    return pathToTempFile;
		//}

		private static string GetHeadwordWritingSystemId(ViewTemplate viewTemplate)
		{

				Field lexicalFormField = viewTemplate.Fields.Find(f=> f.FieldName == "EntryLexicalForm");
				if (lexicalFormField != null)
				{
					if (lexicalFormField.WritingSystemIds.Count > 0)
					{
						return lexicalFormField.WritingSystemIds[0];
					}
				}
			return "";
	}


/// <summary>
///
/// </summary>
/// <param name="zipFile">
/// A <see cref="ZipFile"/>
/// </param>
/// <param name="directoryName">
/// A <see cref="System.String"/>
/// </param>
/// <param name="zipDirectory">
/// A <see cref="System.String"/>
/// </param>
		private static void addDirectoryFilesToZip(ZipFile zipFile, string directoryName, string zipDirectory)
		{
			string [] fileNames = System.IO.Directory.GetFiles(directoryName);
			foreach (string f in fileNames)
			{
				string fileName = Path.GetFileName(f);
				// ignore hidden files
				if (fileName.StartsWith(".")) continue;
				string zipPath = Path.Combine(zipDirectory, fileName);
				zipFile.Add(f, zipPath);
			}
			foreach (string d in Directory.GetDirectories(directoryName))
			{
				string zipPath = Path.Combine(zipDirectory, Path.GetFileName(d));
				addDirectoryFilesToZip(zipFile, d, zipPath);
			}
		}

		public bool LaunchAfterExport
		{
			set { _launchAfterExport = value; }
		}

		/*
		public static Stream GetXsltStream(string xsltName)
		{
			Stream stream =
					Assembly.GetExecutingAssembly().GetManifestResourceStream("Addin.Transform." +
																			  xsltName);
			Debug.Assert(stream != null);
			return stream;
		}
		*/
	}
}
