using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestUtilities
{
	public class TemporaryFolder
	{
		private readonly string _folderPath;

		public TemporaryFolder(): this(Directory.GetCurrentDirectory(), "TestTemporaryFolder")
		{}

		public TemporaryFolder(string path): this(path, "TestTemporaryFolder")
		{}

		public TemporaryFolder(string path, string folderName)
		{
			_folderPath = Path.Combine(path, folderName);
			if(Directory.Exists(_folderPath))
			{
				Directory.Delete(_folderPath, true);
			}
			Directory.CreateDirectory(_folderPath);
		}

		public string FolderPath
		{
			get { return _folderPath; }
		}

		public string GetTemporaryFile()
		{
			string randomFileName = Path.GetRandomFileName();
			return GetTemporaryFile(randomFileName);
		}

		public string GetTemporaryFile(string fileName)
		{
			string randomFileName = fileName;
			string pathToTempFile = Path.Combine(_folderPath, randomFileName);
			FileStream newFile = File.Create(pathToTempFile);
			newFile.Close();
			return pathToTempFile;
		}

		public void Delete()
		{
			Directory.Delete(_folderPath, true);
		}
	}
}
