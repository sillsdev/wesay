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
			_task = task;
			this._count.Text = task.Status;
			string cachePath = WeSayWordsProject.Project.PathToCache;


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
				// Console.WriteLine("Could not read cache file: " + cacheFilePath);
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

	}
}
