using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using WeSay.AddinLib;

namespace Addin.Transform
{
	public abstract class LiftTransformer : IWeSayAddin
	{
		protected bool _launchAfterTransform=true;
		private string _pathToOutput;
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


		public abstract void Launch(Form parentForm, ProjectInfo projectInfo);


		protected string TransformLift(ProjectInfo projectInfo, string xsltName, string outputFileSuffix)
		{
			//all this just to allow a DTD statement in the source xslt
			XmlReaderSettings readerSettings = new XmlReaderSettings();
			readerSettings.ProhibitDtd = false;
			XslCompiledTransform transform = new XslCompiledTransform();


			using(Stream stream = GetXsltStream(projectInfo, xsltName))
			{
				using(XmlReader xsltReader = XmlReader.Create(stream, readerSettings))
				{
					XsltSettings settings = new XsltSettings(true, true);
					transform.Load(xsltReader, settings, new XmlUrlResolver());
					xsltReader.Close();
				}
				stream.Close();
			}

			_pathToOutput = Path.Combine(projectInfo.PathToTopLevelDirectory, projectInfo.Name + outputFileSuffix);
			if (File.Exists(_pathToOutput))
			{
				File.Delete(_pathToOutput);
			}

			transform.Transform(projectInfo.PathToLIFT, _pathToOutput);
			transform.TemporaryFiles.Delete();

			return _pathToOutput;
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

//            if (String.IsNullOrEmpty(xsltPath))
//            {
//                throw new ApplicationException("Could not find required file, " + xsltName);
//            }
		}
	}
}
