using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using Mono.Addins;
using Palaso.Reporting;
using Palaso.Xml;
using WeSay.Foundation;

namespace WeSay.AddinLib
{
	public class AddinSet
	{
		private static AddinSet _singleton;
		private readonly FileLocater _locateFile;
		private Dictionary<string, string> _addinSettings;
		private readonly List<string> _addinsToShowInWeSay = new List<string>();

		public delegate XPathNodeIterator AddinSettingsGetter();

		private readonly AddinSettingsGetter _getSettingsNodesFromConfiguration;

		public static AddinSet Create(AddinSettingsGetter getSettingsNodesFromConfiguration,
									  FileLocater locateFile)
		{
			_singleton = new AddinSet(getSettingsNodesFromConfiguration, locateFile);

			return _singleton;
		}

		public static AddinSet Singleton
		{
			get { return _singleton; }
		}

		public FileLocater LocateFile
		{
			get { return _locateFile; }
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
				Load(_getSettingsNodesFromConfiguration());
			}
		}

		public void Load(XPathNodeIterator addinNodes)
		{
			_addinSettings = new Dictionary<string, string>();
			if (addinNodes != null)
			{
				foreach (XPathNavigator node in addinNodes)
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
			writer.WriteAttributeString("id", id);
			// writer.WriteAttributeString("name", name);
			writer.WriteAttributeString("showInWeSay", DoShowInWeSay(id).ToString());

			if (settingsXml != null)
			{
				writer.WriteRaw(settingsXml);
			}
			writer.WriteEndElement();
		}

		public static List<IWeSayAddin> GetAddinsForUser()
		{
			List<IWeSayAddin> addins = new List<IWeSayAddin>();
			try
			{
				List<string> alreadyFound = new List<string>();
				Logger.WriteMinorEvent("Loading Addins");
				if (!AddinManager.IsInitialized)
				{
					//                AddinManager.Initialize(Application.UserAppDataPath);
					//                AddinManager.Registry.Rebuild(null);
					//                AddinManager.Shutdown();
					AddinManager.Initialize(Application.UserAppDataPath);
					AddinManager.Registry.Update(null);
					//these (at least AddinLoaded) does get called after initialize, when you
					//do a search for objects (e.g. GetExtensionObjects())

					//TODO: I added these back on 13 oct because I was seeing no addins!
					AddinManager.Registry.Rebuild(null);
					AddinManager.Shutdown();
					AddinManager.Initialize(Application.UserAppDataPath);
				}

				foreach (IWeSayAddin addin in AddinManager.GetExtensionObjects(typeof (IWeSayAddin))
						)
				{
					if (Singleton.DoShowInWeSay(addin.ID))
					{
						//this alreadyFound business is a hack to prevent duplication in some
						// situation I haven't tracked down yet.
						if (!alreadyFound.Contains(addin.ID))
						{
							alreadyFound.Add(addin.ID);
							addins.Add(addin);
						}
					}
				}
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem(
						"WeSay encountered an error while looking for Addins (e.g., Actions).  The error was: {0}",
						error.Message);
			}
			return addins;
		}
	}
}