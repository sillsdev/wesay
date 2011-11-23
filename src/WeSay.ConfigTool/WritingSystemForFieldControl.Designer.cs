using Palaso.UI.WindowsForms.Widgets;

namespace WeSay.ConfigTool
{
	partial class WritingSystemForFieldControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WritingSystemForFieldControl));
			this._writingSystemListBox = new BetterCheckedListBox();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.btnMoveUp = new System.Windows.Forms.Button();
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			//
			// _writingSystemListBox
			//
			this._writingSystemListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._writingSystemListBox.FormattingEnabled = true;
			this._writingSystemListBox.Items.AddRange(new object[] {
			"Foo(IPA)",
			"Foo(Thai)",
			"Thai",
			"English"});
			this._writingSystemListBox.Location = new System.Drawing.Point(15, 3);
			this._writingSystemListBox.Name = "_writingSystemListBox";
			this._writingSystemListBox.Size = new System.Drawing.Size(135, 139);
			this._writingSystemListBox.TabIndex = 18;
			this._writingSystemListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._writingSystemListBox_ItemCheck);
			this._writingSystemListBox.SelectedValueChanged += new System.EventHandler(this._writingSystemListBox_SelectedIndexChanged);
			//
			// btnMoveDown
			//
			this.btnMoveDown.FlatAppearance.BorderSize = 0;
			this.btnMoveDown.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveDown.Image")));
			this.btnMoveDown.Location = new System.Drawing.Point(-4, 53);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.Size = new System.Drawing.Size(18, 21);
			this.btnMoveDown.TabIndex = 20;
			this.btnMoveDown.UseVisualStyleBackColor = true;
			this.btnMoveDown.Click += new System.EventHandler(this.OnBtnMoveDownClick);
			//
			// btnMoveUp
			//
			this.btnMoveUp.FlatAppearance.BorderSize = 0;
			this.btnMoveUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveUp.Image")));
			this.btnMoveUp.Location = new System.Drawing.Point(-4, 32);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.Size = new System.Drawing.Size(17, 15);
			this.btnMoveUp.TabIndex = 19;
			this.btnMoveUp.UseVisualStyleBackColor = true;
			this.btnMoveUp.Click += new System.EventHandler(this.OnBtnMoveUpClick);
			//
			// WritingSystemForFieldControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.btnMoveDown);
			this.Controls.Add(this.btnMoveUp);
			this.Controls.Add(this._writingSystemListBox);
			this.Name = "WritingSystemForFieldControl";
			this.Load += new System.EventHandler(this.WritingSystemForFieldControl_Load);
			this.VisibleChanged += new System.EventHandler(this.WritingSystemForFieldControl_VisibleChanged);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnMoveDown;
		private System.Windows.Forms.Button btnMoveUp;
		private BetterCheckedListBox _writingSystemListBox;
		private System.Windows.Forms.ToolTip _toolTip;

	}
}
