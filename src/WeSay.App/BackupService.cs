using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using com.db4o.query;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.App
{
	public class BackupService
	{
		private const string s_backupPointFileName = "backuppoint";
		private const int _checkFrequency = 10;
		private int _commitCount;
		private string _directory;
		private Db4oDataSource _datasource;
		private DateTime _timeOfLastQueryForNewRecords;

		event EventHandler BackingUp;

		public BackupService(string directory, Db4oDataSource datasource)
		{
			_directory = directory;
			_datasource = datasource;
		}

		/// <summary>
		/// Give a chance to do incremental backup if warranted
		/// </summary>
		/// <remarks>
		/// If the caller doesn't know when actual comitts happen, that's ok.
		/// Just call at reasonable intervals.
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnDataCommitted(object sender, EventArgs e)
		{
			_commitCount++;
			if (_commitCount < _checkFrequency)
			{
				return;
			}
			_commitCount = 0;
			DoIncrementalXmlBackupNow();
		}

		public void BackupToExternal(string path)
		{
			if(!Directory.Exists(Path.GetPathRoot(path)))
				return;

			ProgressDialog dlg = new ProgressDialog("Backing up to external...");
			dlg.Show();

			try
			{
				ICSharpCode.SharpZipLib.Zip.FastZip f = new ICSharpCode.SharpZipLib.Zip.FastZip();
				f.CreateZip(path, WeSayWordsProject.Project.PathToLocalBackup, true, null);
			}
			finally
			{
				if (dlg != null)
				{
					dlg.Close();
					dlg.Dispose();
				}
			}
		}

		public void DoIncrementalXmlBackupNow( )
		{
			if (BackingUp != null)
			{
				BackingUp.Invoke(this, null);
			}

			if (!Directory.Exists(_directory))
			{
				Directory.CreateDirectory(_directory);
			}

			IList records = GetRecordsNeedingBackup();
			if (records.Count == 0)
			{
				return;
			}

			ProgressDialog dlg=null;
			if (records.Count > 50)
			{
				dlg = new ProgressDialog(string.Format("Doing incremental backup of {0} records...", records.Count));
				dlg.Show();
				Application.DoEvents();
			}

			try
			{
				LiftExporter exporter = new LiftExporter(MakeBackupFileName());
				exporter.AddNoGeneric(records);
				exporter.End();

				WriteBackupPointFile();
			}
			finally
			{
				if (dlg != null)
				{
					dlg.Close();
					dlg.Dispose();
				}
			}

			//the granularity of the file access time stamp is too blunt, so we
			//avoid missing changes with this hack, for now (have *not* tested how small it could be)
			Thread.Sleep(50);


		}

		private  string MakeBackupFileName()
		{
			string timeString = _timeOfLastQueryForNewRecords.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss'-'FFFFF");
			string path = Path.Combine(_directory, timeString);
			path += ".lift.frag.xml";
			return path;
		}

		private  void WriteBackupPointFile()
		{
			string file = Path.Combine(_directory, s_backupPointFileName);
			if (!File.Exists(file))
			{
				File.Create(file).Close();
			}

			File.SetLastWriteTime(file, _timeOfLastQueryForNewRecords);
		}

		private  DateTime GetLastBackupTime()
		{
			Debug.Assert(Directory.Exists(_directory));
			string file = Path.Combine(_directory, s_backupPointFileName);
			if (!File.Exists(file))
			{
				return DateTime.MinValue;
			}
			else
			{
				return File.GetLastWriteTime(file);
			}
		}

		public IList  GetRecordsNeedingBackup()
		{
			DateTime last = GetLastBackupTime();
			Query q =this._datasource.Data.Query();
			q.Constrain(typeof(LexEntry));
			//REVIEW: this is >, not >=. Could a change get lost if the
			//record was modified milliseconds before the last backup?
			q.Descend("_modifiedDate").Constrain(last).Greater();
			_timeOfLastQueryForNewRecords = DateTime.Now;
			return q.Execute();
		}
	}
}