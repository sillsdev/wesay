<ClassProject>
  <Language>CSharp</Language>
  <Entities>
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
	  <Name>LexEntryRepository</Name>
	  <Access>Public</Access>
	  <Member type="Property">public sealed override DateTime LastModified { get; }</Member>
	  <Member type="Property">public sealed override Boolean CanQuery { get; }</Member>
	  <Member type="Property">public sealed override Boolean CanPersist { get; }</Member>
	  <Member type="Method">public sealed override LexEntry CreateItem()</Member>
	  <Member type="Method">public LexEntry CreateItem(Extensible eInfo)</Member>
	  <Member type="Method">public sealed override RepositoryId[] GetAllItems()</Member>
	  <Member type="Method">public sealed override Int32 CountAllItems()</Member>
	  <Member type="Method">public sealed override RepositoryId GetId(LexEntry item)</Member>
	  <Member type="Method">public sealed override LexEntry GetItem(RepositoryId id)</Member>
	  <Member type="Method">public sealed override Void SaveItems(IEnumerable&lt;LexEntry&gt; items)</Member>
	  <Member type="Method">public sealed override ResultSet&lt;LexEntry&gt; GetItemsMatching(Query query)</Member>
	  <Member type="Method">public sealed override Void SaveItem(LexEntry item)</Member>
	  <Member type="Method">public sealed override Void DeleteItem(LexEntry item)</Member>
	  <Member type="Method">public sealed override Void DeleteItem(RepositoryId repositoryId)</Member>
	  <Member type="Method">public sealed override Void DeleteAllItems()</Member>
	  <Member type="Method">public Int32 GetHomographNumber(LexEntry entry, WritingSystem headwordWritingSystem)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetAllEntriesSortedByHeadword(WritingSystem writingSystem)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetAllEntriesSortedByLexicalForm(WritingSystem writingSystem)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetAllEntriesSortedByGloss(WritingSystem writingSystem)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetAllEntriesSortedBySemanticDomain(String fieldName)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetEntriesWithMatchingGlossSortedByLexicalForm(LanguageForm glossForm, WritingSystem lexicalUnitWritingSystem)</Member>
	  <Member type="Method">public LexEntry GetLexEntryWithMatchingId(String id)</Member>
	  <Member type="Method">public LexEntry GetLexEntryWithMatchingGuid(Guid guid)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetEntriesWithSimilarLexicalForm(String lexicalForm, WritingSystem writingSystem, ApproximateMatcherOptions matcherOptions)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetEntriesWithMatchingLexicalForm(String lexicalForm, WritingSystem writingSystem)</Member>
	  <Member type="Method">public ResultSet&lt;LexEntry&gt; GetEntriesWithMissingFieldSortedByLexicalUnit(Field field, WritingSystem lexicalUnitWritingSystem)</Member>
	  <Member type="Method">public IList&lt;RepositoryId&gt; GetItemsModifiedSince(DateTime last)</Member>
	  <Member type="Method">public sealed override Void Startup(ProgressState state)</Member>
	  <Member type="Constructor">public LexEntryRepository(String path)</Member>
	  <Member type="Constructor">public LexEntryRepository(IRepository&lt;LexEntry&gt; repository)</Member>
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
	  <Name>SortDefinition</Name>
	  <Access>Public</Access>
	  <Member type="Property">public String Field { get; }</Member>
	  <Member type="Property">public IComparer Comparer { get; }</Member>
	  <Member type="Constructor">public SortDefinition(String field, IComparer comparer)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Class">
	  <Name>Query</Name>
	  <Access>Public</Access>
	  <Member type="Property">protected virtual String TypeName { get; }</Member>
	  <Member type="Property">private List&lt;KeyValuePair&lt;String, MethodInfo&gt;&gt; ResultProperties { get; }</Member>
	  <Member type="Property">private List&lt;Query&gt; NestedQueries { get; }</Member>
	  <Member type="Method">public IEnumerable&lt;Dictionary&lt;String, Object&gt;&gt; GetResults(Object o)</Member>
	  <Member type="Method">public Query In(String fieldName)</Member>
	  <Member type="Method">public Query ForEach(String fieldName)</Member>
	  <Member type="Method">public Query Show(String fieldName)</Member>
	  <Member type="Method">public Query Show(String fieldName, String label)</Member>
	  <Member type="Method">public Query ShowEach(String fieldName)</Member>
	  <Member type="Method">public Query ShowEach(String fieldName, String label)</Member>
	  <Member type="Method">public override String ToString()</Member>
	  <Member type="Constructor">internal Query(Query root, Type t)</Member>
	  <Member type="Constructor">public Query(Type t)</Member>
	  <Modifier>None</Modifier>
	</Entity>
	<Entity type="Comment">
	  <Text>Backend... methods are work in progress and will be removed soon.</Text>
	</Entity>
  </Entities>
  <Relations>
	<Relation type="Association" first="1" second="0">
	  <Direction>None</Direction>
	  <IsAggregation>False</IsAggregation>
	  <IsComposition>True</IsComposition>
	</Relation>
	<Relation type="Realization" first="1" second="2" />
	<Relation type="Association" first="1" second="3">
	  <Direction>None</Direction>
	  <IsAggregation>False</IsAggregation>
	  <IsComposition>False</IsComposition>
	</Relation>
	<Relation type="Association" first="1" second="4">
	  <Direction>None</Direction>
	  <IsAggregation>False</IsAggregation>
	  <IsComposition>False</IsComposition>
	</Relation>
	<Relation type="Realization" first="0" second="2" />
  </Relations>
  <Positions>
	<Shape>
	  <Location left="1553" top="452" />
	  <Size width="303" height="272" />
	</Shape>
	<Shape>
	  <Location left="614" top="417" />
	  <Size width="846" height="697" />
	</Shape>
	<Shape>
	  <Location left="1569" top="30" />
	  <Size width="303" height="320" />
	</Shape>
	<Shape>
	  <Location left="614" top="1161" />
	  <Size width="297" height="306" />
	</Shape>
	<Shape>
	  <Location left="940" top="1161" />
	  <Size width="395" height="306" />
	</Shape>
	<Shape>
	  <Location left="1366" top="1054" />
	  <Size width="162" height="72" />
	</Shape>
	<Connection>
	  <StartNode isHorizontal="True" location="57" />
	  <EndNode isHorizontal="True" location="22" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="752" />
	  <EndNode isHorizontal="False" location="99" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="118" />
	  <EndNode isHorizontal="False" location="118" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="544" />
	  <EndNode isHorizontal="False" location="218" />
	</Connection>
	<Connection>
	  <StartNode isHorizontal="False" location="184" />
	  <EndNode isHorizontal="False" location="168" />
	</Connection>
  </Positions>
</ClassProject>