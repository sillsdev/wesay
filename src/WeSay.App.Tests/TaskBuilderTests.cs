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
				project.Load(@"..\..\SampleProjects\Thai");
				using (SampleTaskBuilder builder = new SampleTaskBuilder(project))
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
				project.Load(@"..\..\SampleProjects\PRETEND");

				using (SampleTaskBuilder builder = new SampleTaskBuilder(project))
				{
					Assert.Greater(builder.Tasks.Count, 0);
				}
			}
		}
	}
}
