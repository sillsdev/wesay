using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// Summary description for BackgroundActivity. Use the Activity property to gets or sets an activity, and the Start method to start it.
	/// </summary>
	public class BackgroundActivity : System.ComponentModel.Component, Patterns.IActivityEvents
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="container"></param>
		public BackgroundActivity(System.ComponentModel.IContainer container)
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			container.Add(this);
			InitializeComponent();
		}

		public BackgroundActivity()
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			InitializeComponent();
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

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		private System.ComponentModel.ISynchronizeInvoke mSynchronizingObject;
		public System.ComponentModel.ISynchronizeInvoke SynchronizingObject
		{
			get{return mSynchronizingObject;}
			set{mSynchronizingObject = value;}
		}

		private DevAge.Patterns.IActivity mActivity;
		/// <summary>
		/// Gets or sets the associated activity class.
		/// </summary>
		public DevAge.Patterns.IActivity Activity
		{
			get{return mActivity;}
			set{mActivity = value;}
		}

		/// <summary>
		/// Start the activity. Use the Started, Completed, Exception and Progress event.
		/// </summary>
		public void Start()
		{
			if (mActivity == null)
				throw new ArgumentNullException("Activity");

			mActivity.Start(this);
		}

		/// <summary>
		/// Fired when an activity is started.  This event is forced to be syncronized with the thread of the SyncronizingObject)
		/// </summary>
		public event Patterns.ActivityEventHandler ActivityStarted;
		/// <summary>
		/// Fired when an activity is completed.  This event is forced to be syncronized with the thread of the SyncronizingObject)
		/// </summary>
		public event Patterns.ActivityEventHandler ActivityCompleted;
		/// <summary>
		/// Fired when one of the activity throw an exception. This event is forced to be syncronized with the thread of the SyncronizingObject)
		/// </summary>
		public event Patterns.ActivityExceptionEventHandler ActivityException;

		protected virtual void OnActivityStarted(Patterns.ActivityEventArgs e)
		{
			if (ActivityStarted != null)
			{
				if (SynchronizingObject.InvokeRequired)
					SynchronizingObject.Invoke(ActivityStarted, new object[]{this, e});
				else
					ActivityStarted(this, e);
			}
		}
		protected virtual void OnActivityCompleted(Patterns.ActivityEventArgs e)
		{
			if (ActivityCompleted != null)
			{
				if (SynchronizingObject.InvokeRequired)
					SynchronizingObject.Invoke(ActivityCompleted, new object[]{this, e});
				else
					ActivityCompleted(this, e);
			}
		}
		protected virtual void OnActivityException(Patterns.ActivityExceptionEventArgs e)
		{
			if (ActivityException != null)
			{
				if (SynchronizingObject.InvokeRequired)
					SynchronizingObject.Invoke(ActivityException, new object[]{this, e});
				else
					ActivityException(this, e);
			}
		}


		#region IActivityEvents Members
		void Patterns.IActivityEvents.ActivityCompleted(DevAge.Patterns.IActivity sender)
		{
			Patterns.ActivityEventArgs args = new Patterns.ActivityEventArgs(sender);

			OnActivityCompleted(args);
		}

		void Patterns.IActivityEvents.ActivityException(DevAge.Patterns.IActivity sender, Exception exception)
		{
			Patterns.ActivityExceptionEventArgs args = new Patterns.ActivityExceptionEventArgs(sender, exception);

			OnActivityException(args);
		}

		void Patterns.IActivityEvents.ActivityStarted(DevAge.Patterns.IActivity sender)
		{
			Patterns.ActivityEventArgs args = new Patterns.ActivityEventArgs(sender);

			OnActivityStarted(args);
		}
		#endregion
	}
}
