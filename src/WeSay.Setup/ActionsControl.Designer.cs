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
			this.label1 = new System.Windows.Forms.Label();
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
			this.label2.Location = new System.Drawing.Point(7, 12);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(264, 17);
			this.label2.TabIndex = 16;
			this.label2.Text = "The following action addins are installed:";
			//
			// label1
			//
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 281);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(429, 17);
			this.label1.TabIndex = 16;
			this.label1.Text = "If you are or know a programmer, you can create your own actions.";
			//
			// _addinsList
			//
			this._addinsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._addinsList.AutoScroll = true;
			this._addinsList.Location = new System.Drawing.Point(11, 46);
			this._addinsList.Margin = new System.Windows.Forms.Padding(5);
			this._addinsList.Name = "_addinsList";
			this._addinsList.Size = new System.Drawing.Size(744, 218);
			this._addinsList.TabIndex = 19;
			//
			// ActionsControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this._addinsList);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "ActionsControl";
			this.Size = new System.Drawing.Size(777, 325);
			this.VisibleChanged += new System.EventHandler(this.OnVisibleChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ImageList _imageList;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private ControlListBox _addinsList;
	}
}
