using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Gtk;

namespace WeSay.App.Tests
{
	[TestFixture]
	public class TaskBuilderTests
	{
		[SetUp]
		public void Setup()
		{
			Gtk.Application.Init();
		}

		[Test]
		public void SmokeTest()
		{
			BasilProject project = new BasilProject(@"..\..\SampleProjects\Thai");
			WeSay.App.SampleTaskBuilder builder = new SampleTaskBuilder(project);
			using (builder)
			{
				Assert.Greater(builder.Tasks.Count,0);
			}
		}
	}
}
