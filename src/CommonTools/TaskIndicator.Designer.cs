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
			this._layout = new System.Windows.Forms.TableLayoutPanel();
			this._btnName = new System.Windows.Forms.Button();
			this._textShortDescription = new System.Windows.Forms.Label();
			this._intray = new WeSay.CommonTools.ItemsToDoIndicator();
			this._layout.SuspendLayout();
			this.SuspendLayout();
			//
			// _layout
			//
			this._layout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._layout.AutoSize = true;
			this._layout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._layout.ColumnCount = 1;
			this._layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layout.Controls.Add(this._btnName);
			this._layout.Controls.Add(this._textShortDescription);
			this._layout.Location = new System.Drawing.Point(79, 9);
			this._layout.Margin = new System.Windows.Forms.Padding(0);
			this._layout.Name = "_layout";
			this._layout.RowCount = 2;
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layout.Size = new System.Drawing.Size(302, 52);
			this._layout.TabIndex = 4;
			//
			// _btnName
			//
			this._btnName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._btnName.AutoSize = true;
			this._btnName.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._btnName.BackColor = System.Drawing.Color.AliceBlue;
			this._btnName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
			this._btnName.Location = new System.Drawing.Point(3, 3);
			this._btnName.Name = "_btnName";
			this._btnName.Size = new System.Drawing.Size(296, 30);
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
			this._textShortDescription.AutoSize = true;
			this._textShortDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
			this._textShortDescription.Location = new System.Drawing.Point(3, 36);
			this._textShortDescription.Name = "_textShortDescription";
			this._textShortDescription.Size = new System.Drawing.Size(296, 16);
			this._textShortDescription.TabIndex = 2;
			this._textShortDescription.Text = "See words in Foo, write the same words in Boo";
			//
			// _intray
			//
			this._intray.BackColor = System.Drawing.SystemColors.Window;
			this._intray.Count = 327;
			this._intray.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._intray.Location = new System.Drawing.Point(7, 15);
			this._intray.Margin = new System.Windows.Forms.Padding(4);
			this._intray.Name = "_intray";
			this._intray.ReferenceCount = 1000;
			this._intray.Size = new System.Drawing.Size(57, 21);
			this._intray.TabIndex = 3;
			//
			// TaskIndicator
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this._intray);
			this.Controls.Add(this._layout);
			this.DoubleBuffered = true;
			this.Name = "TaskIndicator";
			this.Size = new System.Drawing.Size(387, 61);
			this.BackColorChanged += new System.EventHandler(this.TaskIndicator_BackColorChanged);
			this._layout.ResumeLayout(false);
			this._layout.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _layout;
		private System.Windows.Forms.Button _btnName;
		private System.Windows.Forms.Label _textShortDescription;
		private ItemsToDoIndicator _intray;
	}
}
