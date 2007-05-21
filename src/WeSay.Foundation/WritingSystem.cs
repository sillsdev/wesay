using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Exortech.NetReflector;

namespace WeSay.Language
{
	[ReflectorType("WritingSystem")]
	public class WritingSystem
	{
		public static string IdForUnknownVernacular = "v";
		public static string IdForUnknownAnalysis = "en";


		private Font _font;
		private string _id;
		private string _keyboardName;
		private bool _rightToLeft = false;

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
		public WritingSystem()
		{
		}

		/// <summary>
		/// default for testing only
		/// </summary>
		public WritingSystem(string id, Font font)
		{
			_id = id;
			_font = font;
		}

		public override string ToString()
		{
			return Id;
		}


		[ReflectorProperty("Id", Required = true)]
		public string Id
		{
			get { return _id; }
			set
			{
				_id = value;
			}
		}

	   // [ReflectorProperty("Abbreviation", Required = true)]
		public string Abbreviation
		{
			get { return _id; }
			set
			{
				_id = value;
			}
		}

		[ReflectorProperty("RightToLeft", Required = false)]
		public bool RightToLeft
		{
			get { return _rightToLeft; }
			set
			{
				_rightToLeft = value;
			}
		}

//        //we'll be getting rid of this property
//        [Browsable(true), System.ComponentModel.DisplayName("Vernacular")]
//        public string VernacularDefault
//        {
//            get { return _id; }
//            set
//            {
//                _id = value;
//            }
//        }

		[Browsable(false)]
		public Font Font
		{
			get
			{
				if(_font == null)
				{
					_font = new Font(FontFamily.GenericSansSerif, FontSize);
				}
				return _font;
			}
			set
			{
				_font = value;
			}
		}

		[TypeConverter(typeof(KeyboardListHelper))]
		[Browsable(true)]
		[ReflectorProperty("WindowsKeyman", Required = false)]
		public string KeyboardName
		{
			get
			{
				return _keyboardName;
			}
			set
			{
				_keyboardName = value;
			}
		}

		[Browsable(false)]
		[ReflectorProperty("FontName", Required=true)]
		public string FontName
		{
			get
			{
				if (_font == null)
				{
					return "Arial";
				}
				else
				{
					return _font.Name;
				}
			}
			set
			{
				_font = new Font(value, FontSize);
			}
	   }

		[Browsable(false)]
		[ReflectorProperty("FontSize", Required = true)]
		public int FontSize
		{
			get
			{
				if (_font == null)
				{
					return 12;
				}
				else
				{
					return (int)_font.Size;
				}
			}
			set
			{
				_font = new Font(FontName, value);
			}
		}

		 public class KeyboardListHelper : StringConverter
		{
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				//true means show a combobox
				return true;
			}

			 public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
			 {
				 if ((String)value == String.Empty)
				 {
					 return "default";
				 }
				 else
				 {
					 return value;
				 }
			 }

			public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
			  {
				 if ((String)value == "default")
				 {
					 return String.Empty;
				 }
				 else
				 {
					 return value;
				 }
			 }

			 public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			 {
				 return true;
			 }

			 public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			 {
				 return true;
			 }

			 public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
			 {
				 //true will limit to list. false will show the list,
				 //but allow free-form entry
				 return true;
			 }

			 public override System.ComponentModel.TypeConverter.StandardValuesCollection
				   GetStandardValues(ITypeDescriptorContext context)
			{
				List<String> keyboards = new List<string>();
				keyboards.Add(String.Empty); // for 'default'


				KeymanLink.KeymanLink keymanLink = new KeymanLink.KeymanLink();
				if (keymanLink.Initialize(false))
				{
					foreach (KeymanLink.KeymanLink.KeymanKeyboard keyboard in keymanLink.Keyboards)
					{
						keyboards.Add(keyboard.KbdName);
					}
				}


				foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages )
				{
					keyboards.Add(lang.LayoutName);
				}
				return new StandardValuesCollection(keyboards);
			}
		}
	}
}
