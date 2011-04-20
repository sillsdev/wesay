using System.Drawing;

using NUnit.Framework;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using WeSay.Project;
using Palaso.Lift;
using WeSay.LexicalModel.Foundation;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class RtfRendererTests
	{

		[Test]
		[Category("For review")]
		public void GetActualTextForms_DropsIsAudioForm()
		{
			Assert.Fail("We removed this method in favor of a more general one on the writingsystemrepo.");
			//using (var tempFolder = new TemporaryFolder("ProjectFromRtfRendererTests"))
			//{
			//    var writingSystemCollection = new LdmlInFolderWritingSystemRepository(tempFolder.Path);
			//    writingSystemCollection.Set(WritingSystemDefinition.FromLanguage("en"));
			//    var audio = WritingSystemDefinition.FromLanguage("en");
			//    audio.IsVoice = true;
			//    writingSystemCollection.Set(audio);
			//    var m = new MultiText();
			//    m.SetAlternative("en", "foo");
			//    m.SetAlternative("voice", "boo");
			//    Assert.AreEqual(1, RtfRenderer.GetActualTextForms(m, writingSystemCollection).Count);
			//    Assert.AreEqual("foo", RtfRenderer.GetActualTextForms(m, writingSystemCollection)[0].Form);
			//}
		}

	}
}
