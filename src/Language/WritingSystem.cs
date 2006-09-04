
using System.Drawing;
using System.Xml;

namespace WeSay.Language
{
	public class WritingSystem
	{

		private Font _font;
		private string _id;

//        public WritingSystem(string filePath)
//        {
//            _fontPrefsDoc = new XmlDocument();
//            _fontPrefsDoc.Load(filePath);
//            XmlNode node = _fontPrefsDoc.SelectSingleNode("writingSystemPrefs");
//            _id = node.Attributes["id"].Value;
//        }
		public WritingSystem(XmlNode node)
		{
			_id = node.Attributes["id"].Value;
			XmlNode fontNode = node.SelectSingleNode("font");
			string name = fontNode.Attributes["name"].Value;
			float size = float.Parse(fontNode.Attributes["baseSize"].Value);

			_font= new Font(name, size);
		}

		/// <summary>
		/// default for testing only
		/// </summary>
		public WritingSystem(string id, Font font)
		{
			_id = id;
			_font = font;
		}



		public Font Font
		{
			get
			{
				return _font;
			}
		}

		public string Id
		{
			get { return _id; }
		}



	}
}
