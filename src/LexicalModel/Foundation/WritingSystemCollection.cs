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
	public class WritingSystemCollection: Dictionary<string, WritingSystem>
	{
		private LdmlInFolderWritingSystemStore _ldmlInFolderWritingSystemStore;

		public void Load(string pathToLdmlWritingSystemsFolder)
		{
			if (LdmlWritingSystemsDefinitionsExist(pathToLdmlWritingSystemsFolder))
			{
				_ldmlInFolderWritingSystemStore = new LdmlInFolderWritingSystemStore(pathToLdmlWritingSystemsFolder);
				_ldmlInFolderWritingSystemStore.LoadAllDefinitions();
				foreach (
					WritingSystemDefinition writingSystem in _ldmlInFolderWritingSystemStore.WritingSystemDefinitions)
				{
					WritingSystem wesayWritingSystem = new WritingSystem(writingSystem);
					this.Add(wesayWritingSystem.Id, wesayWritingSystem);
				}
			}
		}

		public void LoadFromLegacyWeSayFile(string PathToWritingSystemPrefsFile)
		{
			if (WeSayWritingSystemsPrefsExist(PathToWritingSystemPrefsFile))
			{
				NetReflectorReader r = new NetReflectorReader(MakeTypeTable());
				XmlReader reader = XmlReader.Create(PathToWritingSystemPrefsFile);
				WritingSystemCollection wesayWsFileCollection = new WritingSystemCollection();
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

		/// <summary>
		/// for tests that really don't care about an actual font
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public WritingSystem AddSimple(string id)
		{
			var writingSystem = new WritingSystem(id, new Font(FontFamily.GenericSansSerif, 12));
			Add(id, writingSystem);
			return writingSystem;
		}

		public new WritingSystem this[string key]
		{
			get
			{
				WritingSystem ws;
				if (!TryGetValue(key, out ws))
				{
					ws = new WritingSystem(key, new Font(FontFamily.GenericSansSerif, 12));
					Add(key, ws);
				}
				return ws;
			}
			set { base[key] = value; }
		}

		public void Write(string pathToLdmlWritingSystemsFolder)
		{
			if (_ldmlInFolderWritingSystemStore == null)
			{
				_ldmlInFolderWritingSystemStore = new LdmlInFolderWritingSystemStore(pathToLdmlWritingSystemsFolder);
			}
			foreach (KeyValuePair<string, WritingSystem> pair in this)
			{
				_ldmlInFolderWritingSystemStore.Set(pair.Value.GetAsPalasoWritingSystemDefinition());
			}
			_ldmlInFolderWritingSystemStore.Save();
			foreach (string pathToLdmlFile in Directory.GetFiles(pathToLdmlWritingSystemsFolder, "*.ldml"))
			{
				if(!this.ContainsKey(Path.GetFileNameWithoutExtension(pathToLdmlFile)))
				{
					File.Delete(pathToLdmlFile);
				}
			}
		}

		private static NetReflectorTypeTable MakeTypeTable()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof (WritingSystemCollection));
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

		//        private WritingSystem FindIndexWithoutUsingId(WritingSystem ws)
		//        {
		//            foreach (WritingSystem w in this.Values)
		//            {
		//                if (w == ws)
		//                {
		//                      return w;
		//                }
		//            }
		//            return null;
		//        }

		//        private string GetIdOfLabelledWritingSystem(string label)
		//        {
		//            return _fontPrefsDoc.SelectSingleNode("prefs").Attributes[label].Value;
		//        }

		public WritingSystem UnknownAnalysisWritingSystem
		{
			get { return this[WritingSystem.IdForUnknownAnalysis]; }
		}

		public WritingSystem UnknownVernacularWritingSystem
		{
			get { return this[WritingSystem.IdForUnknownVernacular]; }
		}

		//        public WritingSystem AnalysisWritingSystemDefault
		//        {
		//            get
		//            {
		//                WritingSystem ws;
		//                TryGetValue(_analysisWritingSystemDefaultId, out ws);
		//                return ws;
		//            }
		//        }
		//
		//        public WritingSystem VernacularWritingSystemDefault
		//        {
		//            get
		//            {
		//                WritingSystem ws;
		//                TryGetValue(_vernacularWritingSystemDefaultId, out ws);
		//                return ws;
		//            }
		//        }

		//        [ReflectorProperty("AnalysisWritingSystemDefaultId", Required = true)]
		//        public string AnalysisWritingSystemDefaultId
		//        {
		//            get
		//            {
		//                if (_analysisWritingSystemDefaultId == null)
		//                    return "";//a null would omit the xml attribute when serializing
		//
		//                return _analysisWritingSystemDefaultId;
		//            }
		//            set
		//            {
		//                _analysisWritingSystemDefaultId = value;
		//            }
		//        }
		//
		//        [ReflectorProperty("VernacularWritingSystemDefaultId", Required = true)]
		//        public string VernacularWritingSystemDefaultId
		//        {
		//            get
		//            {
		//                if (_vernacularWritingSystemDefaultId == null)
		//                    return "";//a null would omit the xml attribute when serializing
		//                return _vernacularWritingSystemDefaultId;
		//            }
		//            set
		//            {
		//                _vernacularWritingSystemDefaultId = value;
		//            }
		//        }

		public string TestWritingSystemAnalId
		{
			get { return "PretendAnalysis"; }
		}

		public string TestWritingSystemVernId
		{
			get { return "PretendVernacular"; }
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
			return this.Values.Where((ws) => !ws.IsAudio);
		}

		public IEnumerable<WritingSystem> GetAllWritingSystems()
		{
			return this.Values;
		}
	}
}