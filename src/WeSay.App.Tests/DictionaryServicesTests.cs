using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Palaso.DictionaryService.Client;
using SampleDictionaryServicesApplication;
using WeSay.Language;
using WeSay.Project;
using Timer=System.Timers.Timer;

namespace WeSay.App.Tests
{
	/// <summary>
	/// Some of what we might want to test, like the whole launching with no UI, later bringing up
	/// the UI, etc.,  is actually in a palaso class, and already tested
	/// over in that library. See Palaso.Tests.Services.
	/// </summary>
	[TestFixture]
	public class DictionaryServicesTests
	{
		private const bool kStartInServerMode=true;
		private const bool kStartInUIMode = false;

		[SetUp]
		public void Setup()
		{

		}

		[Test]
		public void CommandLineArgRequestsServerMode()
		{
			WeSayApp app = new WeSayApp(new string[] { "-server" });
			Assert.IsTrue(app.ServerModeStartRequested);
		}

		[Test]
		public void ServiceExitsWhenLastClientDeregisters()
		{
			using (TestProjectDirectory projectInfo = new TestProjectDirectory(string.Empty))
			{
				Process p = LaunchDictionaryServiceApp(kStartInServerMode, projectInfo);
				int firstClientId = Process.GetCurrentProcess().Id;

				IDictionaryService dictionaryService = GetDictionaryService(projectInfo.PathToLiftFile, firstClientId);
				int secondClientId = firstClientId + 1;//bad thing to do in a non-test setting
				IDictionaryService dictionaryService2 = GetDictionaryService(projectInfo.PathToLiftFile, secondClientId);
				AssertServerIsRunning(projectInfo.PathToLiftFile);
				dictionaryService.DeregisterClient(firstClientId);
				AssertServerIsRunning(projectInfo.PathToLiftFile);

				//now close the last client
				dictionaryService2.DeregisterClient(secondClientId);
				AssertServiceIsClosed(projectInfo.PathToLiftFile);
			}
		}
		private void AssertServerIsRunning(string liftPath)
		{
		   //didn't work Assert.AreEqual(System.ServiceModel.CommunicationState.Opened, ((System.ServiceModel.Channels.IChannel)service).State);

			Assert.IsNotNull(IPCUtils.GetExistingService<IDictionaryService>(GetServiceAddress(liftPath)));
		}
		private void AssertServiceIsClosed(string liftPath)
		{
		   //give it a chance to close
			for (int i = 0; i < 50; i++)
			{
				Thread.Sleep(100);

				if (null == IPCUtils.GetExistingService<IDictionaryService>(GetServiceAddress(liftPath)))
				{
					return;
				}
			}
			Assert.Fail("Service Still avialable after 5 seconds.");
		}

		private delegate void ServiceTestingMethod(IDictionaryService dictionaryService);

		[Test]
		public void FindsExactMatchStartingInUIMode()
		{
			string entriesXml = @"
						<entry id='foo1'>
								<lexical-unit><form lang='v'><text>foo</text></form></lexical-unit>
						</entry>";
			RunTest(kStartInUIMode, entriesXml, delegate(IDictionaryService dictionaryService)
										   {
											   string[] entryIds = dictionaryService.GetIdsOfMatchingEntries("v", "foo", FindMethods.Exact);
											   Assert.AreEqual(1, entryIds.Length);
										   });
		}

		[Test]
		public void FindsExactMatchOfTwo()
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
											   string[] entryIds = dictionaryService.GetIdsOfMatchingEntries("v", "foo", FindMethods.Exact);
											   Assert.AreEqual(2, entryIds.Length);
										   });
		}

		[Test]
		public void FindsNoExactMatch()
		{
			string entriesXml = @"
						<entry id='foo1'>
								<lexical-unit><form lang='v'><text>foo</text></form></lexical-unit>
						</entry>";
			RunTest(kStartInServerMode, entriesXml, delegate(IDictionaryService dictionaryService)
										   {
											   string[] entryIds = dictionaryService.GetIdsOfMatchingEntries("v", "blahblah", FindMethods.Exact);
											   Assert.AreEqual(0, entryIds.Length);

										   });
		}
		/// <summary>
		/// details of the html belong in a different test
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
											   string html = dictionaryService.GetHmtlForEntry("foo1");

											   Assert.IsTrue(html.Contains("gloss for foo"));
										   });
		}

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
			string entriesXml = @"<entry id='foo1'/><entry id='foo2'/><entry id='foo3'/>";
			RunTest(kStartInServerMode, entriesXml, delegate(IDictionaryService dictionaryService)
										   {
											   Assert.IsTrue(dictionaryService.IsInServerMode());
											   dictionaryService.JumpToEntry("foo2");
											  Assert.AreEqual("foo2", dictionaryService.GetCurrentUrl());
										   });
		}

//        [Test]
//        public void JumpToEntryMakesDictionaryTaskShowEnty()
//        {
//            using(TestProjectDirectory projectInfo = new TestProjectDirectory("<entry id='foo1'/>"))
//            {
//                WeSayApp app = new WeSayApp(new string[]{'"'+projectInfo.PathToLiftFile +'"'});
//                DelayedActionOnAnotherThread action = new DelayedActionOnAnotherThread(projectInfo.PathToLiftFile);
//                app.Run();
//                Assert.AreEqual("foo1", app.CurrentUrl);
//            }
//        }

//        class DelayedActionOnAnotherThread
//        {
//            private readonly string _pathToLift;
//
//            public DelayedActionOnAnotherThread(string pathToLift)
//            {
//                _pathToLift = pathToLift;
//                System.Timers.Timer timer = new Timer(1000);
//                timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimer_Elapsed);
//            }
//
//            void OnDoIt(object sender, System.Timers.ElapsedEventArgs e)
//            {
//
//                IDictionaryService dictionaryService = GetDictionaryService(_pathToLift, Process.GetCurrentProcess().Id);
//                dictionaryService.JumpToEntry("foo1");
//
//            }
//        }

		private void RunTest(bool startInServerMode, string entriesXml, ServiceTestingMethod serviceTestingMethod)
		{
			using (TestProjectDirectory projectInfo = new TestProjectDirectory(entriesXml))
			{
				Process p = LaunchDictionaryServiceApp(startInServerMode, projectInfo);
				IDictionaryService dictionaryService = GetDictionaryService(projectInfo.PathToLiftFile, Process.GetCurrentProcess().Id);
				Assert.IsNotNull(dictionaryService);
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


		private Process LaunchDictionaryServiceApp(bool launchInServerMode, TestProjectDirectory projectInfo)
		{

			// System.Diagnostics.Process.Start("SampleDictionaryServicesApplication.exe", "-server");
			string arguments = '"' + projectInfo.PathToLiftFile + '"';
			if(launchInServerMode)
			{
				arguments += " -server";
			}
			System.Diagnostics.ProcessStartInfo psi = new ProcessStartInfo(@"wesay.app.exe",arguments);
			Process p = System.Diagnostics.Process.Start(psi);
			if (launchInServerMode)
			{
				p.WaitForInputIdle(25000);
			}
			return p;
		}

		private static IDictionaryService GetDictionaryService(string liftPath, int clientIdForRegistering)
		{
			IDictionaryService dictionaryService=null;
			for (int i = 0; i < 10; i++)
			{
				Thread.Sleep(500);
				string serviceAddress = GetServiceAddress(liftPath);

				dictionaryService = IPCUtils.GetExistingService<IDictionaryService>(serviceAddress);
				if (dictionaryService != null)
				{
					dictionaryService.RegisterClient(clientIdForRegistering);
					break;
				}
			}
			return dictionaryService;
		}

		private static string GetServiceAddress(string liftPath)
		{
			return "net.pipe://localhost/DictionaryServices/"
				   + Uri.EscapeDataString(liftPath);
		}
	}
}
