using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.Project;
using WeSay.Project.Tests;

namespace WeSay.App.Tests
{
	[TestFixture]
	public class TabbedFormNavigationTests
	{
		private TabbedForm _tabbedForm;
		private WeSayWordsProject _project;
		private MockTask _dictionaryTask;
		private ProjectDirectorySetupForTesting _projectDirectory;
		private MockTask _dashboardTask;
		private bool _didRaiseInitializedEvent;

		[SetUp]
		public void Setup()
		{
			string entriesXml =
					@"<entry id='foo1'><lexical-unit><form lang='v'><text>fooOne</text></form></lexical-unit></entry>
								<entry id='foo2'><lexical-unit><form lang='v'><text>fooTwo</text></form></lexical-unit></entry>
								<entry id='foo3'><lexical-unit><form lang='v'><text>fooThree</text></form></lexical-unit></entry>";
			_projectDirectory = new ProjectDirectorySetupForTesting(entriesXml);

			_project = new WeSayWordsProject();
			_project.LoadFromLiftLexiconPath(_projectDirectory.PathToLiftFile);
			_tabbedForm = new TabbedForm();
			_project.Tasks = new List<ITask>();
			_dashboardTask = new MockTask("Dashboard", "The control center.", true);
			_project.Tasks.Add(_dashboardTask);
			_dictionaryTask = new MockTask("Dictionary blah blah", "The whole lexicon.", true);
			_project.Tasks.Add(_dictionaryTask);

			_tabbedForm.InitializeTasks(_project.Tasks);
		}

		[TearDown]
		public void TearDown()
		{
			_projectDirectory.Dispose();
		}

		[Test]
		public void ShouldRaiseIntializedEventWhenReadyForNavigation()
		{
			_tabbedForm.IntializationComplete += OnTabbedForm_IntializationComplete;
			_tabbedForm.ContinueLaunchingAfterInitialDisplay();
			Application.DoEvents();
			for (int i = 0;i < 1000;i++)
			{
				if (_didRaiseInitializedEvent)
				{
					break;
				}
				Application.DoEvents();
			}
			Assert.IsTrue(_didRaiseInitializedEvent);
		}

		private void OnTabbedForm_IntializationComplete(object sender, EventArgs e)
		{
			_didRaiseInitializedEvent = true;
		}

		[Test]
		public void ShouldSwitchToDictionaryTaskWhenURLCallsForItAndIsNew()
		{
			_tabbedForm.GoToUrl("foo2");
			Assert.AreEqual(_dictionaryTask, _tabbedForm.ActiveTask);
		}

		[Test]
		public void ShouldSwitchToDictionaryTaskWhenURLCallsForItAndIsNotNew()
		{
			_tabbedForm.ActiveTask = _dictionaryTask;
			_tabbedForm.ActiveTask = _dashboardTask;
			_tabbedForm.GoToUrl("foo2");
			Assert.AreEqual(_dictionaryTask, _tabbedForm.ActiveTask);
		}

		[Test]
		public void ShouldStayInDictionaryTaskWhenURLCallsForIt()
		{
			_tabbedForm.ActiveTask = _dictionaryTask;
			_tabbedForm.GoToUrl("foo2");
			Assert.AreEqual(_dictionaryTask, _tabbedForm.ActiveTask);
		}

		[Test]
		public void ShouldSetCurrentUrlToRequestedUrl()
		{
			_tabbedForm.GoToUrl("foo2");
			Assert.AreEqual("foo2", _tabbedForm.CurrentUrl);
		}

		[Test]
		public void ShouldAskTaskToGoToRequestedUrl()
		{
			_tabbedForm.GoToUrl("foo2");
			Assert.AreEqual("foo2", _dictionaryTask._urlThatItWasToldToGoTo);
		}

		/*
 * TODO
		[Test]
		public void ShouldGiveMessageIfNeededTaskIsNotFound()
		{
			Palaso.Reporting.ErrorReport.JustRecordNonFatalMessagesForTesting = true;
			Assert.IsNull(Palaso.Reporting.ErrorReport.PreviousNonFatalMessage);
			_tabbedForm.GoToUrl("foo2");
			Assert.IsNotNull(Palaso.Reporting.ErrorReport.PreviousNonFatalMessage);
		}
 */
	}
}