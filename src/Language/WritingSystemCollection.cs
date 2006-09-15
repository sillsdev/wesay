using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;

namespace WeSay.Language
{
	public class WritingSystemCollection : Dictionary<string, WritingSystem>
	{

		private XmlDocument _fontPrefsDoc;
		private WritingSystem _vernacularWritingSystemDefault;
		private WritingSystem _analysisWritingSystemDefault;

		public void Load(string path)
		{
			_fontPrefsDoc = new XmlDocument();
			_fontPrefsDoc.Load(path);
			foreach (XmlNode node in _fontPrefsDoc.SelectNodes("prefs/writingSystem"))
			{
				WritingSystem ws = new WritingSystem(node);
				this.Add(ws.Id, ws);
			}

			string id = GetIdOfLabelledWritingSystem("analysisWritingSystem");
			_analysisWritingSystemDefault = this[id];
			id = GetIdOfLabelledWritingSystem("vernacularWritingSystem");
			_vernacularWritingSystemDefault = this[id];
		}

		private string GetIdOfLabelledWritingSystem(string label)
		{
			return _fontPrefsDoc.SelectSingleNode("prefs").Attributes[label].Value;
		}

		public WritingSystem AnalysisWritingSystemDefault
		{
			get
			{
				return _analysisWritingSystemDefault;
			}
		}

		public WritingSystem VernacularWritingSystemDefault
		{
			get
			{
				return _vernacularWritingSystemDefault;
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
