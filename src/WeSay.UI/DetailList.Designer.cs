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
			AutoScroll = true; //but we need to make sure children are never wider than we are
			this.Name = "DetailList";
			//AutoSize = true;
			//AutoSizeMode = AutoSizeMode.GrowAndShrink;
			HScroll = false;
			//VerticalScroll.Visible = true;
			ColumnCount = 2;
			DoubleBuffered = true;
			Padding = new Padding(0,0,20,0);

			this.ResumeLayout(false);

		}

		#endregion
	}
}
