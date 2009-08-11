using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Chorus.SyncPanel;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;

namespace Addin.Backup
{
	public partial class SynchronizeDialog: Form
	{


		public SynchronizeDialog(SyncPanel syncPanel)
		{
			InitializeComponent();
			syncPanel.Location = new Point(0,0);
			syncPanel.Dock = DockStyle.Fill;

			Controls.Add(syncPanel);
//            pictureBox1.Image = Resources.backupToDeviceImage;
//            _cancelButton.Text = StringCatalog.Get(_cancelButton.Text);
//            _cancelButton.Font = StringCatalog.ModifyFontForLocalization(_cancelButton.Font);
		}


	}
}