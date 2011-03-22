using System.Drawing;

using NUnit.Framework;
using WeSay.Project;
using Palaso.Lift;
using WeSay.LexicalModel.Foundation;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class RtfRendererTests
	{

		[Test]
		public void GetActualTextForms_DropsIsAudioForm()
		{
			var writingSystemCollection = new WritingSystemCollection();
			writingSystemCollection.Set(WritingSystem.FromRFC5646("en"));
			var audio = WritingSystem.FromRFC5646("en");
			audio.IsVoice = true;
			writingSystemCollection.Set(audio);
			var m = new MultiText();
			m.SetAlternative("en", "foo");
			m.SetAlternative("voice", "boo");
			Assert.AreEqual(1, RtfRenderer.GetActualTextForms(m, writingSystemCollection).Count);
			Assert.AreEqual("foo", RtfRenderer.GetActualTextForms(m, writingSystemCollection)[0].Form);
		}

	}
}
