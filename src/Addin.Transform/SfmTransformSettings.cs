using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using WeSay.AddinLib;
using WeSay.Language;

namespace Addin.Transform
{
	[Serializable]
	public class SfmTransformSettings
	{
		private string _sfmTagConversions;
		private string _englishLanguageWritingSystemId;
		private string _nationalLanguageWritingSystemId;
		private string _regionalLanguageWritingSystemId;
		private string _vernacularLanguageWritingSystemId;

		public SfmTransformSettings()
		{
			SfmTagConversions = "";
			_englishLanguageWritingSystemId = "en";
		}

		public void FillEmptySettingsWithGuesses(ProjectInfo projectInfo)
		{
			if (String.IsNullOrEmpty(this.VernacularLanguageWritingSystemId))
			{
				if (projectInfo.WritingSystems.ContainsKey("v"))
				{
					this.VernacularLanguageWritingSystemId = "v";
				}
				else //guess
				{
					foreach (string id in projectInfo.WritingSystems.Keys)
					{
						if (!"en fr chn th tpi".Contains(id))
						{
							this.VernacularLanguageWritingSystemId = id;
							break;
						}
					}
				}
			}

			if (String.IsNullOrEmpty(this.NationalLanguageWritingSystemId))
			{
				if (projectInfo.WritingSystems.ContainsKey("tpi")) //melanesian pidgin
				{
					this.NationalLanguageWritingSystemId = "tpi";
				}
				if (projectInfo.WritingSystems.ContainsKey("TPI")) //melanesian pidgin
				{
					this.NationalLanguageWritingSystemId = "TPI";
				}
				if (projectInfo.WritingSystems.ContainsKey("th")) //thai
				{
					this.NationalLanguageWritingSystemId = "th";
				}
				if (projectInfo.WritingSystems.ContainsKey("fr")) //french
				{
					this.NationalLanguageWritingSystemId = "fr";
				}
			}
		}

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

			public ChangePair( )
			{
			}
			public ChangePair( string sfrom, string sto)
			{
				from = @"(\W)*" + sfrom+ @"(\W)";
				to = "$1" + sto +   "$2"; //put the spaces back in

			}
		}

		[XmlIgnore]
		public IEnumerable<SfmTransformSettings.ChangePair> ChangePairs
		{
			get
			{
				List<ChangePair> pairs = new List<ChangePair>();
				pairs.Add(new ChangePair( "BaseForm", "base"));
				pairs.Add(new ChangePair( "SemanticDomainDdp4", "sd"));

				if (!String.IsNullOrEmpty(_vernacularLanguageWritingSystemId))
				{
					pairs.Add(new ChangePair( "lx_" + _vernacularLanguageWritingSystemId, "lx"));
					pairs.Add(new ChangePair( "lc_" + _vernacularLanguageWritingSystemId, "lc"));
					pairs.Add(new ChangePair( "x_" + _vernacularLanguageWritingSystemId,   "xv"));
					pairs.Add(new ChangePair( "d_" + _vernacularLanguageWritingSystemId,   "dv"));
				}

				if (!String.IsNullOrEmpty(_englishLanguageWritingSystemId))
				{
					pairs.Add(new ChangePair("g_" + _englishLanguageWritingSystemId, "ge"));
					pairs.Add(new ChangePair( "d_" + _englishLanguageWritingSystemId, "de"));
					pairs.Add(new ChangePair( "x_" + _englishLanguageWritingSystemId, "xe"));
					pairs.Add(new ChangePair( "nt_" + _englishLanguageWritingSystemId, "nt"));
				}

				if (!String.IsNullOrEmpty(_nationalLanguageWritingSystemId))
				{
					pairs.Add(new ChangePair( "g_" + _nationalLanguageWritingSystemId, "gn"));
					pairs.Add(new ChangePair( "d_" + _nationalLanguageWritingSystemId, "dn"));
					pairs.Add(new ChangePair( "x_" + _nationalLanguageWritingSystemId, "xn"));
					pairs.Add(new ChangePair( "nt_" + _nationalLanguageWritingSystemId, "ntn"));
				}

				if (!String.IsNullOrEmpty(_regionalLanguageWritingSystemId))
				{
					pairs.Add(new ChangePair( "g_" + _regionalLanguageWritingSystemId, "gr"));
					pairs.Add(new ChangePair( "d_" + _regionalLanguageWritingSystemId, "dr"));
					pairs.Add(new ChangePair( "x_" + _regionalLanguageWritingSystemId, "xr"));
				}

				if (_sfmTagConversions == null)
				{
					return pairs;
				}
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
								ChangePair p = new ChangePair(  );
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

				return pairs;
			}
		}

		public string  EnglishLanguageWritingSystemId
		{
			get
			{
				return _englishLanguageWritingSystemId;
			}
			set
			{
				_englishLanguageWritingSystemId = value;
			}
		}

		public string NationalLanguageWritingSystemId
		{
			get
			{
				return _nationalLanguageWritingSystemId;
			}
			set
			{
				_nationalLanguageWritingSystemId = value;
			}
		}

		public string RegionalLanguageWritingSystemId
		{
			get
			{
				return _regionalLanguageWritingSystemId;
			}
			set
			{
				_regionalLanguageWritingSystemId = value;
			}
		}

		public string VernacularLanguageWritingSystemId
		{
			get
			{
				return _vernacularLanguageWritingSystemId;
			}
			set
			{
				_vernacularLanguageWritingSystemId = value;
			}
		}
	}
}