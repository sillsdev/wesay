using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Palaso.Data;
using Palaso.Misc;
using Palaso.Reporting;
using Palaso.Text;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Data;
using Palaso.LexicalModel;
using Palaso.LexicalModel.Options;
using WeSay.LexicalModel;
using Palaso.LexicalModel;
using Palaso.LexicalModel.Options;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;
using System.Linq;

namespace WeSay.LexicalTools.GatherBySemanticDomains
{
	public class GatherBySemanticDomainTask: WordGatheringTaskBase
	{
		internal const string DomainIndexTaskMemoryKey = "DomainIndex";
		internal const string QuestionIndexTaskMemoryKey= "QuestionIndex";
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
		private bool _alreadyReportedWSLookupFailure;
		private TaskMemory _taskMemory;
		private GatherBySemanticDomainConfig _config;
		private readonly ILogger _logger;
		public WritingSystem DefinitionWritingSystem { get; set; }

		public GatherBySemanticDomainTask(GatherBySemanticDomainConfig config,
										  LexEntryRepository lexEntryRepository,
										  ViewTemplate viewTemplate,
										TaskMemoryRepository taskMemoryRepository,
										ILogger logger)
			: base(
			   config,
				lexEntryRepository,
				viewTemplate, taskMemoryRepository)
		{
			_config = config;
			_logger = logger;
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (string.IsNullOrEmpty(config.semanticDomainsQuestionFileName))
			{
				throw new ArgumentNullException("config.semanticDomainsQuestionFileName");
			}
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}

			_taskMemory = taskMemoryRepository.FindOrCreateSettingsByTaskId(config.TaskName);


			_currentDomainIndex = -1;
			_currentQuestionIndex = 0;
			_words = null;
			_semanticDomainQuestionsFileName =
				DetermineActualQuestionsFileName(config.semanticDomainsQuestionFileName);
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
					string pathInProgramDir = Path.Combine(BasilProject.ApplicationCommonDirectory,
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

			_semanticDomainField = viewTemplate.GetField(LexSense.WellKnownProperties.SemanticDomainDdp4);
			var definitionWsId= viewTemplate.GetField(LexSense.WellKnownProperties.Definition).WritingSystemIds.First();
			WritingSystem definitionWS;
			viewTemplate.WritingSystems.TryGetValue(definitionWsId, out definitionWS);
			Guard.AgainstNull(definitionWS, "Defintion Writing System");
			DefinitionWritingSystem = definitionWS;

			EnsureQuestionsFileExists();//we've added this paranoid code because of ws-1156
		}


//        /// <summary>
//        /// for old unit tests
//        /// </summary>
//        /// <param name="semanticDomainsQuestionFileName"></param>
//        /// <param name="lexEntryRepository"></param>
//        /// <param name="viewTemplate"></param>
//        public GatherBySemanticDomainTask(string semanticDomainsQuestionFileName, LexEntryRepository lexEntryRepository, ViewTemplate viewTemplate)
//            : this(GatherBySemanticDomainConfig.CreateForTests(semanticDomainsQuestionFileName),
//                    lexEntryRepository,
//                    viewTemplate, null, new StringLogger())
//        {
//
//        }

		private void EnsureQuestionsFileExists()
		{
			if (!File.Exists(_semanticDomainQuestionsFileName))
			{
				throw new ApplicationException(
					string.Format(
						"Could not find the semanticDomainQuestions file {0}.",
						_semanticDomainQuestionsFileName));
			}
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
			string prefix = "";
			string number = option.Abbreviation.GetBestAlternativeString(new string[]{SemanticDomainWritingSystemId, "en"});
			var indentLevel = 0;
			if (!string.IsNullOrEmpty(number))
			{
				foreach (char c in number.ToCharArray())
				{
					if (c == '.')
						indentLevel++;
				}
				prefix = "      ".Substring(0, indentLevel) + number + " ";
			}
			return prefix //this puts the number back in
				+ option.Name.GetBestAlternativeString(new string[] { SemanticDomainWritingSystemId, "en" });
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
				RecordLocationInTaskMemory();
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

		public string Reminder
		{
			get
			{
				return StringCatalog.Get("~Do not just translate these words; instead think of words in your own language.",
										 "This is shown in the Gather By Semantic Domains task after each semantic domain question. The questions contain example words to help clarify what the domain is about, and this was added to remind the user that he/she is gathering based on the domain, not these examples.");
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
				RecordLocationInTaskMemory();
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
#if OLD
				This code took you literally to the next one

				if (_currentDomainIndex < DomainKeys.Count - 1)
				{
					CurrentDomainIndex = _currentDomainIndex+1;
					_currentQuestionIndex = 0;
				}
#endif
				GotoNextDomainLackingAnswers();
			}
			else
			{
				_currentQuestionIndex++;
			}
		  RecordLocationInTaskMemory();
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
				if (_semanticDomainWritingSystem == null)
					return string.Empty;//happens during some unrelated tests
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

		/// <summary>
		/// Add the word, gloss, and current semantic domain, somehow, to the lexicon
		/// </summary>
		/// <returns>the affected words, to help unit tests check what was done</returns>
		public IList<LexEntry> AddWord(string lexicalForm)
		{
			return AddWord(lexicalForm, string.Empty);
		}

		public IList<LexEntry> AddWord(string lexicalForm, string gloss)
		{
			VerifyTaskActivated();

			if (string.IsNullOrEmpty(lexicalForm))
			{
				throw new ArgumentNullException();
			}
			var modifiedEntries = new List<LexEntry>();
			if (lexicalForm != string.Empty)
			{
				ResultSet<LexEntry> recordTokens =
					LexEntryRepository.GetEntriesWithMatchingLexicalForm(lexicalForm,
																		 WordWritingSystem);
				if (recordTokens.Count == 0)//no entries with a matching form
				{
					LexEntry entry = LexEntryRepository.CreateItem();
					entry.LexicalForm.SetAlternative(WordWritingSystemId, lexicalForm);
					AddCurrentSemanticDomainToEntry(entry,gloss);
					LexEntryRepository.SaveItem(entry);
					modifiedEntries.Add(entry);

					_logger.WriteConciseHistoricalEvent("SD-Added '{0}' with Domain to '{1}'", entry.GetSimpleFormForLogging(), CurrentDomainName);
				}
				else// one or more matching entries
				{
					var entriesMatchingWord = new List<LexEntry>(from RecordToken<LexEntry> x in recordTokens select x.RealObject);
					foreach (var entry in entriesMatchingWord)
					{
						if(HasMatchingGloss(entry, gloss))
						{
							modifiedEntries.Add(entry);
							AddCurrentSemanticDomainToEntry(entry, gloss);
							_logger.WriteConciseHistoricalEvent("SD-Added Domain to '{0}'", entry.GetSimpleFormForLogging());
							break;
						}
					}
					if (modifiedEntries.Count == 0) //didn't find any matching glosses
					{
						//NB: what to do IS NOT CLEAR. This just adds to the first entry,
						// but it's just rolling the dice!  What to do???
						var first = entriesMatchingWord.First();
						modifiedEntries.Add(first);
						AddCurrentSemanticDomainToEntry(first, gloss);
						_logger.WriteConciseHistoricalEvent("SD-Added Domain {0} to '{1}' REVIEW", CurrentDomainName, first.GetSimpleFormForLogging());
					}
				}
			}

			UpdateCurrentWords();
			return modifiedEntries;
		}

		private bool HasMatchingGloss(LexEntry entry, string gloss)
		{
			return null != entry.Senses.FirstOrDefault(s => s.Definition.ContainsEqualForm(gloss, DefinitionWritingSystem.Id));
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

		private void AddCurrentSemanticDomainToEntry(LexEntry entry, string gloss)
		{
			LexSense sense = null;
			//is the gloss empty? THen just ggrab the first sense
			if (string.IsNullOrEmpty(gloss))
			{
				sense = entry.Senses.FirstOrDefault();
			}
			else
			{
				//is there a sense with a matching gloss?
				sense = entry.Senses.FirstOrDefault(
					s => s.Definition.ContainsEqualForm(gloss, DefinitionWritingSystem.Id));
			}
			if(sense==null)
			{
				sense = entry.GetOrCreateSenseWithMeaning(new MultiText());
				sense.Definition.SetAlternative(DefinitionWritingSystem.Id, gloss);
			}
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

			beginIndex =
				recordTokens.FindFirstIndex(
					delegate(RecordToken<LexEntry> token) { return (string) token["SemanticDomain"] == domainKey; });
			if (beginIndex < 0)
			{
				pastEndIndex = beginIndex;
				return;
			}
			pastEndIndex = beginIndex + 1;
			while (pastEndIndex < recordTokens.Count &&
				   (string) recordTokens[pastEndIndex]["SemanticDomain"] == domainKey)
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
					string domainKey = reader.GetAttribute("id").Trim();
					List<string> questions = new List<string>();
					XmlReader questionReader = reader.ReadSubtree();
					questionReader.MoveToContent();
					questionReader.ReadToFollowing("question");
					while (questionReader.IsStartElement("question"))
					{
						questions.Add(questionReader.ReadElementString("question").Trim());
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
						ViewTemplate.GetField(LexSense.WellKnownProperties.SemanticDomainDdp4).
							WritingSystemIds[0];
				}
				catch (Exception)
				{
					if (!_alreadyReportedWSLookupFailure)
					{
						_alreadyReportedWSLookupFailure = true;
						ErrorReport.NotifyUserOfProblem(
							"WeSay was unable to get a writing system to use from the configuration Semantic Domain Field. English will be used.");
					}
				}
				return ws;
			}
		}

		public bool CanGoToNext
		{
			get
			{
				return true;
				// this would stop us at the end, but we now loop around: HasNextDomainQuestion
			}
		}

		public bool ShowDefinitionField
		{
			get { return _config.ShowMeaningField; }
		}

		public override void Activate()
		{
			EnsureQuestionsFileExists();//we've added this paranoid code because of ws-1156

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
			ReadTaskMemory();

			UpdateCurrentWords();
			if (CurrentDomainIndex == -1)
			{//this is probably never (or rarely?) encountered now that we have task memory
				GotoLastDomainWithAnswers();
			}
			_gatherControl = new GatherBySemanticDomainsControl(this);
		}


		private void RecordLocationInTaskMemory()
		{
			_taskMemory.Set(DomainIndexTaskMemoryKey, _currentDomainIndex);
			_taskMemory.Set(QuestionIndexTaskMemoryKey, _currentQuestionIndex);
		}

		private void ReadTaskMemory()
		{
			if (_taskMemory == null)
				return;

			var domainIndexString = _taskMemory.Get(DomainIndexTaskMemoryKey, null);
			var questionIndexString = _taskMemory.Get(QuestionIndexTaskMemoryKey, null);
			int x;
			if (int.TryParse(domainIndexString, out x))
			{
				if (x >= 0 && x < DomainKeys.Count)
				{
					_currentDomainIndex = x;
					if (int.TryParse(questionIndexString, out x))
					{
						if (x >= 0 && x < Questions.Count)
						{
							_currentQuestionIndex = x;
						}
					}
				}
			}
		}

		private ResultSet<LexEntry> GetAllEntriesSortedBySemanticDomain()
		{
			return
				LexEntryRepository.GetEntriesWithSemanticDomainSortedBySemanticDomain(
					_semanticDomainField.FieldName);
		}

		public override void Deactivate()
		{
			if(_gatherControl != null)
			{
				_gatherControl.Cleanup();
			}
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
				string semanticDomain = (string) token["SemanticDomain"];
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

		public void GotoNextDomainLackingAnswers()
		{
			VerifyTaskActivated();
			var entries = GetAllEntriesSortedBySemanticDomain();
			for (int i = _currentDomainIndex+1; i < DomainKeys.Count; i++)
			{
				if (GetHasWords(i, entries))
				{
					CurrentDomainIndex = i;
					return;
				}
			}
			//wrap around from the beginning.  the <= here ensures that we land on the current one, if it is empty
			for (int i = 0; i <= _currentDomainIndex; i++)
			{
				if (GetHasWords(i, entries))
				{
					CurrentDomainIndex = i;
					return;
				}
			}
			//all domains filled, including the current one
			CurrentDomainIndex = DomainKeys.Count - 1;
		}

		private bool GetHasWords(int domainIndex, ResultSet<LexEntry> entries)
		{
				int beginIndex;
				int pastEndIndex;
				GetWordsIndexes(entries,
								domainIndex,
								out beginIndex,
								out pastEndIndex);
			return (pastEndIndex == beginIndex);
		}


	}
}
