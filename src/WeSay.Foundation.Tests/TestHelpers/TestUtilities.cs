using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml;
using NUnit.Framework;

namespace WeSay.Foundation.Tests.TestHelpers
{
	public class TestUtilities
	{

		public static void DeleteFolderThatMayBeInUse(string folder)
		{
			if (Directory.Exists(folder))
			{
				for (int i = 0; i < 50; i++)//wait up to five seconds
				{
					try
					{
						Directory.Delete(folder, true);
						return;
					}
					catch (Exception)
					{
					}
					Thread.Sleep(100);
				}
				//maybe we can at least clear it out a bit
				try
				{
					Debug.WriteLine("TestUtilities.DeleteFolderThatMayBeInUse(): gave up trying to delete the whole folder. Some files may be abandoned in your temp folder.");

					string[] files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
					foreach (string s in files)
					{
						File.Delete(s);
					}
					//sleep and try again
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
