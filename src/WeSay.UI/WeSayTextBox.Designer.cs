namespace WeSay.UI
{
	partial class WeSayTextBox
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
		   // Debug.WriteLine(" TextBox " + Name + "-textBox   Disposing=" + disposing);
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
			this.SuspendLayout();
			//
			// WeSayTextBox
			//
			this.Enter += new System.EventHandler(this.WeSayTextBox_Enter);
			this.Leave += new System.EventHandler(this.WeSayTextBox_Leave);
			this.ResumeLayout(false);

		}

		#endregion
	}
}
