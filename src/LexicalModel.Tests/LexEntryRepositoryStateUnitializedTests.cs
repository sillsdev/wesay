using NUnit.Framework;
using SIL.IO;
using System.IO;

namespace WeSay.LexicalModel.Tests
{
	[TestFixture]
	public class LexEntryRepositoryStateUnitializedTests
	{
		/* NOMORELOCKING
		[Test]
		[NUnit.Framework.Category("UsesObsoleteExpectedExceptionAttribute"), ExpectedException(typeof(IOException))]
		public void Constructor_FileIsWriteableAfterRepositoryIsCreated_Throws()
		{
			using (File.OpenWrite(_persistedFilePath))
			{
			}
		}
*/

		[Test]
		public void Constructor_FileIsNotWriteableWhenRepositoryIsCreated_Throws()
		{
			using (TempFile t = TempFile.CreateAndGetPathButDontMakeTheFile())
			{
				using (File.OpenWrite(t.Path))
				{
					// Note: Will throw => Dispose will not be called.
					Assert.Throws<IOException>(() => new LexEntryRepository(t.Path));
				}
			}
		}
	}
}