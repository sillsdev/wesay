using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace WeSay.ConfigTool
{
	[Serializable]
	[XmlRoot("RecentlyUsedFiles")]
	public class MruProjects
	{
		static public MruProjects CreateOne
	{
		get
		{
			return new MruProjects();
		}

	}
		private readonly List<string> _paths;
		public MruProjects()
		{
			_paths = new List<string>();
		}
		[XmlElement("Path")]
		public string[] Paths
		{
			get
			{
				List<string> paths = new List<string>(GetNonStalePaths());
				return paths.ToArray();
			}
			set
			{
				 _paths.Clear();
				if (value != null)
				{
					foreach (string path in value)
					{
						if (!_paths.Contains(path))
						{
							_paths.Add(path);
						}
					}
				}

			}
		}

		[XmlIgnore]
		public string Latest
		{
			get
			{
				foreach (string path in GetNonStalePaths())
				{
					return path;
				}
				return null;
			}
		}

		private IEnumerable<string> GetNonStalePaths()
		{
		   foreach (string path in _paths)
			{
				if (File.Exists(path))
				{
					yield return path;
				}
			}
		}

		/// <summary>
		/// Adds path to top of list of most recently used files if it exists (returns false if it doesn't exist)
		/// </summary>
		/// <param name="path"></param>
		/// <returns>true if successful, false if given file does not exist</returns>
		public bool AddNewPath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (!File.Exists(path))
			{
				return false;
			}
			_paths.Remove(path);

			_paths.Insert(0, path);
			return true;
		}
	}
}