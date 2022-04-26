using SIL.Progress;
using SIL.Reporting;
using SIL.Windows.Forms.Progress;
using SIL.Xml;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using WeSay.AddinLib;
using WeSay.Foundation;

namespace Addin.Transform
{
	public abstract class LiftTransformer : IWeSayAddin
	{
		protected bool _launchAfterTransform = true;
		protected string _pathToOutput;
		private static ProgressState _staticProgressStateForWorker;

		public delegate void FileManipulationMethod(object sender, DoWorkEventArgs e);

		private FileManipulationMethod _postTransformMethod;
		private object _postTransformArgument;
		private int _postTransformWorkSteps;

		public abstract Image ButtonImage { get; }

		public abstract string LocalizedName { get; }

		public abstract string Description { get; }

		#region IWeSayAddin Members

		public abstract string ID { get; }

		#endregion

		#region IThingOnDashboard Members

		public DashboardGroup Group
		{
			get { return DashboardGroup.Share; }
		}

		public abstract string LocalizedLabel { get; }

		public abstract string LocalizedLongLabel { get; }

		public ButtonStyle DashboardButtonStyle
		{
			get { return ButtonStyle.IconVariableWidth; }
		}

		public virtual Image DashboardButtonImage
		{
			get { return null; }
		}

		#endregion

		public virtual bool Available
		{
			get { return true; }
		}
		public bool Deprecated
		{
			get { return false; }
		}

		//for unit tests
		public string PathToOutput
		{
			get { return _pathToOutput; }
		}

		//for unit tests
		public bool LaunchAfterTransform
		{
			set { _launchAfterTransform = value; }
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

		protected string TransformLiftToText(ProjectInfo projectInfo,
											 string xsltName,
											 string outputFileSuffix)
		{
			return TransformLift(projectInfo,
								 xsltName,
								 outputFileSuffix,
								 new XsltArgumentList(),
								 false);
		}

		protected string TransformLift(ProjectInfo projectInfo,
									   string xsltName,
									   string outputFileSuffix,
									   XsltArgumentList arguments,
									   bool outputToXml)
		{
			_pathToOutput = Path.Combine(projectInfo.PathToExportDirectory,
										 projectInfo.Name + outputFileSuffix);
			if (File.Exists(_pathToOutput))
			{
				File.Delete(_pathToOutput);
			}

			TransformWorkerArguments targs = new TransformWorkerArguments();
			targs.postTransformMethod = _postTransformMethod;
			targs.postTransformArgument = _postTransformArgument;
			targs.postTransformSteps = _postTransformWorkSteps;
			targs.outputFilePath = _pathToOutput;
			targs.outputToXml = outputToXml;
			using (targs.outputStream = File.Create(_pathToOutput))
			{
				targs.inputDocument = new XmlDocument();
				targs.inputDocument.PreserveWhitespace = true;
				targs.inputDocument.Load(projectInfo.PathToLIFT);
				targs.xsltStream = GetXsltStream(projectInfo, xsltName);
				targs.xsltArguments = arguments;
				if (!DoTransformWithProgressDialog(targs))
				{
					return null;
				}
			}

			return _pathToOutput;
		}

		/// <summary>
		/// Execute a transform while displaying progress to the user
		///	TODO: Refactor to use a progress indicator that can be replaced with a null indicator during tests
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns>false if not successful or cancelled</returns>
		private static bool DoTransformWithProgressDialog(TransformWorkerArguments arguments)
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
			public XmlDocument inputDocument;
			public XsltArgumentList xsltArguments;
			public Stream outputStream;
			public Stream xsltStream;
			public FileManipulationMethod postTransformMethod;
			public string outputFilePath;
			public object postTransformArgument;
			public int postTransformSteps;
			public bool outputToXml = true;
		}

		/// <summary>
		/// this runs in a worker thread
		/// </summary>
		private static void OnDoTransformWork(object sender, DoWorkEventArgs args)
		{
			ProgressState progressState = (ProgressState)args.Argument;
			XslCompiledTransform transform = null;
			try
			{
				TransformWorkerArguments workerArguments =
						(TransformWorkerArguments)progressState.Arguments;

				transform = new XslCompiledTransform();

				//all this just to allow a DTD statement in the source xslt
				var readerSettings = new XmlReaderSettings {DtdProcessing = DtdProcessing.Parse};

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

				progressState.StatusLabel = "Transforming...";
				int entriesCount = workerArguments.inputDocument.SelectNodes("//entry").Count;
				progressState.TotalNumberOfSteps = 2 * (entriesCount) + workerArguments.postTransformSteps;
				_staticProgressStateForWorker = progressState;
				workerArguments.xsltArguments.XsltMessageEncountered += OnXsltMessageEncountered;

				if (!workerArguments.outputToXml)
				{
					transform.Transform(workerArguments.inputDocument,
										workerArguments.xsltArguments,
										workerArguments.outputStream);
				}
				else
				{
					//all this is to stop sticking on the BOM, which trips up princeXML
					XmlWriterSettings writerSettings = CanonicalXmlSettings.CreateXmlWriterSettings();
					writerSettings.Encoding = new UTF8Encoding(false);

					using (var writer = XmlWriter.Create(workerArguments.outputStream, writerSettings))
					{
						transform.Transform(workerArguments.inputDocument,
											workerArguments.xsltArguments,
											writer);
					}
				}

				workerArguments.outputStream.Close(); //let the next guy get at the file
				Debug.Assert(progressState.NumberOfStepsCompleted <= entriesCount,
							 "Should use up more than we reserved for ourselves");
				progressState.NumberOfStepsCompleted = entriesCount;
				if (workerArguments.postTransformMethod != null)
				{
					workerArguments.postTransformMethod.Invoke(sender, args);
				}
				progressState.State = ProgressState.StateValue.Finished;
			}
			catch (CancelingException) // not an error
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
					TempFileCollection tempfiles = transform.TemporaryFiles;
					if (tempfiles != null) // tempfiles will be null when debugging is not enabled
					{
						tempfiles.Delete();
					}
				}
			}
		}

		private static void OnXsltMessageEncountered(object sender,
													 XsltMessageEncounteredEventArgs e)
		{
			_staticProgressStateForWorker.NumberOfStepsCompleted++;
			if (_staticProgressStateForWorker.Cancel)
			{
				throw new CancelingException();
			}
		}

		/// <summary>
		/// used to break us out of the xslt transformer if the user cancels
		/// </summary>
		private class CancelingException : ApplicationException { }

		public static Stream GetXsltStream(ProjectInfo projectInfo, string xsltName)
		{
			//xslt can be in one of the project/wesay locations, (so user can override with their own copy)
			//or just in a resource (helps with forgetting to put it in the installer)
			string xsltPath = projectInfo.LocateFile(xsltName);
			if (String.IsNullOrEmpty(xsltPath))
			{
				return
						Assembly.GetExecutingAssembly().GetManifestResourceStream(
								"Addin.Transform." + xsltName);
			}
			return File.OpenRead(xsltPath);
		}

		protected void SetupPostTransformMethod(FileManipulationMethod work,
												object arguments,
												int howManySteps)
		{
			_postTransformMethod = work;
			_postTransformArgument = arguments;
			_postTransformWorkSteps = howManySteps;
		}
	}
}