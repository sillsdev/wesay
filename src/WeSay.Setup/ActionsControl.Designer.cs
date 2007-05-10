namespace WeSay.Setup
{
	partial class ActionsControl
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
			this._listView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this._imageList = new System.Windows.Forms.ImageList(this.components);
			this.label2 = new System.Windows.Forms.Label();
			this._launchButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// _listView
			//
			this._listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.columnHeader1,
			this.columnHeader2});
			this._listView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._listView.HideSelection = false;
			this._listView.Location = new System.Drawing.Point(7, 28);
			this._listView.MultiSelect = false;
			this._listView.Name = "_listView";
			this._listView.Size = new System.Drawing.Size(559, 189);
			this._listView.SmallImageList = this._imageList;
			this._listView.TabIndex = 15;
			this._listView.UseCompatibleStateImageBehavior = false;
			this._listView.View = System.Windows.Forms.View.Details;
			this._listView.DoubleClick += new System.EventHandler(this.OnLaunch);
			this._listView.SelectedIndexChanged += new System.EventHandler(this.OnListView_SelectedIndexChanged);
			//
			// columnHeader1
			//
			this.columnHeader1.Text = "Action";
			this.columnHeader1.Width = 180;
			//
			// columnHeader2
			//
			this.columnHeader2.Text = "Description";
			this.columnHeader2.Width = 500;
			//
			// _imageList
			//
			this._imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._imageList.ImageSize = new System.Drawing.Size(16, 16);
			this._imageList.TransparentColor = System.Drawing.Color.Transparent;
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(5, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(198, 13);
			this.label2.TabIndex = 16;
			this.label2.Text = "The following action addins are installed:";
			//
			// _launchButton
			//
			this._launchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._launchButton.Location = new System.Drawing.Point(491, 223);
			this._launchButton.Name = "_launchButton";
			this._launchButton.Size = new System.Drawing.Size(75, 23);
			this._launchButton.TabIndex = 17;
			this._launchButton.Text = "&Launch";
			this._launchButton.UseVisualStyleBackColor = true;
			this._launchButton.Click += new System.EventHandler(this.OnLaunch);
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(5, 228);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(322, 13);
			this.label1.TabIndex = 16;
			this.label1.Text = "If you are or know a programmer, you can create your own actions.";
			//
			// ActionsControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this._launchButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._listView);
			this.Name = "ActionsControl";
			this.Size = new System.Drawing.Size(583, 264);
			this.VisibleChanged += new System.EventHandler(this.OnVisibleChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView _listView;
		private System.Windows.Forms.ImageList _imageList;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button _launchButton;
		private System.Windows.Forms.Label label1;
	}
}
