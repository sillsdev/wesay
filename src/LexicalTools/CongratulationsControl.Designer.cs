using System.Windows.Forms;

namespace WeSay.LexicalTools
{
    partial class CongratulationsControl
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
            this.checkmarkLabel = new System.Windows.Forms.Label();
            this._statusMessage2 = new System.Windows.Forms.Label();
            this._messageText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // checkmarkLabel
            // 
            this.checkmarkLabel.AutoSize = true;
            this.checkmarkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkmarkLabel.Location = new System.Drawing.Point(19, 21);
            this.checkmarkLabel.Name = "checkmarkLabel";
            this.checkmarkLabel.Size = new System.Drawing.Size(45, 36);
            this.checkmarkLabel.TabIndex = 1;
            // 
            // _statusMessage2
            // 
            this._statusMessage2.Location = new System.Drawing.Point(0, 0);
            this._statusMessage2.Name = "_statusMessage2";
            this._statusMessage2.Size = new System.Drawing.Size(100, 23);
            this._statusMessage2.TabIndex = 0;
            // 
            // _messageText
            // 
            this._messageText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._messageText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._messageText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._messageText.Location = new System.Drawing.Point(67, 35);
            this._messageText.Multiline = true;
            this._messageText.Name = "_messageText";
            this._messageText.Size = new System.Drawing.Size(554, 166);
            this._messageText.TabIndex = 2;
            // 
            // CongratulationsControl
            // 
            this.Controls.Add(this._messageText);
            this.Controls.Add(this.checkmarkLabel);
            this.Name = "CongratulationsControl";
            this.Size = new System.Drawing.Size(659, 227);
            this.BackColorChanged += new System.EventHandler(this.CongratulationsControl_BackColorChanged);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label checkmarkLabel;
        private Label _statusMessage2;
        private TextBox _messageText;
    }
}
