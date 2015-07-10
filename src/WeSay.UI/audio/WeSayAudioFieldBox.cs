using System;
using System.IO;
using System.Windows.Forms;
using SIL.Reporting;
using SIL.WritingSystems;
using WeSay.LexicalModel.Foundation;
using WeSay.UI.audio;

namespace WeSay.UI.audio
{
	public partial class WeSayAudioFieldBox : UserControl, IControlThatKnowsWritingSystem
	{
		private AudioPathProvider _audioPathProvider;
		private readonly ILogger _logger;
		public WritingSystemDefinition WritingSystem { get; set; }

		public WeSayAudioFieldBox(WritingSystemDefinition writingSystem, AudioPathProvider audioPathProvider,
			SIL.Reporting.ILogger logger)
		{
			_audioPathProvider = audioPathProvider;
			_logger = logger;
			WritingSystem = writingSystem;
			InitializeComponent();


			// may be changed in a moment when the actual field is read
			_shortSoundFieldControl1.Path = _audioPathProvider.GetNewPath();

			_shortSoundFieldControl1.SoundRecorded += (sender, e) =>
														 {
															 _fileName.Text =
																 _audioPathProvider.GetPartialPathFromFull(
																	 _shortSoundFieldControl1.Path);
															 _logger.WriteConciseHistoricalEvent("Recorded Sound");
														 }


	;
			_shortSoundFieldControl1.SoundDeleted += (sender, e) =>
				{
					_fileName.Text = string.Empty;
					_logger.WriteConciseHistoricalEvent("Deleted Sound");
				};
			_shortSoundFieldControl1.BeforeStartingToRecord += new EventHandler(shortSoundFieldControl1_BeforeStartingToRecord);

			this.Height = _shortSoundFieldControl1.Height + 10;
		}

		void shortSoundFieldControl1_BeforeStartingToRecord(object sender, EventArgs e)
		{
			// if this is a new entry, we might not have had an entry form to use for the sound
			// path name, when we were contructed.
			// So if there isn't already a sound recorded, see if one is available yet.
			if(!File.Exists(_shortSoundFieldControl1.Path))
				_shortSoundFieldControl1.Path = _audioPathProvider.GetNewPath();
		}

		public bool PlayOnly
		{
			get { return _shortSoundFieldControl1.PlayOnly; }
			set
			{
				_shortSoundFieldControl1.PlayOnly = value;
			}
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
					_shortSoundFieldControl1.Path = _audioPathProvider.GetCompletePathFromPartial(value);
			}
		}

		private void _fileName_TextChanged(object sender, EventArgs e)
		{
			//not allowed this.TextChanged.Invoke(this, null);

			base.Text = Path.GetRandomFileName(); //hack: we just need a way to tell the binding that we changed
		}
	}
}