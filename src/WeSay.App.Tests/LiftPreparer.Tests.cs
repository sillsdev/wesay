using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using WeSay.Project;
using WeSay.Project.Tests;

namespace WeSay.App.Tests
{
	[TestFixture]
	public class LiftPreparerTests
	{
		[SetUp]
		public void Setup()
		{
			Palaso.Reporting.ErrorReport.IsOkToInteractWithUser = false;
		}


		[Test]
		public void PopulateDefinitions_EmptyLift()
		{
			XmlDocument dom =  GetTransformedDom("");
			Expect(dom, "lift", 1);
		}


		[Test]
		public void PopulateDefinitions_GetsDefinitionWithConcatenatedGlosses()
		{
			string entriesXml =
					@"<entry id='foo1'>
						<sense>
							<gloss lang='en'>
								<text>one</text>
							</gloss>
							<gloss lang='en'>
								<text>two</text>
							</gloss>
						</sense>
					</entry>";
			XmlDocument dom = GetTransformedDom(entriesXml);
			Expect(dom, "lift/entry/sense/gloss", 2);
			Expect(dom, "lift/entry/sense/definition", 1);
			ExpectSingleInstanceWithInnerXml(dom, "lift/entry/sense/definition/form[@lang='en']/text", "one; two");
		}

		[Test]
		public void PopulateDefinitions_MergesInWritingSystemsWithExistingDefinition()
		{
			string entriesXml =
					@"<entry id='foo1'>
						<sense>
							<definition>
								<form lang='a'>
									<text>a definition</text>
								</form>
								<form lang='b'>
									<text>b definition</text>
								</form>
							</definition>
							<gloss lang='b'>
								<text>SHOULD NOT SEE IN DEF</text>
							</gloss>
							<gloss lang='c'>
								<text>c gloss</text>
							</gloss>
						</sense>
					</entry>";
			XmlDocument dom = GetTransformedDom(entriesXml);
			Expect(dom, "lift/entry/sense/gloss", 2);
			Expect(dom, "lift/entry/sense/definition", 1);
			ExpectSingleInstanceWithInnerXml(dom, "lift/entry/sense/definition/form[@lang='a']/text", "a definition");
			ExpectSingleInstanceWithInnerXml(dom, "lift/entry/sense/definition/form[@lang='b']/text", "b definition");
			ExpectSingleInstanceWithInnerXml(dom, "lift/entry/sense/definition/form[@lang='c']/text", "c gloss");
		}

		private void Expect(XmlDocument dom, string xpath, int expectedCount)
		{
			Assert.AreEqual(expectedCount, dom.SelectNodes(xpath).Count);
		}

		private void ExpectSingleInstanceWithInnerXml(XmlDocument dom, string xpath, string expectedValue)
		{
			Assert.AreEqual(1, dom.SelectNodes(xpath).Count);
			Assert.AreEqual(expectedValue, dom.SelectNodes(xpath)[0].InnerXml);
		}

		private XmlDocument GetTransformedDom(string entriesXml)
		{
			XmlDocument doc = new XmlDocument();
			using (Project.Tests.ProjectDirectorySetupForTesting pd = new ProjectDirectorySetupForTesting(entriesXml))
			{
				using (WeSayWordsProject project = pd.CreateLoadedProject())
				{
					LiftPreparer preparer = new LiftPreparer(project);
					string outputPath = preparer.PopulateDefinitions(project.PathToLiftFile);
					Assert.IsTrue(File.Exists(outputPath));
					doc.Load(outputPath);
			   }
			}
			return doc;
		}
	}
}
