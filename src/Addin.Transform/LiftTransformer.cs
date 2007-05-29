using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using WeSay.AddinLib;

namespace Addin.Transform
{
	public abstract class LiftTransformer : IWeSayAddin
	{
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
		public virtual bool DefaultVisibleInWeSay
		{
			get
			{
				return false;
			}
		}



		public abstract void Launch(Form parentForm, ProjectInfo projectInfo);


		protected static string TransformLift(ProjectInfo projectInfo, string xsltName, string outputFileSuffix)
		{
			XslCompiledTransform transform = new XslCompiledTransform();

			string xslName = xsltName;
			string xsltPath = projectInfo.LocateFile(xslName);
			if (String.IsNullOrEmpty(xsltPath))
			{
				throw new ApplicationException("Could not find required file, " + xslName);
			}

			//all this just to allow a DTD statement in the source xslt
			XmlReaderSettings readerSettings = new XmlReaderSettings();
			readerSettings.ProhibitDtd = false;
			using (FileStream xsltReader = File.OpenRead(xsltPath))
			{
				transform.Load(XmlReader.Create(xsltReader, readerSettings));
				xsltReader.Close();
			}

			string output = Path.Combine(projectInfo.PathToTopLevelDirectory, projectInfo.Name+outputFileSuffix);
			transform.Transform(projectInfo.PathToLIFT, output);
			return output;
		}
	}
}
