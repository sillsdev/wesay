using System;
using System.Windows.Forms;

namespace WeSay.CommonTools
{
	partial class TaskIndicator
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
			this._btnName = new System.Windows.Forms.Button();
			this._textShortDescription = new System.Windows.Forms.TextBox();
			this.localizationHelper1 = new WeSay.UI.LocalizationHelper(this.components);
			this._intray = new WeSay.CommonTools.ItemsToDoIndicator();
			((System.ComponentModel.ISupportInitialize)(this.localizationHelper1)).BeginInit();
			this.SuspendLayout();
			//
			// _btnName
			//
			this._btnName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._btnName.BackColor = System.Drawing.Color.AliceBlue;
			this._btnName.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._btnName.Location = new System.Drawing.Point(79, 9);
			this._btnName.Name = "_btnName";
			this._btnName.Size = new System.Drawing.Size(225, 33);
			this._btnName.TabIndex = 1;
			this._btnName.Text = "Gather from Foo words";
			this._btnName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._btnName.UseVisualStyleBackColor = false;
			this._btnName.Click += new System.EventHandler(this.OnBtnNameClick);
			//
			// _textShortDescription
			//
			this._textShortDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._textShortDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._textShortDescription.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._textShortDescription.Location = new System.Drawing.Point(79, 48);
			this._textShortDescription.Multiline = true;
			this._textShortDescription.Name = "_textShortDescription";
			this._textShortDescription.Size = new System.Drawing.Size(222, 32);
			this._textShortDescription.TabIndex = 2;
			this._textShortDescription.TabStop = false;
			this._textShortDescription.Text = "See words in Foo, write the same words in Boo";
			//
			// localizationHelper1
			//
			this.localizationHelper1.Parent = this;
			//
			// _intray
			//
			this._intray.BackColor = System.Drawing.SystemColors.Window;
			this._intray.Count = 327;
			this._intray.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._intray.Location = new System.Drawing.Point(7, 15);
			this._intray.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._intray.Name = "_intray";
			this._intray.ReferenceCount = 1000;
			this._intray.Size = new System.Drawing.Size(57, 21);
			this._intray.TabIndex = 3;
			//
			// TaskIndicator
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._intray);
			this.Controls.Add(this._textShortDescription);
			this.Controls.Add(this._btnName);
			this.Name = "TaskIndicator";
			this.Size = new System.Drawing.Size(304, 83);
			this.BackColorChanged += new System.EventHandler(this.TaskIndicator_BackColorChanged);
			((System.ComponentModel.ISupportInitialize)(this.localizationHelper1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _btnName;
		private System.Windows.Forms.TextBox _textShortDescription;
		private ItemsToDoIndicator _intray;
		private WeSay.UI.LocalizationHelper localizationHelper1;
	}
}
