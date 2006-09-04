using Gtk;
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
			Application.Init();
		}

		[Test]
		public void SmokeTest()
		{
			BasilProject project = new BasilProject(@"..\..\SampleProjects\Thai");
			SampleTaskBuilder builder = new SampleTaskBuilder(project);
			using (builder)
			{
				Assert.Greater(builder.Tasks.Count,0);
			}
		}
	}
}
