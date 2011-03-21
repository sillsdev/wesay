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
	public class WritingSystemCollection_V1: Dictionary<string, WritingSystem>
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
				foreach (KeyValuePair<string, WritingSystem> pair in wesayWsFileCollection)
				{
					if (!this.ContainsKey(pair.Key))
					{
						this.Add(pair.Key, pair.Value);
					}
				}
			}

		}

		private static bool WeSayWritingSystemsPrefsExist(string pathToWritingSystemPrefsFile)
		{
			bool exists = File.Exists(pathToWritingSystemPrefsFile) && (new FileInfo(pathToWritingSystemPrefsFile).Length != 0);
			return exists;
		}

		private static bool LdmlWritingSystemsDefinitionsExist(string pathToLdmlWritingSystemsFolder)
		{
			bool exists = Directory.Exists(pathToLdmlWritingSystemsFolder) && (Directory.GetFiles(pathToLdmlWritingSystemsFolder, "*.ldml").Length != 0);
			return exists;
		}

		public static bool WritingSystemsExistInProject(string pathToWritingSystemPrefsFile, string pathToLdmlWritingSystemsFolder)
		{
			return WeSayWritingSystemsPrefsExist(pathToWritingSystemPrefsFile) || LdmlWritingSystemsDefinitionsExist(pathToLdmlWritingSystemsFolder);
		}

		private static NetReflectorTypeTable MakeTypeTable()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystemCollection_V1));
			t.Add(typeof (WritingSystem));
			return t;
		}

		public void IdOfWritingSystemChanged(WritingSystem ws, string oldKey)
		{
			Remove(oldKey);
			//            if (_analysisWritingSystemDefaultId == oldKey)
			//            {
			//                _analysisWritingSystemDefaultId = ws.Id;
			//            }
			//            if (_vernacularWritingSystemDefaultId  == oldKey)
			//            {
			//                _vernacularWritingSystemDefaultId = ws.Id;
			//            }
			Add(ws.Id, ws);
		}

		/// <summary>
		/// For serialization only
		/// </summary>
		[ReflectorArray("members", Required = true)]
		public WritingSystem[] MembersAsArray
		{
			get
			{
				WritingSystem[] holder = new WritingSystem[Values.Count];
				Values.CopyTo(holder, 0);
				return holder;
			}
			set
			{
				Clear();
				foreach (WritingSystem w in value)
				{
					if (this.ContainsKey(w.Id))
					{
						Palaso.Reporting.ErrorReport.NotifyUserOfProblem(
							new Palaso.Reporting.ShowOncePerSessionBasedOnExactMessagePolicy(),
							"Your writing systems file (WritingSystemPrefs.xml) contains multiple entries for {0}. Please report this problem by sending an email to issues@wesay.org.",
							w.Id);
					}
					else
					{
						Add(w.Id, w);
					}
				}
			}
		}

		//private WritingSystem GetWritingSystem(string id)
		//{
		//    if (!ContainsKey(id))
		//    {
		//        System.Drawing.Font font = new System.Drawing.Font("Arial", 12, FontStyle.Italic);//italic 'cause something's wrong
		//        Add(id, new WritingSystem(id, font));
		//    }
		//    return this[id];
		//}
		public IList<string> TrimToActualTextWritingSystemIds(IList<string> ids)
		{
			var x = ids.Where((id) => !this[id].IsAudio);
			return new List<string>(x);
		}

		public IList<string> TrimToAudioWritingSystemIds(IList<string> ids)
		{
			var x = ids.Where((id) => this[id].IsAudio);
			return new List<string>(x);
		}

		public IEnumerable<WritingSystem> GetActualTextWritingSystems()
		{
			WritingSystemDefinition x;
			return this.Values.Where((ws) => !ws.IsAudio);
		}

		public IEnumerable<WritingSystem> GetAllWritingSystems()
		{
			return this.Values;
		}
	}
}