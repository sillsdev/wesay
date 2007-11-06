using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Palaso.Progress;
using Palaso.UI.WindowsForms.Progress;
using Palaso.WritingSystems.Collation;
using WeSay.AddinLib;
using WeSay.Language;

namespace Addin.Transform
{
	public abstract class LiftTransformer : IWeSayAddin
	{
		protected bool _launchAfterTransform=true;
		private string _pathToOutput;
		private static ProgressState _staticProgressStateForWorker;
		public delegate void FileManipulationMethod(object sender, DoWorkEventArgs e);
		private  FileManipulationMethod _postTransformMethod;
		private object _postTransformArgument;
		private int _postTransformWorkProgressStepsPerEntry;

		public abstract Image ButtonImage { get;}


		public abstract string Name
		{
			get;
		}

		public abstract string ShortDescription
		{
			get;
		}

		public virtual string ID
		{
			get
			{
				return Name;
			}
		}


		public virtual bool Available
		{
			get
			{
				return true;
			}
		}


		//for unit tests
		public string PathToOutput
		{
			get
			{
				return _pathToOutput;
			}
		}

		//for unit tests
		public bool LaunchAfterTransform
		{
			set
			{
				_launchAfterTransform = value;
			}
		}

		/// <summary>
		/// Provides progress-bar access to a long running method that happens after the transform
		/// </summary>
		public FileManipulationMethod PostTransformMethod
		{
			get { return _postTransformMethod; }
			set { _postTransformMethod = value; }
		}


		public abstract void Launch(Form parentForm, ProjectInfo projectInfo);

		protected string TransformLift(ProjectInfo projectInfo, string xsltName, string outputFileSuffix)
		{
			return TransformLift(projectInfo, xsltName, outputFileSuffix, new XsltArgumentList());
		}

		protected string TransformLift(ProjectInfo projectInfo, string xsltName, string outputFileSuffix, XsltArgumentList arguments)
		{
			_pathToOutput = Path.Combine(projectInfo.PathToTopLevelDirectory, projectInfo.Name + outputFileSuffix);
			if (File.Exists(_pathToOutput))
			{
				File.Delete(_pathToOutput);
			}

			TransformWorkerArguments targs = new TransformWorkerArguments();
			targs.postTransformMethod = _postTransformMethod;
			targs.postTransformArgument = _postTransformArgument;
			targs.progressStepsPerEntry = _postTransformWorkProgressStepsPerEntry;
			targs.outputFilePath = _pathToOutput;
			using (targs.outputStream = File.Create(_pathToOutput))
			{
				targs.inputDocument = new XmlDocument();
				targs.inputDocument.PreserveWhitespace = true;
				targs.inputDocument.Load(projectInfo.PathToLIFT);
				XPathNavigator navigator = targs.inputDocument.CreateNavigator();

				PrepareSorting(projectInfo, navigator);
				targs.xsltStream = GetXsltStream(projectInfo, xsltName);
				targs.xsltArguments = arguments;
				if(!DoTransformWithProgressDialog(targs))
				{
					return null;
				}
			}

			return _pathToOutput;
		}


		/// <summary>
		///
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns>false if not successful or cancelled</returns>
		private bool DoTransformWithProgressDialog(TransformWorkerArguments arguments)
		{
			using (ProgressDialog dlg = new ProgressDialog())
			{
				dlg.Overview = "Please wait...";
				BackgroundWorker worker = new BackgroundWorker();
				worker.DoWork += new DoWorkEventHandler(OnDoTransformWork);
				dlg.BackgroundWorker = worker;
				dlg.CanCancel = true;
				//dlg.CancelRequested += new EventHandler(OnCancelRequested);
				dlg.InitialProgressState.Arguments = arguments;
				dlg.ShowDialog();
				if (dlg.ProgressStateResult!=null && dlg.ProgressStateResult.ExceptionThatWasEncountered != null)
				{
					Palaso.Reporting.ErrorNotificationDialog.ReportException(dlg.ProgressStateResult.ExceptionThatWasEncountered,null,false);
					return false;
				}
				return !dlg.InitialProgressState.Cancel;
			}
			return false;
		}

		/// <summary>
		/// do a hard cancel of the thread, because the xslt transformer doesn't give us a way.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnCancelRequested(object sender, EventArgs e)
		{

		  //didn't work  ((ProgressDialog) sender).BackgroundWorker.Dispose();
		}

		internal class TransformWorkerArguments
		{
			public XmlDocument inputDocument;
			public XsltArgumentList xsltArguments;
			public Stream outputStream;
			public Stream xsltStream;
			public FileManipulationMethod postTransformMethod;
			public string outputFilePath;
			public object postTransformArgument;
			public int progressStepsPerEntry;
		}

		/// <summary>
		/// this runs in a worker thread
		/// </summary>
		private static void OnDoTransformWork(object sender, DoWorkEventArgs args)
		{
			ProgressState progressState = (ProgressState) args.Argument;
			XslCompiledTransform transform = null;
			try
			{
				TransformWorkerArguments workerArguments = (TransformWorkerArguments) progressState.Arguments;

				transform = new XslCompiledTransform();

				//all this just to allow a DTD statement in the source xslt
				XmlReaderSettings readerSettings = new XmlReaderSettings();
				readerSettings.ProhibitDtd = false;

				progressState.StatusLabel = "Preparing...";
				using (Stream stream = workerArguments.xsltStream)
				{
					using (XmlReader xsltReader = XmlReader.Create(stream, readerSettings))
					{
						XsltSettings settings = new XsltSettings(true, true);
						transform.Load(xsltReader, settings, new XmlUrlResolver());
						xsltReader.Close();
					}
					stream.Close();
				}

				progressState.StatusLabel = "Exporting...";
				int entriesCount = workerArguments.inputDocument.SelectNodes("//entry").Count;
				progressState.TotalNumberOfSteps = entriesCount + (int)((entriesCount * workerArguments.progressStepsPerEntry));
				_staticProgressStateForWorker = progressState;
				workerArguments.xsltArguments.XsltMessageEncountered += new XsltMessageEncounteredEventHandler(OnXsltMessageEncountered);
				transform.Transform(workerArguments.inputDocument, workerArguments.xsltArguments,
									workerArguments.outputStream);

				workerArguments.outputStream.Close();//let the next guy get at the file
				if(workerArguments.postTransformMethod != null)
				{
					workerArguments.postTransformMethod.Invoke(sender, args);
				}
				progressState.State = ProgressState.StateValue.Finished;
			}
			catch(CancelingException notAnErr)
			{
				progressState.State = ProgressState.StateValue.Finished;
			}
			catch (Exception err)
			{
				//currently, error reporter can choke because this is
				//being called from a non sta thread.
				//so let's leave it to the progress dialog to report the error
				//                Reporting.ErrorReporter.ReportException(args,null, false);
				progressState.ExceptionThatWasEncountered = err;
				progressState.WriteToLog(err.Message);
				progressState.State = ProgressState.StateValue.StoppedWithError;
			}
			finally
			{
				if (transform != null)
				{
					progressState.StatusLabel = "Cleaning up...";
					transform.TemporaryFiles.Delete();
				}
			}
		}

		static void OnXsltMessageEncountered(object sender, XsltMessageEncounteredEventArgs e)
		{
			_staticProgressStateForWorker.NumberOfStepsCompleted++;
			if(_staticProgressStateForWorker.Cancel)
			{
			   throw new CancelingException();
			}
		}

		 /// <summary>
		/// used to break us out of the xslt transformer if the user cancels
		/// </summary>
		private class CancelingException : ApplicationException
		{
		}

		private void PrepareSorting(ProjectInfo projectInfo, XPathNavigator navigator)
		{
			//TODO:
			//This is a stop-gap the majority case is one where there is only a
			//single writing system so it works. In reality, the user should
			//decide what writing system they want as their head word writing system.
			//It needs to be passed to the xslt as well. Currently the xslt just
			//uses the first one it finds (just like the sort).

			XPathNavigator headwordWritingSystemAttribute = navigator.SelectSingleNode("//lexical-unit/form/@lang");
			if (headwordWritingSystemAttribute != null)
			{
				string headwordWritingSystem = headwordWritingSystemAttribute.Value;
				WritingSystem ws;
				if (projectInfo.WritingSystems.TryGetValue(headwordWritingSystem, out ws))
				{
					string xpathSortKeySource = string.Format("//lexical-unit/form[@lang='{0}']",
															  headwordWritingSystem);
					AddSortKeysToXml.AddSortKeys(navigator,
												 xpathSortKeySource,
												 ws.GetSortKey,
												 "ancestor::entry",
												 "sort-key");
				}
			}
		}

		public static Stream GetXsltStream(ProjectInfo projectInfo, string xsltName)
		{
			//xslt can be in one of the project/wesay locations, (so user can override with their own copy)
			//or just in a resource (helps with forgetting to put it in the installer)
			string xsltPath = projectInfo.LocateFile(xsltName);
			if (String.IsNullOrEmpty(xsltPath))
			{
				return Assembly.GetExecutingAssembly().GetManifestResourceStream("Addin.Transform." + xsltName);
			}
			return File.OpenRead(xsltPath);

		}

		protected void SetupPostTransformMethod(FileManipulationMethod work, object arguments, int progressStepsPerEntry)
		{
			_postTransformMethod = work;
			_postTransformArgument = arguments;
			_postTransformWorkProgressStepsPerEntry = progressStepsPerEntry;
		}
	}
}
