using System;
using System.Drawing;
using System.IO;

namespace WeSay.Foundation
{
	public class StringCatalog
	{
		private System.Collections.Specialized.StringDictionary _catalog;
		private static StringCatalog _singleton;
		private static Font _font;
		private static bool _inInternationalizationTestMode;

		/// <summary>
		/// Construct with no actual string file
		/// </summary>
		public StringCatalog(): this(String.Empty, 9)
		{
		}
		/// <summary>
		/// Construct with no actual string file
		/// </summary>
		public StringCatalog(string labelFontName, float labelFontSizeInPoints)
		{
			Init();
			SetupUIFont(labelFontName, labelFontSizeInPoints );
		}
		public StringCatalog(string pathToPoFile, string labelFontName, float labelFontSizeInPoints)
		{
			Init();
			_inInternationalizationTestMode = pathToPoFile == "test";
			if (!_inInternationalizationTestMode)
			{
				TextReader reader = (TextReader) File.OpenText(pathToPoFile);
				try
				{
					string id = null;
					string line = reader.ReadLine();
					while (line != null)
					{
						if (line.StartsWith("msgid"))
						{
							id = GetStringBetweenQuotes(line);
						}
						else if (line.StartsWith("msgstr") && id != null && id.Length > 0)
						{
							string s = GetStringBetweenQuotes(line);
							if (s.Length > 0)
							{
								_catalog.Add(id, s);
							}
							id = null;
						}

						line = reader.ReadLine();
					}
				}
				finally
				{
					reader.Close();
				}
			}

			SetupUIFont(labelFontName,  labelFontSizeInPoints);
		}

		private void SetupUIFont(string labelFontName, float labelFontSizeInPoints)
		{
			if (_inInternationalizationTestMode)
			{
				LabelFont = new Font("Comic Sans MS", 9);
				return;
			}

			LabelFont = new Font(FontFamily.GenericSansSerif, (float) 8.25, FontStyle.Regular);
			if(!String.IsNullOrEmpty(labelFontName ))
			{
				try
				{
					LabelFont = new Font(labelFontName, labelFontSizeInPoints, FontStyle.Regular);
				}
				catch (Exception)
				{
					Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
						"Could not find the requested UI font '{0}'.  Will use a generic font instead.",
						labelFontName);
				}
			}
		}

		public static string Get(string id)
		{
			return Get(id, String.Empty);
		}

		public static string Get(string id, string translationNotes)
		{
			if (!String.IsNullOrEmpty(id) && id[0] == '~')
			{
				id = id.Substring(1);
			}
			if (_singleton == null) //todo: this should not be needed
			{
				return id;
			}

			if (_inInternationalizationTestMode)
			{
				return "*"+_singleton[id];
			}
			else
			{
				return _singleton[id];
			}
		}


		private void Init()
		{
			_singleton = this;
			_catalog = new System.Collections.Specialized.StringDictionary();
		}

		private static string GetStringBetweenQuotes(string line)
		{
			int s = line.IndexOf('"');
			int f = line.LastIndexOf('"');
			return line.Substring(s + 1, f - (s + 1)).Trim();
		}

		public string this[string id]
		{
			get
			{
				string s = _catalog[id];
				if (s == null)
				{
					return id;
				}
				else
				{
					return s;
				}
			}
		}

		public static Font LabelFont
		{
			get
			{
				return _font;
			}
			set
			{
				_font = value;
			}
		}
		public static Font ModifyFontForLocalization(Font incoming)
		{
			float sBaseFontSizeInPoints = (float)8.25;
			float points = incoming.SizeInPoints + (StringCatalog.LabelFont.SizeInPoints- sBaseFontSizeInPoints);
			//float points = incoming.SizeInPoints * (StringCatalog.LabelFont.SizeInPoints / sBaseFontSizeInPoints);
			return new Font(StringCatalog.LabelFont.Name, points, incoming.Style);

		}
	}
}