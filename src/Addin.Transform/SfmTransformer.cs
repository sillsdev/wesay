using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using Mono.Addins;
using WeSay.AddinLib;

namespace Addin.Transform
{
	[Extension]
	public class SfmTransformer : LiftTransformer, IWeSayAddinHasSettings
	{
		public SfmTransformer()
		{
			_settings = new SfmTransformSettings();
			_settings.SfmTagConversions = "g_en ge";
		}

		public override string Name
		{
			get
			{
				return "SFM Exporter";
			}
		}

		public override  string ShortDescription
		{
			get
			{
				return "Saves the lexicon in a form of standard format.";
			}
		}


		public override Image ButtonImage
		{
			get
			{
				return Resources.SfmTransformerButtonImage;
			}
		}

		#region IWeSayAddinHasSettings Members

		public bool DoShowSettingsDialog(Form parentForm)
		{
			SFMChangesDialog dlg = new SFMChangesDialog(_settings);
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
				_settings = (SfmTransformSettings)value;
			}
		}

		#endregion


		[Serializable]
		public class SfmTransformSettings
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
								string[] parts = line.Split(new char[] { ' ' });
								if (parts.Length != 2)
								{
									//hmmmm
								}
								else
								{
									ChangePair p = new ChangePair();
									p.from = @"(\W)*" + parts[0] + @"(\W)"; //only match if bounded by white space
									p.to = "$1" + parts[1] + "$2"; // put the spaces back in
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

		private SfmTransformSettings _settings;


		private void GrepFile(string inputPath)
		{
			string tempPath = inputPath + ".tmp";
			SfmTransformSettings.ChangePair[] pairs = _settings.ChangePairs;

			using (StreamReader reader = File.OpenText(inputPath))
			{
				using (StreamWriter writer = new StreamWriter(tempPath))
				{
					while (!reader.EndOfStream)
					{
						string line = reader.ReadLine();
						foreach (SfmTransformSettings.ChangePair pair in pairs)
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





		public override void Launch(Form parentForm, ProjectInfo projectInfo)
		{
			string output = TransformLift(projectInfo, "lift2sfm.xsl", "-sfm.txt");

			GrepFile(output);
			System.Diagnostics.Process.Start(output);
		}
	}
}
