using System;
using System.IO;
using System.Windows.Forms;
using WeSay.Foundation;
using WeSay.UI.audio;

namespace WeSay.UI.audio
{
	public partial class WeSayAudioFieldBox : UserControl, ITextOrAudioBox
	{
		private AudioPathProvider _audioPathProvider;
		public WritingSystem WritingSystem { get; set; }

		public WeSayAudioFieldBox(WritingSystem writingSystem, AudioPathProvider audioPathProvider)
		{
			_audioPathProvider = audioPathProvider;
			WritingSystem = writingSystem;
			InitializeComponent();


			// may be changed in a moment when the actual field is read
			shortSoundFieldControl1.Path = _audioPathProvider.GetNewPath();

			shortSoundFieldControl1.SoundRecorded += (sender, e) => _fileName.Text = shortSoundFieldControl1.Path;
			shortSoundFieldControl1.SoundDeleted += (sender, e) => _fileName.Text = string.Empty;
		}

		public override string Text
		{
			get
			{
				return _fileName.Text;
			}
			set
			{
				_fileName.Text = value;
				if (!string.IsNullOrEmpty(value))
					shortSoundFieldControl1.Path = value;
			}
		}

		private void _fileName_TextChanged(object sender, EventArgs e)
		{
			//not allowed this.TextChanged.Invoke(this, null);

			base.Text = Path.GetRandomFileName(); //hack: we just need a way to tell the binding that we changed
		}
	}
}