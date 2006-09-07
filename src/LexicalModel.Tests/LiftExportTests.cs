using System.IO;
using NUnit.Framework;
using WeSay.Data;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LiftExportTests
	{
		private Db4oBindingList<LexEntry> _entriesList;
		private string _filePath ;


		[SetUp]
		public void Setup()
		{
			_filePath = Path.GetTempFileName();
		}
		[TearDown]
		public void TearDown()
		{
			this._entriesList.Dispose();
			File.Delete(_filePath);
		}


	}
}
