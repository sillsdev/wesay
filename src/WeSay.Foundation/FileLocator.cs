using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Palaso.Reporting;
using System.Linq;

namespace WeSay.Foundation
{
	public interface IFileLocator
	{
		string LocateFile(string fileName);
		string LocateFile(string fileName, string descriptionForErrorMessage);
	}

	public class FileLocator :IFileLocator
	{
		private readonly IEnumerable<string> _searchPaths;

		public FileLocator(IEnumerable<string> searchPaths  )
		{
			_searchPaths = searchPaths;
		}

		public string LocateFile(string fileName)
		{
			foreach (var path in _searchPaths)
			{
				var fullPath = Path.Combine(path, fileName);
				if(File.Exists(fullPath))
					return fullPath;
			}
			return string.Empty;
		}

		public string LocateFile(string fileName, string descriptionForErrorMessage)
		{

			var path = LocateFile(fileName);
			if (string.IsNullOrEmpty(path) || !File.Exists(path))
			{
				ErrorReport.ReportNonFatalMessage(
					"WeSay could not find the {0}.  It expected to find it in one of these locations: {1}",
					descriptionForErrorMessage, _searchPaths.Concat(", ")
					);
			}
			return path;
		}
	}


}
