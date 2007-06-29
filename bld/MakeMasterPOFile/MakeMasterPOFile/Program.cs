using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PoMaker
{
	//merge with something like: "msmerge -U br.po WeSay.pot"
	class Program
	{
		static Dictionary<string, List<string>> _entries = new Dictionary<string, List<string>>();
		private static bool _makeEnglishPo;


		static void Main(string[] args)
		{
			string rootDirectory = args[0];
			if (args.Length > 1)
			{
				_makeEnglishPo = args[1] == "-en";
			}
			ProcessXmlFiles(rootDirectory);
			ProcessSourceDirectory(Path.Combine(rootDirectory, "src"));

			//"POT-Creation-Date: 2007-06-29T03:32:46+07:00\n"
//"PO-Revision-Date: 2006-09-06 17:01+0700\n"

			Console.WriteLine(Resource.header, DateTime.UtcNow.ToString("s"));
			foreach (KeyValuePair<string, List<string>> pair in _entries)
			{
				WriteEntry(pair.Key, pair.Value);
			}
		}

		private static void ProcessXmlFiles(string rootDirectory)
		{
			string commonDir = Path.Combine(rootDirectory, "common");
			foreach (string filePath in Directory.GetFiles(commonDir, "*.WeSayConfig"))
			{
				ProcessXmlFile(filePath);
			}
		}

		private static void ProcessXmlFile(string filePath)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filePath);
			foreach(XmlNode node in doc.SelectNodes("//label | //description"))
			{
				AddStringInstance(node.InnerText, String.Empty);
			}
		}

		private static void AddStringInstance(string stringToTranslate, string commentsForTranslator)
		{
			if (!_entries.ContainsKey(stringToTranslate)) //first time we've encountered this string?
			{
				_entries.Add(stringToTranslate, new List<string>());
			}
			_entries[stringToTranslate].Add(commentsForTranslator);//add this reference
		}

		private static void ProcessSourceDirectory(string rootSourceDirectory)
		{
			foreach (string d in Directory.GetDirectories(rootSourceDirectory))
			{
				foreach (string filePath in Directory.GetFiles(d, "*.cs"))
				{
					if (!Path.GetFileName(filePath).ToLower().Contains("test"))
					{
						ProcessSrcFile(filePath);
					}
				}
				ProcessSourceDirectory(d);
			}
		}

		private static void ProcessSrcFile(string filePath)
		{
			string contents = File.ReadAllText(filePath);
			System.Text.RegularExpressions.Regex pattern =
			new System.Text.RegularExpressions.Regex(@"""~([^""]*)""\s*(,\s*""(.*)"")?", System.Text.RegularExpressions.RegexOptions.Compiled);

			foreach (System.Text.RegularExpressions.Match match in pattern.Matches(contents))
			{
				string str = match.Groups[1].Value;
				if (!_entries.ContainsKey(str)) //first time we've encountered this string?
				{
					_entries.Add(str, new List<string>());
				}
				string comments = "#; "+filePath;
				if (match.Groups.Count >= 3 && match.Groups[3].Length>0)
				{
					comments += System.Environment.NewLine + "#. " + match.Groups[3].Value;
				}
				_entries[str].Add(comments);//add this reference
			}
		}

		private static void WriteEntry(string key, List<string> comments)
		{
			Console.WriteLine("");
			foreach (string s in comments)
			{
				Console.WriteLine(s);
			}
			Console.WriteLine("msgid \"" + key + "\"");
			if (_makeEnglishPo)
			{
				string val = key;
				if (key == "fontFamily")
				{
					val = "Arial";
				}
				if (key == "fontSize")
				{
					val = "9";
				}
				Console.WriteLine("msgstr \"{0}\"", val);
			}
			else
			{
				Console.WriteLine("msgstr \"\"");
			}
		}

	}
}
