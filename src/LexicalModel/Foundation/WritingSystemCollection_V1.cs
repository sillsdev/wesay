using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.IO;
using Exortech.NetReflector;
using WeSay.LexicalModel.Foundation;
using Palaso.WritingSystems;

namespace WeSay.LexicalModel.Foundation
{
	[ReflectorType("WritingSystemCollection")]
	public class WritingSystemCollection_V1: Dictionary<string, WritingSystem_V1>
	{
		public void LoadFromLegacyWeSayFile(string PathToWritingSystemPrefsFile)
		{
			if (WeSayWritingSystemsPrefsExist(PathToWritingSystemPrefsFile))
			{
				NetReflectorReader r = new NetReflectorReader(MakeTypeTable());
				XmlReader reader = XmlReader.Create(PathToWritingSystemPrefsFile);
				var wesayWsFileCollection = new WritingSystemCollection_V1();
				try
				{
					r.Read(reader, wesayWsFileCollection);
				}
				finally
				{
					reader.Close();
				}
				foreach (KeyValuePair<string, WritingSystem_V1> pair in wesayWsFileCollection)
				{
					if (!ContainsKey(pair.Key))
					{
						Add(pair.Key, pair.Value);
					}
				}
			}

		}

		private static bool WeSayWritingSystemsPrefsExist(string pathToWritingSystemPrefsFile)
		{
			bool exists = File.Exists(pathToWritingSystemPrefsFile) && (new FileInfo(pathToWritingSystemPrefsFile).Length != 0);
			return exists;
		}

		private static NetReflectorTypeTable MakeTypeTable()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystemCollection_V1));
			t.Add(typeof (WritingSystem_V1));
			return t;
		}

		/// <summary>
		/// For serialization only
		/// </summary>
		[ReflectorArray("members", Required = true)]
		public WritingSystem_V1[] MembersAsArray
		{
			get
			{
				WritingSystem_V1[] holder = new WritingSystem_V1[Values.Count];
				Values.CopyTo(holder, 0);
				return holder;
			}
			set
			{
				Clear();
				foreach (WritingSystem_V1 w in value)
				{
					if (this.ContainsKey(w.ISO))
					{
						Palaso.Reporting.ErrorReport.NotifyUserOfProblem(
							new Palaso.Reporting.ShowOncePerSessionBasedOnExactMessagePolicy(),
							"Your writing systems file (WritingSystemPrefs.xml) contains multiple entries for {0}. Please report this problem by sending an email to issues@wesay.org.",
							w.ISO);
					}
					else
					{
						Add(w.ISO, w);
					}
				}
			}
		}
	}
}