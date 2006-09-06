using System.IO;

namespace WeSay.UI
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
			return _singleton[id];
		}

		private System.Collections.Specialized.StringDictionary _catalog;
		private static StringCatalog _singleton;

		/// <summary>
		/// Construct with no actual string file
		/// </summary>
		public StringCatalog()
		{
			Init();
		}

		public StringCatalog(string pathToPoFile)
		{
			Init();

			TextReader reader =  (TextReader)File.OpenText(pathToPoFile);
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

		private void Init()
		{
			_singleton = this;
			_catalog = new System.Collections.Specialized.StringDictionary();
		}

		private string GetStringBetweenQuotes(string line)
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
	}
}
