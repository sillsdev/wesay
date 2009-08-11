using System;
using System.IO;
using NUnit.Framework;
using WeSay.Data.Tests;
using Palaso.TestUtilities;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntryRepositoryStateUnitializedTests
	{
		/* NOMORELOCKING
		[Test]
		[ExpectedException(typeof(IOException))]
		public void Constructor_FileIsWriteableAfterRepositoryIsCreated_Throws()
		{
			using (File.OpenWrite(_persistedFilePath))
			{
			}
		}
*/

		[Test]
		[ExpectedException(typeof(IOException))]
		public void Constructor_FileIsNotWriteableWhenRepositoryIsCreated_Throws()
		{
			using (TempFile t = TempFile.CreateAndGetPathButDontMakeTheFile())
			{
				using (File.OpenWrite(t.Path))
				{
					// Note: Will throw => Dispose will not be called.
					using (var dm = new LexEntryRepository(t.Path))
					{
					}
				}
			}
		}
	}
}