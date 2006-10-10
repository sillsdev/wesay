//using Gtk;
using System.ComponentModel;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Tests;
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

		[Test]
		public void ThaiTest()
		{
			_project.LoadFromProjectDirectoryPath(@"..\..\SampleProjects\Thai");
			TabbedForm tabbedForm = new TabbedForm();

			using (SampleTaskBuilder builder = new SampleTaskBuilder(_project, tabbedForm, GetRecordListManager()))
			{
				Assert.Greater(builder.Tasks.Count, 0);
			}
		}
		[Test]
		public void PretendTest()
		{
			_project.LoadFromProjectDirectoryPath(@"..\..\SampleProjects\PRETEND");
			TabbedForm tabbedForm = new TabbedForm();

			using (SampleTaskBuilder builder = new SampleTaskBuilder(_project, tabbedForm, GetRecordListManager()))
			{
				Assert.Greater(builder.Tasks.Count, 0);
			}
		}

		private IRecordListManager GetRecordListManager()
		{
			IRecordListManager recordListManager;
			if (this._project.PathToWeSaySpecificFilesDirectoryInProject.IndexOf("PRETEND") > -1)
			{
				IBindingList entries = new PretendRecordList();
				recordListManager = new InMemoryRecordListManager();
				IRecordList<LexEntry> masterRecordList = recordListManager.Get<LexEntry>();
				foreach (LexEntry entry in entries)
				{
					masterRecordList.Add(entry);
				}
			}
			else
			{
				recordListManager = new Db4oRecordListManager(this._project.PathToLexicalModelDB);
			}
			return recordListManager;
		}

	}
}
