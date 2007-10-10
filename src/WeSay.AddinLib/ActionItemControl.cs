using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Palaso.Reporting;
using WeSay.Foundation;

namespace WeSay.AddinLib
{
	public partial class ActionItemControl : UserControl //, IControlForListBox
	{
		private IWeSayAddin _addin;
		private bool _inAdminMode;
		// private bool _showInWeSay;
		public event EventHandler Launch;

//        public ActionItemControl(bool inAdminMode)
//        {
//            _inAdminMode = inAdminMode;
//            InitializeComponent();
//            UpdateVisualThings();
//            _toggleShowInWeSay.Visible = inAdminMode;
//            _setupButton.Visible = inAdminMode;
//        }

		public bool DoShowInWeSay
		{
			get { return AddinSet.Singleton.DoShowInWeSay(_addin.ID); }
			set { AddinSet.Singleton.SetDoShowInWeSay(_addin.ID, value); }
		}

		private static bool ReturnFalse()
		{
			return false;
		}

		public ActionItemControl(IWeSayAddin addin, bool inAdminMode)
		{
			_addin = addin;
			_inAdminMode = inAdminMode;
			InitializeComponent();
			_actionName.Font = StringCatalog.ModifyFontForLocalization(_actionName.Font);
			_description.Font = StringCatalog.ModifyFontForLocalization(_description.Font);

			_toggleShowInWeSay.Visible = inAdminMode;
			_setupButton.Visible = inAdminMode;

			if (addin.ButtonImage != null)
			{
				//review: will these be disposed when the button is disposed?
				_launchButton.Image =
						addin.ButtonImage.GetThumbnailImage(_launchButton.Width - 10,
															_launchButton.Height - 10,
															ReturnFalse,
															IntPtr.Zero);
			}
			LoadSettings();

			UpdateVisualThings();

			//nb: we do want to show the setup, even if the addin says unavailable.
			//Maybe that's *why it's unavailable*.. it may need to be set up first.

			_setupButton.Visible = inAdminMode &&
								   _addin is IWeSayAddinHasSettings &&
								   ((IWeSayAddinHasSettings) _addin).Settings != null;
		}

		public void Draw(Graphics graphics, Rectangle bounds)
		{
			//this.InvokePaint(this, new PaintEventArgs(graphics, bounds));
		}

		public int GetHeight(int width)
		{
			return Height;
		}

		private void _launchButton_Click(object sender, EventArgs e)
		{
			if (Launch != null)
			{
				Launch.Invoke(_addin, null);
			}
		}

		private void LoadSettings()
		{
			if (! (_addin is IWeSayAddinHasSettings))
			{
				return;
			}
			IWeSayAddinHasSettings addin = (IWeSayAddinHasSettings) _addin;
			object existingSettings = addin.Settings;
			if (existingSettings == null)
			{
				return; // this class doesn't do settings
			}

			//this is not necessarily the right place for this deserialization to be happening
			string settings = AddinSet.Singleton.GetSettingsXmlForAddin(_addin.ID);
			if (!String.IsNullOrEmpty(settings))
			{
				XmlSerializer x = new XmlSerializer(existingSettings.GetType());
				using (StringReader r = new StringReader(settings))
				{
					addin.Settings = x.Deserialize(r);
				}
			}
		}

		private void OnSetupClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				LoadSettings();
				IWeSayAddinHasSettings addin = (IWeSayAddinHasSettings) _addin;
				if (!addin.DoShowSettingsDialog(ParentForm))
				{
					return;
				}

				object settings = addin.Settings;
				if (settings == null)
				{
					return;
				}
				XmlSerializer serializer = new XmlSerializer(settings.GetType());
				StringBuilder builder = new StringBuilder();
				XmlWriterSettings writerSettings = new XmlWriterSettings();
				writerSettings.ConformanceLevel = ConformanceLevel.Fragment; //we don't want the <xml header
				using (StringWriter stringWriter = new StringWriter(builder))
				{
					using (XmlTextWriter writer = new FragmentXmlTextWriter(stringWriter))
					{
						writer.Formatting = Formatting.Indented;
						serializer.Serialize(writer, settings);
						writer.Close();
					}
					string settingsXml = builder.ToString();
					stringWriter.Close();
					AddinSet.Singleton.SetSettingsForAddin(((IWeSayAddin) addin).ID, settingsXml);
				}
			}
			catch (Exception error)
			{
				ErrorReport.ReportNonFatalMessage("Sorry, WeSay had a problem storing those settings. {0}",
												  error.Message);
			}

			UpdateVisualThings();
		}

		private void UpdateVisualThings()
		{
			UpdateEnabledStates();
			_actionName.Text = _addin.Name;
			_description.Text = _addin.ShortDescription;

			if (_inAdminMode && !DoShowInWeSay)
			{
				_toggleShowInWeSay.Text = "Not In WeSay";
				_toolTip.SetToolTip(_toggleShowInWeSay, "Click to make this action available within WeSay.");
//                e.Graphics.DrawLine(Pens.Red, new Point(0,0), new Point(_toggleShowInWeSay.Width,_toggleShowInWeSay.Height));
//                e.Graphics.DrawLine(Pens.Red, new Point(0, _toggleShowInWeSay.Height), new Point(_toggleShowInWeSay.Width, 0));
			}
			else
			{
				_toggleShowInWeSay.Text = "Visible In WeSay";
				_toolTip.SetToolTip(_toggleShowInWeSay, "Click to make this action unavailable within WeSay.");
			}
		}

		private void UpdateEnabledStates()
		{
			if (!_addin.Available)
			{
				_actionName.ForeColor = Color.Gray;
				_description.ForeColor = Color.Gray;
				_launchButton.Enabled = false;
				_setupButton.Visible = false;
				_toggleShowInWeSay.Visible = false;
			}
			else
			{
				_actionName.ForeColor = SystemColors.WindowText;
				_description.ForeColor = SystemColors.WindowText;
				_launchButton.Enabled = true;
			}
		}

		private void _toggleShowInWeSay_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			DoShowInWeSay = !DoShowInWeSay;
			UpdateVisualThings();
			//_toggleShowInWeSay.Invalidate();
		}
	}

	//lets us serialize to an xml fragment
	internal class FragmentXmlTextWriter : XmlTextWriter
	{
		public FragmentXmlTextWriter(TextWriter w) : base(w) {}
		public FragmentXmlTextWriter(Stream w, Encoding encoding) : base(w, encoding) {}
		public FragmentXmlTextWriter(string filename, Encoding encoding) : base(filename, encoding) {}

		public override void WriteStartDocument() {}
	}
}