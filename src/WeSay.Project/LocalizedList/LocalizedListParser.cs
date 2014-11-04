using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Palaso.DictionaryServices.Model;
using Palaso.Lift;
using Palaso.Lift.Options;
using Palaso.Reporting;
using WeSay.LexicalModel;

namespace WeSay.Project.LocalizedList
{
	public class LocalizedListParser
	{
		private List<String> _keys;
		private Dictionary<string, List<string>> _questionDictionary;
		private MultiText _searchKeys;
		private OptionsList _optionsList;
		private bool _alreadyReportedWSLookupFailure;
		public string SemanticDomainWs { get; set; }
		public string ApplicationCommonDirectory { get; set; }
		public string PathToWeSaySpecificFilesDirectoryInProject { get; set; }
		private string _pathToList = "";
		public Dictionary<string, List<string>> QuestionDictionary
		{
			get { return _questionDictionary;  }
		}

		public List<String> Keys
		{
			get { return _keys; }
		}

		public OptionsList OptionsList
		{
			get { return _optionsList; }
		}
		public ViewTemplate ViewTemplate { get; set; }

		/// <summary>
		/// Read a Localized List file.
		/// </summary>
		public int ReadListFile()
		{
			_pathToList = GetListFileName(SemanticDomainWs);
			XmlTextReader reader = null;
			_keys = new List<string>();
			_questionDictionary = new Dictionary<string, List<string>>();
			_optionsList = new OptionsList();
			int retCount = 0;
			try
			{
				reader = new XmlTextReader(_pathToList);
				reader.MoveToContent();
				// True if parsing localized list
				if (reader.IsStartElement("Lists"))
				{
					reader.ReadToDescendant("List");
					while (reader.IsStartElement("List"))
					{
						string itemClass = reader.GetAttribute("itemClass").Trim();
						if (itemClass == "CmSemanticDomain")
						{
							retCount = ParsePossibilities(reader);
						}
						reader.ReadToFollowing("List");
					}
				}
				// true if parsing SemDom.xml
				else if (reader.IsStartElement("LangProject"))
				{
					retCount = ParsePossibilities(reader);
				}
				else
				{
					//what are we going to do when the file is bad?
					Logger.WriteEvent("Bad lists file format, expected Lists element: " + _pathToList );
					Debug.Fail("Bad lists file format, expected Lists element: " + _pathToList);
				}
			}
			catch (XmlException e)
			{
				// log this;
				Logger.WriteEvent("XMLException on parsing Semantic Domain file: " + _pathToList + " " + e.Message);
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}
			}
			return retCount;
		}
		private string GetListFileName(String ws)
		{
			string localizedListFilePath = "LocalizedLists-" + ws + ".xml";
			if (!File.Exists(localizedListFilePath))
			{
				string pathInProject =
					Path.Combine(
						PathToWeSaySpecificFilesDirectoryInProject,
						localizedListFilePath);
				if (File.Exists(pathInProject))
				{
					localizedListFilePath = pathInProject;
				}
				else
				{
					string pathInProgramDir = Path.Combine(ApplicationCommonDirectory,
														   localizedListFilePath);
					if (!File.Exists(pathInProgramDir))
					{
						string semDomFile = "SemDom.xml";
						if (File.Exists(semDomFile))
						{
							localizedListFilePath = semDomFile;
						}
						else
						{
							pathInProject =
								Path.Combine(
									PathToWeSaySpecificFilesDirectoryInProject,
									semDomFile);
							if (File.Exists(pathInProject))
							{
								localizedListFilePath = pathInProject;
							}
							else
							{
								pathInProgramDir = Path.Combine(ApplicationCommonDirectory,
									semDomFile);
								if (!File.Exists(pathInProgramDir))
								{

									throw new ApplicationException(
										string.Format(
											"Could not find the semanticDomainQuestions file {0}. Expected to find it at: {1} or {2}. The name of the file is influenced by the first enabled input system for the Semantic Domain Field.",
											localizedListFilePath,
											pathInProject,
											pathInProgramDir));
								}
								localizedListFilePath = pathInProgramDir;
							}
						}
					}
					else
					{
						localizedListFilePath = pathInProgramDir;
					}
				}
			}
			return localizedListFilePath;
		}
		private int ParsePossibilities(XmlTextReader reader)
		{
			int retCount = 0;
			int semanticDomainCount = 0;
			int semanticDomainEndCount = 0;
			try
			{
				if (!reader.ReadToDescendant("Possibilities"))
				{
					Debug.Fail("Bad format, no Possibilities");
				}
				reader.ReadToDescendant("CmSemanticDomain");
				Option option = new Option();
				_searchKeys = new MultiText();
				MultiText mtName = new MultiText();
				MultiText mtAbbreviation = new MultiText();
				MultiText mtDescription = new MultiText();
				String name = "";
				String abbreviation = "";
				List<String> questions = new List<string>();

				reader.MoveToContent();
				while (!reader.EOF)
				{
					if (reader.IsStartElement())
					{
						switch (reader.Name)
						{
							case "Name":
								mtName = ParseMultiStringElement(reader, false);
								name = mtName.GetFirstAlternative();
								break;
							case "Abbreviation":
								mtAbbreviation = ParseMultiStringElement(reader, false);
								abbreviation = mtAbbreviation.GetFirstAlternative();
								break;
							case "Description":
								mtDescription = ParseDescriptions(reader);
								break;
							case "Questions":
								XmlReader questionReader = reader.ReadSubtree();
								questions = ParseQuestions(questionReader);
								questionReader.Close();
								reader.ReadToFollowing("CmSemanticDomain");
								break;
							case "CmSemanticDomain":
								if (!(String.IsNullOrEmpty(name) || String.IsNullOrEmpty(abbreviation)))
								{
									CreateOptionEntry(abbreviation, name, questions, option,
										mtAbbreviation, mtName, mtDescription);
									retCount++;
									option = new Option();
									_searchKeys = new MultiText();
									mtName = new MultiText();
									mtAbbreviation = new MultiText();
									mtDescription = new MultiText();
									questions = new List<string>();
									name = "";
									abbreviation = "";
								}
								reader.Read();
								semanticDomainCount++;
								break;
							default:
								reader.Read();
								break;
						}
					}
					else
					{
						if (IsEndElement(reader, "Possibilities"))
						{
							break;
						}
						if (IsEndElement(reader, "CmSemanticDomain"))
						{
							semanticDomainEndCount++;
						}
						reader.Read();
					}
				}
				// Take care of last entry
				if (!(String.IsNullOrEmpty(name) || String.IsNullOrEmpty(abbreviation)))
				{
					CreateOptionEntry(abbreviation, name, questions, option,
						mtAbbreviation, mtName, mtDescription);
					retCount++;
				}

			}
			catch (XmlException e)
			{
				// log this;
				Logger.WriteEvent("XMLException on parsing Semantic Domain file: " + _pathToList + " " + e.Message);
			}
			return retCount;
		}

		private void CreateOptionEntry(string abbreviation, string name, List<string> questions, Option option, MultiText mtAbbreviation,
			MultiText mtName, MultiText mtDescription)
		{
			string key = abbreviation + " " + name;
			if (questions.Count == 0)
			{
				questions.Add(string.Empty);
			}
			_questionDictionary.Add(key, questions);
			_keys.Add(key);
			option.Key = key;
			option.Abbreviation = mtAbbreviation;
			option.Name = mtName;
			option.SearchKeys = _searchKeys;
			option.Description = mtDescription;
			_optionsList.Options.Add(option);
		}

		private MultiText ParseMultiStringElement(XmlReader reader, bool exampleWords)
		{
			var entry = new MultiText();
			reader.ReadToFollowing("AUni");
			while (reader.IsStartElement("AUni"))
			{
				string ws = reader["ws"];
				// Next Read reads text
				string tempString = reader.ReadElementString("AUni").Trim();
				if (!String.IsNullOrEmpty(tempString))
				{
					// Only add non empty strings to the multitext
					entry.SetAlternative(ws, tempString);
					if (exampleWords)
					{
						AppendSearchKey(ws, tempString);
					}
				}
			}
			return entry;
		}

		private void AppendSearchKey(string ws, string searchKeys)
		{
			string s = searchKeys.Trim().Replace('\n', ' ').Replace("  ", " ");
			s = s.TrimEnd(new char[] { ',', ' ' });//fieldworks has extra commas
			string existing = _searchKeys.GetExactAlternative(ws);
			if (existing != string.Empty)
			{
				existing += ", ";
			}
			_searchKeys.SetAlternative(ws, existing + s);

		}
		private List<String> ParseQuestions(XmlReader reader)
		{
			var questions = new List<string>();
			reader.ReadToFollowing("CmDomainQ");
			while (reader.IsStartElement("CmDomainQ"))
			{
				XmlReader cmDomainQReader = reader.ReadSubtree();
				cmDomainQReader.ReadToFollowing("Question");
				MultiText mtQuestion = ParseMultiStringElement(cmDomainQReader, false);
				string question = mtQuestion.GetBestAlternative(SemanticDomainWs);
				cmDomainQReader.ReadToFollowing("ExampleWords");
				MultiText mtExampleWords = ParseMultiStringElement(cmDomainQReader, true);
				string exampleWords = mtExampleWords.GetBestAlternative(SemanticDomainWs);
				if (!String.IsNullOrEmpty(question))
				{
					string formattedQuestion = "";
					if (!String.IsNullOrEmpty((exampleWords)))
					{
						formattedQuestion = question + " (" + exampleWords + ")";
					}
					else
					{
						formattedQuestion = question;
					}
					questions.Add(formattedQuestion);
				}
				cmDomainQReader.Close();
				reader.ReadToNextSibling("CmDomainQ");
			}
			return questions;
		}

		private MultiText ParseDescriptions(XmlReader reader)
		{
			var description = new MultiText();
			reader.ReadToFollowing("AStr");
			while (reader.IsStartElement("AStr"))
			{
				reader.ReadToFollowing("Run");
				string ws = reader["ws"];
				// Next Read reads text
				string tempString = reader.ReadElementString("Run").Trim();
				if (!String.IsNullOrEmpty(tempString))
				{
					// Only add non empty strings to the multitext
					description.SetAlternative(ws, tempString);
				}
				reader.ReadToNextSibling("AStr");
				reader.ReadToNextSibling("AStr");
			}
			return description;
		}

		private bool IsEndElement(XmlReader reader, String name)
		{
			bool retVal = false;
			if (reader.NodeType == XmlNodeType.EndElement)
			{
				if ((String.IsNullOrEmpty(name)) || (reader.Name == name))
				{
					retVal = true;
				}
			}
			return retVal;
		}
	}
}
