using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MakeMasterPOFile
{
	//merge with something like: "msmerge -U br.po WeSay.pot"
	class Program
	{
		static Dictionary<string, List<string>> _entries = new Dictionary<string, List<string>>();


		static void Main(string[] args)
		{
			string rootDirectory = args[0];
			ProcessDirectory(rootDirectory);

			Console.WriteLine(Resource.header);
			foreach (KeyValuePair<string, List<string>> pair in _entries)
			{
				WriteEntry(pair.Key, pair.Value);
			}
		}

		private static void ProcessDirectory(string rootDirectory)
		{
			foreach (string d in Directory.GetDirectories(rootDirectory))
			{
				foreach (string filePath in Directory.GetFiles(d,"*.cs"))
				{
					if(!Path.GetFileName(filePath).ToLower().Contains("test"))
					{
						ProcessFile(filePath);
					}
				}
				ProcessDirectory(d);
			}
		}

		private static void ProcessFile(string filePath)
		{
			string contents = File.ReadAllText(filePath);
			System.Text.RegularExpressions.Regex pattern =
//                new System.Text.RegularExpressions.Regex("StringCatalog.Get\\(\"(.*)\"\\)", System.Text.RegularExpressions.RegexOptions.Compiled);
				new System.Text.RegularExpressions.Regex("\"~(.*)\"", System.Text.RegularExpressions.RegexOptions.Compiled);

			foreach (System.Text.RegularExpressions.Match  match in pattern.Matches(contents))
			{
				string str = match.Groups[1].Value;
				if(!_entries.ContainsKey(str)) //first time we've encountered this string?
				{
					_entries.Add(str, new List<string>());
				}
				_entries[str].Add(filePath);//add this reference
			}
		}

		private static void WriteEntry(string key, List<string> references)
		{
			Console.WriteLine("");
			foreach (string s in references)
			{
				Console.WriteLine("#: " + s);
			}
			Console.WriteLine("msgid \"" + key + "\"");
			Console.WriteLine("msgstr \"\"");
		}
	}
}
