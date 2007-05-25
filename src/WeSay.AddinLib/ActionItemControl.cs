using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Mono.Addins;

namespace WeSay.AddinLib
{
	public partial class ActionItemControl : UserControl//, IControlForListBox
	{
		private IWeSayAddin _addin;
		public event EventHandler Launch;

		public ActionItemControl(bool inAdminMode)
		{
			InitializeComponent();
			_setupButton.Visible = inAdminMode;
		}

		private static bool ReturnFalse()
		{
			return false;
		}
		public ActionItemControl(IWeSayAddin addin, bool inAdminMode) : this(inAdminMode)
		{
			_addin = addin;
			_actionName.Text = addin.Name;
			_description.Text = addin.ShortDescription;
			if (addin.ButtonImage != null)
			{
				//review: will these be disposed when the button is disposed?
				_launchButton.Image =
					addin.ButtonImage.GetThumbnailImage(_launchButton.Width-10, _launchButton.Height-10, ReturnFalse,
														IntPtr.Zero);
			}
			if (!addin.Available)
			{
				_setupButton.Visible = false;
				_actionName.ForeColor = System.Drawing.Color.Gray;
				_description.ForeColor = System.Drawing.Color.Gray;
				_launchButton.Enabled = false;
			}
			else
			{
				_setupButton.Visible = inAdminMode &&
					_addin is IWeSayAddinHasSettings &&
					((IWeSayAddinHasSettings)_addin).SettingsToPersist !=null;
			}

		}

		public void Draw(Graphics graphics, Rectangle bounds)
		{
			//this.InvokePaint(this, new PaintEventArgs(graphics, bounds));
		}

		public int GetHeight(int width)
		{
			return this.Height;

		}

		private void _launchButton_Click(object sender, EventArgs e)
		{
			if (Launch != null)
			{
				LoadSettings();
				Launch.Invoke(_addin,null);
			}
		}

		private void LoadSettings()
		{
			if (! (_addin is IWeSayAddinHasSettings))
			{
				return;
			}
			IWeSayAddinHasSettings addin = (IWeSayAddinHasSettings)_addin;
			object existingSettings = addin.SettingsToPersist;
			if (existingSettings == null)
			{
				return;  // this class doesn't do settings
			}

			//this is not necessarily the right place for this deserialization to be happening
			string settings = Project.WeSayWordsProject.Project.GetSettingsXmlForAddin(addin.ID);
			if (!String.IsNullOrEmpty(settings))
			{
				XmlSerializer x = new XmlSerializer(existingSettings.GetType());
				using(StringReader r = new StringReader(settings))
				{
					addin.SettingsToPersist = x.Deserialize(r);
				}
			}
		}

		private void OnSetupClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				LoadSettings();
				IWeSayAddinHasSettings addin = (IWeSayAddinHasSettings)_addin;
				if (!addin.DoShowSettingsDialog(this.ParentForm))
				{
					return;
				}

				object settings = addin.SettingsToPersist;
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
					Project.WeSayWordsProject.Project.SetSettingsForAddin(addin.ID, settingsXml);
				}
			}
			catch (Exception error)
			{
				Reporting.ErrorReporter.ReportNonFatalMessage("Sorry, WeSay had a problem storing those settings. {0}",
															  error.Message);
			}

		}

	}

	//lets us serialize to an xml fragment
	internal class FragmentXmlTextWriter : XmlTextWriter
	{

		public FragmentXmlTextWriter( TextWriter w ) : base( w ) {}
		public FragmentXmlTextWriter( Stream w, Encoding encoding ) : base( w, encoding ) {}
		public FragmentXmlTextWriter(string filename, Encoding encoding) : base(filename, encoding) { }


		public override void WriteStartDocument()
		{
		}
	}
}
