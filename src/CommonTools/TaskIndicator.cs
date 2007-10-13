using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using WeSay.Foundation;
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
		   // this._count.Text = task.Status;
			_intray.Status = task.Status;
			_intray.ReferenceCount = task.ReferenceCount;

			string cachePath = WeSayWordsProject.Project.PathToCache;


			string cacheFilePath = Path.Combine(cachePath, MakeSafeName(task.Label + ".cache"));

			if (task.Status == "-")
			{
				ReadCountFromCacheFile(cacheFilePath);
			}
			else
			{
				WriteCacheFile(cacheFilePath, cachePath);
			}
			//_btnName.Font = StringCatalog.LabelFont;
			this._btnName.Text = task.Label;//these have already gone through the StringCatalog
			this._textShortDescription.Text = task.Description;//these have already gone through the StringCatalog
			_textShortDescription.Font = StringCatalog.ModifyFontForLocalization(_textShortDescription.Font);
		}

		private string MakeSafeName(string fileName)
		{
			foreach (char invalChar in Path.GetInvalidFileNameChars())
			{
				fileName = fileName.Replace(invalChar.ToString(), "");
			}
			return fileName;
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
					sw.Write(_intray.Count +", "+_intray.ReferenceCount);
				}
			}
			catch
			{
				Console.WriteLine("Could not write cache file: " + cacheFilePath);
			}
		}
		private  void ReadCountFromCacheFile(string cacheFilePath)
		{
			try
			{
				if (File.Exists(cacheFilePath))
				{
					using (StreamReader sr = new StreamReader(cacheFilePath))
					{
						string s;
						s = sr.ReadToEnd();
						string[] values = s.Split(',');
						if (values.Length > 1) //old style didn't have reference
						{
							int r;
							bool gotIt = int.TryParse(values[1], out r);
							_intray.ReferenceCount = r;
							Debug.Assert(gotIt);
						}
						if (values.Length > 0) //old style didn't have reference
						{
							_intray.Status = values[0]; // will convert and fill in count if it can
						}
					}
				}
			}
			catch
			{
				// Console.WriteLine("Could not read cache file: " + cacheFilePath);
			}
		}

		public ITask Task
		{
			get { return this._task; }
		}

		private void TaskIndicator_BackColorChanged(object sender, EventArgs e)
		{
		   Debug.Assert(BackColor != System.Drawing.Color.Transparent);
		   this._textShortDescription.BackColor = BackColor;
		   _intray.BackColor = BackColor;
		}

		private void OnBtnNameClick(object sender, EventArgs e)
		{
			selected(this, e);
		}

		private void _btnName_FontChanged(object sender, EventArgs e)
		{

		}

		private void TaskIndicator_Resize(object sender, EventArgs e)
		{
			//int w = this.Width;
		}

	}
}
