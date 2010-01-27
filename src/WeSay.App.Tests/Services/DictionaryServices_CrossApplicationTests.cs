	  #if DictionaryServices
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Palaso.Reporting;
using Palaso.Services;
using Palaso.Services.Dictionary;
using WeSay.Project;
using WeSay.Project.Tests;

namespace WeSay.App.Tests.Services
{
	/// <summary>
	/// Some of what we might want to test, like the whole launching with no UI, later bringing up
	/// the UI, etc.,  is actually in a palaso class, and already tested
	/// over in that library. See Palaso.Tests.Services.
	/// </summary>
	[TestFixture]
	[Category("DictionaryServices")]
	public class DictionaryServices_CrossApplicationTests
	{
		private const bool kStartInServerMode = true;
		private const bool kStartInUIMode = false;

		[SetUp]
		public void Setup()
		{
			ErrorReport.IsOkToInteractWithUser = false;
			WeSayWordsProject.PreventBackupForTests = true;
		}

		[TearDown]
		public void TearDown()
		{
			EnsureNoWeSaysRunning();
		}

		private static void EnsureNoWeSaysRunning()
		{
			for (int i = 0;i < 50;i++)
			{
				Process[] p = Process.GetProcessesByName("WeSay.App");
				if (p.Length == 0)
				{
					break;
				}
				Thread.Sleep(100);
			}
			Process[] doomed = Process.GetProcessesByName("WeSay.App");
			foreach (Process process in doomed)
			{
				if (process.MainWindowTitle.Contains("Whoops"))
				{
					process.Kill();
					Assert.Fail("Client WeSay was showing a failure dialog");
				}
				else
				{
					Debug.WriteLine("Gave up waiting, killing wesay...");
					process.Kill();
				}
			}
			//failing on the build server                Assert.AreEqual(0, doomed.Length, "Teardown shouldn't have to kill any WeSay instances.");
		}

		[Test]
		public void CommandLineArgRequestsServerMode()
		{
			WeSayApp app = new WeSayApp(new string[] {"-server"});
			Assert.IsTrue(app.ServerModeStartRequested);
		}

		/// <summary>
		/// WS-632 regression
		/// </summary>
		[Test]
		[Category("Palaso Services")]
		public void LiftFileIsUpdatedWhenServiceQuits()
		{
			using (
					ProjectDirectorySetupForTesting projectDirectorySetup =
							new ProjectDirectorySetupForTesting(string.Empty))
			{
				LaunchDictionaryServiceApp(kStartInServerMode, projectDirectorySetup);

				IDictionaryService dictionaryService =
						GetDictionaryService(projectDirectorySetup.PathToLiftFile,
											 Process.GetCurrentProcess().Id);
				dictionaryService.AddEntry("v",
										   "dontLooseThisWord",
										   string.Empty,
										   string.Empty,
										   string.Empty,
										   string.Empty);
				dictionaryService.DeregisterClient(Process.GetCurrentProcess().Id);
				Thread.Sleep(1000);
				AssertServiceIsClosed(projectDirectorySetup.PathToLiftFile);

				Assert.IsTrue(
						File.ReadAllText(projectDirectorySetup.PathToLiftFile).Contains(
								"dontLooseThisWord"));
			}
		}

		[Test]
		[Category("Palaso Services")]
		public void ServiceExitsWhenLastClientDeregisters()
		{
			using (
					ProjectDirectorySetupForTesting projectDirectorySetup =
							new ProjectDirectorySetupForTesting(string.Empty))
			{
				LaunchDictionaryServiceApp(kStartInServerMode, projectDirectorySetup);
				int firstClientId = Process.GetCurrentProcess().Id;

				IDictionaryService dictionaryService =
						GetDictionaryService(projectDirectorySetup.PathToLiftFile, firstClientId);
				Assert.IsNotNull(dictionaryService, "Could not get dictionary service, first time.");
				int secondClientId = firstClientId + 1; //bad thing to do in a non-test setting
				IDictionaryService dictionaryService2 =
						GetDictionaryService(projectDirectorySetup.PathToLiftFile, secondClientId);
				Assert.IsNotNull(dictionaryService2,
								 "Could not get dictionary service, second time.");
				AssertServerIsRunning(projectDirectorySetup.PathToLiftFile);
				dictionaryService.DeregisterClient(firstClientId);
				AssertServerIsRunning(projectDirectorySetup.PathToLiftFile);

				//now close the last client
				dictionaryService2.DeregisterClient(secondClientId);
				AssertServiceIsClosed(projectDirectorySetup.PathToLiftFile);
			}
		}

		private static void AssertServerIsRunning(string liftPath)
		{
			//didn't work Assert.AreEqual(System.ServiceModel.CommunicationState.Opened, ((System.ServiceModel.Channels.IChannel)service).State);

			Assert.IsNotNull(
					IpcSystem.GetExistingService<IDictionaryService>(
							DictionaryAccessor.GetServiceName(liftPath)));
		}

		private static void AssertServiceIsClosed(string liftPath)
		{
			//give it a chance to close
			for (int i = 0;i < 50;i++)
			{
				Thread.Sleep(100);

				if (null ==
					IpcSystem.GetExistingService<IDictionaryService>(
							DictionaryAccessor.GetServiceName(liftPath)))
				{
					return;
				}
			}
			Assert.Fail("Service Still avialable after 5 seconds.");
		}

		private delegate void ServiceTestingMethod(IDictionaryService dictionaryService);

		[Test]
		[Category("Palaso Services")]
		public void FindsInUIMode()
		{
			string entriesXml =
					@"
						<entry id='foo1'>
								<lexical-unit><form lang='v'><text>foo</text></form></lexical-unit>
						</entry>";
			RunTest(kStartInUIMode,
					entriesXml,
					delegate(IDictionaryService dictionaryService)
					{
						FindResult r = dictionaryService.GetMatchingEntries("v",
																			"foo",
																			FindMethods.Exact.
																					ToString());
						Thread.Sleep(100); // Timing sensitive?
						Assert.AreEqual(1, r.ids.Length);
					});
		}

		[Test]
		[Category("Palaso Services")]
		public void FindsInServerMode()
		{
			string entriesXml =
					@"
						<entry id='foo1'>
								<lexical-unit><form lang='v'><text>foo</text></form></lexical-unit>
						</entry>
						<entry id='blah22'>
								<lexical-unit><form lang='v'><text>blah</text></form></lexical-unit>
						</entry>
						<entry id='foo2'>
								<lexical-unit><form lang='v'><text>foo</text></form></lexical-unit>
						</entry>";
			RunTest(kStartInServerMode,
					entriesXml,
					delegate(IDictionaryService dictionaryService)
					{
						Assert.IsTrue(dictionaryService.IsInServerMode());

						FindResult r = dictionaryService.GetMatchingEntries("v",
																			"foo",
																			FindMethods.Exact.
																					ToString());
						Assert.AreEqual(2, r.ids.Length);
					});
		}

		/// <summary>
		/// this doesn't look at the details of the html; that job belongs in a different test
		/// </summary>
		[Test]
		[Category("Palaso Services")]
		public void GivesHtml()
		{
			const string entriesXml =
					@"
						<entry id='foo1'>
								<lexical-unit><form lang='v'><text>foo</text></form></lexical-unit>
							  <sense>
								<definition>
									<form lang='en'>
										<text>a definition</text>
									</form>
								</definition>
							 </sense>
						</entry>";
			RunTest(kStartInServerMode,
					entriesXml,
					delegate(IDictionaryService dictionaryService)
					{
						string html = dictionaryService.GetHtmlForEntries(new string[] {"foo1"});

						Assert.IsTrue(html.Contains("<html>"));
						Assert.IsTrue(html.Contains("a definition"));
					});
		}

		[Test]
		[Category("Palaso Services")]
		public void CreateNewEntryInUIMode()
		{
			CreateNewEntry(kStartInUIMode);
		}

		[Test]
		[Category("Palaso Services")]
		public void CreateNewEntryInServerMode()
		{
			CreateNewEntry(kStartInServerMode);
		}

		private static void CreateNewEntry(bool mode)
		{
			string entriesXml = string.Format("<entry id='foo1' guid='{0}'/>", Guid.NewGuid().ToString());
			RunTest(mode,
					entriesXml,
					delegate(IDictionaryService dictionaryService)
					{
						string id = dictionaryService.AddEntry("v",
															   "voom",
															   "en",
															   "def of voom",
															   "v",
															   "vlah voom!");
						Assert.IsNotNull(id);

						FindResult r = dictionaryService.GetMatchingEntries("v",
																			"voom",
																			FindMethods.Exact.
																					ToString());
						Assert.AreEqual(id, r.ids[0]);
					});
		}

		//        [Test]
		//        public void JumpToEntryInServerModeMakesAppComeToFrontOfZOrder()
		//        {
		//            string entriesXml = @"<entry id='foo1'/>";
		//            RunTest(kStartInServerMode, entriesXml, delegate(IDictionaryService dictionaryService)
		//                                           {
		//                                               dictionaryService.JumpToEntry("foo1");
		//
		// DONT KNOW HOW TO TEST THE ZORDER                                              Assert.IsFalse(dictionaryService.IsInServerMode());
		//                                           });
		//        }

		[Test]
		[Category("Palaso Services")]
		public void JumpToEntryMakesAppSwitchToUIMode()
		{
			string entriesXml = @"<entry id='foo1'/>";
			RunTest(kStartInServerMode,
					entriesXml,
					delegate(IDictionaryService dictionaryService)
					{
						Assert.IsTrue(dictionaryService.IsInServerMode());
						dictionaryService.JumpToEntry("lift://whoknows.lift?id=foo1");
						// This test seems timing sensitive on close, resulting in a
						// exception that doesn't occur if we slow it down a bit.
						Thread.Sleep(100);
						Assert.IsFalse(dictionaryService.IsInServerMode());
					});
		}

		[Test]
		[Category("Palaso Services")]
		public void JumpToEntryMakesDictionaryTaskShowEnty()
		{
			string entriesXml =
					@"<entry id='foo1'><lexical-unit><form lang='v'><text>fooOne</text></form></lexical-unit></entry>
								<entry id='foo2'><lexical-unit><form lang='v'><text>fooTwo</text></form></lexical-unit></entry>
								<entry id='foo3'><lexical-unit><form lang='v'><text>fooThree</text></form></lexical-unit></entry>";
			RunTest(kStartInServerMode,
					entriesXml,
					delegate(IDictionaryService dictionaryService)
					{
						Assert.IsTrue(dictionaryService.IsInServerMode());
						string url = string.Format("lift://whoknows.lift?id=foo2");
						dictionaryService.JumpToEntry(url);
						Thread.Sleep(2000);

						var s = dictionaryService.GetCurrentUrl();
						Assert.IsTrue(s.Contains("foo2"));
						url = string.Format("lift://whoknows.lift?id=foo3");
						dictionaryService.JumpToEntry(url);
						Thread.Sleep(2000);
						Assert.IsTrue(dictionaryService.GetCurrentUrl().Contains("foo3"));
					});
		}

		private static void RunTest(bool startInServerMode,
									string entriesXml,
									ServiceTestingMethod serviceTestingMethod)
		{
			using (
					ProjectDirectorySetupForTesting projectDirectorySetup =
							new ProjectDirectorySetupForTesting(entriesXml))
			{
				Process p = LaunchDictionaryServiceApp(startInServerMode, projectDirectorySetup);
				//enhance: is ther a way to know when the process is quiescent?
				Thread.Sleep(2000);

				IDictionaryService dictionaryService =
						GetDictionaryService(projectDirectorySetup.PathToLiftFile,
											 Process.GetCurrentProcess().Id);
				Assert.IsNotNull(dictionaryService,
								 "Could not get ahold of a dictionary service from a launch of WeSay.  This can fail as a result of a timeout, if wesay was just too slow coming up.");
				try
				{
					serviceTestingMethod(dictionaryService);
				}
				finally
				{
					Thread.Sleep(100);
					if (dictionaryService.IsInServerMode())
					{
						dictionaryService.DeregisterClient(Process.GetCurrentProcess().Id);
					}
					else
					{
						if (p.HasExited)
						{
							//may hit this case if we make a test run multiple copies of wesay... not sure
						}
						else
						{
							p.CloseMainWindow();
							//give the app up to 10 seconds to quit before we go deleting the
							//project directory
							for (int i = 0; i < 100 && !p.HasExited; i++ )
								Thread.Sleep(100);
						}
					}
				}
			}
		}

		private static Process LaunchDictionaryServiceApp(bool launchInServerMode,
														  ProjectDirectorySetupForTesting
																  projectDirectorySetup)
		{
			// System.Diagnostics.Process.Start("SampleDictionaryServicesApplication.exe", "-server");
			string arguments = '"' + projectDirectorySetup.PathToLiftFile + '"';
			if (launchInServerMode)
			{
				arguments += " -server -launchedByUnitTest";
			}
			ProcessStartInfo psi = new ProcessStartInfo(@"WeSay.App.exe", arguments);
			Process p = Process.Start(psi);

			//this only works because we only launch it once... wouldn't be adequate logic if we
			//might just be joining an existing process
			if (!launchInServerMode)
			{
				Assert.IsTrue(p.WaitForInputIdle(25000), "Gave up waiting for the UI to come up.");
			}
			else
			{
				Thread.Sleep(2000); // wait for process to start up
			}
			return p;
		}

		private static IDictionaryService GetDictionaryService(string liftPath,
															   int clientIdForRegistering)
		{
			IDictionaryService dictionaryService = null;
			for (int i = 0;i < 20;i++)
			{
				Thread.Sleep(500);
				string serviceName = DictionaryAccessor.GetServiceName(liftPath);

				dictionaryService = IpcSystem.GetExistingService<IDictionaryService>(serviceName);
				if (dictionaryService != null)
				{
					dictionaryService.RegisterClient(clientIdForRegistering);
					break;
				}
			}
			return dictionaryService;
		}
	}
}
#endif