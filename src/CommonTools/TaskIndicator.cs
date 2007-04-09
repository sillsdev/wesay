using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using WeSay.Project;

namespace WeSay.CommonTools
{
	//TODO: what is this user control doing writing the cache?
	public partial class TaskIndicator : UserControl
	{
		public event EventHandler selected = delegate {};

		private ITask _task;

		public TaskIndicator(ITask task)
		{
			if (task == null)
			{
				throw new ArgumentNullException();
			}
			InitializeComponent();
			//if (Environment.OSVersion.Platform != PlatformID.Unix)
			//{
			//    SetAutoSizeToGrowAndShrink();
			//}
			_task = task;
			this._count.Text = task.Status;
			string cachePath = Project.WeSayWordsProject.Project.PathToCache;


			//TODO: this leads to a failure when the label isn't a valid path (like when it says "failed to load: blahblah"
			string cacheFilePath = Path.Combine(cachePath, task.Label + ".cache");

			if (this._count.Text == "-")
			{
				this._count.Text = ReadCountFromCacheFile(cacheFilePath);
			}
			else
			{
				WriteCacheFile(cacheFilePath, cachePath);
			}
			this._btnName.Text = task.Label;
			this._textShortDescription.Text = task.Description;
		}

		//private void SetAutoSizeToGrowAndShrink() {
		//    //this._btnName.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		//}



		private void WriteCacheFile(string cacheFilePath, string cachePath) {
			try
			{
				if (!Directory.Exists(cachePath))
				{
					Directory.CreateDirectory(cachePath);
				}
				using (StreamWriter sw = File.CreateText(cacheFilePath))
				{
					sw.Write(this._count.Text);
				}
			}
			catch
			{
				Console.WriteLine("Could not write cache file: " + cacheFilePath);
			}
		}
		private static string ReadCountFromCacheFile(string cacheFilePath)
		{
			string s = string.Empty;
			try
			{
				if (File.Exists(cacheFilePath))
				{
					using (StreamReader sr = new StreamReader(cacheFilePath))
					{
						s = sr.ReadToEnd();
					}
				}
			}
			catch
			{
				Console.WriteLine("Could not read cache file: " + cacheFilePath);
			}
			return s;
		}

		public ITask Task
		{
			get { return this._task; }
		}

		private void TaskIndicator_BackColorChanged(object sender, EventArgs e)
		{
		   Debug.Assert(BackColor != System.Drawing.Color.Transparent);
		   this._textShortDescription.BackColor = BackColor;
		}

		private void OnBtnNameClick(object sender, EventArgs e)
		{
			selected(this, e);
		}

		//hack 'cause it wasn't resizing (well, it grew, just never shrank (no, not a simple case of wrong AutoSizeMode))
		//public void RecalcSize(object sender, EventArgs e)
		//{
		   //---this worked with the old vbox, but doesn't seem to do anyting the tablelayout one
			//this.Size = new System.Drawing.Size(this.Parent.Width - this.Left, this.Parent.Height - this.Top);

			//---the following kinda worked, but the panelenclosing this ignored our new size, so that would
			//be need to be worked out to make this worth doing.  It would allow us to increase the distance
			//between indicators when the box was thin enough to need 2 lines for description
//            using(Graphics g = this.CreateGraphics())
//            {
//               SizeF sz= g.MeasureString(this._textShortDescription.Text, this._textShortDescription.Font);
//               if (this.Width < sz.Width)
//               {
//                   this._textShortDescription.Height = (int)sz.Height*2;
//                   this.Height = this._textShortDescription.Bottom + 30;
//               }
			// notice nothing has been written yet to shrink it back if it gets wider
//            }
		//}
	}
}
