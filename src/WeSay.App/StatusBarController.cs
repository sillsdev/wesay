using SIL.i18n;
using SIL.UiBindings;
using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.App.Properties;
using WeSay.LexicalTools.Dashboard;

namespace WeSay.App
{
	public class StatusBarController : IDisposable
	{
		private readonly ICountGiver _countGiver;
		private Timer _timer;
		private readonly ToolStripStatusLabel _wordCountLabel;
		private ToolStripItem _launchConfigToolLink;

		public StatusBarController(ICountGiver counterGiver)
		{
			_countGiver = counterGiver;

			_launchConfigToolLink = new ToolStripButton()
			{
				Text = "Configure This Project...",
				//doesn't work Alignment = ToolStripItemAlignment.Right


			};
			_launchConfigToolLink.Margin = new Padding(100, 0, 0, 0);
			_launchConfigToolLink.Image = Resources.WeSayConfigMenuSized;
			_launchConfigToolLink.Click += Dash.OnRunConfigureTool;
			_wordCountLabel = new ToolStripStatusLabel();
			_wordCountLabel.Font = (Font)StringCatalog.LabelFont.Clone();

			_timer = new Timer();
			_timer.Interval = 1000;
			var format = StringCatalog.Get("~Dictionary has {0} words", "Shown at the bottom of the screen.");
			_timer.Tick += new EventHandler(
				(o, e) =>
				{
					try
					{
						_wordCountLabel.Text = String.Format(format, _countGiver.Count);
					}
					catch (Exception)
					{
						_wordCountLabel.Text = "error";
					}

				}
			);
			_timer.Start();
		}

		public bool ShowConfigLauncher
		{
			set
			{
				_launchConfigToolLink.Visible = value;
			}
		}

		public StatusStrip StatusStrip
		{
			get { return null; }
			set
			{
				value.Items.AddRange(new ToolStripItem[] { _wordCountLabel, _launchConfigToolLink });
			}
		}

		public void Dispose()
		{
			if (_timer != null)
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
