
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

using Mono.Addins;

using ICSharpCode.SharpZipLib.Zip;

using Palaso.I8N;
using Palaso.Reporting;
using Palaso.Progress;
using Palaso.UI.WindowsForms.Progress;

using WeSay.Project;
using WeSay.AddinLib;
using WeSay.LexicalModel;
using WeSay.Foundation;

namespace Addin.Transform.OpenOffice
{
	[Extension]
	public class OpenOfficeAddin : IWeSayAddin
	{

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
			get { return StringCatalog.Get("Open Document Text");  }
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

					//projectInfo.LocateFile(Path.Combine("templates", "odfTemplate"));
				string templateDir = Path.Combine(projectInfo.PathToApplicationRootDirectory, "templates");
				args.odfTemplate = Path.Combine(templateDir, "odfTemplate");
				args.topLevelDir = projectInfo.PathToTopLevelDirectory;
				if (! Directory.Exists(args.odfTemplate))
				{
					Object [] msgArgs = {args.odfTemplate };
					ErrorReport.NotifyUserOfProblem("Directory {0} does not exist.", msgArgs);
				}
				if (DoTransformWithProgressDialog(args))
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


			string pliftPath = Path.Combine(arguments.exportDir,
											arguments.name + ".plift");
			progressState.StatusLabel = "Transforming to PLift";
			var maker = new PLiftMaker();
			maker.MakePLiftTempFile(pliftPath, arguments.lexEntryRepository, arguments.viewTemplate);
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
					   new System.IO.StringReader(Resources.openOfficeLift2odfContent);

				XmlReader xsltReader = XmlReader.Create(sr, readerSettings);
				XsltSettings settings = new XsltSettings(true, true);
				transform.Load(xsltReader, settings, new XmlUrlResolver());
				xsltReader.Close();

				XsltArgumentList xsltArgs = new XsltArgumentList();
				xsltArgs.AddParam("title", "", arguments.name);
				// TODO what is the correct url base path for illustrations?
				xsltArgs.AddParam("urlBase", "", arguments.topLevelDir);

				transform.Transform(pliftPath, xsltArgs, contentOutput);
				contentOutput.Close();

				progressState.StatusLabel = "Preparing ODT styles";

				transform = new XslCompiledTransform();

				string writingSystemPath = Path.Combine(arguments.topLevelDir,
														"WritingSystemPrefs.xml");
				System.IO.StringReader srStyles =
					   new System.IO.StringReader(Resources.openOfficeWritingSystem2odfStyles);
				XmlReader xsltReaderStyles = XmlReader.Create(srStyles, readerSettings);
				XsltSettings stylesSettings = new XsltSettings(true, true);
				transform.Load(xsltReaderStyles, stylesSettings, new XmlUrlResolver());
				xsltReaderStyles.Close();

				Field lexicalFormField = arguments.viewTemplate.Fields.Find(f=> f.FieldName == "EntryLexicalForm");
				xsltArgs = new XsltArgumentList();
				if (lexicalFormField != null)
				{
					if (lexicalFormField.WritingSystemIds.Count > 0)
					{
						xsltArgs.AddParam("primaryLangCode", "", lexicalFormField.WritingSystemIds[0]);
					}
				}

				transform.Transform(writingSystemPath, xsltArgs, stylesOutput);

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
				progressState.WriteToLog(e.Message);
				throw e;
			}
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
				string zipPath = Path.Combine(zipDirectory, fileName);
				zipFile.Add(f, zipPath);
			}
			foreach (string d in Directory.GetDirectories(directoryName))
			{
				string zipPath = Path.Combine(zipDirectory, Path.GetFileName(d));
				addDirectoryFilesToZip(zipFile, d, zipPath);
			}
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
