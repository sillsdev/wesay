using System.Windows.Forms;

namespace WeSay.UI
{
	partial class DetailList
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
			Application.RemoveMessageFilter(this);
			base.Dispose(disposing);
			this._disposed = true;
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.SuspendLayout();
			//
			// DetailList
			//
			AutoScroll = false; //but we need to make sure children are never wider than we are
			this.Name = "DetailList";
			ColumnCount = 3;
			DoubleBuffered = true;

			this.ResumeLayout(false);

		}

		#endregion
	}
}
