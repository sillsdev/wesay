using System;
using WeSay.Foundation;

namespace WeSay.UI
{
	partial class MultiTextControl
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
			//Debug.WriteLine("Disposing " + Name + "   Disposing=" + disposing);
			if (disposing && (components != null))
			{
				//foreach (WeSayTextBox box in _textBoxes)
				//{
				//    box.SizeChanged -= box_SizeChanged;
				//}

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
			// MultiTextControl
			//
			this.BackColor = System.Drawing.Color.White;
			this.Name = "MultiTextControl";
			this.AutoSize = true;

			//this.Height = 1;
			//this.Width = 1;
			this.ResumeLayout(false);

		}


		#endregion


	}
}
