using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.LexicalTools;
using Gtk;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class PersistenceTests
	{
		[SetUp]
		public void Setup()
		{
			Gtk.Application.Init();
		}

		[Test]
		public void SmokeTest()
		{
		}
	}
}
