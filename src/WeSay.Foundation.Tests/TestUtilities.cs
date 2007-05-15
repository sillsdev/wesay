using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace WeSay.Foundation.Tests
{
	public class TestUtilities
	{
		public static void DeleteFolderThatMayBeInUse(string folder)
		{
			if (Directory.Exists(folder))
			{
				try
				{
					Directory.Delete(folder, true);
				}
				catch (Exception e)
				{
					try
					{
						Console.WriteLine(e.Message);
						//maybe we can at least clear it out a bit
						string[] files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
						foreach (string s in files)
						{
							File.Delete(s);
						}
						//sleep and try again (seems to work)
						Thread.Sleep(1000);
						Directory.Delete(folder, true);
					}
					catch (Exception)
					{
					}
				}
			}
		}
	}
}
