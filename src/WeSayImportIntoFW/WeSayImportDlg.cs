using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using SIL.FieldWorks.FDO;
using SIL.FieldWorks.Common.COMInterfaces;	// FW WS stuff
using SIL.FieldWorks.Common.Framework;
using SIL.FieldWorks.Common.FwUtils;
using SIL.FieldWorks.Common.Controls;

namespace SIL.FieldWorks.LexText.Controls
{
	public partial class WeSayImportDlg : Form, SIL.FieldWorks.LexText.Controls.IFwConnectedDialog
	{
		private FdoCache m_cache;
		private XCore.Mediator m_mediator;
		public WeSayImportDlg()
		{
			InitializeComponent();
		}
		/// <summary>
		/// From IFwConnectedDialog
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="mediator"></param>
		void SIL.FieldWorks.LexText.Controls.IFwConnectedDialog.Init(FdoCache cache, XCore.Mediator mediator)
		{
			m_cache = cache;
			m_mediator = mediator;
		}

		/// <summary>
		/// (IFwImportDialog)Shows the dialog as a modal dialog
		/// </summary>
		/// <returns>A DialogResult value</returns>
		public System.Windows.Forms.DialogResult Show(IWin32Window owner)
		{
			return MessageBox.Show("Sorry, this is still under construction.");
		}
	}
}