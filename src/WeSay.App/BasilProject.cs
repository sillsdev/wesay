using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.App
{
	public class BasilProject : WeSay.UI.IProject
	{
		private string _projectDirectoryPath;
		public BasilProject(string projectDirectoryPath)
		{
			_projectDirectoryPath = projectDirectoryPath;
		}

		public string MainDBPath
		{
			get
			{
				return System.IO.Path.Combine(_projectDirectoryPath,"thai.yap");
			}
		}
		public string Name
		{
			get
			{
				//we don't really want to give this directory out... this is just for a test
				return "Project: "+_projectDirectoryPath;
			}
		}
	}
}
