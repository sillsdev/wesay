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
		public static MruProjects CreateOne
		{
			get { return new MruProjects(); }
		}

		private readonly List<string> _paths;
		//private string[] _nonstalepaths;
		private static int _numRuns = 0;

		public MruProjects()
		{
			_paths = new List<string>();
			//_nonstalepaths = new string[] {"test"};
			MorePaths = new string[] { "/home/daniel/WeSay/bhele_dictionary/bhy.WeSayConfig", "two" };
			_numRuns++;
		}

		//[XmlArrayItem(ElementName = "Path", IsNullable = false)]
		//[XmlArray(ElementName="Paths", IsNullable = true)]
		//[XmlArrayItem(ElementName= "Path",  IsNullable=true, Type = typeof(string))]
		//[XmlArray]
		//[XmlIgnore]
		public string[] Paths
		{
			get
			{
				return _paths.ToArray();
				//List<string> nspaths = new List<string>(GetNonStalePaths());
				//_nonstalepaths = new string[nspaths.Count];//paths.ToArray();
				//nspaths.CopyTo(_nonstalepaths);
				//_nonstalepaths = new string[] { "/home/daniel/WeSay/bhele_dictionary/bhy.WeSayConfig" };
				//return _nonstalepaths;
			}
			set
			{
				//_nonstalepaths = value;
				//_paths.Clear();
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

		public string[] MorePaths;

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

			Paths = new string[] { path };
			MorePaths = new string[] { "/home/daniel/WeSay/bhele_dictionary/bhy.WeSayConfig", "three" };
			//WeSay.ConfigTool.Properties.Settings.Default.Save();
			return true;
		}
	}
}