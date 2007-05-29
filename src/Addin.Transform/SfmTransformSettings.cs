using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace Addin.Transform
{
	[Serializable]
	public class SfmTransformSettings
	{
		private string _sfmTagConversions;

		public SfmTransformSettings()
		{
			SfmTagConversions = "g_en ge";
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
		}

		[XmlIgnore]
		public IEnumerable<SfmTransformSettings.ChangePair> ChangePairs
		{
			get
			{
				List<ChangePair> pairs = new List<ChangePair>();
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

				return pairs;
			}
		}
	}
}