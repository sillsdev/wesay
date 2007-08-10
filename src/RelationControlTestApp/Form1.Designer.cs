using WeSay.UI;

namespace RelationControlTestApp
{
	partial class Form1
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.button1 = new System.Windows.Forms.Button();
			this._referenceListView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this._placeholder = new System.Windows.Forms.Button();
			this._potentialTargetsListView = new System.Windows.Forms.ListView();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// button1
			//
			this.button1.Location = new System.Drawing.Point(258, 124);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "Refresh";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			//
			// _referenceListView
			//
			this._referenceListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._referenceListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.columnHeader1,
			this.columnHeader2});
			this._referenceListView.Location = new System.Drawing.Point(12, 92);
			this._referenceListView.Name = "_referenceListView";
			this._referenceListView.Size = new System.Drawing.Size(215, 112);
			this._referenceListView.TabIndex = 3;
			this._referenceListView.UseCompatibleStateImageBehavior = false;
			this._referenceListView.View = System.Windows.Forms.View.Details;
			//
			// columnHeader1
			//
			this.columnHeader1.Text = "Key";
			this.columnHeader1.Width = 100;
			//
			// columnHeader2
			//
			this.columnHeader2.Text = "Value";
			this.columnHeader2.Width = 99;
			//
			// _placeholder
			//
			this._placeholder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._placeholder.Location = new System.Drawing.Point(13, 13);
			this._placeholder.Name = "_placeholder";
			this._placeholder.Size = new System.Drawing.Size(304, 38);
			this._placeholder.TabIndex = 4;
			this._placeholder.Text = "Placeholder for control which uses generics so breaks the designer";
			this._placeholder.UseVisualStyleBackColor = true;
			//
			// _sourceListView
			//
			this._potentialTargetsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._potentialTargetsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.columnHeader3,
			this.columnHeader4});
			this._potentialTargetsListView.Location = new System.Drawing.Point(12, 251);
			this._potentialTargetsListView.Name = "_potentialTargetsListView";
			this._potentialTargetsListView.Size = new System.Drawing.Size(215, 112);
			this._potentialTargetsListView.TabIndex = 3;
			this._potentialTargetsListView.UseCompatibleStateImageBehavior = false;
			this._potentialTargetsListView.View = System.Windows.Forms.View.Details;
			//
			// columnHeader3
			//
			this.columnHeader3.Text = "Key";
			this.columnHeader3.Width = 100;
			//
			// columnHeader4
			//
			this.columnHeader4.Text = "Value";
			this.columnHeader4.Width = 99;
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 76);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(106, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Reference Collection";
			this.label2.Click += new System.EventHandler(this.label2_Click);
			//
			// label3
			//
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(10, 235);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(90, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Source Collection";
			this.label3.Click += new System.EventHandler(this.label2_Click);
			//
			// Form1
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(345, 430);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._placeholder);
			this.Controls.Add(this._potentialTargetsListView);
			this.Controls.Add(this._referenceListView);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ListView _referenceListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button _placeholder;
		private System.Windows.Forms.ListView _potentialTargetsListView;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;

	}

}
