namespace WeSay.App
{
	partial class TabbedForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this._localizationHelper = new Palaso.UI.WindowsForms.i8n.LocalizationHelper(this.components);
			this._statusStrip = new System.Windows.Forms.StatusStrip();
			this.tabControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._localizationHelper)).BeginInit();
			this.SuspendLayout();
			//
			// tabControl1
			//
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(631, 412);
			this.tabControl1.TabIndex = 0;
			//
			// tabPage2
			//
			this.tabPage2.Location = new System.Drawing.Point(4, 27);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(623, 381);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			this.tabPage2.UseVisualStyleBackColor = true;
			//
			// _localizationHelper
			//
			this._localizationHelper.Parent = this;
			//
			// _statusStrip
			//
			this._statusStrip.Location = new System.Drawing.Point(0, 412);
			this._statusStrip.Name = "_statusStrip";
			this._statusStrip.Size = new System.Drawing.Size(631, 22);
			this._statusStrip.TabIndex = 1;
			//
			// TabbedForm
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(631, 434);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this._statusStrip);
			this.Icon = global::WeSay.App.Properties.Resources.ApplicationIcon;
			this.Name = "TabbedForm";
			this.Text = "WeSay";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.tabControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._localizationHelper)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage2;
		private Palaso.UI.WindowsForms.i8n.LocalizationHelper _localizationHelper;
		private System.Windows.Forms.StatusStrip _statusStrip;
	}
}