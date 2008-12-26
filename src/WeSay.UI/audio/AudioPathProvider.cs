using System;
using System.IO;

namespace WeSay.UI.audio
{
	public class AudioPathProvider
	{
		private readonly string _pathToAudioFiles;
		private readonly WeSay.Foundation.Func<string> _entryNameFunction;

		public AudioPathProvider(string pathToAudioFiles, WeSay.Foundation.Func<string> entryNameFunction)
		{
			_pathToAudioFiles = pathToAudioFiles;
			_entryNameFunction = entryNameFunction;
			if(!Directory.Exists(pathToAudioFiles))
			{
				Directory.CreateDirectory(pathToAudioFiles);
			}
		}

		public string GetNewPath()
		{
			string entryName = _entryNameFunction.Invoke();
			string name = string.IsNullOrEmpty(entryName) ? "xunknown" : entryName;
			return Path.Combine(_pathToAudioFiles, MakeSafeName(name + "-" + GetSomewhatRandomString())+".wav");
		}

		private string GetSomewhatRandomString()
		{
		   // return Guid.NewGuid().ToString();
			return (DateTime.UtcNow.Ticks/10000).ToString();//enhance... still kinda long!
		}

		private static string MakeSafeName(string fileName)
		{
			foreach (char invalChar in Path.GetInvalidFileNameChars())
			{
				fileName = fileName.Replace(invalChar.ToString(), "");
			}
			return fileName;
		}

		/// <summary>
		/// Given a relative path (i.e., the filename), turn that into a real, full path
		/// for playing & recording
		/// </summary>
		/// <param name="partial"></param>
		/// <returns></returns>
		public string GetCompletePathFromPartial(string partial)
		{
			return Path.Combine(_pathToAudioFiles, partial);
		}

		/// <summary>
		/// We only want to store the relative path (i.e. the filename)
		/// </summary>
		/// <param name="fullPath"></param>
		/// <returns></returns>
		public string GetPartialPathFromFull(string fullPath)
		{
			string pathToAudioFiles = _pathToAudioFiles;
			//make sure there is a trailing slash
			if(!pathToAudioFiles.EndsWith(Path.DirectorySeparatorChar+string.Empty))
			{
				pathToAudioFiles += Path.DirectorySeparatorChar;
			}
			//leave only the relative part (i.e., just the filename)
			return fullPath.Replace(pathToAudioFiles, "");
		}
	}
}