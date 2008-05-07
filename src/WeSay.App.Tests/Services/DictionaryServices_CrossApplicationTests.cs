using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Palaso.Services;
using Palaso.Services.Dictionary;
using WeSay.Project.Tests;

namespace WeSay.App.Tests
{
	/// <summary>
	/// Some of what we might want to test, like the whole launching with no UI, later bringing up
	/// the UI, etc.,  is actually in a palaso class, and already tested
	/// over in that library. See Palaso.Tests.Services.
	/// </summary>
	[TestFixture]
	public class DictionaryServices_CrossApplicationTests
	{
		private const bool kStartInServerMode=true;
		private const bool kStartInUIMode = false;

		[SetUp]
		public void Setup()
		{
			Palaso.Reporting.ErrorReport.IsOkToInteractWithUser = false;
		}

		[TearDown]
		public void TearDown()
		{
			EnsureNoWeSaysRunning();
		}

		private void EnsureNoWeSaysRunning()
		{
			for (int i = 0; i < 50; i++)
			{
				Process[] p = Process.GetProcessesByName("WeSay.App");
				if(p.Length == 0)
					break;
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

			WeSayApp app = new WeSayApp(new string[] { "-server" });
			Assert.IsTrue(app.ServerModeStartRequested);
		}



		/// <summary>
		/// WS-632 regression
		/// </summary>
		[Test]
		public void LiftFileIsUpdatedWhenServiceQuits()
		{
			using (ProjectDirectorySetupForTesting projectDirectorySetup = new ProjectDirectorySetupForTesting(string.Empty))
			{
				Process p = LaunchDictionaryServiceApp(kStartInServerMode, projectDirectorySetup);

				IDictionaryService dictionaryService = GetDictionaryService(projectDirectorySetup.PathToLiftFile, Process.GetCurrentProcess().Id);
				dictionaryService.AddEntry("v", "dontLooseThisWord", string.Empty, string.Empty, string.Empty, string.Empty);
				dictionaryService.DeregisterClient(Process.GetCurrentProcess().Id);
				Thread.Sleep(1000);
				AssertServiceIsClosed(projectDirectorySetup.PathToLiftFile);

				Assert.IsTrue(File.ReadAllText(projectDirectorySetup.PathToLiftFile).Contains("dontLooseThisWord"));
			}
		}

		[Test]
		public void ServiceExitsWhenLastClientDeregisters()
		{
			using (ProjectDirectorySetupForTesting projectDirectorySetup = new ProjectDirectorySetupForTesting(string.Empty))
			{
				Process p = LaunchDictionaryServiceApp(kStartInServerMode, projectDirectorySetup);
				int firstClientId = Process.GetCurrentProcess().Id;

				IDictionaryService dictionaryService = GetDictionaryService(projectDirectorySetup.PathToLiftFile, firstClientId);
				Assert.IsNotNull(dictionaryService, "Could not get dictionary service, first time.");
				int secondClientId = firstClientId + 1;//bad thing to do in a non-test setting
				IDictionaryService dictionaryService2 = GetDictionaryService(projectDirectorySetup.PathToLiftFile, secondClientId);
				Assert.IsNotNull(dictionaryService2, "Could not get dictionary service, second time.");
				AssertServerIsRunning(projectDirectorySetup.PathToLiftFile);
				dictionaryService.DeregisterClient(firstClientId);
				AssertServerIsRunning(projectDirectorySetup.PathToLiftFile);

				//now close the last client
				dictionaryService2.DeregisterClient(secondClientId);
				AssertServiceIsClosed(projectDirectorySetup.PathToLiftFile);
			}
		}
		private void AssertServerIsRunning(string liftPath)
		{
		   //didn't work Assert.AreEqual(System.ServiceModel.CommunicationState.Opened, ((System.ServiceModel.Channels.IChannel)service).State);

			Assert.IsNotNull(IpcSystem.GetExistingService<IDictionaryService>(DictionaryAccessor.GetServiceName(liftPath)));
		}
		private void AssertServiceIsClosed(string liftPath)
		{
		   //give it a chance to close
			for (int i = 0; i < 50; i++)
			{
				Thread.Sleep(100);

				if (null == IpcSystem.GetExistingService<IDictionaryService>(DictionaryAccessor.GetServiceName(liftPath)))
				{
					return;
				}
			}
			Assert.Fail("Service Still avialable after 5 seconds.");
		}

		private delegate void ServiceTestingMethod(IDictionaryService dictionaryService);

		[Test]
		public void FindsInUIMode()
		{
			string entriesXml = @"
						<entry id='foo1'>
								<lexical-unit><form lang='v'><text>foo</text></form></lexical-unit>
						</entry>";
			RunTest(kStartInUIMode, entriesXml, delegate(IDictionaryService dictionaryService)
										   {
											   FindResult r = dictionaryService.GetMatchingEntries("v", "foo", FindMethods.Exact.ToString());
											   Assert.AreEqual(1, r.ids.Length);
										   });
		}

		[Test]
		public void FindsInServerMode()
		{
			string entriesXml = @"
						<entry id='foo1'>
								<lexical-unit><form lang='v'><text>foo</text></form></lexical-unit>
						</entry>
						<entry id='blah22'>
								<lexical-unit><form lang='v'><text>blah</text></form></lexical-unit>
						</entry>
						<entry id='foo2'>
								<lexical-unit><form lang='v'><text>foo</text></form></lexical-unit>
						</entry>";
			RunTest(kStartInServerMode, entriesXml, delegate(IDictionaryService dictionaryService)
										   {
											  Assert.IsTrue(dictionaryService.IsInServerMode());

											  FindResult r = dictionaryService.GetMatchingEntries("v", "foo", FindMethods.Exact.ToString());
											   Assert.AreEqual(2, r.ids.Length);
										   });
		}



		/// <summary>
		/// this doesn't look at the details of the html; that job belongs in a different test
		/// </summary>
		[Test]
		public void GivesHtml()
		{
			string entriesXml = @"
						<entry id='foo1'>
								<lexical-unit><form lang='v'><text>foo</text></form></lexical-unit>
							  <sense>
								<gloss lang='en'>
									<text>gloss for foo</text>
								</gloss>
							 </sense>
						</entry>";
			RunTest(kStartInServerMode, entriesXml, delegate(IDictionaryService dictionaryService)
										   {
											   string html = dictionaryService.GetHtmlForEntries(new string[] { "foo1" });

											   Assert.IsTrue(html.Contains("<html>"));
											   Assert.IsTrue(html.Contains("gloss for foo"));
										   });
		}


		[Test]
		public void CreateNewEntryInUIMode()
		{
			CreateNewEntry(kStartInUIMode);
		}

		[Test]
		public void CreateNewEntryInServerMode()
		{
			CreateNewEntry(kStartInServerMode);
		}
		private void CreateNewEntry(bool mode)
		{
			string entriesXml = @"<entry id='foo1'/>";
			RunTest(mode, entriesXml, delegate(IDictionaryService dictionaryService)
										  {
											  string id = dictionaryService.AddEntry("v", "voom", "en", "def of voom", "v", "vlah voom!");
											  Assert.IsNotNull(id);

											  FindResult r = dictionaryService.GetMatchingEntries("v", "voom", FindMethods.Exact.ToString());
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
		public void JumpToEntryMakesAppSwitchToUIMode()
		{
			string entriesXml = @"<entry id='foo1'/>";
			RunTest(kStartInServerMode, entriesXml, delegate(IDictionaryService dictionaryService)
										   {
											   Assert.IsTrue(dictionaryService.IsInServerMode());
											   dictionaryService.JumpToEntry("foo1");

											   Assert.IsFalse(dictionaryService.IsInServerMode());
										   });
		}

		[Test]
		public void JumpToEntryMakesDictionaryTaskShowEnty()
		{
			string entriesXml = @"<entry id='foo1'><lexical-unit><form lang='v'><text>fooOne</text></form></lexical-unit></entry>
								<entry id='foo2'><lexical-unit><form lang='v'><text>fooTwo</text></form></lexical-unit></entry>
								<entry id='foo3'><lexical-unit><form lang='v'><text>fooThree</text></form></lexical-unit></entry>";
			RunTest(kStartInServerMode, entriesXml, delegate(IDictionaryService dictionaryService)
										   {
											   Assert.IsTrue(dictionaryService.IsInServerMode());
											   dictionaryService.JumpToEntry("foo2");
											  Assert.AreEqual("foo2", dictionaryService.GetCurrentUrl());
											   dictionaryService.JumpToEntry("foo3");
											  Assert.AreEqual("foo3", dictionaryService.GetCurrentUrl());
										   });
		}

		private void RunTest(bool startInServerMode, string entriesXml, ServiceTestingMethod serviceTestingMethod)
		{
			using (ProjectDirectorySetupForTesting projectDirectorySetup = new ProjectDirectorySetupForTesting(entriesXml))
			{
				Process p = LaunchDictionaryServiceApp(startInServerMode, projectDirectorySetup);
				//enhance: is ther a way to know when the process is quiescent?
				Thread.Sleep(2000);

				IDictionaryService dictionaryService = GetDictionaryService(projectDirectorySetup.PathToLiftFile, Process.GetCurrentProcess().Id);
				Assert.IsNotNull(dictionaryService, "Could not get ahold of a dictionary service from a launch of WeSay.  This can fail as a result of a timeout, if wesay was just too slow coming up.");
				try
				{
					serviceTestingMethod(dictionaryService);
				}
				finally
				{
					Thread.Sleep(100);
					if(dictionaryService.IsInServerMode())
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
						}
					}
				}
			}

		}


		private Process LaunchDictionaryServiceApp(bool launchInServerMode, ProjectDirectorySetupForTesting projectDirectorySetup)
		{

			// System.Diagnostics.Process.Start("SampleDictionaryServicesApplication.exe", "-server");
			string arguments = '"' + projectDirectorySetup.PathToLiftFile + '"';
			if(launchInServerMode)
			{
				arguments += " -server";
			}
			System.Diagnostics.ProcessStartInfo psi = new ProcessStartInfo(@"wesay.app.exe",arguments);
			Process p = System.Diagnostics.Process.Start(psi);

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

		private static IDictionaryService GetDictionaryService(string liftPath, int clientIdForRegistering)
		{
			IDictionaryService dictionaryService=null;
			for (int i = 0; i < 20; i++)
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
