using System;
using System.IO;
using System.Windows.Forms;
using WeSay.Foundation;
using WeSay.UI.audio;

namespace WeSay.UI.audio
{
	public partial class WeSayAudioFieldBox : UserControl, IControlThatKnowsWritingSystem
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

			shortSoundFieldControl1.SoundRecorded += (sender, e) => _fileName.Text = _audioPathProvider.GetPartialPathFromFull(shortSoundFieldControl1.Path);
			shortSoundFieldControl1.SoundDeleted += (sender, e) => _fileName.Text = string.Empty;
			shortSoundFieldControl1.BeforeStartingToRecord += new EventHandler(shortSoundFieldControl1_BeforeStartingToRecord);
		}

		void shortSoundFieldControl1_BeforeStartingToRecord(object sender, EventArgs e)
		{
			// if this is a new entry, we might not have had an entry form to use for the sound
			// path name, when we were contructed.
			// So if there isn't already a sound recorded, see if one is available yet.
			if(!File.Exists(shortSoundFieldControl1.Path))
				shortSoundFieldControl1.Path = _audioPathProvider.GetNewPath();
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
					shortSoundFieldControl1.Path = _audioPathProvider.GetCompletePathFromPartial(value);
			}
		}

		private void _fileName_TextChanged(object sender, EventArgs e)
		{
			//not allowed this.TextChanged.Invoke(this, null);

			base.Text = Path.GetRandomFileName(); //hack: we just need a way to tell the binding that we changed
		}
	}
}