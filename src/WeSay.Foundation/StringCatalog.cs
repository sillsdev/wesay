using System;
using System.Drawing;
using System.IO;

namespace WeSay.Language
{

	public class StringCatalog
	{
//        public static string this[string id]
//        {
//            get
//            {
//                string s = _singleton[id];
//                if (s == null)
//                    return id;
//                else
//                    return s;
//            }
//        }
//
		public static string Get(string id)
		{
			return Get(id, String.Empty);
		}

		public static string Get(string id, string translationNotes)
		{
			if (_singleton == null)//todo: this should not be needed
				return id;
			return _singleton[id];
		}

		private System.Collections.Specialized.StringDictionary _catalog;
		private static StringCatalog _singleton;
		private static Font _font;

		/// <summary>
		/// Construct with no actual string file
		/// </summary>
		public StringCatalog()
		{
			Init();
			SetupUIFont();
		}

		public StringCatalog(string pathToPoFile)
		{
			Init();

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
			SetupUIFont();

		}

		/// <summary>
		/// the ui font can be customized by providing 'translations' indicating font name and size
		/// </summary>
		private void SetupUIFont()
		{
			LabelFont = new Font("Comic Sans MS", 9);
			// _font = new Font(FontFamily.GenericMonospace, 12);

			string fontFamily = Get("_fontFamily");
			string fontSize = Get("_fontSize");
			if (fontFamily != "_fontFamily")
			{
				int sz = 9;
				if (fontSize != "_fontSize")
				{
					try
					{
						sz = Int32.Parse(fontSize);
					}
					catch (Exception)
					{
						Reporting.ErrorReporter.ReportNonFatalMessage("The localization initialization could not understand the font size '{0}', which should be an integer number", fontSize);
					}
				}
				try
				{
					LabelFont = new Font(fontFamily, sz);
				}
				catch (Exception)
				{
					Reporting.ErrorReporter.ReportNonFatalMessage("The localization initialization  could not create a font '{0}' of size '{1}'",
																  fontFamily, sz);
			   }
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
			return line.Substring(s + 1, f - (s+1)).Trim();
		}

		public string this[string id]
		{
			get
			{
				string s = _catalog[id];
				if (s == null)
					return id;
				else
					return s;
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
	}
}
