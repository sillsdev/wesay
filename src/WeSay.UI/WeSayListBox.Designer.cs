using System.Windows.Forms;

namespace WeSay.UI
{
	partial class WeSayListBox
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
			RemoveBindingListNotifiers();
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			header = new ColumnHeader();
			components = new System.ComponentModel.Container();
			this.DoubleBuffered = true;
			this.VirtualMode = true;
			this.MultiSelect = false;
			this.HideSelection = false;

			// Set the DrawMode property to draw fixed sized items.
			this.OwnerDraw = true;

			this.View = System.Windows.Forms.View.SmallIcon;

			this.Columns.Add(header);
		}

		#endregion
		ColumnHeader header;
	}
}
