//using Gtk;
using System.ComponentModel;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.LexicalModel.Tests;
using WeSay.Project;
using WeSay.UI;

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




		private IRecordListManager GetRecordListManager()
		{
			IRecordListManager recordListManager;
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
				recordListManager = new Db4oRecordListManager(new WeSayWordsDb4oModelConfiguration(), this._project.PathToLexicalModelDB);
				Db4oLexModelHelper.Initialize(((Db4oRecordListManager)recordListManager).DataSource.Data);
			}
			return recordListManager;
		}

	}
}
