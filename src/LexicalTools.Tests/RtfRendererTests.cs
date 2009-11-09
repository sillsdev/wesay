using System.Drawing;

using NUnit.Framework;
using Palaso.LexicalModel;
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
			writingSystemCollection.Add("en", new WritingSystem("en", new Font("Arial", 12)));
			var audio = new WritingSystem("en", new Font("Arial", 12));
			audio.IsAudio = true;
			writingSystemCollection.Add("voice", audio);
			var m = new MultiText();
			m.SetAlternative("en", "foo");
			m.SetAlternative("voice", "boo");
			Assert.AreEqual(1, RtfRenderer.GetActualTextForms(m, writingSystemCollection).Count);
			Assert.AreEqual("foo", RtfRenderer.GetActualTextForms(m, writingSystemCollection)[0].Form);
		}

	}
}
