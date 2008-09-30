using Resources=WeSay.ConfigTool.Properties.Resources;

namespace WeSay.ConfigTool
{
	partial class BackupPlanControl
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this._historyPanel = new Chorus.UI.HistoryPanel();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this._syncPanel = new Chorus.UI.SyncPanel();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage2.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.SuspendLayout();
			//
			// tabPage2
			//
			this.tabPage2.Controls.Add(this._historyPanel);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(606, 359);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "History";
			this.tabPage2.UseVisualStyleBackColor = true;
			//
			// _historyPanel
			//
			this._historyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._historyPanel.Location = new System.Drawing.Point(3, 3);
			this._historyPanel.Name = "_historyPanel";
			this._historyPanel.Size = new System.Drawing.Size(600, 353);
			this._historyPanel.TabIndex = 0;
			//
			// tabPage1
			//
			this.tabPage1.Controls.Add(this._syncPanel);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(606, 359);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Sync";
			this.tabPage1.UseVisualStyleBackColor = true;
			//
			// _syncPanel
			//
			this._syncPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._syncPanel.Location = new System.Drawing.Point(3, 3);
			this._syncPanel.Name = "_syncPanel";
			this._syncPanel.Size = new System.Drawing.Size(600, 353);
			this._syncPanel.TabIndex = 0;
			this._syncPanel.Load += new System.EventHandler(this.syncPanel1_Load);
			//
			// tabControl1
			//
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(614, 385);
			this.tabControl1.TabIndex = 0;
			//
			// BackupPlanControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.tabControl1);
			this.Name = "BackupPlanControl";
			this.Size = new System.Drawing.Size(614, 385);
			this.tabPage2.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabControl tabControl1;
		private Chorus.UI.HistoryPanel _historyPanel;
		private Chorus.UI.SyncPanel _syncPanel;

	}
}
