using System;
using System.Drawing;
using System.Windows.Forms;

namespace WeSay.CommonTools
{
	partial class CurrentTaskIndicatorControl
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
			this.label1 = new System.Windows.Forms.Label();
			this._indicatorPanel = new System.Windows.Forms.Panel();
			this.localizationHelper1 = new WeSay.UI.LocalizationHelper(this.components);
			this._shapeControl = new ShapeControl.ShapeControl();
			((System.ComponentModel.ISupportInitialize)(this.localizationHelper1)).BeginInit();
			this.SuspendLayout();
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "~Current task:";
			//
			// _indicatorPanel
			//
			this._indicatorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._indicatorPanel.BackColor = System.Drawing.Color.Transparent;
			this._indicatorPanel.Location = new System.Drawing.Point(70, 35);
			this._indicatorPanel.Name = "_indicatorPanel";
			this._indicatorPanel.Size = new System.Drawing.Size(482, 80);
			this._indicatorPanel.TabIndex = 1;
			this._indicatorPanel.Click += new System.EventHandler(this._indicatorPanel_Click);
			this._indicatorPanel.Resize += new System.EventHandler(this._indicatorPanel_Resize);
			//
			// localizationHelper1
			//
			this.localizationHelper1.Parent = this;
			//
			// _shapeControl
			//
			this._shapeControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(253)))), ((int)(((byte)(219)))));
			this._shapeControl.BorderColor = System.Drawing.Color.Black;
			this._shapeControl.BorderStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			this._shapeControl.BorderWidth = 1;
			this._shapeControl.CenterColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this._shapeControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._shapeControl.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
			this._shapeControl.Location = new System.Drawing.Point(0, 0);
			this._shapeControl.Name = "_shapeControl";
			this._shapeControl.Shape = ShapeControl.ShapeType.RoundedRectangle;
			this._shapeControl.Size = new System.Drawing.Size(563, 138);
			this._shapeControl.SurroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this._shapeControl.TabIndex = 2;
			this._shapeControl.TabStop = false;
			this._shapeControl.UseGradient = false;
			this._shapeControl.Resize += new System.EventHandler(this._shapeControl_Resize);
			//
			// CurrentTaskIndicatorControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._indicatorPanel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._shapeControl);
			this.Name = "CurrentTaskIndicatorControl";
			this.Size = new System.Drawing.Size(563, 138);
			this.Resize += new System.EventHandler(this.CurrentTaskIndicatorControl_Resize);
			((System.ComponentModel.ISupportInitialize)(this.localizationHelper1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel _indicatorPanel;
		private WeSay.UI.LocalizationHelper localizationHelper1;
		private ShapeControl.ShapeControl _shapeControl;
	}
}
