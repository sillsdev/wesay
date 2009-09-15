using System;
using WeSay.LexicalTools.GatherByWordList;
using WeSay.UI;
using WeSay.UI.Buttons;
using WeSay.UI.TextBoxes;

namespace WeSay.LexicalTools.Review.AdvancedHistory
{
	partial class AdvancedHistoryControl
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
			this.SuspendLayout();
			//
			// AdvancedHistoryControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.Name = "AdvancedHistoryControl";
			this.Size = new System.Drawing.Size(511, 429);
			this.BackColorChanged += new System.EventHandler(this.OnBackColorChanged);
			this.ResumeLayout(false);

		}

		#endregion




	}
}