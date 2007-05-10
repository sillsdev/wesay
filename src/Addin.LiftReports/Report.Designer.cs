namespace LiftReports
{
	partial class Report
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this._countLabel = new System.Windows.Forms.Label();
			this._senseChart = new Addin.LiftReports.XPathChart();
			this._overviewChart = new Addin.LiftReports.XPathChart();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(93, 37);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(158, 26);
			this.label1.TabIndex = 1;
			this.label1.Text = "Lexicon Report";
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(94, 150);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(147, 24);
			this.label2.TabIndex = 2;
			this.label2.Text = "Entries Contents";
			//
			// label3
			//
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(94, 471);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(152, 24);
			this.label3.TabIndex = 2;
			this.label3.Text = "Senses Contents";
			//
			// pictureBox1
			//
			this.pictureBox1.Image = global::Addin.LiftReports.Properties.Resources.WeSay;
			this.pictureBox1.Location = new System.Drawing.Point(18, 13);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(69, 50);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			//
			// _countLabel
			//
			this._countLabel.AutoSize = true;
			this._countLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._countLabel.Location = new System.Drawing.Point(94, 99);
			this._countLabel.Name = "_countLabel";
			this._countLabel.Size = new System.Drawing.Size(60, 24);
			this._countLabel.TabIndex = 4;
			this._countLabel.Text = "label4";
			//
			// _senseChart
			//
			this._senseChart.BackColor = System.Drawing.Color.White;
			this._senseChart.Location = new System.Drawing.Point(3, 483);
			this._senseChart.Name = "_senseChart";
			this._senseChart.PathToXmlDocument = null;
			this._senseChart.Size = new System.Drawing.Size(353, 300);
			this._senseChart.TabIndex = 0;
			this._senseChart.Load += new System.EventHandler(this._senseChart_Load);
			//
			// _overviewChart
			//
			this._overviewChart.BackColor = System.Drawing.Color.White;
			this._overviewChart.Location = new System.Drawing.Point(3, 163);
			this._overviewChart.Name = "_overviewChart";
			this._overviewChart.PathToXmlDocument = null;
			this._overviewChart.Size = new System.Drawing.Size(353, 298);
			this._overviewChart.TabIndex = 0;
			this._overviewChart.Load += new System.EventHandler(this._overviewChart_Load);
			//
			// Report
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._countLabel);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._senseChart);
			this.Controls.Add(this._overviewChart);
			this.Name = "Report";
			this.Size = new System.Drawing.Size(541, 755);
			this.Load += new System.EventHandler(this.Report_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Addin.LiftReports.XPathChart _overviewChart;
		private System.Windows.Forms.Label label1;
		private Addin.LiftReports.XPathChart _senseChart;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label _countLabel;
	}
}
