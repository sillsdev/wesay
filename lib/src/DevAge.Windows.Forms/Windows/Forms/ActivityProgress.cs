using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// Summary description for ActivityProgress.
	/// </summary>
	public class ActivityProgress : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label labelStatus;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ActivityProgress()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			progressBar.Minimum = 0;
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

				BackgroundActivity = null;
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.labelStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// progressBar
			//
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(0, 20);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(292, 16);
			this.progressBar.TabIndex = 0;
			//
			// labelStatus
			//
			this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelStatus.Location = new System.Drawing.Point(0, 0);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(292, 20);
			this.labelStatus.TabIndex = 1;
			this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			// ActivityProgress
			//
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.progressBar);
			this.Name = "ActivityProgress";
			this.Size = new System.Drawing.Size(292, 40);
			this.ResumeLayout(false);

		}
		#endregion

		private BackgroundActivity mBackgroundActivity;
		public BackgroundActivity BackgroundActivity
		{
			get{return mBackgroundActivity;}
			set
			{
				if (mBackgroundActivity != null)
				{
					mBackgroundActivity.ActivityCompleted -= new DevAge.Patterns.ActivityEventHandler(mBackgroundActivity_ActivityCompleted);
					mBackgroundActivity.ActivityException -= new DevAge.Patterns.ActivityExceptionEventHandler(mBackgroundActivity_ActivityException);
					mBackgroundActivity.ActivityStarted -= new DevAge.Patterns.ActivityEventHandler(mBackgroundActivity_ActivityStarted);
				}

				progressBar.Minimum = 0;
				progressBar.Value = 0;

				mBackgroundActivity = value;

				if (mBackgroundActivity != null)
				{
					mBackgroundActivity.ActivityCompleted += new DevAge.Patterns.ActivityEventHandler(mBackgroundActivity_ActivityCompleted);
					mBackgroundActivity.ActivityException += new DevAge.Patterns.ActivityExceptionEventHandler(mBackgroundActivity_ActivityException);
					mBackgroundActivity.ActivityStarted += new DevAge.Patterns.ActivityEventHandler(mBackgroundActivity_ActivityStarted);
				}
			}
		}

		private string mCompletedMessage = "{0} completed!";
		public string CompletedMessage
		{
			get{return mCompletedMessage;}
			set{mCompletedMessage = value;}
		}
		private string mExceptionMessage = "{0} ERROR!";
		public string ExceptionMessage
		{
			get{return mExceptionMessage;}
			set{mExceptionMessage = value;}
		}
		private string mStartedMessage = "{0} started ...";
		public string StartedMessage
		{
			get{return mStartedMessage;}
			set{mStartedMessage = value;}
		}

		[DefaultValue("")]
		public string Message
		{
			get{return labelStatus.Text;}
			set{labelStatus.Text = value;}
		}

		private void mBackgroundActivity_ActivityCompleted(object sender, DevAge.Patterns.ActivityEventArgs e)
		{
			labelStatus.Text = string.Format(CompletedMessage, e.Activity.FullName);

			RefreshProgress(e.Activity, false);
		}

		private void mBackgroundActivity_ActivityException(object sender, DevAge.Patterns.ActivityExceptionEventArgs e)
		{
			labelStatus.Text = string.Format(ExceptionMessage, e.Activity.FullName);

			RefreshProgress(e.Activity, false);
		}

		private void mBackgroundActivity_ActivityStarted(object sender, DevAge.Patterns.ActivityEventArgs e)
		{
			labelStatus.Text = string.Format(StartedMessage, e.Activity.FullName);

			RefreshProgress(e.Activity, true);
		}

		/// <summary>
		/// Aggiorna la progress bar mostrando le attività in corso.
		/// Da notare che essendo una struttura gerarchica di attività ne visualizzo una parte alla volta.
		/// </summary>
		/// <param name="activity"></param>
		/// <param name="started"></param>
		private void RefreshProgress(DevAge.Patterns.IActivity activity, bool started)
		{
			if (started)
			{
				if (activity.SubActivities.Count > 0)
				{
					progressBar.Value = 0;
					progressBar.Maximum = activity.SubActivities.Count;
				}
				else
				{
					//Nothing
				}
			}
			else //completed or exception
			{
				if (activity.Parent == null) //root
				{
					progressBar.Value = 1;
					progressBar.Maximum = 1;
				}
				else
				{
					progressBar.Value = activity.Parent.SubActivities.IndexOf(activity) + 1;
				}
			}
		}
	}
}
