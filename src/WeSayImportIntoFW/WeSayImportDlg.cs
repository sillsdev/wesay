using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using SIL.FieldWorks.FDO;

namespace WeSay.FieldWorks
{
	public partial class WeSayImportDlg : Form, SIL.FieldWorks.LexText.Controls.IFwExtension
	{
		private FdoCache _cache;
		private XCore.Mediator _mediator;
		public WeSayImportDlg()
		{
			InitializeComponent();
		}
		/// <summary>
		/// From IFwExtension
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="mediator"></param>
		void SIL.FieldWorks.LexText.Controls.IFwExtension.Init(FdoCache cache, XCore.Mediator mediator)
		{
			_cache = cache;
			_mediator = mediator;
		}

		/// <summary>
		/// (IFwImportDialog)Shows the dialog as a modal dialog
		/// </summary>
		/// <returns>A DialogResult value</returns>
		public System.Windows.Forms.DialogResult Show(IWin32Window owner)
		{
			return this.ShowDialog(owner);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			UpdateButtons();
			if (!btnOK.Enabled)
				return;
			DoImport();
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			if (DialogResult.OK != openFileDialog1.ShowDialog())
				return;

			tbPath.Text = openFileDialog1.FileName;
			UpdateButtons();
		}

		private void UpdateButtons()
		{
			btnOK.Enabled = tbPath.Text.Length > 0 &&
				System.IO.File.Exists(tbPath.Text);
		}

		private void DoImport()
		{

			_cache.BeginUndoTask("Undo import from WeSay:Words", "Redo from import from WeSay:Words");
			try
			{
				WeSay.FieldWorks.Importer importer = new Importer(_cache);
				this.Cursor = Cursors.WaitCursor;
				importer.ImportWeSayFile(openFileDialog1.FileName);
			}
			//catch (Exception error)
			//{
			//    //TODO: shouldn't there be a method on the cache to cancel the undo task?
			//    MessageBox.Show("Something went wrong while FieldWorks was attempting to import.");
			//    //TODO: is it may be better to just let it die of the normal green box death?
			//}
			finally
			{
				_cache.EndUndoTask();
			}
		}

		private void WeSayImportDlg_Load(object sender, EventArgs e)
		{
			UpdateButtons();
		}
	}
}