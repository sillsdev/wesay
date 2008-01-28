using ControlListBox=WeSay.UI.ControlListBox;

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
			this._imageList = new System.Windows.Forms.ImageList(this.components);
			this.label2 = new System.Windows.Forms.Label();
			this._addinsList = new WeSay.UI.ControlListBox();
			this.SuspendLayout();
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
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(4, 6);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(467, 15);
			this.label2.TabIndex = 16;
			this.label2.Text = "Choose which actions to make available in WeSay, and customize them.";
			//
			// _addinsList
			//
			this._addinsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._addinsList.AutoScroll = true;
			this._addinsList.Location = new System.Drawing.Point(5, 37);
			this._addinsList.Margin = new System.Windows.Forms.Padding(4);
			this._addinsList.Name = "_addinsList";
			this._addinsList.Size = new System.Drawing.Size(574, 223);
			this._addinsList.TabIndex = 19;
			//
			// ActionsControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this._addinsList);
			this.Controls.Add(this.label2);
			this.Name = "ActionsControl";
			this.Size = new System.Drawing.Size(583, 264);
			this.Load += new System.EventHandler(this.ActionsControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ImageList _imageList;
		private System.Windows.Forms.Label label2;
		private ControlListBox _addinsList;
	}
}
