using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using WeSay.Foundation;

namespace WeSay.AddinLib
{
	public class AddinSet
	{
		private static AddinSet _singleton;
		private FileLocater _locateFile;
		private Dictionary<string, string> _addinSettings = null;
		private List<string> _addinsToShowInWeSay = new List<string>();

		public delegate XmlNodeList AddinSettingsGetter();

		private AddinSettingsGetter _getSettingsNodesFromConfiguration;

		public static AddinSet Create(AddinSettingsGetter getSettingsNodesFromConfiguration, FileLocater locateFile)
		{
		   _singleton = new AddinSet(getSettingsNodesFromConfiguration,locateFile);

		   return _singleton;
		}


		public  static AddinSet Singleton
		{
			get
			{
				return _singleton;
			}
		}

		public FileLocater LocateFile
		{
			get
			{
				return _locateFile;
			}
		}

		private AddinSet(AddinSettingsGetter getSettingsNodesFromConfiguration,
			FileLocater locateFile)
		{
			_locateFile = locateFile;
			_getSettingsNodesFromConfiguration = getSettingsNodesFromConfiguration;
		}

		public string GetSettingsXmlForAddin(String id)
		{
			InitializeIfNeeded();
			string settings;
			_addinSettings.TryGetValue(id, out settings);
			return settings;
		}

		public void SetSettingsForAddin(string id, string settingsXml)
		{
			if (_addinSettings.ContainsKey(id))
			{
				_addinSettings.Remove(id);
			}
			_addinSettings.Add(id, settingsXml);
		}

		public void SetDoShowInWeSay(string id, bool doShow)
		{
			if (_addinsToShowInWeSay.Contains(id))
			{
				_addinsToShowInWeSay.Remove(id);
			}

			if (doShow)
			{
				_addinsToShowInWeSay.Add(id);
			}
		}

		public bool DoShowInWeSay(string id)
		{
			InitializeIfNeeded();
			return _addinsToShowInWeSay.Contains(id);
		}

		public void InitializeIfNeeded()
		{
			if (_addinSettings == null)
			{
				Load();
			}
		}


		private void Load()
		{
			_addinSettings = new Dictionary<string, string>();
			XmlNodeList nodes = _getSettingsNodesFromConfiguration();
			if (nodes != null)
			{
				foreach (XmlNode node in nodes)
				{
					string sid = XmlUtils.GetManditoryAttributeValue(node, "id");
					// Guid id = new Guid(sid);
					SetDoShowInWeSay(sid, XmlUtils.GetBooleanAttributeValue(node, "showInWeSay"));
					string contents = node.InnerXml;
					if (!String.IsNullOrEmpty(contents))
					{
						_addinSettings.Add(sid, contents);
					}
				}
			}
		}

		public void Save(XmlWriter writer)
		{
			Debug.Assert(_addinSettings != null, "Call InitializeIfNeeded before Save");

			writer.WriteStartElement("addins");

			List<string> leftToOutput = new List<string>(_addinsToShowInWeSay);


			foreach (KeyValuePair<string, string> pair in _addinSettings)
			{
				WriteAddinNode(pair.Key, pair.Value, writer);
				leftToOutput.Remove(pair.Key);
			}
			//get any who don't have settings
			foreach (string id in leftToOutput)
			{
				WriteAddinNode(id, null, writer);
			}
			writer.WriteEndElement();
		}

		private void WriteAddinNode(string id, string settingsXml, XmlWriter writer)
		{
			writer.WriteStartElement("addin");
			writer.WriteAttributeString("id", id.ToString());
		   // writer.WriteAttributeString("name", name);
			writer.WriteAttributeString("showInWeSay", DoShowInWeSay(id).ToString());

			if (settingsXml != null)
			{
				writer.WriteRaw(settingsXml);
			}
			writer.WriteEndElement();
		}
	}
}
