using Resources=WeSay.ConfigTool.Properties.Resources;

namespace WeSay.ConfigTool
{
	partial class WelcomeControl
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
			this.blueBar = new System.Windows.Forms.Panel();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.openDifferentProject = new System.Windows.Forms.LinkLabel();
			this.createNewProject = new System.Windows.Forms.LinkLabel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.recentProjectsLabel = new System.Windows.Forms.Label();
			this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.secondCellPanel = new System.Windows.Forms.Panel();
			this.firstCellPanel = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.secondCellPanel.SuspendLayout();
			this.firstCellPanel.SuspendLayout();
			this.SuspendLayout();
			//
			// blueBar
			//
			this.blueBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(111)))), ((int)(((byte)(167)))));
			this.blueBar.Dock = System.Windows.Forms.DockStyle.Top;
			this.blueBar.Location = new System.Drawing.Point(0, 0);
			this.blueBar.Name = "blueBar";
			this.blueBar.Size = new System.Drawing.Size(530, 45);
			this.blueBar.TabIndex = 3;
			//
			// textBox1
			//
			this.textBox1.BackColor = System.Drawing.Color.White;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBox1.Location = new System.Drawing.Point(116, 73);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(411, 20);
			this.textBox1.TabIndex = 4;
			this.textBox1.TabStop = false;
			this.textBox1.Text = "Use this tool to create and configure WeSay Projects.";
			//
			// openDifferentProject
			//
			this.openDifferentProject.AutoSize = true;
			this.openDifferentProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.openDifferentProject.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.openDifferentProject.LinkColor = System.Drawing.Color.Black;
			this.openDifferentProject.Location = new System.Drawing.Point(3, 0);
			this.openDifferentProject.Name = "openDifferentProject";
			this.openDifferentProject.Size = new System.Drawing.Size(167, 20);
			this.openDifferentProject.TabIndex = 1;
			this.openDifferentProject.TabStop = true;
			this.openDifferentProject.Text = "Open Different Project";
			this.openDifferentProject.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.openDifferentProject_LinkClicked);
			//
			// createNewProject
			//
			this.createNewProject.AutoSize = true;
			this.createNewProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.createNewProject.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.createNewProject.LinkColor = System.Drawing.Color.Black;
			this.createNewProject.Location = new System.Drawing.Point(3, 20);
			this.createNewProject.Name = "createNewProject";
			this.createNewProject.Size = new System.Drawing.Size(145, 20);
			this.createNewProject.TabIndex = 2;
			this.createNewProject.TabStop = true;
			this.createNewProject.Text = "Create New Project";
			this.createNewProject.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.createNewProject_LinkClicked);
			//
			// pictureBox1
			//
			this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.pictureBox1.Image = global::WeSay.ConfigTool.Properties.Resources.WelcomeImage;
			this.pictureBox1.InitialImage = null;
			this.pictureBox1.Location = new System.Drawing.Point(27, 21);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(70, 70);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			//
			// recentProjectsLabel
			//
			this.recentProjectsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.recentProjectsLabel.Location = new System.Drawing.Point(3, 0);
			this.recentProjectsLabel.Name = "recentProjectsLabel";
			this.recentProjectsLabel.Size = new System.Drawing.Size(168, 20);
			this.recentProjectsLabel.TabIndex = 6;
			this.recentProjectsLabel.Text = "Open Recent Project";
			//
			// flowLayoutPanel2
			//
			this.flowLayoutPanel2.AutoSize = true;
			this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel2.Location = new System.Drawing.Point(28, 23);
			this.flowLayoutPanel2.MaximumSize = new System.Drawing.Size(0, 200);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Size = new System.Drawing.Size(0, 0);
			this.flowLayoutPanel2.TabIndex = 0;
			this.flowLayoutPanel2.WrapContents = false;
			//
			// tableLayoutPanel1
			//
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.secondCellPanel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.firstCellPanel, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(85, 123);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(180, 95);
			this.tableLayoutPanel1.TabIndex = 9;
			//
			// secondCellPanel
			//
			this.secondCellPanel.Controls.Add(this.createNewProject);
			this.secondCellPanel.Controls.Add(this.openDifferentProject);
			this.secondCellPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.secondCellPanel.Location = new System.Drawing.Point(3, 35);
			this.secondCellPanel.Name = "secondCellPanel";
			this.secondCellPanel.Size = new System.Drawing.Size(174, 57);
			this.secondCellPanel.TabIndex = 7;
			//
			// firstCellPanel
			//
			this.firstCellPanel.AutoSize = true;
			this.firstCellPanel.Controls.Add(this.flowLayoutPanel2);
			this.firstCellPanel.Controls.Add(this.recentProjectsLabel);
			this.firstCellPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.firstCellPanel.Location = new System.Drawing.Point(3, 3);
			this.firstCellPanel.Name = "firstCellPanel";
			this.firstCellPanel.Size = new System.Drawing.Size(174, 26);
			this.firstCellPanel.TabIndex = 8;
			//
			// WelcomeControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.blueBar);
			this.Name = "WelcomeControl";
			this.Size = new System.Drawing.Size(530, 338);
			this.Load += new System.EventHandler(this.WelcomeControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.secondCellPanel.ResumeLayout(false);
			this.secondCellPanel.PerformLayout();
			this.firstCellPanel.ResumeLayout(false);
			this.firstCellPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel blueBar;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.LinkLabel openDifferentProject;
		private System.Windows.Forms.LinkLabel createNewProject;
		private System.Windows.Forms.Label recentProjectsLabel;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel secondCellPanel;
		private System.Windows.Forms.Panel firstCellPanel;

	}
}
