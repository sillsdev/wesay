<ClassProject>
  <Language>CSharp</Language>
  <Entities>
	<Entity type="Class">
	  <Name>LiftPreparer</Name>
	  <Access>Internal</Access>
	  <Member type="Method">internal Void PopulateDefinitions(ProgressState state)</Member>
	  <Member type="Method">internal String PopulateDefinitions(String pathToLift)</Member>
	  <Member type="Method">public Boolean IsMigrationNeeded()</Member>
	  <Member type="Method">public Void MigrateLiftFile(ProgressState state)</Member>
	  <Member type="Constructor">public LiftPreparer(String liftFilePath)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>LiftRepository</Name>
	  <Access>Internal</Access>
	  <Member type="Property">public virtual Boolean CanPersist { get; }</Member>
	  <Member type="Property">private String LiftDirectory { get; }</Member>
	  <Member type="Property">public Boolean IsLiftFileLocked { get; }</Member>
	  <Member type="Method">public override Void Startup(ProgressState state)</Member>
	  <Member type="Method">public override Void DeleteItem(RepositoryId id)</Member>
	  <Member type="Method">public override Void DeleteItem(LexEntry item)</Member>
	  <Member type="Method">public override Void SaveItem(LexEntry item)</Member>
	  <Member type="Method">public override Void SaveItems(IEnumerable&lt;LexEntry&gt; items)</Member>
	  <Member type="Method">public override Void Dispose()</Member>
	  <Member type="Method">public Boolean MergeIncrementFiles()</Member>
	  <Member type="Constructor">public LiftRepository(String filePath)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>LiftMerger</Name>
	  <Access>Internal</Access>
	  <Member type="Event">public event EventHandler&lt;EntryCreatedEventArgs&gt; EntryCreatedEvent</Member>
	  <Member type="Property">public IList&lt;String&gt; ExpectedOptionTraits { get; }</Member>
	  <Member type="Property">public IList&lt;String&gt; ExpectedOptionCollectionTraits { get; }</Member>
	  <Member type="Method">public sealed override LexEntry GetOrMakeEntry(Extensible eInfo, Int32 order)</Member>
	  <Member type="Method">public sealed override Void EntryWasDeleted(Extensible info, DateTime dateDeleted)</Member>
	  <Member type="Method">public sealed override LexSense GetOrMakeSense(LexEntry entry, Extensible eInfo, String rawXml)</Member>
	  <Member type="Method">public sealed override LexSense GetOrMakeSubsense(LexSense sense, Extensible info, String rawXml)</Member>
	  <Member type="Method">public sealed override LexExampleSentence GetOrMakeExample(LexSense sense, Extensible eInfo)</Member>
	  <Member type="Method">public sealed override Void MergeInLexemeForm(LexEntry entry, LiftMultiText forms)</Member>
	  <Member type="Method">public sealed override Void MergeInCitationForm(LexEntry entry, LiftMultiText contents)</Member>
	  <Member type="Method">public sealed override WeSayDataObject MergeInPronunciation(LexEntry entry, LiftMultiText contents, String rawXml)</Member>
	  <Member type="Method">public sealed override WeSayDataObject MergeInVariant(LexEntry entry, LiftMultiText contents, String rawXml)</Member>
	  <Member type="Method">public sealed override Void MergeInGloss(LexSense sense, LiftMultiText forms)</Member>
	  <Member type="Method">public sealed override Void MergeInExampleForm(LexExampleSentence example, LiftMultiText forms)</Member>
	  <Member type="Method">public sealed override Void MergeInTranslationForm(LexExampleSentence example, String type, LiftMultiText forms, String rawXml)</Member>
	  <Member type="Method">public sealed override Void MergeInSource(LexExampleSentence example, String source)</Member>
	  <Member type="Method">public sealed override Void MergeInDefinition(LexSense sense, LiftMultiText contents)</Member>
	  <Member type="Method">public sealed override Void MergeInPicture(LexSense sense, String href, LiftMultiText caption)</Member>
	  <Member type="Method">public sealed override Void MergeInNote(WeSayDataObject extensible, String type, LiftMultiText contents)</Member>
	  <Member type="Method">public sealed override WeSayDataObject GetOrMakeParentReversal(WeSayDataObject parent, LiftMultiText contents, String type)</Member>
	  <Member type="Method">public sealed override WeSayDataObject MergeInReversal(LexSense sense, WeSayDataObject parent, LiftMultiText contents, String type, String rawXml)</Member>
	  <Member type="Method">public sealed override WeSayDataObject MergeInEtymology(LexEntry entry, String source, LiftMultiText form, LiftMultiText gloss, String rawXml)</Member>
	  <Member type="Method">public sealed override Void ProcessRangeElement(String range, String id, String guid, String parent, LiftMultiText description, LiftMultiText label, LiftMultiText abbrev)</Member>
	  <Member type="Method">public sealed override Void ProcessFieldDefinition(String tag, LiftMultiText description)</Member>
	  <Member type="Method">public sealed override Void MergeInGrammaticalInfo(WeSayDataObject senseOrReversal, String val, List&lt;Trait&gt; traits)</Member>
	  <Member type="Method">public sealed override Void MergeInField(WeSayDataObject extensible, String typeAttribute, DateTime dateCreated, DateTime dateModified, LiftMultiText contents, List&lt;Trait&gt; traits)</Member>
	  <Member type="Method">public sealed override Void MergeInTrait(WeSayDataObject extensible, Trait trait)</Member>
	  <Member type="Method">public sealed override Void MergeInRelation(WeSayDataObject extensible, String relationFieldId, String targetId, String rawXml)</Member>
	  <Member type="Method">public sealed override Void Dispose()</Member>
	  <Member type="Method">public sealed override Void FinishEntry(LexEntry entry)</Member>
	  <Member type="Constructor">public LiftMerger(LiftRepository repository)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>LiftExporter</Name>
	  <Access>Public</Access>
	  <Member type="Property">public String ProducerString { get; }</Member>
	  <Member type="Property">protected XmlWriter Writer { get; }</Member>
	  <Member type="Method">public Void End()</Member>
	  <Member type="Method">public virtual Void Add(LexEntry entry)</Member>
	  <Member type="Method">public Void Add(LexEntry entry, Int32 order)</Member>
	  <Member type="Method">public String GetHumanReadableId(LexEntry entry, Dictionary&lt;String, Int32&gt; idsAndCounts)</Member>
	  <Member type="Method">public Void Add(LexSense sense)</Member>
	  <Member type="Method">public Void Add(LexExampleSentence example)</Member>
	  <Member type="Method">public Void Add(String propertyName, MultiText text)</Member>
	  <Member type="Method">public Void AddDeletedEntry(LexEntry entry)</Member>
	  <Member type="Constructor">public LiftExporter(String path)</Member>
	  <Member type="Constructor">public LiftExporter(StringBuilder builder, Boolean produceFragmentOnly)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Interface">
	  <Name>IRepository&lt;T&gt;</Name>
	  <Access>Public</Access>
	  <Member type="Property">DateTime LastModified { get; }</Member>
	  <Member type="Property">Boolean CanQuery { get; }</Member>
	  <Member type="Property">Boolean CanPersist { get; }</Member>
	  <Member type="Method">Void Startup(ProgressState state)</Member>
	  <Member type="Method">T CreateItem()</Member>
	  <Member type="Method">Int32 CountAllItems()</Member>
	  <Member type="Method">RepositoryId GetId(T item)</Member>
	  <Member type="Method">T GetItem(RepositoryId id)</Member>
	  <Member type="Method">Void DeleteItem(T item)</Member>
	  <Member type="Method">Void DeleteItem(RepositoryId id)</Member>
	  <Member type="Method">Void DeleteAllItems()</Member>
	  <Member type="Method">RepositoryId[] GetAllItems()</Member>
	  <Member type="Method">Void SaveItem(T item)</Member>
	  <Member type="Method">Void SaveItems(IEnumerable&lt;T&gt; items)</Member>
	  <Member type="Method">ResultSet&lt;T&gt; GetItemsMatching(Query query)</Member>
	</Entity>
	<Entity type="Class">
	  <Name>SynchronicRepository&lt;T&gt;</Name>
	  <Access>Public</Access>
	  <Member type="Property">public sealed override DateTime LastModified { get; }</Member>
	  <Member type="Property">public sealed override Boolean CanQuery { get; }</Member>
	  <Member type="Property">public sealed override Boolean CanPersist { get; }</Member>
	  <Member type="Method">public sealed override Void Startup(ProgressState state)</Member>
	  <Member type="Method">public sealed override T CreateItem()</Member>
	  <Member type="Method">public sealed override Int32 CountAllItems()</Member>
	  <Member type="Method">public sealed override RepositoryId GetId(T item)</Member>
	  <Member type="Method">public sealed override T GetItem(RepositoryId id)</Member>
	  <Member type="Method">public sealed override Void DeleteItem(T item)</Member>
	  <Member type="Method">public sealed override Void DeleteItem(RepositoryId id)</Member>
	  <Member type="Method">public sealed override Void DeleteAllItems()</Member>
	  <Member type="Method">public sealed override RepositoryId[] GetAllItems()</Member>
	  <Member type="Method">public sealed override Void SaveItem(T item)</Member>
	  <Member type="Method">public sealed override Void SaveItems(IEnumerable&lt;T&gt; items)</Member>
	  <Member type="Method">public sealed override ResultSet&lt;T&gt; GetItemsMatching(Query query)</Member>
	  <Member type="Method">public sealed override Void Dispose()</Member>
	  <Member type="Constructor">public SynchronicRepository&lt;T&gt;()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>Db4oRepository&lt;T&gt;</Name>
	  <Access>Internal</Access>
	  <Member type="Property">protected IExtObjectContainer InternalDatabase { get; }</Member>
	  <Member type="Property">internal IExtObjectContainer Database { get; }</Member>
	  <Member type="Property">public sealed override DateTime LastModified { get; private set; }</Member>
	  <Member type="Property">Boolean WeSay.Data.IRepository&lt;T&gt;.CanQuery { get; }</Member>
	  <Member type="Property">Boolean WeSay.Data.IRepository&lt;T&gt;.CanPersist { get; }</Member>
	  <Member type="Method">public sealed override Void Startup(ProgressState state)</Member>
	  <Member type="Method">public sealed override T CreateItem()</Member>
	  <Member type="Method">public sealed override Int32 CountAllItems()</Member>
	  <Member type="Method">public sealed override RepositoryId GetId(T item)</Member>
	  <Member type="Method">public sealed override T GetItem(RepositoryId id)</Member>
	  <Member type="Method">public sealed override Void DeleteItem(T item)</Member>
	  <Member type="Method">public sealed override Void DeleteItem(RepositoryId id)</Member>
	  <Member type="Method">public sealed override Void DeleteAllItems()</Member>
	  <Member type="Method">public sealed override RepositoryId[] GetAllItems()</Member>
	  <Member type="Method">public RepositoryId[] GetItemsModifiedSince(DateTime last)</Member>
	  <Member type="Method">public sealed override Void SaveItems(IEnumerable&lt;T&gt; items)</Member>
	  <Member type="Method">public sealed override ResultSet&lt;T&gt; GetItemsMatching(Query query)</Member>
	  <Member type="Method">public sealed override Void SaveItem(T item)</Member>
	  <Member type="Method">public Boolean CanQuery()</Member>
	  <Member type="Method">public Boolean CanPersist()</Member>
	  <Member type="Method">public sealed override Void Dispose()</Member>
	  <Member type="Constructor">public Db4oRepository&lt;T&gt;()</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>MemoryRepository&lt;T&gt;</Name>
	  <Access>Internal</Access>
	  <Member type="Property">public sealed override DateTime LastModified { get; internal set; }</Member>
	  <Member type="Property">public virtual Boolean CanQuery { get; }</Member>
	  <Member type="Property">public virtual Boolean CanPersist { get; }</Member>
	  <Member type="Method">public virtual Void Startup(ProgressState state)</Member>
	  <Member type="Method">public virtual T CreateItem()</Member>
	  <Member type="Method">public virtual Void DeleteItem(T item)</Member>
	  <Member type="Method">public virtual Void DeleteItem(RepositoryId id)</Member>
	  <Member type="Method">public sealed override Void DeleteAllItems()</Member>
	  <Member type="Method">public sealed override RepositoryId[] GetAllItems()</Member>
	  <Member type="Method">public virtual Void SaveItem(T item)</Member>
	  <Member type="Method">public virtual Void SaveItems(IEnumerable&lt;T&gt; items)</Member>
	  <Member type="Method">public virtual ResultSet&lt;T&gt; GetItemsMatching(Query query)</Member>
	  <Member type="Method">public virtual Int32 CountAllItems()</Member>
	  <Member type="Method">public virtual RepositoryId GetId(T item)</Member>
	  <Member type="Method">public virtual T GetItem(RepositoryId id)</Member>
	  <Member type="Method">public virtual Void Dispose()</Member>
	  <Member type="Constructor">public MemoryRepository&lt;T&gt;()</Member>
	  <Modifier>None</Modifier>
	</Entity>
  </Entities>
  <Relations>
	<Relation type="Realization" first="7" second="4" />
	<Relation type="Association" first="5" second="4">
	  <Direction>None</Direction>
	  <IsAggregation>False</IsAggregation>
	  <IsComposition>True</IsComposition>
	</Relation>
	<Relation type="Association" first="5" second="4">
	  <Direction>None</Direction>
	  <IsAggregation>False</IsAggregation>
	  <IsComposition>True</IsComposition>
	</Relation>
	<Relation type="Association" first="1" second="0">
	  <Direction>None</Direction>
	  <IsAggregation>False</IsAggregation>
	  <IsComposition>False</IsComposition>
	</Relation>
	<Relation type="Association" first="1" second="3">
	  <Direction>None</Direction>
	  <IsAggregation>False</IsAggregation>
	  <IsComposition>False</IsComposition>
	</Relation>
	<Relation type="Association" first="1" second="2">
	  <Direction>None</Direction>
	  <IsAggregation>False</IsAggregation>
	  <IsComposition>False</IsComposition>
	</Relation>
	<Relation type="Realization" first="6" second="4" />
	<Relation type="Generalization" first="1" second="7" />
	<Relation type="Realization" first="5" second="4" />
  </Relations>
  <Positions>
	<Shape>
	  <Location left="1925" top="922" />
	  <Size width="295" height="216" />
	</Shape>
	<Shape>
	  <Location left="1569" top="867" />
	  <Size width="303" height="272" />
	</Shape>
	<Shape>
	  <Location left="1925" top="1459" />
	  <Size width="876" height="612" />
	</Shape>
	<Shape>
	  <Location left="1925" top="1153" />
	  <Size width="505" height="289" />
	</Shape>
	<Shape>
	  <Location left="1569" top="30" />
	  <Size width="303" height="320" />
	</Shape>
	<Shape>
	  <Location left="1925" top="417" />
	  <Size width="295" height="391" />
	</Shape>
	<Shape>
	  <Location left="2263" top="417" />
	  <Size width="345" height="476" />
	</Shape>
	<Shape>
	  <Location left="1569" top="417" />
	  <Size width="303" height="391" />
	</Shape>
	<Connection>
	  <StartNode isHorizontal="False" location="152" />
	  <EndNode isHorizontal="False" location="152" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="94" />
	  <EndNode isHorizontal="True" location="15" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="74" />
	  <EndNode isHorizontal="True" location="33" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="True" location="109" />
	  <EndNode isHorizontal="True" location="54" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="257" />
	  <EndNode isHorizontal="True" location="24" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="219" />
	  <EndNode isHorizontal="True" location="21" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="81" />
	  <EndNode isHorizontal="True" location="260" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="113" />
	  <EndNode isHorizontal="False" location="113" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="54" />
	  <EndNode isHorizontal="False" location="205" />
	</Connection>
  </Positions>
</ClassProject>