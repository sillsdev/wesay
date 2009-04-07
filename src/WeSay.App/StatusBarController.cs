using System;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;

namespace WeSay.App
{
	public class StatusBarController : IDisposable
	{
		private readonly ICountGiver _countGiver;
		private Timer _timer;
		private ToolStripStatusLabel _wordCountLabel;

		public StatusBarController(ICountGiver counterGiver)
		{
			_countGiver = counterGiver;
			_wordCountLabel = new ToolStripStatusLabel();

			_timer = new System.Windows.Forms.Timer();
			_timer.Interval = 1000;
			var format = StringCatalog.Get("~Dictionary has {0} words", "Shown at the bottom of the screen.");
			_timer.Tick += new EventHandler((o, e) =>
												{
													try
													{
														_wordCountLabel.Text = String.Format(format, _countGiver.Count);
													}
													catch (Exception)
													{
														_wordCountLabel.Text = "error";
													}

												});
			_timer.Start();
		}

		public StatusStrip StatusStrip
		{
			get { return null;}
			set
			{
				value.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { _wordCountLabel });
			}
		}

		public void Dispose()
		{
			if(_timer!=null)
			{
				_timer.Dispose();
			}
			_timer = null;
		}
	}

	/// <summary>
	/// for unit tests
	/// </summary>
	public class NullStatusBarController : StatusBarController
	{
		public NullStatusBarController() : base(new NullCountGiver())
		{

		}
	}
}
