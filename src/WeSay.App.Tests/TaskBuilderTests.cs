//using Gtk;
using NUnit.Framework;
using WeSay.UI;

namespace WeSay.App.Tests
{
	[TestFixture]
	public class TaskBuilderTests
	{
		[SetUp]
		public void Setup()
		{
		 //   Application.Init();
		}

		[Test]
		public void ThaiTest()
		{
			using (WeSayWordsProject project = new WeSayWordsProject())
			{
				project.LoadFromProjectDirectoryPath(@"..\..\SampleProjects\Thai");
				TabbedForm tabbedForm = new TabbedForm();

				using (SampleTaskBuilder builder = new SampleTaskBuilder(project, tabbedForm))
				{
					Assert.Greater(builder.Tasks.Count, 0);
				}
			}
		}
		[Test]
		public void PretendTest()
		{
			using (WeSayWordsProject project = new WeSayWordsProject())
			{
				project.LoadFromProjectDirectoryPath(@"..\..\SampleProjects\PRETEND");
				TabbedForm tabbedForm = new TabbedForm();

				using (SampleTaskBuilder builder = new SampleTaskBuilder(project, tabbedForm))
				{
					Assert.Greater(builder.Tasks.Count, 0);
				}
			}
		}
	}
}
