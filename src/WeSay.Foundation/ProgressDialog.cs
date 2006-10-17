//originally from Matthew Adams

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using WeSay;

namespace MultithreadProgress
{
	/// <summary>
	/// Provides a progress dialog similar to the one shown by Windows
	/// </summary>
	public class ProgressDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.Label statusLabel1;
		private System.Windows.Forms.Label statusLabel2;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label progressLabel;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;
		private bool showOnce;
		private System.Windows.Forms.Timer progressTimer;
		private bool isClosing;
		private DateTime startTime = DateTime.Now;

		/// <summary>
		/// Standard constructor
		/// </summary>
		public ProgressDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Get / set the time in ms to delay
		/// before showing the dialog
		/// </summary>
		public int DelayShowInterval
		{
			get
			{
				return timer1.Interval;
			}
			set
			{
				timer1.Interval = value;
			}
		}

//		/// <summary>
//		/// Get / set the image to display in the animation control area
//		/// </summary>
//		public Image ProgressImage
//		{
//			get
//			{
//				return pictureBox.Image;
//			}
//			set
//			{
//				pictureBox.Image = value;
//			}
//		}

		/// <summary>
		/// Get / set the text to display in the first status panel
		/// </summary>
		public string StatusText1
		{
			get
			{
				return statusLabel1.Text;
			}
			set
			{
				statusLabel1.Text = value;
			}
		}

		/// <summary>
		/// Get / set the text to display in the second status panel
		/// </summary>
		public string StatusText2
		{
			get
			{
				return statusLabel2.Text;
			}
			set
			{
				statusLabel2.Text = value;
			}
		}

		/// <summary>
		/// Get / set the minimum range of the progress bar
		/// </summary>
		public int ProgressRangeMinimum
		{
			get
			{
				return progressBar.Minimum;
			}
			set
			{
				progressBar.Minimum = value;
			}
		}

		/// <summary>
		/// Get / set the maximum range of the progress bar
		/// </summary>
		public int ProgressRangeMaximum
		{
			get
			{
				return progressBar.Maximum;
			}
			set
			{
				progressBar.Maximum = value;
			}
		}

		/// <summary>
		/// Get / set the current value of the progress bar
		/// </summary>
		public int Progress
		{
			get
			{
				return progressBar.Value;
			}
			set
			{
				progressBar.Value = value;
			}
		}

		/// <summary>
		/// Get/set a boolean which determines whether the form
		/// will show a cancel option (true) or not (false)
		/// </summary>
		public bool CanCancel
		{
			get
			{
				return cancelButton.Enabled;
			}
			set
			{
				cancelButton.Enabled = value;
			}
		}

		/// <summary>
		/// Show the control, but honor the
		/// <see cref="DelayShowInterval"/>.
		/// </summary>
		public void DelayShow()
		{
			// This creates the control, but doesn't
			// show it; you can't use CreateControl()
			// here, because it will return because
			// the control is not visible
			CreateHandle();
		}

		/// <summary>
		/// Close the dialog, ignoring cancel status
		/// </summary>
		public void ForceClose()
		{
			isClosing = true;
			Close();
		}

		/// <summary>
		/// Raised when the cancel button is clicked
		/// </summary>
		public event EventHandler Cancelled;

		/// <summary>
		/// Raises the cancelled event
		/// </summary>
		/// <param name="e">Event data</param>
		protected virtual void OnCancelled( EventArgs e )
		{
			EventHandler cancelled = Cancelled;
			if( cancelled != null )
			{
				cancelled( this, e );
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Custom handle creation code
		/// </summary>
		/// <param name="e">Event data</param>
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated (e);
			if( !showOnce )
			{
				// First, we don't want this to happen again
				showOnce = true;
				// Then, start the timer which will determine whether
				// we are going to show this again
				timer1.Start();
			}
		}

		/// <summary>
		/// Custom close handler
		/// </summary>
		/// <param name="e">Event data</param>
		protected override void OnClosing(CancelEventArgs e)
		{
			if( !isClosing )
			{
				e.Cancel = true;
				cancelButton.PerformClick();
			}
			base.OnClosing( e );
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressDialog));
			this.statusLabel1 = new System.Windows.Forms.Label();
			this.statusLabel2 = new System.Windows.Forms.Label();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.cancelButton = new System.Windows.Forms.Button();
			this.progressLabel = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.progressTimer = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			//
			// statusLabel1
			//
			resources.ApplyResources(this.statusLabel1, "statusLabel1");
			this.statusLabel1.Name = "statusLabel1";
			//
			// statusLabel2
			//
			resources.ApplyResources(this.statusLabel2, "statusLabel2");
			this.statusLabel2.Name = "statusLabel2";
			//
			// pictureBox
			//
			resources.ApplyResources(this.pictureBox, "pictureBox");
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.TabStop = false;
			//
			// progressBar
			//
			resources.ApplyResources(this.progressBar, "progressBar");
			this.progressBar.Name = "progressBar";
			//
			// cancelButton
			//
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			//
			// progressLabel
			//
			resources.ApplyResources(this.progressLabel, "progressLabel");
			this.progressLabel.Name = "progressLabel";
			//
			// timer1
			//
			this.timer1.Interval = 2000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			//
			// progressTimer
			//
			this.progressTimer.Enabled = true;
			this.progressTimer.Interval = 1000;
			this.progressTimer.Tick += new System.EventHandler(this.progressTimer_Tick);
			//
			// ProgressDialog
			//
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.cancelButton;
			this.ControlBox = false;
			this.Controls.Add(this.progressLabel);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.pictureBox);
			this.Controls.Add(this.statusLabel2);
			this.Controls.Add(this.statusLabel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProgressDialog";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion


		private void timer1_Tick(object sender, System.EventArgs e)
		{
			// Show the window now the timer has elapsed, and stop the timer
			timer1.Stop();
			this.Show();
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			// Prevent further cancellation
			CanCancel = false;
			progressTimer.Stop();
			progressLabel.Text =  "Canceling...";
			// Tell people we're canceling
			OnCancelled( e );
		}

		private void progressTimer_Tick(object sender, System.EventArgs e)
		{
			int range = progressBar.Maximum - progressBar.Minimum;
			if( range <= 0 )
			{
				return;
			}
			if( progressBar.Value <= 0 )
			{
				return;
			}
			TimeSpan elapsed = DateTime.Now - startTime;
			double estimatedSeconds = (elapsed.TotalSeconds * (double) range) / (double)progressBar.Value;
			TimeSpan estimatedToGo = new TimeSpan(0,0,0,(int)(estimatedSeconds - elapsed.TotalSeconds),0);
//			progressLabel.Text = String.Format(
//				System.Globalization.CultureInfo.CurrentUICulture,
//                "Elapsed: {0} Remaining: {1}",
//				GetStringFor(elapsed),
//				GetStringFor(estimatedToGo) );
			progressLabel.Text = String.Format(
				System.Globalization.CultureInfo.CurrentUICulture,
				"{0}",
				//GetStringFor(elapsed),
				GetStringFor(estimatedToGo));
		}

		private static string GetStringFor( TimeSpan span )
		{
			if( span.TotalDays > 1 )
			{
				return string.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0} day {1} hour", span.Days, span.Hours);
			}
			else if( span.TotalHours > 1 )
			{
				return string.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0} hour {1} min", span.Hours, span.Minutes);
			}
			else if( span.TotalMinutes > 1 )
			{
				return string.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0} min {1}s", span.Minutes, span.Seconds);
			}
			return string.Format( System.Globalization.CultureInfo.CurrentUICulture, "{0}s", span.Seconds );
		}

		private static string GetResourceString( string name )
		{
			System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager( "MultiThreadProgress.Strings", typeof( ProgressDialog ).Assembly );
			return resourceManager.GetString( name, System.Globalization.CultureInfo.CurrentUICulture );
		}
	}
}
