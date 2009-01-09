using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;
using Exortech.NetReflector;

namespace WeSay.Foundation
{
	[ReflectorType("WritingSystemCollection")]
	public class WritingSystemCollection: Dictionary<string, WritingSystem>
	{
		public void Load(string path)
		{
			NetReflectorReader r = new NetReflectorReader(MakeTypeTable());
			XmlReader reader = XmlReader.Create(path);
			try
			{
				r.Read(reader, this);
			}
			finally
			{
				reader.Close();
			}
		}

		/// <summary>
		/// for tests that really don't care about an actual font
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public WritingSystem AddSimple(string id)
		{
			var writingSystem = new WritingSystem(id, new Font(FontFamily.GenericSansSerif, 10));
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

		public void Write(XmlWriter writer)
		{
			try
			{
				writer.WriteStartDocument();
				NetReflector.Write(writer, this);
			}
			finally
			{
				writer.Close();
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
					Add(w.Id, w);
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
	}
}