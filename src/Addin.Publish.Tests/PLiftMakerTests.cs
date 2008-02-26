using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using NUnit.Framework;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Db4o_Specific;
using WeSay.Project;

namespace Addin.Publish.Tests
{
	[TestFixture]
	public class PLiftMakerTests
	{
		private PLiftMaker _maker;
		private string _outputPath;
		private ViewTemplate _viewTemplate;
		private List<string> _writingSystemIds;

		[SetUp]
		public void Setup()
		{
			Db4oLexModelHelper.InitializeForNonDbTests();
			_maker = new PLiftMaker();
			_outputPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());


		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(_outputPath))
			{
				File.Delete(_outputPath);
			}
		}

	}
}
