using System.ComponentModel;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.LexicalModel.Tests;
using WeSay.Project;

namespace WeSay.App.Tests
{
	[TestFixture]
	public class TaskBuilderTests
	{
		private WeSayWordsProject _project;

		[SetUp]
		public void Setup()
		{
			this._project = new WeSayWordsProject();
		}

		[TearDown]
		public void TearDown()
		{
			_project.Dispose();
		}




		private LexEntryRepository GetRecordListManager()
		{
			LexEntryRepository recordListManager;
			if (this._project.PathToWeSaySpecificFilesDirectoryInProject.IndexOf("PRETEND") > -1)
			{
				IBindingList entries = new PretendRecordList();
				recordListManager = new InMemoryRecordListManager();
				Db4oLexModelHelper.InitializeForNonDbTests();
				IRecordList<LexEntry> masterRecordList = recordListManager.GetListOfType<LexEntry>();
				foreach (LexEntry entry in entries)
				{
					masterRecordList.Add(entry);
				}
			}
			else
			{
				recordListManager = new LexEntryRepository(new WeSayWordsDb4oModelConfiguration(), this._project.PathToDb4oLexicalModelDB);
				Db4oLexModelHelper.Initialize(((LexEntryRepository)recordListManager).DataSource.Data);
			}
			return recordListManager;
		}

	}
}
