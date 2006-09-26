using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;
using Exortech.NetReflector;

namespace WeSay.Language
{
	[ReflectorType("WritingSystemCollection")]
	public class WritingSystemCollection : Dictionary<string, WritingSystem>
	{

		private string _vernacularWritingSystemDefaultId;
		private string _analysisWritingSystemDefaultId;

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

		private NetReflectorTypeTable MakeTypeTable()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof(WritingSystemCollection));
			t.Add(typeof(WritingSystem));
			return t;
		}

		public void IdOfWritingSystemChanged(WritingSystem ws, string oldKey)
		{
			Remove(oldKey);
			if (_analysisWritingSystemDefaultId == oldKey)
			{
				_analysisWritingSystemDefaultId = ws.Id;
			}
			if (_vernacularWritingSystemDefaultId  == oldKey)
			{
				_vernacularWritingSystemDefaultId = ws.Id;
			}
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

		public WritingSystem AnalysisWritingSystemDefault
		{
			get
			{
				WritingSystem ws;
				this.TryGetValue(_analysisWritingSystemDefaultId, out ws);
				return ws;
			}
		}

		public WritingSystem VernacularWritingSystemDefault
		{
			get
			{
				WritingSystem ws;
				this.TryGetValue(_vernacularWritingSystemDefaultId, out ws);
				return ws;
			}
		}

		[ReflectorProperty("AnalysisWritingSystemDefaultId", Required = true)]
		public string AnalysisWritingSystemDefaultId
		{
			get
			{
				if (_analysisWritingSystemDefaultId == null)
					return "";//a null would omit the xml attribute when serializing

				return _analysisWritingSystemDefaultId;
			}
			set
			{
				_analysisWritingSystemDefaultId = value;
			}
		}

		[ReflectorProperty("VernacularWritingSystemDefaultId", Required = true)]
		public string VernacularWritingSystemDefaultId
		{
			get
			{
				if (_vernacularWritingSystemDefaultId == null)
					return "";//a null would omit the xml attribute when serializing
				return _vernacularWritingSystemDefaultId;
			}
			set
			{
				_vernacularWritingSystemDefaultId = value;
			}
		}

		/// <summary>
		/// For serialization only
		/// </summary>
		[ReflectorArray("members", Required = true)]
		public WritingSystem[] MembersAsArray
		{
			get
			{
				WritingSystem[] holder = new WritingSystem[this.Values.Count];
				this.Values.CopyTo(holder,0);
				return holder;
			}
			set
			{
				foreach(WritingSystem w in value)
				{
					this.Add(w.Id,w);
				}
			}
		}

		private WritingSystem GetWritingSystem(string id)
		{
			if (!this.ContainsKey(id))
			{
				System.Drawing.Font font = new System.Drawing.Font("Arial", 12, FontStyle.Italic);//italic 'cause something's wrong
				this.Add(id, new WritingSystem(id, font));
			}
			return this[id];
		}

	}
}
