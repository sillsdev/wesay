using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Palaso.Reporting;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Options;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public class GatherBySemanticDomainTask: WordGatheringTaskBase
	{
		private readonly string _semanticDomainQuestionsFileName;
		private GatherBySemanticDomainsControl _gatherControl;
		private Dictionary<string, List<string>> _domainQuestions;
		private List<string> _domainKeys;
		private List<string> _domainNames;
		private List<string> _words;

		private WritingSystem _semanticDomainWritingSystem;
		private readonly Field _semanticDomainField;
		private OptionsList _semanticDomainOptionsList;

		private int _currentDomainIndex;
		private int _currentQuestionIndex;
		private bool _alreadyReportedWSLookupFailure = false;

		public GatherBySemanticDomainTask(LexEntryRepository lexEntryRepository,
										  string label,
										  string longLabel,
										  string description,
										  string remainingCountText,
										  string referenceCountText,
										  string semanticDomainQuestionsFileName,
										  ViewTemplate viewTemplate,
										  string semanticDomainFieldName)
				: base(label, longLabel, description, remainingCountText, referenceCountText, false, lexEntryRepository, viewTemplate)
		{
			if (semanticDomainQuestionsFileName == null)
			{
				throw new ArgumentNullException("semanticDomainQuestionsFileName");
			}
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}

			_currentDomainIndex = -1;
			_currentQuestionIndex = 0;
			_words = null;
			_semanticDomainQuestionsFileName =
					DetermineActualQuestionsFileName(semanticDomainQuestionsFileName);
			if (!File.Exists(_semanticDomainQuestionsFileName))
			{
				string pathInProject =
						Path.Combine(
								WeSayWordsProject.Project.PathToWeSaySpecificFilesDirectoryInProject,
								_semanticDomainQuestionsFileName);
				if (File.Exists(pathInProject))
				{
					_semanticDomainQuestionsFileName = pathInProject;
				}
				else
				{
					string pathInProgramDir =
							Path.Combine(WeSayWordsProject.ApplicationCommonDirectory,
										 _semanticDomainQuestionsFileName);
					if (!File.Exists(pathInProgramDir))
					{
						throw new ApplicationException(
								string.Format(
										"Could not find the semanticDomainQuestions file {0}. Expected to find it at: {1} or {2}. The name of the file is influenced by the first enabled writing system for the Semantic Domain Field.",
										_semanticDomainQuestionsFileName,
										pathInProject,
										pathInProgramDir));
					}
					_semanticDomainQuestionsFileName = pathInProgramDir;
				}
			}

			_semanticDomainField = viewTemplate.GetField(semanticDomainFieldName);
		}

		private string DetermineActualQuestionsFileName(string nameFromTaskConfiguration)
		{
			int extension = nameFromTaskConfiguration.LastIndexOf('-');
			if (extension == -1)
			{
				return nameFromTaskConfiguration;
			}

			string name = nameFromTaskConfiguration.Substring(0, extension + 1) +
						  WritingSystemIdForNamesAndQuestions +
						  Path.GetExtension(nameFromTaskConfiguration);
			return name;
		}

		/// <summary>
		/// The GatherWordListControl associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public override Control Control
		{
			get
			{
				VerifyTaskActivated();
				return _gatherControl;
			}
		}

		public List<string> DomainKeys
		{
			get
			{
				VerifyTaskActivated();
				return _domainKeys;
			}
		}

		public string CurrentDomainKey
		{
			get
			{
				VerifyTaskActivated();
				return DomainKeys[CurrentDomainIndex];
			}
		}

		public List<string> DomainNames
		{
			get
			{
				VerifyTaskActivated();

				if (_domainNames == null)
				{
					PopulateDomainNames();
				}
				return _domainNames;
			}
		}

		private void PopulateDomainNames()
		{
			_domainNames = new List<string>();
			foreach (string domainKey in _domainKeys)
			{
				_domainNames.Add(GetOptionNameFromKey(domainKey));
			}
		}

		public string CurrentDomainName
		{
			get
			{
				VerifyTaskActivated();
				return GetOptionNameFromKey(CurrentDomainKey);
			}
		}

		private string GetOptionNameFromKey(string key)
		{
			Option option = GetOptionFromKey(key);
			if (option == null)
			{
				return key;
			}
			return option.Name.GetExactAlternative(SemanticDomainWritingSystemId);
		}

		private Option GetOptionFromKey(string key)
		{
			return
					_semanticDomainOptionsList.Options.Find(
							delegate(Option o) { return o.Key == key; });
		}

		public string CurrentDomainDescription
		{
			get
			{
				VerifyTaskActivated();

				Option option = GetOptionFromKey(CurrentDomainKey);
				if (option == null)
				{
					return string.Empty;
				}
				return option.Description.GetExactAlternative(SemanticDomainWritingSystemId);
			}
		}

		public int CurrentDomainIndex
		{
			get
			{
				VerifyTaskActivated();
				return _currentDomainIndex;
			}
			set
			{
				VerifyTaskActivated();
				if (value < 0 || value >= DomainKeys.Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (value != _currentDomainIndex)
				{
					_currentDomainIndex = value;
					_currentQuestionIndex = 0;
					_words = null;
				}
			}
		}

		private void UpdateCurrentWords()
		{
			_words = null;
		}

		public string CurrentQuestion
		{
			get
			{
				VerifyTaskActivated();
				return Questions[CurrentQuestionIndex];
			}
		}

		public int CurrentQuestionIndex
		{
			get
			{
				VerifyTaskActivated();
				return _currentQuestionIndex;
			}
			set
			{
				VerifyTaskActivated();
				if (value < 0 || value >= Questions.Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				_currentQuestionIndex = value;
			}
		}

		public List<string> Questions
		{
			get
			{
				VerifyTaskActivated();
				return _domainQuestions[_domainKeys[_currentDomainIndex]];
			}
		}

		public int WordsInDomain(int domainIndex)
		{
			VerifyTaskActivated();
			if (domainIndex < 0 || domainIndex >= _domainKeys.Count)
			{
				throw new ArgumentOutOfRangeException();
			}

			int beginIndex;
			int pastEndIndex;
			GetWordsIndexes(GetAllEntriesSortedBySemanticDomain(),
							domainIndex,
							out beginIndex,
							out pastEndIndex);
			return pastEndIndex - beginIndex;
		}

		public List<string> CurrentWords
		{
			get
			{
				VerifyTaskActivated();

				if (_words == null)
				{
					_words = new List<string>();
					ResultSet<LexEntry> recordTokens = GetAllEntriesSortedBySemanticDomain();
					int beginIndex;
					int pastEndIndex;
					GetWordsIndexes(recordTokens,
									CurrentDomainIndex,
									out beginIndex,
									out pastEndIndex);
					for (int i = beginIndex;i < pastEndIndex;i++)
					{
						LexEntry entry = recordTokens[i].RealObject;
						_words.Add(entry.LexicalForm.GetBestAlternative(WordWritingSystemId, "*"));
					}
				}
				_words.Sort(WordWritingSystem);
				return _words;
			}
		}

		public bool HasNextDomainQuestion
		{
			get
			{
				VerifyTaskActivated();

				if (_currentDomainIndex < DomainKeys.Count - 1)
				{
					return true; // has another domain
				}

				if (_currentQuestionIndex < Questions.Count - 1)
				{
					return true; // has another question
				}

				return false;
			}
		}

		public void GotoNextDomainQuestion()
		{
			VerifyTaskActivated();

			if (_currentQuestionIndex == Questions.Count - 1)
			{
				if (_currentDomainIndex < DomainKeys.Count - 1)
				{
					_currentDomainIndex++;
					_currentQuestionIndex = 0;
				}
			}
			else
			{
				_currentQuestionIndex++;
			}
			UpdateCurrentWords();
		}

		public bool HasPreviousDomainQuestion
		{
			get
			{
				VerifyTaskActivated();

				if (_currentDomainIndex == 0 && _currentQuestionIndex == 0)
				{
					return false;
				}

				return true;
			}
		}

		public string SemanticDomainWritingSystemId
		{
			get
			{
				VerifyTaskActivated();
				return _semanticDomainWritingSystem.Id;
			}
		}

		public WritingSystem SemanticDomainWritingSystem
		{
			get
			{
				VerifyTaskActivated();
				return _semanticDomainWritingSystem;
			}
		}

		public void GotoPreviousDomainQuestion()
		{
			VerifyTaskActivated();

			if (_currentQuestionIndex != 0)
			{
				_currentQuestionIndex--;
			}
			else
			{
				if (_currentDomainIndex != 0)
				{
					_currentDomainIndex--;
					_currentQuestionIndex = Questions.Count - 1;
				}
			}
			UpdateCurrentWords();
		}

		public void AddWord(string lexicalForm)
		{
			VerifyTaskActivated();

			if (lexicalForm == null)
			{
				throw new ArgumentNullException();
			}
			if (lexicalForm != string.Empty)
			{
				ResultSet<LexEntry> recordTokens =
						LexEntryRepository.GetEntriesWithMatchingLexicalForm(lexicalForm,
																			 WordWritingSystem);
				if (recordTokens.Count == 0)
				{
					LexEntry entry = LexEntryRepository.CreateItem();
					entry.LexicalForm.SetAlternative(WordWritingSystemId, lexicalForm);
					AddCurrentSemanticDomainToEntry(entry);
					LexEntryRepository.SaveItem(entry);
					GetAllEntriesSortedBySemanticDomain();
				}
				else
				{
					foreach (RecordToken<LexEntry> recordToken in recordTokens)
					{
						AddCurrentSemanticDomainToEntry(recordToken.RealObject);
					}
				}
			}

			UpdateCurrentWords();
		}

		public void DetachFromMatchingEntries(string lexicalForm)
		{
			VerifyTaskActivated();

			if (lexicalForm == null)
			{
				throw new ArgumentNullException();
			}
			if (lexicalForm != string.Empty)
			{
				// this task was coded to have a list of word-forms, not actual entries.
				//so we have to go searching for possible matches at this point.
				ResultSet<LexEntry> matchingEntries =
						LexEntryRepository.GetEntriesWithMatchingLexicalForm(lexicalForm,
																			 WordWritingSystem);
				foreach (RecordToken<LexEntry> recordToken in matchingEntries)
				{
					DisassociateCurrentSemanticDomainFromEntry(recordToken); // might remove senses
				}
			}

			UpdateCurrentWords();
		}

		private void DisassociateCurrentSemanticDomainFromEntry(RecordToken<LexEntry> recordToken)
		{
			// have to iterate through these in reverse order
			// since they might get modified
			LexEntry entry = recordToken.RealObject;
			for (int i = entry.Senses.Count - 1;i >= 0;i--)
			{
				LexSense sense = entry.Senses[i];
				OptionRefCollection semanticDomains =
						sense.GetProperty<OptionRefCollection>(_semanticDomainField.FieldName);
				if (semanticDomains != null)
				{
					semanticDomains.Remove(CurrentDomainKey);
				}
			}
			entry.CleanUpAfterEditting();
			if (entry.IsEmptyExceptForLexemeFormForPurposesOfDeletion)
			{
				LexEntryRepository.DeleteItem(entry); // if there are no senses left, get rid of it
			}
			else
			{
				LexEntryRepository.SaveItem(entry);
			}
		}

		private void AddCurrentSemanticDomainToEntry(LexEntry entry)
		{
			LexSense sense = entry.GetOrCreateSenseWithMeaning(new MultiText());
			OptionRefCollection semanticDomains =
					sense.GetOrCreateProperty<OptionRefCollection>(_semanticDomainField.FieldName);
			if (!semanticDomains.Contains(CurrentDomainKey))
			{
				semanticDomains.Add(CurrentDomainKey);
			}
			LexEntryRepository.SaveItem(entry);
		}

		private void GetWordsIndexes(ResultSet<LexEntry> recordTokens,
									 int domainIndex,
									 out int beginIndex,
									 out int pastEndIndex)
		{
			string domainKey = DomainKeys[domainIndex];

			beginIndex = recordTokens.FindFirstIndex(delegate(RecordToken<LexEntry> token)
													 {
														 return (string) token["SemanticDomain"] == domainKey;
													 });
			if (beginIndex < 0)
			{
				pastEndIndex = beginIndex;
				return;
			}
			pastEndIndex = beginIndex + 1;
			while (pastEndIndex < recordTokens.Count &&
				   (string)recordTokens[pastEndIndex]["SemanticDomain"] == domainKey)
			{
				++pastEndIndex;
			}
		}

		private void ParseSemanticDomainFile()
		{
			XmlTextReader reader = null;
			try
			{
				reader = new XmlTextReader(_semanticDomainQuestionsFileName);
				reader.MoveToContent();
				if (!reader.IsStartElement("semantic-domain-questions"))
				{
					//what are we going to do when the file is bad?
					Debug.Fail("Bad file format, expected semantic-domain-questions element");
				}
				//string ws = reader.GetAttribute("lang"); got it from the configuration file

				// should verify that this writing system is in optionslist
				_semanticDomainWritingSystem =
						BasilProject.Project.WritingSystems[WritingSystemIdForNamesAndQuestions];
				string semanticDomainType = reader.GetAttribute("semantic-domain-type");
				// should verify that domain type matches type of optionList in semantic domain field

				reader.ReadToDescendant("semantic-domain");
				while (reader.IsStartElement("semantic-domain"))
				{
					string domainKey = reader.GetAttribute("id");
					List<string> questions = new List<string>();
					XmlReader questionReader = reader.ReadSubtree();
					questionReader.MoveToContent();
					questionReader.ReadToFollowing("question");
					while (questionReader.IsStartElement("question"))
					{
						questions.Add(questionReader.ReadElementString("question"));
					}
					_domainKeys.Add(domainKey);
					if (questions.Count == 0)
					{
						questions.Add(string.Empty);
					}
					_domainQuestions.Add(domainKey, questions);
					reader.ReadToFollowing("semantic-domain");
				}
			}
			catch (XmlException)
			{
				// log this;
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}
			}
		}

		private string WritingSystemIdForNamesAndQuestions
		{
			get
			{
				string ws = "en";
				try
				{
					ws =
							ViewTemplate.GetField(LexSense.WellKnownProperties.SemanticDomainsDdp4).
									WritingSystemIds[0];
				}
				catch (Exception)
				{
					if (!_alreadyReportedWSLookupFailure)
					{
						_alreadyReportedWSLookupFailure = true;
						ErrorReport.ReportNonFatalMessage(
								"WeSay was unable to get a writing system to use from the configuration Semantic Domain Field. English will be used.");
					}
				}
				return ws;
			}
		}

		public override void Activate()
		{
			base.Activate();
			if (DomainKeys == null)
			{
				_domainKeys = new List<string>();
				_domainQuestions = new Dictionary<string, List<string>>();
				ParseSemanticDomainFile();

				// always have at least one domain and one question
				// so default indexes of 0 are valid.
				if (_domainKeys.Count == 0)
				{
					_domainKeys.Add(string.Empty);
				}
				if (_domainQuestions.Count == 0)
				{
					List<string> emptyList = new List<string>();
					emptyList.Add(string.Empty);
					_domainQuestions.Add(string.Empty, emptyList);
				}
			}
			if (_semanticDomainOptionsList == null)
			{
				_semanticDomainOptionsList =
						WeSayWordsProject.Project.GetOptionsList(_semanticDomainField, false);
			}

			UpdateCurrentWords();
			if (CurrentDomainIndex == -1)
			{
				GotoLastDomainWithAnswers();
			}
			_gatherControl = new GatherBySemanticDomainsControl(this);
		}

		private ResultSet<LexEntry> GetAllEntriesSortedBySemanticDomain()
		{
			return
					LexEntryRepository.GetEntriesWithSemanticDomainSortedBySemanticDomain(
							_semanticDomainField.FieldName);
		}

		public override void Deactivate()
		{
			// get the counts cached before we deactivate
			GetRemainingCount();
			GetReferenceCount();
			base.Deactivate();
			_gatherControl.Dispose();
			_gatherControl = null;
			UpdateCurrentWords(); // clears out orphan records
		}

		protected override int ComputeCount(bool returnResultEvenIfExpensive)
		{
			if (!IsActive)
			{
				return CountNotComputed;
			}
			int remainingCount = DomainKeys.Count;

			string lastDomain = null;
			foreach (RecordToken<LexEntry> token in GetAllEntriesSortedBySemanticDomain())
			{
				string semanticDomain = (string)token["SemanticDomain"];
				if (semanticDomain != lastDomain)
				{
					lastDomain = semanticDomain;
					remainingCount--;
				}
			}

			return remainingCount;
		}

		protected override int ComputeReferenceCount()
		{
			if (!IsActive)
			{
				return CountNotComputed;
			}
			return DomainKeys.Count;
		}

		public void GotoLastDomainWithAnswers()
		{
			VerifyTaskActivated();
			for (int i = 0;i < DomainKeys.Count;i++)
			{
				CurrentDomainIndex = i;
				int beginIndex;
				int pastEndIndex;
				GetWordsIndexes(GetAllEntriesSortedBySemanticDomain(),
								CurrentDomainIndex,
								out beginIndex,
								out pastEndIndex);
				if (pastEndIndex == beginIndex)
				{
					CurrentDomainIndex = (i == 0) ? i : i - 1;
					return;
				}
			}
			// there were no empty domains. Stay at the last domain (as a side effect of having positioned
			// ourself in the above loop
		}
	}
}