using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Palaso.DictionaryServices.Model;
using Palaso.Reporting;
using WeSay.AddinLib;

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
			if (String.IsNullOrEmpty(VernacularLanguageWritingSystemId))
			{
				if (projectInfo.WritingSystems.ContainsKey("v"))
				{
					VernacularLanguageWritingSystemId = "v";
				}
				else //guess
				{
					foreach (string id in projectInfo.WritingSystems.Keys)
					{
						if (!"en fr chn th tpi".Contains(id))
						{
							VernacularLanguageWritingSystemId = id;
							break;
						}
					}
				}
			}

			if (String.IsNullOrEmpty(NationalLanguageWritingSystemId))
			{
				if (projectInfo.WritingSystems.ContainsKey("tpi")) //melanesian pidgin
				{
					NationalLanguageWritingSystemId = "tpi";
				}
				if (projectInfo.WritingSystems.ContainsKey("TPI")) //melanesian pidgin
				{
					NationalLanguageWritingSystemId = "TPI";
				}
				if (projectInfo.WritingSystems.ContainsKey("th")) //thai
				{
					NationalLanguageWritingSystemId = "th";
				}
				if (projectInfo.WritingSystems.ContainsKey("fr")) //french
				{
					NationalLanguageWritingSystemId = "fr";
				}
			}
		}

		[Browsable(true)]
		[Description(
				@"The raw output gives tags like \lx_bth, but you likely want to see simple tags like \lx. Enter from<space>to pairs, one per line. E.g. lx_bth lx"
				)]
		public string SfmTagConversions
		{
			get { return _sfmTagConversions; }
			set { _sfmTagConversions = value; }
		}

		public class ChangePair
		{
			private readonly string _from;
			public string to;
			public Regex regex;

			/*            public ChangePair( string sfrom, string sto)
			{
//                _from = @"(\W)*" + sfrom+ @"(\W)";
 //                to = "$1" + sto + " "; //put the spaces back in
			   _from = @"\\" + sfrom + @"\s+";
				to =  @"\\" + sto + " "; //put the spaces back in
				regex = new Regex(_from, RegexOptions.Compiled);
			}
 * */

			private ChangePair(string sfrom, string sto)
			{
				_from = sfrom;
				to = sto;

				//can throw exception
				regex = new Regex(_from, RegexOptions.Compiled);
			}

			public static ChangePair CreateFullMarkerReplacement(string fromMarkerNoSlash,
																 string toMarkerNoSlash)
			{
				string from = @"\\" + fromMarkerNoSlash + @"\s+";
				string to = @"\" + toMarkerNoSlash + " ";
				ChangePair p = new ChangePair(from, to);
				return p;
			}

			/// <summary>
			///
			/// </summary>
			/// <exception cref="ArgumentException">if the regex doesn't parse</exception>
			public static ChangePair CreateReplacementFromTweak(string from, string to)
			{
				ChangePair p = new ChangePair(from, to);
				return p;
			}
		}

		[XmlIgnore]
		public IEnumerable<ChangePair> ChangePairs
		{
			get
			{
				List<ChangePair> pairs = new List<ChangePair>();
				pairs.Add(ChangePair.CreateFullMarkerReplacement("BaseForm", "base"));
				pairs.Add(ChangePair.CreateFullMarkerReplacement(LexSense.WellKnownProperties.SemanticDomainDdp4, "sd"));
				pairs.Add(ChangePair.CreateFullMarkerReplacement("citation", "lc"));
				pairs.Add(ChangePair.CreateFullMarkerReplacement("definition", "d"));

				if (!String.IsNullOrEmpty(_vernacularLanguageWritingSystemId))
				{
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"lx_" + _vernacularLanguageWritingSystemId, "lx"));
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"lc_" + _vernacularLanguageWritingSystemId, "lc"));
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"x_" + _vernacularLanguageWritingSystemId, "xv"));
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"d_" + _vernacularLanguageWritingSystemId, "dv"));
				}

				if (!String.IsNullOrEmpty(_englishLanguageWritingSystemId))
				{
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"g_" + _englishLanguageWritingSystemId, "ge"));
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"d_" + _englishLanguageWritingSystemId, "de"));
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"x_" + _englishLanguageWritingSystemId, "xe"));
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"nt_" + _englishLanguageWritingSystemId, "nt"));
				}

				if (!String.IsNullOrEmpty(_nationalLanguageWritingSystemId))
				{
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"g_" + _nationalLanguageWritingSystemId, "gn"));
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"d_" + _nationalLanguageWritingSystemId, "dn"));
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"x_" + _nationalLanguageWritingSystemId, "xn"));
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"nt_" + _nationalLanguageWritingSystemId, "ntn"));
				}

				if (!String.IsNullOrEmpty(_regionalLanguageWritingSystemId))
				{
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"g_" + _regionalLanguageWritingSystemId, "gr"));
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"d_" + _regionalLanguageWritingSystemId, "dr"));
					pairs.Add(
							ChangePair.CreateFullMarkerReplacement(
									"x_" + _regionalLanguageWritingSystemId, "xr"));
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
							string[] parts = line.Split(new char[] {' '});
							if (parts.Length != 2)
							{
								//hmmmm
							}
							else
							{
								//                                string from=@"(\W)*" + parts[0] + @"(\W)"; //only match if bounded by white space
								//                                string to = "$1" + parts[1] + "$2";
								try
								{
									ChangePair p = ChangePair.CreateReplacementFromTweak(parts[0],
																						 parts[1]);

									pairs.Add(p);
								}
								catch (ArgumentException err)
								{
									ErrorReport.NotifyUserOfProblem(
											"Sorry, there is a problem in one of the tweaks for SFM export.  They must each be valid 'regular expressions'.  The error was: {0}",
											err.Message);
								}
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

		public string EnglishLanguageWritingSystemId
		{
			get { return _englishLanguageWritingSystemId; }
			set { _englishLanguageWritingSystemId = value; }
		}

		public string NationalLanguageWritingSystemId
		{
			get { return _nationalLanguageWritingSystemId; }
			set { _nationalLanguageWritingSystemId = value; }
		}

		public string RegionalLanguageWritingSystemId
		{
			get { return _regionalLanguageWritingSystemId; }
			set { _regionalLanguageWritingSystemId = value; }
		}

		public string VernacularLanguageWritingSystemId
		{
			get { return _vernacularLanguageWritingSystemId; }
			set { _vernacularLanguageWritingSystemId = value; }
		}
	}
}