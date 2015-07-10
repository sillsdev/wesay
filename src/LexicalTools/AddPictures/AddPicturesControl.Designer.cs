using SIL.Windows.Forms.ImageGallery;

namespace WeSay.LexicalTools.AddPictures
{
	partial class AddPicturesControl
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
			this._thumbnailViewer = new ThumbnailViewer();
			this._searchWords = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this._searchButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			// _thumbnailViewer
			//
			this._thumbnailViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._thumbnailViewer.Location = new System.Drawing.Point(16, 67);
			this._thumbnailViewer.Name = "_thumbnailViewer";
			this._thumbnailViewer.Size = new System.Drawing.Size(484, 361);
			this._thumbnailViewer.TabIndex = 0;
			this._thumbnailViewer.ThumbBorderColor = System.Drawing.Color.Wheat;
			this._thumbnailViewer.ThumbNailSize = 95;
			//
			// _searchWords
			//
			this._searchWords.Location = new System.Drawing.Point(95, 16);
			this._searchWords.Name = "_searchWords";
			this._searchWords.Size = new System.Drawing.Size(281, 20);
			this._searchWords.TabIndex = 1;
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Search Words";
			//
			// _searchButton
			//
			this._searchButton.Location = new System.Drawing.Point(383, 12);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(75, 23);
			this._searchButton.TabIndex = 3;
			this._searchButton.Text = "Search";
			this._searchButton.UseVisualStyleBackColor = true;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			//
			// AddPicturesControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._searchButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._searchWords);
			this.Controls.Add(this._thumbnailViewer);
			this.Name = "AddPicturesControl";
			this.Size = new System.Drawing.Size(515, 431);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ThumbnailViewer _thumbnailViewer;
		private System.Windows.Forms.TextBox _searchWords;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button _searchButton;
	}
}
