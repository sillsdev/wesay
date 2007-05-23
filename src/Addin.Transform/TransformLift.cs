using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Mono.Addins;
using WeSay.AddinLib;
using System.Drawing;

namespace Addin.Transform
{
	[Extension]
	public class TransformLift : WeSay.AddinLib.IWeSayAddin, WeSay.AddinLib.IWeSayAddinHasSettings
	{
		public Image ButtonImage
		{
			get
			{
				return Resources.buttonImage;
			}
		}

		public bool Available
		{
			get
			{
				return true;
			}
		}

		public string Name
		{
			get
			{
				return "SFM Exporter";
			}
		}

		public string ShortDescription
		{
			get
			{
				return "[Under construction] Saves the lexicon in a form of standard format.";
			}
		}

		#region IWeSayAddin Members


		[Serializable]
		public class TransformSettings
		{
			private string _sfmTagConversions;

			[Browsable(true)]

			[Description(@"The raw output gives tags like \lx_bth, but you likely want to see simple tags like \lx. Enter from<space>to pairs, one per line. E.g. lx_bth lx")]
			public string SfmTagConversions
			{
				get
				{
				   return _sfmTagConversions;
				}
				set
				{
					_sfmTagConversions = value;
				}
			}

			public class ChangePair
			{
				public string from;
				public string to;
			}

			[XmlIgnore]
			public ChangePair[] ChangePairs
			{
				get
				{
					List<ChangePair> pairs = new List<ChangePair>();
					using (StringReader reader = new StringReader(_sfmTagConversions))
					{
						while (true)
						{
							string line = reader.ReadLine();
							if (line != null)
							{
								string[] parts = line.Split(new char[] {' '});
								if (parts.Length != 2)
								{
									//hmmmm
								}
								else
								{
									ChangePair p = new ChangePair();
									p.from = @"(\W)*"+parts[0]+@"(\W)"; //only match if bounded by white space
									p.to = "$1"+parts[1]+"$2"; // put the spaces back in
									pairs.Add(p);
								}
							}
							else
							{
								break;
							}
						}
					}

					return pairs.ToArray();
				}
			}
		}

		private TransformSettings _settings;
		private Guid _id = new Guid("35F67EF4-F984-40b2-88FA-35C10CCFD3A0");

		public TransformLift()
		{
			_settings = new TransformSettings();
			_settings.SfmTagConversions = "g_en ge";
		}



		#endregion

		public void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			System.Xml.Xsl.XslCompiledTransform transform = new System.Xml.Xsl.XslCompiledTransform();

//            XmlWriterSettings os = new XmlWriterSettings();
//            os.OmitXmlDeclaration = true;
//            os.OutputMethod = XmlOutputMethod.Text;
//            t.OutputSettings = os;

			string xslName = "lift2sfm.xsl";
			string xsltPath = projectInfo.LocateFile(xslName);
			if (String.IsNullOrEmpty(xsltPath))
			{
				throw new ApplicationException("Could not find required file, " + xslName);
			}

			//all this just to allow a DTD statement in the source xslt
			System.Xml.XmlReaderSettings readerSettings = new XmlReaderSettings();
			readerSettings.ProhibitDtd = false;
			using (FileStream xsltReader = File.OpenRead(xsltPath))
			{
				transform.Load(XmlReader.Create(xsltReader, readerSettings));
				xsltReader.Close();
			}

			string output = Path.Combine(projectInfo.PathToTopLevelDirectory, projectInfo.Name+"-sfm.txt");
			transform.Transform(projectInfo.PathToLIFT, output);

			GrepFile(output);
			System.Diagnostics.Process.Start(output);
		}


		 private void GrepFile(string inputPath)
		{
			string tempPath = inputPath + ".tmp";
			TransformSettings.ChangePair[] pairs = _settings.ChangePairs;

			using (StreamReader reader = File.OpenText(inputPath))
			{
				using (StreamWriter writer = new StreamWriter(tempPath))
				{
					while (!reader.EndOfStream)
					{
						string line = reader.ReadLine();
						foreach (TransformSettings.ChangePair pair in pairs)
						{
							if (Regex.IsMatch(line, pair.from))
							{
								line = Regex.Replace(line, pair.from, pair.to);
								break; //only supporting one match per line
							}
						}
						writer.WriteLine(line);
					}
					writer.Close();
				}
				reader.Close();
			}

			File.Delete(inputPath);
			File.Move(tempPath, inputPath);//, backupPath);
		}

		#region IWeSayAddinHasSettings Members

		public bool DoShowSettingsDialog(Form parentForm)
		{
			SettingsDialog dlg = new SettingsDialog(_settings);
			return dlg.ShowDialog(parentForm) == DialogResult.OK;
		}


		public object SettingsToPersist
		{
			get
			{
				return _settings;
			}
			set
			{
				_settings = (TransformSettings)value;
			}
		}

		public Guid ID
		{
			get
			{

				return _id;
			}
			set
			{
				_id = value;
			}
		}
		#endregion
	}
}
