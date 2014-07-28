using System.Windows.Forms;

namespace WeSay.UI.AutoCompleteTextBox
{
	partial class AutoCompleteWithCreationBox<KV, ValueT>
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;


		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this._addNewButton = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			//
			// _addNewButton
			//
			this._addNewButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._addNewButton.Location = new System.Drawing.Point(110, -1);
			this._addNewButton.Name = "_addNewButton";
			this._addNewButton.Size = new System.Drawing.Size(30, 18);
			this._addNewButton.TabIndex = 1;
			this._addNewButton.Text = "+";
			this._addNewButton.UseVisualStyleBackColor = true;
			this._addNewButton.Click += new System.EventHandler(this.OnAddNewButton_Click);
			this._addNewButton.AutoSize = true;
			this._addNewButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			//
			// AutoCompleteWithCreationBox
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this._addNewButton);
			this.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.Name = "AutoCompleteWithCreationBox";
			this.Size = new System.Drawing.Size(150, 25);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _addNewButton;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}