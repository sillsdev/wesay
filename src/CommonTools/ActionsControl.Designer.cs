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
			this._addinsList = new System.Windows.Forms.TableLayoutPanel();
			this.SuspendLayout();
			//
			// _addinsList
			//
			this._addinsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._addinsList.AutoScroll = true;
			this._addinsList.Location = new System.Drawing.Point(0, 19);
			this._addinsList.Name = "_addinsList";
			this._addinsList.Size = new System.Drawing.Size(485, 308);
			this._addinsList.TabIndex = 0;

			this._addinsList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._addinsList.ColumnCount = 1;
			this._addinsList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._addinsList.Margin = new System.Windows.Forms.Padding(4);
			this._addinsList.RowCount = 0;

			//
			// ActionsControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoScroll = true;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._addinsList);
			this.Name = "ActionsControl";
			this.Size = new System.Drawing.Size(499, 342);
			this.ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _addinsList;


	}
}
