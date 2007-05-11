using System;
using WeSay.AddinLib;

namespace WeSay.CommonTools
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
			this._addinsList = new WindowsApplication2.ControlListBox();
			this.SuspendLayout();
			//
			// _addinsList
			//
			this._addinsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._addinsList.AutoScroll = true;
			this._addinsList.Location = new System.Drawing.Point(0, 0);
			this._addinsList.Name = "_addinsList";
			this._addinsList.Size = new System.Drawing.Size(485, 327);
			this._addinsList.TabIndex = 0;
			//
			// ActionsControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._addinsList);
			this.Name = "ActionsControl";
			this.Size = new System.Drawing.Size(499, 342);
			this.ResumeLayout(false);

		}

		#endregion

		private WindowsApplication2.ControlListBox _addinsList;


	}
}
