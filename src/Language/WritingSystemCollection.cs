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
//           XmlDocument _fontPrefsDoc = new XmlDocument();
//            _fontPrefsDoc.Load(path);
//            foreach (XmlNode node in _fontPrefsDoc.SelectNodes("prefs/writingSystem"))
//            {
//                WritingSystem ws = new WritingSystem(node);
//                this.Add(ws.Id, ws);
//            }
//
//            _analysisWritingSystemDefaultId = GetIdOfLabelledWritingSystem("analysisWritingSystem");
//            _vernacularWritingSystemDefaultId = GetIdOfLabelledWritingSystem("vernacularWritingSystem");

			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof(WritingSystemCollection));
			t.Add(typeof(WritingSystem));

			NetReflectorReader r = new NetReflectorReader(t);
			XmlReader reader = XmlReader.Create(path);
			r.Read(reader, this);
			reader.Close();

		}

//        private string GetIdOfLabelledWritingSystem(string label)
//        {
//            return _fontPrefsDoc.SelectSingleNode("prefs").Attributes[label].Value;
//        }

		public WritingSystem AnalysisWritingSystemDefault
		{
			get
			{
				return this[_analysisWritingSystemDefaultId];
			}
		}

		public WritingSystem VernacularWritingSystemDefault
		{
			get
			{
				return this[_vernacularWritingSystemDefaultId];
			}
		}

		[ReflectorProperty("AnalysisWritingSystemDefaultId", Required = true)]
		public string AnalysisWritingSystemDefaultId
		{
			get
			{
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
