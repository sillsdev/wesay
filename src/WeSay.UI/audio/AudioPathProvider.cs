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
	}
}