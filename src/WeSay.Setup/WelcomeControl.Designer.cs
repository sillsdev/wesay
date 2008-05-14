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
			this.panel1 = new System.Windows.Forms.Panel();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.openRecentProject = new System.Windows.Forms.LinkLabel();
			this.openDifferentProject = new System.Windows.Forms.LinkLabel();
			this.createNewProject = new System.Windows.Forms.LinkLabel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			//
			// panel1
			//
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(111)))), ((int)(((byte)(167)))));
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(530, 45);
			this.panel1.TabIndex = 0;
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
			this.textBox1.TabIndex = 1;
			this.textBox1.TabStop = false;
			this.textBox1.Text = "Use this tool to create and configure WeSay Projects.";
			//
			// openRecentProject
			//
			this.openRecentProject.AutoSize = true;
			this.openRecentProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.openRecentProject.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.openRecentProject.LinkColor = System.Drawing.Color.Black;
			this.openRecentProject.Location = new System.Drawing.Point(111, 146);
			this.openRecentProject.Name = "openRecentProject";
			this.openRecentProject.Size = new System.Drawing.Size(180, 26);
			this.openRecentProject.TabIndex = 0;
			this.openRecentProject.TabStop = true;
			this.openRecentProject.Text = "Recently Opened";
			this.openRecentProject.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.openRecentProject_LinkClicked);
			//
			// openDifferentProject
			//
			this.openDifferentProject.AutoSize = true;
			this.openDifferentProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.openDifferentProject.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.openDifferentProject.LinkColor = System.Drawing.Color.Black;
			this.openDifferentProject.Location = new System.Drawing.Point(112, 182);
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
			this.createNewProject.Location = new System.Drawing.Point(112, 212);
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
			this.pictureBox1.Image = Resources.WelcomeImage;
			this.pictureBox1.InitialImage = null;
			this.pictureBox1.Location = new System.Drawing.Point(27, 21);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(70, 70);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			//
			// WelcomeControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.createNewProject);
			this.Controls.Add(this.openDifferentProject);
			this.Controls.Add(this.openRecentProject);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.panel1);
			this.Name = "WelcomeControl";
			this.Size = new System.Drawing.Size(530, 338);
			this.Load += new System.EventHandler(this.WelcomeControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.LinkLabel openRecentProject;
		private System.Windows.Forms.LinkLabel openDifferentProject;
		private System.Windows.Forms.LinkLabel createNewProject;

	}
}
