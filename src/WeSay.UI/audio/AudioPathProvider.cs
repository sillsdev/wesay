using System;
using System.IO;

namespace WeSay.UI.audio
{
	public class AudioPathProvider
	{
		private readonly string _pathToAudioFiles;
		private readonly string _entryName;

		public AudioPathProvider(string pathToAudioFiles, string entryName)
		{
			_pathToAudioFiles = pathToAudioFiles;
			_entryName = entryName;
			if(!Directory.Exists(pathToAudioFiles))
			{
				Directory.CreateDirectory(pathToAudioFiles);
			}
		}

		public string GetNewPath()
		{
			string name = string.IsNullOrEmpty(_entryName)? "xunknown" : _entryName;
			return Path.Combine(_pathToAudioFiles, MakeSafeName(name + "-" + GetSomewhatRandomString())+".wav");
		}

		private string GetSomewhatRandomString()
		{
		   // return Guid.NewGuid().ToString();
			return DateTime.UtcNow.Ticks.ToString();
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